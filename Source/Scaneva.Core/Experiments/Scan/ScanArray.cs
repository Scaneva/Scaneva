#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpScanArray.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see http://www.gnu.org/licenses/.
//  </copyright>
//  <summary>
//  Url: https://github.com/Scaneva
//  </summary>
//  Inspiration from the following sources: 
//  http://www.ruhr-uni-bochum.de/elan/
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Scaneva.Core;
using Scaneva.Core.ExperimentData;
using Scaneva.Core.Settings;
using Scaneva.Tools;


namespace Scaneva.Core.Experiments.ScanEva
{
    [Category("Scan Experiments")]
    public class ScanArray : ExperimentContainer, IExperiment
    {
        private ScannerArray Scanner;
        private TiltCorrection Tilt;

        public ScanArray(LogHelper log)
            : base(log)
        {
            settings = new ScanArraySettings();
        }

        public ScanArraySettings Settings
        {
            get
            {
                return (ScanArraySettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public override Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                var positioners = value.Where(x => typeof(IPositioner).IsAssignableFrom(x.Value.GetType())).Select(x => x);
                Settings.Positioners = positioners.ToDictionary(item => item.Key, item => (IPositioner)item.Value);
                hwStore = value;
            }
        }

        public override enExperimentStatus Abort()
        {
            if ((scanArrayTask != null) && (!scanArrayTask.IsCompleted))
            {
                status = enExperimentStatus.Aborted;
                abortExperiment = true;
                AbortChildExperiments();
                return status;
            }
            return enExperimentStatus.Error;
        }

        private ScanData scanData = null;
        private int currentXPos, currentYPos;
        private int scanPointsX = 0;
        private int scanPointsY = 0;
        private bool reverseScanX = false;
        private bool reverseScanY = false;

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            if (status != enExperimentStatus.Running)
            {
                this.parent = parent;

                ResultsFilePath = Path.Combine(resultsFilePath, Name); ;
                ResultsFileName = "ExpScanArray - " + Name + ".dat";

                IPositioner pos = Settings.Positioners[Settings.Positioner];
                if (Settings.Tilt)
                {
                    pos = Settings.Positioners[Settings.TiltPositioner];
                    Tilt = new TiltCorrection(pos, Settings.Pos1, Settings.Pos2, Settings.Pos3, Settings.Offset);
                    Tilt.PositionStore = PositionStore;
                }

                Scanner = new ScannerArray( pos, Settings.Lengths, Settings.Increments, Settings.Speeds,
                    Settings.ReverseSpeeds, Settings.PreMovementHook, Settings.PostMovementHook, Tilt);
                Scanner.Initialize();

                //safety check: if the tilt correction is in use, a scan in Z direction is not a good idea.
                if ((Math.Abs(Settings.Increments.Z) > 0) && (Math.Abs(Settings.Lengths.Z) > 0) &&
                    (Math.Abs(Settings.Increments.Z) <= Math.Abs(Settings.Lengths.Z)) && (Settings.Tilt))
                {
                    //todo: describe error
                    return enExperimentStatus.Error;
                }

                // create ScanData container
                scanData = new ScanData();
                scanData.experimentName = Name;

                scanPointsX = (int)Math.Floor(Math.Abs(Settings.Lengths.X / Settings.Increments.X)) + 1;
                scanPointsY = (int)Math.Floor(Math.Abs(Settings.Lengths.Y / Settings.Increments.Y)) + 1;
                currentXPos = 0; //todo: consider scan direction and starting point
                currentYPos = 0;
                reverseScanX = Settings.Increments.X < 0;
                reverseScanY = Settings.Increments.Y < 0;
                scanData.setScanDimensions(scanPointsX, scanPointsY);

                // Current Positioner pos
                Position startPos = pos.AbsolutePosition();

                // set scanData dimensions
                scanData.X0 = Math.Min(startPos.X, (scanPointsX - 1) * Settings.Increments.X);
                scanData.Y0 = Math.Min(startPos.Y, (scanPointsY - 1) * Settings.Increments.Y);
                scanData.X1 = Math.Max(startPos.X, (scanPointsX - 1) * Settings.Increments.X);
                scanData.Y1 = Math.Max(startPos.Y, (scanPointsY - 1) * Settings.Increments.Y);
                status = enExperimentStatus.Idle;
                return status;
            }
            return enExperimentStatus.Error;
        }

