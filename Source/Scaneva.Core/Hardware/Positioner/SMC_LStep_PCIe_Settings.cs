#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SMC_LStep_PCIe_Settings.cs" company="Scaneva">
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaneva.Core.Settings
{
    public class SMC_LStep_PCIe_Settings : ISettings
    {
        //COM Port
        private int intCOMPort = 4;
        private String strPath = AppDomain.CurrentDomain.BaseDirectory + @"LStepConfig.LSControl";
        private bool reloadConfiguration = false;
        SMC_Gnrc_Gnrc_Settings_Axis XAxis = new SMC_Gnrc_Gnrc_Settings_Axis();
        SMC_Gnrc_Gnrc_Settings_Axis YAxis = new SMC_Gnrc_Gnrc_Settings_Axis();
        SMC_Gnrc_Gnrc_Settings_Axis ZAxis = new SMC_Gnrc_Gnrc_Settings_Axis();

        [Category("LStep controller settings")]
        [DisplayName("COM port [#]")]
        [Description("COM port used by the controller card as an integer number")]
        public int COMPort { get => intCOMPort; set => intCOMPort = value; }

        [Category("LStep controller settings")]
        [DisplayName("Configuration file")]
        public string Path { get => strPath; set => strPath = value; }

        [Category("LStep controller settings")]
        [DisplayName("Reload configuration")]
        [Description("Activate this option to reload controller configuration and save the parameters into EEPROM")]
        public bool ReloadConfiguration { get => reloadConfiguration; set => reloadConfiguration = value; }

        [Category("Axis parameters")]
        [DisplayNameAttribute("X Axis Settings")]
        public SMC_Gnrc_Gnrc_Settings_Axis X { get => XAxis; set => XAxis = value; }

        [Category("Axis parameters")]
        [DisplayNameAttribute("Y Axis Settings")]
        public SMC_Gnrc_Gnrc_Settings_Axis Y { get => YAxis; set => YAxis = value; }

        [Category("Axis parameters")]
        [DisplayNameAttribute("Z Axis Settings")]
        public SMC_Gnrc_Gnrc_Settings_Axis Z { get => ZAxis; set => ZAxis = value; }
        
    }
}
