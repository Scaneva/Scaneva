#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PIDControllerSettings.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg)
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
//  Inspiration from the following sources: 
//  http://www.ruhr-uni-bochum.de/elan/
// --------------------------------------------------------------------------------------------------------------------
#endregion


using System.ComponentModel;
using System.Drawing.Design;

namespace Scaneva.Core
{
    public class PIDControllerSettings : DynamicDisplayNames, ISettings
    {

        public PIDControllerSettings()
        {
            TypeDescriptor.AddProvider(new CustomTypeDescriptionProvider<PIDControllerSettings>(TypeDescriptor.GetProvider(typeof(PIDControllerSettings))), this);
        }

        public override string ToString()
        {
            return "PID controller settings";
        }

        [Category("PID controller")]
        [DisplayName("Proportional band []")]
        [Description("Maximal deviation which is taken into account per correction step. " +
            "This should be equal or less than the proportional range of the controlled process variable " +
            "around the setpoint. If the setpoint is 0.7 and the signal proportional between 0.5 and 0.9, " +
            "set it <=0.4. If the setpoint is 0.4 and the signal proportional between 0.3 and 1, set it <=0.1")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(double.MinValue, double.MaxValue)]
        public double PB { get; set; } = 1;

        [Category("PID controller")]
        [DisplayName("Proportional gain P [µm/source signal unit]")]
        [Description("Proportional factor for controller. Maximum should be the distance " +
            "the signal changes from baseline to setpoint. This is the regulation at the maximum/minimum " +
            "of the proportional band. E.g. ProportionalBand is set to 0.2 and we expect the signal to change " +
            "by 0.2 with correction of 10, then set Kp to less than 10 or be risky. Only positive values - " +
            "polarity of the process in changed by ProcessState().")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, double.MaxValue)]
        public double Kp { get; set; } = 1;

        [Category("PID controller")]
        [DisplayName("Integral term I")]
        [Description("With a steady-state deviation this builds up and acts as a long-term corrective. " +
            "Only positive values - polarity of the process in changed by ProcessState().")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, double.MaxValue)]
        public double? Ki { get; set; } = null;

        [Category("PID controller")]
        [DisplayName("Derivative term D")]
        [Description("The faster the regulated process changes, the bigger is the  differential component. " +
            "It should be significantly smaller than the proportional component. " +
            "Only positive values - polarity of the process in changed by ProcessState().")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, double.MaxValue)]
        public double? Kd { get; set; } = null;

        [Category("PID controller")]
        [DisplayName("Anti-Windup")]
        [Description("Maximum deviation sum for the integral part (for a simple Anti-Windup of the controller).")]
        //(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax()]
        public double MaxDeviationSum { get; set; } = 0;
    }
}
