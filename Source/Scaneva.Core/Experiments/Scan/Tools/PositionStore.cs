#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="PositionStore.cs" company="Scaneva">
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
//  Inspiration from the following sources: 
//  http://www.ruhr-uni-bochum.de/elan/
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Scaneva.Core
{
    public enum enuPStoreStat
    {
        PositionProtected = 0,
        PositionOK = 1,
        PositionInvalid = 2,
        PositionUnreachable = 4,
        PositionUnknown = 8,
        PositionExists = 16,
        PositionCreated = 32,
        PositionUpdated = 64,
        PositionDeleted = 128,
    }

    public sealed class PositionStore
    { 
        //dictionary of positions
        private Dictionary<string, Position> mobjPositions; //Here we store our positions so far the program runs.
        public Dictionary<string, IHWManager> hwStore = null;
        private string positionStoreFile;

        public PositionStore(Dictionary<string, IHWManager> hwStore, string positionStoreFile)
        {
            mobjPositions = new Dictionary<string, Position>();
            this.hwStore = hwStore;
            this.positionStoreFile = positionStoreFile;

            Position mCurrentPosition = new Position();
            Position objpos = new Position();
            objpos = CurrentAbsolutePosition();
            mobjPositions.Add("Home", objpos);
            LoadAll();
        }

        /// <summary>
        /// Save all positions into XML
        /// </summary>
        public void SaveAll()
        {
            // Create an XmlWriter using XmlWriterSettings.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            XmlWriter writer = XmlWriter.Create(positionStoreFile, settings);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer ser = new XmlSerializer(typeof(Position), "");           

            // Serialize HardwareSettings
            writer.WriteStartElement("ScanevaPositionStore", "");
            foreach (KeyValuePair<string, Position> ele in mobjPositions)
            {
                if (ele.Key != "Home") //skip home position, which is never stored permanently right now.
                {
                    writer.WriteStartElement("Position");
                    writer.WriteAttributeString("Name", ele.Key);

                    ser.Serialize(writer, ele.Value, ns);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();

            writer.Close();
        }

        /// <summary>
        /// Load all positions from XML
        /// </summary>
        public void LoadAll()
        {
            try
            {
                XPathDocument xPathDoc = new XPathDocument(positionStoreFile);
                XPathNavigator navigator = xPathDoc.CreateNavigator();

                XPathNodeIterator xPathIterator = navigator.Select("/ScanevaPositionStore/Position");

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlSerializer ser = new XmlSerializer(typeof(Position), "");

                foreach (XPathNavigator compoNav in xPathIterator)
                {
                    string name = compoNav.GetAttribute("Name", "");

                    try
                    {
                        // Create instance
                        Position newPos = (Position)ser.Deserialize(compoNav.SelectSingleNode("Position").ReadSubtree());

                        if (mobjPositions.ContainsKey(name))
                        {
                            mobjPositions[name] = newPos;
                        }
                        else
                        {
                            mobjPositions.Add(name, newPos);
                        }
                    }
                    catch (Exception e)
                    {
                        //log.Add("Error loading Position " + name + " - " + e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                //log.Add("Error loading Position Store - " + e.ToString());
            }
        }

        public void Save(string _pos)
        {
            SaveAll();
            //if (_pos != "Home" && mobjPositions.ContainsKey(_pos)) //skip home position, which is never stored permanently right now.
            //{
            //    Position pos = mobjPositions[_pos];
            //    //Save the given pos into XML
            //}
        }

        public Position GetRelativePosition(string _pname)
        {
            return mobjPositions[_pname];
        }

        public Position GetAbsolutePosition(string _pname) => mobjPositions["Home"].Sum(mobjPositions[_pname]);

        //absolute global position as returned by all positioners
        public Position CurrentAbsolutePosition()
        {
            Position pos = new Position(0, 0, 0);

            var positioners = hwStore.Where(x => typeof(IPositioner).IsAssignableFrom(x.Value.GetType())).Select(x => x);
            foreach (var iterator in positioners)
            {
                IPositioner positioner = iterator.Value as IPositioner;
                pos = pos.Sum(positioner.AbsolutePosition());
            }
            return pos;
        }

        public Position CurrentRelativePosition()
        //global position relative to home position.
        {
            //take the home position into account.
            Position Home = mobjPositions["Home"];
            Position pos = this.CurrentAbsolutePosition().Delta(Home);
            return pos;
        }

        public enuPStoreStat SetPosition(String _pname, Position _pos)
        {
            if (_pname.Length < 1)
            {
                return enuPStoreStat.PositionInvalid;
            }

            if (_pname != "Home")
            //don't forget to take the home position into account (if we are not updating home).
            {
                Position Home = mobjPositions["Home"];
                //store pos relative to home.
                _pos = _pos.Delta(Home);
            }

            if (mobjPositions.ContainsKey(_pname))
            {
                mobjPositions.Remove(_pname);
                mobjPositions.Add(_pname, _pos);
                Save(_pname);
                return enuPStoreStat.PositionUpdated;
            }
            else
            {
                mobjPositions.Add(_pname, _pos);
                Save(_pname);
                return enuPStoreStat.PositionCreated;
            }
        }

        public enuPStoreStat DeletePosition(String _pname)
        {
            if (_pname.Length < 1)
            {
                return enuPStoreStat.PositionInvalid;
            }

            if (_pname == "Home")
            //the home position MUST ALWAYS exist and can't be deleted.
            {
                return enuPStoreStat.PositionProtected;
            }

            if (mobjPositions.ContainsKey(_pname))
            {
                mobjPositions.Remove(_pname);
                //REMOVE FROM XML
                SaveAll();

                return enuPStoreStat.PositionDeleted;                
            }
            else
            {
                return enuPStoreStat.PositionUnknown;
            }
        }

        public ICollection<string> PositionsList()
        //returns an Array of positions names
        {
            return mobjPositions.Keys;
        }


        public bool Exists(string _pname)
        {
            return mobjPositions.ContainsKey(_pname);
        }


        public string HumanReadable()
        {
            return CurrentAbsolutePosition().HumanReadable();
         }

        ~PositionStore()
        {
            mobjPositions.Clear();
            mobjPositions = null;
        }

    }
}

