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
using MySql = OSGeo.FDO.Providers.Rdbms.Override.MySQL;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class MySqlAssociationPropertyDefinitionItem : RdbmsAssociationPropertyDefinitionItem<MySql.OvAssociationPropertyDefinition>
    {
        private object[] _idProps;

        public MySqlAssociationPropertyDefinitionItem(MySql.OvAssociationPropertyDefinition value)
            : base(value)
        {
            List<object> props = new List<object>();
            foreach (Rdbms.OvPropertyDefinition propDef in value.Properties)
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
            _idProps = props.ToArray();
        }

        public object[] IdentityProperties
        {
            get { return _idProps; }
        }
    }
}
