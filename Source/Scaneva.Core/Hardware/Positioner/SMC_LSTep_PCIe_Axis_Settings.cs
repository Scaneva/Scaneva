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
    public class SMC_LStep_Axis_Settings : SMC_Gnrc_Gnrc_Settings_Axis, ISettings
    {
        [Category("Special axis settings")]
        [DisplayName("Re-calibrate the axis after next start?")]
        [Description("Shall the axis be re-calibrated during the next program start? You will be asked, " +
                    "whether you still want to re-calibrate the axis by the next program start. The setting will be automatically deactivated then.")]
        public bool Recalibrate { get; set; } = false;

        [DisplayName("Re-measure the axis after next start?")]
        [Description("Shall the axis be re-measured during the next program start? You will be asked, " +
                    "whether you still want to re-measure the axis by the next program start. The setting will be automatically deactivated then.")]
        public bool Remeasure { get; set; } = false;


    }
}
