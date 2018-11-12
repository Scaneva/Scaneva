#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="IHWCompo.cs" company="Scaneva">
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

using System.Collections.Generic;

namespace Scaneva.Core
{
    public enum enuHWStatus
    {
        NotInitialized = 0,
        Ready = 1,
        Busy = 2,
        Error = 4,
    }

    public interface IHWManager
    {
        bool IsEnabled { get; set; }
        enuHWStatus Connect();
        enuHWStatus Initialize();
        enuHWStatus HWStatus { get;}
        void Release();      
    }
}