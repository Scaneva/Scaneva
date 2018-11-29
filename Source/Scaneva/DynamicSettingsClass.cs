#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="DynamicSettingsClass.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
//  Inspired by: https://stackoverflow.com/questions/313822
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Scaneva
{
    /// <summary>
    /// DynamicSettingsClass (Which is binding to property grid)
    /// </summary>
    public class DynamicSettingsClass: List<CustomProperty>, ICustomTypeDescriptor
	{
        [Browsable(false)]
        public String Name { get; set; }

        [Browsable(false)]
        public object RefObject { get; set; }

		/// <summary>
		/// Remove item from List
		/// </summary>
		/// <param name="Name"></param>
		public void Remove(string Name)
		{
			foreach(CustomProperty prop in this)
			{
				if(prop.Name == Name)
				{
					this.Remove(prop);
					return;
				}
			}
		}

		#region "TypeDescriptor Implementation"
		/// <summary>
		/// Get Class Name
		/// </summary>
		/// <returns>String</returns>
		public String GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		/// <summary>
		/// GetAttributes
		/// </summary>
		/// <returns>AttributeCollection</returns>
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this,true);
		}

		/// <summary>
		/// GetComponentName
		/// </summary>
		/// <returns>String</returns>
		public String GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		/// <summary>
		/// GetConverter
		/// </summary>
		/// <returns>TypeConverter</returns>
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		/// <summary>
		/// GetDefaultEvent
		/// </summary>
		/// <returns>EventDescriptor</returns>
		public EventDescriptor GetDefaultEvent() 
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		/// <summary>
		/// GetDefaultProperty
		/// </summary>
		/// <returns>PropertyDescriptor</returns>
		public PropertyDescriptor GetDefaultProperty() 
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		/// <summary>
		/// GetEditor
		/// </summary>
		/// <param name="editorBaseType">editorBaseType</param>
		/// <returns>object</returns>
		public object GetEditor(Type editorBaseType) 
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes) 
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
            PropertyDescriptor[] newProps = new PropertyDescriptor[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				CustomProperty prop = (CustomProperty) this[i];               
                newProps[i] = new CustomPropertyDescriptor(ref prop, attributes);
            }

			return new PropertyDescriptorCollection(newProps);
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return TypeDescriptor.GetProperties(this, true);
		}
        
        public object GetPropertyOwner(PropertyDescriptor pd) 
		{
			return this;
		}
		#endregion
	
	}

	/// <summary>
	/// Custom property class 
	/// </summary>
	public class CustomProperty
	{
		private string sName = string.Empty;
        private bool bReadOnly = false;
		private bool bVisible = true;
		private object objValue = null;

		public CustomProperty(string sName, string sDescription, string sCategory, object value, Type type, bool bReadOnly, bool bVisible )
		{
			this.sName = sName;
            this.Description = sDescription;
            this.Category = sCategory;
            this.objValue = value;
			this.type = type;
			this.bReadOnly = bReadOnly;
			this.bVisible = bVisible;
		}

		private Type type;
		public Type Type
		{
			get { return type; }
		}

		public bool ReadOnly
		{
			get
			{
				return bReadOnly;
			}
		}

		public string Name
		{
			get
			{
				return sName;
			}
		}

        public string Description { get; set; }

        public string Category { get; set; }

        public bool Visible
		{
			get
			{
				return bVisible;
			}
		}

		public object Value
		{
			get
			{
				return objValue;
			}
			set
			{
				objValue = value;
			}
		}

    }

	/// <summary>
	/// Custom PropertyDescriptor
	/// </summary>
	public class CustomPropertyDescriptor: PropertyDescriptor
	{
		CustomProperty m_Property;
		public CustomPropertyDescriptor(ref CustomProperty myProperty, Attribute [] attrs) :base(myProperty.Name, attrs)
		{
			m_Property = myProperty;
		}

		#region PropertyDescriptor specific
		
		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return null; }
		}

		public override object GetValue(object component)
		{
			return m_Property.Value;
		}

		public override string Description
		{
			get { return m_Property.Description; }
		}
		
		public override string Category
		{
			get	{ return m_Property.Category; }
		}

		public override string DisplayName
		{
			get { return m_Property.Name; }
		}

		public override bool IsReadOnly
		{
			get { return m_Property.ReadOnly; }
		}

		public override void ResetValue(object component)
		{
			//Have to implement
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void SetValue(object component, object value)
		{
			m_Property.Value = value;
		}

		public override Type PropertyType
		{
			get { return m_Property.Type; }
		}

        #endregion


    }
}
