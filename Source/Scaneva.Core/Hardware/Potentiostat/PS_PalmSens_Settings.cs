#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PS_PalmSens_Settings.cs" company="Scaneva">
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
using System.Reflection;
using System.Xml.Serialization;
using PalmSens.Devices;

namespace Scaneva.Core.Settings
{
    public class PS_PalmSens_Settings : ISettings
    {
        private string[] _ListofConnections;
        private string[] _ListofCurrentRanges;

        [Browsable(false)]
        public string[] ListofConnections { get => _ListofConnections; set => _ListofConnections = value; }
        [Browsable(false)]
        public string[] ListofCurrentRanges { get => _ListofCurrentRanges; set => _ListofCurrentRanges = value; }

        private string connection;
        private string manualControlCurrentRange;

        [Category("PalmSens Settings")]
        [Browsable(true)]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofConnections")]
        public string Connection
        {
            //When first loaded set property with the first item in the rule list.
            get
            {
                string S = "";
                if (connection != null)
                {
                    S = connection;
                }
                else
                {
                    if (ListofConnections != null)
                    {
                        if (ListofConnections.Length > 0)
                        {
                            //Sort the list before displaying it
                            Array.Sort(ListofConnections);
                            S = ListofConnections[0];
                        }
                    }
                }
                return S;
            }
            set { connection = value; }
        }

        [Category("PalmSens Settings")]
        [DisplayName("Current Range")]
        [Description("Set the current range for manual control.")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofCurrentRanges")]
        public string ManualControlCurrentRange
        {
            //When first loaded set property with the third item in the rule list.
            get
            {
                string S = "";
                if ((manualControlCurrentRange != null) && (manualControlCurrentRange != ""))
                {
                    S = manualControlCurrentRange;
                }
                else
                {
                    if (ListofCurrentRanges != null)
                    {
                        if (ListofCurrentRanges.Length > 7)
                        {
                            ////Sort the list before displaying it
                            //Array.Sort(PalmSens4Settings_GlobalVars._ListofConnections);
                            S = ListofCurrentRanges[7];
                        }
                        else if (ListofCurrentRanges.Length > 0)
                        {
                            S = ListofCurrentRanges[ListofCurrentRanges.Length - 1];
                        }
                    }

                }
                return S;
            }
            set { manualControlCurrentRange = value; }
        }


    }
}
