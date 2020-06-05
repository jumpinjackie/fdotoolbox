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
using System.Collections.Generic;

namespace FdoCmd.Commands
{
    [Verb("list-destroy-datastore-params", HelpText = "List all available parameters for data store destruction")]
    public class ListDestroyDataStoreParametersCommand : ProviderCommand<IDestroyDataStore>, ISummarizableCommand
    {
        [Option("full-details", Required = false, Default = false, HelpText = "If specified, print out full details of each parameter")]
        public bool Detailed { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("List SQL Server Destroy Data Store Parameters", new ListDestroyDataStoreParametersCommand
                {
                    Provider = "OSGeo.SQLServerSpatial"
                });
            }
        }

        public ListDestroyDataStoreParametersCommand()
            : base(OSGeo.FDO.Commands.CommandType.CommandType_DestroyDataStore, "destroying data stores")
        { }

        protected override int ExecuteCommand(IDestroyDataStore cmd)
        {
            var dsp = cmd.DataStoreProperties;
            PrintUtils.WritePropertyDict(this, dsp);
            return (int)CommandStatus.E_OK;
        }
    }
}
