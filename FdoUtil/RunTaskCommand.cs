#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core;
using System.IO;
using FdoToolbox.Core.Configuration;
using System.Xml.Serialization;

namespace FdoUtil
{
    public class RunTaskCommand : ConsoleCommand
    {
        private string _file;
        private string[] _taskNames;
        private string _logFile;

        public RunTaskCommand(string file)
        {
            _file = file;
            _taskNames = new string[0];
        }

        public RunTaskCommand(string file, string[] tasks)
        {
            _file = file;
            if (tasks != null)
                _taskNames = tasks;
            else
                _taskNames = new string[0];
        }

        public string LogFile
        {
            get { return _logFile; }
            set { _logFile = value; }
        }

        private string GenerateLogFileName(string prefix)
        {
            if (!string.IsNullOrEmpty(_logFile))
                return _logFile;

            var dt = DateTime.Now;
            return prefix + string.Format("{0}y{1}m{2}d{3}h{4}m{5}s", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second) + ".log";
        }

        public override int Execute()
        {
            CommandStatus retCode;
            DefinitionLoader loader = new DefinitionLoader();
            string name = null;
            if (TaskDefinitionHelper.IsBulkCopy(_file))
            {
                FdoBulkCopyTaskDefinition def = null;
                using (var sr = new StreamReader(_file))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
                    def = (FdoBulkCopyTaskDefinition)ser.Deserialize(sr);
                }

                if (def == null)
                {
                    return (int)CommandStatus.E_FAIL_TASK_VALIDATION;
                }

                //If more than one task specified, load the default task and weed
                //out unneeded elements.
                if (_taskNames.Length > 0)
                {
                    base.WriteLine("Certain tasks have been specified, only part of the bulk copy definition will execute");
                    var keepConnections = new Dictionary<string, FdoConnectionEntryElement>();
                    var keepTasks = new List<FdoCopyTaskElement>();

                    //Store needed tasks
                    foreach (var task in def.CopyTasks)
                    {
                        if (Array.IndexOf(_taskNames, task.name) >= 0)
                        {
                            keepTasks.Add(task);
                        }
                    }

                    //Store needed connections
                    foreach (var task in keepTasks)
                    {
                        foreach (var conn in def.Connections)
                        {
                            //Is referenced as source/target connection?
                            if (task.Source.connection == conn.name || task.Target.connection == conn.name)
                            {
                                if (!keepConnections.ContainsKey(conn.name))
                                    keepConnections.Add(conn.name, conn);
                            }
                        }
                    }

                    if (keepTasks.Count != _taskNames.Length)
                    {
                        List<string> names = new List<string>();
                        foreach (var n in _taskNames)
                        {
                            bool found = false;
                            foreach (var task in keepTasks)
                            {
                                if (task.name == n)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                                names.Add(n);
                        }

                        base.WriteError("Could not find specified tasks in bulk copy definition: " + string.Join(",", names.ToArray()));

                        return (int)CommandStatus.E_FAIL_MISSING_BULK_COPY_TASKS;
                    }

                    //Now replace
                    def.Connections = new List<FdoConnectionEntryElement>(keepConnections.Values).ToArray();
                    def.CopyTasks = keepTasks.ToArray();
                }
                
                using (FdoBulkCopyOptions opts = loader.BulkCopyFromXml(def, ref name, true))
                {   
                    FdoBulkCopy copy = new FdoBulkCopy(opts);
                    copy.ProcessMessage += delegate(object sender, MessageEventArgs e)
                    {
                        base.WriteLine(e.Message);
                    };
                    copy.ProcessAborted += delegate(object sender, EventArgs e)
                    {
                        base.WriteLine("Bulk Copy Aborted");
                    };
                    copy.ProcessCompleted += delegate(object sender, EventArgs e)
                    {
                        base.WriteLine("Bulk Copy Completed");
                    };
                    copy.Execute();
                    List<Exception> errors = new List<Exception>(copy.GetAllErrors());
                    if (errors.Count > 0)
                    {
                        string file = GenerateLogFileName("bcp-error-");
                        LogErrors(errors, file);
                        base.WriteError("Errors were encountered during bulk copy.");
                        retCode = CommandStatus.E_FAIL_BULK_COPY_WITH_ERRORS;
                    }
                    else { retCode = CommandStatus.E_OK; }
                }
            }
            else if (TaskDefinitionHelper.IsJoin(_file))
            {
                if (_taskNames.Length > 0)
                {
                    base.WriteError("Parameter -bcptask is not applicable for join tasks");
                    return (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS;
                }

                using (FdoJoinOptions opts = loader.JoinFromXml(_file, ref name, true))
                {
                    opts.Left.Connection.Open();
                    opts.Right.Connection.Open();
                    opts.Target.Connection.Open();
                    FdoJoin join = new FdoJoin(opts);
                    join.ProcessMessage += delegate(object sender, MessageEventArgs e)
                    {
                        base.WriteLine(e.Message);
                    };
                    join.Execute();
                    List<Exception> errors = new List<Exception>(join.GetAllErrors());
                    if (errors.Count > 0)
                    {
                        string file = GenerateLogFileName("join-error-");
                        LogErrors(errors, file);
                        base.WriteError("Errors were encountered during join operation");
                        retCode = CommandStatus.E_FAIL_JOIN_WITH_ERRORS;
                    }
                    else { retCode = CommandStatus.E_OK; }
                }
            }
            else if (TaskDefinitionHelper.IsSequentialProcess(_file))
            {
                var def = (SequentialProcessDefinition)SequentialProcessDefinition.Serializer.Deserialize(File.OpenRead(_file));
                var proc = new FdoSequentialProcess(def);
                proc.ProcessMessage += delegate(object sender, MessageEventArgs e)
                {
                    base.WriteLine(e.Message);
                };
                proc.Execute();
                List<Exception> errors = new List<Exception>(proc.GetAllErrors());
                if (errors.Count > 0)
                {
                    string file = GenerateLogFileName("seq-process-");
                    LogErrors(errors, file);
                    base.WriteError("Errors were encountered during sequential process");
                }
                //Why E_OK? the user should check the log for the underlying return codes
                //of individual FdoUtil.exe invocations!
                retCode = CommandStatus.E_OK;
            }
            else
            {
                retCode = CommandStatus.E_FAIL_UNRECOGNISED_TASK_FORMAT;
            }

            return (int)retCode;
        }

        private void LogErrors(List<Exception> errors, string file)
        {
            string dir = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            base.WriteLine("Saving errors to: " + file);

            using (StreamWriter writer = new StreamWriter(file, false))
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    writer.WriteLine("------- EXCEPTION #" + (i + 1) + " -------");
                    writer.WriteLine(errors[i].ToString());
                    writer.WriteLine("------- EXCEPTION END -------");
                }
            }

            base.WriteError("Errors have been logged to {0}", file);
        }
    }
}
