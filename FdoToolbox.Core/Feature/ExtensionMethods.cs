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
using FdoToolbox.Core.Feature.Overrides;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Connections.Capabilities;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.Feature
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the number of features in a given class definition.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="bruteForce">Uses a brute force counting approach if no other approaches are available</param>
        /// <returns></returns>
        public static long GetFeatureCount(this IConnection conn, string className, string filter, bool bruteForce)
        {
            var walker = new SchemaWalker(conn);
            var cls = walker.GetClassByName(className);
            if (cls != null)
                return GetFeatureCount(conn, cls, filter, bruteForce);
            return 0;
        }

        /// <summary>
        /// Gets the number of features in a given class definition. If the class definition is a raster
        /// feature class, it will always return 0. This does not do counting by brute force if other
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="filter"></param>
        /// <param name="bruteForce">Use the brute force approach if no other approaches are available</param>
        /// <returns></returns>
        public static long GetFeatureCount(this IConnection conn, ClassDefinition classDef, string filter, bool bruteForce)
        {
            long count = 0;

            //HACK: Raster feature classes are un-countable.
            if (ExpressUtility.HasRaster(classDef))
                return count;

            string className = classDef.Name;
            string qualifiedName = classDef.QualifiedName;
            string property = "FEATURECOUNT";

            /* Try getting feature count by using these methods (in order of support):
             * 
             * - ISelectAggregate w/ count() expression function
             * - Raw SQL: SELECT COUNT(*) FROM [table name]
             * - IExtendedSelect's Count property
             * - Brute force (only if specified)
             */

            if (SupportsCommand(CommandType.CommandType_SelectAggregates) && classDef.IdentityProperties.Count > 0 && SupportsFunction("COUNT"))
            {
                using (var select = (ISelectAggregates)conn.CreateCommand(CommandType.CommandType_SelectAggregates))
                {
                    select.SetFeatureClassName(qualifiedName);
                    if (!string.IsNullOrEmpty(filter))
                        select.SetFilter(filter);

                    var funcArgs = new ExpressionCollection();
                    var arg = new Identifier(classDef.IdentityProperties[0].Name);
                    funcArgs.Add(arg);
                    var func = new Function("Count", funcArgs);
                    var computedId = new ComputedIdentifier(property, func);

                    select.PropertyNames.Add(computedId);
                    //select.PropertyNames.Add(new ComputedIdentifier(property, Expression.Parse("COUNT(" + classDef.IdentityProperties[0].Name + ")")));

                    using (IDataReader reader = select.Execute())
                    {
                        if (reader.ReadNext() && !reader.IsNull(property))
                        {
                            DataType dt = reader.GetDataType(property);
                            switch (dt)
                            {
                                case DataType.DataType_Int16:
                                    count = reader.GetInt16(property);
                                    break;
                                case DataType.DataType_Int32:
                                    count = reader.GetInt32(property);
                                    break;
                                case DataType.DataType_Int64:
                                    count = reader.GetInt64(property);
                                    break;
                                case DataType.DataType_Double:
                                    count = Convert.ToInt64(reader.GetDouble(property));
                                    break;
                                case DataType.DataType_Single:
                                    count = Convert.ToInt64(reader.GetSingle(property));
                                    break;
                                case DataType.DataType_Decimal:
                                    count = Convert.ToInt64(reader.GetDouble(property));
                                    break;
                            }
                        }
                        reader.Close();
                    }
                }
            }
            else if (SupportsCommand(CommandType.CommandType_SQLCommand))
            {
                using (var cmd = (ISQLCommand)conn.CreateCommand(CommandType.CommandType_SQLCommand))
                {
                    string sql = string.Empty;

                    //Perhaps the feature class name as given by the provider, does not match the underlying database
                    //table name. So see if there's an override that provides the proper name.
                    string table = className;
                    var ci = conn.ConnectionInfo;
                    ITableNameOverride ov = TableNameOverrideFactory.GetTableNameOverride(ci.ProviderName);
                    if (ov != null)
                        table = ov.GetTableName(className);

                    sql = string.Format("SELECT COUNT(*) AS {0} FROM {1}", property, table);
                    if (!string.IsNullOrEmpty(filter))
                        sql += " WHERE " + filter;

                    cmd.SQLStatement = sql;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.ReadNext())
                        {
                            DataType dt = reader.GetColumnType(property);
                            switch (dt)
                            {
                                case DataType.DataType_Int16:
                                    count = reader.GetInt16(property);
                                    break;
                                case DataType.DataType_Int32:
                                    count = reader.GetInt32(property);
                                    break;
                                case DataType.DataType_Int64:
                                    count = reader.GetInt64(property);
                                    break;
                                case DataType.DataType_Double:
                                    count = Convert.ToInt64(reader.GetDouble(property));
                                    break;
                                case DataType.DataType_Single:
                                    count = Convert.ToInt64(reader.GetSingle(property));
                                    break;
                                case DataType.DataType_Decimal:
                                    count = Convert.ToInt64(reader.GetDouble(property));
                                    break;
                            }
                        }
                        reader.Close();
                    }
                }
            }
            else if (SupportsCommand(CommandType.CommandType_ExtendedSelect))
            {
                using (var select = (IExtendedSelect)conn.CreateCommand(CommandType.CommandType_ExtendedSelect))
                {
                    select.SetFeatureClassName(qualifiedName);
                    if (!string.IsNullOrEmpty(filter))
                        select.SetFilter(filter);

                    using (IScrollableFeatureReader reader = select.ExecuteScrollable())
                    {
                        count = reader.Count();
                        reader.Close();
                    }
                }
            }
            else if (bruteForce)
            {
                using (var select = (ISelect)conn.CreateCommand(CommandType.CommandType_Select))
                {
                    select.SetFeatureClassName(qualifiedName);
                    if (!string.IsNullOrEmpty(filter))
                        select.SetFilter(filter);
                    using (IFeatureReader reader = select.Execute())
                    {
                        while (reader.ReadNext())
                        {
                            count++;
                        }
                        reader.Close();
                    }
                }
            }
            return count;

            bool SupportsCommand(CommandType cmd)
            {
                return Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)cmd) >= 0;
            }

            bool SupportsFunction(string name)
            {
                var exprCaps = conn.ExpressionCapabilities;
                foreach (FunctionDefinition funcDef in exprCaps.Functions)
                {
                    if (funcDef.Name.ToUpper() == name.ToUpper())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

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
