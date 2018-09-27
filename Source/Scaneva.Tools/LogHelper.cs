#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="LogHelper.cs" company="Scaneva">
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

using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Scaneva.Tools
{
    public class LogEntryAddedEventArgs : EventArgs
    {
        private readonly string logEntry;

        public LogEntryAddedEventArgs(string logEntry)
        {
            this.logEntry = logEntry;
        }

        public string LogEntry
        {
            get { return this.logEntry; }
        }
    }

    public class StatusUpdatedEventArgs : EventArgs
    {
        private readonly int statusId;
        private readonly object statusMessage;

        public StatusUpdatedEventArgs(int statusId, object statusMessage)
        {
            this.statusId = statusId;
            this.statusMessage = statusMessage;
        }

        public int StatusId
        {
            get { return this.statusId; }
        }

        public object StatusMessage
        {
            get { return this.statusMessage; }
        }
    }

    public class LogHelper : IDisposable
    {
        private StreamWriter logStreamWriter = null;

        /// <summary>
        /// An event that clients can use to be notified whenever a LogEntry was added
        /// </summary>
        public event EventHandler<LogEntryAddedEventArgs> LogEntryAdded;

        /// <summary>
        /// An event that clients can use to be notified whenever a StatusUpdate was added
        /// </summary>        
        public event EventHandler<StatusUpdatedEventArgs> StatusUpdated;

        /// <summary>
        /// Create a new LogHelper with default path and name
        /// </summary>
        public LogHelper()
            : this("", "")
        {
        }

        /// <summary>
        /// Create a new LogHelper
        /// </summary>
        /// <param name="logFileName">File Name of Logfile</param>
        /// <param name="logFilePath">Path to Logfile</param>
        public LogHelper(string logFileName, string logFilePath)
        {
            if (logFileName == "")
            {
                logFileName = "Log" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
            }
            if (logFilePath == "")
            {
                logFilePath = getMainDirectory();
            }

            // Check if Path exist
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

            // Create Log File writer (append)
            string logFile = Path.Combine(logFilePath, logFileName);
            logStreamWriter = new StreamWriter(logFile, true);
            string timeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            logStreamWriter.Write("\r\n ===== Logging started: " + timeStr + " ===== \r\n\r\n");
            logStreamWriter.Flush();
        }

        /// <summary>
        /// Invoke the LogEntryAdded event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLogEntryAdded(LogEntryAddedEventArgs e)
        {
            LogEntryAdded?.Invoke(null, e);
        }

        /// <summary>
        /// Invoke the StatusWasUpdate event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStatusUpdate(StatusUpdatedEventArgs e)
        {
            StatusUpdated?.Invoke(null, e);
        }

        /// <summary>
        /// Log an Error
        /// </summary>
        /// <param name="logMessage">Error message Text</param>
        public void Error(string logMessage)
        {
            Add(logMessage, "Error");
        }

        /// <summary>
        /// Log a Warning
        /// </summary>
        /// <param name="logMessage">Earning message text</param>
        public void Warning(string logMessage)
        {
            Add(logMessage, "Warning");
        }

        /// <summary>
        /// Add a Message to the Log
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="prefix"></param>
        public void Add(string logMessage, string prefix = "", bool logOnly = false)
        {
            string timeStr = DateTime.Now.ToString("HH:mm:ss.fff");

            string logStr = "[" + timeStr;
            if (prefix != "")
            {
                logStr += " " + prefix;
            }
            logStr += "] : ";

            if (prefix.ToUpper() == "ERROR")
            {
                string callingMethod = "Add";
                string callingClass = "LogHelper";
                int i = 1;

                while (callingClass == "LogHelper")
                {
                    StackFrame caller = new StackFrame(i++);
                    callingMethod = caller.GetMethod().Name;
                    callingClass = caller.GetMethod().ReflectedType.Name;
                }                

                logStr += callingClass + "." + callingMethod + " - ";
            }
            logStr += logMessage;

            // Saving to Log-File here
            logStreamWriter.Write(logStr + "\r\n");
            logStreamWriter.Flush();

            // Notify Listeners
            if (!logOnly)
            {
                OnLogEntryAdded(new LogEntryAddedEventArgs(logStr));
            }
        }

        /// <summary>
        /// Notify GUI about a status change
        /// </summary>
        /// <param name="statusChannel"></param>
        /// <param name="message"></param>
        public void AddStatusUpdate(int statusChannel, object message)
        {
            OnStatusUpdate(new StatusUpdatedEventArgs(statusChannel, message));
        }

        /// <summary>
        /// Returns current path of the executing assembly
        /// </summary>
        public static string getMainDirectory()
        {
            //string dir = AppDomain.CurrentDomain.BaseDirectory;
            string dir = Assembly.GetExecutingAssembly().CodeBase;
            dir = dir.Substring(0, dir.LastIndexOf('/') + 1);
            Uri uri = new Uri(dir);
            dir = uri.LocalPath;

            return dir;
        }

        public void Dispose()
        {
            if (logStreamWriter != null)
            {
                logStreamWriter.Flush();
                logStreamWriter.Close();
                logStreamWriter.Dispose();
            }
        }

    }
}
