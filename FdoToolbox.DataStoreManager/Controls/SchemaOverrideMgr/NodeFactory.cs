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
using System.Linq;
using System.Text;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.ClientServices;
using ICSharpCode.Core;
using System.Windows.Forms;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class NodeFactory
    {
        public static TreeNode CreateNode(PhysicalSchemaMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException("mapping");

            //It is okay to return object, since we're feeding this to the property grid anyway.

            using (ProviderNameTokens providerName = new ProviderNameTokens(mapping.Provider))
            {
                string name = string.Join(".", providerName.GetNameTokens().Take(2).ToArray()).ToUpper();
                switch (name)
                {
                    case "OSGEO.ODBC":
                        return CreateOdbcMappingNode(new OSGeo.FDO.Providers.Rdbms.Override.ODBC.OvPhysicalSchemaMapping(mapping, false));
                    case "OSGEO.SHP":
                        return CreateShpMappingNode(new OSGeo.FDO.Providers.SHP.Override.PhysicalSchemaMapping(mapping, false));
                    case "OSGEO.MYSQL":
                        return CreateMySqlMappingNode(new OSGeo.FDO.Providers.Rdbms.Override.MySQL.OvPhysicalSchemaMapping(mapping, false));
                    case "OSGEO.SQLSERVERSPATIAL":
                        return CreateSqlServerMappingNode(new OSGeo.FDO.Providers.Rdbms.Override.SQLServerSpatial.OvPhysicalSchemaMapping(mapping, false));
                    case "OSGEO.WMS":
                        return CreateWmsMappingNode(new OSGeo.FDO.Providers.WMS.Override.OvPhysicalSchemaMapping(mapping, false));    
                }
            }

            throw new NotSupportedException(ResourceService.GetString("ERR_SCHEMA_MAPPING_UNSUPPORTED_PROVIDER"));
        }

        private static TreeNode CreateOdbcMappingNode(OSGeo.FDO.Providers.Rdbms.Override.ODBC.OvPhysicalSchemaMapping mapping)
        {
            var schema = new OdbcPhysicalSchemaMappingItem(mapping);
            var node = new TreeNode(schema.Name);
            node.Text = schema.Name;
            node.Tag = schema;

            foreach (OdbcClassDefinitionItem cls in schema.Classes)
            {
                var clsNode = new TreeNode(cls.Name);
                clsNode.Text = cls.Name;
                clsNode.Tag = cls;

                foreach (object prop in cls.Properties)
                {
                    if (prop.GetType() == typeof(OdbcDataPropertyDefinitionItem))
                    {
                        OdbcDataPropertyDefinitionItem p = (OdbcDataPropertyDefinitionItem)prop;
                        var propNode = new TreeNode(p.Name);
                        propNode.Text = p.Name;
                        propNode.Tag = p;
                        clsNode.Nodes.Add(propNode);
                    }
                    else if (prop.GetType() == typeof(OdbcGeometricPropertyDefinitionItem))
                    {
                        OdbcGeometricPropertyDefinitionItem p = (OdbcGeometricPropertyDefinitionItem)prop;
                        var propNode = new TreeNode(p.Name);
                        propNode.Text = p.Name;
                        propNode.Tag = p;
                        clsNode.Nodes.Add(propNode);
                    }
                }

                node.Nodes.Add(clsNode);
            }

            return node;
        }

        private static TreeNode CreateMySqlMappingNode(OSGeo.FDO.Providers.Rdbms.Override.MySQL.OvPhysicalSchemaMapping mapping)
        {
            var schema = new MySqlPhysicalSchemaMappingItem(mapping);
            var node = new TreeNode(schema.Name);
            node.Text = schema.Name;
            node.Tag = schema;

            foreach (MySqlClassDefinitionItem cls in schema.Classes)
            {
                var clsNode = new TreeNode(cls.Name);
                clsNode.Text = cls.Name;
                clsNode.Tag = cls;

                foreach (object prop in cls.Properties)
                {
                    if (prop.GetType() == typeof(MySqlDataPropertyDefinitionItem))
                    {
                        MySqlDataPropertyDefinitionItem p = (MySqlDataPropertyDefinitionItem)prop;
                        var propNode = new TreeNode(p.Name);
                        propNode.Text = p.Name;
                        propNode.Tag = p;
                        clsNode.Nodes.Add(propNode);
                    }
                    else if (prop.GetType() == typeof(MySqlGeometricPropertyDefinitionItem))
                    {
                        MySqlGeometricPropertyDefinitionItem p = (MySqlGeometricPropertyDefinitionItem)prop;
                        var propNode = new TreeNode(p.Name);
                        propNode.Text = p.Name;
                        propNode.Tag = p;
                        clsNode.Nodes.Add(propNode);
                    }
                }

                node.Nodes.Add(clsNode);
            }

            return node;
        }

        private static TreeNode CreateSqlServerMappingNode(OSGeo.FDO.Providers.Rdbms.Override.SQLServerSpatial.OvPhysicalSchemaMapping mapping)
        {
            var schema = new SqlServerPhysicalSchemaMappingItem(mapping);
            var node = new TreeNode(schema.Name);
            node.Text = schema.Name;
            node.Tag = schema;

            foreach (SqlServerClassDefinitionItem cls in schema.Classes)
            {
                var clsNode = new TreeNode(cls.Name);
                clsNode.Text = cls.Name;
                clsNode.Tag = cls;

                foreach (object prop in cls.Properties)
                {
                    if (prop.GetType() == typeof(SqlServerDataPropertyDefinitionItem))
                    {
                        SqlServerDataPropertyDefinitionItem p = (SqlServerDataPropertyDefinitionItem)prop;
                        var propNode = new TreeNode(p.Name);
                        propNode.Text = p.Name;
                        propNode.Tag = p;
                        clsNode.Nodes.Add(propNode);
                    }
                    else if (prop.GetType() == typeof(SqlServerGeometricPropertyDefinitionItem))
                    {
                        SqlServerGeometricPropertyDefinitionItem p = (SqlServerGeometricPropertyDefinitionItem)prop;
                        var propNode = new TreeNode(p.Name);
                        propNode.Text = p.Name;
                        propNode.Tag = p;
                        clsNode.Nodes.Add(propNode);
                    }
                }

                node.Nodes.Add(clsNode);
            }

            return node;
        }

        private static TreeNode CreateShpMappingNode(OSGeo.FDO.Providers.SHP.Override.PhysicalSchemaMapping mapping)
        {
            var schema = new ShpPhysicalSchemaMappingItem(mapping);
            var node = new TreeNode(schema.Name);
            node.Text = schema.Name;
            node.Tag = schema;

            foreach (ShpClassDefinitionItem cls in schema.Classes)
            {
                var clsNode = new TreeNode(cls.Name);
                clsNode.Text = cls.Name;
                clsNode.Tag = cls;

                foreach (ShpPropertyDefinitionItem p in cls.Properties)
                {
                    var propNode = new TreeNode(p.Name);
                    propNode.Text = p.Name;
                    propNode.Tag = p;
                    clsNode.Nodes.Add(propNode);
                }

                node.Nodes.Add(clsNode);
            }

            return node;
        }

        private static TreeNode CreateWmsMappingNode(OSGeo.FDO.Providers.WMS.Override.OvPhysicalSchemaMapping mapping)
        {
            var schema = new WmsPhysicalSchemaMappingItem(mapping);
            var node = new TreeNode(schema.Name);
            node.Text = schema.Name;
            node.Tag = schema;

            foreach (WmsClassDefinitionItem cls in schema.Classes)
            {
                var clsNode = new TreeNode(cls.Name);
                clsNode.Text = cls.Name;
                clsNode.Tag = cls;

                node.Nodes.Add(clsNode);
            }

            return node;
        }
    }
}
