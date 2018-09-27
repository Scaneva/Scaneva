#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="BiPotSettings.cs" company="Scaneva">
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
using Scaneva.Core.Hardware;
using PalmSens;

namespace Scaneva.Core.Experiments.PalmSens
{
    public class PS_BiPotSettings
    {
        private string[] _ListofCurrentRanges;

        [Browsable(false)]
        public string[] ListofCurrentRanges { get => _ListofCurrentRanges; set => _ListofCurrentRanges = value; }

        //Bipot settings
        private EnumExtraValue recordExtraValue = EnumExtraValue.WE2; //Set to WE2 to record data from bipot
        private Method.EnumPalmSensBipotMode biPotModePS = Method.EnumPalmSensBipotMode.constant; //Set bipot to a fixed potential (constant) or an offset of the main WE (offset)
        private float biPotPotential = 1f; //Set the fixed or offset potential of WE2 in Volt
        private string biPotCR = null; //Set the current range for the WE2 recordings (autoranging not supported)

        [Category("BiPot Settings")]
        [DisplayName("Auxillary Channel")]
        [Description("Auxillary channel to record. Set to WE2 to record data from bipot.")]
        public EnumExtraValue RecordExtraValue { get => recordExtraValue; set => recordExtraValue = value; }

        [Category("BiPot Settings")]
        [DisplayName("BiPot Mode")]
        [Description("Set bipot to a fixed potential (constant) or an offset of the main WE (offset)")]
        public Method.EnumPalmSensBipotMode BiPotModePS { get => biPotModePS; set => biPotModePS = value; }

        [Category("BiPot Settings")]
        [DisplayName("BiPot Potential (V)")]
        [Description("Set the fixed or offset potential of WE2 in Volt")]
        public float BiPotPotential { get => biPotPotential; set => biPotPotential = value; }

        [Category("BiPot Settings")]
        [DisplayName("BiPot Current Range")]
        [Description("Set the current range for the WE2 recordings (autoranging not supported)")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofCurrentRanges")]
        public string BiPotCR
        {
            //When first loaded set property with the third item in the rule list.
            get
            {
                string S = "";
                if (biPotCR != null)
                {
                    S = biPotCR;
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
            set { biPotCR = value; }
        }

        public override string ToString()
        {
            return recordExtraValue.ToString();
        }

        public void ConfigureMethod(Method meth, PS_PalmSens hw)
        {
            //Bipot settings
            meth.RecordExtraValue = RecordExtraValue;

            if (RecordExtraValue != EnumExtraValue.None)
            {
                CurrentRange range = hw.SupportedRanges.First(x => x.ToString() == BiPotCR);
                
                meth.BipotModePS = BiPotModePS;
                meth.BiPotPotential = BiPotPotential;
                meth.BiPotCR = range;
            }
        }
    }
}
