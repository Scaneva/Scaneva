#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Pump_Tecan_Centris_Settings.cs" company="Scaneva">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using PumpCommServerTypeLib;

using Scaneva.Tools;

namespace Scaneva.Core.Hardware.Pump
{
    public class Pump_Hamilton_PSD_Settings : ISettings
    {
        public enum ValveType : int
        {
            [Description("No valve")]
            NoValve = 0,

            [Description("3-way valve (Y-valve) (default)")]
            Valve_3way = 1,

            [Description("4-way valve")]
            Valve_4way = 2,

            [Description("6-port distribution valve")]
            Valve_6port = 7,

            [Description("9-port distribution valve")]
            Valve_9port = 8,

            [Description("4-port dual-loop valve")]
            Valve_4port_dl = 9,

            [Description("3-port distribution valve")]
            Valve_3port = 11,

            [Description("12-port distribution valve")]
            Valve_12port = 12
        }

        public enum MicrostepMode : int
        {
            [Description("Normal mode")]
            Normal = 0,

            [Description("Fine positioning mode")]
            Fine = 1,

            [Description("Micro-step mode")]
            Microstep = 2,

        }

        public enum SyringeVolume : int
        {
            [Description("250 µL")]
            Volume_250uL = 90,

            [Description("1250 µL (default)")]
            Volume_1250uL = 91, // Default

            [Description("5000 µL")]
            Volume_5000uL = 92,

            [Description("50 µL")]
            Volume_50uL = 93,

            [Description("100 µL")]
            Volume_100uL = 94,

            [Description("500 µL")]
            Volume_500uL = 95,

            [Description("1000 µL")]
            Volume_1000uL = 96,

            [Description("2500 µL")]
            Volume_2500uL = 97,

            [Description("12500 µL")]
            Volume_12500uL = 98
        }

        [Browsable(false)]
        [XmlIgnore]
        public List<string> ComPortList { get; set; } = new List<string>();

        [Category("1. Communication settings")]
        [DisplayName("COM port")]
        [Description("COM port used by the pump")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ComPortList")]
        public string COMPort { get; set; } = "COM1";

        [Category("1. Communication settings")]
        [DisplayName("Baud rate")]
        public EBaudRate Baudrate { get; set; } = EBaudRate.Baud9600;

        [Category("1. Communication settings")]
        [DisplayName("Pump Adress")]
        [Description("Address of pump as selected by switch on the back panel of the pump. Up to sixteen (16) Centris pumps can be connected together in a multi-pump configuration.")]
        public byte PumpAdress { get; set; } = 0;

        [Category("2. Valve settings")]
        [DisplayName("Valve type")]
        public ValveType Valve { get; set; } = ValveType.Valve_3way;

        [Category("2. Valve settings")]
        [DisplayName("Valve polarity")]
        [Description("Valve initialization direction.\r\nClockwise: Homes the valve in a clockwise direction. Valve ports are numbered 1–X, starting in a clockwise direction at the first port after the syringe port.\r\nCounter-Clockwise: Homes the valve in a counter - clockwise direction. Valve ports are numbered 1 - X in a counterclockwise direction starting with the first port after the syringe port.")]
        public enValveMode InitializationValvePolarity { get; set; } = enValveMode.Clockwise;

        [Category("2. Valve settings")]
        [DisplayName("Input port")]
        [Description("Sets initialization input port 1-x : Sets initialization input port for distribution valves, where X is the number of ports on the valve. 0 : Sets initialization input port to port 1 (default).")]
        public byte InitializationInputPort { get; set; } = 0;

        [Category("2. Valve settings")]
        [DisplayName("Output port")]
        [Description("Sets initialization output port 1-x : Sets initialization output port for distribution valves, where X is the number of ports on the valve. 0 : Sets initialization output port to port X (default).")]
        public byte InitializationOutputPort { get; set; } = 0;

        [Category("3. Syringe settings")]
        [DisplayName("Initialization speed")]
        [Description("Initializes at the speed code (4-25). See the [S] command for list of speed codes (default 7).")]
        public byte InitializationSpeed { get; set; } = 7;

        [Category("3. Syringe settings")]
        [DisplayName("Syringe volume")]
        public SyringeVolume Volume { get; set; } = SyringeVolume.Volume_1250uL;

        [Category("3. Syringe settings")]
        [DisplayName("Microstepping")]
        public MicrostepMode Mode { get; set; } = MicrostepMode.Normal;

    }
}
