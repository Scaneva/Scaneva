#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Pump_Tecan_Centris.cs" company="Scaneva">
// 
//  Copyright (C) 2019 Roche Diabetes Care GmbH (Christoph Pieper)
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

using PumpCommServerTypeLib;

namespace Scaneva.Core.Hardware.Pump
{
    [DisplayName("Tecan Cavro Centris Pump")]
    [Category("Pumps")]
    public class Pump_Tecan_Centris : ParametrizableObject, IHWManager, ISyringePump, ITransducer
    {
        // Pump
        PumpComm mCommPort;

        //transducer stuff
        public List<TransducerChannel> channels = new List<TransducerChannel>();

        //status
        private enuPumpStatus pumpStatus = enuPumpStatus.NotInitialized;
        private enuHWStatus hwStatus = enuHWStatus.NotInitialized;

        public Pump_Tecan_Centris(LogHelper log) : base(log)
        {
            settings = new Pump_Tecan_Centris_Settings();
            log.Add("Initializing Tecan Cavro Pump");
            InitTransducerChannels();

            string[] ports = SerialPort.GetPortNames();
            Settings.ComPortList.AddRange(ports);

            hwStatus = enuHWStatus.NotInitialized;
        }

        public Pump_Tecan_Centris_Settings Settings
        {
            get
            {
                return (Pump_Tecan_Centris_Settings)settings;
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
                mCommPort = new PumpComm();
                mCommPort.LogComPort = true;
                mCommPort.PumpEnableLogWindow(1);

                mCommPort.BaudRate = Settings.Baudrate;
                mCommPort.PumpInitComm(byte.Parse(Settings.COMPort.Substring(3)));

                int devStatus = mCommPort.PumpCheckDevStatus(Settings.PumpAdress);
                string answerString = "";
                int plungerForce = 0;

                switch (devStatus)
                {
                    case 0:
                        log.Error("Pump " + Name + " adress " + Settings.PumpAdress + " on " + Settings.COMPort + " is busy.");
                        hwStatus = enuHWStatus.Busy;
                        break;

                    case 1:
                        

                        // mCommPort.PumpSendCommand("U11R", Settings.PumpAdress, ref answerString);
                        //mCommPort.PumpSendCommand("U41R", Settings.PumpAdress, ref answerString);
                        // mCommPort.PumpSendCommand("U8R", Settings.PumpAdress, ref answerString);
                        //mCommPort.PumpSendCommand("U31R", Settings.PumpAdress, ref answerString);
                          mCommPort.PumpSendCommand("zR", Settings.PumpAdress, ref answerString);
                          mCommPort.PumpSendCommand("U30R", Settings.PumpAdress, ref answerString);
                      //  mCommPort.PumpSendCommand("W1R", Settings.PumpAdress, ref answerString);
                        if (SyringeVolume < 1000)
                        {
                            plungerForce = 1; //Initializes at half plunger force and at default initialization speed
                        }
                        if (SyringeVolume <= 100)
                        {
                            plungerForce = 2; //Initializes at one-third plunger force and at default initialization speed                                         
                        }

                        //  mCommPort.PumpSendCommand("N0R", Settings.PumpAdress, ref answerString);

                        if (Settings.InitializationValvePolarity == enValveMode.Clockwise)
                        {
                            //Initialize Plunger and Valve Drive(CW Polarity)
                            mCommPort.PumpSendCommand("Z" + plungerForce + "," + Settings.InitializationInputPort + "," + Settings.InitializationOutputPort + "R", Settings.PumpAdress, ref answerString);  // R for Execute.

                        }
                        else
                        {
                            //Initialize Plunger and Valve Drive (CCW Polarity)
                            mCommPort.PumpSendCommand("Y" + plungerForce + "," + Settings.InitializationInputPort + "," + Settings.InitializationOutputPort + "R", Settings.PumpAdress, ref answerString);  // R for Execute.
                        }




                        log.Add("Pump " + Name + " adress " + Settings.PumpAdress + " on " + Settings.COMPort + " was successfully initialized.");

                        hwStatus = enuHWStatus.Ready;
                        break;

                    default:
                        log.Error("Pump " + Name + " adress " + Settings.PumpAdress + " on " + Settings.COMPort + " can not be initialized. Check connections.");
                        hwStatus = enuHWStatus.Error;
                        break;
                }

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
            mCommPort.PumpExitComm();
        }

        public double SyringeVolume
        {
            get
            {
                double volume = double.NaN;
                switch (Settings.Volume)
                {
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_250uL:
                        volume = 250;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_1250uL:
                        volume = 1250;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_5000uL:
                        volume = 5000;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_50uL:
                        volume = 50;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_100uL:
                        volume = 100;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_500uL:
                        volume = 500;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_1000uL:
                        volume = 1000;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_2500uL:
                        volume = 2500;
                        break;
                    case Pump_Tecan_Centris_Settings.SyringeVolume.Volume_12500uL:
                        volume = 12500;
                        break;
                    default:
                        break;
                }
                return volume;
            }
        }

        public byte MaxPortNum
        {
            get
            {
                byte max = 0;
                switch (Settings.Valve)
                {
                    case Pump_Tecan_Centris_Settings.ValveType.NoValve:
                        break;
                    case Pump_Tecan_Centris_Settings.ValveType.Valve_3way:
                        max = 2;
                        break;
                    case Pump_Tecan_Centris_Settings.ValveType.Valve_4way:
                        max = 3;
                        break;
                    case Pump_Tecan_Centris_Settings.ValveType.Valve_6port:
                        max = 6;
                        break;
                    case Pump_Tecan_Centris_Settings.ValveType.Valve_9port:
                        max = 9;
                        break;
                    case Pump_Tecan_Centris_Settings.ValveType.Valve_4port_dl:
                        max = 3;
                        break;
                    case Pump_Tecan_Centris_Settings.ValveType.Valve_3port:
                        max = 3;
                        break;
                    case Pump_Tecan_Centris_Settings.ValveType.Valve_12port:
                        max = 12;
                        break;
                    default:
                        break;
                }
                return max;
            }
        }

        public enValveMode ValveMode { get; set; } = enValveMode.Clockwise;

        public string ValvePosition //starts with 1
        {
            get
            {
                string res = "error";
                if (hwStatus == enuHWStatus.Ready)
                {
                    string answerString = "";
                    try
                    {
                        mCommPort.PumpSendCommand("?20", Settings.PumpAdress, ref answerString);
                        return answerString;
                    }
                    catch (Exception e)
                    {
                        log.Error(e.Message);
                    }
                }

                return res;
            }

            set
            {
                if (hwStatus == enuHWStatus.Ready)
                {
                    bool isByte = Byte.TryParse(value, out byte portNum);
                    if (((isByte) && (portNum > 0) && (portNum <= MaxPortNum)) || ((value == "I" || value == "O" || value == "B" || value == "E")))
                    {
                        string answerString = "";
                        try
                        {
                            if (!isByte)
                            {
                                mCommPort.PumpSendCommand(value + "R", Settings.PumpAdress, ref answerString);
                            }
                            else
                            {
                                string command = (ValveMode == enValveMode.Clockwise) ? "I" : "O";
                                mCommPort.PumpSendCommand(command + value + "R", Settings.PumpAdress, ref answerString);
                            }
                            log.Add(Name + ": Valve moved to position " + value + " - answer: " + answerString);
                        }
                        catch (Exception e)
                        {
                            log.Error(e.Message);
                        }
                    }
                    else
                    {
                        log.Error("Invalid ValvePostion value: " + value + " (allowed are 1 - " + MaxPortNum + ", I, O, B and E");
                    }

                }
            }
        }

        public double Speed //µl/s!
        {
            get
            {
                double res = double.NaN;
                if (hwStatus == enuHWStatus.Ready)
                {
                    string answerString = "";
                    try
                    {
                        mCommPort.PumpSendCommand("?37", Settings.PumpAdress, ref answerString);
                        return double.Parse(answerString, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        log.Error(e.Message);
                    }
                }

                return res;
            }

            set
            {
                if (hwStatus == enuHWStatus.Ready)
                {
                    if ((value < Math.Max(0.001, SyringeVolume / 1e5)) || (value > (SyringeVolume * 1.1)))
                    {
                        log.Error("Cannot set Speed to value smaler " + Math.Max(0.001, SyringeVolume / 1e5).ToString("0:0.###") + " or value greater " + (SyringeVolume * 1.1).ToString("0:0.###"));
                    }
                    else
                    {
                        string answerString = "";
                        try
                        {
                            // V<n1>, <n2> Set Top Speed
                            // <n2> = 1 : Set top speed in microliters per second. Up to 3 decimal places are allowed.
                            // The equivalent speed in Inc/Sec is calculated based on the configured syringe volume and rounded to 1 decimal place. This is set to the new top speed.
                            mCommPort.PumpSendCommand("V" + value.ToString("0:0.###") + ",1R", Settings.PumpAdress, ref answerString);
                            log.Add(Name + ": Plunger Top speed set to " + value.ToString("0:0.###") + " µL/s - answer: " + answerString);
                        }
                        catch (Exception e)
                        {
                            log.Error(e.Message);
                        }
                    }
                }
            }
        }

        public double RelativePickup //µl
        {
            set
            {
                if (hwStatus == enuHWStatus.Ready)
                {
                    if ((value < 0) || (value > SyringeVolume))
                    {
                        log.Error("Cannot pick up a negative volume or volume greater SyringeVolume");
                    }
                    else
                    {
                        string answerString = "";
                        try
                        {
                            double steps = (Settings.Mode == Pump_Tecan_Centris_Settings.MicrostepMode.Normal) ? (value * 6000 / SyringeVolume) : (value * 4800 / SyringeVolume);

                            // P<n1>, <n2> Relative Pickup
                            // <n2> = 1 Relative position in microliters. Up to 3 decimal places are allowed.
                            mCommPort.PumpSendCommand("P" + steps.ToString() + "R", Settings.PumpAdress, ref answerString);
                            log.Add(Name + ": Plunger picked up " + value.ToString("") + " µL - answer: " + answerString);
                        }
                        catch (Exception e)
                        {
                            log.Error(e.Message);
                        }
                    }
                }
            }
        }

        public double RelativeDispense //µl
        {
            set
            {
                if (hwStatus == enuHWStatus.Ready)
                {
                    if ((value < 0) || (value > SyringeVolume))
                    {
                        log.Error("Cannot dispense a negative volume or volume greater SyringeVolume");
                    }
                    else
                    {
                        string answerString = "";
                        try
                        {
                            double steps = (Settings.Mode == Pump_Tecan_Centris_Settings.MicrostepMode.Normal) ? (value * 6000 / SyringeVolume) : (value * 4800 / SyringeVolume);
                            // D<n1>, <n2> Relative Dispense
                            // <n2> = 1 Relative position in microliters. Up to 3 decimal places are allowed.
                            mCommPort.PumpSendCommand("D" + steps.ToString() + "R", Settings.PumpAdress, ref answerString);
                            log.Add(Name + ": Plunger dispense " + value.ToString("0:0.###") + " µL - answer: " + answerString);
                        }
                        catch (Exception e)
                        {
                            log.Error(e.Message);
                        }
                    }
                }
            }
        }

        public double PlungerPosition //µl
        {
            get
            {
                double res = double.NaN;
                if (hwStatus == enuHWStatus.Ready)
                {
                    string answerString = "";
                    try
                    {
                        mCommPort.PumpSendCommand("?18", Settings.PumpAdress, ref answerString);
                        return double.Parse(answerString, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        log.Error(e.Message);
                    }
                }

                return res;
            }

            set
            {
                if (hwStatus == enuHWStatus.Ready)
                {
                    if ((value < 0) || (value > SyringeVolume))
                    {
                        log.Error("Cannot set PlungerPostion to negative value or value greater SyringeVolume");
                    }
                    else
                    {
                        string answerString = "";
                        try
                        {
                            // A<n1>, <n2> Absolute Position
                            // <n2> = 1 Absolute position in microliters. Up to 3 decimal places are allowed.
                            double steps = (Settings.Mode == Pump_Tecan_Centris_Settings.MicrostepMode.Normal) ? (value * 6000 / SyringeVolume) : (value * 4800 / SyringeVolume);
                            mCommPort.PumpSendCommand("A" + steps.ToString() + "R", Settings.PumpAdress, ref answerString);
                            log.Add(Name + ": Plunger moved to position " + value.ToString() + " µL - answer: " + answerString);
                        }
                        catch (Exception e)
                        {
                            log.Error(e.Message);
                        }
                    }
                }
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

        public double GetValue(TransducerChannel channel)
        {
            switch (channel.Name)
            {
                case "Valve Position":
                    double.TryParse(ValvePosition, out double valvePos);
                    return valvePos;
                case "Speed":
                    return Speed;
                case "Plunger Position":
                    return PlungerPosition;

                default:
                    return 0;
            }
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
            switch (channel.Name)
            {
                case "Valve Position":
                    ValvePosition = _value.ToString();
                    break;

                case "Speed":
                    Speed = _value;
                    break;

                case "Plunger Position":
                    PlungerPosition = _value;
                    break;

                case "Relative Pickup":
                    RelativePickup = _value;
                    break;

                case "Relative Dispense":
                    RelativeDispense = _value;
                    break;

                default:
                    return 0;
            }
            return enuTChannelStatus.OK;
        }
    }
}
