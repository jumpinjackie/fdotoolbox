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
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Connections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.Feature
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Enumerates all spatial contexts in the current connection
        /// </summary>
        /// <returns></returns>
        public static ReadOnlyCollection<SpatialContextInfo> GetSpatialContexts(this IConnection conn)
        {
            var contexts = new List<SpatialContextInfo>();
            using (var get = (IGetSpatialContexts)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_GetSpatialContexts))
            {
                get.ActiveOnly = false;
                using (var reader = get.Execute())
                {
                    while (reader.ReadNext())
                    {
                        SpatialContextInfo info = new SpatialContextInfo(reader);
                        contexts.Add(info);
                    }
                }
            }
            return contexts.AsReadOnly();
        }

        public static SpatialContextInfo GetActiveSpatialContext(this IConnection conn)
        {
            using (var get = (IGetSpatialContexts)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_GetSpatialContexts))
            {
                using (var reader = get.Execute())
                {
                    if (reader.ReadNext())
                    {
                        SpatialContextInfo info = new SpatialContextInfo(reader);
                        return info;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a spatial context by name
        /// </summary>
        /// <param name="name">The name of the spatial context</param>
        /// <returns>The spatial context information if found. null if otherwise</returns>
        public static SpatialContextInfo GetSpatialContext(this IConnection conn, string name)
        {
            ReadOnlyCollection<SpatialContextInfo> contexts = GetSpatialContexts(conn);
            foreach (SpatialContextInfo info in contexts)
            {
                if (info.Name == name)
                    return info;
            }
            return null;
        }
    }
}
