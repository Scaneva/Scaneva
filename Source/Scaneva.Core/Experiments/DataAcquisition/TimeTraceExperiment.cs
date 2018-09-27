#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="TimeTraceExperiment.cs" company="Scaneva">
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
//  Inspiration from the following sources: 
//  http://www.ruhr-uni-bochum.de/elan/
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

namespace Scaneva.Core.Experiments.ScanEva
{
    public class TimeTraceExperiment : ExperimentBase, IExperiment
    {
        public TimeTraceExperiment(LogHelper log)
            : base(log)
        {
            settings = new TimeTraceExperimentSettings();
        }

        public TimeTraceExperimentSettings Settings
        {
            get
            {
                return (TimeTraceExperimentSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        private Dictionary<string, TransducerChannel> transducerChannels;
        private List<TransducerChannel> activeChannels = null;

        public override Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                transducerChannels = new Dictionary<string, TransducerChannel>();
                transducerChannels.Add("NONE", null);

                var transducers = value.Where(x => typeof(ITransducer).IsAssignableFrom(x.Value.GetType())).Select(x => x).ToArray();

                // Iterate Transducers
                foreach (KeyValuePair<string, IHWManager> ele in transducers)
                {
                    ITransducer ducer = (ITransducer)ele.Value;
                    // Iterate Channels
                    foreach (TransducerChannel chan in ducer.Channels)
                    {
                        // only add passiv and mixed channels
                        if ((chan.ChannelType == enuChannelType.passive) || (chan.ChannelType == enuChannelType.mixed))
                        {
                            string name = ele.Key + "." + chan.Name;
                            transducerChannels.Add(name, chan);
                        }
                    }
                }

                //Settings.TransducerChannels = transducerChannels.Select(d => d.Value).ToList();
                Settings.InputChannelSettings.TransducerChannels = transducerChannels;

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


        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            this.parent = parent;
            ResultsFilePath = resultsFilePath;
            ResultsFileName = "TimeTraceExperiment - " + Name + ".dat";

            activeChannels = new List<TransducerChannel>();

            string headerString = "Experiment: TimeTraceExperiment - " + Name + "\r\n";
            List<string> dataColumnHeaders = new List<string>();
            int i = 0;

            dataColumnHeaders.Add("Time [s]");

            foreach (string chan in Settings.InputChannelSettings.Channels)
            {
                if ((chan != null) && (chan != "NONE") && (transducerChannels.ContainsKey(chan)))
                {
                    i++;

                    //ITransducer transducer = HWStore[Settings.Channel.Split('.').First()] as ITransducer;

                    TransducerChannel channel = transducerChannels[chan];
                    activeChannels.Add(channel);

                    headerString += "Channel [" + i + "]: " + chan + "\r\n";
                    headerString += "Channel [" + i + "] Name: " + channel.Name + "\r\n";

                    string unit = (channel.Prefix == enuPrefix.none) ? channel.Unit : channel.Prefix + channel.Unit;
                    headerString += "Channel [" + i + "] Unit: " + unit + "\r\n";
                    dataColumnHeaders.Add(channel.Name + " [" + unit + "]");
                }
            }
            writeHeader(headerString, dataColumnHeaders.ToArray(), positionColumns: false);

            if (activeChannels.Count == 0)
            {
                log.Add("No Transducer Channel Selected or Invalid Configuration", "Error");
                return enExperimentStatus.Error;
            }

            status = enExperimentStatus.Idle;
            return status;
        }

        Task measurementTask = null;

        public override enExperimentStatus Run()
        {
            // Only Run if Experiment is Idle and Task not running
            if ((status == enExperimentStatus.Idle) && ((measurementTask == null) || (measurementTask.IsCompleted) || (measurementTask.IsCanceled)))
            {
                abortExperiment = false;
                status = enExperimentStatus.Running;
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
            double samplingRate = Settings.SamplingRate;
            long samplingDelayMS = (long)Math.Floor((1 / Settings.SamplingRate) * 1000);
            double duration = Settings.Duration;

            data = new Generic2DExperimentData();

            foreach (TransducerChannel channel in activeChannels)
            {
                data.datasetNames.Add(transducerChannels.FirstOrDefault(x => x.Value == channel).Key);
                data.axisNames.Add(new string[] { "Time", channel.Name });
                data.axisUnits.Add(new string[] { "ms", (channel.Prefix == enuPrefix.none) ? channel.Unit : channel.Prefix + channel.Unit });

                double[][] dataBuffer = new double[2][] { new double[BUFFER_LENGTH], new double[BUFFER_LENGTH] };
                
                // initialize the arrays
                for (int i = 0; i < BUFFER_LENGTH; i++)
                {
                    dataBuffer[0][i] = double.NaN;
                    dataBuffer[1][i] = double.NaN;
                }

                data.data.Add(dataBuffer);
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            long lastLoopTime = stopWatch.ElapsedMilliseconds;
            long lastDataUdate = lastLoopTime - 1000;
            bool bFirstData = true;

            int currentBufferIdx = 0;
            while ((lastLoopTime < (duration * 1000)) && (!abortExperiment))
            {
                ExecuteSingleValueMeasurement(stopWatch.ElapsedMilliseconds, ref currentBufferIdx);

                // Check if we need to send a data update
                if ((stopWatch.ElapsedMilliseconds > (lastDataUdate + 250)) || (currentBufferIdx == 1))
                {
                    NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(data, !bFirstData));
                    lastDataUdate = stopWatch.ElapsedMilliseconds;
                    bFirstData = false;
                }

                // calculate required delay
                long targetDelay = (lastLoopTime + samplingDelayMS) - stopWatch.ElapsedMilliseconds;

                // sleep everything > 15 ms
                if (targetDelay > 15)
                {
                    Thread.Sleep((int)(targetDelay - 15));                
                }

                // now idle around until the time is elapsed for acurate timing
                while (stopWatch.ElapsedMilliseconds < (lastLoopTime + samplingDelayMS))
                {
                    Thread.SpinWait(1);
                }

                if (targetDelay > 0)
                {
                    // there was some time to burn up so we calculate 
                    lastLoopTime += samplingDelayMS;
                }
                else
                {
                    // runnning as fast as we can, this is just for reference
                    lastLoopTime = stopWatch.ElapsedMilliseconds;
                }
            }
            status = enExperimentStatus.Idle;

            NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, data));
        }

        private void ExecuteSingleValueMeasurement(long ellapsedMilliSeconds, ref int currentBufferIdx)
        {
            double[] values = new double[activeChannels.Count + 1];

            values[0] = ellapsedMilliSeconds / 1000d;

            int nextBufferIdx = ((currentBufferIdx + 1) >= BUFFER_LENGTH) ? 0 : (currentBufferIdx + 1);

            int i = 0;
            foreach (TransducerChannel channel in activeChannels)
            {
                double value = channel.GetAveragedValue();
                data.data[i][0][currentBufferIdx] = ellapsedMilliSeconds / 1000d;
                data.data[i][1][currentBufferIdx] = value;

                data.data[i][0][nextBufferIdx] = double.NaN;
                data.data[i][1][nextBufferIdx] = double.NaN;

                values[i + 1] = value;
                i++;
            }

            currentBufferIdx = nextBufferIdx;

            appendResultsValues(values, positionColumns: false);
        }

    }
}
