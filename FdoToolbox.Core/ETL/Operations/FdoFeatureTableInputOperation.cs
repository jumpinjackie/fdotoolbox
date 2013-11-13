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
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// A <see cref="FdoFeatureTable"/> input source
    /// </summary>
    public class FdoFeatureTableInputOperation : FdoOperationBase
    {
        private FdoFeatureTable _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoFeatureTableInputOperation"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public FdoFeatureTableInputOperation(FdoFeatureTable table)
        {
            _table = table;
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoFeature feat in _table.Rows)
            {
                yield return FdoRow.FromFeatureRow(feat);
            }
        }
    }
}
