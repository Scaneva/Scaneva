﻿#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpScanArraySettings.cs" company="Scaneva">
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
    public class ScanArraySettings : ISettings
    {
        private Dictionary<string, IPositioner> positioners = new Dictionary<string, IPositioner>();
        private Dictionary<string, IPositioner> tiltpositioners = new Dictionary<string, IPositioner>();

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, IPositioner> Positioners { get => positioners; set => positioners = value; }

        [Category("1. Hardware")]
        [DisplayName("Select positioner")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("Positioners")]
        public string Positioner { get; set; } = null;

        [Category("2. Scanner settings")]
        [DisplayName("Pre-movement hook (µm)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position PreMovementHook { get; set; } = new Position();

        [Category("2. Scanner settings")]
        [DisplayName("Post-movement hook (µm)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position PostMovementHook { get; set; } = new Position();

        [Category("2. Scanner settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Speeds (µm/s)")]
        public Position Speeds { get; set; } = new Position();

        [Category("2. Scanner settings")]
        [DisplayName("Reverse speeds (µm/s)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position ReverseSpeeds { get; set; } = new Position();

        [Category("2. Scanner settings")]
        [DisplayName("Lengths (µm)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Lengths { get; set; } = new Position();

        [Category("2. Scanner settings")]
        [DisplayName("Increments (µm)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Increments { get; set; } = new Position();

        [Category("3. Tilt correction")]
        [DisplayName("Use tilt correction")]
        public bool Tilt { get; set; } = false;

        [Category("3. Tilt correction")]
        [DisplayName("Tilting positioner")]
        [TypeConverter(typeof(DropdownListConverter))]
        [DropdownList("Positioners")]
        public string TiltPositioner { get; set; } = null;

        [Category("3. Tilt correction")]
        [DisplayName("Position 1")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Pos1 { get; set; } = new Position();

        [Category("3. Tilt correction")]
        [DisplayName("Position 2")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Pos2 { get; set; } = new Position();

        [Category("3. Tilt correction")]
        [DisplayName("Position 3")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Position Pos3 { get; set; } = new Position();

        [Category("3. Tilt correction")]
        [DisplayName("z-offset")]
        public float Offset { get; set; } = 0;
    }
}