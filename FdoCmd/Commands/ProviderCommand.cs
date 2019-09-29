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
using OSGeo.FDO.Connections;

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

    public abstract class ProviderConnectionCommand : ProviderCommand
    {
        [Option("connection", Required = true, HelpText = "The FDO connection string")]
        public string ConnectionString { get; set; }

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
                if (!string.IsNullOrEmpty(this.ConnectionString))
                    conn.ConnectionString = this.ConnectionString;

                var stat = conn.Open();
                if (!IsValidConnectionStateForCommand(stat))
                {
                    WriteError("Failed to connect");
                    return (int)CommandStatus.E_FAIL_CONNECT;
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
}
