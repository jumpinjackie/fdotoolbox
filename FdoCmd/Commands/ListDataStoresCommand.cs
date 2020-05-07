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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;

namespace FdoCmd.Commands
{
    [Verb("list-datastores", HelpText = "Lists datastores for the given connection")]
    public class ListDataStoresCommand : ProviderConnectionCommand
    {
        [Option("fdo-only", Required = false, Default = false, HelpText = "Only show data stores with FDO metadata")]
        public bool FdoOnly { get; set; }

        protected override bool IsValidConnectionStateForCommand(ConnectionState state)
        {
            return state == ConnectionState.ConnectionState_Open || state == ConnectionState.ConnectionState_Pending;
        }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            if (!HasCommand(conn, OSGeo.FDO.Commands.CommandType.CommandType_ListDataStores, "listing data stores", out var ret))
                return ret.Value;

            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                var datastores = service.ListDataStores(FdoOnly);
                WriteLine("Listing datastores:\n");
                foreach (DataStoreInfo dstore in datastores)
                {
                    WriteLine("-> {0}", dstore.Name);
                    using (Indent())
                    {
                        WriteLine("Description: {0}", dstore.Description);
                        WriteLine("Has FDO Metadata: {0}", dstore.IsFdoEnabled);
                    }
                }
            }
            return (int)CommandStatus.E_OK;
        }
    }
}
