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
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Defines a conversion rule between two data properties
    /// </summary>
    public class FdoDataPropertyConversionRule
    {
        /// <summary>
        /// Gets the source property.
        /// </summary>
        /// <value>The source property.</value>
        public string SourceProperty { get; }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <value>The target property.</value>
        public string TargetProperty { get; }

        /// <summary>
        /// Gets the type of the source data.
        /// </summary>
        /// <value>The type of the source data.</value>
        public DataType SourceDataType { get; }

        /// <summary>
        /// Gets the type of the target data.
        /// </summary>
        /// <value>The type of the target data.</value>
        public DataType TargetDataType { get; }

        /// <summary>
        /// Gets a value indicating whether conversion will return null on failed conversion
        /// </summary>
        /// <value><c>true</c> if [null on failure]; otherwise, <c>false</c>.</value>
        public bool NullOnFailure { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FdoDataPropertyConversionRule"/> will truncate.
        /// </summary>
        /// <value><c>true</c> if truncate; otherwise, <c>false</c>.</value>
        public bool Truncate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoDataPropertyConversionRule"/> class.
        /// </summary>
        /// <param name="src">The source property</param>
        /// <param name="dst">The target property</param>
        /// <param name="srcDataType">The source data type</param>
        /// <param name="targetDataType">The target data type</param>
        /// <param name="nullOnFailure">if set to <c>true</c> [null on failure].</param>
        /// <param name="truncate">if set to <c>true</c> [truncate].</param>
        public FdoDataPropertyConversionRule(string src, string dst, DataType srcDataType, DataType targetDataType, bool nullOnFailure, bool truncate)
        {
            NullOnFailure = nullOnFailure;
            Truncate = truncate;
            SourceDataType = srcDataType;
            TargetDataType = targetDataType;
            SourceProperty = src;
            TargetProperty = dst;
        }
    }
}
