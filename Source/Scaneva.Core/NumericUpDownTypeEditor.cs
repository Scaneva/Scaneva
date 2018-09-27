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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Scaneva.Core;

namespace Scaneva
{
    /// <summary>
    /// Range modification for direct edit override
    /// </summary>
    public class NumericUpDownTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Attempt to do them all
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            try
            {
                Type underlyingType = Nullable.GetUnderlyingType(context.PropertyDescriptor.PropertyType) ?? context.PropertyDescriptor.PropertyType;

                string Value = value as string;
                if (!(value is string))
                {
                    Value = Convert.ChangeType(value, underlyingType).ToString();
                }

                decimal decVal;
                if (!decimal.TryParse(Value, out decVal))
                {
                    if (Nullable.GetUnderlyingType(context.PropertyDescriptor.PropertyType) != null)
                    {
                        // it is a nullable we can safely return null
                        return null;
                    }
                    decVal = decimal.One;
                }

                MinMaxAttribute attr = (MinMaxAttribute)context.PropertyDescriptor.Attributes[typeof(MinMaxAttribute)];
                if (attr != null)
                {
                    decVal = attr.PutInRange(decVal);
                }
                return Convert.ChangeType(decVal, underlyingType);
            }
            catch
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                if (destinationType == typeof(string))
                {
                    if ((value == null) && (Nullable.GetUnderlyingType(context.PropertyDescriptor.PropertyType) != null))
                    {
                        return "<Ignore parameter>";
                    }
                    else
                    {
                        return Convert.ChangeType(value, context.PropertyDescriptor.PropertyType).ToString();
                    }
                }
                else
                {
                    return Convert.ChangeType(value, destinationType);
                }
            }
            catch { }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class NumericUpDownTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context == null || context.Instance == null)
                return base.GetEditStyle(context);
            return context.PropertyDescriptor.IsReadOnly ? UITypeEditorEditStyle.None : UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                Type underlyingType = Nullable.GetUnderlyingType(context.PropertyDescriptor.PropertyType) ?? context.PropertyDescriptor.PropertyType;

                if (context == null || context.Instance == null || provider == null)
                    return value;

                //use IWindowsFormsEditorService object to display a control in the dropdown area  
                IWindowsFormsEditorService frmsvr = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (frmsvr == null)
                    return value;

                MinMaxAttribute attr = (MinMaxAttribute)context.PropertyDescriptor.Attributes[typeof(MinMaxAttribute)];
                if (attr != null)
                {
                    NumericUpDown nmr = new NumericUpDown
                    {
                        Size = new Size(60, 120),
                        Minimum = attr.Min,
                        Maximum = attr.Max,
                        Increment = attr.Increment,
                        DecimalPlaces = attr.DecimalPlaces,
                        Value = (value == null) ? attr.PutInRange(Decimal.One) : attr.PutInRange(value)
                    };
                    frmsvr.DropDownControl(nmr);
                    context.OnComponentChanged();

                    return Convert.ChangeType(nmr.Value, underlyingType);
                }
            }
            catch { }
            return value;
        }
    }
}
