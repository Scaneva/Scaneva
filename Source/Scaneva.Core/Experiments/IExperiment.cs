#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="IExperiment.cs" company="Scaneva">
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

namespace Scaneva.Core
{
    public enum enExperimentStatus
    {
        OK = 0,
        Uninitialized = 1,
        Idle = 2,
        Running = 3,
        Completed = 4,
        Aborted = 5,
        Error = 255,
    }

    public class ExperimentEndedEventArgs : EventArgs
    {
        private readonly enExperimentStatus status;
        private readonly IExperimentData data;

        public ExperimentEndedEventArgs(enExperimentStatus status, IExperimentData data)
        {
            this.status = status;
            this.data = data;
        }

        public enExperimentStatus Status
        {
            get { return this.status; }
        }

        public IExperimentData Data
        {
            get { return this.data; }
        }
    }

    public class ExperimentDataEventArgs : EventArgs
    {
        private readonly IExperimentData data;
        private readonly bool updatedData;

        public ExperimentDataEventArgs(IExperimentData data, bool updatedData)
        {
            this.data = data;
            this.updatedData = updatedData;
        }

        public IExperimentData Data
        {
            get { return this.data; }
        }

        public bool IsUpdatedData
        {
            get { return this.updatedData; }
        }
    }

    public interface IExperiment
    {
        Dictionary<string, IHWManager> HWStore { get; set; }
        PositionStore PositionStore { get; set; }

        enExperimentStatus Status { get; }
        enExperimentStatus Configure(IExperiment parent, string resultsFilePath);
        enExperimentStatus Run();
        enExperimentStatus Abort();
        //position of experiment, if any.
        //most experiments will return the position of their parent experiment...
        Position Position();

        event EventHandler<ExperimentEndedEventArgs> NotifyExperimentEnded;
        void NotifyExperimentEndedNow(ExperimentEndedEventArgs eventArgs);

        event EventHandler<ExperimentDataEventArgs> NotifyExperimentDataUpdated;
        void NotifyExperimentDataUpdatedNow(ExperimentDataEventArgs eventArgs);
    }
}
