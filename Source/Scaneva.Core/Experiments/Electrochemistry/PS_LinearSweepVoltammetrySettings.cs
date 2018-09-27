#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="LinearSweepVoltammetrySettings.cs" company="Scaneva">
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

using PalmSens;

namespace Scaneva.Core.Experiments.PalmSens
{
    public class PS_LinearSweepVoltammetrySettings : ISettings
    {
        private string[] _ListofPalmSensHW;

        [Browsable(false)]
        public string[] ListofPalmSensHW { get => _ListofPalmSensHW; set => _ListofPalmSensHW = value; }

        private string hwName = "";
        private PS_AutoRangingSettings autoRangingSettings = new PS_AutoRangingSettings();
        private float equilibrationTime = 5;
        private float beginPotential = -0.5f;
        private float endPotential = 0.5f;
        private float scanrate = 0.1f;
        private float stepPotential = 0.01f;
        private PS_BiPotSettings biPotSettings = new PS_BiPotSettings();

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("PalmSens HW Name")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofPalmSensHW")]
        public string HwName { get => hwName; set => hwName = value; }

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("Auto Rangin Settings")]
        [Description("Auto Rangin Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_AutoRangingSettings AutoRangingSettings { get => autoRangingSettings; set => autoRangingSettings = value; }

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("Equilibration Time (s)")]
        [Description("Equilibration duration in seconds BeginPotential is applied during equilibration")]
        public float EquilibrationTime { get => equilibrationTime; set => equilibrationTime = value; }

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("Begin Potential (V)")]
        [Description("Potential where scan starts. The applicable range of the potential depends on the device connected. Reflects start potential if CV.\r\n Resolution is 1 mV")]
        public float BeginPotential { get => beginPotential; set => beginPotential = value; }

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("End Potential (V)")]
        [Description("Potential in V where measurement stops (Except for CV).\r\nResolution is 1 mV.")]
        public float EndPotential { get => endPotential; set => endPotential = value; }

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("Scanrate (V/s)")]
        [Description("The applied scan rate. The applicable range depends on the value of E step.")]
        public float Scanrate { get => scanrate; set => scanrate = value; }

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("Step Potential (V)")]
        [Description("Step potential in V.\r\nResolution is 1 mV.")]
        public float StepPotential { get => stepPotential; set => stepPotential = value; }

        [Category("Linear Sweep Voltammetry")]
        [DisplayName("BiPot Settings")]
        [Description("BiPot Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_BiPotSettings BiPotSettings { get => biPotSettings; set => biPotSettings = value; }
    }
}
