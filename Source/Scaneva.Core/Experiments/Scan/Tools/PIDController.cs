#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PIDController.cs" company="Scaneva">
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
using Scaneva.Tools;

namespace Scaneva.Core
{
    public class PIDController : ParametrizableObject
    {
        double LastControlOutput;  //last output of the controller
        double DeviationSum = 0;   //sum of deviation from the setpoint since last reset
        double LastDeviation;      //last deviation of the desired setpoint

        public PIDController(LogHelper log) : base(log)
        {
            settings = new PIDControllerSettings();
            // check all parameters and set error flag
        }

        public PIDControllerSettings Settings
        {
            get
            {
                return (PIDControllerSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public double Setpoint { get; set; } = 0;
        public int Error { get; set; } = 0;

        public double SimpleCorrection(double _processVariable)
        {
            double res = 0;

            //deviation of the process variable from the setpoint
            double deviation = (Setpoint - _processVariable); 

            //limit the deviation
            if (Math.Abs(deviation) > Settings.PB)
            {
                deviation = (deviation < 0) ? -Settings.PB : Settings.PB;
                DeviationSum = 0;
            }
            
            res = deviation * Settings.Kp;   //val * µm/val

            //differential component if configured.
            if ((Settings.Kd != null) && (Settings.Kd.Value != 0) && (LastControlOutput != 0))
            {
                res -= (((LastDeviation - deviation) / LastControlOutput) * Settings.Kd.Value);
            }

            //integral component if configured
            if ((Settings.Ki != null) && (Settings.Ki.Value != 0))
            {
                res += (DeviationSum * Settings.Ki.Value);
            }

            //simple Anti-Windup. sum should not exceed MaxDeviationSum or -MaxDeviationSum.
            if ((Settings.MaxDeviationSum > 0) && (Math.Abs(deviation + DeviationSum) < Math.Abs(Settings.MaxDeviationSum)))
            {
                DeviationSum += deviation;
            }
            LastDeviation = deviation;
            LastControlOutput = res;
            return res;
        }

        public double NormalizedCorrection(float _bulksignal, float _currentSignal)
        {
            double res = 0;
            double deviation = ((_currentSignal - _bulksignal) / _bulksignal);

            if (Math.Abs(deviation) < (Setpoint / 100))
            {
                //limit the deviation
                if (Math.Abs(deviation) > Settings.Kp)
                {
                    res = (deviation < 0) ? -Settings.Kp : Settings.Kp;
                    DeviationSum = 0;
                }
                else
                {
                    return deviation;
                }
            }
            else
            {
                //limit the deviation
                if (Math.Abs(deviation) > Settings.Kp)
                {
                    res = (deviation < 0) ? Settings.Kp : -Settings.Kp;
                    DeviationSum = 0;
                }
                else
                {
                    return deviation;
                }
            }

            // proportional component
            res *= Settings.Kp; //val * µm/val

            //differential component if configured.
            if ((Settings.Kd != null) && (Settings.Kd.Value != 0) && (LastControlOutput != 0))
            {
                res -= (((LastDeviation - deviation) / LastControlOutput) * Settings.Kd.Value);
            }

            //integral component if configured
            if ((Settings.Ki != null) && (Settings.Ki.Value != 0))
            {
                res += (DeviationSum * Settings.Ki.Value);
            }

            //simple Anti-Windup. sum should not exceed MaxDeviationSum or -MaxDeviationSum.
            if ((Settings.MaxDeviationSum > 0) && (Math.Abs(deviation + DeviationSum) < Math.Abs(Settings.MaxDeviationSum)))
            {
                DeviationSum += deviation;
            }
            LastDeviation = deviation;
            LastControlOutput = res;
            return res;
        }

        public void Reset()
        {
            DeviationSum = 0;
            LastControlOutput = 0;
            LastDeviation = 0;
        }
    }
}
