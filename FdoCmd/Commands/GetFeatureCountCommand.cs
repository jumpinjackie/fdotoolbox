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
using System.Globalization;

namespace FdoCmd.Commands
{
    [Verb("get-feature-count", HelpText = "Gets the total number of features in the given feature class")]
    public class GetFeatureCountCommand : ProviderConnectionCommand
    {
        [Option("class", HelpText = "The name of the feature class")]
        public string ClassName { get; set; }

        [Option("filter", HelpText = "An optional FDO filter")]
        public string Filter { get; set; }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            var total = conn.GetFeatureCount(ClassName, Filter, true);
            WriteLine(total.ToString(CultureInfo.InvariantCulture));
            return (int)CommandStatus.E_OK;
        }
    }
}
