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
using System.IO;
using OSGeo.FDO.Common;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Geometry;
//
// FGF to WKT conversion code copied from:
// 
// http://trac.osgeo.org/fdo/browser/trunk/Providers/OGR/OgrFdoUtil.cpp
//
namespace FdoToolbox.Core.Utility
{
    /// <summary>
    /// Utility class to convert fgf geometries to other forms
    /// and vice versa.
    /// </summary>
    public sealed class FdoGeometryUtil
    {
        /// <summary>
        /// FDO bitmask value for X/Y dimensionality
        /// </summary>
        public const int FDO_DIM_XY = 0;
        /// <summary>
        /// FDO bitmask value for Z dimensionality
        /// </summary>
        public const int FDO_DIM_Z = 1;
        /// <summary>
        /// FDO bitmask value for M dimensionality
        /// </summary>
        public const int FDO_DIM_M = 2;

        private FdoGeometryUtil() { }

        /// <summary>
        /// Forces the given geometry to be WKB compliant. If necessary, a <see cref="Flatten"/> call
        /// will be made on the geometry.
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="fact"></param>
        /// <returns></returns>
        public static IGeometry ForceWkb(IGeometry geom, FgfGeometryFactory fact)
        {
            byte [] wkb = null;
            try
            {
                wkb = fact.GetWkb(geom);
            }
            catch
            {
                using (IGeometry wg = Flatten(geom, fact))
                {
                    wkb = fact.GetWkb(geom);
                }
            }
            return fact.CreateGeometryFromWkb(wkb);
        }

        /// <summary>
        /// Gets the geometric types.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static GeometricType[] GetGeometricTypes(int value)
        {
            if (value == (int)GeometricType.GeometricType_All)
            {
                return (GeometricType[])Enum.GetValues(typeof(GeometricType));
            }
            else
            {
                List<GeometricType> gts = new List<GeometricType>();
                GeometricType[] gtypes = (GeometricType[])Enum.GetValues(typeof(GeometricType));
                foreach (GeometricType gtype in gtypes)
                {
                    if ((value & (int)gtype) == (int)gtype)
                        gts.Add(gtype);
                }
                return gts.ToArray();
            }
        }

        /// <summary>
        /// Gets the geometry types.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static GeometryType[] GetGeometryTypes(int value)
        {
            List<GeometryType> gts = new List<GeometryType>();
            GeometryType[] gtypes = (GeometryType[])Enum.GetValues(typeof(GeometryType));
            foreach (GeometryType gtype in gtypes)
            {
                if ((value & (int)gtype) == (int)gtype)
                    gts.Add(gtype);
            }
            return gts.ToArray();
        }

