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

namespace FdoToolbox.Express.Controls.Ogr
{
    public enum OgrType
    {
        /// <summary>
        /// Generic interface
        /// </summary>
        Generic,
        /// <summary>
        /// OGR Virtual Driver
        /// </summary>
        Virtual,
        /// <summary>
        /// MapInfo
        /// </summary>
        MapInfo,
        ///// <summary>
        ///// ESRI Shape File
        ///// </summary>
        ShapeFile,
        ///// <summary>
        ///// Geography Markup Language
        ///// </summary>
        //GML,
        ///// <summary>
        ///// MySQL
        ///// </summary>
        //MySQL,
        ///// <summary>
        ///// S-57
        ///// </summary>
        S57,
        /// <summary>
        /// Comma Separated values
        /// </summary>
        CSV,
        ///// <summary>
        ///// Microstation DGN
        ///// </summary>
        DGN,
        ///// <summary>
        ///// GeoJSON
        ///// </summary>
        GeoJSON,
        /// <summary>
        /// ESRI Personal Geodatabase
        /// </summary>
        EsriPGB,
        /// <summary>
        /// ESRI ArcInfo Coverage (*.e00)
        /// </summary>
        ArcCoverage,
        /// <summary>
        /// Atlas BNA
        /// </summary>
        AtlasBna
    }
}
