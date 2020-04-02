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
using Rdbms = OSGeo.FDO.Providers.Rdbms.Override;
using Odbc = OSGeo.FDO.Providers.Rdbms.Override.ODBC;
using System.ComponentModel;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class OdbcClassDefinitionItem : RdbmsClassDefinitionItem<Odbc.OvClassDefinition>
    {
        private OdbcTableItem _table;

        public OdbcClassDefinitionItem(Odbc.OvClassDefinition value)
            : base(value)
        {
            _table = new OdbcTableItem(value.Table);
        }

        protected override object[] GetClassProperties()
        {
            List<object> props = new List<object>();
            foreach (Rdbms.OvPropertyDefinition propDef in this.InternalValue.Properties)
            {
                if (propDef.GetType() == typeof(Odbc.OvDataPropertyDefinition))
                    props.Add(new OdbcDataPropertyDefinitionItem((Odbc.OvDataPropertyDefinition)propDef));
                else if (propDef.GetType() == typeof(Odbc.OvGeometricPropertyDefinition))
                    props.Add(new OdbcGeometricPropertyDefinitionItem((Odbc.OvGeometricPropertyDefinition)propDef));
            }
            return props.ToArray();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public override object Table
        {
            get
            {
                return _table;
            }
            set
            {
                var table = value as OdbcTableItem;
                if (table != null)
                {
                    _table = table;
                    this.InternalValue.Table = table.InternalValue;
                }
            }
        }
    }
}
