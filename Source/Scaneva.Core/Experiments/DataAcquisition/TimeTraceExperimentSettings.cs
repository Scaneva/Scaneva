#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="TimeTraceExperimentSettings.cs" company="Scaneva">
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
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Scaneva.Tools;

namespace Scaneva.Core.Experiments.ScanEva
{
    public class TimeTraceExperimentSettings : ISettings
    {
        [Category("Time Trace Settings")]
        [DisplayName("Sample Rate (Hz)")]
        [Description("Desired acquisition sample rate. Sampling will be performed as fast as possible if desired rate can not be achieved.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0.1, 100)]
        public double SamplingRate { get; set; } = 1;

        [Category("Time Trace Settings")]
        [DisplayName("Duration (s)")]
        [Description("Duration of Time Trace experiment")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 100000, 0.1, 1)]
        public double Duration { get; set; } = 10;

        [Category("Time Trace  Settings")]
        [DisplayName("Input Channels")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GetSingleValueSettings InputChannelSettings { get; set; } = new GetSingleValueSettings();
    }

}
