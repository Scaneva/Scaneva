#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="FastAmperometrySettings.cs" company="Scaneva">
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

namespace Scaneva.Core.Experiments.PalmSens
{
    public class FastAmperometrySettings : ISettings
    {
        private string[] _ListofPalmSensHW;
        private string[] _ListofCurrentRanges;

        [Browsable(false)]
        public string[] ListofPalmSensHW { get => _ListofPalmSensHW; set => _ListofPalmSensHW = value; }

        [Browsable(false)]
        public string[] ListofCurrentRanges { get => _ListofCurrentRanges; set => _ListofCurrentRanges = value; }

        private string hwName = "";
        private string faCurrentRange = null;
        private float equilibrationTime = 5;
        private float equilibrationPotential = 1.0f;
        private float potential = 0.5f;
        private float interval = 0.05f;
        private float run = 10.0f;

        [Category("Fast Amperometry")]
        [DisplayName("PalmSens HW Name")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofPalmSensHW")]
        public string HwName { get => hwName; set => hwName = value; }

        [Category("Fast Amperometry")]
        [DisplayName("Current Range")]
        [Description("Set the current range for the Fast Amperometry recordings (no autoranging)")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofCurrentRanges")]
        public string FACurrentRange
        {
            //When first loaded set property with the third item in the rule list.
            get
            {
                string S = "";
                if (faCurrentRange != null)
                {
                    S = faCurrentRange;
                }
                else
                {
                    if (ListofCurrentRanges != null)
                    {
                        if (ListofCurrentRanges.Length > 5)
                        {
                            ////Sort the list before displaying it
                            //Array.Sort(PalmSens4Settings_GlobalVars._ListofConnections);
                            S = ListofCurrentRanges[5];
                        }
                        else if (ListofCurrentRanges.Length > 0)
                        {
                            S = ListofCurrentRanges[0];
                        }
                    }

                }
                return S;
            }
            set { faCurrentRange = value; }
        }

        [Category("Fast Amperometry")]
        [DisplayName("Equilibration Time (s)")]
        [Description("Equilibration duration in seconds Begin Potential is applied during equilibration")]
        public float EquilibrationTime { get => equilibrationTime; set => equilibrationTime = value; }

        [Category("Fast Amperometry")]
        [DisplayName("Equilibration Potential (V)")]
        [Description("Equilibration Potential. The applicable range of the potential depends on the device connected.\r\n Resolution is 1 mV")]
        public float EquilibrationPotential { get => equilibrationPotential; set => equilibrationPotential = value; }

        [Category("Fast Amperometry")]
        [DisplayName("DC Potential (V)")]
        [Description("Potential applied during measurement. Applicable range is -2V to +2V.")]
        public float Potential { get => potential; set => potential = value; }

        [Category("Fast Amperometry")]
        [DisplayName("Interval Time (s)")]
        [Description("Interval time.")]
        public float IntervalTime { get => interval; set => interval = value; }

        [Category("Fast Amperometry")]
        [DisplayName("Run Time (s)")]
        [Description("Run time.")]
        public float RunTime { get => run; set => run = value; }

    }
}
