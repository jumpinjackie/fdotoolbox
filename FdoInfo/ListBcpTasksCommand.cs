#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.Xml.Serialization;
using FdoToolbox.Core.Configuration;
using System.IO;

namespace FdoInfo
{
    public class ListBcpTasksCommand : ConsoleCommand
    {
        private string _file;

        public ListBcpTasksCommand(string file)
        {
            _file = file;
        }

        public override int Execute()
        {
            if (!TaskDefinitionHelper.IsBulkCopy(_file))
            {
                return (int)CommandStatus.E_FAIL_TASK_VALIDATION;
            }

            var ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
            try
            {
                using (var reader = new StreamReader(_file))
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
