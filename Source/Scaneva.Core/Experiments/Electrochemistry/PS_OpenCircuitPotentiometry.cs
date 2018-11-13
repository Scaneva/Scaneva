#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="OpenCircuitPotentiometryExperiment.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
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
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Scaneva.Core.Hardware;
using Scaneva.Core.ExperimentData;
using Scaneva.Tools;

using PalmSens;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.Plottables;
using PalmSens.Techniques;
using PalmSens.Windows;
using PalmSens.Windows.Devices;
using PalmSens.Data;

namespace Scaneva.Core.Experiments.PalmSens
{
    [DisplayName("Open Circuit Potentiometry")]
    [Category("PalmSens")]
    public class PS_OpenCircuitPotentiometry : ExperimentBase, IExperiment
    {
        public PS_OpenCircuitPotentiometry(LogHelper log)
            : base(log)
        {
            settings = new PS_OpenCircuitPotentiometrySettings();
        }

        public PS_OpenCircuitPotentiometrySettings Settings
        {
            get
            {
                return (PS_OpenCircuitPotentiometrySettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        private Generic2DExperimentData experimentData;

        public override Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                var hwList = value.Where(x => x.Value.GetType().Equals(typeof(PS_PalmSens))).Select(x => x.Key).ToArray();
                Settings.ListofPalmSensHW = hwList;

                hwStore = value;

                // Get List of Current Ranges (if Device is available)
                refreshCurrentRanges();
            }
        }

        private OpenCircuitPotentiometry exp = new OpenCircuitPotentiometry();
        private PS_PalmSens hw;

        private void refreshCurrentRanges()
        {
            if ((Settings.HwName != null) && (hwStore.ContainsKey(Settings.HwName)))
            {
                var tempHW = (PS_PalmSens)HWStore[Settings.HwName];
                Settings.BiPotSettings.ListofCurrentRanges = tempHW.Settings.ListofCurrentRanges;
                Settings.AutoRangingSettings.ListofCurrentRanges = tempHW.Settings.ListofCurrentRanges;
            }
        }

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            // get Reference to HW from Store
            if (!HWStore.ContainsKey(Settings.HwName))
            {
                log.Error("No HW with Name: " + Settings.HwName);
                return enExperimentStatus.Error;
            }
            hw = (PS_PalmSens)HWStore[Settings.HwName];

            Settings.AutoRangingSettings.ConfigureMethod(exp, hw);

            exp.ConditioningTime = Settings.ConditioningTime;
            exp.ConditioningPotential = Settings.ConditioningPotential;
            exp.DepositionTime = Settings.DepositionTime;
            exp.DepositionPotential = Settings.DepositionPotential;

            exp.IntervalTime = Settings.IntervalTime;
            exp.RunTime = Settings.RunTime;

            // Configure Aux/ BiPot Settings
            Settings.BiPotSettings.ConfigureMethod(exp, hw);

            // Setup Results File
            ResultsFilePath = resultsFilePath;
            string cords = "";
            ExperimentContainer container = parent as ExperimentContainer;

            if (container != null)
            {
                cords = container.ChildIndexer();
                if (cords != "")
                {
                    cords = " " + cords;
                }
            }

            ResultsFileName = "OpenCircuitPotentiometryExperiment - " + Name + cords + ".dat";

            string headerString = "Experiment: OpenCircuitPotentiometryExperiment - " + Name + cords + "\r\n";

            if (Settings.BiPotSettings.RecordExtraValue == ExtraValueMask.None)
            {
                // 2 columns
                writeHeader(headerString, new string[] { "Time [s]", "Potential [V]" }, settingsObj: Settings, positionColumns: false);
            }
            else
            {
                // 4 columns
                writeHeader(headerString, new string[] { "Time [s]", "Potential [V]", "Time [s]", "WE2 Potential [V]" }, settingsObj: Settings, positionColumns: false);
            }


            status = enExperimentStatus.Idle;
            return status;
        }

        private List<Curve> resultCurves = null;

