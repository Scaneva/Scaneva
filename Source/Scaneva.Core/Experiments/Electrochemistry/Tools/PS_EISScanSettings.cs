#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="EISScanSettings.cs" company="Scaneva">
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
using System.Xml.Serialization;

using PalmSens.Techniques;

namespace Scaneva.Core.Experiments.PalmSens
{
    [XmlInclude(typeof(EISScanSettingsPotentialScan))]
    [XmlInclude(typeof(EISScanSettingsTimeScan))]
    [XmlInclude(typeof(EISScanSettingsFixedPotential))]
    public class PS_EISScanSettings
    {
        public override string ToString()
        {
            return "[ ... ]";
        }
    }

    public class EISScanSettingsPotentialScan : PS_EISScanSettings
    {
        private float beginPotential = 0.0f;
        private float endPotential = 1.0f;
        private float stepPotential = 0.2f;
        private float potentialAmplitude = 0.01f;

        [DisplayName("Begin Potential (V)")]
        [Description("Potential where scan starts. The applicable range of the potential depends on the device connected. Reflects start potential if CV.\r\n Resolution is 1 mV")]
        public float BeginPotential { get => beginPotential; set => beginPotential = value; }

        [DisplayName("End Potential (V)")]
        [Description("Potential in V where measurement stops (Except for CV).\r\nResolution is 1 mV.")]
        public float EndPotential { get => endPotential; set => endPotential = value; }

        [DisplayName("Step Potential (V)")]
        [Description("Step potential in V.\r\nResolution is 1 mV.")]
        public float StepPotential { get => stepPotential; set => stepPotential = value; }

        [DisplayName("AC Potential (V)")]
        [Description("(RMS) Potential amplitude in Volt. Applicable range is 0.001V to 0.25V.")]
        public float PotentialAmplitude { get => potentialAmplitude; set => potentialAmplitude = value; }
    }

    public class EISScanSettingsTimeScan : PS_EISScanSettings
    {
        private float potential = 0.0f;
        private float run = 10.0f;
        private float interval = 0.1f;
        private float potentialAmplitude = 0.1f;

        [DisplayName("DC Potential (V)")]
        [Description("Potential applied during measurement. Applicable range is -2V to +2V.")]
        public float Potential { get => potential; set => potential = value; }

        [DisplayName("Run Time (s)")]
        [Description("Run time.")]
        public float RunTime { get => run; set => run = value; }

        [DisplayName("Interval Time (s)")]
        [Description("Interval time.")]
        public float IntervalTime { get => interval; set => interval = value; }

        [DisplayName("AC Potential (V)")]
        [Description("(RMS) Potential amplitude in Volt. Applicable range is 0.001V to 0.25V.")]
        public float PotentialAmplitude { get => potentialAmplitude; set => potentialAmplitude = value; }
    }

    public class EISScanSettingsFixedPotential : PS_EISScanSettings
    {
        private float potential = 0.0f;
        private float potentialAmplitude = 0.1f;

        [DisplayName("DC Potential (V)")]
        [Description("Potential applied during measurement. Applicable range is -2V to +2V.")]
        public float Potential { get => potential; set => potential = value; }

        [DisplayName("AC Potential (V)")]
        [Description("(RMS) Potential amplitude in Volt. Applicable range is 0.001V to 0.25V.")]
        public float PotentialAmplitude { get => potentialAmplitude; set => potentialAmplitude = value; }
    }
}
