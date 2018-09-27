#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="EISFrequencySettings.cs" company="Scaneva">
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
    [XmlInclude(typeof(EISFrequencySettingsFixed))]
    [XmlInclude(typeof(EISFrequencySettingsScan))]
    public class PS_EISFrequencySettings
    {
        public override string ToString()
        {
            return "[ ... ]";
        }
    }

    public class EISFrequencySettingsFixed : PS_EISFrequencySettings
    {
        private float frequency = 1000.0f;

        [DisplayName("Frequency (Hz)")]
        [Description("Fixed Frequency in Hz.")]
        public float Frequency { get => frequency; set => frequency = value; }
    }

    public class EISFrequencySettingsScan : PS_EISFrequencySettings
    {
        private float minFrequency = 100.0f;
        private float maxFrequency = 1e5f;
        private int nFrequencies = 10;

        [DisplayName("Min Frequency (Hz)")]
        [Description("Minimum Frequency in Hz.")]
        public float MinFrequency { get => minFrequency; set => minFrequency = value; }

        [DisplayName("Max Frequency (Hz)")]
        [Description("Maximum Frequency in Hz.")]
        public float MaxFrequency { get => maxFrequency; set => maxFrequency = value; }

        [DisplayName("N Frequencies")]
        [Description("Number of samples/frequencies in scan.")]
        public int NFrequencies { get => nFrequencies; set => nFrequencies = value; }
    }

}
