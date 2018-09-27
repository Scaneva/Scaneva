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
        //these have to be miltiple axes (typically three), each axis has the settings set as below.

        private int intAxisNumber = 0; //(typ. 0=x, 1=y, 2=z)
        private long lngPosition = 0; //current axis position in µm
        private double dblMaxSpeed = 10000; //maximal speed of the stage in µm/s
        private bool blnSign = true; //direction of the axis (defaults: true = right, forward (away from me) and up for X, Y and Z respectively
        private double dblTravel = 100000; //max. travel distance in µm
/*
        public PC_Gnrc_Gnrc_Settings_Axis(int _AxisNumber = 0)
        {
            intAxisNumber = _AxisNumber;
        }
*/
        public int AxisNumber { get => intAxisNumber; set => intAxisNumber = value; }
        public long Position { get => lngPosition; set => lngPosition = value; }
        public double MaxSpeed { get => dblMaxSpeed; set => dblMaxSpeed = value; }
        public bool Sign { get => blnSign; set => blnSign = value; }
        public double Travel { get => dblTravel; set => dblTravel = value; }

        public override string ToString()
        {
            return "Axis " + intAxisNumber;
        }

    }
}
