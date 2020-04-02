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
using FdoToolbox.Base.Services;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands;
using FdoToolbox.Base.Commands;
using System.Collections;
using OSGeo.FDO.Connections;
using System.Diagnostics;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Base.Controls
{
    public class FdoObjectExplorerExtender : IObjectExplorerExtender
    {
        public const string RootNodeName = "NODE_FDO";

        private const string IMG_CONNECTION = "database_connect";
        private const string IMG_SCHEMA = "chart_organisation";
        private const string IMG_FEATURE_CLASS = "feature_class";
        private const string IMG_CLASS = "database_table";
        private const string IMG_DATA_PROPERTY = "table";
        private const string IMG_ID_PROPERTY = "key";
        private const string IMG_GEOM_PROPERTY = "shape_handles";
        private const string IMG_RASTER_PROPERTY = "image";
        private const string IMG_OBJECT_PROPERTY = "package";
        private const string IMG_ASSOC_PROPERTY = "table_relationship";

        private const string IMG_DB_CONNECTION = "database_connection";
        private const string IMG_SERVER_CONNECTION = "server_connection";
        private const string IMG_FILE_CONNECTION = "file_connection";

        private const string NODE_CONNECTION = "NODE_CONNECTION";
        private const string NODE_SCHEMA = "NODE_SCHEMA";
        private const string NODE_CLASS = "NODE_CLASS";
        //private const string NODE_PROPERTY = "NODE_PROPERTY";

        const string PATH_SELECTED_CONNECTION = "/ObjectExplorer/ContextMenus/SelectedConnection";
        const string PATH_SELECTED_SCHEMA = "/ObjectExplorer/ContextMenus/SelectedSchema";
        const string PATH_SELECTED_CLASS = "/ObjectExplorer/ContextMenus/SelectedClass";

        const int NODE_LEVEL_CONNECTION = 1;
        const int NODE_LEVEL_SCHEMA = 2;
        const int NODE_LEVEL_CLASS = 3;
        const int NODE_LEVEL_PROPERTY = 4;

        private IObjectExplorer _explorer;
        private FdoConnectionManager _connMgr;

        public void Decorate(IObjectExplorer explorer)
        {
            _explorer = explorer;
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            _connMgr.ConnectionAdded += new ConnectionEventHandler(OnConnectionAdded);
            _connMgr.ConnectionRenamed += new ConnectionRenamedEventHandler(OnConnectionRenamed);
            _connMgr.ConnectionRemoved += new ConnectionEventHandler(OnConnectionRemoved);
            _connMgr.ConnectionRefreshed += new ConnectionEventHandler(OnConnectionRefreshed);

            _explorer.RegisterImage(IMG_ASSOC_PROPERTY);
            _explorer.RegisterImage(IMG_FEATURE_CLASS);
            _explorer.RegisterImage(IMG_CLASS);
            _explorer.RegisterImage(IMG_CONNECTION);
            _explorer.RegisterImage(IMG_DATA_PROPERTY);
            _explorer.RegisterImage(IMG_GEOM_PROPERTY);
            _explorer.RegisterImage(IMG_ID_PROPERTY);
            _explorer.RegisterImage(IMG_OBJECT_PROPERTY);
            _explorer.RegisterImage(IMG_RASTER_PROPERTY);
            _explorer.RegisterImage(IMG_SCHEMA);

            _explorer.RegisterImage(IMG_DB_CONNECTION);
            _explorer.RegisterImage(IMG_SERVER_CONNECTION);
            _explorer.RegisterImage(IMG_FILE_CONNECTION);

            _explorer.RegisterRootNode(RootNodeName, "FDO Data Sources", "database_connect", "/ObjectExplorer/ContextMenus/FdoConnections");
            _explorer.RegisterContextMenu(NODE_CONNECTION, PATH_SELECTED_CONNECTION);
            _explorer.RegisterContextMenu(NODE_SCHEMA, PATH_SELECTED_SCHEMA);
            _explorer.RegisterContextMenu(NODE_CLASS, PATH_SELECTED_CLASS);

            _explorer.AfterExpansion += new TreeViewEventHandler(OnAfterNodeExpansion);
            //_explorer.RegisterContextMenu(NODE_PROPERTY, "/ObjectExplorer/ContextMenus/SelectedProperty");
        }

        class TreeNodeScopedTextUpdater : IDisposable
        {
            private TreeNode _node;
            private string _origText;

            public TreeNodeScopedTextUpdater(TreeNode node, string text)
            {
                _node = node;
                _origText = node.Text;
                _node.Text = text;
            }

            public void Dispose()
            {
                _node.Text = _origText;
            }
        }

        private static bool IsChild(TreeNodeCollection nodes, TreeNode node, int maxLevel)
        {
            if (nodes.Contains(node))
                return true;

            foreach (TreeNode n in nodes)
            {
                if (n.Level < maxLevel && IsChild(n.Nodes, node, NODE_LEVEL_CLASS))
                    return true;
            }

            return false;
        }

        void OnAfterNodeExpansion(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Expand)
            {
                TreeNode node = e.Node;
                TreeNode root = _explorer.GetRootNode(RootNodeName);
                //Is a FDO data source node
                if (IsChild(root.Nodes, node, NODE_LEVEL_CLASS))
                {
                    //Find the connection node
                    TreeNode connNode = node;
                    while(connNode.Level > 1)
                    {
                        connNode = connNode.Parent;
                    }
                    string connName = connNode.Name;

                    using (new TempCursor(Cursors.WaitCursor))
                    {
                        switch (node.Level)
                        {
                            case NODE_LEVEL_CONNECTION: //Connection node
                                {
                                    if (!(bool)node.Tag) //Not loaded, load it now
                                    {
                                        //Clear out dummy node
                                        node.Nodes.Clear();
                                        CreateSchemaNodes(node);
                                        node.Tag = true; //Schema is loaded
                                    }
                                }
                                break;
                            case NODE_LEVEL_SCHEMA: //Schema Node
                                {
                                    bool isPartial = Convert.ToBoolean(node.Tag);
                                    if (isPartial)
                                    {
                                        Debug.Assert(node.Nodes.Count == 1); //Has a dummy node
                                        string schemaName = node.Name;
                                        FdoConnection conn = _connMgr.GetConnection(connName);

                                        using (FdoFeatureService svc = conn.CreateFeatureService())
                                        {
                                            Debug.Assert(svc.SupportsPartialSchemaDiscovery());
                                            List<string> classNames = svc.GetClassNames(schemaName);
                                            GetClassNodesPartial(classNames, node);
                                            node.Tag = false; //This node is no longer partial
                                            node.Expand();
                                        }
                                    }
                                }
                                break;
                            case NODE_LEVEL_CLASS: //Class Node
                                {
                                    bool isPartial = Convert.ToBoolean(node.Tag);
                                    if (isPartial)
                                    {
                                        Debug.Assert(node.Nodes.Count == 1); //Has a dummy node
                                        string schemaName = node.Parent.Name;
                                        FdoConnection conn = _connMgr.GetConnection(connName);
                                        using (FdoFeatureService svc = conn.CreateFeatureService())
                                        {
                                            Debug.Assert(svc.SupportsPartialSchemaDiscovery());
                                            ClassDefinition cd = svc.GetClassByName(schemaName, node.Name);
                                            if (cd != null)
                                            {
                                                UpdateClassNode(node, cd);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        void OnConnectionRefreshed(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            TreeNode root = _explorer.GetRootNode(RootNodeName);
            root.Nodes.RemoveByKey(e.Data);
            string name = e.Data;
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;

            FdoConnection conn = _connMgr.GetConnection(name);
            ProviderDatastoreType dtype = conn.DataStoreType;
            switch (dtype)
            {
                case ProviderDatastoreType.ProviderDatastoreType_DatabaseServer:
                    node.ImageKey = node.SelectedImageKey = IMG_DB_CONNECTION;
                    break;
                case ProviderDatastoreType.ProviderDatastoreType_File:
                    node.ImageKey = node.SelectedImageKey = IMG_FILE_CONNECTION;
                    break;
                case ProviderDatastoreType.ProviderDatastoreType_Unknown:
                    node.ImageKey = node.SelectedImageKey = IMG_CONNECTION;
                    break;
                case ProviderDatastoreType.ProviderDatastoreType_WebServer:
                    node.ImageKey = node.SelectedImageKey = IMG_SERVER_CONNECTION;
                    break;
            }

            node.ContextMenuStrip = _explorer.GetContextMenu(NODE_CONNECTION);

            CreateSchemaNodes(node);
            node.Tag = true; //Schema fully loaded

            SetConnectionToolTip(node, conn);

            root.Nodes.Add(node);
            node.Expand();
            root.Expand();
        }

        void OnConnectionRemoved(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            _explorer.GetRootNode(RootNodeName).Nodes.RemoveByKey(e.Data);
        }

        void OnConnectionRenamed(object sender, ConnectionRenameEventArgs e)
        {
            TreeNode node = _explorer.GetRootNode(RootNodeName).Nodes[e.OldName];
            node.Name = node.Text = e.NewName;
        }

        void OnConnectionAdded(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            string name = e.Data;
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;

            FdoConnection conn = _connMgr.GetConnection(name);
            ProviderDatastoreType dtype = conn.DataStoreType;
            switch (dtype)
            {
                case ProviderDatastoreType.ProviderDatastoreType_DatabaseServer:
                    node.ImageKey = node.SelectedImageKey = IMG_DB_CONNECTION;
                    break;
                case ProviderDatastoreType.ProviderDatastoreType_File:
                    node.ImageKey = node.SelectedImageKey = IMG_FILE_CONNECTION;
                    break;
                case ProviderDatastoreType.ProviderDatastoreType_Unknown:
                    node.ImageKey = node.SelectedImageKey = IMG_CONNECTION;
                    break;
                case ProviderDatastoreType.ProviderDatastoreType_WebServer:
                    node.ImageKey = node.SelectedImageKey = IMG_SERVER_CONNECTION;
                    break;
            }
            
            node.ContextMenuStrip = _explorer.GetContextMenu(NODE_CONNECTION);

            //Don't Describe the schema now, do it when node is expanded for the first time
            //use a boolean tag to indicate this state.

            node.Tag = false; //Schema not loaded
            //HACK: TreeNode requires at least one child node to display the expand icon
            //so add a dummy node. When expanded, and describe schema executes for the first time, 
            //the node will be gone anyway.
            node.Nodes.Add(string.Empty);

            TreeNode root = _explorer.GetRootNode(RootNodeName);
            root.Nodes.Add(node);
            root.Expand();

            SetConnectionToolTip(node, conn);
        }

        void CreateSchemaNodes(TreeNode connNode)
        {
            Action<TimeSpan> act = (ts) => LoggingService.Info("Connection " + connNode.Name + ": Schema population completed in " + ts.TotalMilliseconds + "ms");
            FdoConnection conn = _connMgr.GetConnection(connNode.Name);
            if (conn != null)
            {
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    if (service.SupportsPartialSchemaDiscovery())
                    {
                        using (var measure = new TimeMeasurement(act))
                        {
                            List<string> schemaNames = service.GetSchemaNames();

                            //Pre-sort
                            SortedList<string, string> sorted = new SortedList<string, string>();
                            foreach (string name in schemaNames)
                                sorted.Add(name, name);

                            foreach (string name in schemaNames)
                            {
                                TreeNode schemaNode = CreateSchemaNode(name, true);
                                connNode.Nodes.Add(schemaNode);
                                schemaNode.Nodes.Add(ResourceService.GetString("TEXT_LOADING"));
                            }
                        }
                    }
                    else
                    {
                        using (var measure = new TimeMeasurement(act))
                        {
                            FeatureSchemaCollection schemas = service.DescribeSchema();

                            //Pre-sort
                            SortedList<string, FeatureSchema> sorted = new SortedList<string, FeatureSchema>();
                            foreach (FeatureSchema schema in schemas)
                                sorted.Add(schema.Name, schema);

                            foreach (FeatureSchema schema in schemas)
                            {
                                TreeNode schemaNode = CreateSchemaNode(schema.Name, false);
                                GetClassNodesFull(schema, schemaNode);
                                connNode.Nodes.Add(schemaNode);
                                schemaNode.Expand();
                            }
                        }
                    }
                }
            }
        }

        private TreeNode CreateSchemaNode(string name, bool partial)
        {
            TreeNode schemaNode = new TreeNode();
            schemaNode.Name = schemaNode.Text = name;
            schemaNode.ContextMenuStrip = _explorer.GetContextMenu(NODE_SCHEMA);
            schemaNode.ImageKey = schemaNode.SelectedImageKey = IMG_SCHEMA;
            schemaNode.Tag = partial;
            return schemaNode;
        }

        private static void SetConnectionToolTip(TreeNode connNode, FdoConnection conn)
        {
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                List<string> ctxStrings = new List<string>();
                try
                {
                    ICollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                    foreach (SpatialContextInfo sci in contexts)
                    {
                        if (sci.IsActive)
                            ctxStrings.Add("- " + sci.Name + " (Active)");
                        else
                            ctxStrings.Add("- " + sci.Name);
                    }
                }
                catch
                {
                    ctxStrings.Add("Could not retrieve spatial contexts");
                }
                string configStatus = conn.HasConfiguration ? "(This connection has custom configuration applied)" : string.Empty;
                connNode.ToolTipText = string.Format(
                    "Provider: {0}{4}Type: {1}{4}Connection String: {2}{4}Spatial Contexts:{4}{3}{4}{5}",
                    conn.Provider,
                    conn.DataStoreType,
                    conn.SafeConnectionString,
                    ctxStrings.Count > 0 ? string.Join("\n", ctxStrings.ToArray()) : "none",
                    Environment.NewLine,
                    configStatus);
            }
        }

        void GetClassNodesPartial(List<string> clsNames, TreeNode schemaNode)
        {
            //Pre-sort
            SortedList<string, string> sorted = new SortedList<string, string>();
            foreach (string str in clsNames)
                sorted.Add(str, str);

            schemaNode.Nodes.Clear();
            foreach (string clsName in sorted.Values)
            {
                TreeNode classNode = new TreeNode();
                classNode.Name = classNode.Text = clsName;
                classNode.ContextMenuStrip = _explorer.GetContextMenu(NODE_CLASS);
                classNode.ImageKey = classNode.SelectedImageKey = IMG_CLASS;
                classNode.Tag = true; //Is partial
                classNode.Nodes.Add(ResourceService.GetString("TEXT_LOADING"));
                schemaNode.Nodes.Add(classNode);
            }
        }

        void GetClassNodesFull(FeatureSchema schema, TreeNode schemaNode)
        {
            //Pre-sort
            SortedList<string, ClassDefinition> sorted = new SortedList<string, ClassDefinition>();
            foreach (ClassDefinition classDef in schema.Classes)
                sorted.Add(classDef.Name, classDef);

            foreach (ClassDefinition classDef in sorted.Values)
            {
                TreeNode classNode = CreateClassNode(classDef);
                schemaNode.Nodes.Add(classNode);
            }
        }

        private void UpdateClassNode(TreeNode classNode, ClassDefinition classDef)
        {
            if (classNode.Level != NODE_LEVEL_CLASS)
                return;

            classNode.Nodes.Clear();
            classNode.Name = classNode.Text = classDef.Name;
            classNode.ContextMenuStrip = _explorer.GetContextMenu(NODE_CLASS);
            classNode.ImageKey = classNode.SelectedImageKey = (classDef.ClassType == ClassType.ClassType_FeatureClass ? IMG_FEATURE_CLASS : IMG_CLASS);
            classNode.ToolTipText = string.Format("Type: {0}\nIsAbstract: {1}\nIsComputed: {2}\nBase Class: {3}\nUnique Constraints: {4}",
                classDef.ClassType,
                classDef.IsAbstract,
                classDef.IsComputed,
                classDef.BaseClass != null ? classDef.BaseClass.Name : "(none)",
                GetUniqueConstraintString(classDef.UniqueConstraints));
            classNode.Tag = false; //Is not partial
            CreatePropertyNodes(classDef, classNode);
        }

        private TreeNode CreateClassNode(ClassDefinition classDef)
        {
            TreeNode classNode = new TreeNode();
            classNode.Name = classNode.Text = classDef.Name;
            classNode.ContextMenuStrip = _explorer.GetContextMenu(NODE_CLASS);
            classNode.ImageKey = classNode.SelectedImageKey = (classDef.ClassType == ClassType.ClassType_FeatureClass ? IMG_FEATURE_CLASS : IMG_CLASS);
            classNode.ToolTipText = string.Format("Type: {0}\nIsAbstract: {1}\nIsComputed: {2}\nBase Class: {3}\nUnique Constraints: {4}",
                classDef.ClassType,
                classDef.IsAbstract,
                classDef.IsComputed,
                classDef.BaseClass != null ? classDef.BaseClass.Name : "(none)",
                GetUniqueConstraintString(classDef.UniqueConstraints));
            classNode.Tag = false; //Is not partial
            CreatePropertyNodes(classDef, classNode);
            return classNode;
        }

        private static string GetUniqueConstraintString(UniqueConstraintCollection constraints)
        {
            if (constraints.Count == 0)
                return "(none)";
            else
            {
                List<string> ucstr = new List<string>();
                foreach (UniqueConstraint uc in constraints)
                {
                    List<string> str = new List<string>();
                    foreach (DataPropertyDefinition dp in uc.Properties)
                    {
                        str.Add(dp.Name);
                    }
                    ucstr.Add(" - (" + string.Join(",", str.ToArray()) + ")");
                }
                return "\n" + string.Join("\n", ucstr.ToArray());
            }
        }

        void CreatePropertyNodes(ClassDefinition classDef, TreeNode classNode)
        {
            //Pre-sort
            SortedList<string, PropertyDefinition> sorted = new SortedList<string, PropertyDefinition>();
            foreach (PropertyDefinition propDef in classDef.Properties)
                sorted.Add(propDef.Name, propDef);

            foreach (PropertyDefinition propDef in sorted.Values)
            {
                TreeNode propertyNode = new TreeNode();
                propertyNode.Name = propertyNode.Text = propDef.Name;
                switch (propDef.PropertyType)
                {
                    case PropertyType.PropertyType_DataProperty:
                        {
                            DataPropertyDefinition dataDef = propDef as DataPropertyDefinition;
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_DATA_PROPERTY;
                            if (classDef.IdentityProperties.Contains(dataDef))
                                propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_ID_PROPERTY;
                            propertyNode.ToolTipText = string.Format("Data Type: {0}\nLength: {1}\nAuto-Generated: {2}\nRead-Only: {3}\nNullable: {4}", dataDef.DataType, dataDef.Length, dataDef.IsAutoGenerated, dataDef.ReadOnly, dataDef.Nullable);
                        }
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        {
                            GeometricPropertyDefinition geomDef = propDef as GeometricPropertyDefinition;
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_GEOM_PROPERTY;
                            propertyNode.ToolTipText = string.Format("Has Elevation: {0}\nHas Measure: {1}\nRead-Only: {2}\nGeometry Types: {3}\nSpatial Context: {4}", geomDef.HasElevation, geomDef.HasMeasure, geomDef.ReadOnly, GeomTypesAsString(geomDef.GeometryTypes), geomDef.SpatialContextAssociation);
                        }
                        break;
                    case PropertyType.PropertyType_RasterProperty:
                        {
                            RasterPropertyDefinition rastDef = propDef as RasterPropertyDefinition;
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_RASTER_PROPERTY;
                            propertyNode.ToolTipText = "Raster Property";
                        }
                        break;
                    case PropertyType.PropertyType_ObjectProperty:
                        {
                            ObjectPropertyDefinition objDef = propDef as ObjectPropertyDefinition;
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_OBJECT_PROPERTY;
                            propertyNode.ToolTipText = "Object Property";
                        }
                        break;
                    case PropertyType.PropertyType_AssociationProperty:
                        {
                            AssociationPropertyDefinition assocDef = propDef as AssociationPropertyDefinition;
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_ASSOC_PROPERTY;
                            propertyNode.ToolTipText = "Association Property";
                        }
                        break;
                }
                classNode.Nodes.Add(propertyNode);
            }
        }

        private static string GeomTypesAsString(int p)
        {
            List<string> values = new List<string>();
            foreach (var gt in FdoGeometryUtil.GetGeometricTypes(p))
            {
                values.Add(gt.ToString());
            }
            return string.Join(" ", values.ToArray());
        }
    }
}
