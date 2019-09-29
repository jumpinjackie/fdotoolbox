#region LGPL Header
// Copyright (C) 2019, Jackie Ng
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
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Configuration;
using FdoToolbox.Core.ETL;
using System;
using System.IO;
using System.Xml.Serialization;

namespace FdoCmd.Commands
{
    [Verb("list-bcp-tasks", HelpText = "Lists bulk copy tasks in the givne bulk copy definition file")]
    public class ListBcpTasksCommand : BaseCommand
    {
        [Option("file", Required = true, HelpText = "The path to the bulk copy definition file")]
        public string File { get; set; }

        public override int Execute()
        {
            if (!TaskDefinitionHelper.IsBulkCopy(this.File))
            {
                WriteError("Not a bulk copy definition file");
                return (int)CommandStatus.E_FAIL_TASK_VALIDATION;
            }

            var ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
            try
            {
                using (var reader = new StreamReader(this.File))
                {
                    var def = (FdoBulkCopyTaskDefinition)ser.Deserialize(reader);

                    foreach (var task in def.CopyTasks)
                    {
                        Console.WriteLine(task.name);
                    }
                }
            }
            catch (IOException ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_IO_ERROR;
            }

            return (int)CommandStatus.E_OK;
        }
    }
}
