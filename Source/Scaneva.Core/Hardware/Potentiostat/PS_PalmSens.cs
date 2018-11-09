using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using PalmSens;
using PalmSens.Comm;
using static PalmSens.Comm.CommManager;
#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PS_PalmSens.cs" company="Scaneva">
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

using PalmSens.Devices;
using PalmSens.Plottables;
using PalmSens.Techniques;
using PalmSens.Windows;
using PalmSens.Windows.Devices;

using Scaneva.Core.Settings;
using Scaneva.Tools;


namespace Scaneva.Core.Hardware
{
    [DisplayName("PalmSens Potentiostat")]
    [Category("Potentiostats")]
    public class PS_PalmSens : ParametrizableObject, IHWManager, ITransducer
    {
        private static List<Device> deviceList = null;
        private static DateTime lastDeviceRefresh = DateTime.Now;

        public enuHWStatus HWStatus => enuHWStatus.Ready;
        public List<CurrentRange> SupportedRanges;

        public List<TransducerChannel> channels = new List<TransducerChannel>();

        public bool IsEnabled { get; set; }

        /// <summary>
        /// The CommManager takes care of all communications with the device
        /// </summary>
        private CommManager Comm = null;
        private Thread CommManagerThread;
        private Device device = null;
        private DeviceCapabilities capabilities = null;


        public PS_PalmSens(LogHelper log)
            : base(log)
        {
            settings = new PS_PalmSens_Settings();

            log.Add("Initializing PalmSens CoreDependencies");
            CoreDependencies.Init();

            refreshDeviceList();


        }

        private void refreshDeviceList()
        {
            string error = String.Empty;

            bool bNeedRefresh = (DateTime.Now - lastDeviceRefresh).TotalSeconds > 30; // Refresh after 30 Seconds

            if ((deviceList == null) || (bNeedRefresh))
            {
                lastDeviceRefresh = DateTime.Now;

                deviceList = FTDIDevice.DiscoverDevices(ref error);
                if (error != String.Empty)
                {
                    log.Add("Error calling FTDIDevice.DiscoverDevices: " + error);
                }

                deviceList.AddRange(USBCDCDevice.DiscoverDevices(ref error));
                if (error != String.Empty)
                {
                    log.Add("Error calling USBCDCDevice.DiscoverDevices: " + error);
                }

                deviceList.AddRange(SerialPortDevice.DiscoverDevices(ref error));
                if (error != String.Empty)
                {
                    log.Add("Error calling SerialPortDevice.DiscoverDevices: " + error);
                }
            }

            Settings.ListofConnections = deviceList.Select(x => ((x.ToString().StartsWith("PalmSens4") || (x.ToString().StartsWith("PalmSens3"))) ? x.ToString() : (Regex.Replace(x.ToString(), @" \[[0-9]+\]", "")))).ToArray();
        }

