#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpScanArcSettings.cs" company="Scaneva">
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
using System.Xml.Serialization;

namespace Scaneva.Core.Experiments.ScanEva
{
    public class ScanArcSettings : ISettings
    {
        private Dictionary<string, IPositioner> positioners = new Dictionary<string, IPositioner>();
        private Dictionary<string, IPositioner> tiltpositioners = new Dictionary<string, IPositioner>();

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, IPositioner> Positioners { get => positioners; set => positioners = value; }

        string positioner = null;
        string tiltpositioner = null;

        Position preMovementHook = new Position();
        Position postMovementHook = new Position();
        Position speeds = new Position();
        Position reverseSpeeds = new Position();
        Position Parameters = new Position();
        double radius =0;
        double radiusIncrement = 0;
        double angularIncrement = 0;
        bool tilt = false;
        Position tpos1 = new Position();
        Position tpos2 = new Position();
        Position tpos3 = new Position();
        float offset = 0;

        //User interface
        [Category("1. Hardware")]
        [DisplayName("Select positioner")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("Positioners")]
        public string Positioner { get => positioner; set => positioner = value; }

        [Category("2. Scanner settings")]
        [DisplayName("Pre-movement hook (µm)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position PreMovementHook { get => preMovementHook; set => preMovementHook = value; }

        [Category("2. Scanner settings")]
        [DisplayName("Post-movement hook (µm)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position PostMovementHook { get => postMovementHook; set => postMovementHook = value; }

        [Category("2. Scanner settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Speeds (µm/s)")]
        public Position Speeds { get => speeds; set => speeds = value; }

        [Category("2. Scanner settings")]
        [DisplayName("Reverse speeds (µm/s)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position ReverseSpeeds { get => reverseSpeeds; set => reverseSpeeds = value; }

        [Category("2. Scanner settings")]
        [DisplayName("Radius (µm)")]
        public Double Radius { get => radius; set => radius = value; }

        [Category("2. Scanner settings")]
        [DisplayName("Radius increment (µm)")]
        public double RadiusIncrement { get => radiusIncrement; set => radiusIncrement = value; }

        [Category("2. Scanner settings")]
        [DisplayName("Angular increment (µm)")]
        public double AngularIncrement { get => angularIncrement; set => angularIncrement = value; }

        [Category("3. Tilt correction")]
        [DisplayName("Use tilt correction")]
        public bool Tilt { get => tilt; set => tilt = value; }

        [Category("3. Tilt correction")]
        [DisplayName("Tilting positioner")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("Positioners")]
        public string TiltPositioner { get => tiltpositioner; set => tiltpositioner = value; }

        [Category("3. Tilt correction")]
        [DisplayName("Position 1")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Pos1 { get => tpos1; set => tpos1 = value; }

        [Category("3. Tilt correction")]
        [DisplayName("Position 2")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Pos2 { get => tpos2; set => tpos2 = value; }

        [Category("3. Tilt correction")]
        [DisplayName("Position 3")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Pos3 { get => tpos3; set => tpos3 = value; }

        [Category("3. Tilt correction")]
        [DisplayName("z-offset")]
        public float Offset { get => offset; set => offset = value; }
    }
}
