#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SetSingleValueSettings.cs" company="Scaneva">
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
    public class SetSingleValueSettings : ISettings
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

        private string[] channels = new string[8] { "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE" };
        private double[] values = new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

        [Browsable(false)]
        public string[] Channels
        {
            get => channels;
            set
            {
                if (value != null)
                {
                    int i = 0;
                    for (; (i < 8) && (i < value.Length); i++)
                    {
                        if (value[i] == null)
                        {
                            channels[i] = "NONE";
                        }
                        else
                        {
                            channels[i] = value[i];
                        }
                    }
                    for (; i < 8; i++)
                    {
                        channels[i] = "NONE";
                    }
                }
            }
        }

        [Browsable(false)]
        public double[] Values
        {
            get => values;
            set
            {
                if (value != null)
                {
                    int i = 0;
                    for (; (i < 8) && (i < value.Length); i++)
                    {
                        values[i] = value[i];
                    }
                    for (; i < 8; i++)
                    {
                        values[i] = 0.0;
                    }
                }
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 1")]
        [Description("Select transducer channel 1")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel1
        {
            get => channels[0];
            set
            {
                channels[0] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 1 Value")]
        [Description("Output value for channel 1")]
        [XmlIgnore]
        public double ChannelValue1
        {
            get => values[0];
            set
            {
                values[0] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 2")]
        [Description("Select transducer channel 2")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel2
        {
            get => channels[1];
            set
            {
                channels[1] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 2 Value")]
        [Description("Output value for channel 2")]
        [XmlIgnore]
        public double ChannelValue2
        {
            get => values[1];
            set
            {
                values[1] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 3")]
        [Description("Select transducer channel 3")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel3
        {
            get => channels[2];
            set
            {
                channels[2] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 3 Value")]
        [Description("Output value for channel 3")]
        [XmlIgnore]
        public double ChannelValue3
        {
            get => values[2];
            set
            {
                values[2] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 4")]
        [Description("Select transducer channel 4")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel4
        {
            get => channels[3];
            set
            {
                channels[3] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 4 Value")]
        [Description("Output value for channel 4")]
        [XmlIgnore]
        public double ChannelValue4
        {
            get => values[3];
            set
            {
                values[3] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 5")]
        [Description("Select transducer channel 5")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel5
        {
            get => channels[4];
            set
            {
                channels[4] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 5 Value")]
        [Description("Output value for channel 5")]
        [XmlIgnore]
        public double ChannelValue5
        {
            get => values[4];
            set
            {
                values[4] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 6")]
        [Description("Select transducer channel 6")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel6
        {
            get => channels[5];
            set
            {
                channels[5] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 6 Value")]
        [Description("Output value for channel 6")]
        [XmlIgnore]
        public double ChannelValue6
        {
            get => values[5];
            set
            {
                values[5] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 7")]
        [Description("Select transducer channel 7")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel7
        {
            get => channels[6];
            set
            {
                channels[6] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 7 Value")]
        [Description("Output value for channel 7")]
        [XmlIgnore]
        public double ChannelValue7
        {
            get => values[6];
            set
            {
                values[6] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 8")]
        [Description("Select transducer channel 8")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        [XmlIgnore]
        public string Channel8
        {
            get => channels[7];
            set
            {
                channels[7] = value;
            }
        }

        [Category("Set Single Value Settings")]
        [DisplayName("Channel 8 Value")]
        [Description("Output value for channel 8")]
        [XmlIgnore]
        public double ChannelValue8
        {
            get => values[7];
            set
            {
                values[7] = value;
            }
        }
    }
}
