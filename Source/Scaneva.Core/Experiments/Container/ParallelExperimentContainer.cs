#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ParallelExperimentContainer.cs" company="Scaneva">
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

using Scaneva.Core.ExperimentData;
using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{
    [Category("Experiment Containers")]
    [DisplayName("Parallel Container")]
    [Description("Parallel Container\n\nExecutes all contained experiments in parallel. Experiments are started simultaneous. Container completes when the longest running Experiment has completed.")]
    public class ParallelExperimentContainer : ExperimentContainer
    {
        public ParallelExperimentContainer(LogHelper log) : base(log)
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
            Task childRunner = RunChildExperimentsParallel();
            // Wait for completion of ChildExperiments
            childRunner.Wait();

            NotifyExperimentEndedNow(new ExperimentEndedEventArgs(status, null));
        }

        protected Task RunChildExperimentsParallel()
        {
            return Task.Factory.StartNew(new Action(ParallelChildExperimentRunner), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private TaskCompletionSource<bool>[] childExperimentsCompleted = null;
        private Generic2DExperimentData experimentData;
        private bool firstExperimentData = false;

        private void ParallelChildExperimentRunner()
        {
            childExperimentsCompleted = new TaskCompletionSource<bool>[this.Count];
            int i = 0;
            enExperimentStatus expStatus = enExperimentStatus.OK;

            foreach (IExperiment exp in this)
            {
                childExperimentsCompleted[i] = new TaskCompletionSource<bool>();

                log.Add("Configuring " + ((ParametrizableObject)exp).Name + "...");
                expStatus = exp.Configure(this, Path.Combine(ResultsFilePath, (exp as ParametrizableObject).Name));
                if ((expStatus != enExperimentStatus.OK) && (expStatus != enExperimentStatus.Idle))
                {
                    log.Warning("Experiment Sequence Aborted due to exp.Configure() returning: " + expStatus);
                    status = enExperimentStatus.Error;
                    return;
                }

                exp.NotifyExperimentDataUpdated -= Child_ParallelExperimentDataUpdated;
                exp.NotifyExperimentDataUpdated += Child_ParallelExperimentDataUpdated;
                exp.NotifyExperimentEnded -= Child_ParallelExperimentEnded;
                exp.NotifyExperimentEnded += Child_ParallelExperimentEnded;

                i++;
            }

            experimentData = new Generic2DExperimentData();
            experimentData.experimentName = "Linear Sweep Voltammetry";
            firstExperimentData = false;

            // fill Experiment Data with dummies
            for (i = 0; i< this.Count; i++)
            {
                // Add a dummy
                experimentData.datasetNames.Add("");
                experimentData.axisNames.Add(new string[] { "", "" });
                experimentData.axisUnits.Add(new string[] { "", "" });
                experimentData.data.Add(new double[2][] { new double[] { double.NaN }, new double[] { double.NaN } });
            }

            i = 0;
            foreach (IExperiment exp in this)
            {
                enExperimentStatus childStatus = exp.Run();
                if ((childStatus != enExperimentStatus.OK) && (childStatus != enExperimentStatus.Running))
                {
                    expStatus = enExperimentStatus.Error;
                }
            }

            if (expStatus == enExperimentStatus.Error)
            {
                foreach (IExperiment exp in this)
                {
                    exp.NotifyExperimentDataUpdated -= Child_ParallelExperimentDataUpdated;
                    exp.NotifyExperimentEnded -= Child_ParallelExperimentEnded;
                    exp.Abort();
                }

                log.Warning("Parallel experiment execution error during exp.Run() - all experiments aborted");
                status = enExperimentStatus.Error;
                return;
            }

            Task.WaitAll(childExperimentsCompleted.Select(x => x.Task).ToArray());

            // Check if all Children completed normaly (Task.Result == true)
            if (childExperimentsCompleted.Count(x => x.Task.Result == true) == this.Count)
            {
                status = enExperimentStatus.Completed;
            }

            childExperimentsCompleted = null;
        }

        public override enExperimentStatus Abort()
        {
            if (childExperimentsCompleted != null)
            {

                abortExperiment = true;
                int i = 0;
                foreach (IExperiment exp in this)
                {
                    try
                    {
                        exp.NotifyExperimentDataUpdated -= Child_ParallelExperimentDataUpdated;
                        exp.NotifyExperimentEnded -= Child_ParallelExperimentEnded;
                        exp.Abort();
                    }
                    catch (Exception e)
                    {
                        log.Warning(e.Message);
                    }
                    finally
                    {
                        childExperimentsCompleted[i].TrySetResult(false);
                        i++;
                    }
                }
                status = enExperimentStatus.Aborted;
                return status;
            }
            return enExperimentStatus.Error;
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

        private void Child_ParallelExperimentEnded(object sender, ExperimentEndedEventArgs e)
        {
            IExperiment exp = sender as IExperiment;
            int i = IndexOf(exp);

            // Was there an error?
            if (e.Status == enExperimentStatus.Error)
            {
                status = enExperimentStatus.Error;
                childExperimentsCompleted[i].SetResult(false);
            }
            else
            {
                Child_ExperimentEndedHook(sender, e);
                childExperimentsCompleted[i].SetResult(true);
            }
        }

        private void Child_ParallelExperimentDataUpdated(object sender, ExperimentDataEventArgs e)
        {
            // aggregate and forward Child Experiment Data (only 2D Data supported)
            IExperiment exp = sender as IExperiment;
            int expIdx = IndexOf(exp);

            if (e.Data.GetDimensions() == 2)
            {
                // Is new dataset?
                if (!e.IsUpdatedData)
                {
                    experimentData.datasetNames[expIdx] = e.Data.GetDatasetName(0) + " (" + expIdx + ")" ;
                    experimentData.axisNames[expIdx] = (new string[] { e.Data.GetAxisName(0, 0), e.Data.GetAxisName(0, 1) });
                    experimentData.axisUnits[expIdx] = (new string[] { e.Data.GetAxisUnits(0, 0), e.Data.GetAxisUnits(0, 1) });
                }
                experimentData.data[expIdx] = e.Data.Get2DData(0);
            }

            NotifyExperimentDataUpdatedNow(new ExperimentDataEventArgs(experimentData, firstExperimentData));
            firstExperimentData = false;

            // TODO
        }
    }
}
