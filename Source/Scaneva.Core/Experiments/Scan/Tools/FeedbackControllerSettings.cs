#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="FeedbackControllerSettings.cs" company="Scaneva">
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
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Scaneva.Core;

namespace Scaneva.Core
{
    public class FeedbackControllerSettings : DynamicDisplayNames, ISettings
    {
        private Dictionary<string, TransducerChannel> transducerChannels = new Dictionary<string, TransducerChannel>();
                
        string channel = null;
        Position safetyLimits;
        public PIDControllerSettings PIDController { get; set; } = new PIDControllerSettings();

        public enum enuFeedbackMode
        {
            Absolute = 0,
            Relative = 1,
            Normalized = 2,
        }

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, TransducerChannel> TransducerChannels
        {
            get => transducerChannels;
            set
            {
                transducerChannels = value;
                updateDisplayNames();
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, IPositioner> Positioners { get; set; } = new Dictionary<string, IPositioner>();

        //User interface
        [Category("1. Hardware")]
        [DisplayName("Select positioner")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("Positioners")]
        public string Positioner { get; set; } = null;

        [Category("1. Hardware")]
        [DisplayName("Positioner speeds [µm/s]")]
        [Description("Specify the positioner speed in [µm/s].")]
        public Position Speeds { get; set; }

       

        public override string ToString()
        {
            return "Feedback controller settings";
        }

        [Category("1. Hardware")]
        [DisplayName("Select signal source")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("TransducerChannels")]
        public string Channel
        {
            get => channel;
            set
            {
                channel = value;
                updateDisplayNames();
            }
            //todo: add averaging control!
        }

        [Category("1. Hardware")]
        [DisplayName("Source signal averaging [#]")]
        [Description("Specify the number of sample being averaged for the selected transducer.")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, int.MaxValue)]
        public int Averaging { get; set; } = 1;

        [Category("2. Feedback controller")]
        [DisplayName("Feedback controller mode")]
        [Description("Feedback controller mode.")]
        public enuFeedbackMode ControllerMode { get; set; } = enuFeedbackMode.Absolute;

        [Category("2. Feedback controller")]
        [DisplayName("Minimum setpoint")]
        [Description("Specify the lower limit of the setpoint in signal source units.")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(double.MinValue, double.MaxValue)]
        public double MinSetpoint { get; set; } = 0;

        [Category("2. Feedback controller")]
        [DisplayName("Maximum setpoint")]
        [Description("Specify the upper limit of the setpoint in signal source units.")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(double.MinValue, double.MaxValue)]
        public double MaxSetpoint { get; set; } = 1;

        [Category("2. Feedback controller")]
        [DisplayName("Number of bulk signal samples")]
        [Description("Specify the number of bulk signal samples to be acquired.")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, long.MaxValue, 1)]
        public long BulkSamples { get; set; } = 10;

        [Category("2. Feedback controller")]
        [DisplayName("Time out")]
        [Description("Specify the time out for the feedback controller in [ms].")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, int.MaxValue, 1)]
        public int TimeOut { get; set; } = 30000;

        [Category("2. Feedback controller")]
        [DisplayName("Loop delay [ms]")]
        [Description("Specify the delay between feedback loops in [ms].")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, int.MaxValue, 1)]
        public int LoopDelay { get; set; } = 20;

        [Category("3. Post-approach feedback")]
        [DisplayName("Enable post-approach feedback control")]
        [Description("Enable post-approach feedback control?")]
        public bool PostAppFBC { get; set; } = false;

        [Category("3. Post-approach feedback")]
        [DisplayName("Loop delay at setpoint [ms]")]
        [Description("Specify the delay between feedback loops in [ms].")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, int.MaxValue, 1)]
        public int SPLoopDelay { get; set; } = 1000;

        [Category("3. Post-approach feedback")]
        [DisplayName("Time out [ms]")]
        [Description("Specify the time out for the post-approach feedback controller in [ms].")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, int.MaxValue, 1)]
        public int PostAppTimeOut { get; set; } = 30000;



        /*
        [Category("2. Feedback controller")]
        public bool ExitFeedback { get; set; } = false;

        [Category("2. Feedback controller")]
        public bool PreExitFeedback { get; set; } = false;

        [Category("2. Feedback controller")]
        public bool ReacquireBulk { get; set; } = false;
        */

        [Category("3. Safety limits")]
        [Description("Specify the lower safety limit of the feedback signal in signal source units.")]
        // [TypeConverter(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 0.1)]
        public double? MinSafetyLimit { get; set; } = null;

        [Category("3. Safety limits")]
        [Description("Specify the upper safety limit of the feedback signal in signal source units.")]
        // [TypeConverter(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 0.1)]
        public double? MaxSafetyLimit { get; set; } = null;

        [Category("3. Safety limits")]
        [Description("Specify the lower positioner safety limit in µm relative to home position")]
        // [TypeConverter(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 0.1)]
        public double? MinPositionerSafetyLimit { get; set; } = null;

        [Category("3. Safety limits")]
        [Description("Specify the upper positioner safety limit in µm relative to home position")]
        // [TypeConverter(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 0.1)]
        public double? MaxPositionerSafetyLimit { get; set; } = null;

        private void updateDisplayNames()
        {
            string units = "";

            if ((channel != null) && (transducerChannels.ContainsKey(channel)))
            {
                if (transducerChannels[channel].Prefix != enuPrefix.none)
                {
                    units += transducerChannels[channel].Prefix;
                }
                units += transducerChannels[channel].Unit;
            }

            SetDisplayName("Min Setpoint", "Min Setpoint (" + units + ")");
            SetDisplayName("Max Setpoint", "Max Setpoint (" + units + ")");
        }
    }
}
