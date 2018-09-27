#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="MathGenStore.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper, Kirill Sliozberg)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Scaneva.Core;

namespace Scaneva.Core
{

    public enum enuMStoreStat
    {
    }

    public sealed class MathGeneratorStore
    {
        //dictionary of MathGens
        private Dictionary<string, MathGenerator> mMathGens; //Here we store our positions so far the program runs.
        private string mathStoreFile;

        public MathGeneratorStore(Dictionary<string, IHWManager> hwStore, string mathStoreFile)
        {
            mMathGens = new Dictionary<string, MathGenerator>();
            this.mathStoreFile = mathStoreFile;
            LoadAll();
        }

        /// <summary>
        /// Load all MathGens from XML
        /// </summary>
        public void LoadAll()
        {
            try
            {
                XPathDocument xPathDoc = new XPathDocument(mathStoreFile);
                XPathNavigator navigator = xPathDoc.CreateNavigator();
                XPathNodeIterator xPathIterator = navigator.Select("/ScanevaMathGensStore/MathGen");

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlSerializer ser = new XmlSerializer(typeof(MathGenerator), "");

                foreach (XPathNavigator compoNav in xPathIterator)
                {
                    string name = compoNav.GetAttribute("Name", "");

                    try
                    {
                        // Create instance
                        MathGenerator newMathGen = (MathGenerator)ser.Deserialize(compoNav.SelectSingleNode("MathGenerator").ReadSubtree());

                        if (mMathGens.ContainsKey(name))
                        {
                            mMathGens[name] = newMathGen;
                        }
                        else
                        {
                            mMathGens.Add(name, newMathGen);
                        }
                    }
                    catch (Exception e)
                    {
                        //log.Add("Error loading MathGen " + name + " - " + e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                //log.Add("Error loading MathGen Store - " + e.ToString());
            }
        }

        /// <summary>
        /// Save all positions into XML
        /// </summary>
        public void SaveAll()
        {
            /*
            // Create an XmlWriter using XmlWriterSettings.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            //XmlWriter writer = XmlWriter.Create(Properties.Settings.Default.MathGenStoreFilePath, settings);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer ser = new XmlSerializer(typeof(Position), "");

            // Serialize HardwareSettings
            writer.WriteStartElement("ScanevaMathGenStore", "");
            foreach (KeyValuePair<string, Position> ele in mMathGens)
            {
                writer.WriteStartElement("MathGen");
                writer.WriteAttributeString("Name", ele.Key);
                ser.Serialize(writer, ele.Value, ns);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Close();
    */
        }

        public MathGenerator GetMathGen(string _mname)
        {
            if (mMathGens.ContainsKey(_mname))
            {
                MathGenerator mathgen = new MathGenerator();
                mathgen.LoadParameters();
                return mathgen;
            }
            return null;
        }

        public void Save(string _pos)
        {
            SaveAll();
        }
    }
}

