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
using Sql = OSGeo.FDO.Providers.Rdbms.Override.SQLServerSpatial;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class SqlServerAssociationPropertyDefinitionItem : RdbmsAssociationPropertyDefinitionItem<Sql.OvAssociationPropertyDefinition>
    {
        public SqlServerAssociationPropertyDefinitionItem(Sql.OvAssociationPropertyDefinition value)
            : base(value)
        {
            List<object> props = new List<object>();
            foreach (Rdbms.OvPropertyDefinition propDef in value.Properties)
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
            IdentityProperties = props.ToArray();
        }

        public object[] IdentityProperties { get; }
    }
}
