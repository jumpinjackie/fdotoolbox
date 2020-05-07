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
using FdoToolbox.Core.Connections;
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

    public abstract class ProviderConnectionCommand : BaseCommand
    {
        [Option("provider", Required = true, SetName = "space-delimited", HelpText = "The FDO provider name")]
        public string Provider { get; set; }

        [Option("connect-params", SetName = "space-delimited", Required = true, HelpText = "Connection Parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> ConnectParameters { get; set; }

        [Option("from-file", SetName = "file-based", Required = true, HelpText = "The path to the data file to create a FDO connection from")]
        public string FilePath { get; set; }

        protected virtual string GetActualProvider() => _inferredFileProvider ?? Provider;

        protected string _inferredFileProvider;

        protected virtual bool RequireConnect => true;

        protected virtual bool IsValidConnectionStateForCommand(ConnectionState state) => state == ConnectionState.ConnectionState_Open;

        private (IConnection conn, bool bConnect, int? retCode) TryCreateConnection()
        {
            IConnection conn = null;
            int? retCode = null;
            bool bConnect = RequireConnect;
            try
            {
                if (!string.IsNullOrWhiteSpace(this.FilePath))
                {
                    (conn, _inferredFileProvider) = FileExtensionMapper.TryCreateConnection(this.FilePath);
                    bConnect = (conn != null);
                }
                else
                {
                    var prv = GetActualProvider();
                    //Derived classes may override and make this optional despite being required at this level
                    if (!string.IsNullOrWhiteSpace(prv))
                    {
                        conn = FeatureAccessManager.GetConnectionManager().CreateConnection(prv);
                        var connp = (this.ConnectParameters ?? Enumerable.Empty<string>()).ToList();
                        if (connp.Count > 0)
                        {
                            if ((connp.Count % 2) != 0)
                            {
                                Console.Error.WriteLine("Incorrect parameters format. Expected: <name1> <value1> ... <nameN> <valueN>");
                                return (null, false, (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS);
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
                    }
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                retCode = (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
            }

            return (conn, bConnect, retCode);
        }

        public override int Execute()
        {
            try
            {
                var (conn, bConnect, rc) = TryCreateConnection();
                if (rc.HasValue)
                    return rc.Value;

                if (bConnect)
                {
                    var stat = conn.Open();
                    if (!IsValidConnectionStateForCommand(stat))
                    {
                        WriteError("Failed to connect");
                        return (int)CommandStatus.E_FAIL_CONNECT;
                    }
                }

                if (conn == null)
                {
                    WriteError("Could not create a FDO connection");
                    return (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
                }

                using (conn)
                {
                    try
                    {
                        return ExecuteConnection(conn, GetActualProvider());
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
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }
        }

        protected abstract int ExecuteConnection(IConnection conn, string provider);
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

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            if (!HasCommand(conn, _cmdType, _capDesc, out var ret) && ret.HasValue)
                return ret.Value;

            using (var cmd = (TFdoCommand)conn.CreateCommand(_cmdType))
            {
                return ExecuteCommand(conn, provider, cmd);
            }
        }

        protected abstract int ExecuteCommand(IConnection conn, string provider, TFdoCommand cmd);
    }
}
