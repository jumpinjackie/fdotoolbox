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

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Codes for an incompatible class
    /// </summary>
    public enum IncompatibleClassReason
    {
        /// <summary>
        /// The class type is not supported by the target connection
        /// </summary>
        UnsupportedClassType,
        /// <summary>
        /// The target connection does not support multiple identity properties
        /// </summary>
        UnsupportedCompositeKeys,
        /// <summary>
        /// The target connection does not support inheritance
        /// </summary>
        UnsupportedInheritance,
        /// <summary>
        /// The target connection does not support auto-generated properties
        /// </summary>
        UnsupportedAutoProperties
    }

    /// <summary>
    /// Codes for an incompatible class property
    /// </summary>
    public enum IncompatiblePropertyReason
    {
        /// <summary>
        /// The property is an identity property which is not supported
        /// </summary>
        UnsupportedIdentityProperty,
        /// <summary>
        /// The property is an identity property whose data type is not supported
        /// </summary>
        UnsupportedIdentityPropertyType,
        /// <summary>
        /// The property is an auto-generated property whose data type is not supported
        /// </summary>
        UnsupportedAutoGeneratedType,
        /// <summary>
        /// The property is an association property which is not supported
        /// </summary>
        UnsupportedAssociationProperties,
        /// <summary>
        /// The property has exclusive value range constraints which are not supported
        /// </summary>
        UnsupportedExclusiveValueRangeConstraints,
        /// <summary>
        /// The property has inclusive value range constraints which are not supported
        /// </summary>
        UnsupportedInclusiveValueRangeConstraints,
        /// <summary>
        /// The property is nullable which is not supported
        /// </summary>
        UnsupportedNullValueConstraints,
        /// <summary>
        /// The property is an object property which is not supported
        /// </summary>
        UnsupportedObjectProperties,
        /// <summary>
        /// The property has unique value constraints which are not supported
        /// </summary>
        UnsupportedUniqueValueConstraints,
        /// <summary>
        /// The property has value list constraints which are not supported
        /// </summary>
        UnsupportedValueListConstraints,
        /// <summary>
        /// The property's data type is not supported
        /// </summary>
        UnsupportedDataType,
        /// <summary>
        /// The property has default values which are not supported
        /// </summary>
        UnsupportedDefaultValues,
        /// <summary>
        /// The property is a string/BLOB/CLOB and it has a length of 0
        /// </summary>
        ZeroLengthProperty
    }
}
