#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Pump_Watson_Marlow_520Du_Settings.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
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
using System.IO.Ports;
using Scaneva.Core.Hardware;

namespace Scaneva.Core.Settings
{
    public class Pump_Watson_Marlow_520Du_Settings : ISettings
    {

        private string headType = "520R";
        private double volPerRev = 15.84;
        private bool useTableofVolPerRev = true;
        private double tubeSize = 9.6;

        private string tubing;
        private List<PumpTubing> _ListofTubings;

        //COM Port
        private int cOMPort = 1;
        private int baudrate = 9600;
        private Parity parity = System.IO.Ports.Parity.None;
        private StopBits stopBits = System.IO.Ports.StopBits.Two;
        private Handshake handshake = System.IO.Ports.Handshake.None;
        private int databits = 8;
        bool autoEcho = true; //todo: check
        int pumpID = 1;

        [Browsable(false)]
        [XmlArrayItem("PumpTubing")]
        public List<PumpTubing> ListofTubings { get => _ListofTubings; set => _ListofTubings = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("Pump ID [#]")]
        [Description("ID assigned to the pump as an integer number")]
        public int PumpID { get => pumpID; set => pumpID = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("COM port [#]")]
        [Description("COM port used by the pump as an integer number")]
        public int COMPort { get => cOMPort; set => cOMPort = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("Baud rate [Bd]")]
        public int Baudrate { get => baudrate; set => baudrate = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("Parity")]
        public Parity Parity { get => parity; set => parity = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("Stop bits [#]")]
        public StopBits StopBits { get => stopBits; set => stopBits = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("Handshake")]
        public Handshake Handshake { get => handshake; set => handshake = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("Databits [#]")]
        public int Databits { get => databits; set => databits = value; }

        [Category("Watson Marlow 520Du RS232 settings")]
        [DisplayName("Auto echo")]
        public bool AutoEcho { get => autoEcho; set => autoEcho = value; }

        [Category("Watson Marlow 520Du hardware settings")]
        [DisplayName("Head type")]
        [Description("Head type installed")] //todo: make a droplist
        public string HeadType { get => headType; set => headType = value; }


        [Category("Watson Marlow 520Du hardware settings")]
        [DisplayName("Tubing type")]
        [Description("Select the used tubing")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("ListofTubings")]
        public string Tubing
        {
            //When first loaded set property with the third item in the rule list.
            get
            {
                string S = "";
                if (tubing != null)
                {
                    S = tubing.ToString();
                }
                else
                {
                    if (ListofTubings != null)
                    {
                        if (ListofTubings.Count > 0)
                        {
                            S = ListofTubings.First().ToString();
                        }
                    }
                }
                return S;
            }
            set
            {
                tubing = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public PumpTubing TubingObj
        {
            get => ListofTubings.Where(x => x.Name == tubing).FirstOrDefault();
        }

        //public PumpTubing Tubing { get => tubing; set => tubing = value; }

        [Category("Watson Marlow 520Du settings")]
        [DisplayName("Volume per revolution [ml/rev]")]
        [Description("Volume per revolution [ml/rev]")]
        public double VolPerRev { get => volPerRev; set => volPerRev = value; }

        [Category("Watson Marlow 520Du settings")]
        [DisplayName("Volume per revolution [ml/rev]")]
        [Description("Volume per revolution [ml/rev]")] //todo calculate it based on head and tube used
        public bool UseTableofVolPerRev { get => useTableofVolPerRev; set => useTableofVolPerRev = value; }

        [Category("Watson Marlow 520Du settings")]
        [DisplayName("Tube size [mm]")]
        [Description("Tube size [mm]")] //todo calculate it based on head and tube used
        public double TubeSize { get => tubeSize; set => tubeSize = value; }
        
    }
}
