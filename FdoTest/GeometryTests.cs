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
using OSGeo.FDO.Geometry;

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
    }
}
