#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Pump_Watson-Marlow_520Du.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
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
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

using Scaneva.Core.Settings;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware
{
    [DisplayName("Watson-Marlow 520Du peristaltic pump")]
    [Category("Pumps")]
    class Pump_Watson_Marlow_520Du : ParametrizableObject, IHWManager, IPump, ITransducer
    {
        //serial stuff
        SerialPort _serialPort;
        private byte[] byteRxBuffer = new byte[1024];
        private string serialAsyncReadBuf = "";
        private bool bAsyncHandlingEnabled = true;
        string res = ""; //todo: good to have it global?

        //transducer stuff
        public List<TransducerChannel> channels = new List<TransducerChannel>();

        //status
        enuPumpStatus pumpStatus = enuPumpStatus.NotInitialized;
        enuHWStatus hwStatus = enuHWStatus.NotInitialized;
        public List<PumpTubing> Tubes = new List<PumpTubing>();

        //IHWCompo
        public bool IsEnabled { get; set; }

        public enuHWStatus Connect()
        {
            hwStatus = enuHWStatus.NotInitialized;

            _serialPort = new SerialPort();
            _serialPort.PortName = "COM" + Convert.ToString(Settings.COMPort);
            _serialPort.BaudRate = Settings.Baudrate;
            _serialPort.Parity = Settings.Parity;
            _serialPort.DataBits = Settings.Databits;
            _serialPort.StopBits = Settings.StopBits;
            _serialPort.Handshake = Settings.Handshake;
            //todo auto eacho parameter
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            //Bind the events on the following event handler
            _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialErrorReceived);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            if (Status().HasFlag(enuPumpStatus.Ready))
            {
                hwStatus = enuHWStatus.Ready;
            }
            else
            {
                log.Add("Could not initiate pump!");
            }
            return hwStatus;
        }

        public enuHWStatus Initialize()
        {
            return enuHWStatus.Ready;
        }

        public enuHWStatus HWStatus => hwStatus;

        public void Release()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort = null;
            }
        }

        public Pump_Watson_Marlow_520Du(LogHelper log) : base(log)
        {
            settings = new Pump_Watson_Marlow_520Du_Settings();
            log.Add("Initializing Watson-Marlow 520Du peristalric pump");
            Tubes.Add(new PumpTubing("520R Neoprene 0.5 mm", 0.5, 0.1, 220, 0.004, 9.5));
            Tubes.Add(new PumpTubing("520R Neoprene 0.8 mm", 0.8, 0.1, 220, 0.01, 24));
            Tubes.Add(new PumpTubing("520R Neoprene 1.6 mm", 1.6, 0.1, 220, 0.04, 97));
            Tubes.Add(new PumpTubing("520R Neoprene 3.2 mm", 3.2, 0.1, 220, 0.18, 390));
            Tubes.Add(new PumpTubing("520R Neoprene 4.8 mm", 4.8, 0.1, 220, 0.4, 870));
            Tubes.Add(new PumpTubing("520R Neoprene 6.4 mm", 6.4, 0.1, 220, 0.7, 1500));
            Tubes.Add(new PumpTubing("520R Neoprene 8.0 mm", 8.0, 0.1, 220, 1.1, 2400));
            Tubes.Add(new PumpTubing("520R Neoprene 9.6 mm", 9.6, 0.1, 220, 1.6, 3500));

            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 0.5 mm", 0.5, 0.1, 220, 0.004, 9));
            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 0.8 mm", 0.8, 0.1, 220, 0.01, 23));
            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 1.6 mm", 1.6, 0.1, 220, 0.04, 92));
            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 3.2 mm", 3.2, 0.1, 220, 0.17, 370));
            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 4.8 mm", 4.8, 0.1, 220, 0.38, 830));
            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 6.4 mm", 6.4, 0.1, 220, 0.67, 1500));
            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 8.0 mm", 8.0, 0.1, 220, 1.1, 2300));
            Tubes.Add(new PumpTubing("520R Marpene/Bioprene 64 9.6 mm", 9.6, 0.1, 220, 1.5, 3300));

            Tubes.Add(new PumpTubing("520R Fluorel 1.6 mm", 1.6, 0.1, 220, 0.03, 70));
            Tubes.Add(new PumpTubing("520R Fluorel 3.2 mm", 3.2, 0.1, 220, 0.13, 280));
            Tubes.Add(new PumpTubing("520R Fluorel 4.8 mm", 4.8, 0.1, 220, 0.29, 630));
            Tubes.Add(new PumpTubing("520R Fluorel 6.4 mm", 6.4, 0.1, 220, 0.51, 1100));
            Tubes.Add(new PumpTubing("520R Fluorel 8.0 mm", 8.0, 0.1, 220, 0.8, 1800));

            Settings.ListofTubings = Tubes;
            InitTransducerChannels();
        }

        public Pump_Watson_Marlow_520Du_Settings Settings
        {
            get
            {
                return (Pump_Watson_Marlow_520Du_Settings)settings;
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
            channels.Add(new TransducerChannel(this, "Counter", "l", enuPrefix.µ, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Speed", "l/s", enuPrefix.µ, enuChannelType.mixed, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Run", "", enuPrefix.none, enuChannelType.mixed, enuSensorStatus.OK)); // 1 - run, 0, stop
            channels.Add(new TransducerChannel(this, "Direction", "", enuPrefix.none, enuChannelType.mixed, enuSensorStatus.OK)); //1 - CW, 0 - CCW
        }

        public enuTransducerType TransducerType => enuTransducerType.Pump;

        public List<TransducerChannel> Channels { get => channels; }

        public double GetValue(TransducerChannel channel)
        {
            switch (channel.Name)
            {
                case "Run":
                    return Math.Abs(Convert.ToDouble(Run.HasFlag(enuPumpStatus.Running)));
                case "Direction":
                    return Math.Abs(Convert.ToDouble(Run.HasFlag(enuPumpStatus.Direction)));
                case "Speed":
                    return Speed;
                case "Counter":
                    return Counter;
                default:
                    return 0;
            }
        }

        public double GetAveragedValue(TransducerChannel channel)
        {
            return GetValue(channel);
        }

        public int Averaging { get => 1; set => throw new NotImplementedException(); } //todo: get rid of the exception

        public void SetValue(TransducerChannel channel, double _value)
        {
            switch (channel.Name)
            {
                case "Run":
                    if (_value > 0)
                    {
                        Run |= enuPumpStatus.Running;
                    }
                    else
                    {
                        Run &= ~enuPumpStatus.Running;
                    }
                    return;

                case "Direction":
                    if (_value > 0)
                    {
                        Run |= enuPumpStatus.Direction;
                    }
                    else
                    {
                        Run &= ~enuPumpStatus.Direction;
                    }
                    return;

                case "Speed":
                    Speed = _value;
                    return;

                case "Counter":
                    Counter = 0;
                    return;

                default:
                    return;
            }
        }

        //IPump
        public enuPumpStatus Dose(long _pulses, int _drip)
        {
            return enuPumpStatus.Ready;
        }

        public double Counter // in µl!
        {
            get
            {
                if (sendCommand(Convert.ToString(Settings.PumpID) + "RT", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Settings.TubingObj.RPM2Speed(1) * Convert.ToDouble(res) / 10982; //10,982 pulses per revolution                 
                }
            }
            set
            {
                if (sendCommand(Convert.ToString(Settings.PumpID) + "TC", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
        }

        public double Speed //in µl/s!
        {
            get
            {
                if (sendCommand(Convert.ToString(Settings.PumpID) + "RS", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    String[] substrings = res.Split(new char[0]);
                    return Settings.TubingObj.RPM2Speed(Convert.ToDouble(substrings[4]));
                }
            }
            set
            {
                double rpm = (value <= Settings.TubingObj.RPM2Speed(220)) ? Settings.TubingObj.Speed2RPM(value) : Settings.TubingObj.RPM2Speed(220);
                if (sendCommand(Convert.ToString(Settings.PumpID) + "SP" + rpm.ToString("F1"), out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
        }

        public enuPumpStatus Run
        {
            get
            {
                if (sendCommand(Convert.ToString(Settings.PumpID) + "ZY", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    hwStatus = enuHWStatus.Error;
                    pumpStatus = enuPumpStatus.Error;
                }
                else
                {
                    if (res == "1")
                    {
                        pumpStatus |= enuPumpStatus.Running;
                    }
                    else if (res == "0")
                    {
                        pumpStatus &= ~enuPumpStatus.Running;
                    }
                    else
                    {
                        pumpStatus = enuPumpStatus.Error;
                    }
                }
                return pumpStatus;
            }
            set
            {
                if (sendCommand(Convert.ToString(Settings.PumpID) + (value.HasFlag(enuPumpStatus.Running) ? "GO" : "ST"), out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
        }


        public enuPumpStatus Direction
        {
            get
            {
                if (sendCommand(Convert.ToString(Settings.PumpID) + "RS", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
                else
                {
                    String[] substrings = res.Split(new char[0]);
                    if (Convert.ToString(substrings[5]) == "CW")
                    {
                        pumpStatus |= enuPumpStatus.Direction;
                    }
                    else if (Convert.ToString(substrings[5]) == "CCW")
                    {
                        pumpStatus &= ~enuPumpStatus.Direction;
                    }
                    else
                    {
                        pumpStatus = enuPumpStatus.Error;
                    }

                    if (Convert.ToString(substrings[8]) == "1")
                    {
                        pumpStatus |= enuPumpStatus.Running;
                    }
                    else if (Convert.ToString(substrings[5]) == "0")
                    {
                        pumpStatus &= ~enuPumpStatus.Running;
                    }
                    else
                    {
                        pumpStatus = enuPumpStatus.Error;
                    }
                }
                return pumpStatus;
            }
            set
            {
                if (sendCommand(Convert.ToString(Settings.PumpID) + (value.HasFlag(enuPumpStatus.Direction) ? "RR" : "RL"), out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
        }

        public double? Flowrate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Pressure { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        enuPumpStatus IPump.Run { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        enuPumpStatus IPump.Direction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<int, double?> Composition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? Valve { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public enuPumpStatus Status()
        {
            if (sendCommand(Convert.ToString(Settings.PumpID) + "RS", out res) != enuHWStatus.Ready)
            {
                log.Add("COM communication failed!");
                pumpStatus = enuPumpStatus.Error;
            }
            else
            {
                pumpStatus = enuPumpStatus.Ready;
                String[] substrings = res.Split(new char[0]);
                if (Convert.ToString(substrings[5]) == "CW")
                {
                    pumpStatus |= enuPumpStatus.Direction;
                }
                else if (Convert.ToString(substrings[5]) == "CCW")
                {
                    pumpStatus &= ~enuPumpStatus.Direction;
                }
                else
                {
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            return pumpStatus;
        }


        //private

        // Send the given command string directly to the serial port
        // <param name="command">Command String to send</param>
        // <param name="returnValue">Response</param>
        private enuHWStatus sendCommand(string command, out string returnValue)
        {
            //NanoInterfaceReturnCode retCode = NanoInterfaceReturnCode.Return_CommandNotAcknowledged;
            returnValue = "";
            if (_serialPort != null)
            {
                bAsyncHandlingEnabled = false;
                _serialPort.DiscardInBuffer();
                _serialPort.Write(command + "\r");
                //   AppendToLogSend(command + "\n");
                int timeoutCounter = 0; //todo: use built in time out
                bool commComplete = false;

                while ((!commComplete) && (timeoutCounter < 1000))
                {
                    int idx = serialAsyncReadBuf.IndexOf("\r");
                    if (idx >= 0)
                    {
                        string inputString = serialAsyncReadBuf.Substring(0, idx);
                        if ((idx) == serialAsyncReadBuf.Length)
                        {
                            serialAsyncReadBuf = "";
                        }
                        else
                        {
                            serialAsyncReadBuf = serialAsyncReadBuf.Substring(idx);
                        }
                        //AppendToLogReceive(inputString);
                        commComplete = true;
                        returnValue = inputString;
                        hwStatus = enuHWStatus.Ready;
                    }

                    if (!commComplete)
                    {
                        Thread.Sleep(10);
                        timeoutCounter += 10;
                    }
                }

                bAsyncHandlingEnabled = true;
                // Call Handler to process any additional return messages
                DataReceivedHandler(null, null);
            }
            else
            {
                hwStatus = enuHWStatus.NotInitialized;
            }
            return hwStatus;
        }

        /// Data Received Handler
        /// </summary>
        private void DataReceivedHandler(
                            object sender,
                            SerialDataReceivedEventArgs e)
        {
            // Read Bytes
            serialAsyncReadBuf += _serialPort.ReadExisting();

            if (bAsyncHandlingEnabled)
            {
                bool bComplete = serialAsyncReadBuf.EndsWith("\r");
                string[] strs = serialAsyncReadBuf.Split("\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < (strs.Length - 1); i++)
                {
                    // AppendToLogReceive(strs[i]);
                    // messageListener.NanoInterfaceMessageReceived(strs[i]);
                }
                if (bComplete)
                {
                    // AppendToLogReceive(strs[strs.Length - 1]);
                    // messageListener.NanoInterfaceMessageReceived(strs[strs.Length - 1]);
                    serialAsyncReadBuf = "";
                }
                else if (strs.Length != 0)
                {
                    serialAsyncReadBuf = strs[strs.Length - 1];
                }
            }
        }

        //Serial Port Error Event Handler
        private void SerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //AppendToLogInfo(e.ToString());
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public override void ParameterChanged(string name)
        {
            switch (name)
            {
                case "Settings Loaded":
                    Settings.ListofTubings = Tubes;
                    break;

                default:
                    break;
            }

        }

        public override void SerializeParameterValues(XmlWriter writer)
        {
            base.SerializeParameterValues(writer);
        }

        public override void DeserializeParameterValues(IXPathNavigable node, Dictionary<string, Type> availableExperiments)
        {
            base.DeserializeParameterValues(node, availableExperiments);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        enuPumpStatus IPump.Status()
        {
            throw new NotImplementedException();
        }
    }
}

/*       

RS232 and RS485 command strings
Command     Parameters              Meaning
nCA         -                       Clear LCD display
nCH         -                       Home the cursor
nDO         xxxxxxxxxx<,yyyyy>      Set and run one dose of xxxxxxxxxx tacho pulses, with optional drip of yyyyy (maximum 11,000) tacho pulses. See note 1
nTC         -                       Clear the cumulative tachometer count
nSP         xxx.x                   Set speed to xxx.x rpm
nSI         -                       Increment speed by 1rpm
nSD         -                       Decrement speed by 1rpm
nGO         -                       Start running
nST         -                       Stop running
nRC         -                       Change direction
nRR         -                       Set direction to clockwise
nRL         -                       Set direction to counter-clockwise
nRS         -                       Return status. See note 2
nRT         -                       Return the cumulative tachometer count
nW          [line 1]~[line 2]~[line 3]~[line 4]@    Display text on 1 to 4 lines with ~ as the line delimiter. Terminated by the @ character. See note 3
nZY         -                       Return 0 for stopped or 1 for running

Note 1: The correlation between tachometer pulses and motor rotation is fixed
and provides a measurable and absolute way of monitoring the number of revolutions
of the gearbox output shaft - 10,982 pulses per revolution. This in turn
allows the count to be equated to the amount of material dispensed - assuming
that the pumphead type and tube size are known.

Note 2: The status is returned to the sender in the following format: [pump
type] [ml/rev] [pumphead] [tube size] [speed] [CW/CCW] P/N [pump number]
[tacho count] [0/1 (stopped/running)] !
For example: 520Du 15.84 520R 9.6MM 220.0 CW P/N 1 123456789 1 !

Note 3: If the pump speed is changed subsequently, the
pump must display the screen shown here (example figures
only) for 4 seconds before reverting to the custom
display. 1 to 4 lines of text can be written with ~ as the
line delimiter and @ as the message end.
i.e., 1W520Du@ and 1W520Du~@ are both valid commands.

Note 4: In all cases ‘n’ can be any number from 1 to 16 inclusive, and by
exception the # symbol can be used as an all-drives command; but not with the
RS, RT or ZY commands, as the results would be indeterminate

*/
