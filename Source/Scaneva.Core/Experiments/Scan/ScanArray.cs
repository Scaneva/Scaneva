﻿#region Copyright (C)
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
            Settings.ScannerModes.Add("Comb");
            Settings.ScannerModes.Add("Meander");
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
        private bool reverseScanX = false;
        private bool reverseScanY = false;

        public override bool CheckParametersOk(out string errorMessage)
        {
            errorMessage = String.Empty;

            // Check positioner
            if ((Settings.Positioner == null) || (!Settings.Positioners.ContainsKey(Settings.Positioner)))
            {
                errorMessage = "Configuration Error in '" + Name + "': Selected positioner invalid or disabled";
                return false;
            }

            // Check Scan parameters
            if (((Settings.Lengths.X != 0) && (Settings.Speeds.X <= 0)) ||
                ((Settings.Lengths.Y != 0) && (Settings.Speeds.Y <= 0)) ||
                ((Settings.Lengths.Z != 0) && (Settings.Speeds.Z <= 0)))
            {
                errorMessage = "Configuration Error in '" + Name + "': Speeds must be > 0";
                return false;
            }

            if (((Settings.Lengths.X != 0) && (Settings.Increments.X == 0)) ||
                ((Settings.Lengths.Y != 0) && (Settings.Increments.Y == 0)) ||
                ((Settings.Lengths.Z != 0) && (Settings.Increments.Z == 0)))
            {
                errorMessage = "Configuration Error in '" + Name + "': Increments must be != 0";
                return false;
            }

            return true;
        }

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

                Scanner = new ScannerArray(Settings.ScannerMode, pos, Settings.Lengths, Settings.Increments, Settings.Speeds,
                    Settings.ReverseSpeeds, Settings.PreMovementHook, Settings.PostMovementHook, Tilt,
                    Settings.XDelay, Settings.YDelay, Settings.ZDelay,
                    log);
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

                reverseScanX = Settings.Increments.X < 0;
                reverseScanY = Settings.Increments.Y < 0;
                scanData.setScanDimensions(Scanner.NumScanPoints[0], Scanner.NumScanPoints[1]);

                // Current Positioner pos
                Position startPos = new Position();
                if(pos.GetAbsolutePosition(ref startPos) != enuPositionerStatus.Ready) return enExperimentStatus.Error;

                // set scanData dimensions
                scanData.X0 = Math.Min(startPos.X, (Scanner.NumScanPoints[0] - 1) * Settings.Increments.X);
                scanData.Y0 = Math.Min(startPos.Y, (Scanner.NumScanPoints[1] - 1) * Settings.Increments.Y);
                scanData.X1 = Math.Max(startPos.X, (Scanner.NumScanPoints[0] - 1) * Settings.Increments.X);
                scanData.Y1 = Math.Max(startPos.Y, (Scanner.NumScanPoints[1] - 1) * Settings.Increments.Y);
                status = enExperimentStatus.Idle;
                return status;
            }
            return enExperimentStatus.Error;
        }

        private Task scanArrayTask = null;

        /// <summary>
        /// Return relative Postion
        /// </summary>
        /// <returns></returns>
        public override string ChildIndexer()
        {
            if (Scanner != null)
            {
                Position currentPos = new Position();
                Scanner.Position(ref currentPos);
                return currentPos.X + "_" + currentPos.Y + "_" + currentPos.Z;
            }
            else
            {
                return "";
            }
        }

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

        public enuPositionerStatus Position(ref Position _pos)
        {
            return Scanner.Position(ref _pos);
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
                    if ((Scanner.ScanPointIndex[0] == 0) && (Scanner.ScanPointIndex[1] == 0) && (!scanData.GetDatasetNames().Contains(dataset)))
                    {
                        scanData.addDataset(dataset, data.axisNames[0] + " (" + data.axisUnits[0] + ")");
                    }
                    // Add first point of 1D Data (Single Value Experiment should only have one)
                    scanData.setValue(dataset,
                        reverseScanX ? (Scanner.NumScanPoints[0] - Scanner.ScanPointIndex[0] - 1) : Scanner.ScanPointIndex[0],
                        reverseScanY ? (Scanner.NumScanPoints[1] - Scanner.ScanPointIndex[1] - 1) : Scanner.ScanPointIndex[1],
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
            while ((!res.HasFlag(enuScannerErrors.Finished)) && (!abortExperiment))
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

                i++;
            }

            //if (!abortExperiment)
            Scanner.BackToStart();

            // Signal Experiment end
            if (abortExperiment)
            {
                // Experiment was aborted
                status = enExperimentStatus.Aborted;
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Aborted, null));
            }
            else if (res.HasFlag(enuScannerErrors.Finished))
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
