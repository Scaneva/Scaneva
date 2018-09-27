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
        [DisplayName("Proportional gain P [µm/source signal unit]")]
        [Description("Proportional factor for controller. For SECM the maximum should be the distance the signal changes from baseline to setpoint.")]
        //(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax()]
        public double P { get; set; } = 1;

        [Category("PID controller")]
        [DisplayName("Integral term I")]
        [Description("With a steady-state deviation this builds up and acts as a long-term corrective.")]
        //(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax()]
        public double? I { get; set; } = null;

        [Category("PID controller")]
        [DisplayName("Derivative term D")]
        [Description("The faster the regulated process changes, the bigger is the  differential component. It should be significantly smaller than the proportional component.")]
        //(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax()]
        public double? D { get; set; } = null;

        [Category("1. PID controller")]
        [DisplayName("Anti-Windup")]
        [Description("Maximum deviation sum for the integral part (for a simple Anti-Windup of the controller).")]
        //(typeof(NumericUpDownTypeConverter))]
        // [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax()]
        public double MaxDeviationSum { get; set; } = 0;
    }
}
