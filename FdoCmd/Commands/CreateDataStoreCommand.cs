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
using CommandLine.Text;
using FdoToolbox.Core.AppFramework;
using OSGeo.FDO.Commands.DataStore;
using OSGeo.FDO.Connections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("create-datastore", HelpText = "Creates a data store against the given FDO connection")]
    public class CreateDataStoreCommand : ProviderConnectionCommand<ICreateDataStore>
    {
        public CreateDataStoreCommand()
            : base (OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore, CommandCapabilityDescriptions.CreateDataStore)
        { }

        [Option("create-params", Required = true, HelpText = "Data store creation parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> DataStoreParameters { get; set; }

        //Shadow properties so we can apply new attributes on top

        [Option("provider", SetName = "space-delimited", HelpText = "The FDO provider name")]
        public new string Provider { get; set; }

        [Option("connect-params", SetName = "space-delimited", HelpText = "Connection Parameters. Generally required for RDBMS providers so it knows what server to create the data store in. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public new IEnumerable<string> ConnectParameters { get; set; }

        [Option("from-file", SetName = "file-based", HelpText = "The path to the data file to create a FDO connection from")]
        public new string FilePath { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Create SDF file", new CreateDataStoreCommand { Provider = "OSGeo.SDF", DataStoreParameters = new[] { "File", "C:\\path\\to\\MyFile.sdf" } });
                yield return new Example("Create SQL Server Data Store", new CreateDataStoreCommand { Provider = "OSGeo.SQLServerSpatial", ConnectParameters = new[] { "Service", "mysqlserverhostnameorip", "Username", "myusername", "Password", "mypassword" }, DataStoreParameters = new[] { "DataStore", "MyDatabase" } });
                yield return new Example("Create PostGIS Data Store", new CreateDataStoreCommand { Provider = "OSGeo.PostgreSQL", ConnectParameters = new[] { "Service", "mypghostorip", "Username", "myusername", "Password", "mypassword" }, DataStoreParameters = new[] { "DataStore", "MyDatabase" } });
            }
        }

        // Override so it properly evaluates against the shadowing property
        protected override string GetActualProvider() => _inferredFileProvider ?? Provider;

        protected override List<string> GetConnectParamTokens() => (this.ConnectParameters ?? Enumerable.Empty<string>()).ToList();

        protected override string GetFilePath() => FilePath;

        protected override bool RequireConnect => false;

        protected override bool IsValidConnectionStateForCommand(ConnectionState state)
            => state == ConnectionState.ConnectionState_Open || state == ConnectionState.ConnectionState_Pending;

        protected override int ExecuteCommand(IConnection conn, string provider, ICreateDataStore cmd)
        {
            CommandStatus retCode = CommandStatus.E_OK;
            var (dpp, rc) = ValidateTokenPairSet("--create-params", this.DataStoreParameters);
            if (rc.HasValue)
            {
                return rc.Value;
            }
            else
            {
                var dsp = cmd.DataStoreProperties;
                foreach (var kvp in dpp)
                {
                    dsp.SetProperty(kvp.Key, kvp.Value);
                }
                cmd.Execute();
                Console.WriteLine("Created data store using provider: " + provider);
                return (int)retCode;
            }
        }
    }
}