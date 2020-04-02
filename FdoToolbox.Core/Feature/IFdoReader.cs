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
namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// A "bridge" interface that all fdo reader wrapper classes implement
    /// </summary>
    public interface IFdoReader : System.Data.IDataReader
    {
        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool GetBoolean(string name);
        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        byte GetByte(string name);
        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        DateTime GetDateTime(string name);
        /// <summary>
        /// Gets the type of the fdo property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        FdoPropertyType GetFdoPropertyType(string name);
        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        double GetDouble(string name);
        /// <summary>
        /// Gets the geometry.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        byte[] GetGeometry(string name);
        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        short GetInt16(string name);
        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        int GetInt32(string name);
        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        long GetInt64(string name);
        /// <summary>
        /// Gets the LOB.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        OSGeo.FDO.Expression.LOBValue GetLOB(string name);
        /// <summary>
        /// Gets the LOB stream reader.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        OSGeo.FDO.Common.IStreamReader GetLOBStreamReader(string name);
        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        float GetSingle(string name);
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string GetString(string name);
        /// <summary>
        /// Determines whether the specified property name is null.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified name is null; otherwise, <c>false</c>.
        /// </returns>
        bool IsNull(string name);
        /// <summary>
        /// Reads the next feature/row
        /// </summary>
        /// <returns></returns>
        bool ReadNext();
        /// <summary>
        /// Gets the geometry properties.
        /// </summary>
        /// <value>The geometry properties.</value>
        string[] GeometryProperties { get; }
        /// <summary>
        /// Gets the default geometry property.
        /// </summary>
        /// <value>The default geometry property.</value>
        string DefaultGeometryProperty { get; }
        /// <summary>
        /// Gets the spatial context association for a geometry property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetSpatialContextAssociation(string name);
        /// <summary>
        /// Determines whether the specified property name is an identity property
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property is an identity property; otherwise, <c>false</c>.
        /// </returns>
        bool IsIdentity(string name);

        /// <summary>
        /// Gets the name of the feature class that this reader originates from. If this reader was
        /// produced from a SQL or aggregate query, an empty string is returned.
        /// </summary>
        /// <returns></returns>
        string GetClassName();
    }
}
