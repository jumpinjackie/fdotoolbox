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
        const string OPT_WKT_OV = nameof(OPT_WKT_OV);

        private ContextMenuStrip ctxDeleteTarget;
        private ContextMenuStrip ctxSourceFilter;
        private ContextMenuStrip ctxBatchSize;
        private ContextMenuStrip ctxFlatten;
        private ContextMenuStrip ctxForceWkb;
        private ContextMenuStrip ctxAddWktOverride;

        private TreeNode _deleteTargetNode;
        private TreeNode _sourceFilterNode;
        private TreeNode _batchSizeNode;
        private TreeNode _flattenNode;
        private TreeNode _forceWkbNode;
        private TreeNode _overridesNode;

        internal OptionsNodeDecorator(CopyTaskNodeDecorator parent, TreeNode optionsNode)
        {
            Parent = parent;
            _node = optionsNode;

            InitContextMenus();

            //Options - Delete Target
            _deleteTargetNode = new TreeNode("Delete Target");
            _deleteTargetNode.ToolTipText = "Delete all features on the feature class before copying (true: enabled, false: disabled)";
            _deleteTargetNode.Name = OPT_DEL_TARGET;
            _deleteTargetNode.ContextMenuStrip = ctxDeleteTarget;

            //Options - Source Class Filter
            _sourceFilterNode = new TreeNode("Source Class Filter");
            _sourceFilterNode.ToolTipText = "The filter to apply to the source query where the features will be copied from";
            _sourceFilterNode.Name = OPT_CLS_FILTER;
            _sourceFilterNode.ContextMenuStrip = ctxSourceFilter;

            //Options - Flatten Geometries
            _flattenNode = new TreeNode("Flatten Geometries");
            _flattenNode.ToolTipText = "If true, will strip all Z and M coordinates from geometries being copied";
            _flattenNode.Name = OPT_FLATTEN;
            _flattenNode.ContextMenuStrip = ctxFlatten;

            //Options - Force WKB
            _forceWkbNode = new TreeNode("Force WKB Geometry");
            _forceWkbNode.ToolTipText = "If true, will force the input geometry to be WKB compliant";
            _forceWkbNode.Name = OPT_FORCEWKB;
            _forceWkbNode.ContextMenuStrip = ctxForceWkb;

            //Options - Spatial Context WKT overrides
            _overridesNode = new TreeNode("Spatial context overrides");
            _overridesNode.ToolTipText = "Add spatial context overrides for any spatial contexts that would be copied";
            _overridesNode.Name = OPT_WKT_OV;
            _overridesNode.Tag = new Dictionary<string, string>();
            _overridesNode.ContextMenuStrip = ctxAddWktOverride;

            _node.Nodes.Add(_deleteTargetNode);
            _node.Nodes.Add(_sourceFilterNode);
            _node.Nodes.Add(_flattenNode);
            _node.Nodes.Add(_forceWkbNode);
            _node.Nodes.Add(_overridesNode);

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
                    _batchSizeNode = new TreeNode("Insert Batch Size");
                    _batchSizeNode.ToolTipText = "The batch size to use for batch insert. If set to 0, normal insert will be used";
                    _batchSizeNode.ContextMenuStrip = ctxBatchSize;
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
            ctxAddWktOverride = new ContextMenuStrip();

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
                return _batchSizeNode != null ? Convert.ToInt32(_node.Nodes[4].Tag) : 0;
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

        public bool ForceWkb
        {
            get { return Convert.ToBoolean(_forceWkbNode.Tag); }
            set
            {
                _forceWkbNode.Tag = value;
                _forceWkbNode.Text = "Force WKB: " + value;
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
