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
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base;
using OSGeo.FDO.Schema;
using System.Diagnostics;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    /// <summary>
    /// Helper class to perform manipulation of the tree nodes
    /// </summary>
    internal class CopyTaskNodeDecorator
    {
        private FdoConnectionManager _connMgr;

        public TreeNode DecoratedNode { get; }

        public string Name
        {
            get { return DecoratedNode.Text; }
            set { DecoratedNode.Text = value; }
        }

        public string Description
        {
            get { return DecoratedNode.ToolTipText; }
            set { DecoratedNode.ToolTipText = value; }
        }

        internal FdoConnection GetSourceConnection() => _connMgr.GetConnection(_srcConnName);

        internal FdoConnection GetTargetConnection() => _connMgr.GetConnection(_dstConnName);

        internal CopyTaskNodeDecorator(TreeNode root, string srcConnName, string srcSchema, string srcClass, string dstConnName, string dstSchema, string dstClass, string dstClassOv, string taskName, bool createIfNotExists)
        {
            DecoratedNode = new TreeNode();
            root.Nodes.Add(DecoratedNode);

            DecoratedNode.Nodes.Add(new TreeNode("Description"));
            DecoratedNode.Nodes.Add(new TreeNode("Options"));
            DecoratedNode.Nodes.Add(new TreeNode("Property Mappings"));
            DecoratedNode.Nodes.Add(new TreeNode("Expression Mappings (Right click to add)"));

            this.Name = taskName;
            this.Description = "Copies features from " + srcClass + " to " + dstClass;

            InitDescription(srcConnName,
                            srcSchema,
                            srcClass,
                            dstConnName,
                            dstSchema,
                            dstClass,
                            dstClassOv,
                            createIfNotExists);

            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();

            _srcConnName = srcConnName;
            _dstConnName = dstConnName;
            this.CreateIfNotExists = createIfNotExists;

            using (FdoFeatureService srcSvc = GetSourceConnection().CreateFeatureService())
            using (FdoFeatureService dstSvc = GetTargetConnection().CreateFeatureService())
            {
                ClassDefinition sourceClass = srcSvc.GetClassByName(srcSchema, srcClass);
                Debug.Assert(sourceClass != null);

                ClassDefinition targetClass = dstSvc.GetClassByName(dstSchema, dstClass);
                if (targetClass == null && !this.CreateIfNotExists)
                    throw new InvalidOperationException("Target class " + dstClass + " does not exist. If you want this class created make sure you checked \"Create class of the name name\" when creating a new copy task");

                SourceClass = sourceClass;
                TargetClass = targetClass;
            }

            Options = new OptionsNodeDecorator(this, DecoratedNode.Nodes[1]);
            PropertyMappings = new PropertyMappingsNodeDecorator(this, DecoratedNode.Nodes[2]);
            ExpressionMappings = new ExpressionMappingsNodeDecorator(this, DecoratedNode.Nodes[3]);

            DecoratedNode.ExpandAll();
        }

        private string _dstSchemaName;

        public string SourceSchemaName { get; private set; }

        public string SourceClassName => _srcNode.Tag.ToString();

        public string TargetSchemaName => _dstSchemaName;

        public string TargetClassName => _dstNode.Tag.ToString();

        public string TargetClassNameOverride => _ovTargetClassNode?.Tag?.ToString();

        public bool CreateIfNotExists { get; private set; }

        private TreeNode _srcNode;
        private TreeNode _dstNode;
        private TreeNode _ovTargetClassNode;

        private void InitDescription(string srcConnName,
                                     string srcSchema,
                                     string srcClass,
                                     string dstConnName,
                                     string dstSchema,
                                     string dstClass,
                                     string dstClassOv,
                                     bool createIfNotExists)
        {
            _srcNode = new TreeNode("Source");
            _dstNode = new TreeNode("Target");
            _srcNode.Nodes.Add("Connection: " + srcConnName);
            _srcNode.Nodes.Add("Schema: " + srcSchema);
            _srcNode.Nodes.Add("Feature Class: " + srcClass);

            _dstNode.Nodes.Add("Connection: " + dstConnName);
            _dstNode.Nodes.Add("Schema: " + dstSchema);

            if (createIfNotExists)
                dstClass = srcClass;

            var suffix = "(created if it doesn't exist)";
            if (!string.IsNullOrWhiteSpace(dstClassOv))
                suffix = $"(created as [{dstClassOv}] if it doesn't exist)";

            if (createIfNotExists)
            {
                var n = _dstNode.Nodes.Add("Feature Class: " + dstClass);
                _ovTargetClassNode = n.Nodes.Add(suffix);
                _ovTargetClassNode.Tag = dstClassOv;
                n.Expand();
            }
            else
                _dstNode.Nodes.Add("Feature Class: " + dstClass);

            DecoratedNode.Nodes[0].Nodes.Add(_srcNode);
            DecoratedNode.Nodes[0].Nodes.Add(_dstNode);

            SourceSchemaName = srcSchema;
            _srcNode.Tag = srcClass;

            _dstSchemaName = dstSchema;
            _dstNode.Tag = dstClass;
        }

        private string _srcConnName;

        public string SourceConnectionName
        {
            get
            {
                return _srcConnName;
            }
            internal set
            {
                _srcConnName = value;
                _srcNode.Nodes[0].Text = "Connection: " + _srcConnName;
            }
        }

        private string _dstConnName;

        public string TargetConnectionName
        {
            get
            {
                return _dstConnName;
            }
            internal set
            {
                _dstConnName = value;
                _dstNode.Nodes[0].Text = "Connection: " + _dstConnName;
            }
        }

        internal ClassDefinition SourceClass { get; }

        internal ClassDefinition TargetClass { get; }

        public OptionsNodeDecorator Options { get; }

        public PropertyMappingsNodeDecorator PropertyMappings { get; }

        public ExpressionMappingsNodeDecorator ExpressionMappings { get; }
    }
}
