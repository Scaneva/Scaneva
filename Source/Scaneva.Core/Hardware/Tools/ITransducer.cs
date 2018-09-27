#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ITransducer.cs" company="Scaneva">
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

using System.Collections.Generic;

namespace Scaneva.Core
{
    public enum enuTransducerType
    {
        DAC = 1,
        ADC = 2,
        DO = 4,
        DI = 8,
        Potentiostat = 16,
        Lamp = 32,
        Positioner = 64,
        Pump = 128,
        Sensor = 256,
    }

    public interface ITransducer
    {
        enuTransducerType TransducerType { get; }
        List<TransducerChannel> Channels { get; }
        double GetValue(TransducerChannel channel);
        double GetAveragedValue(TransducerChannel channel);
        int Averaging { get; set; }
        void SetValue(TransducerChannel channel, double _value);
    }
}