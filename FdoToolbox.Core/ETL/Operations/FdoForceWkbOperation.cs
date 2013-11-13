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
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// A pipeline operation that forces all input geometries to be WKB compliant
    /// </summary>
    public class FdoForceWkbOperation : FdoOperationBase
    {
        private FgfGeometryFactory _geomFactory = new FgfGeometryFactory();

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoRow row in rows)
            {
                yield return MakeWkbCompliant(row);
            }
        }

        private FdoRow MakeWkbCompliant(FdoRow row)
        {
            //Not applicable?
            if (row.Geometry == null)
                return row;

            //Already 2D? Move along!
            //if (FdoGeometryUtil.Is2D(row.Geometry))
            //    return row;

            IGeometry geom = row.Geometry;
            IGeometry trans = FdoGeometryUtil.ForceWkb(geom, _geomFactory);

            //Dispose the original if ForceWkb returned a new instance
            if (!IGeometry.ReferenceEquals(geom, trans))
            {
                row.Geometry = trans;
                geom.Dispose();
            }

            return row;
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _geomFactory.Dispose();

            base.Dispose(disposing);
        }
    }
}
