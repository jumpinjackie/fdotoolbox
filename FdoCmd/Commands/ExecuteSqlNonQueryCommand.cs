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
using OSGeo.FDO.Common;
using OSGeo.FDO.Connections;
using System.Globalization;
using System.IO;

namespace FdoCmd.Commands
{
    [Verb("execute-sql-command", HelpText = "Executes a non-select SQL command")]
    public class ExecuteSqlNonQueryCommand : ProviderConnectionCommand<ISQLCommand>
    {
        public ExecuteSqlNonQueryCommand()
            : base(CommandType.CommandType_SQLCommand, "Executing SQL queries")
        { }

        [Option("sql", HelpText = "The SQL query to execute. Can be inline SQL or a path to a file containing the SQL query to execute", Required = true)]
        public string Sql { get; set; }

        protected override int ExecuteCommand(IConnection conn, string provider, ISQLCommand cmd)
        {
            CommandStatus retCode = CommandStatus.E_OK;
            var sql = this.Sql;
            if (File.Exists(sql))
            {
                sql = File.ReadAllText(sql);
            }
            if (sql.Trim().ToUpper().StartsWith("SELECT"))
            {
                WriteError("SQL must be a non-SELECT command");
                retCode = CommandStatus.E_FAIL_INVALID_SQL;
            }
            else
            {
                cmd.SQLStatement = sql;
                try
                {
                    int res = cmd.ExecuteNonQuery();
                    WriteLine(res.ToString(CultureInfo.InvariantCulture));
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    retCode = CommandStatus.E_FAIL_SQL_EXECUTION_ERROR;
                }
            }
            return (int)retCode;
        }
    }
}
