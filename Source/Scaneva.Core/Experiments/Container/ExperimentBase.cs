
#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ExperimentBase.cs" company="Scaneva">
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{
    public abstract class ExperimentBase : ParametrizableObject, IExperiment
    {
        protected Dictionary<string, IHWManager> hwStore;
        protected enExperimentStatus status = enExperimentStatus.Uninitialized;
        protected IExperiment parent = null;

        public ExperimentBase(LogHelper log)
            : base(log)
        {
        }

        public abstract Dictionary<string, IHWManager> HWStore { get; set; }
        public PositionStore PositionStore { get; set; }

        public enExperimentStatus Status { get { return status; } }

        public abstract enExperimentStatus Abort();
        public abstract enExperimentStatus Configure(IExperiment parent, string resultsFilePath);
        public abstract enExperimentStatus Run();

        /// <summary>
        /// Default implementation returns no offset for Experiment
        /// override if necessary
        /// </summary>
        /// <returns></returns>
        public virtual Position Position()
        {
            if (parent != null)
            {
                return parent.Position();
            }
            return new Position(0, 0, 0);
        }

        private string resultsFilePath = null;

        public string ResultsFilePath { get { return resultsFilePath; }
            set
            {
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
                resultsFilePath = value;
            }
        }
        public string ResultsFileName { get; set; }

        private void appendResultsText(string text)
        {
            string fullFile = Path.Combine(ResultsFilePath, ResultsFileName);
            using (StreamWriter writer = File.AppendText(fullFile))
            {
                writer.Write(text);
            }
        }

        protected void writeHeader(string header, string[] valueColumnNames, object settingsObj = null, bool positionColumns = true)
        {
            // write header only once
            if (!File.Exists(Path.Combine(ResultsFilePath, ResultsFileName)))
            {
                string[] lines = header.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string formattedHeader = "";
                foreach (string line in lines)
                {
                    if (line[0] != '#')
                    {
                        formattedHeader += "# " + line + "\r\n";
                    }
                    else
                    {
                        formattedHeader += line + "\r\n";
                    }
                }

                if (settingsObj != null)
                {
                    formattedHeader += "# === Settings ===\r\n";
                    formattedHeader += formatSettings(settingsObj);
                    formattedHeader += "# ================\r\n";
                }

                formattedHeader += "# Start Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";

                // Current Absolute Position
                Position currPos = PositionStore.CurrentAbsolutePosition();

                formattedHeader += "# Start Abs. X [µm]: " + currPos.X.ToString("F6", CultureInfo.InvariantCulture) + "\r\n";
                formattedHeader += "# Start Abs. Y [µm]: " + currPos.Y.ToString("F6", CultureInfo.InvariantCulture) + "\r\n";
                formattedHeader += "# Start Abs. Z [µm]: " + currPos.Z.ToString("F6", CultureInfo.InvariantCulture) + "\r\n";

                appendResultsText(formattedHeader);
                writeColumnHeaders(valueColumnNames, positionColumns);
            }
        }

        private string formatSettings(object settingsObject, string prefix = "")
        {
            string retStr = "";

            Type type = settingsObject.GetType();

            // Iterate all Properties
            List<PropertyInfo> pis = type.GetProperties().Where(pi => !pi.GetCustomAttributes<BrowsableAttribute>().Contains(BrowsableAttribute.No)).ToList();
            foreach (PropertyInfo fi in pis)
            {
                object value = fi.GetValue(settingsObject);

                if (value != null)
                {
                    if ((fi.PropertyType.IsArray) && (fi.PropertyType.GetElementType().Equals(typeof(byte))))
                    {
                        retStr += "# " + prefix + fi.Name + " : " + ((byte[])fi.GetValue(settingsObject)).toHexString() + "\r\n";
                    }
                    else if ((fi.PropertyType.IsClass) && (!fi.PropertyType.IsPrimitive) && (!fi.PropertyType.IsEnum) && (!fi.PropertyType.Equals(typeof(string))) && ((value.GetType().GetFields().Count() > 0) || (value.GetType().GetProperties().Count() > 0)))
                    {
                        retStr += formatSettings(fi.GetValue(settingsObject), prefix + fi.Name + ".");
                    }
                    else
                    {
                        retStr += "# " + prefix + fi.Name + " : " + fi.GetValue(settingsObject).ToString() + "\r\n";
                    }
                }
                else
                {
                    retStr += "# " + prefix + fi.Name + " : " + "null\r\n";
                }
            }

            return retStr;
        }
    

        private void writeColumnHeaders(string[] valueColumnNames, bool positionColumns)
        {
            string header = "# ";
            int i = 0;
            if(positionColumns)
            {
                header += "X [µm], Y [µm], Z [µm]";
                i = 3;
            }
            foreach (string val in valueColumnNames)
            {
                if (i > 0)
                {
                    header += ", ";
                }
                header += val;
                i++;
            }
            appendResultsText(header + "\r\n");
        }

        protected void appendCommentLines(string comment)
        {
            string[] lines = comment.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string formattedComment = "";
            foreach (string line in lines)
            {
                if (line[0] != '#')
                {
                    formattedComment += "# " + line + "\r\n";
                }
                else
                {
                    formattedComment += line + "\r\n";
                }
            }

            appendResultsText(formattedComment);
        }

        protected void appendResultsValues(double[] values, bool positionColumns = true)
        {
            string valueString = "";
            int i = 0;
            if (positionColumns)
            {
                // Current Position
                Position currPos = PositionStore.CurrentRelativePosition();

                valueString += currPos.X.ToString("F6", CultureInfo.InvariantCulture);
                valueString += ", " + currPos.Y.ToString("F6", CultureInfo.InvariantCulture);
                valueString += ", " + currPos.Z.ToString("F6", CultureInfo.InvariantCulture);
                i = 3;
            }

            foreach (double val in values)
            {
                if (i > 0)
                {
                    valueString += ", ";
                }
                valueString += val.ToString("F6", CultureInfo.InvariantCulture);
                i++;
            }
            appendResultsText(valueString + "\r\n");
        }

        public event EventHandler<ExperimentEndedEventArgs> NotifyExperimentEnded;

        public void NotifyExperimentEndedNow()
        {
            OnNotifyExperimentEnded(new ExperimentEndedEventArgs(enExperimentStatus.OK, null));
        }

        public void NotifyExperimentEndedNow(ExperimentEndedEventArgs eventArgs)
        {
            OnNotifyExperimentEnded(eventArgs);
        }

        private void OnNotifyExperimentEnded(ExperimentEndedEventArgs e)
        {
            if (NotifyExperimentEnded != null) NotifyExperimentEnded(this, e);
        }

        public event EventHandler<ExperimentDataEventArgs> NotifyExperimentDataUpdated;

        public void NotifyExperimentDataUpdatedNow(ExperimentDataEventArgs eventArgs)
        {
            OnNotifyExperimentDataUpdated(eventArgs);
        }

        private void OnNotifyExperimentDataUpdated(ExperimentDataEventArgs e)
        {
            if (NotifyExperimentDataUpdated != null) NotifyExperimentDataUpdated(this, e);
        }


    }
}
