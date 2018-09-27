#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="DynamicPropertyNames.cs" company="Scaneva">
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scaneva.Core
{
    /// A generic custom type descriptor for the specified type  
    /// </summary>  
    public sealed class CustomTypeDescriptionProvider<T> : TypeDescriptionProvider where T : DynamicDisplayNames
    {
        /// <summary>  
        /// Constructor  
        /// </summary>  
        public CustomTypeDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
        }

        /// <summary>  
        /// Create and return a custom type descriptor and chains it with the original  
        /// custom type descriptor  
        /// </summary>  
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new DynamicDisplayNameTypeDescriptor<T>(base.GetTypeDescriptor(objectType, instance));
        }
    }

    /// <summary>  
    /// A custom type descriptor which attaches a <see cref="SecuredAttribute"/> to   
    /// an instance of a type which implements <see cref="ISecurable"/>  
    /// </summary>  
    public sealed class DynamicDisplayNameTypeDescriptor<T> : CustomTypeDescriptor where T : DynamicDisplayNames
    {
        /// <summary>  
        /// Constructor  
        /// </summary>  
        public DynamicDisplayNameTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        {
        }

        public override AttributeCollection GetAttributes()
        {
            AttributeCollection baseAttr = base.GetAttributes();

            //if (typeof(DynamicDisplayNames).IsAssignableFrom(typeof(T)))
            //{

            //}

            return baseAttr;

            //Type settingsType = typeof(T).GetInterface(typeof(ISettings).Name);
            //if (settingsType != null)
            //{
            //    ISettings securableInstance = GetPropertyOwner(base.GetProperties().Cast<PropertyDescriptor>().First()) as ISettings;
            //    string[] instanceLevelRoles = securableInstance.GetRoles();
            //    List<Attribute> attributes = new List<Attribute>(base.GetAttributes().Cast<Attribute>());
            //    SecuredAttribute securedAttrib = new SecuredAttribute(instanceLevelRoles);
            //    TypeDescriptor.AddAttributes(securableInstance, securedAttrib);
            //    attributes.Add(securedAttrib);
            //    return new AttributeCollection(attributes.ToArray());
            //}
        }

        /// <summary>  
        /// This method add a new property to the original collection  
        /// </summary>  
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Enumerate the original set of properties and create our new set with it  
            PropertyDescriptorCollection originalProperties = base.GetProperties(attributes);
            List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();
            //Type settingsType = typeof(T).GetInterface("ISettings");
            //if (settingsType != null)
            if (typeof(DynamicDisplayNames).IsAssignableFrom(typeof(T)))
            {
                foreach (PropertyDescriptor pd in originalProperties)
                {
                    DynamicDisplayNames settingsInstance = GetPropertyOwner(pd) as DynamicDisplayNames;
                    //string[] propertyRoles = securableInstance.GetRoles(pd.Name);
                    //SecuredAttribute securedAttrib = new SecuredAttribute(propertyRoles);

                    List<Attribute> newAttribs = new List<Attribute>();
                    foreach(Attribute attr in pd.Attributes)
                    {
                        if (attr.GetType().Equals(typeof(DisplayNameAttribute)))
                        {
                            string oldDispName = ((DisplayNameAttribute)attr).DisplayName;

                            if (settingsInstance.DisplayNameMapping.ContainsKey(oldDispName))
                            {
                                newAttribs.Add(new DisplayNameAttribute(settingsInstance.DisplayNameMapping[oldDispName]));
                            }
                            else
                            {                                
                                newAttribs.Add(attr);
                            }
                        }
                        else
                        {
                            newAttribs.Add(attr);
                        }
                    }
                    
                    // Create a new property and add it to the collection  
                    PropertyDescriptor newProperty = TypeDescriptor.CreateProperty(typeof(T), pd.Name, pd.PropertyType, newAttribs.ToArray());
                    newProperties.Add(newProperty);
                }
                // Finally return the list  
                return new PropertyDescriptorCollection(newProperties.ToArray(), true);
            }
            return base.GetProperties();
        }
    }

}
