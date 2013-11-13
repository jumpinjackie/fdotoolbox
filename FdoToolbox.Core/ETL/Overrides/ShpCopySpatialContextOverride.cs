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
using System.Diagnostics;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.ETL.Overrides
{
    /// <summary>
    /// Copy spatial context override for SHP target. SHP requires
    /// that the Spatial Context Name is also the Coordinate System Name
    /// </summary>
    public class ShpCopySpatialContextOverride : CopySpatialContext
    {
        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        public override void Execute(FdoConnection source, FdoConnection target, bool overwrite, string [] spatialContextNames)
        {
            if (spatialContextNames.Length == 0)
                return;

            string srcName = spatialContextNames[0];
            FdoFeatureService srcService = source.CreateFeatureService();
            FdoFeatureService destService = target.CreateFeatureService();
            SpatialContextInfo context = srcService.GetSpatialContext(srcName);
            if (context != null)
            {
                //Make sure that CSName != Spatial Context Name
                WKTParser parser = new WKTParser(context.CoordinateSystemWkt);
                if (!string.IsNullOrEmpty(parser.CSName))
                {
                    context.CoordinateSystem = parser.CSName;
                    context.Name = parser.CSName;
                    destService.CreateSpatialContext(context, overwrite);
                }
            }
        }

        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="spatialContexts">The spatial contexts.</param>
        /// <param name="target">The target.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public override void Execute(ICollection<SpatialContextInfo> spatialContexts, FdoConnection target, bool overwrite)
        {
            List<SpatialContextInfo> contexts = new List<SpatialContextInfo>(spatialContexts);
            foreach (SpatialContextInfo sc in contexts)
            {
                //Make sure that CSName != Spatial Context Name
                WKTParser parser = new WKTParser(sc.CoordinateSystemWkt);
                if (!string.IsNullOrEmpty(parser.CSName))
                {
                    sc.CoordinateSystem = parser.CSName;
                    sc.Name = parser.CSName;
                }
            }
            base.Execute(contexts, target, overwrite);
        }
    }
}
