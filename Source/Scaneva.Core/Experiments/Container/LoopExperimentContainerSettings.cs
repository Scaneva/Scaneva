#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="LoopExperimentContainerSettings.cs" company="Scaneva">
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{
    public class LoopExperimentContainerSettings : ISettings
    {
        int numIterations = 1;

        double? maxDuration = null;

        //User interface
        [Category("Loop Settings")]
        [DisplayName("Iterations")]
        [Description("The number of iterations the child experiments in this container will be repeated.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 1000)]
        public int NumIterations { get => numIterations; set => numIterations = value; }

        //User interface
        [Category("Loop Settings")]
        [DisplayName("Maximum Duration (s)")]
        [Description("Loop experiment will abort after current iteration when time elapsed is longer than this value.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1, 100000, 0.1, 3)]
        public double? MaxDuration { get => maxDuration; set => maxDuration = value; }

    }

}
