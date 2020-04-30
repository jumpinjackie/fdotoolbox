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

        [Option("connect-params", SetName = "space-delimited", HelpText = "Connection Parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>. Can be specified as an alternative to --connection-string")]
        public new IEnumerable<string> ConnectParameters { get; set; }

        [Option("connection-string", SetName = "connection-string", HelpText = "The FDO connection string. Can be specified as an alternative to --connect-params")]
        public new string ConnectionString { get; set; }

        protected override bool RequireConnect => false;

        protected override int ExecuteCommand(IConnection conn, ICreateDataStore cmd)
        {
            CommandStatus retCode = CommandStatus.E_OK;
            var dpp = this.DataStoreParameters.ToList();
            if ((dpp.Count % 2) != 0)
            {
                Console.Error.WriteLine("Incorrect parameters format. Expected: <name1> <value1> ... <nameN> <valueN>");
                retCode = CommandStatus.E_FAIL_INVALID_ARGUMENTS;
            }
            else
            {
                var dsp = cmd.DataStoreProperties;
                for (int i = 0; i < dpp.Count; i += 2)
                {
                    var name = dpp[i];
                    var value = dpp[i + 1];
                    dsp.SetProperty(name, value);
                }
                cmd.Execute();
                Console.WriteLine("Created data store using provider: " + this.Provider);
            }
            return (int)retCode;
        }
    }
}