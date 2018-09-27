#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SetPumpParam.cs" company="Scaneva">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Scaneva.Core.Hardware;
using Scaneva.Core.ExperimentData;
using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{
    [DisplayName("Set pump parameters")]
    public class SetPumpValue : ExperimentBase, IExperiment
    {
        IPump mPump;
        public SetPumpValue(LogHelper log)
            : base(log)
        {
            settings = new SetPumpValueSettings();
        }

        public SetPumpValueSettings Settings
        {
            get
            {
                return (SetPumpValueSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public override Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                var pumps = value.Where(x => typeof(IPump).IsAssignableFrom(x.Value.GetType())).Select(x => x);
                Settings.Pumps = pumps.ToDictionary(item => item.Key, item => (IPump)item.Value);
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
            mPump = Settings.Pumps[Settings.Pump];
            this.parent = parent;
            ResultsFilePath = resultsFilePath;
            ResultsFileName = "SetPumpParam - " + Name + ".dat";
            status = enExperimentStatus.Idle;
            return status;
        }

        Task measurementTask = null;

        public override enExperimentStatus Run()
        {
            // Only Run if Experiment is Idle and Task not running
            if ((status == enExperimentStatus.Idle) && ((measurementTask == null) || (measurementTask.IsCompleted) || (measurementTask.IsCanceled)))
            {
                measurementTask = new Task(() => { ExecuteSetPumpParam(); });
                status = enExperimentStatus.Running;

                measurementTask.Start();
                return status;
            }
            log.Add("Experiment not configured or already running", "Error");
            return enExperimentStatus.Error;
        }

        private void ExecuteSetPumpParam()
        {    
            Dictionary<int, double?> dict = new Dictionary<int, double?>();
            dict.Add(0, Settings.CompositionA);
            dict.Add(1, Settings.CompositionB);
            dict.Add(2, Settings.CompositionC);
            dict.Add(3, Settings.CompositionD);
            mPump.Composition = dict;
            
            if (Settings.Flowrate != null)
            {
                mPump.Flowrate = Settings.Flowrate.Value;
            }

            status = enExperimentStatus.Idle;
            NotifyExperimentEndedNow(new ExperimentEndedEventArgs(enExperimentStatus.Completed, null));
        }
    }
}
