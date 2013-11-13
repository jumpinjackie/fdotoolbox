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
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.SQL;

namespace FdoUtil
{
    public class ExecuteSqlCommand : ConsoleCommand
    {
        private string _prv;
        private string _connStr;
        private string _sql;

        public ExecuteSqlCommand(string provider, string connectionString, string sql)
        {
            _prv = provider;
            _connStr = connectionString;
            _sql = sql;
        }

        public override int Execute()
        {
            CommandStatus ret = CommandStatus.E_OK;
            if (_sql.Trim().ToUpper().StartsWith("SELECT"))
            {
                ret = CommandStatus.E_FAIL_INVALID_SQL;
            }
            else
            {
                IConnection conn = null;
                try
                {
                    conn = CreateConnection(_prv, _connStr);
                    conn.Open();
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    WriteException(ex);
                    ret = CommandStatus.E_FAIL_CONNECT;
                }

                if (conn.ConnectionState == ConnectionState.ConnectionState_Open)
                {
                    try
                    {
                        var caps = conn.CommandCapabilities;
                        if (Array.IndexOf<int>(caps.Commands, (int)CommandType.CommandType_SQLCommand) < 0)
                        {
                            ret = CommandStatus.E_FAIL_SQL_COMMAND_NOT_SUPPORTED;
                        }
                        else
                        {
                            using (ISQLCommand cmd = (ISQLCommand)conn.CreateCommand(CommandType.CommandType_SQLCommand))
                            {
                                cmd.SQLStatement = _sql;
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    WriteException(ex);
                                    ret = CommandStatus.E_FAIL_SQL_EXECUTION_ERROR;
                                }
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return (int)ret;
        }
    }
}
