#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Pump_Tecan_Centris.cs" company="Scaneva">
// 
//  Copyright (C) 2019 Roche Diabetes Care GmbH (Kirill Sliozberg)
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
using System.Globalization;
using System.IO.Ports;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware.Pump
{
    [DisplayName("Hamilton PSD Syringe Pump")]
    [Category("Pumps")]
    public class Pump_Hamilton_PSD : ParametrizableObject, IHWManager, ISyringePump, ITransducer
    {

        //serial stuff
        SerialPort _serialPort;
        private byte[] byteRxBuffer = new byte[1024];
        private string serialAsyncReadBuf = "";
        private bool bAsyncHandlingEnabled = true;
        string res = ""; //todo: good to have it global?

        private string cSTX = "" + (char)2;
        private string cETX = "" + (char)3;
        private string cEOT = "" + (char)4;
        private string cENQ = "" + (char)5;
        private string cACK = "" + (char)6;
        private string cNAK = "" + (char)21;

        //transducer stuff
        public List<TransducerChannel> channels = new List<TransducerChannel>();

        //status
        private enuPumpStatus pumpStatus = enuPumpStatus.NotInitialized;
        private enuHWStatus hwStatus = enuHWStatus.NotInitialized;

        public Pump_Hamilton_PSD(LogHelper log) : base(log)
        {
            settings = new Pump_Tecan_Centris_Settings();
            log.Add("Initializing Hamilton PSD Pump");
            InitTransducerChannels();

            string[] ports = SerialPort.GetPortNames();
            Settings.ComPortList.AddRange(ports);

            hwStatus = enuHWStatus.NotInitialized;
        }

        public Pump_Hamilton_PSD_Settings Settings
        {
            get
            {
                return (Pump_Hamilton_PSD_Settings)settings;
            }
            set
            {
                settings = value;
            }
        }

        //IHWCompo
        public bool IsEnabled { get; set; }

        public enuHWStatus HWStatus => hwStatus;

        public enuHWStatus Initialize()
        {
            return enuHWStatus.Ready;
        }

        public enuHWStatus Connect()
        {
            try
            {
               
              

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                hwStatus = enuHWStatus.Error;
            }

            return hwStatus;
        }

        public void Release()
        {
            
        }

       
        public double RelativePickup //µl
        {
            set
            {
               
            }
        }

        public double RelativeDispense //µl
        {
            set
            {
               
            }
        }

        public double PlungerPosition //µl
        {
            get
            {
               
                return 0;
            }

            set
            {
               
            }
        }

        //Transducer
        private void InitTransducerChannels()
        {
            channels = new List<TransducerChannel>();
            channels.Add(new TransducerChannel(this, "Valve Position", "", enuPrefix.none, enuChannelType.mixed, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Speed", "µl/s", enuPrefix.none, enuChannelType.mixed, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Plunger Position", "µl", enuPrefix.none, enuChannelType.mixed, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Relative Pickup", "µl", enuPrefix.none, enuChannelType.active, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Relative Dispense", "µl", enuPrefix.none, enuChannelType.active, enuTChannelStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.Pump;

        public List<TransducerChannel> Channels { get => channels; }
        public enValveMode ValveMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ValvePosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Speed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PlungerPostion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double GetValue(TransducerChannel channel)
        {
            
                    return 0;
        
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
                value = +GetValue(channel);
            }

            return value / channel.Averaging;
        }

        public enuTChannelStatus SetValue(TransducerChannel channel, double _value)
        {
            
            return enuTChannelStatus.OK;
        }

        private string SendRequest(string _req)
        {
            return "";
        }
       
    }
}
