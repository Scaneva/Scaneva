#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpScanArc.cs" company="Scaneva">
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
//  http://dx.doi.org/10.1016/j.electacta.2013.12.041 "New SECM scanning algorithms for improved potentiometric imaging of circularly symmetric targets" by András Kissa and Géza Nagya
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
    public class ScanArc : ExperimentContainer, IExperiment
    {
        private ScannerArc Scanner;
        private TiltCorrection Tilt;

        public ScanArc(LogHelper log)
            : base(log)
        {
            settings = new ScanArcSettings();
        }

        public ScanArcSettings Settings
        {
            get
            {
                return (ScanArcSettings)settings;
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

        private ScanDataFreeform scanData = null;
        private int currentPosIDX;
        private double currentXPos, currentYPos;
       // private int scanPointsX = 0;
        //private int scanPointsY = 0;

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            if (status != enExperimentStatus.Running)
            {
                this.parent = parent;

                ResultsFilePath = Path.Combine(resultsFilePath, Name); ;
                ResultsFileName = "ExpScanArc - " + Name + ".dat";

                IPositioner pos = Settings.Positioners[Settings.Positioner];
                if (Settings.Tilt)
                {
                    pos = Settings.Positioners[Settings.TiltPositioner];
                    Tilt = new TiltCorrection(pos, Settings.Pos1, Settings.Pos2, Settings.Pos3, Settings.Offset);
                    Tilt.PositionStore = PositionStore;
                }

                Position Parameters = new Position(Settings.Radius, Settings.RadiusIncrement, Settings.AngularIncrement);
                //we just create a scanner object and it is dealing with the rest
                Scanner = new ScannerArc( pos, Parameters, Settings.Speeds,
                    Settings.ReverseSpeeds, Settings.PreMovementHook, Settings.PostMovementHook, Tilt, log);
                Scanner.Initialize();

                // create ScanData container
                scanData = new ScanDataFreeform();
                scanData.experimentName = Name;

                // reset Position Counter
                currentPosIDX = 0;

                // Current Positioner pos
                Position currentPos = new Position();
                if (Scanner.Position(ref currentPos) != enuPositionerStatus.Ready) return enExperimentStatus.Error;
                currentXPos = currentPos.X;
                currentYPos = currentPos.Y;

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
        /// Return current Scanner Position relative to scan start
        /// </summary>
        /// <returns></returns>
        public override Position Position()
        {
            Position pos = new Position();
            Scanner.Position(ref pos);
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
                    if ((currentPosIDX == 0) && (!scanData.GetDatasetNames().Contains(dataset)))
                    {
                        scanData.addDataset(dataset, data.axisNames[0] + " (" + data.axisUnits[0] + ")");
                    }
                    // Add first point of 1D Data (Single Value Experiment should only have one)
                    scanData.addValue(dataset, currentXPos, currentYPos, data.Get1DData(i)[0]);
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
                currentPosIDX++;

                // Current Positioner pos
                Position currentPos = new Position();
                Scanner.Position(ref currentPos);
                currentXPos = currentPos.X;
                currentYPos = currentPos.Y;
            }

            if (!abortExperiment)
            {
                Scanner.BackToStart();
            }

            // Signal Experiment end
            if (abortExperiment)
            {               
                // Experiment was aborted
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Aborted, null));
                status = enExperimentStatus.Aborted;
            }
            else if (res.HasFlag(enuScannerErrors.Finished))
            {
                // Experiment ended regularly
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, scanData));
                status = enExperimentStatus.Completed;
            }
            else
            {
                // Something else happend => error
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Error, null));
                status = enExperimentStatus.Error;
            }
        }
    }
}
