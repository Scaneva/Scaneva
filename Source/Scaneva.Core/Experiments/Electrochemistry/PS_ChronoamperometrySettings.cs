#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ChronoamperometrySettings.cs" company="Scaneva">
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

namespace Scaneva.Core.Experiments.PalmSens
{
    public class PS_ChronoamperometrySettings : ISettings
    {
        private string[] _ListofPalmSensHW;

        [Browsable(false)]
        public string[] ListofPalmSensHW { get => _ListofPalmSensHW; set => _ListofPalmSensHW = value; }

        private string hwName = "";
        private PS_AutoRangingSettings autoRangingSettings = new PS_AutoRangingSettings();
        private float equilibrationTime = 5;
        private float potential = 0.5f;
        private float interval = 0.1f;
        private float run = 10.0f;
        private PS_BiPotSettings biPotSettings = new PS_BiPotSettings();

        [Category("Chronoamperometry")]
        [DisplayName("PalmSens HW Name")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofPalmSensHW")]
        public string HwName { get => hwName; set => hwName = value; }

        [Category("Chronoamperometry")]
        [DisplayName("Auto Rangin Settings")]
        [Description("Auto Rangin Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_AutoRangingSettings AutoRangingSettings { get => autoRangingSettings; set => autoRangingSettings = value; }

        [Category("Chronoamperometry")]
        [DisplayName("Equilibration Time (s)")]
        [Description("Equilibration duration in seconds DC Potential is applied during equilibration")]
        public float EquilibrationTime { get => equilibrationTime; set => equilibrationTime = value; }

        [Category("Chronoamperometry")]
        [DisplayName("DC Potential (V)")]
        [Description("Potential applied during measurement. Applicable range is -2V to +2V.")]
        public float Potential { get => potential; set => potential = value; }

        [Category("Chronoamperometry")]
        [DisplayName("Interval Time (s)")]
        [Description("Interval time.")]
        public float IntervalTime { get => interval; set => interval = value; }

        [Category("Chronoamperometry")]
        [DisplayName("Run Time (s)")]
        [Description("Run time.")]
        public float RunTime { get => run; set => run = value; }

        [Category("Chronoamperometry")]
        [DisplayName("BiPot Settings")]
        [Description("BiPot Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_BiPotSettings BiPotSettings { get => biPotSettings; set => biPotSettings = value; }

    }
}
