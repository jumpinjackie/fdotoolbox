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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;

namespace FdoCmd.Commands
{
    [Verb("create-data-store", HelpText = "Creates a data store against the given FDO connection")]
    public class CreateDataStoreCommand : ProviderConnectionCommand
    {
        [Option("parameters", HelpText = "Data store creation parameters string")]
        public string DataStoreStr { get; set; }

        protected override int ExecuteConnection(IConnection conn)
        {
            CommandStatus retCode;
            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                try
                {
                    service.CreateDataStore(this.DataStoreStr);
                    WriteLine("Data Store Created!");
                    retCode = CommandStatus.E_OK;
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    WriteException(ex);
                    retCode = CommandStatus.E_FAIL_CREATE_DATASTORE;
                }
            }
            return (int)retCode;
        }
    }
}