        public override enExperimentStatus Run()
        {
            experimentData = new Generic2DExperimentData();
            experimentData.experimentName = "Open circuit potentiometry";
            //experimentData.axisNames = new string[] {"Potential", "Current", "WE2 Potential", "WE2 Current" };
            //experimentData.axisUnits = new string[] {"V", "A", "V", "A" };

            //Add events
            hw.BeginReceiveCurve += HW_BeginReceiveCurve;
            hw.EndMeasurement += HW_EndMeasurement;

            resultCurves = new List<Curve>();

            string errors = hw.Measure(exp);

            if (!String.IsNullOrEmpty(errors))
            {
                log.Add("PS_PalmSens - Error: " + errors);
                return enExperimentStatus.Error;
            }

            status = enExperimentStatus.Running;
            return status;
        }

        private void HW_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            log.Add("New curve received");

            Curve curve = e.GetCurve();
            resultCurves.Add(curve);

            double[] xVals = curve.GetXValues();
            double[] yVals = curve.GetYValues();
            bool bUpdate = false;

            if (curve.YAxisDataArray.ArrayType != (int)DataArrayType.ExtraValue) //Check if curve contains Bipot (WE2) data
            {
                experimentData.data.Add(new double[2][] { xVals, yVals });

                experimentData.axisNames.Add(new string[] { "Time", "Potential" });
                experimentData.axisUnits.Add(new string[] { curve.XUnit.ToString(), curve.YUnit.ToString() });
                experimentData.datasetNames.Add("WE");
            }
            else
            {
                experimentData.data.Add(new double[2][] { xVals, yVals });

                experimentData.axisNames.Add(new string[] { "Time", "WE2 Potential" });
                experimentData.axisUnits.Add(new string[] { curve.XUnit.ToString(), curve.YUnit.ToString() });
                experimentData.datasetNames.Add(Settings.BiPotSettings.RecordExtraValue.ToString());

                bUpdate = true;
            }

            e.GetCurve().NewDataAdded -= HW_NewDataAdded;
            e.GetCurve().NewDataAdded += HW_NewDataAdded;

            NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(experimentData, bUpdate));
        }

        private void HW_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            var curve = sender as Curve;
            int curveIdx = resultCurves.IndexOf(curve);

            double[] xVals = curve.GetXValues();
            double[] yVals = curve.GetYValues();

            experimentData.data[curveIdx][0] = xVals;
            experimentData.data[curveIdx][1] = yVals;

            NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(experimentData, true));
        }

        private void HW_EndMeasurement(object sender, EventArgs e)
        {
            status = enExperimentStatus.Completed;
            log.Add("Measurement Ended");

            //Add events
            hw.BeginReceiveCurve -= HW_BeginReceiveCurve;
            hw.EndMeasurement -= HW_EndMeasurement;

            // save data
            if (exp.ExtraValueMsk == ExtraValueMask.None)
            {
                // 2 columns
                double[] dataX = resultCurves[0].GetXValues();
                double[] dataY = resultCurves[0].GetYValues();

                for (int i = 0; i < dataX.Length; i++)
                {
                    appendResultsValues(new double[] { dataX[i], dataY[i] }, positionColumns: false);
                }
            }
            else
            {
                // 4 columns
                double[] dataX = resultCurves[0].GetXValues();
                double[] dataY = resultCurves[0].GetYValues();
                double[] dataX2 = resultCurves[1].GetXValues();
                double[] dataY2 = resultCurves[1].GetYValues();

                for (int i = 0; i < dataX.Length; i++)
                {
                    appendResultsValues(new double[] { dataX[i], dataY[i], dataX2[i], dataY2[i] }, positionColumns: false);
                }
            }

            NotifyExperimentEndedNow();
        }

        public override enExperimentStatus Abort()
        {
            if (status == enExperimentStatus.Running)
            {
                hw.AbortMeasurement();
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Aborted, null));
                status = enExperimentStatus.Aborted;
                return status;
            }
            return enExperimentStatus.Error;
        }

        public override void ParameterChanged(string name)
        {
            base.ParameterChanged(name);

            switch (name)
            {
                case "Settings Loaded":
                    refreshCurrentRanges();
                    break;

                case "Open Circuit Potentiometry.PalmSens HW Name":
                    refreshCurrentRanges();
                    break;

                default:
                    break;
            }
        }

    }
}
