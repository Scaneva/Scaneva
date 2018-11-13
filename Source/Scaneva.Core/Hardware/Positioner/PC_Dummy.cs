#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PC_Dummy.cs" company="Scaneva">
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Scaneva.Core.Settings;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware
{
    [DisplayName("Dummy Positioner")]
    [Category("Dummy HW")]
    class PC_Dummy : ParametrizableObject, IHWManager, IPositioner, ITransducer
    {
        Position mPos = new Position();
        Position mSpeeds = new Position();
        public bool IsEnabled { get; set; }
        public List<TransducerChannel> channels = new List<TransducerChannel>();

        public PC_Dummy(LogHelper log) : base(log)
        {
            log.Add("Instantiating dummy positioner");
            settings = new PC_Dummy_Settings();
        }

        public PC_Dummy_Settings Settings
        {
            get
            {
                return (PC_Dummy_Settings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public enuPositionerStatus GetAxisSpeed(enuAxes _axis, ref double _speed)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    _speed = mSpeeds.X;
                    break;
                case enuAxes.YAxis:
                    _speed = mSpeeds.Y;
                    break;
                case enuAxes.ZAxis:
                    _speed = mSpeeds.Z;
                    break;
            }
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidateAxisSpeed(enuAxes _axis, ref double _speed)
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus SetAxisSpeed(enuAxes _axis, double _speed)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    mSpeeds.X = _speed;
                    break;
                case enuAxes.YAxis:
                    mSpeeds.Y = _speed;
                    break;
                case enuAxes.ZAxis:
                    mSpeeds.Z = _speed;
                    break;
            }
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidateAxisRelativeMovement(enuAxes _axis, ref double _distance)
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus SetAxisRelativePosition(enuAxes _axis, double _increment)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    mPos.X += _increment;
                    Thread.Sleep((int)(_increment / mSpeeds.X));
                    break;
                case enuAxes.YAxis:
                    mPos.Y += _increment;
                    Thread.Sleep((int)(_increment / mSpeeds.Y));
                    break;
                case enuAxes.ZAxis:
                    mPos.Z += _increment;
                    Thread.Sleep((int)(_increment / mSpeeds.Z));
                    break;
            }
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus GetAxisAbsolutePosition(enuAxes _axis, ref double _pos)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    _pos = mPos.X;
                    break;
                case enuAxes.YAxis:
                    _pos = mPos.Y;
                    break;
                case enuAxes.ZAxis:
                    _pos = mPos.Z;
                    break;
            }
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidateAxisAbsolutePosition(enuAxes _axis, ref double _pos)
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus SetAxisAbsolutePosition(enuAxes _axis, double _position)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    Thread.Sleep((int)((Math.Abs(mPos.X - _position) / mSpeeds.X)));
                    mPos.X = _position;
                    break;
                case enuAxes.YAxis:
                    Thread.Sleep((int)((Math.Abs(mPos.Y - _position) / mSpeeds.Y)));
                    mPos.Y = _position;
                    break;
                case enuAxes.ZAxis:
                    Thread.Sleep((int)((Math.Abs(mPos.Z - _position) / mSpeeds.Z)));
                    mPos.Z = _position;
                    break;
            }
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus AxisStop(enuAxes _axis)
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus GetSpeeds(ref Position _speeds)
        {
            _speeds = mSpeeds;
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidateSpeeds(ref Position _speeds)
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus SetSpeeds(Position _speeds)
        {
            mSpeeds = _speeds;
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidateRelativeMovement(ref Position _pos)
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus SetRelativePosition(Position _pos)
        {
            mPos = mPos.Sum(_pos);
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus GetAbsolutePosition(ref Position _pos)
        {
            _pos = mPos;
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidateAbsolutePosition(ref Position _pos)
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus SetAbsolutePosition(Position _pos)
        {
            mPos = _pos;
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus StopMovement()
        {
            return enuPositionerStatus.Ready;
        }

        private int AxisIndex(enuAxes _axis)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    return Settings.X.AxisNumber;
                case enuAxes.YAxis:
                    return Settings.Y.AxisNumber;
                case enuAxes.ZAxis:
                    return Settings.Z.AxisNumber;
            }
            return 0;
        }


        private void InitTransducerChannels()
        {
            channels = new List<TransducerChannel>();
            channels.Add(new TransducerChannel(this, "X-Axis", "m", enuPrefix.µ, enuChannelType.mixed, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Y-Axis", "m", enuPrefix.µ, enuChannelType.mixed, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Z-Axis", "m", enuPrefix.µ, enuChannelType.mixed, enuTChannelStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.Positioner;
        public List<TransducerChannel> Channels { get => channels; }

        public enuPositionerStatus GetPositionerStatus => enuPositionerStatus.Ready;

        enuHWStatus IHWManager.HWStatus => throw new NotImplementedException();

        public double GetValue(TransducerChannel channel)
        {
            double res = double.NaN;
            switch (channel.Name)
            {
                case "X-Axis":
                    GetAxisAbsolutePosition(enuAxes.XAxis, ref res);
                    break;
                case "Y-Axis":
                    GetAxisAbsolutePosition(enuAxes.YAxis, ref res);
                    break;
                case "Z-Axis":
                    GetAxisAbsolutePosition(enuAxes.ZAxis, ref res);
                    break;
                case "A-Axis":
                    GetAxisAbsolutePosition(enuAxes.AAxis, ref res);
                    break;
            }
            return res;
        }

        public enuTChannelStatus SetAveraging(TransducerChannel channel, int _value)
        {
            channel.Averaging = _value;
            return enuTChannelStatus.OK;
        }

        public int GetAveraging(TransducerChannel channel)
        {
            return channel.Averaging;
        }

        public double GetAveragedValue(TransducerChannel channel)
        {
            double value = 0;
            for (int i = 1; i <= channel.Averaging; i++)
            {
                value += GetValue(channel);
            }

            return value / channel.Averaging;
        }

        public enuTChannelStatus SetValue(TransducerChannel channel, double _value)
        {
            switch (channel.Name)
            {
                case "X-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.XAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                case "Y-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.YAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                case "Z-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.ZAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                case "A-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.AAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                default:
                    return enuTChannelStatus.Error;
            }
            return enuTChannelStatus.OK;
        }

        public enuHWStatus Connect()
        {
            return enuHWStatus.Ready;
        }



        public enuHWStatus Initialize()
        {
            return enuHWStatus.Ready;
        }

        public void Release()
        {
        }

        public enuHWStatus HWStatus()
        {
            return enuHWStatus.Ready;
        }

        public enuPositionerStatus GetAxisStatus(enuAxes _axis)
        {
            return enuPositionerStatus.Ready;
        }
    }
}
