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
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Connections;

namespace FdoCmd.Commands
{
    [Verb("destroy-spatial-context", HelpText = "Destroys a spatial context")]
    public class DestroySpatialContextCommand : ProviderConnectionCommand<IDestroySpatialContext>
    {
        public DestroySpatialContextCommand()
            : base(OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext, "destroying spatial contexts")
        { }

        [Option("name", Required = true, HelpText = "The name of the spatial context")]
        public string Name { get; set; }

        protected override int ExecuteCommand(IConnection conn, string provider, IDestroySpatialContext cmd)
        {
            cmd.Name = this.Name;
            cmd.Execute();
            WriteLine("Destroyed spatial context: " + this.Name);
            return (int)CommandStatus.E_OK;
        }
    }
}
