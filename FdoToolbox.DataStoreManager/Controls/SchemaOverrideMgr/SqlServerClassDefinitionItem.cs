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
using Rdbms = OSGeo.FDO.Providers.Rdbms.Override;
using Sql = OSGeo.FDO.Providers.Rdbms.Override.SQLServerSpatial;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class SqlServerClassDefinitionItem : RdbmsClassDefinitionItem<Sql.OvClassDefinition>
    {
        public SqlServerClassDefinitionItem(Sql.OvClassDefinition value)
            : base(value)
        {
            _table = new SqlServerTableItem(value.Table);
        }

        protected override object[] GetClassProperties()
        {
            List<object> props = new List<object>();
            foreach (Rdbms.OvPropertyDefinition propDef in this.InternalValue.Properties)
            {
                if (propDef.GetType() == typeof(Sql.OvDataPropertyDefinition))
                    props.Add(new SqlServerDataPropertyDefinitionItem((Sql.OvDataPropertyDefinition)propDef));
                else if (propDef.GetType() == typeof(Sql.OvGeometricPropertyDefinition))
                    props.Add(new SqlServerGeometricPropertyDefinitionItem((Sql.OvGeometricPropertyDefinition)propDef));
                else if (propDef.GetType() == typeof(Sql.OvAssociationPropertyDefinition))
                    props.Add(new SqlServerAssociationPropertyDefinitionItem((Sql.OvAssociationPropertyDefinition)propDef));
                else if (propDef.GetType() == typeof(Sql.OvObjectPropertyDefinition))
                    props.Add(new SqlServerObjectPropertyDefinitionItem((Sql.OvObjectPropertyDefinition)propDef));
            }
            return props.ToArray();
        }

        private SqlServerTableItem _table;

        public override object Table
        {
            get
            {
                return _table;
            }
            set
            {
                var table = value as SqlServerTableItem;
                if (table != null)
                {
                    _table = table;
                    this.InternalValue.Table = table.InternalValue;
                }
            }
        }

        public int IdentityIncrement
        {
            get { return this.InternalValue.IdentityIncrement; }
            set { this.InternalValue.IdentityIncrement = value; }
        }

        public bool IdentityIsGloballyUnique
        {
            get { return this.InternalValue.IdentityIsGloballyUnique; }
            set { this.InternalValue.IdentityIsGloballyUnique = value; }
        }

        public string IdentityPropertyName
        {
            get { return this.InternalValue.IdentityPropertyName; }
            set { this.InternalValue.IdentityPropertyName = value; }
        }

        public int IdentitySeed
        {
            get { return this.InternalValue.IdentitySeed; }
            set { this.InternalValue.IdentitySeed = value; }
        }
    }
}
