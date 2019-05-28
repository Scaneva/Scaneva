#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="AirTrackerExperiment.cs" company="Scaneva">
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
using System.Linq;
using System.Threading.Tasks;
using Scaneva.Core.ExperimentData;
using Scaneva.Tools;
using System.Threading;
using System.Diagnostics;
using Scaneva.Core.Hardware;
using System.Collections.Concurrent;

namespace Scaneva.Core.Experiments.DataAcquisition
{
    public class AirTrackerExperiment : ExperimentBase, IExperiment, ISerialConnectionReceiveListener
    {
        public AirTrackerExperiment(LogHelper log)
            : base(log)
        {
            settings = new AirTrackerExperimentSettings();
            messageQueue = new ConcurrentQueue<MessageObject>();
        }

        public AirTrackerExperimentSettings Settings
        {
            get
            {
                return (AirTrackerExperimentSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        private class MessageObject
        {
            public DateTime TimeStamp;
            public string Message;

            public MessageObject(string message)
            {
                TimeStamp = DateTime.Now;
                Message = message;
            }
        }

        private Dictionary<string, Generic_SerialConnection> serialConnections;
        private ConcurrentQueue<MessageObject> messageQueue;

        private Generic_SerialConnection connection = null;

        public override Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                serialConnections = new Dictionary<string, Generic_SerialConnection>();

                var connections = value.Where(x => typeof(Generic_SerialConnection).IsAssignableFrom(x.Value.GetType())).Select(x => x).ToArray();

                // Iterate Transducers
                foreach (KeyValuePair<string, IHWManager> ele in connections)
                {
                    Generic_SerialConnection con = (Generic_SerialConnection)ele.Value;
                    // Add
                    serialConnections.Add(ele.Key, con);
                }

                //Settings.TransducerChannels = transducerChannels.Select(d => d.Value).ToList();
                Settings.SerialConnections = serialConnections;

                hwStore = value;
            }
        }

        protected bool abortExperiment;

        public override enExperimentStatus Abort()
        {
            status = enExperimentStatus.Aborted;
            abortExperiment = true;
            return status;
        }

        public override bool CheckParametersOk(out string errorMessage)
        {
            errorMessage = String.Empty;

            if (Settings.Duration <= 0)
            {
                errorMessage = "Configuration Error in '" + Name + "': Duration must be > 0";
                return false;
            }

            if ((Settings.SerialConnection == null) || (!serialConnections.ContainsKey(Settings.SerialConnection)))
            {
                errorMessage = "Configuration Error in '" + Name + "': invalid or no serial connection configured";
                return false;
            }

            connection = serialConnections[Settings.SerialConnection];

            return true;
        }

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            this.parent = parent;
            ResultsFilePath = resultsFilePath;
            ResultsFileName = "AirTrackerExperiment - " + Name + ".dat";

            status = enExperimentStatus.Idle;
            return status;
        }

        Task measurementTask = null;

