#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PC_PI_E_727_3x_Settings.cs" company="Scaneva">
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

namespace Scaneva.Core.Settings
{
    public class PC_PI_E_727_3x_Settings : ISettings
    {

        bool blnAutoZero = false;
        bool blnClosedLoop = false;
        PC_Gnrc_Gnrc_Settings_Axis XAxis = new PC_Gnrc_Gnrc_Settings_Axis();
        PC_Gnrc_Gnrc_Settings_Axis YAxis = new PC_Gnrc_Gnrc_Settings_Axis();
        PC_Gnrc_Gnrc_Settings_Axis ZAxis = new PC_Gnrc_Gnrc_Settings_Axis();

        public bool AutoZero { get => blnAutoZero; set => blnAutoZero = value; }
        public bool ClosedLoop { get => blnClosedLoop; set => blnClosedLoop = value; }
        public PC_Gnrc_Gnrc_Settings_Axis X { get => XAxis; set => XAxis = value; }
        public PC_Gnrc_Gnrc_Settings_Axis Y { get => YAxis; set => YAxis = value; }
        public PC_Gnrc_Gnrc_Settings_Axis Z { get => ZAxis; set => ZAxis = value; }
    }
}
