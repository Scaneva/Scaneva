#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpAutoapproach.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scaneva.Core.Settings;
using Scaneva.Core.Hardware;
using Scaneva.Core.ExperimentData;
using Scaneva.Tools;

namespace Scaneva.Core.Experiments.ScanEva
{
    [Category("Scan Experiments")]
    public class Autoapproach : ExperimentBase, IExperiment
    {

        FeedbackController FBC;

        public Autoapproach(LogHelper log)
                : base(log)
        {
            settings = new AutoapproachSettings();
        }

        public AutoapproachSettings Settings
        {
            get
            {
                return (AutoapproachSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        private Generic2DExperimentData experimentData;
        private Generic2DExperimentData experimentData_PA; // for Post Approach

        public override Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                var positioners = value.Where(x => typeof(IPositioner).IsAssignableFrom(x.Value.GetType())).Select(x => x);
                Settings.FeedbackController.Positioners = positioners.ToDictionary(item => item.Key, item => (IPositioner)item.Value);

                Dictionary<string, TransducerChannel> transducerChannels = new Dictionary<string, TransducerChannel>();
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

                Settings.FeedbackController.TransducerChannels = transducerChannels;
                hwStore = value;
            }
        }

        private bool abortExperiment = false;

        public override enExperimentStatus Abort()
        {
            if (status == enExperimentStatus.Running)
            {
                abortExperiment = true;
                FBC.Abort();
                return status;
            }

            return enExperimentStatus.Error;
        }

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            FBC = new FeedbackController(log);
            FBC.Settings = Settings.FeedbackController;
            FBC.FBPositionUpdated += FBC_FBPositionUpdated;

            if (!FBC.Initialize().HasFlag(enuFeedbackStatusFlags.Ready))
            {
                return enExperimentStatus.Error;
            }
            this.parent = parent;
            ResultsFilePath = resultsFilePath;
            string cords = "";
            ExperimentContainer container = parent as ExperimentContainer;
            this.parent = parent;

            if (container != null)
            {
                cords = container.ChildIndexer();
                if (cords != "")
                {
                    cords = " " + cords;
                }
            }

            ResultsFileName = "AutoapproachExperiment - " + Name + cords + ".dat";

            TransducerChannel signalChan = FBC.Settings.TransducerChannels[FBC.Settings.Channel];            

            string headerString = "Experiment: AutoapproachExperiment - " + Name + "\r\n";

            string unit = (signalChan.Prefix == enuPrefix.none) ? signalChan.Unit : signalChan.Prefix + signalChan.Unit;
            string[] dataColumnHeaders = new string[] { "Z-Position [µm]", signalChan.Name + " [" + unit + "]" };

            headerString += "Positioner: " + FBC.Settings.Positioner + "\r\n";
            headerString += "Sensor: " + FBC.Settings.Channel + "\r\n";
            writeHeader(headerString, dataColumnHeaders.ToArray(), settingsObj: Settings, positionColumns: false, timeColumn: true);

            // Init ResultData
            experimentData = new Generic2DExperimentData();
            experimentData.axisNames.Add(new string[] { "Z-Position", signalChan.Name });
            experimentData.axisUnits.Add(new string[] { "µm", unit });
            experimentData.data.Add(new double[2][]);
            signalData = new List<double>(128);
            positionData = new List<double>(128);
            experimentData.datasetNames.Add("Autoapproach");

            experimentData_PA = new Generic2DExperimentData();
            experimentData_PA.axisNames.Add(new string[] { "Time", signalChan.Name });
            experimentData_PA.axisUnits.Add(new string[] { "s", unit });
            experimentData_PA.data.Add(new double[2][]);
            signalData_PA = new List<double>(128);
            timeData_PA = new List<double>(128);
            experimentData_PA.datasetNames.Add("Post approach");

            status = enExperimentStatus.Idle;
            return status;
        }

        private void FBC_FBPositionUpdated(object sender, FBPositionUpdatedEventArgs e)
        {
            if (experimentData != null)
            {
                if (e.IsPostApproachPhase)
                {
                    if (signalData_PA.Count < 1)
                    {
                        appendCommentLines("# ================ Start of post approach ================");
                    }

                    appendResultsValues(new double[] { e.Position, e.Signal }, positionColumns: false);

                    signalData_PA.Add(e.Signal);
                    timeData_PA.Add(e.Time);

                    experimentData_PA.data[0][0] = timeData_PA.ToArray();
                    experimentData_PA.data[0][1] = signalData_PA.ToArray();

                    NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(experimentData_PA, signalData_PA.Count > 1));
                }
                else
                {
                    appendResultsValues(new double[] { e.Position, e.Signal }, positionColumns: false);

                    signalData.Add(e.Signal);
                    positionData.Add(e.Position);

                    experimentData.data[0][0] = positionData.ToArray();
                    experimentData.data[0][1] = signalData.ToArray();

                    NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(experimentData, signalData.Count > 1));
                }
            }
        }

        Task measurementTask = null;
        private List<double> signalData;
        private List<double> positionData;
        private List<double> signalData_PA;
        private List<double> timeData_PA;

        public override enExperimentStatus Run()
        {
            // Only Run if Experiment is Idle and Task not running
            if ((status == enExperimentStatus.Idle) && ((measurementTask == null) || (measurementTask.IsCompleted) || (measurementTask.IsCanceled)))
            {
                abortExperiment = false;
                measurementTask = new Task(() => { ExecuteAutoapproach(); });
                status = enExperimentStatus.Running;

                measurementTask.Start();
                return status;
            }
            log.Add("Experiment not Configured or already running", "Error");
            return enExperimentStatus.Error;
        }

        private void ExecuteAutoapproach()
        {
            enuFeedbackStatusFlags stat;
            if (status.HasFlag(enExperimentStatus.Running))
            {
                if (abortExperiment)
                {
                    status = enExperimentStatus.Aborted;
                    NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Aborted, experimentData));
                }
                else
                {
                    stat = FBC.GoToSetpoint();
                    if (stat.HasFlag(enuFeedbackStatusFlags.AtSetpoint))
                    {
                        status = enExperimentStatus.Completed;
                        NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, experimentData));
                    }
                    else if (stat.HasFlag(enuFeedbackStatusFlags.Aborted))
                    {
                        status = enExperimentStatus.Aborted;
                        NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Aborted, experimentData));
                    }
                    else
                    {
                        status = enExperimentStatus.Error;
                        NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Error, experimentData));
                    }
                }
            }
            else
            {
                status = enExperimentStatus.Error;
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Error, experimentData));
            }
        }
    }
}