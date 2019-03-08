#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Generic_SerialConnection.cs" company="Scaneva">
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
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Scaneva.Core.Settings;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware
{
    /// <summary>
    /// Interface used to notify a Listener about new Messages from the Nano.
    /// </summary>
    public interface ISerialConnectionReceiveListener
    {
        void MessageReceived(string sMsg);
    }

    [DisplayName("Serial Connection")]
    public class Generic_SerialConnection : ParametrizableObject, IHWManager
    {
        // Pump
        private SerialPort _serialPort = null;
        private bool bPortConnected = false;
        private byte[] byteRxBuffer = new byte[1024];

        private StreamWriter logStreamWriter = null;
        private ISerialConnectionReceiveListener receiveListener = null;

        private string serialAsyncReadBuf = "";
        private bool bAsyncHandlingEnabled = true;

        //status
        private enuPumpStatus pumpStatus = enuPumpStatus.NotInitialized;
        private enuHWStatus hwStatus = enuHWStatus.NotInitialized;

        public Generic_SerialConnection(LogHelper log) : base(log)
        {
            settings = new Generic_SerialConnection_Settings();
            log.Add("Initializing Serial Connection");

            string[] ports = SerialPort.GetPortNames();
            Settings.ComPortList.AddRange(ports);

            //Settings.BaudRateList.AddRange(new int[] { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 });

            hwStatus = enuHWStatus.NotInitialized;
        }

        public Generic_SerialConnection_Settings Settings
        {
            get
            {
                return (Generic_SerialConnection_Settings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public void SetMessageListener(ISerialConnectionReceiveListener listener)
        {
            receiveListener = listener;
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
                // Connect Serial Port
                _serialPort = new SerialPort(Settings.COMPort, (int)Settings.Baudrate, Settings.Parity, Settings.Databits, Settings.StopBits);
                _serialPort.Handshake = Handshake.None;

                //Bind the events on the following event handler
                _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialErrorReceived);
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                _serialPort.ReadTimeout = Settings.ReceiveTimeout;

                _serialPort.Open();
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();

                // Mark connecion as open
                bPortConnected = true;
                bAsyncHandlingEnabled = true;

                hwStatus = enuHWStatus.Ready;
                log.Add(Name + ": opened connection on " + Settings.COMPort);
            }
            catch (Exception e)
            {
                log.Error(Name + ": " + e.Message);
                hwStatus = enuHWStatus.Error;
            }

            return hwStatus;
        }

        public void Release()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
        }

        public void SendMessage(string Message)
        {
            _serialPort.Write(Message);
        }

        /// <summary>
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
                bool bComplete = serialAsyncReadBuf.EndsWith(Settings.Termination.Replace("\\r", "\r").Replace("\\n", "\n"));
                string[] strs = serialAsyncReadBuf.Split(new string[] { Settings.Termination.Replace("\\r", "\r").Replace("\\n", "\n") }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < (strs.Length - 1); i++)
                {
                    //AppendToLogReceive(strs[i]);
                    receiveListener?.MessageReceived(strs[i]);
                }
                if (bComplete)
                {
                    //AppendToLogReceive(strs[strs.Length - 1]);
                    receiveListener?.MessageReceived(strs[strs.Length - 1]);
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
            log.Error(Name + ": " + e.ToString());
            hwStatus = enuHWStatus.Error;
        }

    }
}
