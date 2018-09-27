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
        //functions for single axes.
        enuPositionerStatus AxisStatus(enuAxes _axis);
        double ValidateDistance(enuAxes _axis, double _distance);
        double ValidateSpeed(enuAxes _axis, double _speed);
        double Speed(enuAxes _axis);
        enuPositionerStatus Speed(enuAxes _axis, double _speed);
        double AxisAbsolutePosition(enuAxes _axis);
        enuPositionerStatus MoveRelativ(enuAxes _axis, double _increment, double _speed);
        enuPositionerStatus MoveAbsolut(enuAxes _axis, double _position, double _speed);
        enuPositionerStatus AxisStop(enuAxes _axis);

        //function and properties for all axises.
        enuPositionerStatus Status { get; }
        enuPositionerStatus  ValidatePosition(ref Position _pos);
        enuPositionerStatus ValidateSpeeds(ref Position _speed);
        Position Speeds();
        enuPositionerStatus Speeds(Position _speed);
        Position AbsolutePosition();
        enuPositionerStatus AbsolutePosition(Position _pos);
        enuPositionerStatus RelativePosition(Position _pos);
        enuPositionerStatus StopMovement();
    }
}
