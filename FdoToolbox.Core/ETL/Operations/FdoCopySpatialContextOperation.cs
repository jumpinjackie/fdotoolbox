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
using FdoToolbox.Core.ETL.Overrides;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// An ETL operation that copies spatial contexts from one data source to another
    /// </summary>
    public class FdoCopySpatialContextOperation : FdoSingleActionOperationBase
    {
        private FdoConnection _source;
        private FdoConnection _target;
        private string[] _scNames;
        private bool _overwrite;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoCopySpatialContextOperation"/> class.
        /// </summary>
        /// <param name="src">The source connection.</param>
        /// <param name="dst">The target connection.</param>
        /// <param name="sourceSpatialContextNames">The source spatial context names.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public FdoCopySpatialContextOperation(FdoConnection src, FdoConnection dst, string[] sourceSpatialContextNames, bool overwrite)
        {
            _source = src;
            _target = dst;
            _scNames = sourceSpatialContextNames;
            _overwrite = overwrite;
        }

        public override void ExecuteAction()
        {
            ICopySpatialContext copy = CopySpatialContextOverrideFactory.GetCopySpatialContextOverride(_target);
            copy.Execute(_source, _target, _overwrite, _scNames);
        }
    }
}
