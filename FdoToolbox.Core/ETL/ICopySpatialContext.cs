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

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Defines a method to override the default behaviour of copying spatial contexts
    /// across.
    /// </summary>
    public interface ICopySpatialContext
    {
        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="spatialContexts">The spatial contexts.</param>
        /// <param name="target">The target.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        void Execute(ICollection<SpatialContextInfo> spatialContexts, FdoConnection target, bool overwrite);

        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        void Execute(FdoConnection source, FdoConnection target, bool overwrite);

        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        void Execute(FdoConnection source, FdoConnection target, bool overwrite, string [] spatialContextNames);

        /// <summary>
        /// Copies the named spatial context
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will ovewrite the spatial context of the same name on the target connection</param>
        /// <param name="spatialContextName"></param>
        void Execute(FdoConnection source, FdoConnection target, bool overwrite, string spatialContextName);
    }
}
