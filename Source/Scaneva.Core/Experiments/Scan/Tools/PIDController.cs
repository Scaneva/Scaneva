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
        double mSetpoint = 0;
        double mLastControlOutput; //last output of the controller
        double mDeviationSum = 0;//sum of deviation from the setpoint since last reset
        double mLastDeviation;//last deviation of the desired setpoint (Regeldifferenz)
        int error = 0; 
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

        public double Setpoint { get => mSetpoint; set => mSetpoint = value; }
        public int Error { get => error; set => error = value; }

        public double SimpleCorrection(double _processVariable)
        {
            double res = 0;
            double deviation = 0;

            //deviation of the process variable from the setpoint
            deviation = (Setpoint - _processVariable); 

            //limit the deviation
            if (Math.Abs(deviation) > Settings.P)
            {
                deviation = (deviation < 0) ? -Settings.P : Settings.P;
                mDeviationSum = 0;
            }
            
            res = deviation;   //val * µm/val

            //differential component if configured.
            if ((Settings.D != null) && (Settings.D.Value != 0) && (mLastControlOutput != 0))
            {
                res -= (((mLastDeviation - deviation) / mLastControlOutput) * Settings.D.Value);
            }

            //integral component if configured
            if ((Settings.I != null) && (Settings.I.Value != 0))
            {
                res += (mDeviationSum * Settings.I.Value);
            }

            //simple Anti-Windup. sum should not exceed MaxDeviationSum or -MaxDeviationSum.
            if ((Settings.MaxDeviationSum > 0) && (Math.Abs(deviation + mDeviationSum) < Math.Abs(Settings.MaxDeviationSum)))
            {
                mDeviationSum += deviation;
            }
            mLastDeviation = deviation;
            mLastControlOutput = res;
            return res;
        }

        public double NormalizedCorrection(float _bulksignal, float _currentSignal)
        {
            double res = 0;
            double deviation = ((_currentSignal - _bulksignal) / _bulksignal);

            if (Math.Abs(deviation) < (Setpoint / 100))
            {
                //limit the deviation
                if (Math.Abs(deviation) > Settings.P)
                {
                    res = (deviation < 0) ? -Settings.P : Settings.P;
                    mDeviationSum = 0;
                }
                else
                {
                    return deviation;
                }
            }
            else
            {
                //limit the deviation
                if (Math.Abs(deviation) > Settings.P)
                {
                    res = (deviation < 0) ? Settings.P : -Settings.P;
                    mDeviationSum = 0;
                }
                else
                {
                    return deviation;
                }
            }

            // proportional component
            res *= Settings.P; //val * µm/val

            //differential component if configured.
            if ((Settings.D != null) && (Settings.D.Value != 0) && (mLastControlOutput != 0))
            {
                res -= (((mLastDeviation - deviation) / mLastControlOutput) * Settings.D.Value);
            }

            //integral component if configured
            if ((Settings.I != null) && (Settings.I.Value != 0))
            {
                res += (mDeviationSum * Settings.I.Value);
            }

            //simple Anti-Windup. sum should not exceed MaxDeviationSum or -MaxDeviationSum.
            if ((Settings.MaxDeviationSum > 0) && (Math.Abs(deviation + mDeviationSum) < Math.Abs(Settings.MaxDeviationSum)))
            {
                mDeviationSum += deviation;
            }
            mLastDeviation = deviation;
            mLastControlOutput = res;
            return res;
        }

        public void Reset()
        {
            mDeviationSum = 0;
            mLastControlOutput = 0;
            mLastDeviation = 0;
        }
    }
}
