#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Generic1DExperimentData.cs" company="Scaneva">
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

namespace Scaneva.Core.ExperimentData
{
    public class Generic1DExperimentData : IExperimentData
    {
        public List<double[]> data = new List<double[]>();
        public List<string> axisNames = new List<string>();
        public List<string> axisUnits = new List<string>();
        public List<string> datasetNames = new List<string>();
        public string experimentName;
        public bool partialData = true;

        public double[] Get1DData(int dataset)
        {
            return data[dataset];
        }

        public double[][] Get2DData(int dataset)
        {
            return null;
        }

        public double[][][] Get3DData(int dataset)
        {
            return null;
        }

        public string GetAxisName(int dataset, int axis)
        {
            return axisNames[dataset];
        }

        public string GetAxisUnits(int dataset, int axis)
        {
            return axisUnits[dataset];
        }

        public string GetDatasetName(int dataset)
        {
            if ((datasetNames != null) && (dataset >= 0) && (dataset < datasetNames.Count))
            {
                return datasetNames[dataset];
            }
            return null;
        }

        public int GetDatasets()
        {
            return data.Count;
        }

        public int GetDimensions()
        {
            return 1;
        }

        public string GetExperimentName()
        {
            return experimentName;
        }

        public bool IsPartialData()
        {
            return partialData;
        }
    }
}
