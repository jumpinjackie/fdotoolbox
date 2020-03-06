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
    [Verb("get-spatial-contexts", HelpText = "Gets spatial contexts for the given connection")]
    public class ListSpatialContextsCommand : ProviderConnectionCommand
    {
        protected override int ExecuteConnection(IConnection conn)
        {
            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                var contexts = service.GetSpatialContexts();
                PrintUtils.WriteSpatialContexts(this, contexts);
            }
            return (int)CommandStatus.E_OK;
        }
    }
}
