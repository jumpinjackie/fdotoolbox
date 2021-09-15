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
using CommandLine.Text;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Connections;
using System.Collections.Generic;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("list-connection-params", HelpText = "Lists connection parameters for the given FDO provider")]
    public class ListConnectionParametersCommand : BaseCommand, ISummarizableCommand
    {
        [Option("full-details", Required = false, Default = false, HelpText = "If specified, print out full details of each parameter")]
        public bool Detailed { get; set; }

        [Option("connect-params", SetName = "space-delimited", Required = false, HelpText = "Connection Parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> ConnectParameters { get; set; }

        [Option("from-file", SetName = "file-based", Required = true, HelpText = "The path to the data file to create a FDO connection from")]
        public string FilePath { get; set; }

        [Option("provider", Required = true, SetName = "space-delimited", HelpText = "The FDO provider name")]
        public string Provider { get; set; }

        protected virtual string GetActualProvider() => _inferredFileProvider ?? Provider;

        protected string _inferredFileProvider;

        protected virtual bool RequireConnect => false;

        protected virtual bool IsValidConnectionStateForCommand(ConnectionState state) => true;

        protected virtual List<string> GetConnectParamTokens() => (this.ConnectParameters ?? Enumerable.Empty<string>()).ToList();

        protected virtual string GetFilePath() => FilePath;

        private (IConnection conn, bool bConnect, int? retCode) TryCreateConnection()
        {
            IConnection conn = null;
            int? retCode = null;
            bool bConnect = RequireConnect;
            try
            {
                var fp = this.GetFilePath();
                if (!string.IsNullOrWhiteSpace(fp))
                {
                    (conn, _inferredFileProvider) = FileExtensionMapper.TryCreateConnection(fp);
                    bConnect = (conn != null);
                }
                else
                {
                    var prv = GetActualProvider();
                    //Derived classes may override and make this optional despite being required at this level
                    if (!string.IsNullOrWhiteSpace(prv))
                    {
                        var connMgr = FeatureAccessManager.GetConnectionManager();
                        conn = connMgr.CreateConnection(prv);
                        var (connp, rc) = ValidateTokenPairSet("--connect-params", this.GetConnectParamTokens());
                        if (rc.HasValue)
                        {
                            return (null, false, rc.Value);
                        }
                        else
                        {
                            var ci = conn.ConnectionInfo;
                            var cnp = ci.ConnectionProperties;
                            foreach (var kvp in connp)
                            {
                                cnp.SetProperty(kvp.Key, kvp.Value);
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
                        var ci = conn.ConnectionInfo;
                        var dict = ci.ConnectionProperties;
                        PrintUtils.WritePropertyDict(this, dict);
                        return (int)CommandStatus.E_OK;
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

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("List SQL Server Connection Parameters", new ListClassPropertiesCommand
                {
                    Provider = "OSGeo.SQLServerSpatial",
                    ConnectParameters = new[] { "Service", "mysqlserverhostnameorip", "Username", "myusername", "Password", "mypassword", "DataStore", "MyDatabase" },
                    Schema = "Default",
                    Class = "MyFeatureClass"
                });
            }
        }
    }
}
