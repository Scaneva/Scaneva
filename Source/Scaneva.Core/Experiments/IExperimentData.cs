#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="IExperimentData.cs" company="Scaneva">
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
    public interface IExperimentData
    {
        string GetExperimentName();
        string GetDatasetName(int dataset);
        bool IsPartialData();
        int GetDimensions();
        int GetDatasets();
        string GetAxisName(int dataset, int axis);
        string GetAxisUnits(int dataset, int axis);
        double[][][] Get3DData(int dataset);
        double[][] Get2DData(int dataset);
        double[] Get1DData(int dataset);
    }
}
