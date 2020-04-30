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
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Connections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FdoCmd.Commands
{
    public abstract class ProviderCommand : BaseCommand
    {
        [Option("provider", Required = true, HelpText = "The FDO provider name")]
        public string Provider { get; set; }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = FeatureAccessManager.GetConnectionManager().CreateConnection(this.Provider);
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
            }

            using (conn)
            {
                return ExecuteConnection(conn);
            }
        }

        protected abstract int ExecuteConnection(IConnection conn);
    }

    public abstract class ProviderCommand<TFdoCommand> : ProviderCommand
        where TFdoCommand : ICommand
    {
        readonly CommandType _cmdType;
        readonly string _capDesc;

        protected ProviderCommand(CommandType cmdType, string capDesc)
        {
            _cmdType = cmdType;
            _capDesc = capDesc;
        }

        protected override int ExecuteConnection(IConnection conn)
        {
            if (!HasCommand(conn, _cmdType, _capDesc, out var ret) && ret.HasValue)
                return ret.Value;

            using (var cmd = (TFdoCommand)conn.CreateCommand(_cmdType))
            {
                return ExecuteCommand(cmd);
            }
        }

        protected abstract int ExecuteCommand(TFdoCommand cmd);
    }

    public abstract class ProviderConnectionCommand : ProviderCommand
    {
        [Option("connect-params", HelpText = "Connection Parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> ConnectParameters { get; set; }

        protected virtual bool RequireConnect => true;

        protected virtual bool IsValidConnectionStateForCommand(ConnectionState state) => state == ConnectionState.ConnectionState_Open;

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = FeatureAccessManager.GetConnectionManager().CreateConnection(this.Provider);
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
            }

            try
            {
                bool bConnect = RequireConnect;

                var connp = (this.ConnectParameters ?? Enumerable.Empty<string>()).ToList();
                if (connp.Count > 0)
                {
                    if ((connp.Count % 2) != 0)
                    {
                        Console.Error.WriteLine("Incorrect parameters format. Expected: <name1> <value1> ... <nameN> <valueN>");
                        return (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS;
                    }
                    else
                    {
                        var ci = conn.ConnectionInfo;
                        var cnp = ci.ConnectionProperties;
                        for (int i = 0; i < connp.Count; i += 2)
                        {
                            var name = connp[i];
                            var value = connp[i + 1];
                            cnp.SetProperty(name, value);
                        }
                        bConnect = true;
                    }
                }

                if (bConnect)
                {
                    var stat = conn.Open();
                    if (!IsValidConnectionStateForCommand(stat))
                    {
                        WriteError("Failed to connect");
                        return (int)CommandStatus.E_FAIL_CONNECT;
                    }
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }

            using (conn)
            {
                try
                {
                    return ExecuteConnection(conn);
                }
                finally
                {
                    try
                    {
                        if (conn.ConnectionState != ConnectionState.ConnectionState_Closed)
                            conn.Close();
                    }
                    catch { }
                }
            }
        }
    }

    public abstract class ProviderConnectionCommand<TFdoCommand> : ProviderConnectionCommand
        where TFdoCommand : ICommand
    {
        readonly CommandType _cmdType;
        readonly string _capDesc;

        protected ProviderConnectionCommand(CommandType cmdType, string capDesc)
        {
            _cmdType = cmdType;
            _capDesc = capDesc;
        }

        protected override int ExecuteConnection(IConnection conn)
        {
            if (!HasCommand(conn, _cmdType, _capDesc, out var ret) && ret.HasValue)
                return ret.Value;

            using (var cmd = (TFdoCommand)conn.CreateCommand(_cmdType))
            {
                return ExecuteCommand(conn, cmd);
            }
        }

        protected abstract int ExecuteCommand(IConnection conn, TFdoCommand cmd);
    }
}
