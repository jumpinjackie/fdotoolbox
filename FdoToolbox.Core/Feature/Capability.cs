#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Filter;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Common;
using OSGeo.FDO.Commands.Locking;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Connections.Capabilities;
using OSGeo.FDO.Commands;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Generic provider capability interface
    /// </summary>
    public interface ICapability : IDisposable
    {
        /// <summary>
        /// Gets the boolean capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        bool GetBooleanCapability(CapabilityType cap);
        /// <summary>
        /// Gets the int32 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        int GetInt32Capability(CapabilityType cap);
        /// <summary>
        /// Gets the int64 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        long GetInt64Capability(CapabilityType cap);
        /// <summary>
        /// Gets the string capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        string GetStringCapability(CapabilityType cap);
        /// <summary>
        /// Gets the array capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        Array GetArrayCapability(CapabilityType cap);
        /// <summary>
        /// Gets the object capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        object GetObjectCapability(CapabilityType cap);
        /// <summary>
        /// Determines if an array capability contains the specified value
        /// </summary>
        /// <param name="capabilityType">The capability (must be an array capability)</param>
        /// <param name="value">The value to check for</param>
        /// <returns>True of the value exists; false otherwise</returns>
        bool HasArrayCapability(CapabilityType capabilityType, object value);
        /// <summary>
        /// Gets the CLR data type of the given capability value
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The CLR data type, returns null if the data type is not known for this capability</returns>
        Type GetCapabilityValueType(CapabilityType type);
    }

    /// <summary>
    /// Allows querying of FDO provider capabilities in a generic fashion.
    /// </summary>
    public class Capability : ICapability
    {
        private Lazy<ICommandCapabilities> commandCaps;
        private Lazy<IConnectionCapabilities> connCaps;
        private Lazy<IExpressionCapabilities> exprCaps;
        private Lazy<IGeometryCapabilities> geomCaps;
        private Lazy<IFilterCapabilities> filterCaps;
        private Lazy<IRasterCapabilities> rasterCaps;
        private Lazy<ISchemaCapabilities> schemaCaps;
        private Lazy<ITopologyCapabilities> topoCaps;

        internal Capability(FdoConnection conn)
        {
            IConnection internalConn = conn.InternalConnection;
            commandCaps = new Lazy<ICommandCapabilities>(() => internalConn.CommandCapabilities);
            connCaps = new Lazy<IConnectionCapabilities>(() => internalConn.ConnectionCapabilities);
            exprCaps = new Lazy<IExpressionCapabilities>(() => internalConn.ExpressionCapabilities);
            filterCaps = new Lazy<IFilterCapabilities>(() => internalConn.FilterCapabilities);
            geomCaps = new Lazy<IGeometryCapabilities>(() => internalConn.GeometryCapabilities);
            rasterCaps = new Lazy<IRasterCapabilities>(() => internalConn.RasterCapabilities);
            schemaCaps = new Lazy<ISchemaCapabilities>(() => internalConn.SchemaCapabilities);
            topoCaps = new Lazy<ITopologyCapabilities>(() => internalConn.TopologyCapabilities);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (commandCaps.IsValueCreated)
                commandCaps.Value.Dispose();
            if (connCaps.IsValueCreated)
                connCaps.Value.Dispose();
            if (exprCaps.IsValueCreated)
                exprCaps.Value.Dispose();
            if (geomCaps.IsValueCreated)
                geomCaps.Value.Dispose();
            if (filterCaps.IsValueCreated)
                filterCaps.Value.Dispose();
            if (rasterCaps.IsValueCreated)
                rasterCaps.Value.Dispose();
            if (schemaCaps.IsValueCreated)
                schemaCaps.Value.Dispose();
            if (topoCaps.IsValueCreated)
                topoCaps.Value.Dispose();

            commandCaps = null;
            connCaps = null;
            exprCaps = null;
            geomCaps = null;
            filterCaps = null;
            rasterCaps = null;
            schemaCaps = null;
            topoCaps = null;
        }

        /// <summary>
        /// Gets the boolean capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public bool GetBooleanCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_SupportsAssociationProperties:
                    return schemaCaps.Value.SupportsAssociationProperties;
                case CapabilityType.FdoCapabilityType_SupportsAutoIdGeneration:
                    return schemaCaps.Value.SupportsAutoIdGeneration;
                //case CapabilityType.FdoCapabilityType_SupportsCalculatedProperties:
                case CapabilityType.FdoCapabilityType_SupportsCompositeId:
                    return schemaCaps.Value.SupportsCompositeId;
                case CapabilityType.FdoCapabilityType_SupportsCompositeUniqueValueConstraints:
                    return schemaCaps.Value.SupportsCompositeUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsConfiguration:
                    return connCaps.Value.SupportsConfiguration();
                case CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName:
                    return connCaps.Value.SupportsCSysWKTFromCSysName();
                //case CapabilityType.FdoCapabilityType_SupportsDataModel:
                    //return rasterCaps.Value.SupportsDataModel(OSGeo.FDO.Raster.RasterDataModel;
                case CapabilityType.FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration:
                    return schemaCaps.Value.SupportsDataStoreScopeUniqueIdGeneration;
                case CapabilityType.FdoCapabilityType_SupportsDefaultValue:
                    return schemaCaps.Value.SupportsDefaultValue;
                case CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints:
                    return schemaCaps.Value.SupportsExclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsFlush:
                    return connCaps.Value.SupportsFlush();
                //case CapabilityType.FdoCapabilityType_SupportsGeodesicDistance:
                case CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints:
                    return schemaCaps.Value.SupportsInclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsInheritance:
                    return schemaCaps.Value.SupportsInheritance;
                case CapabilityType.FdoCapabilityType_SupportsJoins:
                    return connCaps.Value.SupportsJoins();
                case CapabilityType.FdoCapabilityType_SupportsLocking:
                    return connCaps.Value.SupportsLocking();
                case CapabilityType.FdoCapabilityType_SupportsLongTransactions:
                    return connCaps.Value.SupportsLongTransactions();
                case CapabilityType.FdoCapabilityType_SupportsMultipleSchemas:
                    return schemaCaps.Value.SupportsMultipleSchemas;
                case CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts:
                    return connCaps.Value.SupportsMultipleSpatialContexts();
                //case CapabilityType.FdoCapabilityType_SupportsMultiUserWrite:
                case CapabilityType.FdoCapabilityType_SupportsNetworkModel:
                    return schemaCaps.Value.SupportsNetworkModel;
                //case CapabilityType.FdoCapabilityType_SupportsNonLiteralGeometricOperations:
                case CapabilityType.FdoCapabilityType_SupportsNullValueConstraints:
                    return schemaCaps.Value.SupportsNullValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsObjectProperties:
                    return schemaCaps.Value.SupportsObjectProperties;
                case CapabilityType.FdoCapabilityType_SupportsParameters:
                    return commandCaps.Value.SupportsParameters();
                case CapabilityType.FdoCapabilityType_SupportsRaster:
                    return rasterCaps.Value.SupportsRaster();
                case CapabilityType.FdoCapabilityType_SupportsSchemaModification:
                    return schemaCaps.Value.SupportsSchemaModification;
                case CapabilityType.FdoCapabilityType_SupportsSchemaOverrides:
                    return schemaCaps.Value.SupportsSchemaOverrides;
                case CapabilityType.FdoCapabilityType_SupportsSelectDistinct:
                    return commandCaps.Value.SupportsSelectDistinct();
                case CapabilityType.FdoCapabilityType_SupportsSelectExpressions:
                    return commandCaps.Value.SupportsSelectExpressions();
                case CapabilityType.FdoCapabilityType_SupportsSelectFunctions:
                    return commandCaps.Value.SupportsSelectFunctions();
                case CapabilityType.FdoCapabilityType_SupportsSelectGrouping:
                    return commandCaps.Value.SupportsSelectGrouping();
                case CapabilityType.FdoCapabilityType_SupportsSelectOrdering:
                    return commandCaps.Value.SupportsSelectOrdering();
                case CapabilityType.FdoCapabilityType_SupportsSQL:
                    return connCaps.Value.SupportsSQL();
                case CapabilityType.FdoCapabilityType_SupportsStitching:
                    return rasterCaps.Value.SupportsStitching();
                case CapabilityType.FdoCapabilityType_SupportsSubsampling:
                    return rasterCaps.Value.SupportsSubsampling();
                case CapabilityType.FdoCapabilityType_SupportsCommandTimeout:
                    return commandCaps.Value.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsConnectionTimeout:
                    return connCaps.Value.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsTransactions:
                    return connCaps.Value.SupportsTransactions();
                case CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints:
                    return schemaCaps.Value.SupportsUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsValueConstraintsList:
                    return schemaCaps.Value.SupportsValueConstraintsList;
                //case CapabilityType.FdoCapabilityType_SupportsWritableIdentityProperties:
                //case CapabilityType.FdoCapabilityType_SupportsWrite:
                default:
                    throw new ArgumentException(cap.ToString());
            }
        }

        /// <summary>
        /// Gets the int32 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public int GetInt32Capability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return schemaCaps.Value.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return schemaCaps.Value.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);
                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return geomCaps.Value.Dimensionalities;
                default:
                    throw new ArgumentException(cap.ToString());
            }
        }

        /// <summary>
        /// Gets the int64 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public long GetInt64Capability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_BLOB:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_BLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Boolean:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Boolean);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Byte:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Byte);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_CLOB:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_CLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_DateTime:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_DateTime);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Decimal:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Decimal);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Double:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Double);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int16:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int16);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int32:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int32);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int64:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int64);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Single:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Single);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_String:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_String);

                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return schemaCaps.Value.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return schemaCaps.Value.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);
             
                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return geomCaps.Value.Dimensionalities;

                default:
                    throw new ArgumentException(cap.ToString());
            }
        }

        /// <summary>
        /// Gets the string capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public string GetStringCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_ReservedCharactersForName:
                    return schemaCaps.Value.ReservedCharactersForName;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the array capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public Array GetArrayCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_ClassTypes:
                    //return Array.ConvertAll<ClassType, int>(schemaCaps.Value.ClassTypes, delegate(ClassType ct) { return (int)ct; });
                    return schemaCaps.Value.ClassTypes;
                case CapabilityType.FdoCapabilityType_CommandList:
                    return Array.ConvertAll<int, CommandType>(commandCaps.Value.Commands, delegate(int cmd) { return (CommandType)cmd; });
                case CapabilityType.FdoCapabilityType_ConditionTypes:
                    //return Array.ConvertAll<ConditionType, int>(filterCaps.Value.ConditionTypes, delegate(ConditionType ct) { return (int)ct; });
                    return filterCaps.Value.ConditionTypes;
                case CapabilityType.FdoCapabilityType_DataTypes:
                    //return Array.ConvertAll<DataType, int>(schemaCaps.Value.DataTypes, delegate(DataType dt) { return (int)dt; });
                    return schemaCaps.Value.DataTypes;
                case CapabilityType.FdoCapabilityType_DistanceOperations:
                    //return Array.ConvertAll<DistanceOperations, int>(filterCaps.Value.DistanceOperations, delegate(DistanceOperations d) { return (int)d; });
                    return filterCaps.Value.DistanceOperations;
                case CapabilityType.FdoCapabilityType_ExpressionTypes:
                    //return Array.ConvertAll<ExpressionType, int>(exprCaps.Value.ExpressionTypes, delegate(ExpressionType e) { return (int)e; });
                    return exprCaps.Value.ExpressionTypes;
                case CapabilityType.FdoCapabilityType_GeometryComponentTypes:
                    //return Array.ConvertAll<GeometryComponentType, int>(geomCaps.Value.GeometryComponentTypes, delegate(GeometryComponentType g) { return (int)g; });
                    return geomCaps.Value.GeometryComponentTypes;
                case CapabilityType.FdoCapabilityType_GeometryTypes:
                    //return Array.ConvertAll<GeometryType, int>(geomCaps.Value.GeometryTypes, delegate(GeometryType g) { return (int)g; });
                    return geomCaps.Value.GeometryTypes;
                case CapabilityType.FdoCapabilityType_JoinTypes:
                    {
                        var jtypes = new List<JoinType>();
                        int val = connCaps.Value.JoinTypes;
                        foreach (JoinType jt in Enum.GetValues(typeof(JoinType)))
                        {
                            if ((val & (int)jt) == (int)jt)
                                jtypes.Add(jt);
                        }
                        return jtypes.ToArray();
                    }
                case CapabilityType.FdoCapabilityType_LockTypes:
                    //return Array.ConvertAll<LockType, int>(connCaps.Value.LockTypes, delegate(LockType l) { return (int)l; });
                    return connCaps.Value.LockTypes;
                case CapabilityType.FdoCapabilityType_SpatialContextTypes:
                    //return Array.ConvertAll<SpatialContextExtentType, int>(connCaps.Value.SpatialContextTypes, delegate(SpatialContextExtentType s) { return (int)s; });
                    return connCaps.Value.SpatialContextTypes;
                case CapabilityType.FdoCapabilityType_SpatialOperations:
                    //return Array.ConvertAll<SpatialOperations, int>(filterCaps.Value.SpatialOperations, delegate(SpatialOperations s) { return (int)s; });
                    return filterCaps.Value.SpatialOperations;
                case CapabilityType.FdoCapabilityType_SupportedAutoGeneratedTypes:
                    //return Array.ConvertAll<DataType, int>(schemaCaps.Value.SupportedAutoGeneratedTypes, delegate(DataType dt) { return (int)dt; });
                    return schemaCaps.Value.SupportedAutoGeneratedTypes;
                case CapabilityType.FdoCapabilityType_SupportedIdentityPropertyTypes:
                    //return Array.ConvertAll<DataType, int>(schemaCaps.Value.SupportedIdentityPropertyTypes, delegate(DataType dt) { return (int)dt; }); 
                    return schemaCaps.Value.SupportedIdentityPropertyTypes;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the object capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public object GetObjectCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_SupportedAutoGeneratedTypes:
                    return schemaCaps.Value.SupportedAutoGeneratedTypes;
                case CapabilityType.FdoCapabilityType_SupportedIdentityPropertyTypes:
                    return schemaCaps.Value.SupportedIdentityPropertyTypes;
                case CapabilityType.FdoCapabilityType_SupportsAssociationProperties:
                    return schemaCaps.Value.SupportsAssociationProperties;
                case CapabilityType.FdoCapabilityType_SupportsAutoIdGeneration:
                    return schemaCaps.Value.SupportsAutoIdGeneration;
                //case CapabilityType.FdoCapabilityType_SupportsCalculatedProperties:
                case CapabilityType.FdoCapabilityType_SupportsCompositeId:
                    return schemaCaps.Value.SupportsCompositeId;
                case CapabilityType.FdoCapabilityType_SupportsCompositeUniqueValueConstraints:
                    return schemaCaps.Value.SupportsCompositeUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsConfiguration:
                    return connCaps.Value.SupportsConfiguration();
                case CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName:
                    return connCaps.Value.SupportsCSysWKTFromCSysName();
                //case CapabilityType.FdoCapabilityType_SupportsDataModel:
                //    return rasterCaps.Value.SupportsDataModel();
                case CapabilityType.FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration:
                    return schemaCaps.Value.SupportsDataStoreScopeUniqueIdGeneration;
                case CapabilityType.FdoCapabilityType_SupportsDefaultValue:
                    return schemaCaps.Value.SupportsDefaultValue;
                case CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints:
                    return schemaCaps.Value.SupportsExclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsFlush:
                    return connCaps.Value.SupportsFlush();
                //case CapabilityType.FdoCapabilityType_SupportsGeodesicDistance:
                case CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints:
                    return schemaCaps.Value.SupportsInclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsInheritance:
                    return schemaCaps.Value.SupportsInheritance;
                case CapabilityType.FdoCapabilityType_SupportsJoins:
                    return GetBooleanCapability(cap);
                case CapabilityType.FdoCapabilityType_SupportsLocking:
                    return connCaps.Value.SupportsLocking();
                case CapabilityType.FdoCapabilityType_SupportsLongTransactions:
                    return connCaps.Value.SupportsLongTransactions();
                case CapabilityType.FdoCapabilityType_SupportsMultipleSchemas:
                    return schemaCaps.Value.SupportsMultipleSchemas;
                case CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts:
                    return connCaps.Value.SupportsMultipleSpatialContexts();
                //case CapabilityType.FdoCapabilityType_SupportsMultiUserWrite:
                case CapabilityType.FdoCapabilityType_SupportsNetworkModel:
                    return schemaCaps.Value.SupportsNetworkModel;
                //case CapabilityType.FdoCapabilityType_SupportsNonLiteralGeometricOperations:
                case CapabilityType.FdoCapabilityType_SupportsNullValueConstraints:
                    return schemaCaps.Value.SupportsNullValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsObjectProperties:
                    return schemaCaps.Value.SupportsObjectProperties;
                case CapabilityType.FdoCapabilityType_SupportsParameters:
                    return commandCaps.Value.SupportsParameters();
                case CapabilityType.FdoCapabilityType_SupportsRaster:
                    return rasterCaps.Value.SupportsRaster();
                case CapabilityType.FdoCapabilityType_SupportsSchemaModification:
                    return schemaCaps.Value.SupportsSchemaModification;
                case CapabilityType.FdoCapabilityType_SupportsSchemaOverrides:
                    return schemaCaps.Value.SupportsSchemaOverrides;
                case CapabilityType.FdoCapabilityType_SupportsSelectDistinct:
                    return commandCaps.Value.SupportsSelectDistinct();
                case CapabilityType.FdoCapabilityType_SupportsSelectExpressions:
                    return commandCaps.Value.SupportsSelectExpressions();
                case CapabilityType.FdoCapabilityType_SupportsSelectFunctions:
                    return commandCaps.Value.SupportsSelectFunctions();
                case CapabilityType.FdoCapabilityType_SupportsSelectGrouping:
                    return commandCaps.Value.SupportsSelectGrouping();
                case CapabilityType.FdoCapabilityType_SupportsSelectOrdering:
                    return commandCaps.Value.SupportsSelectOrdering();
                case CapabilityType.FdoCapabilityType_SupportsSQL:
                    return connCaps.Value.SupportsSQL();
                case CapabilityType.FdoCapabilityType_SupportsStitching:
                    return rasterCaps.Value.SupportsStitching();
                case CapabilityType.FdoCapabilityType_SupportsSubsampling:
                    return rasterCaps.Value.SupportsSubsampling();
                case CapabilityType.FdoCapabilityType_SupportsCommandTimeout:
                    return commandCaps.Value.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsConnectionTimeout:
                    return connCaps.Value.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsTransactions:
                    return connCaps.Value.SupportsTransactions();
                case CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints:
                    return schemaCaps.Value.SupportsUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsValueConstraintsList:
                    return schemaCaps.Value.SupportsValueConstraintsList;
                //case CapabilityType.FdoCapabilityType_SupportsWritableIdentityProperties:
                //case CapabilityType.FdoCapabilityType_SupportsWrite:
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_BLOB:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_BLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Boolean:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Boolean);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Byte:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Byte);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_CLOB:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_CLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_DateTime:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_DateTime);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Decimal:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Decimal);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Double:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Double);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int16:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int16);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int32:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int32);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int64:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int64);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Single:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Single);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_String:
                    return schemaCaps.Value.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_String);
                case CapabilityType.FdoCapabilityType_ExpressionFunctions:
                    return exprCaps.Value.Functions;

                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return schemaCaps.Value.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return schemaCaps.Value.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return schemaCaps.Value.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);

                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return geomCaps.Value.Dimensionalities;
                case CapabilityType.FdoCapabilityType_ReservedCharactersForName:
                    return schemaCaps.Value.ReservedCharactersForName;
                case CapabilityType.FdoCapabilityType_ClassTypes:
                    return schemaCaps.Value.ClassTypes;
                case CapabilityType.FdoCapabilityType_CommandList:
                    return Array.ConvertAll<int, CommandType>(commandCaps.Value.Commands, delegate(int i) { return (CommandType)i; });
                case CapabilityType.FdoCapabilityType_ConditionTypes:
                    return filterCaps.Value.ConditionTypes;
                case CapabilityType.FdoCapabilityType_DataTypes:
                    return schemaCaps.Value.DataTypes;
                case CapabilityType.FdoCapabilityType_DistanceOperations:
                    return filterCaps.Value.DistanceOperations;
                case CapabilityType.FdoCapabilityType_ExpressionTypes:
                    return exprCaps.Value.ExpressionTypes;
                case CapabilityType.FdoCapabilityType_GeometryComponentTypes:
                    return geomCaps.Value.GeometryComponentTypes;
                case CapabilityType.FdoCapabilityType_GeometryTypes:
                    return geomCaps.Value.GeometryTypes;
                case CapabilityType.FdoCapabilityType_LockTypes:
                    return connCaps.Value.LockTypes;
                case CapabilityType.FdoCapabilityType_JoinTypes:
                    return GetArrayCapability(cap);
                case CapabilityType.FdoCapabilityType_SpatialContextTypes:
                    return connCaps.Value.SpatialContextTypes;
                case CapabilityType.FdoCapabilityType_SpatialOperations:
                    return filterCaps.Value.SpatialOperations;
                case CapabilityType.FdoCapabilityType_ThreadCapability:
                    return connCaps.Value.ThreadCapability;

                default:
                    throw new ArgumentException(cap.ToString());
            }
        }


        /// <summary>
        /// Determines if an array capability contains the specified value
        /// </summary>
        /// <param name="capabilityType">The capability (must be an array capability)</param>
        /// <param name="value">The value to check for</param>
        /// <returns>
        /// True of the value exists; false otherwise
        /// </returns>
        public bool HasArrayCapability(CapabilityType capabilityType, object value)
        {
            Array values = this.GetArrayCapability(capabilityType);
            if (values != null)
                return Array.IndexOf(values, value) >= 0;
            return false;
        }

        /// <summary>
        /// Gets the CLR data type of the given capability value
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The CLR data type, returns null if the data type is not known for this capability</returns>
        public Type GetCapabilityValueType(CapabilityType type)
        {
            switch (type)
            {
                case CapabilityType.FdoCapabilityType_ClassTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_CommandList:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_ConditionTypes:
                    return typeof(Array);
                //case CapabilityType.FdoCapabilityType_CSSupportsArcs:
                //    return typeof(bool);
                case CapabilityType.FdoCapabilityType_DataTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_DistanceOperations:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_ExpressionFunctions:
                    return typeof(FunctionDefinitionCollection);
                case CapabilityType.FdoCapabilityType_ExpressionTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_GeometryComponentTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_GeometryTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_LockTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_BLOB:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Boolean:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Byte:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_CLOB:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_DateTime:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Decimal:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Double:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int16:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int32:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int64:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Single:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_String:
                    return typeof(long);
                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return typeof(int);
                case CapabilityType.FdoCapabilityType_ReservedCharactersForName:
                    return typeof(string);
                case CapabilityType.FdoCapabilityType_SpatialContextTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_SpatialOperations:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_SupportedAutoGeneratedTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_SupportedIdentityPropertyTypes:
                    return typeof(Array);
                case CapabilityType.FdoCapabilityType_SupportsAssociationProperties:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsAutoIdGeneration:
                    return typeof(bool);
                //case CapabilityType.FdoCapabilityType_SupportsCalculatedProperties:
                //    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsCommandTimeout:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsCompositeId:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsCompositeUniqueValueConstraints:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsConfiguration:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsConnectionTimeout:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName:
                    return typeof(bool);
                //case CapabilityType.FdoCapabilityType_SupportsDataModel:
                //    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsDefaultValue:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsFlush:
                    return typeof(bool);
                //case CapabilityType.FdoCapabilityType_SupportsGeodesicDistance:
                //    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsInheritance:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsLocking:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsLongTransactions:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsMultipleSchemas:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts:
                    return typeof(bool);
                //case CapabilityType.FdoCapabilityType_SupportsMultiUserWrite:
                //    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsNetworkModel:
                    return typeof(bool);
                //case CapabilityType.FdoCapabilityType_SupportsNonLiteralGeometricOperations:
                //    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsNullValueConstraints:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsObjectProperties:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsParameters:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsRaster:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSchemaModification:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSchemaOverrides:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSelectDistinct:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSelectExpressions:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSelectFunctions:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSelectGrouping:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSelectOrdering:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSQL:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsStitching:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsSubsampling:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsTransactions:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints:
                    return typeof(bool);
                case CapabilityType.FdoCapabilityType_SupportsValueConstraintsList:
                    return typeof(bool);
                //case CapabilityType.FdoCapabilityType_SupportsWritableIdentityProperties:
                //    return typeof(bool);
                //case CapabilityType.FdoCapabilityType_SupportsWrite:
                //    return typeof(bool);
                case CapabilityType.FdoCapabilityType_ThreadCapability:
                    return typeof(int);
                default:
                    return null;
            }
        }
    }
}
