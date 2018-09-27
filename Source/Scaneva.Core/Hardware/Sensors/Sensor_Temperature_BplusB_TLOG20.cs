#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Sensor_Temperature_BplusB_TLOG20.cs" company="Scaneva">
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
    [DisplayName("B+B temperature sensor")]
    [Category("Sensors")]
    class Sensor_Temperature_BplusB_TLOG20 : ParametrizableObject, IHWManager, ITransducer
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
            //todo auto eacho parameter
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            //Bind the events on the following event handler
            _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialErrorReceived);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            if (true)
            {
                InitTransducerChannels();
                hwStatus = enuHWStatus.Ready;
            }
            else
            {
                log.Add("Could not initiate sensor!", "Error");
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

        public Sensor_Temperature_BplusB_TLOG20(LogHelper log) : base(log)
        {
            settings = new Sensor_Temperature_BplusB_TLOG20_Settings();
            log.Add("Initializing B+B temperature sensor TLOG20");         
        }

        public Sensor_Temperature_BplusB_TLOG20_Settings Settings
        {
            get
            {
                return (Sensor_Temperature_BplusB_TLOG20_Settings)settings;
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
            channels.Add(new TransducerChannel(this, "Temperature", "°C", enuPrefix.none, enuChannelType.passive, enuSensorStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.Sensor;

        public List<TransducerChannel> Channels { get => channels; }

        public double GetValue(TransducerChannel channel)
        {
            switch (channel.Name)
            {
                case "Temperature":
                    return Temperature;
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
           //noop;
        }

        private double Temperature //°C
        {
            get
            {
                string res = "";
                    return Convert.ToDouble(res, CultureInfo.InvariantCulture);
                
            }
            set
            { }
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
 * Format der Datenübertragung
Die Schnittstelle arbeitet mit einer Datenrate von 4800 Baud, 8 Datenbits,
keiner Parität und einem Stopbit.
Die Übertragung der Nutzdaten erfolgt in Zeilen. Alle Zeichen sind
ASCII-codiert. Alle Informationen werden fortlaufend ohne Trennzeichen
gesendet.
In einer Zeile werden nur Informationen zu einem Kanal übertragen.
Am Ende einer Zeile steht in den letzten zwei ASCII-Zeichen die Prüfsumme
(CRC) der aktuellen Zeile. Jede Zeile schließt mit dem Zeichen
´Wagenrücklauf´ ´<CR>´ ab. Mehrere Zeilen bilden einen Datenblock.
Ein Datenblock kann beispielsweise folgenden Inhalt haben:

    @<CR>
    I011010E0223C000000B1<CR>
    V0108DA7D<CR>
    I02101050013C00000021<CR>
    V0208C276<CR>
    $<CR>


Der Datenblock hat folgenden Aufbau:
• Ein Synchronisationsmuster für den Beginn eines Datenblockes.
Zur Synchronisation dient die Sequenz ´@ <CR>´
• Die Konfigurationsdaten (´Identifier´) eines Kanals. Die Datenzeile
beginnt mit dem Zeichen ´I´ , gefolgt von der logischen Kanalnummer,
gefolgt von den Konfigurationsdaten und der SensorSeriennummer.
Die Zeile wird mit der Prüfsumme und mit dem
Zeichen ´<CR>´abgeschlossen.
• Die Messwerte eines Kanals. Die Datenzeile beginnt mit dem
Zeichen ´V´, gefolgt von der logischen Kanalnummer, gefolgt von
den Nutzdaten. Es werden nur die numerischen Messwerte sowie,
am Ende der Zeile, die Prüfsumme (CRC) übertragen. Alle
anderen Informationen wie Zahlenformat, Anzahl der Zeichen,
physikalische Einheit, etc. sind in den Konfigurationsdaten (Fühlerkennung)
enthalten.
• Die Konfigurationsdaten und Messwerte folgen im gleichen Schema
für jeden weiteren Kanal.
• Am Schluss eines Datenblocks wird die Zeichenfolge ´$´ <CR>´
gesendet


    Aufbau der Konfig-Datenzeile
Die Konfigurations-Datenzeile enthält alle Informationen zu dem am
entsprechenden Kanal betriebenen Sensor. Die Zeile hat folgenden
Aufbau:
• Kennbuchstaben ´I´ am Beginn der Zeile.
• 8 Bit (zwei ASCII-Zeichen) logische Kanalnummer. Die logische
Kanalnummer dient dazu, die Konfigurationsdaten den Messwerten
zuzuordnen. Die Kanalnummer wird im Gerät erzeugt. Der
erste Kanal besitzt die Nummer 01. Es werden maximal 20 Kanäle
übertragen. Die Nummern werden fortlaufend vergeben.
• 8 Bit (zwei ASCII-Zeichen) Hardware-Kennung (Typ des Messfühlers).
Die Dallas-Sensoren haben die Kennziffer 10 für 1820
und 28 (HEX) für 18B20.
• 48 bit (zwölf ASCII-Zeichen) Seriennummer des Sensors. Hier
wird die Seriennummer des Dallas-Sensors ausgegeben.
• 8 Bit (zwei ASCII-Zeichen) CRC (Prüfsumme)
• ´<CR>´als Zeilenabschluss
Aufbau der Messwert-Datenzeilen
Die Messwerte-Datenzeile enthält die aktuellen Messwerte zu dem am
entsprechenden Kanal betriebenen Sensor. Alle Informationen sind
binär dargestellt und werden ASCII-codiert ohne Trennzeichen übertragen.
Für die Fühlerkennung 01 hat die Zeile folgenden Aufbau:
• Kennbuchstaben ´V´ am Beginn der Zeile
• 8 Bit (zwei ASCII-Zeichen) logische Kanalnummer
• 2 Byte (4 ASCII-Zeichen) Messdaten in 0,01 °C Auflösung. Der
hexadezimale Wert in 2´s competent Format ist in eine dezimale
Zahl umzuwandeln und durch 100 zu teilen. Damit erhält man
den Temperaturwert in °C mit zwei Nachkommastellen.
• 8 Bit (zwei ASCII-Zeichen) Prüfsumme (CRC)
• ´<CR>´als Zeilenabschluss 

    Inbetriebnahme des Geräts
Bei der ersten Inbetriebnahme muss das System einmalig konfiguriert
werden. Dazu werden zunächst alle Sensoren angeschlossen und die
Betriebsspannung angelegt.
• Nachdem der Konfigurationstaster auf der Platine dreimal kurz
(jeweils ca. 1,5 Sekunde drücken, 1 Sekunde Pause) gedrückt
wurde, verzweigt das Programm in den Autosearch-Modus und
die am Bus angeschlossenen Sensoren werden gesucht. Die
LED blinkt zyklisch dreimal kurz hintereinander. Eine Liste mit
den Serien-Nummern der Sensoren wird per Schnittstelle übertragen,
dann stoppt das Gerät die Kommunikation. Man muss
entweder die Konfiguration speichern oder das Gerät neu starten.
Wird die Servicetaste nun für mindestens 5 Sekunden betätigt,
wird die neue Konfiguration im Speicher des Controllers
abgelegt. Die LED leuchtet während der Speicherung länger.
Der Vorgang kann beliebig oft wiederholt werden. Nachdem die
Konfiguration gespeichert ist, startet das Gerät die Datensammlung
Es erscheinen die aktuellen Temperaturwerte auf dem Bildschirm.
• Wenn Sie sich nach dem Autosearch-Modus enscheiden, die Konfiguration
nicht zu speichern, muss die Betriebsspannung kurzzeitig
abgeschalten werden. Bei Neustart des Geräts ist die alte
Konfiguration noch gespeichert.
    */
