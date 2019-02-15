#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ISyringePump.cs" company="Scaneva">
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

namespace Scaneva.Core
{
    public enum enValveMode
    {
        Clockwise,
        CounterClockwise
    }

    public interface ISyringePump
    {
        enValveMode ValveMode { get; set; }
        string ValvePosition { get; set; } //starts with 1

        double Speed { get; set; } //µl/s!

        double PlungerPosition { get; set; } // set or get Plunger absolute position in microliters
        double RelativePickup { set; } //µl
        double RelativeDispense { set; } //µl

    }
}