        public PS_PalmSens_Settings Settings
        {
            get
            {
                return (PS_PalmSens_Settings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public override void ParameterChanged(string name)
        {
            switch (name)
            {
                case "Settings Loaded":
                    refreshDeviceList();
                    tryFetchCurrentRanges();
                    break;

                case "Sonstiges.Connection":
                    tryFetchCurrentRanges();
                    break;

                default:
                    break;
            }

        }

        private void FetchCurrentRanges(CommManager lComm)
        {
            SupportedRanges = lComm.Capabilities.SupportedRanges;
            Settings.ListofCurrentRanges = SupportedRanges.Select(x => x.ToString()).ToArray();

            //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            //lComm.ClientConnection.Run(() =>
            //{
            //    try
            //    {
            //        SupportedRanges = lComm.Capabilities.SupportedRanges;
            //        Settings.ListofCurrentRanges = SupportedRanges.Select(x => x.ToString()).ToArray();
            //        tcs.SetResult(true);
            //    }
            //    catch (Exception e)
            //    {
            //        tcs.SetResult(false);
            //        tcs.SetException(e);
            //    }
            //});

            //tcs.Task.Wait();
            //if ((!tcs.Task.Result) && (tcs.Task.Exception != null))
            //{
            //    throw tcs.Task.Exception;
            //}
        }

        private void tryFetchCurrentRanges()
        {
            try
            {
                if ((SupportedRanges != null) && (SupportedRanges.Count > 0))
                {
                    Settings.ListofCurrentRanges = SupportedRanges.Select(x => x.ToString()).ToArray();
                }
                else
                {
                    if (Comm != null)
                    {
                        FetchCurrentRanges(Comm);
                    }
                    else
                    {
                        // Get Connection Name from Settings and find according Device
                        string connName = Settings.Connection;

                        string error = String.Empty;

                        refreshDeviceList();

                        Device device = deviceList.Where(x => (((x.ToString().StartsWith("PalmSens4") || (x.ToString().StartsWith("PalmSens3"))) ? x.ToString() : (Regex.Replace(x.ToString(), @" \[[0-9]+\]", ""))) == connName)).FirstOrDefault() as Device;

                        device.Open(); //try to open this COM port

                        CommManager lComm = new CommManager(device);

                        FetchCurrentRanges(lComm);

                        lComm.Disconnect();
                        lComm.Dispose();

                        device.Close();
                    }
                }
            }
            catch (Exception e)
            {
                log.Warning(e.Message);
            }
        }

        public enuHWStatus Initialize()
        {
            log.Add("PS_PalmSens.Initialize");

            refreshDeviceList();

            // Get Connection Name from Settings and find according Device
            string connName = Settings.Connection;
            device = deviceList.Where(x => (((x.ToString().StartsWith("PalmSens4") || (x.ToString().StartsWith("PalmSens3"))) ? x.ToString() : (Regex.Replace(x.ToString(), @" \[[0-9]+\]", ""))) == connName)).FirstOrDefault() as Device;

            if (device == null)
            {
                log.Add("No device on: " + connName, "Error");
                return enuHWStatus.Error;
            }

            log.Add("Connecting Device " + device.ToString() + "...");
            try
            {
                device.Open(); //try to open this COM port
            }
            catch (Exception ex)
            {
                log.Add("Cannot open: " + ex.Message, "Error");
                return enuHWStatus.Error;
            }

            log.Add("Opened " + device.ToString() + " with Baudrate = " + device.Baudrate);

            try
            {
                TaskCompletionSource<Exception> commSarted = new TaskCompletionSource<Exception>();
                CommManagerThread = new Thread(() =>
                {
                    try
                    {
                        Comm = new CommManager(device, 3000);
                        commSarted.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        commSarted.SetResult(ex);
                    }

                });
                CommManagerThread.Start();

                commSarted.Task.Wait();

                if (commSarted.Task.Result != null)
                {
                    // rethrow exception of inner task
                    throw commSarted.Task.Result;
                }

                log.Add("CommManagerThread.IsAlive: " + CommManagerThread.IsAlive);
            }
            catch (Exception ex)
            {
                device.Close();
                log.Add("Cannot connect: " + ex.Message + "\r\n" + ex.StackTrace, "Error");
                return enuHWStatus.Error;
            }

            log.Add("Connected to " + device.ToString());

            //Fill list with available current ranges
            tryFetchCurrentRanges();
            StartManualControl();

            Comm.ReceiveStatus -= Comm_ReceiveStatus;
            Comm.EndMeasurement -= Comm_EndMeasurement;
            Comm.StateChanged -= Comm_StateChanged;
            Comm.UnknownDataEvent -= Comm_UnknownDataEvent;
            Comm.CommErrorOccorred -= Comm_CommErrorOccorred;

            //Add Comm events
            Comm.ReceiveStatus += Comm_ReceiveStatus;
            Comm.EndMeasurement += Comm_EndMeasurement;
            Comm.StateChanged += Comm_StateChanged;
            Comm.UnknownDataEvent += Comm_UnknownDataEvent;
            Comm.CommErrorOccorred += Comm_CommErrorOccorred;

            capabilities = Comm.Capabilities;

            InitTransducerChannels();

            return enuHWStatus.Ready;
        }

        private void StartManualControl()
        {
            CurrentRange range = SupportedRanges.First(x => x.ToString() == Settings.ManualControlCurrentRange);
            Comm.CurrentRange = range;
            Comm.Potential = 0.0f;
            Comm.CellOn = false; //todo: depending on what is being recorded the cell has to be switched on or off
        }

        #region eventForwarding
        /// <summary>
        /// Forward Events from CommManager to any subscribers of PS_PalmSens (e.g. Experiments)
        /// </summary>
        public event StatusEventHandler ReceiveStatus
        {
            add { Comm.ReceiveStatus += value; }
            remove { Comm.ReceiveStatus -= value; }
        }

        public event BeginMeasurementEventHandler BeginMeasurement
        {
            add { Comm.BeginMeasurement += value; }
            remove { Comm.BeginMeasurement -= value; }
        }

        public event EventHandler EndMeasurement
        {
            add { Comm.EndMeasurement += value; }
            remove { Comm.EndMeasurement -= value; }
        }

        public event StatusChangedEventHandler StateChanged
        {
            add { Comm.StateChanged += value; }
            remove { Comm.StateChanged -= value; }
        }

        public event CurveEventHandler BeginReceiveCurve
        {
            add { Comm.BeginReceiveCurve += value; }
            remove { Comm.BeginReceiveCurve -= value; }
        }

        public event EISDataEventHandler BeginReceiveEISData
        {
            add { Comm.BeginReceiveEISData += value; }
            remove { Comm.BeginReceiveEISData -= value; }
        }
        #endregion

        private void Comm_UnknownDataEvent(object sender, CommManager.UnknownDataEventArgs e)
        {
            log.Add("PamSensHW " + Name + " - UnknownDataEvent [" + e.Data + "]", "Error");
        }

        private void Comm_CommErrorOccorred(Exception e)
        {
            log.Add("PamSensHW " + Name + " - CommErrorOccorred [" + e.ToString() + "]", "Error");
        }

        private void Comm_ReceiveStatus(object sender, StatusEventArgs e)
        {
            Status stat = e.GetStatus();
            if (stat != null)
            {
                PalmSens.Data.CurrentReading cReading = stat.CurrentReading;
                PalmSens.Data.VoltageReading pReading = stat.PotentialReading;

                string txtPotential = pReading.GetFormattedValue();
                string txtCurrent = cReading.ReadingStatus.ToString();
                if (cReading.ReadingStatus == ReadingStatus.OK)
                {
                    txtCurrent = $"{cReading.ValueInRange:0.000}";
                }
                string txtCR = $"{cReading.CurrentRange}";
                string txtStatus = $"{cReading.ReadingStatus}";
                log.Add("PamSensHW " + Name + " - Status [Potential = " + txtPotential + "V, Current = " + txtCurrent + " * " + txtCR + ", Status = " + txtStatus + "]");
            }
        }

        private void Comm_EndMeasurement(object sender, EventArgs e)
        {
            log.Add("PamSensHW " + Name + " - Measurement ended");
        }

        private void Comm_StateChanged(object sender, CommManager.DeviceState CurrentState)
        {
            log.Add("PamSensHW " + Name + " - Instrument state: " + CurrentState);
        }

        public enuHWStatus Connect()
        {
            return enuHWStatus.Ready;
        }

        public string Measure(Method m)
        {
            string errors = m.ValidationErrors(Comm.Capabilities);

            if (String.IsNullOrEmpty(errors))
            {
                log.Add("PamSensHW " + Name + " - Starting: " + m.GetType().Name);
                try
                {
                    TaskCompletionSource<string> measurementStarted = new TaskCompletionSource<string>();

                    //if (Comm.DeviceType == enumDeviceType.PalmSens4)
                    //{
                    //    errors = Comm.Measure(m);
                    //}
                    Comm.ClientConnection.Run(() =>
                    {
                        try
                        {
                            measurementStarted.SetResult("");
                            Comm.Measure(m);
                        }
                        catch (Exception e)
                        {
                            measurementStarted.SetResult(e.Message);
                        }
                    });

                    measurementStarted.Task.Wait();
                    errors = measurementStarted.Task.Result;
                }
                catch (Exception e)
                {
                    errors = e.Message;
                    log.Error("PamSensHW " + Name + " - Exception in Comm.Measure(): " + e.Message);
                }
            }
            else
            {
                log.Error("PamSensHW " + Name + " - Measurement Paramater Validation Error: " + errors);
            }
            return errors;
        }

        public void AbortMeasurement()
        {
            if (Comm != null)
            {
                Comm.Abort();
            }
        }

        public void Release()
        {
            if (Comm != null)
            {
                Comm.Disconnect();
                Comm.Dispose();
                Comm = null;
            }

            if (device != null)
            {
                device.Close();
            }
        }

        private void InitTransducerChannels()
        {
            channels = new List<TransducerChannel>();
            channels.Add(new TransducerChannel(this, "Potential", "V", enuPrefix.none, enuChannelType.mixed, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Current", "A", enuPrefix.µ, enuChannelType.mixed, enuSensorStatus.OK));            
            channels.Add(new TransducerChannel(this, "Current Range", "-1 .. 7", enuPrefix.none, enuChannelType.active, enuSensorStatus.OK));
            channels.Add(new TransducerChannel(this, "Cell On", "On (1)/Off (0)", enuPrefix.none, enuChannelType.active, enuSensorStatus.OK));

            if (capabilities.BiPotPresent)
            {
                channels.Add(new TransducerChannel(this, "Potential Bi-Pot", "A", enuPrefix.none, enuChannelType.active, enuSensorStatus.OK));
                channels.Add(new TransducerChannel(this, "Current Bi-Pot", "A", enuPrefix.µ, enuChannelType.passive, enuSensorStatus.OK));
                channels.Add(new TransducerChannel(this, "Current Range Bi-Pot", "-1 .. 7", enuPrefix.none, enuChannelType.active, enuSensorStatus.OK));
                channels.Add(new TransducerChannel(this, "Cell On Bi-Pot", "On (1)/Off (0)", enuPrefix.none, enuChannelType.active, enuSensorStatus.OK));
            }
        }

        public enuTransducerType TransducerType => enuTransducerType.Potentiostat;

        public List<TransducerChannel> Channels { get => channels; }

        public double GetValue(TransducerChannel channel)
        {
            if ((Comm != null) && Comm.Active && (channel != null))
            {
                float value = float.NaN;
                try
                {
                    Task<float> t = null;
                    switch (channel.Name)
                    {
                        case "Potential":
                            t = new Task<float>(() => { return (Comm?.Potential).GetValueOrDefault(float.NaN); });
                            break;

                        case "Current":
                            t = new Task<float>(() => { return (Comm?.Current).GetValueOrDefault(float.NaN); });
                            break;

                        case "Current Range":
                            t = new Task<float>(() => { return ((float?)Comm?.CurrentRange?.CRbyte).GetValueOrDefault(float.NaN); });
                            break;

                        case "Cell On":
                            t = new Task<float>(() => { return (Comm?.CellOn).GetValueOrDefault(false) ? 1.0f : 0.0f; });
                            break;

                        case "Potential Bi-Pot":
                            //t = new Task<float>(() => { return (Comm?. ??? ).GetValueOrDefault(float.NaN); });
                            break;

                        case "Current Bi-Pot":
                            t = new Task<float>(() => { return (float)(Comm?.ReadBiPotCurrent).GetValueOrDefault(double.NaN); });
                            break;                   

                        case "Current Range Bi-pot":
                            t = new Task<float>(() => { return ((float?)Comm?.BiPotCurrentRange?.CRbyte).GetValueOrDefault(float.NaN); });
                            break;

                        case "Cell On Bi-Pot":
                            t = new Task<float>(() => { return (Comm?.IsBipotOn).GetValueOrDefault(false) ? 1.0f : 0.0f; });
                            break;

                        default:
                            break;
                    }
                    Comm.ClientConnection.Run(t).Wait();
                    value = t.Result;
                }
                catch (Exception e)
                {
                    log.Add(e.Message, "Error");
                }
                return value;
            }
            return double.NaN;
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
                value += GetValue(channel);
            }

            return value / channel.Averaging;

        }

        public void SetValue(TransducerChannel channel, double _value)
        {
            if ((Comm != null) && Comm.Active && (channel != null))
            {
                switch (channel.Name)
                {
                    case "Potential":
                        Comm.Potential = (float)_value;
                        break;

                    case "Current":
                        Comm.Current = (float)_value;
                        break;

                    case "Current Range":
                        try
                        {
                            sbyte sbRange = Convert.ToSByte(_value);
                            CurrentRange cr = SupportedRanges.Find(x => (x.CRbyte == sbRange));
                            if (cr != null)
                            {
                                Comm.CurrentRange = cr;
                            }
                        }
                        catch
                        {
                        }
                        break;

                    case "Cell On":
                        Comm.CellOn = (_value != 0.0);  // 0 is off, everything else on
                        break;

                    case "Potential Bi-Pot":
                        Comm.BiPotPotential = (float)_value;
                        break;

                    case "Current Bi-Pot":
                        //Comm. ??? = (float)_value;
                        break;


                    case "Bi-pot current Range":
                        try
                        {
                            sbyte sbRange = Convert.ToSByte(_value);
                            CurrentRange cr = SupportedRanges.Find(x => (x.CRbyte == sbRange));
                            if (cr != null)
                            {
                                Comm.BiPotCurrentRange = cr;
                            }
                        }
                        catch
                        {
                        }
                        break;


                    case "Cell On Bi-pot":
                        if (_value == 0) Comm.SetBipotOff();
                        // ???  else Comm.SetBipotOnAndCurrentRange();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
