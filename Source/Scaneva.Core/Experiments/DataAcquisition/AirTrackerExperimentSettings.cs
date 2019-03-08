#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="AirTrackerExperimentSettings.cs" company="Scaneva">
// 
//  Copyright (C) 2019 Roche Diabetes Care GmbH (Christoph Pieper)
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
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Scaneva.Tools;
using Scaneva.Core.Hardware;

namespace Scaneva.Core.Experiments.DataAcquisition
{
    public class AirTrackerExperimentSettings : ISettings
    {
        private Dictionary<string, Generic_SerialConnection> serialConnections = new Dictionary<string, Generic_SerialConnection>();

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, Generic_SerialConnection> SerialConnections
        {
            get => serialConnections;
            set
            {
                serialConnections = value;
            }
        }

        [Category("AirTracker Settings")]
        [DisplayName("Serial connection")]
        [Description("Select AirTracker serial connection")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("SerialConnections")]
        public string SerialConnection { get; set; }

        //[Category("AirTracker Settings")]
        //[DisplayName("Sample Rate (Hz)")]
        //[Description("Desired acquisition sample rate. Sampling will be performed as fast as possible if desired rate can not be achieved.")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0.1, 100)]
        //public double SamplingRate { get; set; } = 1;

        [Category("AirTracker Settings")]
        [DisplayName("Duration (s)")]
        [Description("Duration of AirTracker experiment")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 100000, 0.1, 1)]
        public double Duration { get; set; } = 10;

        [Category("AirTracker  Settings")]
        [DisplayName("Polarization Voltage (mV)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 400)]
        public int PolarizationVoltage { get; set; } = 350;
    }
}
