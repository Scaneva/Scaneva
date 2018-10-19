#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SetSingleValue.cs" company="Scaneva">
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
    public class SetSingleValue : ExperimentBase, IExperiment
    {
        public SetSingleValue(LogHelper log)
            : base(log)
        {
            settings = new SetSingleValueSettings();
        }

        public SetSingleValueSettings Settings
        {
            get
            {
                return (SetSingleValueSettings)settings;
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
                        // only add active and mixed channels
                        if ((chan.ChannelType == enuChannelType.active) || (chan.ChannelType == enuChannelType.mixed))
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

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            this.parent = parent;
            ResultsFilePath = resultsFilePath;
            ResultsFileName = "SetSingleValue - " + Name + ".dat";

            activeChannels = new List<TransducerChannel>();

            foreach (string chan in Settings.Channels)
            {
                if ((chan != null) && (chan != "NONE") && (transducerChannels.ContainsKey(chan)))
                {
                    //ITransducer transducer = HWStore[Settings.Channel.Split('.').First()] as ITransducer;
                    TransducerChannel channel = transducerChannels[chan];
                    activeChannels.Add(channel);
                }
            }

            if (activeChannels.Count == 0)
            {
                log.Add("No transducer channel selected or invalid configuration", "Error");
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
                // Write Settings
                writeHeader("", new string[] { }, Settings, false);

                measurementTask = new Task(() => { ExecuteSetSingleValue(); });
                status = enExperimentStatus.Running;
                measurementTask.Start();
                return status;
            }
            log.Add("Experiment not configured or already running", "Error");
            return enExperimentStatus.Error;
        }

        private void ExecuteSetSingleValue()
        {
            double[] values = Settings.Values;

            for (int i = 0; i < Settings.Channels.Count(); i++)
            {
                string chan = Settings.Channels[i];

                if ((chan != null) && (chan != "NONE"))
                {
                    TransducerChannel channel = transducerChannels[chan];
                    channel.SetValue(values[i]);
                }                
            }

            status = enExperimentStatus.Idle;

            NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, null));
        }
    }
}
