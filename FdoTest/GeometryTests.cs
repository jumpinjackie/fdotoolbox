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

using FdoToolbox.Core.CoordinateSystems.Transform;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;

namespace FdoTest
{
    internal class FlippingGeometryConverter : FdoGeometryConverter
    {
        public override void ConvertPosition(ref double x, ref double y)
        {
            double temp = x;
            x = y;
            y = temp;
        }

        public override void ConvertPosition(ref double x, ref double y, ref double z)
        {
            double temp = x;
            x = y;
            y = z;
            z = temp;
        }
    }

    public static class GeometryTests
    {
        public static void Test_GeometryConverterContract_Point()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("POINT (1 2)");
            var geom2 = geomFactory.CreateGeometry("POINT XYZ (1 2 3)");
            var geom3 = geomFactory.CreateGeometry("POINT XYM (1 2 3)");
            var geom4 = geomFactory.CreateGeometry("POINT XYZM (1 2 3 4)");

            var cGeom1 = converter.ConvertOrdinates(geom1);
            var cGeom2 = converter.ConvertOrdinates(geom2);
            var cGeom3 = converter.ConvertOrdinates(geom3);
            var cGeom4 = converter.ConvertOrdinates(geom4);

            var text1 = cGeom1.Text;
            var text2 = cGeom2.Text;
            var text3 = cGeom3.Text;
            var text4 = cGeom4.Text;

            Assert.Equal("POINT (2 1)", text1);
            Assert.Equal("POINT XYZ (2 3 1)", text2);
            Assert.Equal("POINT XYM (2 1 3)", text3);
            Assert.Equal("POINT XYZM (2 3 1 4)", text4);
        }

        public static void Test_GeometryConverterContract_LineString()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("LINESTRING (30 10, 10 30, 40 40)");
            var geom2 = geomFactory.CreateGeometry("LINESTRING XYZ (30 10 1, 10 30 2, 40 40 3)");
            var geom3 = geomFactory.CreateGeometry("LINESTRING XYM (30 10 1, 10 30 2, 40 40 3)");
            var geom4 = geomFactory.CreateGeometry("LINESTRING XYZM (30 10 1 2, 10 30 3 4, 40 40 5 6)");

            var cGeom1 = converter.ConvertOrdinates(geom1);
            var cGeom2 = converter.ConvertOrdinates(geom2);
            var cGeom3 = converter.ConvertOrdinates(geom3);
            var cGeom4 = converter.ConvertOrdinates(geom4);

            var text1 = cGeom1.Text;
            var text2 = cGeom2.Text;
            var text3 = cGeom3.Text;
            var text4 = cGeom4.Text;

