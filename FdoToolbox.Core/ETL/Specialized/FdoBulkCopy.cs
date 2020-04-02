#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// https://github.com/jumpinjackie/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.ETL.Operations;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// A specialized ETL process that consists of one or more <see cref="FdoClassToClassCopyProcess"/> instances
    /// </summary>
    public class FdoBulkCopy : FdoSpecializedEtlProcess
    {
        private int _ReportFrequency = 50;

        /// <summary>
        /// Occurs before execution takes place. Subscribers have an opportunity to abort the execution.
        /// </summary>
        public event System.ComponentModel.CancelEventHandler BeforeExecute = delegate { };

        /// <summary>
        /// Gets or sets the frequency at which progress feedback is made
        /// </summary>
        /// <value>The report frequency.</value>
        public int ReportFrequency
        {
            get { return _ReportFrequency; }
            set { _ReportFrequency = value; }
        }

        private FdoBulkCopyOptions _options;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public FdoBulkCopyOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkCopy"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public FdoBulkCopy(FdoBulkCopyOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkCopy"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="reportFrequency">The report frequency.</param>
        public FdoBulkCopy(FdoBulkCopyOptions options, int reportFrequency)
            : this(options)
        {
            _ReportFrequency = reportFrequency;
        }

        /// <summary>
        /// Registers the specified operation.
        /// </summary>
        /// <param name="op">The operation.</param>
        public new void Register(IFdoOperation op)
        {
            throw new NotSupportedException("Bulk Copy does not support direct registration of operations");
        }

        private List<EtlProcess> subProcesses = new List<EtlProcess>();
        private Dictionary<string, List<Exception>> subProcessErrors = new Dictionary<string, List<Exception>>();

        private bool execute = true;

        /// <summary>
        /// Initializes the process
        /// </summary>
        protected override void Initialize()
        {
            Reset();
            
            System.ComponentModel.CancelEventArgs ce = new System.ComponentModel.CancelEventArgs(false);
            this.BeforeExecute(this, ce);
            if (ce.Cancel)
            {
                SendMessage("Bulk Copy Cancelled");
                execute = false;
                return;
            }

            foreach (FdoClassCopyOptions copt in Options.ClassCopyOptions)
            {
                FdoClassToClassCopyProcess proc = new FdoClassToClassCopyProcess(copt);
                proc.ReportFrequency = this.ReportFrequency;
                subProcesses.Add(proc);
            }
        }

        private void Reset()
        {
            ClearErrors();
            subProcesses.Clear();
            subProcessErrors.Clear();
        }

        /// <summary>
        /// Executes this process
        /// </summary>
        public override void Execute()
        {
            Initialize();
            if (execute)
            {
                foreach (EtlProcess proc in subProcesses)
                {
                    SendMessage("[Bulk Copy] Running sub-process [" + proc.Name + "]:");
                    proc.ProcessMessage += new MessageEventHandler(OnSubProcessMessage);
                    proc.Execute();
                    List<Exception> errors = new List<Exception>(proc.GetAllErrors());
                    SendMessageFormatted("[Bulk Copy] sub-process completed with {0} errors", errors.Count);
                    LogSubProcessErrors(proc.Name, errors);
                }
            }
        }

        /// <summary>
        /// Logs all the sub-process errors.
        /// </summary>
        /// <param name="procName">Name of the process.</param>
        /// <param name="errors">The errors.</param>
        private void LogSubProcessErrors(string procName, IEnumerable<Exception> errors)
        {
            if (!subProcessErrors.ContainsKey(procName))
                subProcessErrors[procName] = new List<Exception>();

            subProcessErrors[procName].AddRange(errors);
        }

        void OnSubProcessMessage(object sender, MessageEventArgs e)
        {
            SendMessage(e.Message);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _options.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Saves this process to a file
        /// </summary>
        /// <param name="file">The file to save this process to</param>
        /// <param name="name">The name of the process</param>
        public override void Save(string file, string name)
        {
            FdoBulkCopyTaskDefinition def = new FdoBulkCopyTaskDefinition();
            def.name = name;

            List<FdoConnectionEntryElement> connList = new List<FdoConnectionEntryElement>();
            List<FdoCopyTaskElement> copyTasks = new List<FdoCopyTaskElement>();

            foreach (string connName in _options.ConnectionNames)
            {
                FdoConnection conn = _options.GetConnection(connName);
                FdoConnectionEntryElement entry = new FdoConnectionEntryElement();
                entry.name = connName;
                entry.provider = conn.Provider;
                entry.ConnectionString = conn.ConnectionString;

                if (conn.HasConfiguration)
                {
                    string path = Path.GetDirectoryName(file);
                    path = Path.Combine(path, entry.name + "_configuration.xml");

                    conn.SaveConfiguration(path);
                    entry.configPath = path;
                }

                connList.Add(entry);
            }

            foreach (FdoClassCopyOptions copt in _options.ClassCopyOptions)
            {
                copyTasks.Add(copt.ToElement());
            }

            def.Connections = connList.ToArray();
            def.CopyTasks = copyTasks.ToArray();

            using (StreamWriter writer = new StreamWriter(file, false))
            {
                XmlSerializer ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
                ser.Serialize(writer, def);
            }
        }

        /// <summary>
        /// Determines if this process is capable of persistence
        /// </summary>
        /// <value></value>
        public override bool CanSave
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the file extension associated with this process. For tasks where <see cref="CanSave"/> is
        /// false, an empty string is returned
        /// </summary>
        /// <returns></returns>
        public override string GetFileExtension()
        {
            return TaskDefinitionHelper.BULKCOPYDEFINITION;
        }

        /// <summary>
        /// Gets a description of this process
        /// </summary>
        /// <returns></returns>
        public override string GetDescription()
        {
            return ResourceUtil.GetString("DESC_BULK_COPY_DEFINITION");
        }


        /// <summary>
        /// Gets all errors that occured during the execution of this process
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors()
        {
            foreach (string key in subProcessErrors.Keys)
            {
                foreach (Exception ex in subProcessErrors[key])
                {
                    yield return ex;
                }
            }
        }

        public override void UpdateConnectionReferences(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(oldName))
                throw new ArgumentNullException("oldName");

            if (string.IsNullOrEmpty(newName))
                throw new ArgumentNullException("newName");

            if (this.Options != null)
                this.Options.UpdateConnectionReferences(oldName, newName);
        }
    }
}
