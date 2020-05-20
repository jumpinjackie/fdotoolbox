#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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
using CommandLine;
using FdoToolbox.Core;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Configuration;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ETL.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FdoCmd.Commands
{
    [Verb("run-task", HelpText = "Runs a task from a definition file")]
    public class RunTaskCommand : BaseCommand
    {
        [Option("copy-task-names", HelpText = "If the task is a bulk copy definition, specifies the names of tasks within the specified task definition to execute.")]
        public IEnumerable<string> CopyTaskNames { get; set; }

        [Option("file", HelpText = "The path to the task definition", Required = true)]
        public string File { get; set; }

        [Option("log-file", HelpText = "The path to the log file where errors are logged")]
        public string LogFile { get; set; }

        public override int Execute()
        {
            CommandStatus retCode;
            DefinitionLoader loader = new DefinitionLoader();
            string name = null;
            if (TaskDefinitionHelper.IsBulkCopy(this.File))
            {
                FdoBulkCopyTaskDefinition def = null;
                using (var sr = new StreamReader(this.File))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
                    def = (FdoBulkCopyTaskDefinition)ser.Deserialize(sr);
                }

                if (def == null)
                {
                    return (int)CommandStatus.E_FAIL_TASK_VALIDATION;
                }

                var bcpNames = (this.CopyTaskNames ?? Enumerable.Empty<string>()).ToArray();

                //If more than one task specified, load the default task and weed
                //out unneeded elements.
                if (bcpNames.Length > 0)
                {
                    base.WriteLine("Certain tasks have been specified, only part of the bulk copy definition will execute");
                    var keepConnections = new Dictionary<string, FdoConnectionEntryElement>();
                    var keepTasks = new List<FdoCopyTaskElement>();

                    //Store needed tasks
                    foreach (var task in def.CopyTasks)
                    {
                        if (Array.IndexOf(bcpNames, task.name) >= 0)
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

                    if (keepTasks.Count != bcpNames.Length)
                    {
                        List<string> names = new List<string>();
                        foreach (var n in bcpNames)
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
                    copy.ProcessMessage += (s, e) =>
                    {
                        base.WriteLine(e.Message);
                    };
                    copy.ProcessAborted += (s, e) =>
                    {
                        base.WriteLine("Process Aborted");
                    };
                    copy.ProcessCompleted += (s, e) =>
                    {
                        base.WriteLine("Process Completed");
                    };
                    copy.Execute();
                    var errors = copy.GetAllErrors().ToList();
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
            else if (TaskDefinitionHelper.IsJoin(this.File))
            {
                using (FdoJoinOptions opts = loader.JoinFromXml(this.File, ref name, true))
                {
                    opts.Left.Connection.Open();
                    opts.Right.Connection.Open();
                    opts.Target.Connection.Open();
                    FdoJoin join = new FdoJoin(opts);
                    join.ProcessMessage += delegate (object sender, MessageEventArgs e)
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
            else
            {
                retCode = CommandStatus.E_FAIL_UNRECOGNISED_TASK_FORMAT;
            }

            return (int)retCode;
        }

        private string GenerateLogFileName(string prefix)
        {
            if (!string.IsNullOrEmpty(this.LogFile))
                return this.LogFile;

            var dt = DateTime.Now;
            return prefix + string.Format("{0}y{1}m{2}d{3}h{4}m{5}s", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second) + ".log";
        }

        private void LogErrors(List<Exception> errors, string file)
        {
            string dir = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            base.WriteLine("Saving errors to: " + file);

            using (var writer = new StreamWriter(file, false))
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
