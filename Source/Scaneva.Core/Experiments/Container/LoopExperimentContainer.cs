#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="LoopExperimentContainer.cs" company="Scaneva">
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{
    [Category("Experiment Containers")]
    [DisplayName("Loop Container")]
    [Description("Loop Container\n\nExecutes all enclosed experiments for a given number of iterations")]
    public class LoopExperimentContainer : ExperimentContainer
    {
        public LoopExperimentContainer(LogHelper log) : base(log)
        {
            settings = new LoopExperimentContainerSettings();
        }

        public LoopExperimentContainerSettings Settings
        {
            get
            {
                return (LoopExperimentContainerSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        private Task childRunTask = null;

        public override enExperimentStatus Run()
        {
            if (status == enExperimentStatus.Idle)
            {
                abortExperiment = false;
                childRunTask = Task.Factory.StartNew(new Action(RunChildren), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                status = enExperimentStatus.Running;
                return status;
            }
            return enExperimentStatus.Error;
        }

        private int loopCounter = 0;

        private void RunChildren()
        {
            loopCounter = 1;
            DateTime startTime = DateTime.Now;
            string baseResultsFilePath = ResultsFilePath;

            int numDigits = (int)Math.Floor(Math.Log10(Settings.NumIterations)) + 1;
            
            // loop till a) iterations finsihed b) max duration elapsed or c) experiment was aborted
            while ((loopCounter <= Settings.NumIterations) && (!abortExperiment) &&
                ((Settings.MaxDuration == null) || ((DateTime.Now - startTime).TotalSeconds < Settings.MaxDuration.Value)))
            {
                ResultsFilePath = Path.Combine(baseResultsFilePath, loopCounter.ToString("D"+ numDigits));
                Task childRunner = RunChildExperiments();
                // Wait for completion of ChildExperiments
                childRunner.Wait();
                loopCounter++;
            }

            // Signal Experiment end
            if (abortExperiment)
            {
                // Experiment was aborted
                status = enExperimentStatus.Aborted;
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Aborted, null));
            }
            else
            {
                // Experiment ended regularly
                status = enExperimentStatus.Completed;
                NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, null));
            }
        }

        public override enExperimentStatus Abort()
        {
            if ((childRunTask != null) && (!childRunTask.IsCompleted))
            {
                status = enExperimentStatus.Aborted;
                abortExperiment = true;
                AbortChildExperiments();
                return status;
            }
            return enExperimentStatus.Error;
        }

        public override bool CheckParametersOk(out string errorMessage)
        {
            errorMessage = String.Empty;
            
            // Check Scan parameters
            if ((Settings.NumIterations <= 0))
            {
                errorMessage = "Configuration Error in '" + Name + "': Loop iterations must be > 0";
                return false;
            }

            // Check Scan parameters
            if ((Settings.MaxDuration <= 0))
            {
                errorMessage = "Configuration Error in '" + Name + "': Max duration must be > 0";
                return false;
            }

            return true;
        }

        public override enExperimentStatus Configure(IExperiment parent, string resultsFilePath)
        {
            if (status != enExperimentStatus.Running)
            {
                this.parent = parent;

                ResultsFilePath = Path.Combine(resultsFilePath, Name);
                status = enExperimentStatus.Idle;
                return enExperimentStatus.OK;
            }
            return enExperimentStatus.Error;
        }

        /// <summary>
        /// override => return loop counter as Indexer
        /// </summary>
        /// <returns></returns>
        public override string ChildIndexer()
        {
            int numDigits = (int)Math.Floor(Math.Log10(Settings.NumIterations)) + 1;
            return loopCounter.ToString("D" + numDigits);
        }

    }
}
