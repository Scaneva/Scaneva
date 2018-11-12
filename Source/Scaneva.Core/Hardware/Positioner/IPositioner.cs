#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="IPositioner.cs" company="Scaneva">
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
//  Inspiration from the following sources: 
//  http://www.ruhr-uni-bochum.de/elan/
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;

namespace Scaneva.Core
{
    public enum enuAxes
    {
        XAxis = 0,
        YAxis = 1,
        ZAxis = 2,
        AAxis = 3,
    }

    [Flags]
    public enum enuPositionerStatus
    {
        NotInitialized = 0,
        Ready = 1,
        Busy = 2,
        UpperLimit = 4,
        LowerLimit = 8,
        Error = 16,
    }

    public interface IPositioner
    {
        enuPositionerStatus GetPositionerStatus { get; }

        //functions for single axes.
        enuPositionerStatus GetAxisStatus(enuAxes _axis);

        enuPositionerStatus GetAxisSpeed(enuAxes _axis, ref double _speed);
        enuPositionerStatus ValidateAxisSpeed(enuAxes _axis, ref double _speed);
        enuPositionerStatus SetAxisSpeed(enuAxes _axis, double _speed);

        enuPositionerStatus ValidateAxisRelativeMovement(enuAxes _axis, ref double _pos);
        enuPositionerStatus SetAxisRelativePosition(enuAxes _axis, double _increment);

        enuPositionerStatus GetAxisAbsolutePosition(enuAxes _axis, ref double _pos);
        enuPositionerStatus ValidateAxisAbsolutePosition(enuAxes _axis, ref double _pos);
        enuPositionerStatus SetAxisAbsolutePosition(enuAxes _axis, double _position);

        enuPositionerStatus AxisStop(enuAxes _axis);

        //function and properties for all axises.

        enuPositionerStatus GetSpeeds(ref Position _speeds);
        enuPositionerStatus ValidateSpeeds(ref Position _speeds);
        enuPositionerStatus SetSpeeds(Position _speed);

        enuPositionerStatus ValidateRelativeMovement(ref Position _pos);
        enuPositionerStatus SetRelativePosition(Position _pos);

        enuPositionerStatus GetAbsolutePosition(ref Position _pos);
        enuPositionerStatus ValidateAbsolutePosition(ref Position _pos);
        enuPositionerStatus SetAbsolutePosition(Position _pos);

        enuPositionerStatus StopMovement();
    }
}
