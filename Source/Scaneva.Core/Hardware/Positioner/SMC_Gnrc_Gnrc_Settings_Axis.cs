#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SMC_Gnrc_Gnrc_Settings_Axis.cs" company="Scaneva">
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

using Scaneva.Core;

namespace Scaneva.Core.Settings
{
    public class SMC_Gnrc_Gnrc_Settings_Axis : ISettings
    {

        [DisplayName("Axis enabled?")]
        [Description("Axis direction with true = direct / false = inverted")]
        public bool Enabled { get; set; } = false;

        [DisplayName("Axis Number [#]")]
        [Description("Axis Number (typ. 1=x, 2=y, 3=z)")]
        public int AxisNumber { get; set; } = 0;

        [DisplayName("Pitch [mm/revolution]")]
        [Description("Distance traveled in mm per revolution")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0.1, 10, 0.1)]
        public double Pitch { get; set; } = 1;

        [DisplayName("Full steps per revolution [#]")]
        [Description("Full steps per revolution as an integer number")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 1600, 1)]
        public long FullSteps { get; set; } = 200;

        [DisplayName("Axis travel distance [mm]")]
        [Description("Maximium absolute axis travel distance in [mm]")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 300000, 1)]
        public double Travel { get; set; } = 20000;

        [DisplayName("Maximal speed [µm/s]")]
        [Description("Maximal allowable speed for the mechanics in µm/s")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10000, 1)]
        public double MaxSpeed { get; set; } = 5000;

        [DisplayName("Fail safe speed [µm/s]")]
        [Description("Fail safe speed for the mechanics in µm/s")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10000, 1)]
        public double FailSafeSpeed { get; set; } = 100; 

        [DisplayName("Acceleration [µm/s2]")]
        [Description("Accelerarion for the mechanics in µm/s2")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100, 1)]
        public double Acceleration { get; set; } = 10;

        [DisplayName("Axis direction [#]")]
        [Description("Axis direction with true = direct / false = inverted")]
        public bool Sign { get; set; } = true;

        [DisplayName("End switch available? [#]")]
        [Description("End switch availability with 1 = yes / 0 = no")]
        public bool Switches { get; set; } = false;

        [DisplayName("Motor current [A]")]
        [Description("Motor operation current in [A]")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0.1, 10, 0.1)]
        public double MotorCurrent{ get; set; } = 1;

        [DisplayName("Motor current reduction [%]")]
        [Description("Motor rest current reduction in [%]")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100, 1)]
        public double MotorCurrentReduction { get; set; } = 50;

        [DisplayName("Gear [x]")]
        [Description("Motor gear value [x]")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100, 0.1)]
        public double MotorGear { get; set; } = 1;

        public override string ToString()
        {
            return "Axis " + AxisNumber;
        }
    }
}
