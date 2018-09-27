#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ImpedanceSpectroscopySettings.cs" company="Scaneva">
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PalmSens.Techniques;

namespace Scaneva.Core.Experiments.PalmSens
{
    public class PS_ImpedanceSpectroscopySettings : DynamicDisplayNames, ISettings
    {
        public PS_ImpedanceSpectroscopySettings()
        {
            SetDisplayName("Scan Settings", "Potential Scan Settings");
            SetDisplayName("Frequency Settings", "Fixed Frequency Settings");
            TypeDescriptor.AddProvider(new CustomTypeDescriptionProvider<PS_ImpedanceSpectroscopySettings>(TypeDescriptor.GetProvider(typeof(PS_ImpedanceSpectroscopySettings))), this);
        }

        private string[] _ListofPalmSensHW;

        [Browsable(false)]
        public string[] ListofPalmSensHW { get => _ListofPalmSensHW; set => _ListofPalmSensHW = value; }

        private string hwName = "";
        private PS_AutoRangingSettings autoRangingSettings = new PS_AutoRangingSettings(1, 8);
        private float equilibrationTime = 5;
        private ImpedimetricMethod.enumScanType scanType = ImpedimetricMethod.enumScanType.PotentialScan;
        private ImpedimetricMethod.enumFrequencyType frequencyType = ImpedimetricMethod.enumFrequencyType.Fixed;
        private PS_EISScanSettings scanSettings = new EISScanSettingsPotentialScan();
        private PS_EISFrequencySettings frequencySettings = new EISFrequencySettingsFixed();
        private PS_EISSettingsPalmSens3 advancedSettings = new EISSettingsPalmSens4();

        [Category("Impedance Spectroscopy")]
        [DisplayName("PalmSens HW Name")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofPalmSensHW")]
        public string HwName
        {
            get => hwName;
            set
            {
                if ((value != null) && (value.Contains("PalmSens3")))
                {
                    AdvancedSettings = new EISSettingsPalmSens3();
                }
                else
                {
                    AdvancedSettings = new EISSettingsPalmSens4();
                }
                hwName = value;
            }
        }

        [Category("Impedance Spectroscopy")]
        [DisplayName("Auto Rangin Settings")]
        [Description("Auto Rangin Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_AutoRangingSettings AutoRangingSettings { get => autoRangingSettings; set => autoRangingSettings = value; }

        [Category("Impedance Spectroscopy")]
        [DisplayName("Equilibration Time (s)")]
        [Description("Equilibration duration in seconds DC Potential or begin Potential is applied during equilibration")]
        public float EquilibrationTime { get => equilibrationTime; set => equilibrationTime = value; }

        [Category("Impedance Spectroscopy")]
        [DisplayName("Scan Type")]
        [Description("Select scan type (PotentialScan, TimeScan or FidexPotential).")]
        public ImpedimetricMethod.enumScanType ScanType
        {
            get => scanType;

            set
            {
                switch (value)
                {
                    case ImpedimetricMethod.enumScanType.PotentialScan:
                        SetDisplayName("Scan Settings", "Potential Scan Settings");
                        if (!ScanSettings.GetType().Equals(typeof(EISScanSettingsPotentialScan)))
                        {
                            ScanSettings = new EISScanSettingsPotentialScan();
                        }
                        break;

                    case ImpedimetricMethod.enumScanType.TimeScan:
                        SetDisplayName("Scan Settings", "Time Scan Settings");
                        if (!ScanSettings.GetType().Equals(typeof(EISScanSettingsTimeScan)))
                        {
                            ScanSettings = new EISScanSettingsTimeScan();
                        }
                        break;

                    case ImpedimetricMethod.enumScanType.FixedPotential:
                        SetDisplayName("Scan Settings", "Fixed Potential Settings");
                        if (!ScanSettings.GetType().Equals(typeof(EISScanSettingsFixedPotential)))
                        {
                            ScanSettings = new EISScanSettingsFixedPotential();
                        }
                        break;

                    default:
                        break;
                }
                scanType = value;
            }
        }

        [Category("Impedance Spectroscopy")]
        [DisplayName("Scan Settings")]
        [Description("Scan Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_EISScanSettings ScanSettings { get => scanSettings; set => scanSettings = value; }

        [Category("Impedance Spectroscopy")]
        [DisplayName("Frequency Type")]
        [Description("Select frequency type (Fixed or Scan).")]
        public ImpedimetricMethod.enumFrequencyType FrequencyType
        {
            get => frequencyType;

            set
            {
                switch (value)
                {
                    case ImpedimetricMethod.enumFrequencyType.Fixed:
                        SetDisplayName("Frequency Settings", "Fixed Frequency Settings");
                        if (!FrequencySettings.GetType().Equals(typeof(EISFrequencySettingsFixed)))
                        {
                            FrequencySettings = new EISFrequencySettingsFixed();
                        }
                        break;

                    case ImpedimetricMethod.enumFrequencyType.Scan:
                        SetDisplayName("Frequency Settings", "Frequency Scan Settings");
                        if (!FrequencySettings.GetType().Equals(typeof(EISFrequencySettingsScan)))
                        {
                            FrequencySettings = new EISFrequencySettingsScan();
                        }
                        break;

                    default:
                        break;
                }

                frequencyType = value;
            }
        }

        [Category("Impedance Spectroscopy")]
        [DisplayName("Frequency Settings")]
        [Description("Frequency Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_EISFrequencySettings FrequencySettings { get => frequencySettings; set => frequencySettings = value; }

        [Category("Impedance Spectroscopy")]
        [DisplayName("Advanced EIS Settings")]
        [Description("Advanced EIS Settings for PalmSens3")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_EISSettingsPalmSens3 AdvancedSettings { get => advancedSettings; set => advancedSettings = value; }
    }
}
