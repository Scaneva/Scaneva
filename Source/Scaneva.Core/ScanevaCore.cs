#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ScanevaCore.cs" company="Scaneva">
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
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using System.ComponentModel;
using Scaneva.Core.Experiments;
using Scaneva.Tools;
using System.IO;

namespace Scaneva.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ScanevaCore
    {
        public Dictionary<string, IHWManager> hwStore = new Dictionary<string, IHWManager>();
        public PositionStore positionStore = null;
        public MathGeneratorStore mathGenStrore = null;
        public Dictionary<string, Type> availableHWTypes = null;
        public Dictionary<string, Type> availableExperiments = null;
        public LogHelper log;

        public List<IExperiment> scanMethod = new List<IExperiment>();

        public ScanevaCoreSettings Settings = null;

        public ScanevaCore() : this(new ScanevaCoreSettings())
        {
        }            

        public ScanevaCore(ScanevaCoreSettings settings)
        {
            this.Settings = settings;
            log = new LogHelper("", Path.Combine(Settings.LogDirectory, DateTime.Now.ToString("yyyy_MM_dd")));

            availableHWTypes = GetAvailableTypes(typeof(IHWManager));
            availableExperiments = GetAvailableTypes(typeof(IExperiment));
        }

        private Dictionary<string, Type> GetAvailableTypes(Type implementedInterface)
        {
            Dictionary<string, Type> hwTypes = new Dictionary<string, Type>();

            // find all Types implementing IHWCompo
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => implementedInterface.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            // Iterate and Add
            foreach(Type t in types)
            {
                string displayName = t.Name;

                // Check for DisplayName Atttribute
                var nameAttribute = t.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
                if (nameAttribute != null)
                {
                    displayName = nameAttribute.DisplayName;
                }

                hwTypes.Add(displayName, t);
            }

            return hwTypes;
        }

        public void AddHardware(string hwName, string hwTypeDisplayName)
        {
            // Create new HW and add to listBox
            Type t = availableHWTypes[hwTypeDisplayName];

            IHWManager hwInstance = (IHWManager)Activator.CreateInstance(t, log);
            (hwInstance as ParametrizableObject).Name = hwName;
            hwStore.Add(hwName, hwInstance);
        }

        public void RemoveHardware(string hwName)
        {
            hwStore.Remove(hwName);
        }

        public void InitializeAllHardware()
        {
            foreach (IHWManager compo in hwStore.Values)
            {
                if (compo.IsEnabled)
                {
                    //if (compo.HWStatus == enuHWStatus.Ready)
                    {
                        compo.Connect();
                        compo.Initialize();
                    }
                }
            }

            if (positionStore == null)
            {
                positionStore = new PositionStore(hwStore, Settings.PositionStoreFilePath);
            }
        }

        public void ReleaseAllHardware()
        {
            foreach (IHWManager compo in hwStore.Values)
            {
                compo.Release();
            }
        }

        public void BuildMethodTree(TreeNodeCollection addInMe)
        {
            addInMe.Clear();
            foreach (IExperiment exp in scanMethod)
            {
                BuildMethodTree(exp, addInMe);
            }
        }

        private void BuildMethodTree(IExperiment exp, TreeNodeCollection addInMe)
        {
            TreeNode curNode = new TreeNode(((ParametrizableObject)exp).Name);
            curNode.Tag = exp;
            addInMe.Add(curNode);

            if (typeof(ExperimentContainer).IsAssignableFrom(exp.GetType()))
            {
                ExperimentContainer container = exp as ExperimentContainer;
                foreach (IExperiment subexp in container)
                {
                    BuildMethodTree(subexp, curNode.Nodes);
                }
            }
        }

        public TreeNode AddExperimentToScanMethod(TreeNodeCollection addInMe, TreeNode insertAfter, string expName, string expTypeDisplayName)
        {
            // Create new HW and add to listBox
            Type t = availableExperiments[expTypeDisplayName];

            IExperiment expInstance = (IExperiment)Activator.CreateInstance(t, log);
            expInstance.HWStore = hwStore;
            expInstance.PositionStore = positionStore;
            ((ParametrizableObject)expInstance).Name = expName;

            TreeNode newNode = new TreeNode(expName);
            newNode.Tag = expInstance;

            if (insertAfter == null)
            {
                // insert at ende
                scanMethod.Add(expInstance);
                addInMe.Add(newNode);
            }
            // check if selected node is container
            else if (typeof(ExperimentContainer).IsAssignableFrom(insertAfter.Tag.GetType()))
            {
                // append at end of container
                ((ExperimentContainer)insertAfter.Tag).Add(expInstance);
                insertAfter.Nodes.Add(newNode);
            }
            else if (insertAfter.Parent == null)
            {
                // insert after current Experiment
                int index = insertAfter.Index + 1;
                scanMethod.Insert(index, expInstance);
                addInMe.Insert(index, newNode);
            }
            else if (typeof(ExperimentContainer).IsAssignableFrom(insertAfter.Parent.Tag.GetType()))
            {
                // insert after current Experiment in Container
                int index = insertAfter.Index + 1;
                ((ExperimentContainer)insertAfter.Parent.Tag).Insert(index, expInstance);
                insertAfter.Parent.Nodes.Insert(index, newNode);
            }
            else
            {
                // insert at ende
                scanMethod.Add(expInstance);
                addInMe.Add(newNode);
            }

            return newNode;
        }

        public void RemoveExperimentFromScanMethod(TreeNodeCollection addInMe, TreeNode expNode)
        {
            if ((expNode != null) && (expNode.Parent == null))
            {
                scanMethod.Remove(expNode.Tag as IExperiment);
                addInMe.Remove(expNode);
            }
            else if ((expNode != null) && (expNode.Parent != null) && (typeof(ExperimentContainer).IsAssignableFrom(expNode.Parent.Tag.GetType())))
            {
                TreeNode parent = expNode.Parent;
                ((ExperimentContainer)expNode.Parent.Tag).Remove(expNode.Tag as IExperiment);
                parent.Nodes.Remove(expNode);
            }
        }

        public void MoveExperimentInScanMethodUp(TreeNodeCollection addInMe, TreeNode expNode)
        {
            if ((expNode != null) && (expNode.Parent == null) && (expNode.Index > 0))
            {
                // Not in container and Index > 0
                scanMethod.Remove(expNode.Tag as IExperiment);
                scanMethod.Insert(expNode.Index - 1, expNode.Tag as IExperiment);
                addInMe.Remove(expNode);
                addInMe.Insert(expNode.Index - 1, expNode);
            }
            else if ((expNode != null) && (expNode.Parent != null) && (typeof(ExperimentContainer).IsAssignableFrom(expNode.Parent.Tag.GetType())) && (expNode.Index > 0))
            {
                ExperimentContainer container = expNode.Parent.Tag as ExperimentContainer;
                TreeNode parent = expNode.Parent;
                container.Remove(expNode.Tag as IExperiment);
                container.Insert(expNode.Index - 1, expNode.Tag as IExperiment);
                parent.Nodes.Remove(expNode);
                parent.Nodes.Insert(expNode.Index - 1, expNode);
            }
        }

        public void MoveExperimentInScanMethodDown(TreeNodeCollection addInMe, TreeNode expNode)
        {
            if ((expNode != null) && (expNode.Parent == null) && (expNode.Index < (scanMethod.Count - 1)))
            {
                // Not in container and Index > 0
                scanMethod.Remove(expNode.Tag as IExperiment);
                scanMethod.Insert(expNode.Index + 1, expNode.Tag as IExperiment);
                addInMe.Remove(expNode);
                addInMe.Insert(expNode.Index + 1, expNode);
            }
            else if ((expNode != null) && (expNode.Parent != null) && (typeof(ExperimentContainer).IsAssignableFrom(expNode.Parent.Tag.GetType())) && (expNode.Index < (expNode.Parent.GetNodeCount(false) - 1)))
            {
                ExperimentContainer container = expNode.Parent.Tag as ExperimentContainer;
                TreeNode parent = expNode.Parent;
                container.Remove(expNode.Tag as IExperiment);
                container.Insert(expNode.Index + 1, expNode.Tag as IExperiment);
                parent.Nodes.Remove(expNode);
                parent.Nodes.Insert(expNode.Index + 1, expNode);
            }
        }

        public void MoveExperimentInScanMethodIncLevel(TreeNodeCollection addInMe, TreeNode expNode)
        {
            if ((expNode != null) && (expNode.Parent == null) && (expNode.Index > 0) &&
                (typeof(ExperimentContainer).IsAssignableFrom(addInMe[expNode.Index-1].Tag.GetType())))
            {
                // Not in container and below a container item
                ExperimentContainer newContainer = addInMe[expNode.Index - 1].Tag as ExperimentContainer;
                TreeNode newParent = addInMe[expNode.Index - 1];
                scanMethod.Remove(expNode.Tag as IExperiment);
                newContainer.Add(expNode.Tag as IExperiment);
                addInMe.Remove(expNode);
                newParent.Nodes.Add(expNode);
            }
            else if ((expNode != null) && (expNode.Parent != null) && (typeof(ExperimentContainer).IsAssignableFrom(expNode.Parent.Tag.GetType())) &&
                (expNode.Index > 0) &&
                (typeof(ExperimentContainer).IsAssignableFrom(expNode.Parent.Nodes[expNode.Index - 1].Tag.GetType())))
            {
                // in container and below a container item
                ExperimentContainer newContainer = expNode.Parent.Nodes[expNode.Index - 1].Tag as ExperimentContainer;
                TreeNode newParent = expNode.Parent.Nodes[expNode.Index - 1];
                ((ExperimentContainer)expNode.Parent.Tag).Remove(expNode.Tag as IExperiment);
                newContainer.Add(expNode.Tag as IExperiment);
                expNode.Parent.Nodes.Remove(expNode);
                newParent.Nodes.Add(expNode);
            }
        }

        public void MoveExperimentInScanMethodDecLevel(TreeNodeCollection addInMe, TreeNode expNode)
        {
            if ((expNode != null) && (expNode.Parent != null) && 
                (typeof(ExperimentContainer).IsAssignableFrom(expNode.Parent.Tag.GetType())))
            {
                // Inside a container
                ExperimentContainer container = expNode.Parent.Tag as ExperimentContainer;
                TreeNode parentNode = expNode.Parent;

                // Is level2 or higher?
                if ((parentNode.Parent != null) && (typeof(ExperimentContainer).IsAssignableFrom(parentNode.Parent.Tag.GetType())))
                {
                    ExperimentContainer ppContainer = parentNode.Parent.Tag as ExperimentContainer;
                    TreeNode ppNode = parentNode.Parent;

                    container.Remove(expNode.Tag as IExperiment);
                    ppContainer.Insert(parentNode.Index + 1, expNode.Tag as IExperiment);
                    parentNode.Nodes.Remove(expNode);
                    ppNode.Nodes.Insert(parentNode.Index + 1, expNode);
                }
                else
                {
                    container.Remove(expNode.Tag as IExperiment);
                    scanMethod.Insert(parentNode.Index + 1, expNode.Tag as IExperiment);
                    parentNode.Nodes.Remove(expNode);
                    addInMe.Insert(parentNode.Index + 1, expNode);
                }
            }
        }

        #region Run Scan Method and Events

        private int experimentRunning = -1;
        private string experimentRunningName = "";

        private string scanMethodResultsPath = "";

        private bool cancelRunningExperiments = false;

        public void RunScanMethod()
        {
            if (scanMethod.Count > 0)
            {
                cancelRunningExperiments = false;
                scanMethodResultsPath = Path.Combine(Settings.ScanResultDirectory, DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss"));
                RunNextExperiment(0);
            }
        }

        public void CancelScanMethod()
        {
            // Set Cancel Flag
            cancelRunningExperiments = true;

            // Abort currently running Experiment
            if ((experimentRunning != -1) && (scanMethod != null) && (scanMethod.Count > experimentRunning))
            {
                IExperiment exp = scanMethod[experimentRunning];
                exp.Abort();
            }
        }

        private void RunNextExperiment(int expIndex)
        {
            IExperiment exp = (IExperiment)scanMethod[expIndex];

            log.Add("Configuring " + ((ParametrizableObject)exp).Name + "...");
            enExperimentStatus expStatus = exp.Configure(null, scanMethodResultsPath);
            if ((expStatus != enExperimentStatus.OK) && (expStatus != enExperimentStatus.Idle))
            {
                log.Warning("Experiment Sequence Aborted due to exp.Configure() returning: " + expStatus);
                OnNotifyScanEnded(new ExperimentEndedEventArgs(enExperimentStatus.Error, null));
            }

            exp.NotifyExperimentDataUpdated -= Exp_NotifyExperimentDataUpdated;
            exp.NotifyExperimentDataUpdated += Exp_NotifyExperimentDataUpdated;
            exp.NotifyExperimentEnded -= Exp_NotifyExperimentEnded;
            exp.NotifyExperimentEnded += Exp_NotifyExperimentEnded;
            experimentRunning = expIndex;
            experimentRunningName = (scanMethod[experimentRunning] as ParametrizableObject).Name;

            log.Add("Running " + ((ParametrizableObject)exp).Name + "...");
            expStatus = exp.Run();
            if ((expStatus != enExperimentStatus.OK) && (expStatus != enExperimentStatus.Running))
            {
                exp.NotifyExperimentDataUpdated -= Exp_NotifyExperimentDataUpdated;
                exp.NotifyExperimentEnded -= Exp_NotifyExperimentEnded;
                try
                {
                    // just in case Experiment was started
                    exp.Abort();
                }
                catch (Exception e)
                {

                }
                log.Warning("Experiment Sequence Aborted due to exp.Run() returning: " + expStatus);
                OnNotifyScanEnded(new ExperimentEndedEventArgs(enExperimentStatus.Error, null));
            }
        }

        private void Exp_NotifyExperimentEnded(object sender, ExperimentEndedEventArgs e)
        {
            IExperiment exp = (IExperiment)scanMethod[experimentRunning];
            
            // More Experiments to Run and not canceled? 
            if ((scanMethod.Count > (experimentRunning + 1)) && (!cancelRunningExperiments))
            {
                // Was there an error?
                if (e.Status == enExperimentStatus.Error)
                {
                    OnNotifyScanEnded(new ExperimentEndedEventArgs(enExperimentStatus.Error, null));
                }
                else
                {
                    RunNextExperiment(experimentRunning + 1);
                }
            }
            else
            {
                // Notify listener of Scan End
                OnNotifyScanEnded(new ExperimentEndedEventArgs(cancelRunningExperiments ? enExperimentStatus.Aborted : enExperimentStatus.Completed, null));
            }
        }

        private void Exp_NotifyExperimentDataUpdated(object sender, ExperimentDataEventArgs e)
        {
            // Just forward the Data (e.g. to GUI)
            OnNotifyScanDataUpdated(new ScanDataEventArgs(e.Data, experimentRunningName, e.IsUpdatedData));
        }

        public event EventHandler<ExperimentEndedEventArgs> NotifyScanEnded;

        private void OnNotifyScanEnded(ExperimentEndedEventArgs e)
        {
            if (NotifyScanEnded != null) NotifyScanEnded(this, e);
        }

        public event EventHandler<ScanDataEventArgs> NotifyScanDataUpdated;

        private void OnNotifyScanDataUpdated(ScanDataEventArgs e)
        {
            if (NotifyScanDataUpdated != null) NotifyScanDataUpdated(this, e);
        }

        #endregion

        public void SaveScanMethod(string Filename)
        {
            // Create an XmlWriter using XmlWriterSettings.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            XmlWriter writer = XmlWriter.Create(Filename, settings);

            // Serialize HardwareSettings
            writer.WriteStartElement("ScanevaScanMethod", "");
            foreach (IExperiment child in scanMethod)
            {
                var compo = child as ParametrizableObject;

                writer.WriteStartElement("IExperiment");
                writer.WriteAttributeString("Class", compo.GetType().Name);
                writer.WriteAttributeString("Name", compo.Name);

                compo.SerializeParameterValues(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.Close();
        }

        public void LoadScanMethod(string Filename)
        {
            try
            {
                XPathDocument xPathDoc = new XPathDocument(Filename);
                XPathNavigator navigator = xPathDoc.CreateNavigator();

                XPathNodeIterator xPathIterator = navigator.Select("/ScanevaScanMethod/IExperiment");

                // clear scanMethod
                scanMethod.Clear();

                foreach (XPathNavigator compoNav in xPathIterator)
                {
                    string name = compoNav.GetAttribute("Name", "");
                    string className = compoNav.GetAttribute("Class", "");

                    try
                    {
                        // Create instance
                        Type t = availableExperiments.Values.FirstOrDefault(x => x.Name == className);

                        IExperiment expInstance = (IExperiment)Activator.CreateInstance(t, log);
                        ParametrizableObject compo = expInstance as ParametrizableObject;
                        expInstance.HWStore = hwStore;
                        expInstance.PositionStore = positionStore;
                        compo.Name = name;
                        scanMethod.Add(expInstance);

                        if (compo.GetType().Name == className)
                        {
                            compo.DeserializeParameterValues(compoNav, availableExperiments);
                            expInstance.HWStore = hwStore;
                            expInstance.PositionStore = positionStore;
                            compo.ParameterChanged("Settings Loaded");
                        }
                        else
                        {
                            log.Add("Found settings with name '" + name + "' and class '" + className + "' but no matching Experiment. Settings are ignored.", "Warning");
                        }
                    }
                    catch (Exception e)
                    {
                        log.Add("Error loading Experiment Settings for " + name + " - " + e.ToString());
                    }
                }       
            }
            catch (Exception e)
            {
                log.Add("Error loading Method Settings - " + e.ToString());
            }
        }

        public void SaveHardwareSettings()
        {
            // Create an XmlWriter using XmlWriterSettings.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            XmlWriter writer = XmlWriter.Create(Settings.HWSettingsFilePath, settings);

            // Serialize HardwareSettings
            writer.WriteStartElement("ScanevaHardwareSettings", "");
            foreach (KeyValuePair<string, IHWManager> element in hwStore)
            {
                var compo = element.Value as ParametrizableObject;

                writer.WriteStartElement("IHWCompo");
                writer.WriteAttributeString("Class", compo.GetType().Name);
                writer.WriteAttributeString("Name", element.Key);
                writer.WriteAttributeString("IsEnabled", element.Value.IsEnabled.ToString());

                compo.SerializeParameterValues(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.Close();
        }

        public void LoadHardwareSettings()
        {
            try
            {
                XPathDocument xPathDoc = new XPathDocument(Settings.HWSettingsFilePath);
                XPathNavigator navigator = xPathDoc.CreateNavigator();

                XPathNodeIterator xPathIterator = navigator.Select("/ScanevaHardwareSettings/IHWCompo");
                foreach (XPathNavigator compoNav in xPathIterator)
                {
                    string name = compoNav.GetAttribute("Name", "");
                    string className = compoNav.GetAttribute("Class", "");
                    string isEnabled = compoNav.GetAttribute("IsEnabled", "");

                    try
                    {
                        // Create instance if not in HW store already (first load)
                        if (!hwStore.ContainsKey(name))
                        {
                            Type t = availableHWTypes.Values.FirstOrDefault(x => x.Name == className);

                            IHWManager hwInstance = (IHWManager)Activator.CreateInstance(t, log);
                            hwStore.Add(name, hwInstance);
                        }

                        // Load Settings
                        if (hwStore.ContainsKey(name))
                        {
                            // set enabled state
                            hwStore[name].IsEnabled = (isEnabled.ToLower() == "true");

                            ParametrizableObject compo = hwStore[name] as ParametrizableObject;
                            if (compo.GetType().Name == className)
                            {
                                compo.DeserializeParameterValues(compoNav, null);
                                compo.ParameterChanged("Settings Loaded");
                            }
                            else
                            {
                                log.Add("Found settings with name '" + name + "' and class '" + className + "' but no matching component in hwStore. Settings are ignored.", "Warning");
                            }
                        }
                        else
                        {
                            log.Add("Found settings with name '" + name + "' and class '" + className + "' but no matching component in hwStore. Settings are ignored.", "Warning");
                        }
                    }
                    catch (Exception e)
                    {
                        log.Add("Error loading HW Settings for "+ name + " - " + e.ToString());
                    }
                }
                XPathNavigator objectNode = navigator.SelectSingleNode("/ScanevaHardwareSettings");
            }
            catch (Exception e)
            {
                log.Add("Error loading HW Settings - " + e.ToString());
            }
        }
    }
}
