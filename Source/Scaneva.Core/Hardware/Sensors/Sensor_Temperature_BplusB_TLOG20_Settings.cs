#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Sensor_Temperature_BplusB_TLOG20_Settings.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
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

using System.ComponentModel;
using System.IO.Ports;

namespace Scaneva.Core.Settings
{
    public class Sensor_Temperature_BplusB_TLOG20_Settings : ISettings
    {
        [Category("B+B TLOG20 temperature sensor settings")]
        [DisplayName("COM port [#]")]
        [Description("COM port used by the pump as an integer number")]
        public int COMPort { get; set; } = 1;

        [Category("B+B TLOG20 temperature sensor settings")]
        [DisplayName("Baud rate [Bd]")]
        public int Baudrate { get; set; } = 4800;

        [Category("B+B TLOG20 temperature sensor settings")]
        [DisplayName("Parity")]
        public Parity Parity { get; set; } = System.IO.Ports.Parity.None;

        [Category("B+B TLOG20 temperature sensor settings")]
        [DisplayName("Stop bits [#]")]
        public StopBits StopBits { get; set; } = System.IO.Ports.StopBits.One;

        [Category("B+B TLOG20 temperature sensor settings")]
        [DisplayName("Databits [#]")]
        public int Databits { get; set; } = 8;
    }
}
