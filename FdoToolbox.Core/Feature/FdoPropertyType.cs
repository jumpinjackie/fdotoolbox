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
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Indicates the property type from a FDO reader
    /// </summary>
    public enum FdoPropertyType
    {
        /// <summary>
        /// Property is a boolean
        /// </summary>
        Boolean = 0,
        /// <summary>
        /// Property is a byte
        /// </summary>
        Byte = 1,
        /// <summary>
        /// Property is a DateTime
        /// </summary>
        DateTime = 2,
        /// <summary>
        /// Property is a decimal
        /// </summary>
        Decimal = 3,
        /// <summary>
        /// Property is a double
        /// </summary>
        Double = 4,
        /// <summary>
        /// Property is a Int16
        /// </summary>
        Int16 = 5,
        /// <summary>
        /// Property is a Int32
        /// </summary>
        Int32 = 6,
        /// <summary>
        /// Property is a Int64
        /// </summary>
        Int64 = 7,
        /// <summary>
        /// Property is a single
        /// </summary>
        Single = 8,
        /// <summary>
        /// Property is a string
        /// </summary>
        String = 9,
        /// <summary>
        /// Property is a BLOB
        /// </summary>
        BLOB = 10,
        /// <summary>
        /// Property is a CLOB
        /// </summary>
        CLOB = 11,
        /// <summary>
        /// Property is a Geometry
        /// </summary>
        Geometry,
        /// <summary>
        /// Property is an object
        /// </summary>
        Object,
        /// <summary>
        /// Property is an association
        /// </summary>
        Association,
        /// <summary>
        /// Property is a raster
        /// </summary>
        Raster
    }
}
