#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Schema;
using FdoToolbox.DataStoreManager.Controls.SchemaDesigner;
using ICSharpCode.Core;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.DataStoreManager.Controls
{
    public partial class FdoSchemaView : CollapsiblePanel
    {
        private FdoSchemaViewTreePresenter _presenter;

        public FdoSchemaView()
        {
            InitializeComponent();
        }

        public event EventHandler UpdateState;

        private void FlagUpdateState()
        {
            var handler = this.UpdateState;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            EvaluateStates();
        }

        private void EvaluateStates()
        {
            this.RightPaneVisible = false;
            btnAddSchema.Enabled = true;
            btnFix.Enabled = false;
            this.PhysicalMappingsVisible = false;

            if (_context != null && _context.IsConnected)
            {
                btnFix.Enabled = _context.Schemas.Count > 0;
                this.PhysicalMappingsVisible = _context.CanOverrideSchemas;

                if (_context.Schemas.Count == 1 && !_context.CanHaveMultipleSchemas)
                {
                    btnAddSchema.Enabled = false;
                }
            }
        }

        private SchemaDesignContext _context;

        public SchemaDesignContext Context
        {
            get { return _context; }
            set
            {
                //Stupid WinForms designer!
                if (value == null)
                    return;

                _context = value;
                _presenter = new FdoSchemaViewTreePresenter(this, _context);

                Reset();
            }
        }

        internal void Reset()
        {
            _presenter.Initialize();
            EvaluateStates();
        }

        internal bool PhysicalMappingsVisible
        {
            get { return tabControl1.TabPages.Contains(TAB_PHYSICAL); }
            set
            {
                if (value)
                {
                    if (!tabControl1.TabPages.Contains(TAB_PHYSICAL))
                        tabControl1.TabPages.Add(TAB_PHYSICAL);
                }
                else
                {
                    tabControl1.TabPages.Remove(TAB_PHYSICAL);
                }
            }
        }

        internal bool RightPaneVisible
        {
            get { return !splitContainer1.Panel2Collapsed; }
            set { splitContainer1.Panel2Collapsed = !value; }
        }

        internal void SetLogicalControl(Control c)
        {
            TAB_LOGICAL.Controls.Clear();

            if (c != null)
            {
                c.Dock = DockStyle.Fill;
                TAB_LOGICAL.Controls.Add(c);
            }
        }

        internal void SetPhysicalControl(Control c)
        {
            TAB_PHYSICAL.Controls.Clear();

            if (c != null)
            {
                c.Dock = DockStyle.Fill;
                TAB_PHYSICAL.Controls.Add(c);
            }
        }

        internal string GetSelectedSchema()
        {
            return _presenter.GetSelectedSchema();
        }

        class FdoSchemaViewTreePresenter
        {
            private SchemaDesignContext _context;
            private readonly FdoSchemaView _view;

            const int LEVEL_SCHEMA = 0;
            const int LEVEL_CLASS = 1;
            const int LEVEL_PROPERTY = 2;

            const int IDX_SCHEMA = 0;
            const int IDX_CLASS = 1;
            const int IDX_FEATURE_CLASS = 2;
            const int IDX_KEY = 3;
            const int IDX_DATA_PROPERTY = 4;
            const int IDX_GEOMETRY_PROPERTY = 5;
            const int IDX_ASSOCIATION_PROPERTY = 6;
            const int IDX_OBJECT_PROPERTY = 7;
            const int IDX_RASTER_PROPERTY = 8;

            private ContextMenuStrip ctxSchema;
            private ContextMenuStrip ctxClass;
            private ContextMenuStrip ctxProperty;

            public FdoSchemaViewTreePresenter(FdoSchemaView view, SchemaDesignContext context)
            {
                _view = view;
                _context = context;
                _view.schemaTree.AfterSelect += new TreeViewEventHandler(OnAfterSelect);
                _view.schemaTree.MouseDown += new MouseEventHandler(RightClickHack);

                _context.SchemaAdded += new SchemaElementEventHandler<FeatureSchema>(OnSchemaAdded);
                _context.ClassAdded += new SchemaElementEventHandler<ClassDefinition>(OnClassAdded);
                _context.PropertyAdded += new SchemaElementEventHandler<PropertyDefinition>(OnPropertyAdded);

                _context.ClassRemoved += new SchemaElementEventHandler<ClassDefinition>(OnClassRemoved);
                _context.PropertyRemoved += new SchemaElementEventHandler<PropertyDefinition>(OnPropertyRemoved);

                InitContextMenus();
            }

            void OnPropertyRemoved(PropertyDefinition item)
            {
                if (item.Parent != null && item.Parent.Parent != null)
                {
                    var cls = item.Parent;
                    var schema = cls.Parent;

                    var sn = _view.schemaTree.Nodes[schema.Name];
                    var cn = sn.Nodes[cls.Name];

                    var pn = cn.Nodes[item.Name];
                    pn.Remove();

                    _view.FlagUpdateState();
                }
            }

            void OnClassRemoved(ClassDefinition item)
            {
                if (item.Parent != null)
                {
                    string schema = item.Parent.Name;
                    var sn = _view.schemaTree.Nodes[schema];

                    var cn = sn.Nodes[item.Name];
                    cn.Remove();

                    _view.FlagUpdateState();
                }
            }

            void RightClickHack(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    _view.schemaTree.SelectedNode = _view.schemaTree.GetNodeAt(e.X, e.Y);
                }
            }

            void OnPropertyAdded(PropertyDefinition item)
            {
                if (item.Parent != null && item.Parent.Parent != null)
                {
                    string clsName = item.Parent.Name;
                    string schName = item.Parent.Parent.Name;

                    var sn = _view.schemaTree.Nodes[schName];
                    var cn = sn.Nodes[clsName];

                    var node = CreatePropertyNode(item);

                    if (item.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        ClassDefinition cls = (ClassDefinition)item.Parent;
                        if (cls.IdentityProperties.Contains(item.Name))
                            node.ImageIndex = node.SelectedImageIndex = IDX_KEY;
                    }

                    cn.Nodes.Add(node);
                    cn.Expand();
                    _view.FlagUpdateState();
                }
            }

            void OnClassAdded(ClassDefinition item)
            {
                if (item.Parent != null)
                {
                    string schema = item.Parent.Name;
                    var sn = _view.schemaTree.Nodes[schema];
                    var node = CreateClassNode(item);
                    sn.Nodes.Add(node);

                    foreach (PropertyDefinition pd in item.Properties)
                    {
                        OnPropertyAdded(pd);
                    }

                    sn.Expand();
                    _view.FlagUpdateState();
                }
            }

            void OnSchemaAdded(FeatureSchema item)
            {
                var node = CreateSchemaNode(item);
                _view.schemaTree.Nodes.Add(node);

                foreach (ClassDefinition cd in item.Classes)
                {
                    OnClassAdded(cd);
                }

                _view.EvaluateStates();
                _view.FlagUpdateState();
            }

            private void InitContextMenus()
            {
                ctxClass = new ContextMenuStrip();
                ctxProperty = new ContextMenuStrip();
                ctxSchema = new ContextMenuStrip();

                bool canAdd = !_context.IsConnected || _context.CanModifyExistingSchemas;
                bool canDelete = !_context.IsConnected || _context.CanModifyExistingSchemas;

                //Schema
                if (canAdd)
                {
                    var schemaAdd = new ToolStripMenuItem("Add");

                    if (_context.IsSupportedClass(ClassType.ClassType_FeatureClass))
                        schemaAdd.DropDown.Items.Add("Feature Class", ResourceService.GetBitmap("feature_class"), (s, e) => { AddClass(ClassType.ClassType_FeatureClass); });
                    if (_context.IsSupportedClass(ClassType.ClassType_Class))
                        schemaAdd.DropDown.Items.Add("Class", ResourceService.GetBitmap("database_table"), (s, e) => { AddClass(ClassType.ClassType_Class); });

                    var classAdd = new ToolStripMenuItem("Add");

                    if (_context.IsSupportedProperty(PropertyType.PropertyType_DataProperty))
                        classAdd.DropDown.Items.Add("Data Property", ResourceService.GetBitmap("database_table"), (s, e) => { AddProperty(PropertyType.PropertyType_DataProperty); });

                    if (_context.IsSupportedProperty(PropertyType.PropertyType_GeometricProperty))
                        classAdd.DropDown.Items.Add("Geometric Property", ResourceService.GetBitmap("shape_handles"), (s, e) => { AddProperty(PropertyType.PropertyType_GeometricProperty); });

                    if (_context.IsSupportedProperty(PropertyType.PropertyType_AssociationProperty))
                        classAdd.DropDown.Items.Add("Association Property", ResourceService.GetBitmap("table_relationship"), (s, e) => { AddProperty(PropertyType.PropertyType_AssociationProperty); });

                    if (_context.IsSupportedProperty(PropertyType.PropertyType_ObjectProperty))
                        classAdd.DropDown.Items.Add("Object Property", ResourceService.GetBitmap("package"), (s, e) => { AddProperty(PropertyType.PropertyType_ObjectProperty); });

                    ctxSchema.Items.Add(schemaAdd);
                    ctxClass.Items.Add(classAdd);
                }

                //Add delete item to all
                if (canDelete)
                {
                    ctxClass.Items.Add(new ToolStripSeparator());
                    ctxClass.Items.Add("Delete", ResourceService.GetBitmap("cross"), OnDeleteClass);
                    ctxProperty.Items.Add("Delete", ResourceService.GetBitmap("cross"), OnDeleteProperty);
                }
            }

            void OnDeleteClass(object sender, EventArgs e)
            {
                var node = _view.schemaTree.SelectedNode;
                if (node.Level == LEVEL_CLASS)
                {
                    _context.DeleteClass((ClassDefinition)node.Tag);
                }
            }

            void OnDeleteProperty(object sender, EventArgs e)
            {
                var node = _view.schemaTree.SelectedNode;
                if (node.Level == LEVEL_PROPERTY)
                {
                    _context.DeleteProperty((PropertyDefinition)node.Tag);
                }
            }

            void OnAfterSelect(object sender, TreeViewEventArgs e)
            {
                //For all we know they may have edited something on the previous user interface.
                _view.FlagUpdateState();
                switch (e.Node.Level)
                {
                    case LEVEL_SCHEMA:
                        {
                            _view.TAB_LOGICAL.Text = "Logical Schema";
                            OnSchemaSelected(e.Node);
                        }
                        break;
                    case LEVEL_CLASS:
                        {
                            OnClassSelected(e.Node);
                        }
                        break;
                    case LEVEL_PROPERTY:
                        {
                            OnPropertySelected(e.Node);
                        }
                        break;
                }
            }

            private void OnPropertySelected(TreeNode node)
            {
                PropertyDefinition prop = (PropertyDefinition)node.Tag;
                //C# Lambdas, making code and design compact and elegant since 2007
                NodeUpdateHandler update = () =>
                {
                    node.Text = prop.Name;
                    node.Name = prop.Name;
                };
                Control c = null;
                if (prop.PropertyType == PropertyType.PropertyType_DataProperty)
                {
                    _view.TAB_LOGICAL.Text = "Logical Data Property";
                    c = new DataPropertyCtrl(new DataPropertyDefinitionDecorator((DataPropertyDefinition)prop), _context, update);
                }
                else if (prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                {
                    _view.TAB_LOGICAL.Text = "Logical Geometric Property";
                    c = new GeometricPropertyCtrl(new GeometricPropertyDefinitionDecorator((GeometricPropertyDefinition)prop), _context, update);
                }
                else if (prop.PropertyType == PropertyType.PropertyType_AssociationProperty)
                {
                    _view.TAB_LOGICAL.Text = "Logical Association Property";
                    c = new AssociationPropertyCtrl(new AssociationPropertyDefinitionDecorator((AssociationPropertyDefinition)prop), _context, update);
                }
                else if (prop.PropertyType == PropertyType.PropertyType_ObjectProperty)
                {
                    _view.TAB_LOGICAL.Text = "Logical Object Property";
                    c = new ObjectPropertyCtrl(new ObjectPropertyDefinitionDecorator((ObjectPropertyDefinition)prop), _context, update);
                }

                if (c != null)
                {
                    _view.SetLogicalControl(c);
                    _view.PhysicalMappingsVisible = this.ShowPhysicalMappings;
                    _view.RightPaneVisible = true;
                }
                else
                {
                    _view.RightPaneVisible = false;
                }
            }

            private void OnClassSelected(TreeNode node)
            {
                ClassDefinition cls = (ClassDefinition)node.Tag;
                //C# Lambdas, making code and design compact and elegant since 2007
                NodeUpdateHandler update = () =>
                {
                    node.Text = cls.Name;
                    node.Name = cls.Name;
                };
                NodeUpdateHandler idUpdater = () =>
                {
                    //Go through all the data property nodes and update their images
                    //to reflect what they currently are
                    foreach (TreeNode pn in node.Nodes)
                    {
                        string pName = pn.Name;
                        if (cls.Properties.Contains(pName))
                        {
                            var p = cls.Properties[pName];
                            if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                            {
                                pn.ImageIndex = pn.SelectedImageIndex = IDX_DATA_PROPERTY;
                                if (cls.IdentityProperties.Contains(pName))
                                    pn.ImageIndex = pn.SelectedImageIndex = IDX_KEY;
                            }
                        }
                    }
                };
                Control c = null;
                if (cls.ClassType == ClassType.ClassType_Class)
                {
                    _view.TAB_LOGICAL.Text = "Logical Class";
                    c = new ClassDefinitionCtrl(new ClassDecorator((Class)cls), _context, update, idUpdater);
                }
                else if (cls.ClassType == ClassType.ClassType_FeatureClass)
                {
                    _view.TAB_LOGICAL.Text = "Logical Feature Class";
                    c = new ClassDefinitionCtrl(new FeatureClassDecorator((FeatureClass)cls), _context, update, idUpdater);
                }

                if (c != null)
                {
                    _view.SetLogicalControl(c);
                    _view.PhysicalMappingsVisible = this.ShowPhysicalMappings;
                    _view.RightPaneVisible = true;
                }
                else
                {
                    _view.RightPaneVisible = false;
                }
            }

            internal bool ShowPhysicalMappings
            {
                get { return _context.Connection != null && _context.CanShowPhysicalMapping; }
            }

            private void OnSchemaSelected(TreeNode node)
            {
                FeatureSchema schema = (FeatureSchema)node.Tag;
                //C# Lambdas, making code and design compact and elegant since 2007
                NodeUpdateHandler update = () =>
                {
                    node.Text = schema.Name;
                    node.Name = schema.Name;
                };
                var c = new SchemaCtrl(new FeatureSchemaDecorator(schema), update);
                _view.SetLogicalControl(c);
                _view.PhysicalMappingsVisible = false;
                _view.RightPaneVisible = true;
            }

            public void Initialize()
            {
                _view.schemaTree.Nodes.Clear();
                foreach (FeatureSchema schema in _context.Schemas)
                {
                    var snode = CreateSchemaNode(schema);
                    _view.schemaTree.Nodes.Add(snode);

                    foreach (ClassDefinition cls in schema.Classes)
                    {
                        var cnode = CreateClassNode(cls);
                        snode.Nodes.Add(cnode);

                        foreach (PropertyDefinition prop in cls.Properties)
                        {
                            var pnode = CreatePropertyNode(prop);
                            if (prop.PropertyType == PropertyType.PropertyType_DataProperty)
                            {
                                if (cls.IdentityProperties.Contains(prop.Name))
                                {
                                    pnode.ImageIndex = pnode.SelectedImageIndex = IDX_KEY;
                                }
                            }
                            cnode.Nodes.Add(pnode);
                        }
                    }
                }
            }

            private TreeNode CreateSchemaNode(FeatureSchema schema)
            {
                return new TreeNode()
                {
                    Name = schema.Name,
                    Text = schema.Name,
                    Tag = schema,
                    ImageIndex = IDX_SCHEMA,
                    SelectedImageIndex = IDX_SCHEMA,
                    ContextMenuStrip = ctxSchema
                };
            }

            private TreeNode CreateClassNode(ClassDefinition cls)
            {
                int idx = IDX_CLASS;
                if (cls.ClassType == ClassType.ClassType_FeatureClass)
                    idx = IDX_FEATURE_CLASS;

                return new TreeNode()
                {
                    Name = cls.Name,
                    Text = cls.Name,
                    Tag = cls,
                    ImageIndex = idx,
                    SelectedImageIndex = idx,
                    ContextMenuStrip = ctxClass
                };
            }

            private TreeNode CreatePropertyNode(PropertyDefinition prop)
            {
                int idx = IDX_DATA_PROPERTY;
                if (prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    idx = IDX_GEOMETRY_PROPERTY;
                else if (prop.PropertyType == PropertyType.PropertyType_AssociationProperty)
                    idx = IDX_ASSOCIATION_PROPERTY;
                else if (prop.PropertyType == PropertyType.PropertyType_ObjectProperty)
                    idx = IDX_OBJECT_PROPERTY;
                else if (prop.PropertyType == PropertyType.PropertyType_RasterProperty)
                    idx = IDX_RASTER_PROPERTY;

                return new TreeNode()
                {
                    Name = prop.Name,
                    Text = prop.Name,
                    Tag = prop,
                    ImageIndex = idx,
                    SelectedImageIndex = idx,
                    ContextMenuStrip = ctxProperty
                };
            }

            internal void AddSchema()
            {
                string name = _context.GenerateName("Schema");
                while(_context.SchemaNameExists(name))
                {
                    name = _context.GenerateName("Schema");
                }

                FeatureSchema schema = new FeatureSchema(name, "");
                _context.AddSchema(schema);
            }

            internal void AddClass(ClassType type)
            {
                string schema = GetSelectedSchema();
                if (!string.IsNullOrEmpty(schema))
                {
                    string prefix = "FeatureClass";
                    if (type == ClassType.ClassType_Class)
                        prefix = "Class";

                    string name = _context.GenerateName(prefix);
                    while(_context.ClassNameExists(schema, name))
                    {
                        name = _context.GenerateName(prefix);
                    }
                    ClassDefinition cls = null;
                    if (type == ClassType.ClassType_Class)
                        cls = new Class(name, "");
                    else if (type == ClassType.ClassType_FeatureClass)
                        cls = new FeatureClass(name, "");

                    if (cls != null)
                        _context.AddClass(schema, cls);
                }
            }

            internal void AddProperty(PropertyType type)
            {
                TreeNode node = _view.schemaTree.SelectedNode;
                if (node.Level == LEVEL_CLASS)
                {
                    if (type == PropertyType.PropertyType_AssociationProperty || type == PropertyType.PropertyType_ObjectProperty)
                    {
                        var sn = node.Parent;
                        if (sn.Nodes.Count == 1) //The selected class is the only one
                        {
                            MessageService.ShowError("No other class definitions exist in the current schema");
                            return;
                        }
                    }

                    string schema = node.Parent.Name;
                    string clsName = node.Name;
                    string prefix = "Property";

                    switch (type)
                    {
                        case PropertyType.PropertyType_AssociationProperty:
                            prefix = "AssociationProperty";
                            break;
                        case PropertyType.PropertyType_DataProperty:
                            prefix = "DataProperty";
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            prefix = "GeometricProperty";
                            break;
                        case PropertyType.PropertyType_ObjectProperty:
                            prefix = "ObjectProperty";
                            break;
                        case PropertyType.PropertyType_RasterProperty:
                            prefix = "RasterProperty";
                            break;
                    }

                    string name = _context.GenerateName(prefix);
                    while (_context.PropertyNameExists(schema, clsName, name))
                    {
                        name = _context.GenerateName(prefix);
                    }

                    PropertyDefinition pd = null;
                    switch (type)
                    { 
                        case PropertyType.PropertyType_AssociationProperty:
                            pd = new AssociationPropertyDefinition(name, "");
                            break;
                        case PropertyType.PropertyType_DataProperty:
                            var dp = new DataPropertyDefinition(name, "");
                            if (dp.DataType == DataType.DataType_String)
                                dp.Length = 255;
                            pd = dp;
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            pd = new GeometricPropertyDefinition(name, "");
                            break;
                        case PropertyType.PropertyType_ObjectProperty:
                            pd = new ObjectPropertyDefinition(name, "");
                            break;
                        case PropertyType.PropertyType_RasterProperty:
                            break;
                    }

                    if (pd != null)
                        _context.AddProperty(schema, clsName, pd);
                }
            }

            internal string GetSelectedSchema()
            {
                var node = _view.schemaTree.SelectedNode;
                if (node != null)
                    return node.Level == LEVEL_SCHEMA ? node.Text : null;
                else
                    return null;
            }

            internal void FixIncompatibilities()
            {
                var inc = _context.FindIncompatibilities();
                if (inc.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var i in inc)
                    {
                        sb.Append(i.ToString());
                        sb.Append(Environment.NewLine);
                    }

                    bool attemptAlter = WrappedMessageBox.Confirm("Question",
                        ResourceService.GetStringFormatted("MSG_INCOMPATIBLE_SCHEMA", sb.ToString()), MessageBoxText.YesNo);

                    if (attemptAlter)
                    {
                        _context.FixIncompatibilities();
                        _view.FlagUpdateState();
                    }
                }
                else
                {
                    MessageService.ShowMessage("No incompatibilities detected, nothing changed");
                }
            }

            internal void HandleDeleteKeyPress(TreeNode node)
            {
                switch (node.Level)
                {
                    case LEVEL_CLASS:
                        _context.DeleteClass((ClassDefinition)node.Tag);
                        break;
                    case LEVEL_PROPERTY:
                        _context.DeleteProperty((PropertyDefinition)node.Tag);
                        break;
                }
            }
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            _presenter.FixIncompatibilities();
        }

        private void btnAddSchema_Click(object sender, EventArgs e)
        {
            _presenter.AddSchema();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (!_context.SchemasChanged)
            {
                MessageService.ShowMessage("Schema(s) not changed. Nothing to undo");
                return;
            }

            _context.UndoSchemaChanges();
            Reset();
            FlagUpdateState();
        }

        private void schemaTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var node = schemaTree.SelectedNode;
                if (node != null)
                {
                    _presenter.HandleDeleteKeyPress(node);
                }
            }
        }
    }
}
