#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SingleValueSettings.cs" company="Scaneva">
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

namespace Scaneva.Core.Experiments
{
    public class SingleValueSettings : ISettings
    {
        private Dictionary<string, TransducerChannel> transducerChannels = new Dictionary<string, TransducerChannel>();

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, TransducerChannel> TransducerChannels
        {
            get => transducerChannels;
            set
            {
                transducerChannels = value;
            }
        }

        private string channel = null;

        [Category("Single Value Experiment Settings")]
        [DisplayName("Select transducer channel")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        public string Channel
        {
            get => channel;
            set
            {
                channel = value;
            }
        }
    }
}
