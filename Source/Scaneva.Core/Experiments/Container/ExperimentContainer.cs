#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ExperimentContainer.cs" company="Scaneva">
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{
    public abstract class ExperimentContainer : ExperimentBase, IList<IExperiment>, IExperiment
    {
        protected List<IExperiment> childExperiments = new List<IExperiment>();

        public ExperimentContainer(LogHelper log)
            : base(log)
        {

        }

        public override Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                hwStore = value;

                // Set hwStore for all Child experiments
                foreach(IExperiment exp in childExperiments)
                {
                    exp.HWStore = value;
                }
            }
        }

        /// <summary>
        /// Default implementation returns Postion as Indexer
        /// override if necessary
        /// </summary>
        /// <returns></returns>
        public virtual string ChildIndexer()
        {
            string cords = Position().ToString();
            cords = cords.Substring(1, cords.Length - 2).Replace(";", "_").Replace(",", ".");
            return cords;
        }

        protected void AbortChildExperiments()
        {
            abortExperiment = true;
            if (currentChildExperiment != null)
            {
                currentChildExperiment.Abort();
            }
        }

        //public abstract enExperimentStatus Configure(string resultsFilePath);
        //public abstract enExperimentStatus Run();
        protected bool abortExperiment;

        protected Task RunChildExperiments()
        {
            return Task.Factory.StartNew(new Action(ChildExperimentRunner), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private TaskCompletionSource<bool> childExperimentCompleted = null;
        private IExperiment currentChildExperiment = null;

        private void ChildExperimentRunner()
        {
            foreach (IExperiment exp in this)
            {
                childExperimentCompleted = new TaskCompletionSource<bool>();
                currentChildExperiment = exp;

                log.Add("Configuring " + ((ParametrizableObject)exp).Name + "...");
                enExperimentStatus expStatus = exp.Configure(this, Path.Combine(ResultsFilePath, (exp as ParametrizableObject).Name));
                if ((expStatus != enExperimentStatus.OK) && (expStatus != enExperimentStatus.Idle))
                {
                    log.Warning("Experiment Sequence Aborted due to exp.Configure() returning: " + expStatus);
                    status = enExperimentStatus.Error;
                    return;
                }

                exp.NotifyExperimentDataUpdated -= Child_ExperimentDataUpdated;
                exp.NotifyExperimentDataUpdated += Child_ExperimentDataUpdated;
                exp.NotifyExperimentEnded -= Child_ExperimentEnded;
                exp.NotifyExperimentEnded += Child_ExperimentEnded;
                expStatus = exp.Run();
                if ((expStatus != enExperimentStatus.OK) && (expStatus != enExperimentStatus.Running))
                {
                    exp.NotifyExperimentDataUpdated -= Child_ExperimentDataUpdated;
                    exp.NotifyExperimentEnded -= Child_ExperimentEnded;
                    try
                    {
                        // just in case Experiment was started
                        exp.Abort();
                    }
                    catch (Exception e)
                    {

                    }
                    log.Warning("Experiment Sequence Aborted due to exp.Run() returning: " + expStatus);
                    status = enExperimentStatus.Error;
                    return;
                }

                childExperimentCompleted.Task.Wait();

                // Was there an error?
                if (childExperimentCompleted.Task.Result == false)
                {
                    log.Warning("Child Experiment ended with error status");
                    status = enExperimentStatus.Error;
                    return;
                }

                currentChildExperiment = null;
                exp.NotifyExperimentEnded -= Child_ExperimentEnded;
                exp.NotifyExperimentDataUpdated -= Child_ExperimentDataUpdated;

                // is abort Flag set?
                if (abortExperiment)
                {
                    status = enExperimentStatus.Aborted;
                    return;
                }
            }

            status = enExperimentStatus.Completed;
        }

        // Override to get acess to experiment data
        protected virtual void Child_ExperimentEndedHook(object sender, ExperimentEndedEventArgs e)
        {

        }

        private void Child_ExperimentEnded(object sender, ExperimentEndedEventArgs e)
        {
            // Was there an error?
            if (e.Status == enExperimentStatus.Error)
            {
                childExperimentCompleted.SetResult(false);
            }
            else
            {
                Child_ExperimentEndedHook(sender, e);
                childExperimentCompleted.SetResult(true);
            }
        }

        private void Child_ExperimentDataUpdated(object sender, ExperimentDataEventArgs e)
        {
            // forward Child Experiment Data
            NotifyExperimentDataUpdatedNow(e);
        }

        public override void SerializeParameterValues(XmlWriter writer)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            // Serialize ContainerSettings
            if (settings != null)
            {
                XmlSerializer ser = new XmlSerializer(settings.GetType(), "");
                ser.Serialize(writer, settings, ns);
            }

            // Serialize Children
            writer.WriteStartElement("ChildExperiments", "");
            foreach (IExperiment child in childExperiments)
            {
                var compo = child as ParametrizableObject;

                writer.WriteStartElement("IExperiment");
                writer.WriteAttributeString("Class", compo.GetType().Name);
                writer.WriteAttributeString("Name", compo.Name);

                compo.SerializeParameterValues(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();           
        }

        public override void DeserializeParameterValues(IXPathNavigable node, Dictionary<string, Type> availableExperiments)
        {
            try
            {
                XPathNavigator navigator = node.CreateNavigator();

                if (settings != null)
                {
                    string settingsClassName = settings.GetType().Name;

                    XPathNavigator settingsNode = navigator.SelectSingleNode(settingsClassName);

                    XmlSerializer ser = new XmlSerializer(settings.GetType(), "");
                    settings = (ISettings)ser.Deserialize(settingsNode.ReadSubtree());
                }

                XPathNodeIterator xPathIterator = navigator.Select("ChildExperiments/IExperiment");

                // clear scanMethod
                childExperiments.Clear();

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
                        expInstance.PositionStore = PositionStore;
                        compo.Name = name;
                        childExperiments.Add(expInstance);

                        if (compo.GetType().Name == className)
                        {
                            compo.DeserializeParameterValues(compoNav, availableExperiments);
                            expInstance.HWStore = hwStore;
                            expInstance.PositionStore = PositionStore;
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
                log.Add("Deserializing Parameter Values Failed: " + e.ToString(), "Error");
            }
        }

        #region IList implementation

        public int Count => childExperiments.Count;

        public bool IsReadOnly => false;

        public IExperiment this[int index]
        {
            get => childExperiments[index];
            set
            {
                value.HWStore = HWStore;
                childExperiments[index] = value;
            }
        }

        public int IndexOf(IExperiment item)
        {
            return childExperiments.IndexOf(item);
        }

        public void Insert(int index, IExperiment item)
        {
            item.HWStore = HWStore;
            childExperiments.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            childExperiments.RemoveAt(index);
        }

        public void Add(IExperiment item)
        {
            item.HWStore = HWStore;
            childExperiments.Add(item);
        }

        public void Clear()
        {
            childExperiments.Clear();
        }

        public bool Contains(IExperiment item)
        {
            return childExperiments.Contains(item);
        }

        public void CopyTo(IExperiment[] array, int arrayIndex)
        {
            childExperiments.CopyTo(array, arrayIndex);
        }

        public bool Remove(IExperiment item)
        {
            return childExperiments.Remove(item);
        }

        public IEnumerator<IExperiment> GetEnumerator()
        {
            return childExperiments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return childExperiments.GetEnumerator();
        }

        #endregion //IList implementation

    }
}
