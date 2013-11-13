#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using System.Collections.ObjectModel;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoInfo
{
    public class ListSpatialContextsCommand : ConsoleCommand
    {
        private string _provider;
        private string _connstr;

        public ListSpatialContextsCommand(string provider, string connStr)
        {
            _provider = provider;
            _connstr = connStr;
        }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = CreateConnection(_provider, _connstr);
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }

            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                ReadOnlyCollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                Console.WriteLine("\nSpatial Contexts in connection: {0}", contexts.Count);
                foreach (SpatialContextInfo ctx in contexts)
                {
                    Console.WriteLine("\nName: {0}\n", ctx.Name);
                    Console.WriteLine("\tDescriptionn: {0}", ctx.Description);
                    Console.WriteLine("\tXY Tolerance: {0}\n\tZ Tolerance: {1}", ctx.XYTolerance, ctx.ZTolerance);
                    Console.WriteLine("\tCoordinate System: {0}\n\tCoordinate System WKT:\n\t\t{1}", ctx.CoordinateSystem, ctx.CoordinateSystemWkt);
                    Console.WriteLine("\tExtent Type: {0}", ctx.ExtentType);
                    if (ctx.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Static)
                        Console.WriteLine("\tExtent:\n\t\t{0}", ctx.ExtentGeometryText);
                }
            }
            conn.Close();
            return (int)CommandStatus.E_OK;
        }
    }
}
