#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ParametrizableObject.cs" company="Scaneva">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.ComponentModel;
using Scaneva.Tools;

namespace Scaneva.Core
{
    [DefaultPropertyAttribute("Settings")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ParametrizableObject
    {
        protected ISettings settings = null;
        protected LogHelper log = null;
        private string name = "";

        public ParametrizableObject(LogHelper log)
        {
            this.log = log;
        }

        public ISettings GetSettings()
        {
            return settings;
        }

        public string Name { get => name; set => name = value; }

        public virtual void ParameterChanged(string name)
        {
            log.Add("Parameter changed: " + name);
        }

        public virtual void SerializeParameterValues(XmlWriter writer)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer ser = new XmlSerializer(settings.GetType(), "");
            ser.Serialize(writer, settings, ns);
        }
            
        public virtual void DeserializeParameterValues(IXPathNavigable node, Dictionary<string, Type> availableExperiments)
        {
            try
            {
                XPathNavigator navigator = node.CreateNavigator();
                string settingsClassName = settings.GetType().Name;

                XPathNavigator settingsNode = navigator.SelectSingleNode(settingsClassName);

                XmlSerializer ser = new XmlSerializer(settings.GetType(), "");
                settings = (ISettings)ser.Deserialize(settingsNode.ReadSubtree());
            }
            catch (Exception e)
            {
                log.Add("Deserializing Parameter Values Failed: " + e.ToString(), "Error");
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}