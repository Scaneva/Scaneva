#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="NumericUpDownTypeEditor.cs" company="Smurf-IV">
// 
//  Copyright (C) 2012 Simon Coghlan (Aka Smurf-IV)
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//   any later version.
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
//    https://liquesce.wordpress.com
//    http://social.msdn.microsoft.com/Forums/da-DK/netfxbcl/thread/370ce9d3-fc44-4cdc-9c76-dd913c9b572f
//    http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/afcd4dd5-5538-433b-8cac-78c081ee16b6
//    http://social.msdn.microsoft.com/Forums/en/winforms/thread/b9325e61-767b-43c8-96a2-e0caef2cecad
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;

namespace Scaneva.Core
{
    // ReSharper disable MemberCanBePrivate.Global
    /// <summary>
    /// Attribute to allow ranges to be added to the numeric updowner
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MinMaxAttribute : Attribute
    {
        public decimal Min { get; private set; }
        public decimal Max { get; private set; }
        public decimal Increment { get; private set; }
        public int DecimalPlaces { get; private set; }

        /// <summary>
        /// Use to make a simple UInt16 max. Starts at 0, increment = 1
        /// </summary>
        /// <param name="max"></param>
        public MinMaxAttribute(UInt16 max)
           : this((int)UInt16.MinValue, max)
        {
        }

        /// <summary>
        /// Use to make a simple integer (or default conversion) based range.
        /// default inclrement is 1
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="increment"></param>
        public MinMaxAttribute(int min, int max, int increment = 1)
           : this((double)min, max, increment)
        {
        }

        /// <summary>
        /// Set the Min, Max, increment, and decimal places to be used.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="increment"></param>
        /// <param name="decimalPlaces"></param>
        public MinMaxAttribute(double min, double max, double increment = 1d, int decimalPlaces = 0)
        {
            Min = Convert.ToDecimal(min);
            Max = Convert.ToDecimal(max);
            Increment = Convert.ToDecimal(increment);
            DecimalPlaces = decimalPlaces;
        }

        /// <summary>
        /// Validation function to check if the value is withtin the range (inclusive)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsInRange(object value)
        {
            decimal checkedValue = (decimal)Convert.ChangeType(value, typeof(decimal));
            return ((checkedValue <= Max)
               && (checkedValue >= Min)
               );
        }

        /// <summary>
        /// Takes the value and adjusts if it is out of bounds.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public decimal PutInRange(object value)
        {
            decimal checkedValue = (decimal)Convert.ChangeType(value, typeof(decimal));
            if (checkedValue > Max)
                checkedValue = Max;
            else if (checkedValue < Min)
                checkedValue = Min;
            return checkedValue;
        }
    }
    // ReSharper restore MemberCanBePrivate.Global

}
