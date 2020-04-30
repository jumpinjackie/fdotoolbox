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
using System.Collections.Generic;
using Rdbms = OSGeo.FDO.Providers.Rdbms.Override;
using MySql = OSGeo.FDO.Providers.Rdbms.Override.MySQL;
using System.ComponentModel;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class MySqlClassDefinitionItem : RdbmsClassDefinitionItem<MySql.OvClassDefinition>
    {
        private MySqlTableItem _table;

        public MySqlClassDefinitionItem(MySql.OvClassDefinition clsDef)
            : base(clsDef)
        {
            
        }

        [Description("The auto-increment property name")]
        public string AutoIncrementPropertyName
        {
            get { return this.InternalValue.AutoIncrementPropertyName; }
            set { this.InternalValue.AutoIncrementPropertyName = value; }
        }

        [Description("The auto-increment seed")]
        public long AutoIncrementSeed
        {
            get { return this.InternalValue.AutoIncrementSeed; }
            set { this.InternalValue.AutoIncrementSeed = value; }
        }

        protected override object[] GetClassProperties()
        {
            List<object> props = new List<object>();
            foreach (Rdbms.OvPropertyDefinition propDef in this.InternalValue.Properties)
            {
                if (propDef.GetType() == typeof(MySql.OvDataPropertyDefinition))
                    props.Add(new MySqlDataPropertyDefinitionItem((MySql.OvDataPropertyDefinition)propDef));
                else if (propDef.GetType() == typeof(MySql.OvGeometricPropertyDefinition))
                    props.Add(new MySqlGeometricPropertyDefinitionItem((MySql.OvGeometricPropertyDefinition)propDef));
                else if (propDef.GetType() == typeof(MySql.OvAssociationPropertyDefinition))
                    props.Add(new MySqlAssociationPropertyDefinitionItem((MySql.OvAssociationPropertyDefinition)propDef));
                else if (propDef.GetType() == typeof(MySql.OvObjectPropertyDefinition))
                    props.Add(new MySqlObjectPropertyDefinitionItem((MySql.OvObjectPropertyDefinition)propDef));
            }
            return props.ToArray();
        }

        public override object Table
        {
            get
            {
                return _table;
            }
            set
            {
                var table = value as MySqlTableItem;
                if (table != null)
                {
                    _table = table;
                    this.InternalValue.Table = table.InternalValue;
                }
            }
        }
    }
}