        private Task scanArrayTask = null;

        public override enExperimentStatus Run()
        {
            if (status == enExperimentStatus.Idle)
            {
                abortExperiment = false;
                scanArrayTask = Task.Factory.StartNew(new Action(RunScan), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                status = enExperimentStatus.Running;
                return status;
            }
            return enExperimentStatus.Error;
        }

        /// <summary>
        /// Default implementation returns no offset for Experiment
        /// override if necessary
        /// </summary>
        /// <returns></returns>
        public override Position Position()
        {
            if (parent != null)
            {
                return parent.Position();
            }
            //kopie liefern, keine Referenz aufs Originalobjekt
            Position pos = new Position();
            pos = Scanner.Position();
            return pos;
        }
        
        protected override void Child_ExperimentEndedHook(object sender, ExperimentEndedEventArgs e)
        {
            // Get Data from single vlaue experiments
            if ((e.Data != null) && (e.Data.GetType().Equals(typeof(Generic1DExperimentData))))
            {
                Generic1DExperimentData data = e.Data as Generic1DExperimentData;

                int i = 0;
                foreach (string dataset in data.datasetNames)
                {
                    // only add datasets once
                    if ((currentXPos == 0) && (currentYPos == 0) && (!scanData.GetDatasetNames().Contains(dataset)))
                    {
                        scanData.addDataset(dataset, data.axisNames[0] + " (" + data.axisUnits[0] + ")");
                    }
                    // Add first point of 1D Data (Single Value Experiment should only have one)
                    scanData.setValue(dataset,
                        reverseScanX ? (scanPointsX - currentXPos - 1) : currentXPos,
                        reverseScanY ? (scanPointsY - currentYPos - 1) : currentYPos,
                        data.Get1DData(i)[0]);
                    i++;
                }
            }
        }

        private void RunScan()
        {
            if (Settings.Tilt)
            {
                Tilt.CompensateTilt(); //check status after movement
                                       //todo: log the new position
            }

            int i = 0;
            enuScannerErrors res = enuScannerErrors.Ready;
            while ((!res.HasFlag( enuScannerErrors.Finished)) && (!abortExperiment))
            {
                // Here we run all child experiments including Autoapproch if contained in the queue at the first (and may be single) point of the scan
                Task childRunner = RunChildExperiments();
                // Wait for completion of ChildExperiments
                childRunner.Wait();

                if (status != enExperimentStatus.Completed)
                {
                    // Child Run did not complete regularly, do Error handling
                    NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Error, null));
                    return;
                }

                // Notify about Data update
                NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(scanData, i > 0));

                // Go to Next Point (if there is one)
                //the scanner takes care of pre- and post-movements
                //but also tilt correction if set
                res = Scanner.NextPosition();

                //todo: get coordinate from the scanner
                i++;
                currentXPos++;
                if (currentXPos >= scanPointsX)
                {
                    currentXPos = 0;
                    currentYPos++;
                    if (currentYPos >= scanPointsY)
                    {
                        currentYPos = 0;
                    }
                }
            }

            if (!abortExperiment)
            {
                Scanner.BackToStart();
            }

            // Signal Experiment end
            if (abortExperiment)
            {
                // Experiment was aborted
                status = enExperimentStatus.Aborted;
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Aborted, null));               
            }
            else if (res == enuScannerErrors.Finished)
            {
                // Experiment ended regularly
                status = enExperimentStatus.Completed;
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, scanData));                
            }
            else
            {
                // Something else happend => error
                status = enExperimentStatus.Error;
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Error, null));                
            }
        }
    }
}
