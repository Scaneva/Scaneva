#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ImpedanceSpectroscopyExperiment.cs" company="Scaneva">
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
using System.Collections;

namespace Scaneva.Core.Experiments.PalmSens
{
    [Category("PalmSens")]
    public class PS_ImpedanceSpectroscopy : ExperimentBase, IExperiment
    {
        public PS_ImpedanceSpectroscopy(LogHelper log)
            : base(log)
        {
            settings = new PS_ImpedanceSpectroscopySettings();
            //TypeDescriptor.AddProvider(new CustomTypeDescriptionProvider<ImpedanceSpectroscopySettings>(TypeDescriptor.GetProvider(typeof(ImpedanceSpectroscopySettings))), settings);
        }

        public PS_ImpedanceSpectroscopySettings Settings
        {
            get
            {
                return (PS_ImpedanceSpectroscopySettings)settings;
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

        private ImpedimetricMethod imp = new ImpedimetricMethod();
        private PS_PalmSens hw;

        private void refreshCurrentRanges()
        {
            if ((Settings.HwName != null) && (hwStore.ContainsKey(Settings.HwName)))
            {
                var tempHW = (PS_PalmSens)HWStore[Settings.HwName];
                Settings.AutoRangingSettings.ListofCurrentRanges = tempHW.Settings.ListofCurrentRanges;
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

            ConfigureImpedimetricMethod();

            List<MethodError> errorList = imp.Validate(hw.Capabilities);

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

            ConfigureImpedimetricMethod();

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

            ResultsFileName = "ImpedanceSpectroscopyExperiment - " + Name + cords + ".dat";

            string headerString = "Experiment: ImpedanceSpectroscopyExperiment - " + Name + cords + "\r\n";

            // 2 columns
            writeHeader(headerString, new string[] { /*"Time [s]", "Current [µA]"*/ }, settingsObj: Settings, positionColumns: false);

            status = enExperimentStatus.Idle;
            return status;
        }

        private void ConfigureImpedimetricMethod()
        {
            Settings.AutoRangingSettings.ConfigureMethod(imp, hw);

            imp.EquilibrationTime = Settings.EquilibrationTime;
            imp.ScanType = Settings.ScanType;

            // Scan Settings
            switch (Settings.ScanType)
            {
                case ImpedimetricMethod.enumScanType.PotentialScan:
                    var scanSettingsPotScan = Settings.ScanSettings as EISScanSettingsPotentialScan;
                    imp.BeginPotential = scanSettingsPotScan.BeginPotential;
                    imp.EndPotential = scanSettingsPotScan.EndPotential;
                    imp.StepPotential = scanSettingsPotScan.StepPotential;
                    imp.Eac = scanSettingsPotScan.PotentialAmplitude;
                    break;

                case ImpedimetricMethod.enumScanType.TimeScan:
                    var scanSettingsTime = Settings.ScanSettings as EISScanSettingsTimeScan;
                    imp.Potential = scanSettingsTime.Potential;
                    imp.IntervalTime = scanSettingsTime.IntervalTime;
                    imp.RunTime = scanSettingsTime.RunTime;
                    imp.Eac = scanSettingsTime.PotentialAmplitude;
                    break;

                case ImpedimetricMethod.enumScanType.FixedPotential:
                    var scanSettingsFixed = Settings.ScanSettings as EISScanSettingsFixedPotential;
                    imp.Potential = scanSettingsFixed.Potential;
                    imp.Eac = scanSettingsFixed.PotentialAmplitude; //(RMS) potential amplitude: Volt
                    break;

                default:
                    break;
            }

            imp.FreqType = Settings.FrequencyType;

            // Frequency Settings
            switch (Settings.FrequencyType)
            {
                case ImpedimetricMethod.enumFrequencyType.Fixed:
                    var frequencySettingsFixed = Settings.FrequencySettings as EISFrequencySettingsFixed;
                    imp.FixedFrequency = frequencySettingsFixed.Frequency; //Frequency: Hertz
                    break;

                case ImpedimetricMethod.enumFrequencyType.Scan:
                    var frequencySettingsScan = Settings.FrequencySettings as EISFrequencySettingsScan;
                    imp.MinFrequency = frequencySettingsScan.MinFrequency;
                    imp.MaxFrequency = frequencySettingsScan.MaxFrequency;
                    imp.nFrequencies = frequencySettingsScan.NFrequencies; //Amount of samples/frequencies in scan
                    break;

                default:
                    break;
            }

            // Advanced Settings
            if (typeof(EISSettingsPalmSens3).Equals(Settings.AdvancedSettings.GetType()))
            {

            }

            // Set multiplexer settings to measure on channel 2 and 3 (one at a time).
            imp.MuxMethod = MuxMethod.Sequentially; //only sequential measurements are allowed for EIS
            imp.UseMuxChannel = new BitArray(new bool[] { false, true, true });

        }

        private List<EISData> resultEISData = null;

        public override enExperimentStatus Run()
        {
            experimentData = new Generic2DExperimentData();
            experimentData.experimentName = "Impedance Spectroscopy";

            experimentData.axisNames.Add(new string[] { "Z'", "Z''" });
            experimentData.axisNames.Add(new string[] { "Z'", "Z''" });

            experimentData.axisUnits.Add(new string[] { "Ω", "Ω" });
            experimentData.axisUnits.Add(new string[] { "Ω", "Ω" });

            //Add events
            hw.EndMeasurement += HW_EndMeasurement;
            hw.BeginReceiveEISData += HW_BeginReceiveEISData;

            resultEISData = new List<EISData>();

            string errors = hw.Measure(imp);

            if (!String.IsNullOrEmpty(errors))
            {
                log.Add("PS_PalmSens: " + errors, "Error");
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Error, null));
                return enExperimentStatus.Error;
            }

            status = enExperimentStatus.Running;
            return status;
        }

        private void HW_BeginReceiveEISData(object sender, EISData eisdata)
        {
            eisdata.NewDataAdded -= Eisdata_NewDataAdded;
            eisdata.NewDataAdded += Eisdata_NewDataAdded;

            resultEISData.Add(eisdata);

            //experimentData.data.Add(new double[2][] { xVals, yVals });

            experimentData.data.Add(new double[2][] { new double[] { }, new double[] { } });
            experimentData.datasetNames.Add(eisdata.Title);

            NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(experimentData, false));
        }

