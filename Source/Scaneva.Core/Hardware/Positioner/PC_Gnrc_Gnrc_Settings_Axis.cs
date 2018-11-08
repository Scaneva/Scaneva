using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scaneva.Core;

//Purpose: manages settings of piezo controller (PC)

namespace Scaneva.Core.Settings
{
    public class PC_Gnrc_Gnrc_Settings_Axis : ISettings
    {
        public int AxisNumber { get; set; } = 0;
        public long Position { get; set; } = 0;
        public double MaxSpeed { get; set; } = 10000;
        public bool Sign { get; set; } = true;
        public double Travel { get; set; } = 100000;

        public override string ToString()
        {
            return "Axis " + AxisNumber;
        }

    }
}
