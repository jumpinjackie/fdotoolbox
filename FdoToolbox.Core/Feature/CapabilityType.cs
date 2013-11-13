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

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Describes all the capabilities available in FDO
    /// </summary>
    public enum CapabilityType
    {
        // Command Capabilities

        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_CommandList,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsParameters,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsCommandTimeout,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSelectExpressions,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSelectFunctions,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSelectDistinct,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSelectOrdering,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSelectGrouping,

        // Connection Capabilities

        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_ThreadCapability,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SpatialContextTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_JoinTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_LockTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsConnectionTimeout,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsJoins,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsLocking,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsTransactions,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsLongTransactions,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSQL,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsConfiguration,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsMultipleSpatialContexts,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsCSysWKTFromCSysName,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsWrite,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsMultiUserWrite,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsFlush,

        // Expression Capabilities

        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_ExpressionTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_ExpressionFunctions,

        // Filter Capabilities

        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_ConditionTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SpatialOperations,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_DistanceOperations,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsGeodesicDistance,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsNonLiteralGeometricOperations,

        // Geometry Capabilities

        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_GeometryTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_GeometryComponentTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_Dimensionalities,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_CSSupportsArcs,

        // Raster Capabilities

        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsRaster,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsStitching,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSubsampling,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsDataModel,

        // Schema Capabilities

        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_ClassTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_DataTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportedAutoGeneratedTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportedIdentityPropertyTypes,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_String,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_BLOB,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_CLOB,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Decimal,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Boolean,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Byte,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_DateTime,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Double,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Int16,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Int32,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Int64,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDataValueLength_Single,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDecimalPrecision,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_MaximumDecimalScale,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_NameSizeLimit_Datastore,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_NameSizeLimit_Schema,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_NameSizeLimit_Class,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_NameSizeLimit_Property,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_NameSizeLimit_Description,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_ReservedCharactersForName,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsAssociationProperties,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsAutoIdGeneration,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsCalculatedProperties,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsCompositeId,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsCompositeUniqueValueConstraints,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsDefaultValue,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsExclusiveValueRangeConstraints,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsInclusiveValueRangeConstraints,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsInheritance,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsMultipleSchemas,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsNetworkModel,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsNullValueConstraints,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsObjectProperties,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSchemaModification,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsSchemaOverrides,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsUniqueValueConstraints,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsValueConstraintsList,
        /// <summary>
        /// 
        /// </summary>
        FdoCapabilityType_SupportsWritableIdentityProperties,

    }  //  enum FdoCapabilityType
}
