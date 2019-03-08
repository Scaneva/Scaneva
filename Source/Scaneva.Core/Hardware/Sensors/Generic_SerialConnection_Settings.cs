#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Generic_SerialConnection_Settings.cs" company="Scaneva">
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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Xml.Serialization;

namespace Scaneva.Core.Settings
{
    public class Generic_SerialConnection_Settings : ISettings
    {
        [Browsable(false)]
        [XmlIgnore]
        public List<string> ComPortList { get; set; } = new List<string>();

        public enum BaudRate : int
        {
            Rate_110 = 110,
            Rate_300 = 300,
            Rate_600 = 600,
            Rate_1200 = 1200,
            Rate_2400 = 2400,
            Rate_4800 = 4800,
            Rate_9600 = 9600,
            Rate_14400 = 14400,
            Rate_19200 = 19200,
            Rate_38400 = 38400,
            Rate_57600 = 57600,
            Rate_115200 = 115200,
            Rate_128000 = 128000,
            Rate_256000 = 256000
        }

        [Category("Serial connection settings")]
        [DisplayName("COM port")]
        [Description("COM port used by the connection")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ComPortList")]
        public string COMPort { get; set; } = "";

        [Category("Serial connection settings")]
        [DisplayName("Baud rate")]
        public BaudRate Baudrate { get; set; } = BaudRate.Rate_4800;

        [Category("Serial connection settings")]
        [DisplayName("Databits")]
        public int Databits { get; set; } = 8;

        [Category("Serial connection settings")]
        [DisplayName("Parity")]
        public Parity Parity { get; set; } = System.IO.Ports.Parity.None;

        [Category("Serial connection settings")]
        [DisplayName("Stop bits")]
        public StopBits StopBits { get; set; } = System.IO.Ports.StopBits.One;

        [Category("Serial connection settings")]
        [DisplayName("Recived command termination")]
        public string Termination { get; set; } = "\\r\\n";

        [Category("Serial connection settings")]
        [DisplayName("Received command timeout")]
        [Description("Receive Timeout (ms)")]
        public int ReceiveTimeout { get; set; } = 250;

    }
}
