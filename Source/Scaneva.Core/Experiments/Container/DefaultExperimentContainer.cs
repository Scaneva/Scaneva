#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="DefaultExperimentContainer.cs" company="Scaneva">
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
    [DisplayName("Default Container")]
    public class DefaultExperimentContainer : ExperimentContainer
    {
        public DefaultExperimentContainer(LogHelper log) : base(log)
        {

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

        private void RunChildren()
        {
            Task childRunner = RunChildExperiments();
            // Wait for completion of ChildExperiments
            childRunner.Wait();

            NotifyExperimentEndedNow(new ExperimentEndedEventArgs(status, null));
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
            // Nothing to Check
            errorMessage = String.Empty;
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
        /// override => Default Container only executes children once, empty index returned
        /// </summary>
        /// <returns></returns>
        public override string ChildIndexer()
        {
            return "";
        }

    }
}
