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
using OSGeo.FDO.Commands.SpatialContext;
using System.ComponentModel;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Data Transfer Object for Spatial Contexts
    /// </summary>
    public class SpatialContextInfo
    {
        /// <summary>
        /// The name of the spatial context
        /// </summary>
        [DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Determines if this spatial context is the active one
        /// </summary>
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        /// <summary>
        /// The Z tolerance of this spatial context
        /// </summary>
        [DisplayName("Z Tolerance")]
        public double ZTolerance { get; set; }

        /// <summary>
        /// The X and Y tolerance of this spatial context
        /// </summary>
        [DisplayName("X/Y Tolerance")]
        public double XYTolerance { get; set; }

        /// <summary>
        /// The description of this spatial context
        /// </summary>
        [DisplayName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// The coordinate system name of this spatial context
        /// </summary>
        [DisplayName("Coordinate System")]
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// The type of extent for this spatial context
        /// </summary>
        [DisplayName("Extent Type")]
        public SpatialContextExtentType ExtentType { get; set; }

        /// <summary>
        /// The FGF geometry text that defines this spatial context's extent.
        /// </summary>
        [DisplayName("Extent Geometry")]
        public string ExtentGeometryText { get; set; }

        /// <summary>
        /// The WKT of this spatial context's coordinate system
        /// </summary>
        [DisplayName("Coordinate System WKT")]
        public string CoordinateSystemWkt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialContextInfo"/> class.
        /// </summary>
        public SpatialContextInfo() { }

        internal SpatialContextInfo(ISpatialContextReader reader)
        {
            this.CoordinateSystem = reader.GetCoordinateSystem();
            this.CoordinateSystemWkt = reader.GetCoordinateSystemWkt();
            this.Description = reader.GetDescription();
            this.ExtentType = reader.GetExtentType();
            this.Name = reader.GetName();
            this.XYTolerance = reader.GetXYTolerance();
            this.ZTolerance = reader.GetZTolerance();
            this.IsActive = reader.IsActive();
            try
            {
                byte[] bGeom = reader.GetExtent();
                if (bGeom != null)
                {
                    using (FgfGeometryFactory factory = new FgfGeometryFactory())
                    using (IGeometry geom = factory.CreateGeometryFromFgf(bGeom))
                    {
                        this.ExtentGeometryText = geom.Text;
                    }
                }
            }
            catch
            {
                this.ExtentGeometryText = null;
            }
        }

        internal SpatialContextInfo Clone()
        {
            var sc = new SpatialContextInfo
            {
                CoordinateSystem = this.CoordinateSystem,
                CoordinateSystemWkt = this.CoordinateSystemWkt,
                Description = this.Description,
                ExtentGeometryText = this.ExtentGeometryText,
                ExtentType = this.ExtentType,
                IsActive = this.IsActive,
                Name = this.Name,
                XYTolerance = this.XYTolerance,
                ZTolerance = this.ZTolerance
            };

            return sc;
        }
    }
}
