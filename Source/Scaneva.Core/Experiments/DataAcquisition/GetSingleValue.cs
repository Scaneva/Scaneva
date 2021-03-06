﻿#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SingleValueExperiment.cs" company="Scaneva">
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

using Scaneva.Core.Hardware;
using Scaneva.Core.ExperimentData;
using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{
    public class GetSingleValue : ExperimentBase, IExperiment
    {
        public GetSingleValue(LogHelper log)
            : base(log)
        {
            settings = new GetSingleValueSettings();
        }

        public GetSingleValueSettings Settings
        {
            get
            {
                return (GetSingleValueSettings)settings;
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
                Settings.TransducerChannels = transducerChannels;

                hwStore = value;
            }
        }

        public override enExperimentStatus Abort()
        {
            // Abort Single value Experiment not necessary, it completes immediately
            return status;
        }

        public override bool CheckParametersOk(out string errorMessage)
        {
            errorMessage = String.Empty;
            int count = 0;

            for (int i = 0; i < Settings.Channels.Count(); i++)
            {
                string chan = Settings.Channels[i];

                if ((chan != null) && (chan != "NONE"))
                {
                    if (!transducerChannels.ContainsKey(chan))
                    {
                        errorMessage = "Configuration Error in '" + Name + "': Invalid transducer channel " + chan;
                        return false;
                    }
                    count++;
                }
            }

            if (count == 0)
            {
                errorMessage = "Configuration Error in '" + Name + "': No transducer channels selected";
                return false;
            }

            return true;
        }

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            this.parent = parent;
            ResultsFilePath = resultsFilePath;
            ResultsFileName = "SingleValueExperiment - " + Name + ".dat";

            activeChannels = new List<TransducerChannel>();

            string headerString = "Experiment: SingleValueExperiment - " + Name + "\r\n";
            List<string> dataColumnHeaders = new List<string>();
            int i = 0;
            int j = 0;

            // Add Columns for realtive position if parent is a Scan
            if (parent != null)
            {
                dataColumnHeaders.Add("∆X [µm]");
                dataColumnHeaders.Add("∆Y [µm]");
                dataColumnHeaders.Add("∆Z [µm]");
            }

            foreach (string chan in Settings.Channels)
            {
                if ((chan != null) && (chan != "NONE") && (transducerChannels.ContainsKey(chan)))
                {
                    i++;

                    //ITransducer transducer = HWStore[Settings.Channel.Split('.').First()] as ITransducer;

                    TransducerChannel channel = transducerChannels[chan];
                    activeChannels.Add(channel);

                    channel.Averaging = Settings.ChannelsAvging[j];

                    headerString += "Channel [" + i + "]: " + chan + "\r\n";
                    headerString += "Channel [" + i + "] Name: " + channel.Name + "\r\n";

                    string unit = (channel.Prefix == enuPrefix.none) ? channel.Unit : channel.Prefix + channel.Unit;
                    headerString += "Channel [" + i + "] Unit: " + unit + "\r\n";
                    dataColumnHeaders.Add(channel.Name + " [" + unit + "]");
                }
                j++;
            }
            writeHeader(headerString, dataColumnHeaders.ToArray(), null, true, true);

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
                measurementTask = new Task(() => { ExecuteSingleValueMeasurement(); });
                status = enExperimentStatus.Running;

                measurementTask.Start();
                return status;
            }
            log.Add("Experiment not Configured or already running", "Error");
            return enExperimentStatus.Error;
        }

        private void ExecuteSingleValueMeasurement()
        {
            bool bRelPositions = (parent != null);

            double[] values = new double[bRelPositions ? (activeChannels.Count) + 3 : activeChannels.Count];
            Generic1DExperimentData data = new Generic1DExperimentData();

            int i = 0;

            // Add Relative Koordinates and time
            Position scanRelativePosition = new Position(0, 0, 0);
            if (bRelPositions)
            {
                scanRelativePosition = parent.Position();

                values[0] = scanRelativePosition.X;
                values[1] = scanRelativePosition.Y;
                values[2] = scanRelativePosition.Z;
                i = 3;
            }
            
            foreach (TransducerChannel channel in activeChannels)
            {
                double value = channel.GetAveragedValue();
                data.datasetNames.Add(Name + "." + channel.Name);
                data.axisNames.Add(channel.Name);
                data.axisUnits.Add(channel.Unit);
                data.data.Add(new double[] { value });

                values[i++] = value;
            }

            appendResultsValues(values);

            status = enExperimentStatus.Idle;

            NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, data));
        }
    }
}
