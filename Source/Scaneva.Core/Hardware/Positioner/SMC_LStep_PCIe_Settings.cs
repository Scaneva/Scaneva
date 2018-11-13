#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SMC_LStep_PCIe_Settings.cs" company="Scaneva">
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
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaneva.Core.Settings
{
    public class SMC_LStep_PCIe_Settings : ISettings
    {
        [Category("1. Controller settings")]
        [DisplayName("COM port [#]")]
        [Description("COM port used by the controller card as an integer number")]
        public int COMPort { get; set; } = 4;

        [Category("1. Controller settings")]
        [DisplayName("Configuration file")]
        public string Path { get; set; } = AppDomain.CurrentDomain.BaseDirectory + @"LStepConfig.LSControl";

        [Category("1. Controller settings")]
        [DisplayName("Read configuration from file")]
        [Description("Activate this option to read controller configuration from profile and save them in EEPROM.")]
        public bool ReadConfigurationF { get; set; } = false;

        [Category("1. Controller settings")]
        [DisplayName("Save configuration to file")]
        [Description("Activate this option to reload controller configuration and save the parameters into EEPROM and profile.")]
        public bool SaveConfigurationF { get; set; } = false;

        [Category("1. Controller settings")]
        [DisplayName("Save configuration to EEPROM")]
        [Description("Activate this option to reload controller configuration and save the parameters into EEPROM and profile.")]
        public bool SaveConfigurationE { get; set; } = false;

        [Category("1. Controller settings")]
        [DisplayName("Automatically validate parameters")]
        [Description("Activate this option to automatically validate speeds and distances.")]
        public bool AutoValidateParam{ get; set; } = false;

        [Category("2. Axis settings")]
        [DisplayNameAttribute("X Axis Settings")]
        public SMC_LStep_Axis_Settings X { get; set; } = new SMC_LStep_Axis_Settings();

        [Category("2. Axis settings")]
        [DisplayNameAttribute("Y Axis Settings")]
        public SMC_LStep_Axis_Settings Y { get; set; } = new SMC_LStep_Axis_Settings();

        [Category("2. Axis settings")]
        [DisplayNameAttribute("Z Axis Settings")]
        public SMC_LStep_Axis_Settings Z { get; set; } = new SMC_LStep_Axis_Settings();

        [Category("2. Axis settings")]
        [DisplayNameAttribute("A Axis Settings")]
        public SMC_LStep_Axis_Settings A { get; set; } = new SMC_LStep_Axis_Settings();
    }
}
