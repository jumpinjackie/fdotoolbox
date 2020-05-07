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
using FdoToolbox.Core.AppFramework;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Connections;
using System;

namespace FdoCmd.Commands
{
    [Verb("execute-sql-query", HelpText = "")]
    public class ExecuteSqlCommand : ProviderConnectionCommand
    {
        [Option("sql", HelpText = "The SQL SELECT query to execute", Required = true)]
        public string Sql { get; set; }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            CommandStatus retCode = CommandStatus.E_OK;
            if (this.Sql.Trim().ToUpper().StartsWith("SELECT"))
            {
                retCode = CommandStatus.E_FAIL_INVALID_SQL;
            }
            else
            {
                var caps = conn.CommandCapabilities;
                if (Array.IndexOf<int>(caps.Commands, (int)CommandType.CommandType_SQLCommand) < 0)
                {
                    retCode = CommandStatus.E_FAIL_SQL_COMMAND_NOT_SUPPORTED;
                }
                else
                {
                    using (ISQLCommand cmd = (ISQLCommand)conn.CreateCommand(CommandType.CommandType_SQLCommand))
                    {
                        cmd.SQLStatement = this.Sql;
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            WriteException(ex);
                            retCode = CommandStatus.E_FAIL_SQL_EXECUTION_ERROR;
                        }
                    }
                }
            }
            return (int)retCode;
        }
    }
}