            Assert.Equal("LINESTRING (10 30, 30 10, 40 40)", text1);
            Assert.Equal("LINESTRING XYZ (10 1 30, 30 2 10, 40 3 40)", text2);
            Assert.Equal("LINESTRING XYM (10 30 1, 30 10 2, 40 40 3)", text3);
            Assert.Equal("LINESTRING XYZM (10 1 30 2, 30 3 10 4, 40 5 40 6)", text4);
        }

        public static void Test_GeometryConverterContract_Polygon()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))");
            var geom2 = geomFactory.CreateGeometry("POLYGON XYZ ((30 10 1, 40 40 2, 20 40 3, 10 20 4, 30 10 5))");
            var geom3 = geomFactory.CreateGeometry("POLYGON XYM ((30 10 1, 40 40 2, 20 40 3, 10 20 4, 30 10 5))");
            var geom4 = geomFactory.CreateGeometry("POLYGON XYZM ((30 10 1 2, 40 40 3 4, 20 40 5 6, 10 20 7 8, 30 10 9 10))");

            var cGeom1 = converter.ConvertOrdinates(geom1);
            var cGeom2 = converter.ConvertOrdinates(geom2);
            var cGeom3 = converter.ConvertOrdinates(geom3);
            var cGeom4 = converter.ConvertOrdinates(geom4);

            var text1 = cGeom1.Text;
            var text2 = cGeom2.Text;
            var text3 = cGeom3.Text;
            var text4 = cGeom4.Text;

            Assert.Equal("POLYGON ((10 30, 40 40, 40 20, 20 10, 10 30))", text1);
            Assert.Equal("POLYGON XYZ ((10 1 30, 40 2 40, 40 3 20, 20 4 10, 10 5 30))", text2);
            Assert.Equal("POLYGON XYM ((10 30 1, 40 40 2, 40 20 3, 20 10 4, 10 30 5))", text3);
            Assert.Equal("POLYGON XYZM ((10 1 30 2, 40 3 40 4, 40 5 20 6, 20 7 10 8, 10 9 30 10))", text4);
        }

        public static void Test_GeometryConverterContract_PolygonWithHole()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("POLYGON ((35 10, 45 45, 15 40, 10 20, 35 10), (20 30, 35 35, 30 20, 20 30))");
            var geom2 = geomFactory.CreateGeometry("POLYGON XYZ ((35 10 1, 45 45 2, 15 40 3, 10 20 4, 35 10 5), (20 30 6, 35 35 7, 30 20 8, 20 30 9))");
            var geom3 = geomFactory.CreateGeometry("POLYGON XYM ((35 10 1, 45 45 2, 15 40 3, 10 20 4, 35 10 5), (20 30 6, 35 35 7, 30 20 8, 20 30 9))");
            var geom4 = geomFactory.CreateGeometry("POLYGON XYZM ((35 10 1 2, 45 45 3 4, 15 40 5 6, 10 20 7 8, 35 10 9 10), (20 30 11 12, 35 35 13 14, 30 20 15 16, 20 30 17 18))");

            var cGeom1 = converter.ConvertOrdinates(geom1);
            var cGeom2 = converter.ConvertOrdinates(geom2);
            var cGeom3 = converter.ConvertOrdinates(geom3);
            var cGeom4 = converter.ConvertOrdinates(geom4);

            var text1 = cGeom1.Text;
            var text2 = cGeom2.Text;
            var text3 = cGeom3.Text;
            var text4 = cGeom4.Text;

            Assert.Equal("POLYGON ((10 35, 45 45, 40 15, 20 10, 10 35), (30 20, 35 35, 20 30, 30 20))", text1);
            Assert.Equal("POLYGON XYZ ((10 1 35, 45 2 45, 40 3 15, 20 4 10, 10 5 35), (30 6 20, 35 7 35, 20 8 30, 30 9 20))", text2);
            Assert.Equal("POLYGON XYM ((10 35 1, 45 45 2, 40 15 3, 20 10 4, 10 35 5), (30 20 6, 35 35 7, 20 30 8, 30 20 9))", text3);
            Assert.Equal("POLYGON XYZM ((10 1 35 2, 45 3 45 4, 40 5 15 6, 20 7 10 8, 10 9 35 10), (30 11 20 12, 35 13 35 14, 20 15 30 16, 30 17 20 18))", text4);
        }

        public static void Test_GeometryConverterContract_MultiPoint()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("MULTIPOINT (10 40, 40 30, 20 20, 30 10)");
            var geom2 = geomFactory.CreateGeometry("MULTIPOINT XYZ (10 40 1, 40 30 2, 20 20 3, 30 10 4)");
            var geom3 = geomFactory.CreateGeometry("MULTIPOINT XYM (10 40 1, 40 30 2, 20 20 3, 30 10 4)");
            var geom4 = geomFactory.CreateGeometry("MULTIPOINT XYZM (10 40 1 2, 40 30 3 4, 20 20 5 6, 30 10 7 8)");

            var cGeom1 = converter.ConvertOrdinates(geom1);
            var cGeom2 = converter.ConvertOrdinates(geom2);
            var cGeom3 = converter.ConvertOrdinates(geom3);
            var cGeom4 = converter.ConvertOrdinates(geom4);

            var text1 = cGeom1.Text;
            var text2 = cGeom2.Text;
            var text3 = cGeom3.Text;
            var text4 = cGeom4.Text;

            Assert.Equal("MULTIPOINT (40 10, 30 40, 20 20, 10 30)", text1);
            Assert.Equal("MULTIPOINT XYZ (40 1 10, 30 2 40, 20 3 20, 10 4 30)", text2);
            Assert.Equal("MULTIPOINT XYM (40 10 1, 30 40 2, 20 20 3, 10 30 4)", text3);
            Assert.Equal("MULTIPOINT XYZM (40 1 10 2, 30 3 40 4, 20 5 20 6, 10 7 30 8)", text4);
        }

        public static void Test_GeometryConverterContract_MultiLineString()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))");
            var geom2 = geomFactory.CreateGeometry("MULTILINESTRING XYZ ((10 10 1, 20 20 2, 10 40 3), (40 40 4, 30 30 5, 40 20 6, 30 10 7))");
            var geom3 = geomFactory.CreateGeometry("MULTILINESTRING XYM ((10 10 1, 20 20 2, 10 40 3), (40 40 4, 30 30 5, 40 20 6, 30 10 7))");
            var geom4 = geomFactory.CreateGeometry("MULTILINESTRING XYZM ((10 10 1 2, 20 20 3 4, 10 40 5 6), (40 40 7 8, 30 30 9 10, 40 20 11 12, 30 10 13 14))");

            var cGeom1 = converter.ConvertOrdinates(geom1);
            var cGeom2 = converter.ConvertOrdinates(geom2);
            var cGeom3 = converter.ConvertOrdinates(geom3);
            var cGeom4 = converter.ConvertOrdinates(geom4);

            var text1 = cGeom1.Text;
            var text2 = cGeom2.Text;
            var text3 = cGeom3.Text;
            var text4 = cGeom4.Text;

            Assert.Equal("MULTILINESTRING ((10 10, 20 20, 40 10), (40 40, 30 30, 20 40, 10 30))", text1);
            Assert.Equal("MULTILINESTRING XYZ ((10 1 10, 20 2 20, 40 3 10), (40 4 40, 30 5 30, 20 6 40, 10 7 30))", text2);
            Assert.Equal("MULTILINESTRING XYM ((10 10 1, 20 20 2, 40 10 3), (40 40 4, 30 30 5, 20 40 6, 10 30 7))", text3);
            Assert.Equal("MULTILINESTRING XYZM ((10 1 10 2, 20 3 20 4, 40 5 10 6), (40 7 40 8, 30 9 30 10, 20 11 40 12, 10 13 30 14))", text4);
        }

        public static void Test_GeometryConverterContract_MultiPolygon()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))");
            var geom2 = geomFactory.CreateGeometry("MULTIPOLYGON XYZ (((30 20 1, 45 40 2, 10 40 3, 30 20 4)), ((15 5 5, 40 10 6, 10 20 7, 5 10 8, 15 5 9)))");
            var geom3 = geomFactory.CreateGeometry("MULTIPOLYGON XYM (((30 20 1, 45 40 2, 10 40 3, 30 20 4)), ((15 5 5, 40 10 6, 10 20 7, 5 10 8, 15 5 9)))");
            var geom4 = geomFactory.CreateGeometry("MULTIPOLYGON XYZM (((30 20 1 2, 45 40 3 4, 10 40 5 6, 30 20 7 8)), ((15 5 9 10, 40 10 11 12, 10 20 13 14, 5 10 15 16, 15 5 17 18)))");

            var cGeom1 = converter.ConvertOrdinates(geom1);
            var cGeom2 = converter.ConvertOrdinates(geom2);
            var cGeom3 = converter.ConvertOrdinates(geom3);
            var cGeom4 = converter.ConvertOrdinates(geom4);

            var text1 = cGeom1.Text;
            var text2 = cGeom2.Text;
            var text3 = cGeom3.Text;
            var text4 = cGeom4.Text;

            Assert.Equal("MULTIPOLYGON (((20 30, 40 45, 40 10, 20 30)), ((5 15, 10 40, 20 10, 10 5, 5 15)))", text1);
            Assert.Equal("MULTIPOLYGON XYZ (((20 1 30, 40 2 45, 40 3 10, 20 4 30)), ((5 5 15, 10 6 40, 20 7 10, 10 8 5, 5 9 15)))", text2);
            Assert.Equal("MULTIPOLYGON XYM (((20 30 1, 40 45 2, 40 10 3, 20 30 4)), ((5 15 5, 10 40 6, 20 10 7, 10 5 8, 5 15 9)))", text3);
            Assert.Equal("MULTIPOLYGON XYZM (((20 1 30 2, 40 3 45 4, 40 5 10 6, 20 7 30 8)), ((5 9 15 10, 10 11 40 12, 20 13 10 14, 10 15 5 16, 5 17 15 18)))", text4);
        }

        public static void Test_GeometryConverterContract_GeometryCollection()
        {
            var geomFactory = new FgfGeometryFactory();
            var converter = new FlippingGeometryConverter();

            var geom1 = geomFactory.CreateGeometry("GEOMETRYCOLLECTION (POINT (40 10), LINESTRING(10 10, 20 20, 10 40), POLYGON((40 40, 20 45, 45 30, 40 40)))");

            var cGeom1 = converter.ConvertOrdinates(geom1);

            var text1 = cGeom1.Text;

            Assert.Equal("GEOMETRYCOLLECTION (POINT (10 40), LINESTRING (10 10, 20 20, 40 10), POLYGON ((40 40, 45 20, 30 45, 40 40)))", text1);
        }
    }
}