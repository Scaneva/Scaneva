﻿#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ISettings.cs" company="Scaneva">
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

namespace Scaneva.Core
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public interface ISettings
    {
    }

    public class DynamicDisplayNames
    {
        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, string> DisplayNameMapping = new Dictionary<string, string>();

        public void SetDisplayName(string key, string value)
        {
            if (DisplayNameMapping.ContainsKey(key))
            {
                DisplayNameMapping[key] = value;
            }
            else
            {
                DisplayNameMapping.Add(key, value);
            }
        }
    }
}
