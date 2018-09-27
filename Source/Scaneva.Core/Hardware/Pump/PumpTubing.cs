#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PumpTubing.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
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

namespace Scaneva.Core.Hardware
{
    [Serializable]
    public class PumpTubing
    {
        public PumpTubing()
        {
        }

        public PumpTubing(string _name, double _diameter, double _minRPM, double _maxRPM, double _minSpeed, double _maxSpeed)
        {
            Name = _name;
            Diameter = _diameter;
            MinRPM = _minRPM;
            MaxRPM = _maxRPM;
            MinSpeed = _minSpeed;
            MaxSpeed = _maxSpeed;
        }

        public string Name { get; set; }
        public double Diameter { get; set; }
        public double MinRPM { get; set; }
        public double MaxRPM { get; set; }
        public double MinSpeed { get; set; }
        public double MaxSpeed { get; set; }

        public double RPM2Speed(double _rpm)
        {
            if ((MaxRPM > MinRPM) && (MinRPM >= 0) && (_rpm >= 0))
            {
                return (((MaxSpeed - MinSpeed) / (MaxRPM - MinRPM)) * (_rpm - MinRPM) + MinSpeed);
            }
            return 0;
        }

        public double Speed2RPM(double _speed)
        {
            if ((MaxSpeed > MinSpeed) && (MinSpeed >= 0) && (_speed >= 0))
            {
                return (((MaxRPM - MinRPM) / (MaxSpeed - MinSpeed)) * (_speed - MinSpeed) + MinRPM);
            }
            return 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}