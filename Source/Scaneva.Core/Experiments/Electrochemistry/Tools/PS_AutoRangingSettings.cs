#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="AutoRangingSettings.cs" company="Scaneva">
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

using PalmSens;
using Scaneva.Core.Hardware;

namespace Scaneva.Core.Experiments.PalmSens
{
    public class PS_AutoRangingSettings
    {
        private string[] _ListofCurrentRanges;

        [Browsable(false)]
        public string[] ListofCurrentRanges { get => _ListofCurrentRanges; set => _ListofCurrentRanges = value; }

        private string minCurrentRange = null;
        private string maxCurrentRange = null;

        private int defaultMin = 2;
        private int defaultMax = 6;

        public PS_AutoRangingSettings()
        {

        }

        public PS_AutoRangingSettings(int defaultMin, int defaultMax)
        {
            this.defaultMin = defaultMin;
            this.defaultMax = defaultMax;
        }

        [Category("Auto Ranging")]
        [DisplayName("Min Current Range")]
        [Description("Set the lowest range used for auto ranging.")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofCurrentRanges")]
        public string MinCurrentRange
        {
            //When first loaded set property with the third item in the rule list.
            get
            {
                string S = "";
                if (minCurrentRange != null)
                {
                    S = minCurrentRange;
                }
                else
                {
                    if (ListofCurrentRanges != null)
                    {
                        if (ListofCurrentRanges.Length > 5)
                        {
                            ////Sort the list before displaying it
                            //Array.Sort(PalmSens4Settings_GlobalVars._ListofConnections);
                            S = ListofCurrentRanges[defaultMin];
                        }
                        else if (ListofCurrentRanges.Length > 0)
                        {
                            S = ListofCurrentRanges[0];
                        }
                    }

                }
                return S;
            }
            set { minCurrentRange = value; }
        }

        [Category("Auto Ranging")]
        [DisplayName("Max Current Range")]
        [Description("Set the highest range used for auto ranging.")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofCurrentRanges")]
        public string MaxCurrentRange
        {
            //When first loaded set property with the third item in the rule list.
            get
            {
                string S = "";
                if (maxCurrentRange != null)
                {
                    S = maxCurrentRange;
                }
                else
                {
                    if (ListofCurrentRanges != null)
                    {
                        if (ListofCurrentRanges.Length > defaultMax)
                        {
                            ////Sort the list before displaying it
                            //Array.Sort(PalmSens4Settings_GlobalVars._ListofConnections);
                            S = ListofCurrentRanges[defaultMax];
                        }
                        else if (ListofCurrentRanges.Length > 0)
                        {
                            S = ListofCurrentRanges[ListofCurrentRanges.Length - 1];
                        }
                    }

                }
                return S;
            }
            set { maxCurrentRange = value; }
        }

        public override string ToString()
        {
            return "[" + MinCurrentRange + ", " + MaxCurrentRange + "]";
        }

        public void ConfigureMethod(Method meth, PS_PalmSens hw)
        {
            //Auto Ranging settings
            CurrentRange minRange = hw.SupportedRanges.First(x => x.ToString() == MinCurrentRange);
            CurrentRange maxRange = hw.SupportedRanges.First(x => x.ToString() == MaxCurrentRange);

            meth.Ranging = new AutoRanging(minRange, maxRange);
        }
    }
}
