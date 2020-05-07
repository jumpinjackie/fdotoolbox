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
    [Verb("list-class-properties", HelpText = "Lists properties for the given class")]
    public class ListClassPropertiesCommand : ProviderConnectionCommand, ISummarizableCommand
    {
        [Option("full-details", Required = false, Default = false, HelpText = "If specified, print out full details of each parameter")]
        public bool Detailed { get; set; }

        [Option("schema", HelpText = "The schema name")]
        public string Schema { get; set; }

        [Option("class", Required = true, HelpText = "The class name to list properties of")]
        public string Class { get; set; }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            var walker = new SchemaWalker(conn);
            var clsDef = string.IsNullOrEmpty(this.Schema)
                ? walker.GetClassByName(this.Class)
                : walker.GetClassByName(this.Schema, this.Class);

            PrintUtils.WriteClassProperties(this, clsDef);
            return (int)CommandStatus.E_OK;
        }
    }
}
