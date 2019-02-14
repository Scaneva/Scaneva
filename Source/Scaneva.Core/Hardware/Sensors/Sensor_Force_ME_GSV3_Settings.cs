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
    public class Sensor_Force_ME_GSV3_Settings : ISettings
    {
        [Category("GSV-3 force sensor settings")]
        [DisplayName("COM port [#]")]
        [Description("COM port used by the pump as an integer number")]
        public int COMPort { get; set; } = 1;

        [Category("GSV-3 force sensor settings")]
        [DisplayName("Baud rate [Bd]")]
        public int Baudrate { get; set; } = 4800;

        [Category("GSV-3 force sensor settings")]
        [DisplayName("Parity")]
        public Parity Parity { get; set; } = System.IO.Ports.Parity.None;

        [Category("GSV-3 force sensor settings")]
        [DisplayName("Stop bits [#]")]
        public StopBits StopBits { get; set; } = System.IO.Ports.StopBits.One;

        [Category("GSV-3 force sensor settings")]
        [DisplayName("Databits [#]")]
        public int Databits { get; set; } = 8;

        [Category("GSV-3 force sensor settings")]
        [DisplayName("Buffer size [#]")]
        [Description("Enter buffersize (this number of values will be averaged in the program!")]
        public int Buffersize { get; set; } = 8;

/*
GSVsetBaud Seite 231
GSVsetSlowRate Seite 235
GSVsetSpecialMode Seite 239
GSVsetSpecialModeSlow Seite 285
GSVsetSpecialModeSleep Seite 309
GSVsetSpecialModeFilter Seite 293
GSVsetSpecialModeMax Seite 297
GSVsetSpecialModeFilterAuto Seite 301
GSVsetSpecialModeFilterOrder5 Seite 305
GSVwriteSamplingRate Seite 243
GSVsetBaudRate Seite 325
GSVgetBaud Seite 233
GSVgetSlowRate Seite 237
GSVgetSpecialMode Seite 241
GSVgetSpecialModeSlow Seite 287
GSVgetSpecialModeSleep Seite 311
GSVgetSpecialModeAverage Seite 291
GSVgetSpecialModeFilter Seite 295
GSVgetSpecialModeMax Seite 299
GSVgetSpecialModeFilterAuto Seite 303
GSVgetSpecialModeFilterOrder5 Seite 307
GSVreadSamplingRate Seite 245
GSVreadSamplingFrequency Seite 321
GSVreadSamplingFactor Seite 323
GSVgetBaudRate Seite 327
*/

    }
}
