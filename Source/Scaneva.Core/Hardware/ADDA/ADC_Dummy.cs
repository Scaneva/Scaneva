#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ADC_Dummy.cs" company="Scaneva">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Scaneva.Core.Settings;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware
{
    [DisplayName("Dummy ADC")]
    [Category("Dummy HW")]
    public class ADC_Dummy : ParametrizableObject, IHWManager, ITransducer
    {
        private double LastPotential;
        public enuHWStatus HWStatus => enuHWStatus.Ready;
        public bool IsEnabled { get; set; }

        public List<TransducerChannel> channels = new List<TransducerChannel>();

        public ADC_Dummy(LogHelper log)
            : base(log)
        {
            settings = new ADC_Dummy_Settings();

            log.Add("Initializing dummy ADC");
        }

        public ADC_Dummy_Settings Settings
        {
            get
            {
                return (ADC_Dummy_Settings)settings;
            }
            set
            {
                settings = value;
            }
        }


        public enuHWStatus Initialize()
        {
            log.Add("ADC Dummy.Initialize");
            InitTransducerChannels();
            return enuHWStatus.Ready;
        }


        public enuHWStatus Connect()
        {
            return enuHWStatus.Ready;
        }


        public void Release()
        {
            //throw new NotImplementedException();
        }

        private void InitTransducerChannels()
        {
            channels = new List<TransducerChannel>();
            channels.Add(new TransducerChannel(this, "Dummy potential", "V", enuPrefix.none, enuChannelType.mixed, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Current", "A", enuPrefix.µ, enuChannelType.passive, enuSensorStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.ADC;

        public List<TransducerChannel> Channels { get => channels; }
       

        public double GetValue(TransducerChannel channel)
        {

            switch (channel.Name)
            {
                case "Dummy potential":
                    return LastPotential;

                case "Current":
                    return GetCurrent(LastPotential);

                default:
                    break;
            }
            return double.NaN;
        }

        public void SetValue(TransducerChannel channel, double _value)
        {
            if (channel.Name ==  "Dummy potential")
            {
                LastPotential = _value;
            }
        }

        public void SetAveraging(TransducerChannel channel, int _value)
        {
            channel.Averaging = _value;
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
                value = +GetValue(channel);
            }

            return value / channel.Averaging;

            //todo: make internal avaraging
        }

        //Transducer

        private double GetCurrent(double _potential)
        {
            double i;
            double i0 = 0.0058; //A/m2
            double alphaA = 0.5;
            double alphaC = 0.5;
            int z = 2;
            double F = 96485.33289; //C/mol
            double Eeq = 0.2; //V
            double R = 8.314;
            double T = 273.15;
            return i = i0 * (Math.Exp(((alphaA * z * F * (Eeq-_potential)) / (R * T)) - Math.Exp(((-alphaC * z * F * (Eeq - _potential)) / (R * T)))));
        }
    }
}
