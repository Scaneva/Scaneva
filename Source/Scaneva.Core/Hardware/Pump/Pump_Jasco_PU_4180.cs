#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Pump_Jasco_PU_4180.cs" company="Scaneva">
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
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

using Scaneva.Core.Settings;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware
{
    [DisplayName("Jasco PU 4180 HPLC pump")]
    [Category("Pumps")]
    class Pump_Jasco_PU_4180 : ParametrizableObject, IHWManager, IPump, ITransducer
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
                InitTransducerChannels();
                hwStatus = enuHWStatus.Ready;
            }
            else
            {
                log.Add("Could not initiate pump!", "Error");
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

        public Pump_Jasco_PU_4180(LogHelper log) : base(log)
        {
            settings = new Pump_Jasco_PU_4180_Setting();
            log.Add("Initializing Jasco PU-4180 HPLC pump");         
        }

        public Pump_Jasco_PU_4180_Setting Settings
        {
            get
            {
                return (Pump_Jasco_PU_4180_Setting)settings;
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
            channels.Add(new TransducerChannel(this, "Flowrate", "l/min", enuPrefix.m, enuChannelType.mixed, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Max. pressure", "g/cm2", enuPrefix.k, enuChannelType.mixed, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Min. pressure", "g/cm2", enuPrefix.k, enuChannelType.mixed, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Actual pressure", "g/cm2", enuPrefix.k, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Elapsed time", "s", enuPrefix.none, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Pump-off timer", "h", enuPrefix.none, enuChannelType.active, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Pump-on timer", "h", enuPrefix.none, enuChannelType.active, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Valve position", "#", enuPrefix.none, enuChannelType.mixed, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Composition A", "%", enuPrefix.none, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Composition B", "%", enuPrefix.none, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Composition C", "%", enuPrefix.none, enuChannelType.passive, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Composition D", "%", enuPrefix.none, enuChannelType.passive, enuSensorStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.Pump;

        public List<TransducerChannel> Channels { get => channels; }

        public double GetValue(TransducerChannel channel)
        {
            switch (channel.Name)
            {
                case "Flowrate":
                    return Flowrate.Value;
                case "Max. pressure":
                    return MaxPressure;
                case "Min. pressure":
                    return MinPressure;
                case "Actual pressure":
                    return Pressure;
                case "Elapsed time":
                    return ElapsedTime;
                case "Valve position":
                    return Valve.Value;
                case "Composition A":
                    return CompositionA;
                case "Composition B":
                    return CompositionB;
                case "Composition C":
                    return CompositionC;
                case "Composition D":
                    return CompositionD;
                default:
                    return 0;
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
        public void SetValue(TransducerChannel channel, double _value)
        {
            switch (channel.Name)
            {

                case "Flowrate":
                    Flowrate = _value;
                    return;
                case "Max. pressure":
                    MaxPressure = _value;
                    return;
                case "Min. pressure":
                    MinPressure = _value;
                    return;
                case "Pump-off timer":
                case "Pump-on timer":
                case "Valve position":
                    Valve = Convert.ToInt16(_value);
                    return;
                default:
                    return;
            }
        }

        //IHPump
        public enuPumpStatus Status()
        {
            if (Flowrate != -1) //workaround. We just trying to get any pump information to check, whether the communication works
            {
                pumpStatus |= enuPumpStatus.Ready;
            }
            else
            {
                pumpStatus = enuPumpStatus.Error;
            }
            return pumpStatus;
        }

        public double? Flowrate //ml/min
        {
            get
            {
                if (sendCommand("a_flow load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
            set
            {
                if (value != null)
                {
                    //xx.xxx  0.000 - 10.000 in steps of 0.001 ml/min
                    if (sendCommand(value.Value.ToString("F3", CultureInfo.InvariantCulture) + " flowrate set", out res) != enuHWStatus.Ready)
                    {
                        log.Add("COM communication failed!", "Error");
                        pumpStatus = enuPumpStatus.Error;
                    }
                }
                else
                {
                    log.Add("Wrong parameter!", "Error");
                }
            }
        }

        public double Pressure //kg/cm2
        {
            get
            {
                if (sendCommand("a_press1 load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
            set
            { }
        }

        public double MinPressure //kg/cm2
        {
            get
            {
                if (sendCommand("a_pmin load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
            set
            {
                if ((value >= 0) && (value <= 700))
                {
                    //xxx  0 - 700 in steps of 1 kg/cm2
                    if (sendCommand(value.ToString("F0") + " pmin set", out res) != enuHWStatus.Ready)
                    {
                        log.Add("COM communication failed!", "Error");
                        pumpStatus = enuPumpStatus.Error;
                    }
                }
                else
                {
                    log.Add("Wrong parameter!", "Error");
                }
            }
        }

        public double MaxPressure //kg/cm2
        {
            get
            {
                if (sendCommand("a_pmax load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
            set
            {
                if ((value >= 0) && (value <= 700))
                {
                    //xxx  0 - 700 in steps of 1 kg/cm2
                    if (sendCommand(value.ToString("F0") + " pmax set", out res) != enuHWStatus.Ready)
                    {
                        log.Add("COM communication failed!", "Error");
                        pumpStatus = enuPumpStatus.Error;
                    }
                }
                else
                {
                    log.Add("Wrong parameter!", "Error");
                }
            }
        }

        public double ElapsedTime //s
        {
            get
            {
                if (sendCommand("current_time load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
        }

        public int? Valve //#
        {
            get
            {
                if (sendCommand("valve load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToInt16(res, CultureInfo.InvariantCulture);
                }
            }
            set
            {
                if (value !=null)
                {
                    //xxx  0 - 700 in steps of 1 kg/cm2
                    if (sendCommand(value.Value.ToString("F0") + " valve set", out res) != enuHWStatus.Ready)
                    {
                        log.Add("COM communication failed!", "Error");
                        pumpStatus = enuPumpStatus.Error;
                    }
                }
            }
        }

        public double CompositionA//%
        {
            get
            {
                if (sendCommand("compa load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
        }

        public double CompositionB//%
        {
            get
            {
                if (sendCommand("compb load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
        }

        public double CompositionC//%
        {
            get
            {
                if (sendCommand("compc load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
        }

        public double CompositionD//%
        {
            get
            {
                if (sendCommand("compd load p", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!", "Error");
                    pumpStatus = enuPumpStatus.Error;
                    return -1;
                }
                else
                {
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                }
            }
        }

        public Dictionary<int, double?> Composition
        {
            get
            {
                Dictionary<int, double?> dict = new Dictionary<int, double?>();
                dict.Add(0, CompositionA);
                dict.Add(1, CompositionB);
                dict.Add(2, CompositionC);
                dict.Add(3, CompositionD);
                return dict;
            }
            set
            {//workaround
                int j = 0;
                Dictionary<int, double> dict = new Dictionary<int, double>();
                for (int i = 0; i < 4; i++)
                    if (value[i] != null)
                    {
                        dict.Add(i, value[i].Value);        
                        j = j + Convert.ToInt32 (Math.Pow(2, i));
                    }
                switch (j)
                {
                    case 7:
                        break;
                    case 11:
                        dict[2] = 100 - dict[0] - dict[1] - dict[3];
                        break;
                    case 13:
                        dict[1] = 100 - dict[0] - dict[2] - dict[3];
                        break;
                    case 14:
                        dict[0] = 100 - dict[1] - dict[2] - dict[3];
                        break;
                    case 15:
                        break;
                    default:
                        log.Add("Wrong parameters", "Error");
                        return;
                }
                OpenProgramFile(0);
                SetProgramStepComposition(0, dict[0], dict[1], dict[2]);
                CloseProgramFile();
                SetProgramFile(0);
                ProgramRun();
            }
        }

        public double Counter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Speed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public enuPumpStatus Run { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public enuPumpStatus Direction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //private
        private enuPumpStatus SetProgramFile(int _no)
        {
            if ((_no >= 0) && (_no <= 9))
            {
                if (sendCommand(_no.ToString("F0") + " fileno set", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            else
            {
                log.Add("Wrong parameter!");
            }
            return pumpStatus;
        }

        private enuPumpStatus OpenProgramFile(int _no)
        {
            if ((_no >= 0) && (_no <= 9))
            {
                if (sendCommand(_no.ToString("F0") + " openfile", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            else
            {
                log.Add("Wrong parameter!");
            }
            return pumpStatus;
        }

        private enuPumpStatus CloseProgramFile()
        {
            {
                if (sendCommand("closefile", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            return pumpStatus;
        }

        private enuPumpStatus ProgramRun()
        {
            {
                if (sendCommand("7 pump set", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            return pumpStatus;
        }

        private enuPumpStatus SetProgramStepComposition(double _time, double _A, double _B, double _C) // time in minutes
        {
            if ((_time <= 999.9) && (_time >= 0) && (_A + _B + _C <= 100) && (_A >= 0) && (_B >= 0) && (_C >= 0) && (_A <= 100) && (_B <= 100) && (_C <= 100))
            {
                if (sendCommand(_time.ToString("0.00", CultureInfo.InvariantCulture) + " "
                    + _A.ToString("000.0", CultureInfo.InvariantCulture) + " " 
                    + _B.ToString("000.0", CultureInfo.InvariantCulture) + " " 
                    + _C.ToString("000.0", CultureInfo.InvariantCulture) 
                    + " comp set", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            else
            {
                log.Add("Wrong parameter!");
            }
            return pumpStatus;
        }

        private enuPumpStatus SetProgramStepFlowRate(double _time, double _flowrate) // time in minutes
        {
            if ((_time <= 999.9) && (_time >= 0) && (_flowrate <= 10.0) && (_flowrate >= 0))
            {
                if (sendCommand(_time.ToString("F2") + " " + _flowrate.ToString("F3") + " flow set", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            else
            {
                log.Add("Wrong parameter!");
            }
            return pumpStatus;
        }

        private enuPumpStatus SetProgramStepValve(double _time, int _valve) // time in minutes
        {
            if ((_time <= 999.9) && (_time >= 0) && (_valve <= 10) && (_valve >= 1))
            {
                if (sendCommand(_time.ToString("F2") + " " + _valve.ToString("F0") + " valve set", out res) != enuHWStatus.Ready)
                {
                    log.Add("COM communication failed!");
                    pumpStatus = enuPumpStatus.Error;
                }
            }
            else
            {
                log.Add("Wrong parameter!");
            }
            return pumpStatus;
        }

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
            try
            {
                serialAsyncReadBuf += _serialPort.ReadExisting();
            }
            catch (Exception ex)
            {
                log.Add(ex.ToString());
            }

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
            base.ParameterChanged(name);
        }

        public override void SerializeParameterValues(XmlWriter writer)
        {
            base.SerializeParameterValues(writer);
        }

        public override void DeserializeParameterValues(IXPathNavigable node, Dictionary<string, Type> availableExperiments)
        {
            base.DeserializeParameterValues(node, availableExperiments);
        }


    }
}

/*       

Communication command format: { value command set }
a) value
value is a numeric value set to the parameter specified by the command. All numeric values
are sent in ASCII codes.
b) command
command indicates the parameter to which the numeric value in setting value is to be set.
Both the value and command are specified entirely in ASCII codes as shown in the following
example:
Example: Set flowrate to 1.000 ml/min.
value command
{ 1.000 flowrate set }
Note: Communication command is terminated with CR (0x0d). Any LF (0x0a) code included in a
3
series of the commands are ignored. Data(value and command) is delimited by space (0x20).
[1] [.] [0] [0] [0] [0x20] [f] [l] [o] [w] [r] [a] [t] [e] [0x20] [s] [e] [t] [0x0d]
c) Reading value
With the exception of a few parameter values, value can be read with the following command
format:
{ command load p }
Example: Read current flowrate. { a_flow load p }
The PU-2080 will return the set parameter value when the command is received. These values
are sent in ASCII code with CR (0x0d) and LF (0x0a) added to the end. Number characters are
returned as number characters within the setting ranges.
Note: Be careful with the number zero since it is not suppressed.

When the PU experiences a problem during operation, it will automatically transmit
an appropriate corresponding trouble message to the remote controller.
Over Press %%[TROUBLE OVER PRESS]%%
Under Press %%[TROUBLE UNDER PRESS]%%
Pump Timer Off %%[PUMP STOP TIMER OFF]%%
Pump off input %%[TROUBLE EXT STOP]%%
Motor step out %%[TROUBLE EXT STOP]%%
Leak %%[TROUBLE EXT STOP]%%
Valve error %%[TROUBLE EXT STOP]%%

*/
