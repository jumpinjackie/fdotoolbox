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
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    /// <summary>
    /// Helper class to perform manipulation of the tree nodes
    /// </summary>
    internal class OptionsNodeDecorator
    {
        private TreeNode _node;

        internal readonly CopyTaskNodeDecorator Parent;

        const string OPT_DEL_TARGET = "OPT_DEL_TARGET";
        const string OPT_CLS_FILTER = "OPT_CLS_FILTER";
        const string OPT_BATCH_SIZE = "OPT_BATCH_SIZE";
        const string OPT_FLATTEN = "OPT_FLATTEN";
        const string OPT_FORCEWKB = "OPT_FORCEWKB";
        const string OPT_USE_TARGET_SC = nameof(OPT_USE_TARGET_SC);
        const string OPT_WKT_OV = nameof(OPT_WKT_OV);
        const string OPT_XFORM = nameof(OPT_XFORM);

        private ContextMenuStrip ctxDeleteTarget;
        private ContextMenuStrip ctxSourceFilter;
        private ContextMenuStrip ctxBatchSize;
        private ContextMenuStrip ctxFlatten;
        private ContextMenuStrip ctxForceWkb;
        private ContextMenuStrip ctxTargetScs;
        private ContextMenuStrip ctxAddWktOverride;
        private ContextMenuStrip ctxTransform;

        private TreeNode _deleteTargetNode;
        private TreeNode _sourceFilterNode;
        private TreeNode _batchSizeNode;
        private TreeNode _flattenNode;
        private TreeNode _forceWkbNode;
        private TreeNode _useTargetSpatialContextNode;
        private TreeNode _overridesNode;
        private TreeNode _transformNode;

        internal OptionsNodeDecorator(CopyTaskNodeDecorator parent, TreeNode optionsNode)
        {
            Parent = parent;
            _node = optionsNode;

            InitContextMenus();

            //Options - Delete Target
            _deleteTargetNode = new TreeNode("Delete Target")
            {
                ToolTipText = "Delete all features on the feature class before copying (true: enabled, false: disabled)",
                Name = OPT_DEL_TARGET,
                ContextMenuStrip = ctxDeleteTarget
            };

            //Options - Source Class Filter
            _sourceFilterNode = new TreeNode("Source Class Filter")
            {
                ToolTipText = "The filter to apply to the source query where the features will be copied from",
                Name = OPT_CLS_FILTER,
                ContextMenuStrip = ctxSourceFilter
            };

            //Options - Flatten Geometries
            _flattenNode = new TreeNode("Flatten Geometries")
            {
                ToolTipText = "If true, will strip all Z and M coordinates from geometries being copied",
                Name = OPT_FLATTEN,
                ContextMenuStrip = ctxFlatten
            };

            //Options - Force WKB
            _forceWkbNode = new TreeNode("Force WKB Geometry")
            {
                ToolTipText = "If true, will force the input geometry to be WKB compliant",
                Name = OPT_FORCEWKB,
                ContextMenuStrip = ctxForceWkb
            };

            //Options - Use Target Spatial Context
            _useTargetSpatialContextNode = new TreeNode("Use Target Spatial Context")
            {
                ToolTipText = "Instead of creating a new spatial context from source and associating it to any geometric properties, associate it to an existing spatial context on the target",
                Name = OPT_USE_TARGET_SC,
                ContextMenuStrip = ctxTargetScs
            };

            //Options - Spatial Context WKT overrides
            _overridesNode = new TreeNode("Spatial context overrides")
            {
                ToolTipText = "Add spatial context overrides for any spatial contexts that would be copied",
                Name = OPT_WKT_OV,
                Tag = new Dictionary<string, string>(),
                ContextMenuStrip = ctxAddWktOverride
            };

            //Options - Transform Geometries
            _transformNode = new TreeNode("Transform Geometries")
            {
                ToolTipText = "Transform geometries from the source spatial context CS to the target spatial context CS if they are different",
                Name = OPT_XFORM,
                ContextMenuStrip = ctxTransform
            };

            _node.Nodes.Add(_deleteTargetNode);
            _node.Nodes.Add(_sourceFilterNode);
            _node.Nodes.Add(_flattenNode);
            _node.Nodes.Add(_forceWkbNode);
            _node.Nodes.Add(_useTargetSpatialContextNode);
            _node.Nodes.Add(_overridesNode);
            _node.Nodes.Add(_transformNode);

            //Set default values to avoid any nasty surprises
            this.Delete = false;
            this.SourceFilter = string.Empty;
            this.Flatten = false;
            this.ForceWkb = false;

            //Test for batch support
            using (FdoFeatureService svc = Parent.GetTargetConnection().CreateFeatureService())
            {
                if (svc.SupportsBatchInsertion())
                {
                    _batchSizeNode = new TreeNode("Insert Batch Size")
                    {
                        ToolTipText = "The batch size to use for batch insert. If set to 0, normal insert will be used",
                        ContextMenuStrip = ctxBatchSize
                    };
                    _node.Nodes.Add(_batchSizeNode);
                    //Set default values to avoid any nasty surprises
                    this.BatchSize = 0;
                }
            }
        }

        private void InitContextMenus()
        {
            ctxDeleteTarget = new ContextMenuStrip();
            ctxSourceFilter = new ContextMenuStrip();
            ctxBatchSize = new ContextMenuStrip();
            ctxFlatten = new ContextMenuStrip();
            ctxForceWkb = new ContextMenuStrip();
            ctxTargetScs = new ContextMenuStrip();
            ctxAddWktOverride = new ContextMenuStrip();
            ctxTransform = new ContextMenuStrip();

            //Delete Target
            ctxDeleteTarget.Items.Add("True", null, (s, e) => { this.Delete = true; });
            ctxDeleteTarget.Items.Add("False", null, (s, e) => { this.Delete = false; });

            //Source Filter
            ctxSourceFilter.Items.Add("Set Filter", null, (s, e) => {
                string filter = this.SourceFilter;
                string newFilter = ExpressionEditor.EditExpression(Parent.GetSourceConnection(), Parent.SourceClass, null, filter, ExpressionMode.Filter);
                if (filter != newFilter)
                {
                    this.SourceFilter = newFilter;
                }
            });
            ctxSourceFilter.Items.Add("Clear", null, (s, e) => { this.SourceFilter = string.Empty; });

            //Flatten Geometries
            ctxFlatten.Items.Add("True", null, (s, e) => { this.Flatten = true; });
            ctxFlatten.Items.Add("False", null, (s, e) => { this.Flatten = false; });

            //Force wkb
            ctxForceWkb.Items.Add("True", null, (s, e) => { this.ForceWkb = true; });
            ctxForceWkb.Items.Add("False", null, (s, e) => { this.ForceWkb = false; });

            //Use Target Spatial Context
            using (var svc = Parent.GetTargetConnection().CreateFeatureService())
            {
                var targetScs = svc.GetSpatialContexts();
                foreach (var sc in targetScs)
                {
                    ctxTargetScs.Items.Add(sc.Name, null, (s, e) => { this.UseTargetSpatialContext = sc.Name; });
                }
            }

            //Batch Size
            ctxBatchSize.Items.Add("Set Size", null, (s, e) =>
            {
                string result = MessageService.ShowInputBox("Batch Size", "Set batch size", this.BatchSize.ToString());
                int size;
                while (!int.TryParse(result, out size))
                {
                    result = MessageService.ShowInputBox("Batch Size", "Set batch size", result);
                    if (result == null) //cancelled
                    return;
                }
                this.BatchSize = size;
            });

            //WKT overrides
            ctxAddWktOverride.Items.Add("Set spatial context overrides", null, (s, e) =>
            {
                using (var service = Parent.GetSourceConnection().CreateFeatureService())
                {
                    var spContexts = service.GetSpatialContexts();
                    var diag = new SpatialContextOverridesDialog(spContexts, this.SpatialContextWktOverrides);
                    if (diag.ShowDialog() == DialogResult.OK)
                    {
                        this.SpatialContextWktOverrides = diag.GetOverrides();
                        this.PopulateOverrideNodes();
                    }
                }
            });

            //Transform
            ctxTransform.Items.Add("True", null, (s, e) => { this.Transform = true; });
            ctxTransform.Items.Add("False", null, (s, e) => { this.Transform = false; });
        }

        private void PopulateOverrideNodes()
        {
            _overridesNode.Nodes.Clear();
            var ov = this.SpatialContextWktOverrides;
            if (ov.Count > 0)
            {
                foreach (var kvp in ov)
                {
                    var scNode = _overridesNode.Nodes.Add(kvp.Key);
                    scNode.Nodes.Add($"Copy as: {kvp.Value.OverrideScName ?? "(original name)"}");
                    scNode.Nodes.Add($"CS Name: {kvp.Value.CsName}");
                    scNode.Nodes.Add($"CS WKT: {kvp.Value.CsWkt}");
                }
                _overridesNode.ExpandAll();
            }
        }

        public bool Delete
        {
            get { return Convert.ToBoolean(_deleteTargetNode.Tag); }
            set 
            {
                _deleteTargetNode.Tag = value;
                _deleteTargetNode.Text = "Delete: " + value;
            }
        }

        public string SourceFilter
        {
            get { return _sourceFilterNode.Tag.ToString(); }
            set 
            { 
                _sourceFilterNode.Tag = string.IsNullOrEmpty(value) ? string.Empty : value;
                _sourceFilterNode.Text = "Source Class Filter: " + value;
            }
        }

        public bool Flatten
        {
            get { return Convert.ToBoolean(_flattenNode.Tag); }
            set
            {
                _flattenNode.Tag = value;
                _flattenNode.Text = "Flatten Geometries: " + value;
            }
        }

        public int BatchSize
        {
            get
            {
                return _batchSizeNode != null ? Convert.ToInt32(_batchSizeNode.Tag) : 0;
            }
            set 
            {
                if (_batchSizeNode != null)
                {
                    _batchSizeNode.Tag = value;
                    _batchSizeNode.Text = "Insert Batch Size: " + value;
                }
            }
        }

        public string UseTargetSpatialContext
        {
            get { return _useTargetSpatialContextNode.Tag?.ToString(); }
            set 
            { 
                _useTargetSpatialContextNode.Tag = value;
                _useTargetSpatialContextNode.Text = "Use Target Spatial Context: " + value;
            }
        }

        public bool ForceWkb
        {
            get { return Convert.ToBoolean(_forceWkbNode.Tag); }
            set
            {
                _forceWkbNode.Tag = value;
                _forceWkbNode.Text = "Force WKB: " + value;
            }
        }

        public bool Transform
        {
            get { return Convert.ToBoolean(_transformNode.Tag); }
            set
            {
                _transformNode.Tag = value;
                _transformNode.Text = "Transform Geometries: " + value;
            }
        }

        public Dictionary<string, SCOverrideItem> SpatialContextWktOverrides
        {
            get { return _overridesNode.Tag as Dictionary<string, SCOverrideItem> ?? new Dictionary<string, SCOverrideItem>(); }
            set 
            { 
                _overridesNode.Tag = value;
                this.PopulateOverrideNodes();
            }
        }
    }
}
