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
using FdoToolbox.Core.Feature;
using FdoToolbox.Core;
using SharpMap.Converters.WellKnownBinary;
using FdoToolbox.Core.Feature.RTree;
using Sm = SharpMap.Geometries;
using Fdo = OSGeo.FDO.Geometry;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Base.SharpMapProvider
{
    /// <summary>
    /// Utility class to perform common conversion operations
    /// </summary>
    public sealed class Converter
    {
        /// <summary>
        /// Creates an FDO envelope from a SharpMap bounding box
        /// </summary>
        /// <param name="bbox">The SharpMap bounding box.</param>
        /// <returns></returns>
        public static Fdo.IEnvelope EnvelopeFromBoundingBox(Sm.BoundingBox bbox)
        {
            return FdoGeometryFactory.Instance.CreateEnvelopeXY(bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);
        }

        /// <summary>
        /// Creates the polygon from an envelope.
        /// </summary>
        /// <param name="env">The envelope.</param>
        /// <returns></returns>
        public static Fdo.IPolygon CreatePolygonFromEnvelope(Fdo.IEnvelope env)
        {
            FdoGeometryFactory fact = FdoGeometryFactory.Instance;
            Fdo.IGeometry geom = fact.CreateGeometry(env);
            return (Fdo.IPolygon)geom;
        }

        /// <summary>
        /// Converts an FDO Geometry to a SharpMap geometry
        /// </summary>
        /// <param name="geom">The FDO geometry</param>
        /// <returns></returns>
        public static Sm.Geometry FromFdoGeometry(FdoGeometry geom, OSGeo.FDO.Geometry.FgfGeometryFactory geomFactory)
        {
            if (FdoGeometryUtil.Is2D(geom.InternalInstance))
            {
                //Get the WKB form of the geometry
                byte[] wkb = FdoGeometryFactory.Instance.GetWkb(geom.InternalInstance);
                return GeometryFromWKB.Parse(wkb);
            }
            else
            {
                using (OSGeo.FDO.Geometry.IGeometry flattened = FdoGeometryUtil.Flatten(geom.InternalInstance, geomFactory))
                {
                    //Get the WKB form of the geometry
                    byte[] wkb = FdoGeometryFactory.Instance.GetWkb(flattened);
                    return GeometryFromWKB.Parse(wkb);
                }
            }
        }

        /// <summary>
        /// Converts an SharpMap Geometry to a FDO geometry
        /// </summary>
        /// <param name="geom">The SharpMap geometry</param>
        /// <returns></returns>
        public static FdoGeometry ToFdoGeometry(Sm.Geometry geom)
        {
            //Get the WKB form of the geometry
            byte[] wkb = GeometryToWKB.Write(geom);
            Fdo.IGeometry fg = FdoGeometryFactory.Instance.CreateGeometryFromWkb(wkb);
            return new FdoGeometry(fg);
        }

        /// <summary>
        /// Creates an R-Tree query rectangle from an FDO envelope
        /// </summary>
        /// <param name="env">The env.</param>
        /// <returns></returns>
        public static Rectangle RectFromEnvelope(Fdo.IEnvelope env)
        {
            return new Rectangle((float)env.MinX, (float)env.MinY, (float)env.MaxX, (float)env.MaxY, (float)env.MinZ, (float)env.MaxZ);
        }

        /// <summary>
        /// Creates an R-Tree query rectangle from a SharpMap bounding box
        /// </summary>
        /// <param name="bbox">The SharpMap bounding box</param>
        /// <returns></returns>
        public static Rectangle RectFromBoundingBox(Sm.BoundingBox bbox)
        {
            return new Rectangle((float)bbox.Left, (float)bbox.Bottom, (float)bbox.Right, (float)bbox.Top, (float)0.0, (float)0.0);
        }
    }
}
