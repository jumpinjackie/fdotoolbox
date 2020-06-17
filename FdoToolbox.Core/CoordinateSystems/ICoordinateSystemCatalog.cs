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

using System;
using System.Collections.Generic;

namespace FdoToolbox.Core.CoordinateSystems
{
    /// <summary>
    /// Coordinate system catalog interface
    /// </summary>
    public interface ICoordinateSystemCatalog : IDisposable
    {
        /// <summary>
        /// Gets an array of coordinate system categories
        /// </summary>
        ICoordinateSystemCategory[] Categories { get; }

        /// <summary>
        /// Convers the specified coordinate system code to WKT
        /// </summary>
        /// <param name="coordcode"></param>
        /// <returns></returns>
        string ConvertCoordinateSystemCodeToWkt(string coordcode);

        /// <summary>
        /// Converts the specified epsg code to WKT
        /// </summary>
        /// <param name="epsg"></param>
        /// <returns></returns>
        string ConvertEpsgCodeToWkt(string epsg);

        /// <summary>
        /// Converts the specified WKT into a coordinate system code
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        string ConvertWktToCoordinateSystemCode(string wkt);

        /// <summary>
        /// Converts the specified WKT into an EPSG code
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        string ConvertWktToEpsgCode(string wkt);

        /// <summary>
        /// Gets an array of all coordinate systems in this catalog
        /// </summary>
        IEnumerable<ICoordinateSystem> AllCoordinateSystems { get; }

        /// <summary>
        /// Gets an array of all coordinate systems in the specified category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<ICoordinateSystem> EnumerateCoordinateSystems(string category);

        /// <summary>
        /// Gets the coordinate system that matches the specified code
        /// </summary>
        /// <param name="coordcode"></param>
        /// <returns></returns>
        ICoordinateSystem FindCoordinateSystemByCode(string coordcode);

        /// <summary>
        /// Gets the coordinate system that matches the specified epsg code
        /// </summary>
        /// <param name="epsg"></param>
        /// <returns></returns>
        ICoordinateSystem FindCoordinateSystemByEpsgCode(string epsg);

        /// <summary>
        /// Creates an empty coordinate system
        /// </summary>
        /// <returns></returns>
        ICoordinateSystem CreateEmptyCoordinateSystem();

        /// <summary>
        /// Creates an arbitrary coordinate system for the given WKT
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        ICoordinateSystem CreateArbitraryCoordinateSystem(string wkt);

        /// <summary>
        /// Indicates if the coordinate system WKT is valid
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        bool IsValid(string wkt);
    }
}
