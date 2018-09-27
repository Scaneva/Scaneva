#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Pump_Jasco_PU_4180_Settings.cs" company="Scaneva">
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
    public class Pump_Jasco_PU_4180_Setting : ISettings
    {
        [Category("Jasco PU-4180 RS232 settings")]
        [DisplayName("COM port [#]")]
        [Description("COM port used by the pump as an integer number")]
        public int COMPort { get; set; } = 1;
        [Category("Jasco PU-4180 RS232 settings")]
        [DisplayName("Baud rate [Bd]")]
        public int Baudrate { get; set; } = 4800;
        [Category("Jasco PU-4180 RS232 settings")]
        [DisplayName("Parity")]
        public Parity Parity { get; set; } = System.IO.Ports.Parity.None;
        [Category("Jasco PU-4180 RS232 settings")]
        [DisplayName("Stop bits [#]")]
        public StopBits StopBits { get; set; } = System.IO.Ports.StopBits.Two;
        [Category("Jasco PU-4180 RS232 settings")]
        [DisplayName("Handshake")]
        public Handshake Handshake { get; set; } = System.IO.Ports.Handshake.None;
        [Category("Jasco PU-4180 RS232 settings")]
        [DisplayName("Databits [#]")]
        public int Databits { get; set; } = 8;
    }
}
