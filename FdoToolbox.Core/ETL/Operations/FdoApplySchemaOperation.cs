#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Represents an operation that executes an FDO apply schema command
    /// </summary>
    public class FdoApplySchemaOperation : FdoSingleActionOperationBase
    {
        private FdoConnection _conn;
        private FeatureSchema _schema;

        public FdoApplySchemaOperation(FdoConnection conn, FeatureSchema schema)
        {
            _conn = conn;
            _schema = schema;
        }

        public override void ExecuteAction()
        {
            using (var svc = _conn.CreateFeatureService())
            {
                IncompatibleSchema schema;
                if (!svc.CanApplySchema(_schema, out schema))
                {
                    Info("Fixing incompatibilities in schema");
                    _schema = svc.AlterSchema(_schema, schema);
                }
                Info("Applying schema");
                svc.ApplySchema(_schema);
                Info("Schema applied");
            }
        }
    }
}
