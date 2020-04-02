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
using FdoToolbox.Core.Feature;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ETL.Overrides
{
    /// <summary>
    /// Copy spatial context override for MySQL target. 
    /// 
    /// MySQL requires that if an existing spatial context already exists 
    /// (by name) that it be destroyed, as ICreateSpatialContext::updateExisting 
    /// does not work
    /// 
    /// Also, an existing spatial context cannot be destroyed if there are
    /// Geometric Properties using that spatial context. In this case, we don't
    /// copy that context across
    /// </summary>
    public class MySqlCopySpatialContextOverride : CopySpatialContext
    {
        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="spatialContexts">The spatial contexts.</param>
        /// <param name="target">The target.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public override void Execute(ICollection<SpatialContextInfo> spatialContexts, FdoConnection target, bool overwrite)
        {
            //MySQL supports multiple spatial contexts and IDestorySpatialContext
            //so in this case if overwrite == true, we want to destroy any ones that
            //already exist in the target before creating new ones in its place. This does not
            //prevent creating a series of spatial contexts if overwrite == false and one of
            //the spatial contexts being copied already exists. This is an unfortunate leaky 
            //abstraction in the FDO API.

            using (FdoFeatureService service = target.CreateFeatureService())
            {
                if (overwrite)
                {
                    ReadOnlyCollection<SpatialContextInfo> targetContexts = service.GetSpatialContexts();

                    foreach (SpatialContextInfo sc in spatialContexts)
                    {
                        //Only destroy spatial context if it exists in target connection
                        if (SpatialContextExists(targetContexts, sc))
                            service.DestroySpatialContext(sc);
                    }
                }

                foreach (SpatialContextInfo sc in spatialContexts)
                {
                    service.CreateSpatialContext(sc, false);
                }
            }
        }

        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        public override void Execute(FdoConnection source, FdoConnection target, bool overwrite, string[] spatialContextNames)
        {
            if (spatialContextNames.Length == 0)
                return;

            FdoFeatureService srcService = source.CreateFeatureService();
            FdoFeatureService destService = target.CreateFeatureService();
            ReadOnlyCollection<SpatialContextInfo> srcContexts = srcService.GetSpatialContexts();
            ReadOnlyCollection<SpatialContextInfo> destContexts = destService.GetSpatialContexts();
            foreach (SpatialContextInfo ctx in srcContexts)
            {
                if (SpatialContextInSpecifiedList(ctx, spatialContextNames))
                {
                    try
                    {
                        //Find target spatial context of the same name
                        SpatialContextInfo sci = destService.GetSpatialContext(ctx.Name);
                        if (sci != null && overwrite)
                        {
                            //If found, destroy then create
                            destService.DestroySpatialContext(ctx.Name);
                            destService.CreateSpatialContext(ctx, false);
                        }
                        else
                        {
                            destService.CreateSpatialContext(ctx, false);
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
        }
    }
}