        /// <summary>
        /// Converts a FGF binary to a WKB binary
        /// </summary>
        /// <param name="fgf"></param>
        /// <returns></returns>
        public static byte[] Fgf2Wkb(byte[] fgf)
        {
            byte[] wkb = null;
            using (MemoryStream fgfStream = new MemoryStream(fgf))
            using (MemoryStream wkbStream = new MemoryStream())
            {
                using (BinaryReader reader = new BinaryReader(fgfStream))
                using (BinaryWriter writer = new BinaryWriter(wkbStream))
                {
                    writer.Write((byte)1); //NDR

                    //Geometry type
                    int geom_type = reader.ReadInt32();
                    writer.Write(geom_type);

                    bool isMulti = ((geom_type == (int)GeometryType.GeometryType_MultiLineString)
                                || (geom_type == (int)GeometryType.GeometryType_MultiPolygon)
                                || (geom_type == (int)GeometryType.GeometryType_MultiPoint));

                    int num_geoms = 1;

                    //in case of multipolygon or multilinestring or multipoint,
                    //read poly or linestring count
                    if (isMulti)
                    {
                        num_geoms = reader.ReadInt32();
                        writer.Write(num_geoms);
                    }

                    for (int i = 0; i < num_geoms; i++)
                    {
                        if (isMulti)
                        {
                            //set byte order
                            writer.Write((byte)1);

                            //geom type
                            geom_type = reader.ReadInt32();
                            writer.Write(geom_type);
                        }

                        //read coordinate type
                        int dim = reader.ReadInt32();
                        bool skip = ((dim & FDO_DIM_Z) != 0);

                        if (skip)
                        {
                            //roll back and set the geom_type to wkb 2.5D
                            long currPos = writer.BaseStream.Position;
                            writer.Seek(1, SeekOrigin.Begin);
                            writer.Write((int)(geom_type | 0x80000000));
                            writer.Seek((int)currPos, SeekOrigin.Begin);
                        }

                        // the number of contours in current polygon/linestring
                        int contour_count = 1; //for linestrings

                        if ((geom_type == (int)GeometryType.GeometryType_Polygon)
                         || (geom_type == (int)GeometryType.GeometryType_MultiPolygon))
                        {
                            contour_count = reader.ReadInt32();
                            writer.Write(contour_count);
                        }

                        for (int j = 0; j < contour_count; j++)
                        {
                            int point_count = 1;

                            //point geoms do not have a point count, since
                            //each piece is just one point each
                            if ((geom_type != (int)GeometryType.GeometryType_MultiPoint)
                             && (geom_type != (int)GeometryType.GeometryType_Point))
                            {
                                point_count = reader.ReadInt32();
                                writer.Write(point_count);
                            }

                            int numd = point_count * ((skip ? 1 : 0) + 2);
                            for (int k = 0; k < numd; k++)
                            {
                                double dv = reader.ReadDouble();
                                writer.Write(dv);
                            }
                        }
                    }
                    wkb = wkbStream.ToArray();
                }
            }
            return wkb;
        }

        /// <summary>
        /// Gets the FGF text form of an FGF binary
        /// </summary>
        /// <param name="fgf"></param>
        /// <returns></returns>
        public static string GetFgfText(byte[] fgf)
        {
            StringBuilder wkt = new StringBuilder();
            using (MemoryStream fgfStream = new MemoryStream(fgf))
            {
                using (BinaryReader reader = new BinaryReader(fgfStream))
                {
                    WriteFgfText(wkt, reader);
                }
            }
            return wkt.ToString();
        }

        private static void WriteFgfText(StringBuilder wkt, BinaryReader reader)
        {
            int geom_type = reader.ReadInt32();

            GeometryType gtype = (GeometryType)Enum.Parse(typeof(GeometryType), geom_type.ToString());

            switch (gtype)
            {
                case GeometryType.GeometryType_CurvePolygon:
                    WriteSingleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_CurveString:
                    WriteSingleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_LineString:
                    WriteSingleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_Point:
                    WriteSingleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_Polygon:
                    WriteSingleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_MultiCurvePolygon:
                    WriteMultipleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_MultiCurveString:
                    WriteMultipleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_MultiGeometry:
                    WriteMultipleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_MultiLineString:
                    WriteMultipleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_MultiPoint:
                    WriteMultipleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_MultiPolygon:
                    WriteMultipleGeometryType(reader, wkt, gtype);
                    break;
                case GeometryType.GeometryType_None:
                    break;
            }
        }

        private static void WriteSingleGeometryType(BinaryReader reader, StringBuilder wkt, GeometryType type)
        {
            int dim = reader.ReadInt32();
            switch (type)
            {
                case GeometryType.GeometryType_CurvePolygon:
                    {
                        WriteCurvePolygonText(reader, wkt, dim);
                    }
                    break;
                case GeometryType.GeometryType_CurveString:
                    {
                        WriteCurveStringText(reader, wkt, dim);
                    }
                    break;
                case GeometryType.GeometryType_LineString:
                    {
                        WriteLineStringText(reader, wkt, dim);
                    }
                    break;
                case GeometryType.GeometryType_Point:
                    {
                        WritePointText(reader, wkt, dim);
                    }
                    break;
                case GeometryType.GeometryType_Polygon:
                    {
                        WritePolygonText(reader, wkt, dim);
                    }
                    break;
            }
        }

        private static void WritePointText(BinaryReader reader, StringBuilder wkt, int dim)
        {
            wkt.AppendFormat("POINT {0}(", GetDimensionality(dim));
            WritePointEntity(reader, wkt, dim);
            wkt.Append(")");
        }

