#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SetPumpParamSettings.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg)
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

namespace Scaneva.Core.Experiments
{
    public class SetPumpValueSettings : ISettings
    {
        private Dictionary<string, IPump> pumps = new Dictionary<string, IPump>();
        string pump = null;

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, IPump> Pumps { get => pumps; set => pumps = value; }

        [Category("Hardware")]
        [DisplayName("Select pump")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("Pumps")]
        public string Pump { get => pump; set => pump = value; }

        [Category("Composition")]
        [DisplayName("Composition A [%]")]
        [Description("Specify the flow through the channel A. Range: 0.0-100.0% in steps of 0.1%")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100, 0.1)]
        public double? CompositionA { get; set; } = null;

        [Category("Composition")]
        [DisplayName("Composition B [%]")]
        [Description("Specify the flow through the channel B. Range: 0.0-100.0% in steps of 0.1%")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100, 0.1)]
        public double? CompositionB { get; set; } = null;

        [Category("Composition")]
        [DisplayName("Composition C [%]")]
        [Description("Specify the flow through the channel C. Range: 0.0-100.0% in steps of 0.1%")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100, 0.1)]
        public double? CompositionC { get; set; } = null;

        [Category("Composition")]
        [DisplayName("Composition D [%]")]
        [Description("Specify the flow through the channel D. Range: 0.0-100.0% in steps of 0.1%")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100, 0.1)]
        public double? CompositionD { get; set; } = null;

        [Category("Pump parameters")]
        [DisplayName("Valve position")]
        [Description("Specify the valve position [1-10].")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 10, 1)]
        public int? ValvePosition { get; set; } = 1;

        [Category("Pump parameters")]
        [DisplayName("Flowrate [ml/min]")]
        [Description("Specify the overall flow. Range: 0.000 - 10.000 ml/min in steps of 0.001 ml/min")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10, 0.001)]
        public double? Flowrate { get; set; } = null;

        [Category("Pump parameters")]
        [DisplayName("Minimum pressure [kg/cm2]")]
        [Description("Specify the minimum pressure delivered by the pump. Range: 0 - 700 kg/cm2 in steps of 1 kg/cm2")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 700, 1)]
        public double? MinPresure { get; set; } = null;

        [Category("Pump parameters")]
        [DisplayName("Max pressure [kg/cm2]")]
        [Description("Specify the minimum pressure delivered by the pump. Range: 0 - 700 kg/cm2 in steps of 1 kg/cm2")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 700, 1)]
        public double? MaxPresure { get; set; } = null;

        [Category("Pump parameters")]
        [DisplayName("Pump-off timer [h]")]
        [Description("Specify the timer value for pump-off.  Range: 0.0 - 99.9 hours")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 99.9, 0.1)]
        public double? PumpOffTimer { get; set; } = null;

        [Category("Pump parameters")]
        [DisplayName("Pump-on timer [h]")]
        [Description("Specify the timer value for pump-on. Range: 0.0 - 99.9 hours")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 99.9, 0.1)]
        public double? PumpOnTimer { get; set; } = null;
    }
}
