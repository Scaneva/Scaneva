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

using Scaneva.Core.Settings;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware
{
    [DisplayName("Dummy Positioner")]
    [Category("Dummy HW")]
    class PC_Dummy : ParametrizableObject, IHWManager, IPositioner, ITransducer
    {
        Position pos = new Position();
        Position speeds = new Position();
        enuHWStatus hwStatus = enuHWStatus.NotInitialized;
        public bool IsEnabled { get; set; }

        enuPositionerStatus posStatus = enuPositionerStatus.NotInitialized;
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

        //Transducer
        private void InitTransducerChannels()
        {
            channels = new List<TransducerChannel>();
            channels.Add(new TransducerChannel(this, "X-Axis", "m", enuPrefix.µ, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Y-Axis", "m", enuPrefix.µ, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Z-Axis", "m", enuPrefix.µ, enuChannelType.passive, enuSensorStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.Potentiostat;

        public List<TransducerChannel> Channels { get => channels; }

        public double GetValue(TransducerChannel channel)
        {
            switch (channel.Name)
            {
                case "X-Axis":
                    return pos.X;
                case "Y-Axis":
                    return pos.Y;
                case "Z-Axis":
                    return pos.Z;
                default:
                    return 0;
            }
        }

        public double GetAveragedValue(TransducerChannel channel)
        {
            return GetValue(channel);
        }

        public void SetValue(TransducerChannel channel, double _value)
        {
            //noop
        }

        public int Averaging { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //Transducer

        public enuHWStatus HWStatus => hwStatus;

        public enuPositionerStatus Status => posStatus;

      

        public enuHWStatus Connect()
        {
            hwStatus = enuHWStatus.Ready;
            posStatus = enuPositionerStatus.Ready;
            return hwStatus;
        }

        public enuHWStatus Initialize()
        {
            log.Add("Initializing dummy positioner");
            InitTransducerChannels();
            return enuHWStatus.Ready;
        }

        public void Release()
        {

        }

       public Position AbsolutePosition()
        {
            return pos;
        }

        public enuPositionerStatus AbsolutePosition(Position _pos)
        {
            pos = _pos;
            return enuPositionerStatus.Ready;
        }


        public double AxisAbsolutePosition(enuAxes _axis)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    return pos.X;
                case enuAxes.YAxis:
                    return pos.Y;
                case enuAxes.ZAxis:
                    return pos.Z;
                default: return 0; //todo: log an error event (implement a-axis as well?)
            }
        }

        public double AxisMinIncrement(enuAxes _axis)
        {
            return 0;
        }

        public enuPositionerStatus MoveAbsolut(enuAxes _axis, double _position, double _speed)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    pos.X = _position;
                    break;
                case enuAxes.YAxis:
                    pos.Y = _position;
                    break;
                case enuAxes.ZAxis:
                    pos.Z = _position;
                    break;
                default: return 0; //todo: log an error event (implement a-axis as well?)
            }
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus MoveRelativ(enuAxes _axis, double _increment, double _speed)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    pos.X = pos.X + _increment;
                    break;
                case enuAxes.YAxis:
                    pos.Y = pos.Y + _increment;
                    break;
                case enuAxes.ZAxis:
                    pos.Z = pos.Z + _increment;
                    break;
            }
            return enuPositionerStatus.Ready;
        }

        public double Speed(enuAxes _axis)
        {

            switch (_axis)
            {
                case enuAxes.XAxis:
                    return speeds.X;
                case enuAxes.YAxis:
                    return speeds.Y;
                case enuAxes.ZAxis:
                    return speeds.Z;
                default: return 0; //todo: log an error event (implement a-axis as well?)
            }
        }

        public enuPositionerStatus Speed(enuAxes _axis, double _speed)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    speeds.X = _speed;
                    break;
                case enuAxes.YAxis:
                    speeds.Y = _speed;
                    break;
                case enuAxes.ZAxis:
                    speeds.Z = _speed;
                    break;
                default: return 0; //todo: log an error event (implement a-axis as well?)
            }
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus AxisStatus(enuAxes _axis)
        {
            return enuPositionerStatus.Ready;
        }

        public void AxisStatus(enuAxes _axis, enuPositionerStatus _stat)
        {
            //noop
        }

        public enuPositionerStatus AxisStop(enuAxes _axis)
        {
            return enuPositionerStatus.Ready;
        }

        public double ValidateDistance(enuAxes _axis, double _distance)
        {
            return _distance;
        }

        public double ValidateSpeed(enuAxes _axis, double _speed)
        {
            return _speed;
        }

        public enuPositionerStatus RelativePosition(Position _pos)
        {
            pos = pos.Sum(_pos);
            return enuPositionerStatus.Ready;
        }

        public Position Speeds()
        {
            return speeds;
        }

        public enuPositionerStatus Speeds(Position _speed)
        {
            speeds = _speed;
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus StopMovement()
        {
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidatePosition(ref Position _pos)
        {
            _pos = _pos;
            return enuPositionerStatus.Ready;
        }

        public enuPositionerStatus ValidateSpeeds(ref Position _speed)
        {
            _speed = _speed;
            return enuPositionerStatus.Ready;
        }


    }
}