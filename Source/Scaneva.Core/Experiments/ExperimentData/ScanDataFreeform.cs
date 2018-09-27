#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ScanDataFreeform.cs" company="Scaneva">
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
    public class ScanDataFreeform : IExperimentData
    {
        private List<List<double[]>> data = new List<List<double[]>>();
        private List<string> datasetNames = new List<string>();
        private List<string> datasetValueAxis = new List<string>();

        public string experimentName;
        public bool partialData = true;

        public void addDataset(string name, string valueAxisTitel)
        {
            datasetNames.Add(name);
            datasetValueAxis.Add(valueAxisTitel);
            data.Add(new List<double[]>());
        }

        public void addValue(string dataset, double x, double y, double value)
        {
            int dsIdx = datasetNames.IndexOf(dataset);

            if (dsIdx > -1)
            {
                data[dsIdx].Add(new double[] {x, y, value });
            }
        }

        public List<double[]> GetScanData(int dataset)
        {
            return data[dataset];
        }

        public List<double[]> GetScanData(string dataset)
        {
            int dsIdx = datasetNames.IndexOf(dataset);

            if (dsIdx > -1)
            {
                return data[dsIdx];
            }
            return null;
        }

        public string GetValueAxisTitel(string dataset)
        {
            int dsIdx = datasetNames.IndexOf(dataset);

            if (dsIdx > -1)
            {
                return datasetValueAxis[dsIdx];
            }
            return null;
        }

        public List<string> GetDatasetNames()
        {
            return datasetNames;
        }

        public int GetDatasets()
        {
            return data.Count;
        }

        public int GetDimensions()
        {
            return 2;
        }

        public string GetExperimentName()
        {
            return experimentName;
        }

        public bool IsPartialData()
        {
            return partialData;
        }

        public string GetDatasetName(int dataset)
        {
            throw new NotImplementedException();
        }

        public string GetAxisName(int dataset, int axis)
        {
            throw new NotImplementedException();
        }

        public string GetAxisUnits(int dataset, int axis)
        {
            throw new NotImplementedException();
        }

        public double[][][] Get3DData(int dataset)
        {
            throw new NotImplementedException();
        }

        public double[][] Get2DData(int dataset)
        {
            throw new NotImplementedException();
        }

        public double[] Get1DData(int dataset)
        {
            throw new NotImplementedException();
        }
    }
}
