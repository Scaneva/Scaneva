#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="IPPump.cs" company="Scaneva">
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

namespace Scaneva.Core
{
    [Flags]
    public enum enuPumpStatus
    {
        NotInitialized = 0,
        Ready = 1,
        Running = 2,
        Direction = 4, //CW if the flag is set, CCW if not
        DeliveryMode = 8,
        Error = 16,
    }

    public interface IPump
    {
        double Counter { get; set; } //setter can only reset the counter, in µl!
        double? Flowrate { get; set; } //ml/min
        double Pressure { get; set; } //kg/cm2
        double Speed { get; set; } //µl/s!
        enuPumpStatus Run { get; set; } //1- Run, 0 - stop
        enuPumpStatus Direction { get; set; } //1 - CW, 0 - CCW
        enuPumpStatus Status();
        Dictionary<int, double?> Composition { get; set; } //channel - value
        int? Valve { get; set; } //starts with 1
    }
}