        private void Eisdata_NewDataAdded(object sender, EISData.NewDataEventArgs e)
        {
            //throw new NotImplementedException();
            DataArray[] arrays = e.EISData.EISDataSet.GetDataArrays();

            DataArray z1data = arrays.Where(x => (x.Description == "ZRe")).First();
            DataArray z2data = arrays.Where(x => (x.Description == "ZIm")).First();

            experimentData.data.Last()[0] = z1data.Select(x => x.Value).ToArray();
            experimentData.data.Last()[1] = z2data.Select(x => x.Value).ToArray();

            NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(experimentData, true));
        }

        private void HW_EndMeasurement(object sender, EventArgs e)
        {
            status = enExperimentStatus.Completed;
            log.Add("Measurement Ended");

            //Add events
            hw.BeginReceiveEISData -= HW_BeginReceiveEISData;
            hw.EndMeasurement -= HW_EndMeasurement;

            foreach(EISData data in resultEISData)
            {
                appendCommentLines(data.Title);

                DataArray[] arrays = data.EISDataSet.GetDataArrays();

                string headerStr = "";
                foreach (DataArray array in arrays)
                {
                    if (headerStr.Length > 0)
                    {
                        headerStr += ", ";
                    }
                    headerStr += array.Description + " [" + array.Unit.ToString() + "]";
                }
                headerStr += "\r\n";
                appendCommentLines(headerStr);

                for (int i = 0; i < arrays[0].GetValues().Length; i++)
                {
                    double[] values = new double[arrays.Count()];
                    for (int j = 0; j < arrays.Count(); j++)
                    {
                        values[j] = arrays[j][i].Value;
                    }

                    appendResultsValues(values, false);
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

                case "Impedance Spectroscopy.PalmSens HW Name":
                    refreshCurrentRanges();
                    break;

                default:
                    break;
            }
        }

    }
}