        public override enExperimentStatus Run()
        {
            // Only Run if Experiment is Idle and Task not running
            if ((status == enExperimentStatus.Idle) && ((measurementTask == null) || (measurementTask.IsCompleted) || (measurementTask.IsCanceled)))
            {
                connection?.SetMessageListener(this);

                abortExperiment = false;
                status = enExperimentStatus.Running;

                // clear Queue
                while (messageQueue.TryDequeue(out MessageObject item))
                {
                    // do nothing
                }

                // stop measurement if running
                DateTime start = DateTime.Now;
                connection.SendMessage("S");
                string reply = "";

                while (((DateTime.Now - start).TotalMilliseconds < 2000) && (reply != "OK") && (reply != "Er"))
                {
                    Thread.Sleep(10);
                    MessageObject item;
                    if (messageQueue.TryDequeue(out item))
                    {
                        reply = item.Message;
                    }
                }

                //if ((reply != "OK") && (reply != "Er"))
                //{
                //    log.Error("Error communicating with AirTracker");
                //    return enExperimentStatus.Error;
                //}

                // turn Of potentiostat
                start = DateTime.Now;
                connection.SendMessage("F");
                reply = "";

                while (((DateTime.Now - start).TotalMilliseconds < 2000) && (reply != "OK") && (reply != "Er"))
                {
                    Thread.Sleep(10);
                    MessageObject item;
                    if (messageQueue.TryDequeue(out item))
                    {
                        reply = item.Message;
                    }
                }

                //if ((reply != "OK") && (reply != "Er"))
                //{
                //    log.Error("Error communicating with AirTracker");
                //    return enExperimentStatus.Error;
                //}

                // turn On potentiostat
                start = DateTime.Now;
                connection.SendMessage("V" + Settings.PolarizationVoltage.ToString("000"));
                reply = "";

                while (((DateTime.Now - start).TotalMilliseconds < 2000) && (reply != "OK"))
                {
                    Thread.Sleep(10);
                    MessageObject item;
                    if (messageQueue.TryDequeue(out item))
                    {
                        reply = item.Message;
                    }
                }

                if (reply != "OK")
                {
                    log.Error("Error set polarization voltage on AirTracker");
                    return enExperimentStatus.Error;
                }

                // set polarization voltage
                start = DateTime.Now;
                connection.SendMessage("N");
                reply = "";

                while (((DateTime.Now - start).TotalMilliseconds < 2000) && (reply != "OK"))
                {
                    Thread.Sleep(10);
                    MessageObject item;
                    if (messageQueue.TryDequeue(out item))
                    {
                        reply = item.Message;
                    }
                }

                if (reply != "OK")
                {
                    log.Error("Error communicating with AirTracker");
                    return enExperimentStatus.Error;
                }

                // single measurement
                start = DateTime.Now;
                connection.SendMessage("U");
                reply = "";

                while (((DateTime.Now - start).TotalMilliseconds < 2000) && (reply == ""))
                {
                    Thread.Sleep(10);
                    MessageObject item;
                    if (messageQueue.TryDequeue(out item))
                    {
                        reply = item.Message;
                    }
                }

                string[] results = reply.Split(new string[] { " " }, StringSplitOptions.None);
                if (results.Length != 10)
                {
                    log.Error("Error communicating with AirTracker");
                    return enExperimentStatus.Error;
                }

                string headerString = "Experiment: AirTrackerExperiment - " + Name + "\r\n";

                headerString += "Firmware Rev.: " + results[0] + "\r\n";
                headerString += "Device No.: " + results[1] + "\r\n";
                headerString += "Board No.: " + results[2] + "\r\n";

                List<string> dataColumnHeaders = new List<string>();

                dataColumnHeaders.Add("Time [s]");
                dataColumnHeaders.Add("Polariasation Voltage [mV]");
                dataColumnHeaders.Add("Sensor Current [nA]");
                dataColumnHeaders.Add("Regulation Voltage [mV]");
                dataColumnHeaders.Add("Impedance [Ohm]");

                writeHeader(headerString, dataColumnHeaders.ToArray(), positionColumns: false);

                measurementTask = Task.Factory.StartNew(new Action(ExecuteTimeTraceMeasurement), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                return status;
            }
            log.Add("Experiment not Configured or already running", "Error");
            return enExperimentStatus.Error;
        }

        private Generic2DExperimentData data = null;

        private static readonly int BUFFER_LENGTH = 1024;

        private void ExecuteTimeTraceMeasurement()
        {
            double duration = Settings.Duration;
            data = new Generic2DExperimentData();

            string[] chans = new string[] { "Polariasation Voltage", "Sensor Current", "Regulation Voltage", "Impedance" };
            string[] unit = new string[] { "mV", "nA", "mV", "Ohm" };

            for (int j = 0; j < 3; j++)
            {
                data.datasetNames.Add(chans[j]);
                data.axisNames.Add(new string[] { "Time", chans[j] });
                data.axisUnits.Add(new string[] { "s", unit[j] });

                double[][] dataBuffer = new double[2][] { new double[BUFFER_LENGTH], new double[BUFFER_LENGTH] };

                // initialize the arrays
                for (int i = 0; i < BUFFER_LENGTH; i++)
                {
                    dataBuffer[0][i] = double.NaN;
                    dataBuffer[1][i] = double.NaN;
                }

                data.data.Add(dataBuffer);
            }

            // start measurement
            DateTime start = DateTime.Now;
            connection.SendMessage("M");

            bool bFirstData = true;
            int currentBufferIdx = 0;
            while (((DateTime.Now - start).TotalMilliseconds < (duration * 1000)) && (!abortExperiment))
            {
                MessageObject item;
                if (messageQueue.TryDequeue(out item))
                {
                    string[] results = item.Message.Split(new string[] { " " }, StringSplitOptions.None);
                    if (results.Length == 10)
                    {
                        double[] values = new double[5];

                        values[0] = (item.TimeStamp - start).TotalMilliseconds / 1000d;

                        values[1] = double.Parse(results[5], System.Globalization.CultureInfo.InvariantCulture); // Polarization Voltage
                        values[2] = double.Parse(results[6], System.Globalization.CultureInfo.InvariantCulture); // Sensor Current (nA)
                        values[3] = double.Parse(results[7], System.Globalization.CultureInfo.InvariantCulture) / 10; // Regulation Voltage
                        values[4] = double.Parse(results[8], System.Globalization.CultureInfo.InvariantCulture) * 10; // Impedance

                        int nextBufferIdx = ((currentBufferIdx + 1) >= BUFFER_LENGTH) ? 0 : (currentBufferIdx + 1);

                        for (int j = 0; j < 3; j++)
                        {
                            data.data[j][0][currentBufferIdx] = values[0];
                            data.data[j][1][currentBufferIdx] = values[j + 1];

                            data.data[j][0][nextBufferIdx] = double.NaN;
                            data.data[j][1][nextBufferIdx] = double.NaN;
                        }

                        currentBufferIdx = nextBufferIdx;
                        appendResultsValues(values, positionColumns: false);

                        NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(data, !bFirstData));
                        bFirstData = false;
                    }
                }
                Thread.Sleep(100);
            }

            // stop measurement
            connection?.SetMessageListener(null);
            connection.SendMessage("S");

            status = enExperimentStatus.Idle;

            NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, data));
        }

        public void MessageReceived(string sMsg)
        {
            messageQueue.Enqueue(new MessageObject(sMsg));
        }
    }
}
