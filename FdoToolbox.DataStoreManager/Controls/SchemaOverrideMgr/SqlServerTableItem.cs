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
using Sql = OSGeo.FDO.Providers.Rdbms.Override.SQLServerSpatial;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class SqlServerTableItem : RdbmsTableItem<Sql.OvTable>
    {
        public SqlServerTableItem(Sql.OvTable table)
            : base(table)
        { }

        public SqlServerTableItem(string name)
            : base(new Sql.OvTable(name))
        { }

        public string Database
        {
            get { return this.InternalValue.Database; }
            set { this.InternalValue.Database = value; }
        }

        public string IndexFilegroup
        {
            get { return this.InternalValue.IndexFilegroup; }
            set { this.InternalValue.IndexFilegroup = value; }
        }

        public string Owner
        {
            get { return this.InternalValue.Owner; }
            set { this.InternalValue.Owner = value; }
        }

        public string TableFilegroup
        {
            get { return this.InternalValue.TableFilegroup; }
            set { this.InternalValue.TableFilegroup = value; }
        }

        public string TextFilegroup
        {
            get { return this.InternalValue.TextFilegroup; }
            set { this.InternalValue.TextFilegroup = value; }
        }
    }
}
