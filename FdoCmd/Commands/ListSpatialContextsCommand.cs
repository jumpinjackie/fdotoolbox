﻿#region LGPL Header
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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;
using System.Collections.Generic;

namespace FdoCmd.Commands
{
    [Verb("list-spatial-contexts", HelpText = "Gets spatial contexts for the given connection")]
    public class ListSpatialContextsCommand : ProviderConnectionCommand, ISummarizableCommand
    {
        [Option("full-details", Required = false, Default = false, HelpText = "If specified, print out full details of each spatial context")]
        public bool Detailed { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("List spatial contexts of SHP connection", new ListSpatialContextsCommand
                {
                    Provider = "OSGeo.SHP",
                    ConnectParameters = new[] { "DefaultFileLocation", "C:\\Path\\To\\MyShapefiles" },
                });
            }
        }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            var contexts = conn.GetSpatialContexts();
            PrintUtils.WriteSpatialContexts(this, contexts);
            return (int)CommandStatus.E_OK;
        }
    }
}
