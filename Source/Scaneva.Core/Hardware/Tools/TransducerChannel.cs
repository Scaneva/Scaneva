#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="TransducerChannel.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
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


namespace Scaneva.Core
{
    public enum enuPrefix
    {
        a = -18,
        f = -15,
        p = -12,
        n = -9,
        µ = -6,
        m = -3,
        c = -2,
        d = -1,
        none = 0,
        da = 1,
        h = 2,
        k = 3,
        M = 6,
        G = 9,
        T = 12,
        P = 15,
        E = 18
    }

    public enum enuChannelType
    {
        passive = 0,
        active = 1,
        mixed = 2
    }

    public enum enuSensorStatus
    {
        OK = 1,       //sensor is enabled and available
        Overload = 2,     //sensor was overloaded during last measurement
    }


    public class TransducerChannel
    {
        private ITransducer parent;

        public TransducerChannel(ITransducer parent, string name, string unit, enuPrefix prefix, enuChannelType channelType, enuSensorStatus status)
        {
            this.parent = parent;
            Name = name;
            Unit = unit;
            Prefix = prefix;
            ChannelType = channelType;
            Status = status;
            Averaging = Averaging;
        }

        public string Unit { get; } = "";
        public enuPrefix Prefix { get; } = enuPrefix.none;
        public enuChannelType ChannelType { get; }
        public string Name { get; } = "";
        public enuSensorStatus Status { get; set; }
        public int Averaging { get; set; } = 1;

        public double GetValue()
        {
            return parent.GetValue(this);
        }

        public double GetAveragedValue()
        {
            return parent.GetAveragedValue(this);
        }

        public void SetValue(double _value)
        {
            parent.SetValue(this, _value);
        }

        public override string ToString()
        {
            return ((IHWManager)parent).ToString() + "." + Name;
        }
    }
}