        private static void WriteLineStringText(BinaryReader reader, StringBuilder wkt, int dim)
        {
            wkt.AppendFormat("LINESTRING {0}", GetDimensionality(dim));
            WriteLineStringEntity(reader, wkt, dim);
        }

        private static void WritePolygonText(BinaryReader reader, StringBuilder wkt, int dim)
        {
            wkt.AppendFormat("POLYGON {0}(", GetDimensionality(dim));
            WritePolygonEntity(reader, wkt, dim);
            wkt.Append(")");
        }

        private static void WriteCurveStringText(BinaryReader reader, StringBuilder wkt, int dim)
        {
            wkt.AppendFormat("CURVESTRING {0}(", GetDimensionality(dim));
            WriteCurveStringEntity(reader, wkt, dim);
            wkt.Append(")");
        }

        private static void WriteCurvePolygonText(BinaryReader reader, StringBuilder wkt, int dim)
        {
            wkt.AppendFormat("CURVEPOLYGON {0}(", GetDimensionality(dim));
            WriteCurvePolygonEntity(reader, wkt, dim);
            wkt.Append(")");
        }

        private static void WriteCurveSegmentText(BinaryReader reader, StringBuilder wkt, int dim)
        {
            int type = reader.ReadInt32();
            if (type == (int)GeometryComponentType.GeometryComponentType_CircularArcSegment)
            {
                wkt.Append("CIRCULARARCSEGMENT (");
                WriteCircularArcSegment(reader, wkt, dim);
                wkt.Append(")");
            }
            else if (type == (int)GeometryComponentType.GeometryComponentType_LineStringSegment)
            {
                wkt.Append("LINESTRINGSEGMENT (");
                WriteLineStringSegment(reader, wkt, dim);
                wkt.Append(")");
            }
        }

