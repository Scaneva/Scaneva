#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="FastAmperometryExperiment.cs" company="Scaneva">
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
    [Category("PalmSens")]
    public class PS_FastAmperometry : ExperimentBase, IExperiment
    {
        public PS_FastAmperometry(LogHelper log)
            : base(log)
        {
            settings = new FastAmperometrySettings();
        }

        public FastAmperometrySettings Settings
        {
            get
            {
                return (FastAmperometrySettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        private Generic2DExperimentData experimentData;
        private List<Curve> curves = null;


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

        private FastAmperometry fastAmpo = new FastAmperometry();
        private PS_PalmSens hw;

        private void refreshCurrentRanges()
        {
            if ((Settings.HwName != null) && (hwStore.ContainsKey(Settings.HwName)))
            {
                var tempHW = (PS_PalmSens)HWStore[Settings.HwName];
                Settings.ListofCurrentRanges = tempHW.Settings.ListofCurrentRanges;
            }
        }

        public override bool CheckParametersOk(out string errorMessage)
        {
            errorMessage = String.Empty;

            if ((Settings.HwName == null) || (!HWStore.ContainsKey(Settings.HwName)) || !HWStore[Settings.HwName].IsEnabled)
            {
                errorMessage = "Configuration Error in '" + Name + "': Selected hardware invalid or disabled";
                return false;
            }

            // Try to configure
            hw = (PS_PalmSens)HWStore[Settings.HwName];

            ConfigureFastAmperometryMethod();

            List<MethodError> errorList = fastAmpo.Validate(hw.Capabilities);

            if (errorList.Count > 0)
            {
                errorMessage = "Configuration Error in '" + Name + "':\r\n";
                foreach (MethodError me in errorList)
                {
                    errorMessage += "Parameter " + me.Parameter + ": " + me.Message + "\r\n";
                }

                return false;
            }

            return true;
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

            ConfigureFastAmperometryMethod();

            // Setup Results File
            ResultsFilePath = resultsFilePath;
            string cords = "";
            ExperimentContainer container = parent as ExperimentContainer;
            this.parent = parent;

            if (container != null)
            {
                cords = container.ChildIndexer();
                if (cords != "")
                {
                    cords = " " + cords;
                }
            }

            ResultsFileName = "FastAmperometryExperiment - " + Name + cords + ".dat";

            string headerString = "Experiment: FastAmperometryExperiment - " + Name + cords + "\r\n";

            // 2 columns
            writeHeader(headerString, new string[] { "Time [s]", "Current [µA]" }, settingsObj: Settings, positionColumns: false);

            status = enExperimentStatus.Idle;
            return status;
        }

        private void ConfigureFastAmperometryMethod()
        {
            CurrentRange range = hw.SupportedRanges.First(x => x.ToString() == Settings.FACurrentRange);
            fastAmpo.Ranging = new AutoRanging(range, range, range);  //ranges are -1=100 pA, 0=1nA, 1=10nA, 2=100nA, 3=1uA ... 7=10mA
            fastAmpo.EquilibrationTime = Settings.EquilibrationTime;
            fastAmpo.EqPotentialFA = Settings.EquilibrationPotential;
            fastAmpo.Potential = Settings.Potential;
            fastAmpo.IntervalTime = Settings.IntervalTime;
            fastAmpo.RunTime = Settings.RunTime;

            // No Aux Input for Fast Amperometry
            //// Configure Aux/ BiPot Settings
            //Settings.BiPotSettings.ConfigureMethod(fastAmpo, hw);
        }

        private List<Curve> resultCurves = null;

        public override enExperimentStatus Run()
        {
            curves = new List<Curve>();
            experimentData = new Generic2DExperimentData();
            experimentData.experimentName = "Fast Amperometry";
            //experimentData.axisNames = new string[] {"Potential", "Current", "WE2 Potential", "WE2 Current" };
            //experimentData.axisUnits = new string[] {"V", "A", "V", "A" };

            //Add events
            hw.BeginReceiveCurve += HW_BeginReceiveCurve;
            hw.EndMeasurement += HW_EndMeasurement;

            resultCurves = new List<Curve>();

            string errors = hw.Measure(fastAmpo);

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
            curves.Add(curve);

            if (curve.YAxisDataArray.ArrayType != (int)DataArrayType.ExtraValue) //Check if curve contains Bipot (WE2) data
            {
                experimentData.data.Add(new double[2][] { xVals, yVals });

                experimentData.axisNames.Add(new string[] { "Time", "Current" });
                experimentData.axisUnits.Add(new string[] { curve.XUnit.ToString(), curve.YUnit.ToString() });
                experimentData.datasetNames.Add("WE");
            }
            else
            {
                experimentData.data.Add(new double[2][] { xVals, yVals });

                experimentData.axisNames.Add(new string[] { "Time", "AUX Current" });
                experimentData.axisUnits.Add(new string[] { curve.XUnit.ToString(), curve.YUnit.ToString() });
                experimentData.datasetNames.Add("AUX");

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

            foreach (Curve curve in curves)
            {
                HW_NewDataAdded(curve, null);
            }

            // save data
            // 2 columns
            double[] dataX = resultCurves[0].GetXValues();
            double[] dataY = resultCurves[0].GetYValues();

            for (int i = 0; i < dataX.Length; i++)
            {
                appendResultsValues(new double[] { dataX[i], dataY[i] }, positionColumns: false);
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

                case "Fast Amperometry.PalmSens HW Name":
                    refreshCurrentRanges();
                    break;

                default:
                    break;
            }
        }

    }
}
