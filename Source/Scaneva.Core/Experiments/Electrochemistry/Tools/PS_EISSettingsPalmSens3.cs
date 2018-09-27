#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="EISSettingsPalmSens3.cs" company="Scaneva">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Scaneva.Core.Experiments.PalmSens
{
    public enum EISSensitivityMode
    {
        Low,
        Normal,
        High
    }

    [XmlInclude(typeof(EISSettingsPalmSens4))]
    [XmlInclude(typeof(EISSettingsPalmSens3))]
    public class PS_EISSettingsPalmSens3
    {
        public override string ToString()
        {
            return "[ ... ]";
        }
    }

    public class EISSettingsPalmSens4 : PS_EISSettingsPalmSens3
    {
        public override string ToString()
        {
            return "None";
        }
    }


    public class EISSettingsPalmSens3 : PS_EISSettingsPalmSens3
    {
        private EISSensitivityMode sensitivityMode = EISSensitivityMode.Normal;
        private bool allowACCouplingGreater200Hz = false;
        private bool disableHighStabilityMode = false;

        [DisplayName("Allow AC coupling > 200 Hz")]
        [Description("If true, measurements above 200 Hz will be done AC coupled to improve resolution.")]
        public bool AllowACCouplingGreater200Hz { get => allowACCouplingGreater200Hz; set => allowACCouplingGreater200Hz = value; }

        [DisplayName("Disable High Stabilty mode")]
        [Description("If false, high stability mode is enabled for frequencys below 200 Hz.")]
        public bool DisableHighStabilityMode { get => disableHighStabilityMode; set => disableHighStabilityMode = value; }

        [DisplayName("Sensititvity mode")]
        [Description("Sensititvity mode.")]
        public EISSensitivityMode SensititvityMode { get => sensitivityMode; set => sensitivityMode = value; }
    }
}