        private static void WriteMultipleGeometryType(BinaryReader reader, StringBuilder wkt, GeometryType type)
        {
            switch (type)
            {
                case GeometryType.GeometryType_MultiCurvePolygon:
                    {
                        int dim = GetFirstDimensionality(reader);
                        wkt.AppendFormat("MULTICURVEPOLYGON {0}(", GetDimensionality(dim));
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            int t = reader.ReadInt32();
                            dim = reader.ReadInt32();
                            if (i == 0)
                            {
                                wkt.Append("(");
                                WriteCurvePolygonEntity(reader, wkt, dim);
                                wkt.Append(")");
                            }
                            else
                            {
                                wkt.Append(", (");
                                WriteCurvePolygonEntity(reader, wkt, dim);
                                wkt.Append(")");
                            }
                        }
                        wkt.Append(")");
                    }
                    break;
                case GeometryType.GeometryType_MultiCurveString:
                    {
                        int dim = GetFirstDimensionality(reader);
                        wkt.AppendFormat("MULTICURVESTRING {0}(", GetDimensionality(dim));
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            int t = reader.ReadInt32();
                            dim = reader.ReadInt32();
                            if (i == 0)
                            {
                                wkt.Append("(");
                                WriteCurveStringEntity(reader, wkt, dim);
                                wkt.Append(")");
                            }
                            else
                            {
                                wkt.Append(", (");
                                WriteCurveStringEntity(reader, wkt, dim);
                                wkt.Append(")");
                            }
                        }
                        wkt.Append(")");
                    }
                    break;
                case GeometryType.GeometryType_MultiGeometry:
                    {
                        int dim = GetFirstDimensionality(reader);
                        wkt.AppendFormat("GEOMETRYCOLLECTION {0}(", GetDimensionality(dim));
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            int t = reader.ReadInt32();
                            GeometryType gt = (GeometryType)Enum.Parse(typeof(GeometryType), t.ToString());
                            if (i == 0)
                            {
                                WriteSingleGeometryType(reader, wkt, gt);
                            }
                            else
                            {
                                wkt.Append(", ");
                                WriteSingleGeometryType(reader, wkt, gt);
                            }
                        }
                        wkt.Append(")");
                    }
                    break;
                case GeometryType.GeometryType_MultiLineString:
                    {
                        int dim = GetFirstDimensionality(reader);
                        wkt.AppendFormat("MULTILINESTRING {0}(", GetDimensionality(dim));
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            int t = reader.ReadInt32();
                            dim = reader.ReadInt32();
                            if (i == 0)
                            {
                                WriteLineStringEntity(reader, wkt, dim);
                            }
                            else
                            {
                                wkt.Append(", ");
                                WriteLineStringEntity(reader, wkt, dim);
                            }
                        }
                        wkt.Append(")");
                    }
                    break;
                case GeometryType.GeometryType_MultiPoint:
                    {
                        int dim = GetFirstDimensionality(reader);
                        wkt.AppendFormat("MULTIPOINT {0}(", GetDimensionality(dim));
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            int t = reader.ReadInt32();
                            dim = reader.ReadInt32();
                            if (i == 0)
                            {
                                WritePointEntity(reader, wkt, dim);
                            }
                            else
                            {
                                wkt.Append(", ");
                                WritePointEntity(reader, wkt, dim);
                            }
                        }
                        wkt.Append(")");
                    }
                    break;
                case GeometryType.GeometryType_MultiPolygon:
                    {
                        int dim = GetFirstDimensionality(reader);
                        wkt.AppendFormat("MULTIPOLYGON {0}(", GetDimensionality(dim));
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            int t = reader.ReadInt32();
                            dim = reader.ReadInt32();
                            if (i == 0)
                            {
                                wkt.Append("(");
                                WritePolygonEntity(reader, wkt, dim);
                                wkt.Append(")");
                            }
                            else
                            {
                                wkt.Append(", (");
                                WritePolygonEntity(reader, wkt, dim);
                                wkt.Append(")");
                            }
                        }
                        wkt.Append(")");
                    }
                    break;
            }
        }

        private static int GetFirstDimensionality(BinaryReader reader)
        {
            long position = reader.BaseStream.Position;
            //We are 1 int into the stream, skip the next 2 ints
            reader.BaseStream.Seek(sizeof(int) * 3, SeekOrigin.Begin);
            int dim = reader.ReadInt32();
            //Now go back to original position
            reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return dim;
        }

        private static void WriteCircularArcSegment(BinaryReader reader, StringBuilder wkt, int dim)
        {
            WritePointEntity(reader, wkt, dim);
            wkt.Append(", ");
            WritePointEntity(reader, wkt, dim);
        }

        private static void WriteLineStringSegment(BinaryReader reader, StringBuilder wkt, int dim)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    WritePointEntity(reader, wkt, dim);
                }
                else
                {
                    wkt.Append(", ");
                    WritePointEntity(reader, wkt, dim);
                }
            }
        }

        private static void WriteCurvePolygonEntity(BinaryReader reader, StringBuilder wkt, int dim)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    wkt.Append("(");
                    WriteCurveStringEntity(reader, wkt, dim);
                    wkt.Append(")");
                }
                else
                {
                    wkt.Append(", (");
                    WriteCurveStringEntity(reader, wkt, dim);
                    wkt.Append(")");
                }
            }
        }

        private static void WriteCurveStringEntity(BinaryReader reader, StringBuilder wkt, int dim)
        {
            WritePointEntity(reader, wkt, dim);
            int count = reader.ReadInt32();
            wkt.Append(" (");
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    WriteCurveSegmentText(reader, wkt, dim);
                }
                else
                {
                    wkt.Append(", ");
                    WriteCurveSegmentText(reader, wkt, dim);
                }
            }
            wkt.Append(")");
        }

        private static void WritePolygonEntity(BinaryReader reader, StringBuilder wkt, int dim)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    WriteLineStringEntity(reader, wkt, dim);
                }
                else
                {
                    wkt.Append(", ");
                    WriteLineStringEntity(reader, wkt, dim);
                }
            }
        }

        private static void WriteLineStringEntity(BinaryReader reader, StringBuilder wkt, int dim)
        {
            wkt.Append("(");
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    WritePointEntity(reader, wkt, dim);
                }
                else
                {
                    wkt.Append(", ");
                    WritePointEntity(reader, wkt, dim);
                }
            }
            wkt.Append(")");
        }

        private static void WritePointEntity(BinaryReader reader, StringBuilder wkt, int dim)
        {
            if (dim == FDO_DIM_XY)
            {
                double x = reader.ReadDouble();
                double y = reader.ReadDouble();
                wkt.AppendFormat("{0} {1}", x, y);
            }
            else if (dim == (FDO_DIM_XY | FDO_DIM_Z))
            {
                double x = reader.ReadDouble();
                double y = reader.ReadDouble();
                double z = reader.ReadDouble();
                wkt.AppendFormat("{0} {1} {2}", x, y, z);
            }
            else if (dim == (FDO_DIM_XY | FDO_DIM_M))
            {
                double x = reader.ReadDouble();
                double y = reader.ReadDouble();
                double m = reader.ReadDouble();
                wkt.AppendFormat("{0} {1} {2}", x, y, m);
            }
            else if (dim == (FDO_DIM_XY | FDO_DIM_Z | FDO_DIM_M))
            {
                double x = reader.ReadDouble();
                double y = reader.ReadDouble();
                double z = reader.ReadDouble();
                double m = reader.ReadDouble();
                wkt.AppendFormat("{0} {1} {2} {3}", x, y, z, m);
            }
            else
                throw new System.Exception("Unknown dimensionality");
        }

        private static string GetDimensionality(int dim)
        {
            if (dim == FDO_DIM_XY)
                return "";
            else if (dim == (FDO_DIM_XY | FDO_DIM_Z))
                return "XYZ ";
            else if (dim == (FDO_DIM_XY | FDO_DIM_M))
                return "XYM ";
            else if (dim == (FDO_DIM_XY | FDO_DIM_Z | FDO_DIM_M))
                return "XYZM ";
            else
                throw new System.Exception("Unknown dimensionality type");
        }

        /// <summary>
        /// Gets the WKB text from the given FGF binary
        /// </summary>
        /// <param name="fgf"></param>
        /// <returns></returns>
        public static string GetWkbText(byte[] fgf)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the FGF binary from the given text
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetFgf(string str)
        {
            byte[] fgf = null;
            using (FgfGeometryFactory factory = new FgfGeometryFactory())
            {
                using (IGeometry geom = factory.CreateGeometry(str))
                {
                    fgf = factory.GetFgf(geom);
                }
            }
            return fgf;
        }

        /// <summary>
        /// Determines if this geometry is 2 dimensional (ie. It has no Z or M components)
        /// </summary>
        /// <param name="geom">The geometry</param>
        /// <returns></returns>
        public static bool Is2D(IGeometry geom)
        {
            return (((geom.Dimensionality & FDO_DIM_M) == 0) && ((geom.Dimensionality & FDO_DIM_Z) == 0));
        }

        /// <summary>
        /// Flattens the specified geometry by stripping out the Z and M components of all ordinates
        /// within the geometry.
        /// </summary>
        /// <param name="geom">The geometry to flatten</param>
        /// <param name="factory">The geometry factory.</param>
        /// <returns>The geometry with all Z and M ordinates stripped out. If the input geometry is already 2D, then it is returned as is.</returns>
        public static IGeometry Flatten(IGeometry geom, FgfGeometryFactory factory)
        {
            int dim = geom.Dimensionality;

            //Has no Z or M ordinates, so return geometry as is.
            if (Is2D(geom))
                return geom;

            switch (geom.DerivedType)
            {
                case OSGeo.FDO.Common.GeometryType.GeometryType_CurvePolygon:
                    return FlattenCurvePolygon((ICurvePolygon)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_CurveString:
                    return FlattenCurveString((ICurveString)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_LineString:
                    return FlattenLineString((ILineString)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_MultiCurvePolygon:
                    return FlattenMultiCurvePolygon((IMultiCurvePolygon)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_MultiCurveString:
                    return FlattenMultiCurveString((IMultiCurveString)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_MultiGeometry:
                    return FlattenMultiGeometry((IMultiGeometry)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_MultiLineString:
                    return FlattenMultiLineString((IMultiLineString)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_MultiPoint:
                    return FlattenMultiPoint((IMultiPoint)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_MultiPolygon:
                    return FlattenMultiPolygon((IMultiPolygon)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_Point:
                    return FlattenPoint((IPoint)geom, factory);
                case OSGeo.FDO.Common.GeometryType.GeometryType_Polygon:
                    return FlattenPolygon((IPolygon)geom, factory);
            }
            return null;
        }

        private static ILinearRing FlattenLinearRing(ILinearRing ring, FgfGeometryFactory factory)
        {
            //Copy exterior ring
            double[] flatRingPoints = new double[ring.Count * 2];
            int i = 0;
            foreach (IDirectPosition pos in ring.Positions)
            {
                flatRingPoints[i] = pos.X;
                flatRingPoints[i + 1] = pos.Y;
                i += 2;
            }

            ILinearRing flatRing = factory.CreateLinearRing(FDO_DIM_XY, flatRingPoints.Length, flatRingPoints);
            return flatRing;
        }

        private static IPolygon FlattenPolygon(IPolygon polygon, FgfGeometryFactory factory)
        {
            ILinearRing extRing = FlattenLinearRing(polygon.ExteriorRing, factory);   

            //Copy interior rings
            LinearRingCollection intRings = new LinearRingCollection();
            for (int j = 0; j < polygon.InteriorRingCount; j++)
            {
                ILinearRing ring = polygon.GetInteriorRing(j);
                intRings.Add(FlattenLinearRing(ring, factory));
            }

            IPolygon flatPolygon = factory.CreatePolygon(extRing, intRings);
            return flatPolygon;
        }

        private static IPoint FlattenPoint(IPoint point, FgfGeometryFactory factory)
        {
            return factory.CreatePoint(FDO_DIM_XY, new double[] { point.Position.X, point.Position.Y });
        }

        private static IMultiPolygon FlattenMultiPolygon(IMultiPolygon mpoly, FgfGeometryFactory factory)
        {
            PolygonCollection polys = new PolygonCollection();
            for (int i = 0; i < mpoly.Count; i++)
            {
                polys.Add(FlattenPolygon(mpoly[i], factory));
            }
            return factory.CreateMultiPolygon(polys);
        }

        private static IMultiPoint FlattenMultiPoint(IMultiPoint mpoint, FgfGeometryFactory factory)
        {
            PointCollection points = new PointCollection();
            for (int i = 0; i < mpoint.Count; i++)
            {
                points.Add(FlattenPoint(mpoint[i], factory));
            }
            return factory.CreateMultiPoint(points);
        }

        private static IMultiLineString FlattenMultiLineString(IMultiLineString mlString, FgfGeometryFactory factory)
        {
            LineStringCollection lineStrings = new LineStringCollection();
            for (int i = 0; i < mlString.Count; i++)
            {
                lineStrings.Add(FlattenLineString(mlString[i], factory));
            }
            return factory.CreateMultiLineString(lineStrings);
        }

        private static IMultiGeometry FlattenMultiGeometry(IMultiGeometry mgeom, FgfGeometryFactory factory)
        {
            GeometryCollection geometries = new GeometryCollection();
            for (int i = 0; i < mgeom.Count; i++)
            {
                geometries.Add(Flatten(mgeom[i], factory));
            }
            return factory.CreateMultiGeometry(geometries);
        }

        private static IMultiCurveString FlattenMultiCurveString(IMultiCurveString mcString, FgfGeometryFactory factory)
        {
            CurveStringCollection curveStrings = new CurveStringCollection();
            for (int i = 0; i < mcString.Count; i++)
            {
                curveStrings.Add(FlattenCurveString(mcString[i], factory));
            }
            return factory.CreateMultiCurveString(curveStrings);
        }

        private static IMultiCurvePolygon FlattenMultiCurvePolygon(IMultiCurvePolygon mcPolygon, FgfGeometryFactory factory)
        {
            CurvePolygonCollection curvePolygons = new CurvePolygonCollection();
            for (int i = 0; i < mcPolygon.Count; i++)
            {
                curvePolygons.Add(FlattenCurvePolygon(mcPolygon[i], factory));
            }
            return factory.CreateMultiCurvePolygon(curvePolygons);
        }

        private static ILineString FlattenLineString(ILineString lineStr, FgfGeometryFactory factory)
        {
            double[] lineStrOrdinates = new double[lineStr.Count * 2];
            int i = 0;
            foreach (IDirectPosition pos in lineStr.Positions)
            {
                lineStrOrdinates[i] = pos.X;
                lineStrOrdinates[i + 1] = pos.Y;
                i += 2;
            }

            return factory.CreateLineString(FDO_DIM_XY, lineStrOrdinates.Length, lineStrOrdinates);
        }

        private static ICurveString FlattenCurveString(ICurveString curveStr, FgfGeometryFactory factory)
        {
            CurveSegmentCollection curveSegs = new CurveSegmentCollection();
            for (int i = 0; i < curveStr.Count; i++)
            {
                curveSegs.Add(FlattenCurveSegment(curveStr[i], factory));
            }
            return factory.CreateCurveString(curveSegs);
        }

        private static ICurveSegmentAbstract FlattenCurveSegment(ICurveSegmentAbstract curveSegment, FgfGeometryFactory factory)
        {
            switch (curveSegment.DerivedType)
            {
                case OSGeo.FDO.Common.GeometryComponentType.GeometryComponentType_CircularArcSegment:
                    return FlattenCircularArcSegment((ICircularArcSegment)curveSegment, factory);
                case OSGeo.FDO.Common.GeometryComponentType.GeometryComponentType_LinearRing:
                    throw new NotSupportedException("Unable to flatten this segment type"); //Uh, ILinearRing doesn't inherit from ICurveSegmentAbstract 
                case OSGeo.FDO.Common.GeometryComponentType.GeometryComponentType_LineStringSegment:
                    return FlattenLineStringSegment((ILineStringSegment)curveSegment, factory);
                case OSGeo.FDO.Common.GeometryComponentType.GeometryComponentType_Ring:
                    throw new NotSupportedException("Unable to flatten this segment type"); //Uh, IRing doesn't inherit from ICurveSegmentAbstract 
            }
            throw new ArgumentException("Unknown curve segment type");
        }

        private static IDirectPosition FlattenPosition(IDirectPosition pos, FgfGeometryFactory factory)
        {
            return factory.CreatePositionXY(pos.X, pos.Y);
        }

        private static ICircularArcSegment FlattenCircularArcSegment(ICircularArcSegment circularArc, FgfGeometryFactory factory)
        {
            return factory.CreateCircularArcSegment(
                FlattenPosition(circularArc.StartPosition, factory),
                FlattenPosition(circularArc.MidPoint, factory),
                FlattenPosition(circularArc.EndPosition, factory));
        }

        private static ILineStringSegment FlattenLineStringSegment(ILineStringSegment lineStrSegment, FgfGeometryFactory factory)
        {
            double[] positions = new double[lineStrSegment.Count * 2];
            int j = 0;
            for (int i = 0; i < lineStrSegment.Count; i++)
            {
                IDirectPosition pos = lineStrSegment[i];
                positions[j] = pos.X;
                positions[j+1] = pos.Y;
                j += 2;
            }
            return factory.CreateLineStringSegment(FDO_DIM_XY, positions.Length, positions);
        }

        private static IRing FlattenRing(IRing ring, FgfGeometryFactory factory)
        {
            CurveSegmentCollection curves = new CurveSegmentCollection();
            foreach (ICurveSegmentAbstract curve in ring.CurveSegments)
            {
                curves.Add(FlattenCurveSegment(curve, factory));
            }
            return factory.CreateRing(curves);
        }

        private static ICurvePolygon FlattenCurvePolygon(ICurvePolygon curvePolygon, FgfGeometryFactory factory)
        {
            IRing extRing = FlattenRing(curvePolygon.ExteriorRing, factory);
            RingCollection intRings = new RingCollection();
            for (int i = 0; i < curvePolygon.InteriorRingCount; i++)
            {
                intRings.Add(FlattenRing(curvePolygon.get_InteriorRing(i), factory));
            }
            return factory.CreateCurvePolygon(extRing, intRings);
        }
    }
}
