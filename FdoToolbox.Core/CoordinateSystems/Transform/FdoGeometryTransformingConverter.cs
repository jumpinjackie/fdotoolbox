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
using OSGeo.MapGuide;

namespace FdoToolbox.Core.CoordinateSystems.Transform
{
    /// <summary>
    /// A <see cref="FdoGeometryConverter"/> that does actual coordinate transformation
    /// between 2 coordinate systems
    /// </summary>
    internal class FdoGeometryTransformingConverter : FdoGeometryConverter
    {
        private bool disposedValue = false; // To detect redundant calls
        readonly MgCoordinateSystemTransform _xform;

        internal FdoGeometryTransformingConverter(MgCoordinateSystemTransform xform)
        {
            _xform = xform;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _xform.Dispose();
                }
                disposedValue = true;
            }
            base.Dispose(disposing);
        }

        public override void ConvertPosition(ref double x, ref double y)
        {
            //*sigh*! All the transform APIs allocate. There is one API that
            //takes a MgCoordinate presumably to transform in-place, but the
            //MgCoordinate .net API is immutable! This is the only thing we have
            var pt = _xform.Transform(x, y);
            x = pt.X;
            y = pt.Y;
            pt.Dispose();
        }

        public override void ConvertPosition(ref double x, ref double y, ref double z)
        {
            //*sigh*! All the transform APIs allocate. There is one API that
            //takes a MgCoordinate presumably to transform in-place, but the
            //MgCoordinate .net API is immutable! This is the only thing we have
            var pt = _xform.Transform(x, y, z);
            x = pt.X;
            y = pt.Y;
            z = pt.Z;
            pt.Dispose();
        }
    }
}
