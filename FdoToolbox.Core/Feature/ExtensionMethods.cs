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
        /// <param name="schemaName">The schema name</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="bruteForce">Uses a brute force counting approach if no other approaches are available</param>
        /// <returns></returns>
        public static long GetFeatureCount(this IConnection conn, string schemaName, string className, string filter, bool bruteForce)
        {
            var walker = new SchemaWalker(conn);
            var cls = walker.GetClassByName(schemaName, className);
            if (cls != null)
                return GetFeatureCount(conn, cls, filter, bruteForce);
            return 0;
        }

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
                using (var cmdCaps = conn.CommandCapabilities)
                {
                    return Array.IndexOf<int>(cmdCaps.Commands, (int)cmd) >= 0;
                }
            }

            bool SupportsFunction(string name)
            {
                using (var exprCaps = conn.ExpressionCapabilities)
                {
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

        static void ApplyChangesFrom(this GeometricPropertyDefinition source, GeometricPropertyDefinition input)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // TODO: Handle source.Attributes?

            source.Description = input.Description;
            source.GeometryTypes = input.GeometryTypes;
            source.HasElevation = input.HasElevation;
            source.HasMeasure = input.HasMeasure;
            source.IsSystem = input.IsSystem;
            source.ReadOnly = input.ReadOnly;
            source.SpatialContextAssociation = input.SpatialContextAssociation;
            source.SpecificGeometryTypes = input.SpecificGeometryTypes;
        }

        static void ApplyChangesFrom(this DataPropertyDefinition source, DataPropertyDefinition input)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // TODO: Handle source.Attributes?

            source.DataType = input.DataType;
            source.DefaultValue = input.DefaultValue;
            source.Description = input.Description;
            source.IsAutoGenerated = input.IsAutoGenerated;
            source.IsSystem = input.IsSystem;
            source.Length = input.Length;
            source.Nullable = input.Nullable;
            source.Precision = input.Precision;
            source.ReadOnly = input.ReadOnly;
            source.Scale = input.Scale;

            // TODO: Handle source.ValueConstraint?
        }

        /// <summary>
        /// For the source schema, make modifications based on the input schema
        /// 
        ///  - For any class in input not in source, add it
        ///  - For any class already in source, add any new properties to it from input's class
        ///  - For any class property already in source, alter it with the one from input's class property if present
        /// </summary>
        /// <param name="source"></param>
        /// <param name="input"></param>
        /// <param name="logger"></param>
        public static void ApplyChangesFrom(this FeatureSchema source, FeatureSchema input, Action<string> logger)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var srcClasses = source.Classes;
            var inClasses = input.Classes;

            // For any class in input not in source, add it
            foreach (ClassDefinition cls in inClasses)
            {
                var scidx = srcClasses.IndexOf(cls.Name);
                if (scidx < 0) //Class not in source, add it
                {
                    var clone = FdoSchemaUtil.CloneClass(cls);
                    srcClasses.Add(clone);
                }
                else //Class already in source
                {
                    // For any class already in source, add any new properties to it from input's class
                    // For any class property already in source, alter it with the one from input's class property if present

                    var srcClass = srcClasses[scidx];
                    var srcClassProps = srcClass.Properties;

                    var inClassProps = cls.Properties;
                    foreach (PropertyDefinition pd in inClassProps)
                    {
                        var scpidx = srcClassProps.IndexOf(pd.Name);
                        if (scpidx < 0)
                        {
                            var prop = FdoSchemaUtil.CloneProperty(pd);
                            srcClassProps.Add(prop);

                            // Check if the added prop was identity, because it needs to be
                            // added on the source as well
                            if (pd is DataPropertyDefinition dp)
                            {
                                var inClsIdProps = cls.IdentityProperties;
                                if (inClsIdProps.Contains(dp))
                                {
                                    var srcClsIdProps = srcClass.IdentityProperties;
                                    srcClsIdProps.Add((DataPropertyDefinition)prop);
                                }
                            }
                        }
                        else // Exists, alter it if possible
                        {
                            var srcProp = srcClassProps[scpidx];
                            if (srcProp.PropertyType != pd.PropertyType)
                            {
                                logger($"[WARNING]: Skipping property ({srcProp.QualifiedName}) as the input property's type differs from it");
                            }
                            else
                            {
                                switch (srcProp.PropertyType)
                                {
                                    case PropertyType.PropertyType_DataProperty:
                                        ((DataPropertyDefinition)srcProp).ApplyChangesFrom((DataPropertyDefinition)pd);
                                        break;
                                    case PropertyType.PropertyType_GeometricProperty:
                                        ((GeometricPropertyDefinition)srcProp).ApplyChangesFrom((GeometricPropertyDefinition)pd);
                                        break;
                                    default:
                                        logger($"[WARNING]: Skipping property ({srcProp.QualifiedName}) as modifying properties of type ({srcProp.PropertyType}) is not supported");
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
