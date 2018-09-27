#region Copyright (C)
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
    public class SingleValueExperiment : ExperimentBase, IExperiment
    {
        public SingleValueExperiment(LogHelper log)
            : base(log)
        {
            settings = new SingleValueSettings();
        }

        public SingleValueSettings Settings
        {
            get
            {
                return (SingleValueSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        private Dictionary<string, TransducerChannel> transducerChannels;

        public override Dictionary<string, IHWCompo> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                transducerChannels = new Dictionary<string, TransducerChannel>();
                var transducers = value.Where(x => typeof(ITransducer).IsAssignableFrom(x.Value.GetType())).Select(x => x).ToArray();

                // Iterate Transducers
                foreach (KeyValuePair<string, IHWCompo> ele in transducers)
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

        private TransducerChannel channel = null;

        public override enExperimentStatus Configure(string resultsFilePath)
        {           
            if ((Settings.Channel != null) && (transducerChannels.ContainsKey(Settings.Channel)))
            {
                //ITransducer transducer = HWStore[Settings.Channel.Split('.').First()] as ITransducer;

                ResultsFilePath = resultsFilePath;
                ResultsFileName = "SingleValueExperiment - " + Name + ".dat";

                channel = transducerChannels[Settings.Channel];

                string headerString = "Experiment: SingleValueExperiment - " + Name + "\r\n";
                headerString += "Channel: " + Settings.Channel + "\r\n";
                headerString += "Name: " + channel.Name + "\r\n";

                string unit = (channel.Prefix == enuPrefix.none) ? channel.Unit : channel.Prefix + channel.Unit;
                headerString += "Unit: " + unit + "\r\n";

                writeHeader(headerString, new string[] { channel.Name + " [" + unit + "]" });

                status = enExperimentStatus.Idle;
                return status;
            }
            log.Add("No Transducer Channel Selected or Invalid Configuration", "Error");
            return enExperimentStatus.Error;
        }

        Task measurementTask = null;

        public override enExperimentStatus Run()
        {
            // Only Run if Experiment is Idle and Task not running
            if ((status == enExperimentStatus.Idle) && ((measurementTask == null) || (measurementTask.IsCompleted) || (measurementTask.IsCanceled)))
            {
                measurementTask = new Task(() => { ExecuteSingleValueMeasurement(); });
                measurementTask.Start();

                status = enExperimentStatus.Running;
                return status;
            }
            log.Add("Experiment not Configured or already running", "Error");
            return enExperimentStatus.Error;
        }

        private void ExecuteSingleValueMeasurement()
        {
            double value = channel.GetValue();

            appendResultsValues(new double[] { value });

            status = enExperimentStatus.Idle;

            NotifyExperimentEndedNow();
        }

    }
}
