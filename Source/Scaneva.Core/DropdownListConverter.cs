#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="DropdownListConverter.cs" company="Scaneva">
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;

namespace Scaneva.Core
{
    public class DropdownListAttribute : Attribute
    {
        public DropdownListAttribute(string listfieldName)
        {
            ListfieldName = listfieldName;
        }
        public string ListfieldName { get; set; }
    }

    public class DropdownListConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return true;
        }

        public override System.ComponentModel.TypeConverter.StandardValuesCollection
               GetStandardValues(ITypeDescriptorContext context)
        {
            var obj = context.Instance;
            Type objType = obj.GetType();

            // Get the field that DropdownListConverter was applied to
            string fieldName = context.PropertyDescriptor.Name;
            PropertyInfo pi = objType.GetProperty(fieldName);

            // Get the field containing respective Value List from attributes
            string listfieldName = ((DropdownListAttribute)pi.GetCustomAttribute(typeof(DropdownListAttribute))).ListfieldName;
            pi = objType.GetProperty(listfieldName);

            System.Collections.ICollection valueCollection = pi.GetValue(obj) as System.Collections.ICollection;

            if ((valueCollection != null) && typeof(System.Collections.IDictionary).IsAssignableFrom(valueCollection.GetType()))
            {
                return new StandardValuesCollection(((System.Collections.IDictionary)valueCollection).Keys);
            }

            return new StandardValuesCollection(valueCollection);
        }
    }
}
