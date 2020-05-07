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
using FdoToolbox.Core.AppFramework;
using OSGeo.FDO.Connections;

namespace FdoCmd.Commands
{
    [Verb("list-connection-params", HelpText = "Lists connection parameters for the given FDO provider")]
    public class ListConnectionParametersCommand : ProviderConnectionCommand, ISummarizableCommand
    {
        [Option("full-details", Required = false, Default = false, HelpText = "If specified, print out full details of each parameter")]
        public bool Detailed { get; set; }

        protected override bool IsValidConnectionStateForCommand(ConnectionState state)
        {
            return state == ConnectionState.ConnectionState_Open || state == ConnectionState.ConnectionState_Pending;
        }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            var ci = conn.ConnectionInfo;
            var dict = ci.ConnectionProperties;
            PrintUtils.WritePropertyDict(this, dict);
            return (int)CommandStatus.E_OK;
        }
    }
}
