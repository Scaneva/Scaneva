#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="OpenCircuitPotentiometrySettings.cs" company="Scaneva">
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
using System.ComponentModel;

namespace Scaneva.Core.Experiments.PalmSens
{
    public class PS_OpenCircuitPotentiometrySettings : ISettings
    {
        private string[] _ListofPalmSensHW;

        [Browsable(false)]
        public string[] ListofPalmSensHW { get => _ListofPalmSensHW; set => _ListofPalmSensHW = value; }
        
        //Hardware
        private string hwName = "";

        //Current ranges
        private PS_AutoRangingSettings autoRangingSettings = new PS_AutoRangingSettings();

        //Potentiometry settings
        private float intervalTime = 0.1f;
        private float runTime = 10;

        //Pretreatment settings
        private float conditioningTime = 0;
        private float conditioningPotential = 0;
        private float depositionTime = 0;
        private float depositionPotential = 0;

        //Bipot
        private PS_BiPotSettings biPotSettings = new PS_BiPotSettings();

        //========================================================================
        [Category("Open Circuit Potentiometry")]
        [DisplayName("PalmSens HW Name")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofPalmSensHW")]
        public string HwName { get => hwName; set => hwName = value; }

        [Category("Open Circuit Potentiometry")]
        [DisplayName("Auto Rangin Settings")]
        [Description("Auto Rangin Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_AutoRangingSettings AutoRangingSettings { get => autoRangingSettings; set => autoRangingSettings = value; }

        [Category("Open Circuit Potentiometry")]
        [DisplayName("Time interval (s)")]
        [Description("Interval time.")]
        public float IntervalTime { get => intervalTime; set => intervalTime = value; }

        [Category("Open Circuit Potentiometry")]
        [DisplayName("Run time (s)")]
        [Description("Run time.")]
        public float RunTime { get => runTime; set => runTime = value; }

        //Pretreatment settings
        [Category("Open Circuit Potentiometry")]
        [DisplayName("Conditioning time (s)")]
        [Description("Pretreatment setting: Conditioning Time")]
        public float ConditioningTime { get => conditioningTime; set => conditioningTime = value; }

        [Category("Open Circuit Potentiometry")]
        [DisplayName("Conditioning potential (V)")]
        [Description("Pretreatment setting: Conditioning Potential")]
        public float ConditioningPotential { get => conditioningPotential; set => conditioningPotential = value; }

        [Category("Open Circuit Potentiometry")]
        [DisplayName("Deposition time (s)")]
        [Description("Pretreatment setting: Deposition Time")]
        public float DepositionTime { get => depositionTime; set => depositionTime = value; }

        [Category("Open Circuit Potentiometry")]
        [DisplayName("Deposition potential (V)")]
        [Description("Pretreatment setting: Deposition Potential")]
        public float DepositionPotential { get => depositionPotential; set => depositionPotential = value; }

        [Category("Open Circuit Potentiometry")]
        [DisplayName("BiPot Settings")]
        [Description("BiPot Settings.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PS_BiPotSettings BiPotSettings { get => biPotSettings; set => biPotSettings = value; }
    }
}
