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
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Globally accessible singleton geometry factory
    /// </summary>
    public sealed class FdoGeometryFactory : FgfGeometryFactory, IFdoGeometryFactory
    {
        private static FdoGeometryFactory _instance;

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static FdoGeometryFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FdoGeometryFactory();
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// FDO geometry factory interface
    /// </summary>
    public interface IFdoGeometryFactory
    {
        /// <summary>
        /// Creates the circular arc segment.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="midPosition">The mid position.</param>
        /// <param name="endPosition">The end position.</param>
        /// <returns></returns>
        ICircularArcSegment CreateCircularArcSegment(IDirectPosition startPosition, IDirectPosition midPosition, IDirectPosition endPosition);
        /// <summary>
        /// Creates the curve polygon.
        /// </summary>
        /// <param name="exteriorRing">The exterior ring.</param>
        /// <param name="interiorRings">The interior rings.</param>
        /// <returns></returns>
        ICurvePolygon CreateCurvePolygon(IRing exteriorRing, RingCollection interiorRings);
        /// <summary>
        /// Creates the curve string.
        /// </summary>
        /// <param name="curveSegments">The curve segments.</param>
        /// <returns></returns>
        ICurveString CreateCurveString(CurveSegmentCollection curveSegments);
        /// <summary>
        /// Creates the geometry.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <returns></returns>
        IGeometry CreateGeometry(IEnvelope envelope);
        /// <summary>
        /// Creates the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        IGeometry CreateGeometry(IGeometry geometry);
        /// <summary>
        /// Creates the geometry.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        IGeometry CreateGeometry(string text);
        /// <summary>
        /// Creates the geometry from FGF.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        IGeometry CreateGeometryFromFgf(byte[] bytes);
        /// <summary>
        /// Creates the geometry from FGF.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IGeometry CreateGeometryFromFgf(byte[] bytes, int count);
        /// <summary>
        /// Creates the geometry from WKB.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        IGeometry CreateGeometryFromWkb(byte[] bytes);
        /// <summary>
        /// Creates the linear ring.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <returns></returns>
        ILinearRing CreateLinearRing(DirectPositionCollection positions);
        /// <summary>
        /// Creates the linear ring.
        /// </summary>
        /// <param name="dimensionality">The dimensionality.</param>
        /// <param name="ordinateNumber">The ordinate number.</param>
        /// <param name="ordinates">The ordinates.</param>
        /// <returns></returns>
        ILinearRing CreateLinearRing(int dimensionality, int ordinateNumber, double[] ordinates);
        /// <summary>
        /// Creates the line string.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <returns></returns>
        ILineString CreateLineString(DirectPositionCollection positions);
        /// <summary>
        /// Creates the line string.
        /// </summary>
        /// <param name="dimensionType">Type of the dimension.</param>
        /// <param name="ordinateNumber">The ordinate number.</param>
        /// <param name="ordinates">The ordinates.</param>
        /// <returns></returns>
        ILineString CreateLineString(int dimensionType, int ordinateNumber, double[] ordinates);
        /// <summary>
        /// Creates the line string segment.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <returns></returns>
        ILineStringSegment CreateLineStringSegment(DirectPositionCollection positions);
        /// <summary>
        /// Creates the line string segment.
        /// </summary>
        /// <param name="dimType">Type of the dim.</param>
        /// <param name="ordinateNumber">The ordinate number.</param>
        /// <param name="ordinates">The ordinates.</param>
        /// <returns></returns>
        ILineStringSegment CreateLineStringSegment(int dimType, int ordinateNumber, double[] ordinates);
        /// <summary>
        /// Creates the multi curve polygon.
        /// </summary>
        /// <param name="curvePolygons">The curve polygons.</param>
        /// <returns></returns>
        IMultiCurvePolygon CreateMultiCurvePolygon(CurvePolygonCollection curvePolygons);
        /// <summary>
        /// Creates the multi curve string.
        /// </summary>
        /// <param name="curveStrings">The curve strings.</param>
        /// <returns></returns>
        IMultiCurveString CreateMultiCurveString(CurveStringCollection curveStrings);
        /// <summary>
        /// Creates the multi geometry.
        /// </summary>
        /// <param name="geometries">The geometries.</param>
        /// <returns></returns>
        IMultiGeometry CreateMultiGeometry(GeometryCollection geometries);
        /// <summary>
        /// Creates the multi line string.
        /// </summary>
        /// <param name="lineStrings">The line strings.</param>
        /// <returns></returns>
        IMultiLineString CreateMultiLineString(LineStringCollection lineStrings);
        /// <summary>
        /// Creates the multi point.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        IMultiPoint CreateMultiPoint(PointCollection points);
        /// <summary>
        /// Creates the multi point.
        /// </summary>
        /// <param name="dimensionality">The dimensionality.</param>
        /// <param name="ordinateNumber">The ordinate number.</param>
        /// <param name="ordinates">The ordinates.</param>
        /// <returns></returns>
        IMultiPoint CreateMultiPoint(int dimensionality, int ordinateNumber, double[] ordinates);
        /// <summary>
        /// Creates the multi polygon.
        /// </summary>
        /// <param name="polygons">The polygons.</param>
        /// <returns></returns>
        IMultiPolygon CreateMultiPolygon(PolygonCollection polygons);
        /// <summary>
        /// Creates the point.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        IPoint CreatePoint(IDirectPosition position);
        /// <summary>
        /// Creates the point.
        /// </summary>
        /// <param name="dimensionality">The dimensionality.</param>
        /// <param name="ordinates">The ordinates.</param>
        /// <returns></returns>
        IPoint CreatePoint(int dimensionality, double[] ordinates);
        /// <summary>
        /// Creates the polygon.
        /// </summary>
        /// <param name="exteriorRing">The exterior ring.</param>
        /// <param name="interiorRings">The interior rings.</param>
        /// <returns></returns>
        IPolygon CreatePolygon(ILinearRing exteriorRing, LinearRingCollection interiorRings);
        /// <summary>
        /// Creates the ring.
        /// </summary>
        /// <param name="curveSegments">The curve segments.</param>
        /// <returns></returns>
        IRing CreateRing(CurveSegmentCollection curveSegments);
        /// <summary>
        /// Gets the FGF.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        byte[] GetFgf(IGeometry geometry);
        /// <summary>
        /// Gets the WKB.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        byte[] GetWkb(IGeometry geometry);
    }
}
