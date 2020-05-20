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
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using System.Collections.Specialized;
using FdoToolbox.Core;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Controls;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoJoinCtl : ViewContent, IFdoJoinView
    {
        private FdoJoinPresenter _presenter;

        private FdoJoin _initOptions;

        public FdoJoinCtl()
        {
            InitializeComponent();
            ServiceManager sm = ServiceManager.Instance;
            _presenter = new FdoJoinPresenter(this, 
                sm.GetService<IFdoConnectionManager>(),
                sm.GetService<TaskManager>());
        }

        public FdoJoinCtl(string taskName, FdoJoin options)
            : this()
        {
            _initOptions = options;
            txtName.Text = taskName;
            txtName.ReadOnly = true; //This is edit mode, so the task name can't be changed
        }

        protected override void OnBeforeConnectionRemove(object sender, ConnectionBeforeRemoveEventArgs e)
        {
            if (this.SelectedLeftConnection.Equals(e.ConnectionName) ||
                this.SelectedRightConnection.Equals(e.ConnectionName) ||
                this.SelectedTargetConnection.Equals(e.ConnectionName))
            {
                MessageService.ShowMessage("Cannot remove connection " + e.ConnectionName + " as this join task depends on it");
                e.Cancel = true;
            }
        }

        protected override void OnConnectionRenamed(object sender, ConnectionRenameEventArgs e)
        {
            var left = (BindingList<string>)cmbLeftConnection.DataSource;
            var right = (BindingList<string>)cmbRightConnection.DataSource;
            var target = (BindingList<string>)cmbTargetConnection.DataSource;

            var idx = left.IndexOf(e.OldName);
            if (idx >= 0)
                left[idx] = e.NewName;

            idx = right.IndexOf(e.OldName);
            if (idx >= 0)
                right[idx] = e.NewName;

            idx = target.IndexOf(e.OldName);
            if (idx >= 0)
                target[idx] = e.NewName;

            /*
            string selLeft = this.SelectedLeftConnection;
            string selRight = this.SelectedRightConnection;
            string selTarget = this.SelectedTargetConnection;

            //Fix the selected values if they are affected
            if (selLeft.Equals(e.OldName))
            {
                selLeft = e.NewName;
            }
            if (selRight.Equals(e.OldName))
            {
                selRight = e.NewName;
            }
            if (selTarget.Equals(e.OldName))
            {
                selTarget = e.NewName;
            }

            //Fix the list, any list that is fixed needs to have
            //their current value re-set
            if (left.Contains(e.OldName))
            {
                left.Remove(e.OldName);
                left.Add(e.NewName);

                this.LeftConnections = left;
                this.SelectedLeftConnection = selLeft;
            }
            if (right.Contains(e.OldName))
            {
                right.Remove(e.OldName);
                right.Add(e.NewName);

                this.RightConnections = right;
                this.SelectedRightConnection = selRight;
            }
            if (target.Contains(e.OldName))
            {
                target.Remove(e.OldName);
                target.Add(e.NewName);

                this.TargetConnections = target;
                this.SelectedTargetConnection = selTarget;
            }*/
        }

        protected override void OnLoad(EventArgs e)
        {
            if (_initOptions == null)
                _presenter.Init();
            else
                _presenter.Init(_initOptions);
            base.OnLoad(e);
        }

        public override string Title => ResourceService.GetString("TITLE_JOIN_SETTINGS");

        public List<string> LeftConnections
        {
            set { cmbLeftConnection.DataSource = new BindingList<string>(value); }
        }

        public List<string> RightConnections
        {
            set { cmbRightConnection.DataSource = new BindingList<string>(value); }
        }

        public List<string> TargetConnections
        {
            set { cmbTargetConnection.DataSource = new BindingList<string>(value); }
        }

        public List<string> LeftSchemas
        {
            set { cmbLeftSchema.DataSource = value; }
        }

        public List<string> RightSchemas
        {
            set { cmbRightSchema.DataSource = value; }
        }

        public List<string> TargetSchemas
        {
            set { cmbTargetSchema.DataSource = value; }
        }

        public List<string> LeftClasses
        {
            set { cmbLeftClass.DataSource = value; }
        }

        public List<string> RightClasses
        {
            set { cmbRightClass.DataSource = value; }
        }

        public Array JoinTypes
        {
            set { cmbJoinTypes.DataSource = value; }
        }

        public FdoJoinType SelectedJoinType
        {
            get { return (FdoJoinType)cmbJoinTypes.SelectedItem; }
            set { cmbJoinTypes.SelectedItem = value; }
        }

        public string SelectedLeftConnection
        {
            get
            {
                return cmbLeftConnection.SelectedItem.ToString();
            }
            set
            {
                cmbLeftConnection.SelectedItem = value;
                cmbLeftConnection_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public string SelectedRightConnection
        {
            get
            {
                return cmbRightConnection.SelectedItem.ToString();
            }
            set
            {
                cmbRightConnection.SelectedItem = value;
                cmbRightConnection_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public string SelectedTargetConnection
        {
            get
            {
                return cmbTargetConnection.SelectedItem.ToString();
            }
            set
            {
                cmbTargetConnection.SelectedItem = value;
                cmbTargetConnection_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public string SelectedLeftSchema
        {
            get
            {
                return cmbLeftSchema.SelectedItem?.ToString();
            }
            set
            {
                cmbLeftSchema.SelectedItem = value;
                cmbLeftSchema_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public string SelectedRightSchema
        {
            get
            {
                return cmbRightSchema.SelectedItem?.ToString();
            }
            set
            {
                cmbRightSchema.SelectedItem = value;
                cmbRightSchema_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public string SelectedTargetSchema
        {
            get
            {
                return cmbTargetSchema.SelectedItem?.ToString();
            }
            set
            {
                cmbTargetSchema.SelectedItem = value;
                cmbTargetSchema_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public string SelectedLeftClass
        {
            get
            {
                return cmbLeftClass.SelectedItem?.ToString();
            }
            set
            {
                cmbLeftClass.SelectedItem = value;
                cmbLeftClass_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public string SelectedRightClass
        {
            get
            {
                return cmbRightClass.SelectedItem?.ToString();
            }
            set
            {
                cmbRightClass.SelectedItem = value;
                cmbRightClass_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        public List<string> LeftProperties
        {
            get
            {
                List<string> props = new List<string>();
                foreach (object obj in chkLeftProperties.CheckedItems)
                {
                    props.Add(obj.ToString());
                }
                return props;
            }
            set
            {
                chkLeftProperties.Items.Clear();
                foreach (string prop in value)
                {
                    chkLeftProperties.Items.Add(prop, false);
                }
                COL_LEFT.DataSource = new List<string>(value);
            }
        }

        public List<string> RightProperties
        {
            get
            {
                List<string> props = new List<string>();
                foreach (object obj in chkRightProperties.CheckedItems)
                {
                    props.Add(obj.ToString());
                }
                return props;
            }
            set
            {
                chkRightProperties.Items.Clear();
                foreach (string prop in value)
                {
                    chkRightProperties.Items.Add(prop, false);
                }
                COL_RIGHT.DataSource = new List<string>(value);
            }
        }

        public int BatchSize
        {
            get
            {
                return Convert.ToInt32(numBatchSize.Value);
            }
            set
            {
                numBatchSize.Value = value;
            }
        }

        public bool BatchEnabled
        {
            get
            {
                return numBatchSize.Enabled;
            }
            set
            {
                numBatchSize.Enabled = value;
            }
        }

        public void ClearJoins()
        {
            grdJoin.Rows.Clear();
        }

        public void AddPropertyJoin(string left, string right)
        {
            grdJoin.Rows.Add(new object[] { left, right });
        }

        public void RemoveJoin(string left)
        {
            int idx = -1;
            foreach (DataGridViewRow row in grdJoin.Rows)
            {
                if (row.Cells[0].Value.ToString() == left)
                {
                    idx = row.Index;
                }
            }

            if (idx >= 0)
                grdJoin.Rows.RemoveAt(idx);
        }

        public NameValueCollection GetJoinedProperties()
        {
            NameValueCollection joinPairs = new NameValueCollection();
            foreach (DataGridViewRow row in grdJoin.Rows)
            {
                joinPairs.Add(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
            }
            return joinPairs;
        }

        private void cmbLeftConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ConnectionChanged(JoinSourceType.Left);
        }

        private void cmbRightConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ConnectionChanged(JoinSourceType.Right);
        }

        private void cmbTargetConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ConnectionChanged(JoinSourceType.Target);
        }

        private void cmbLeftSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SchemaChanged(JoinSourceType.Left);
        }

        private void cmbRightSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SchemaChanged(JoinSourceType.Right);
        }

        private void cmbTargetSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SchemaChanged(JoinSourceType.Target);
        }

        private void cmbLeftClass_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ClassChanged(JoinSourceType.Left);
        }

        private void cmbRightClass_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ClassChanged(JoinSourceType.Right);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _presenter.SaveTask();
                base.Close();
            }
            catch (TaskValidationException ex)
            {
                this.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
        }

        public string TaskName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public string SelectedTargetClass
        {
            get { return txtTargetClass.Text; }
            set { txtTargetClass.Text = value; }
        }

        private void chkGeometryProperty_CheckedChanged(object sender, EventArgs e)
        {
            _presenter.GeometryPropertyCheckChanged();
            if (chkGeometryProperty.Checked)
            {
                if (rdLeftGeom.Checked)
                    rdLeftGeom_CheckedChanged(this, EventArgs.Empty);
                else if (rdRightGeom.Checked)
                    rdRightGeom_CheckedChanged(this, EventArgs.Empty);
            }
        }

        public string LeftPrefix
        {
            get { return txtLeftPrefix.Text; }
            set { txtLeftPrefix.Text = value; }
        }

        public string RightPrefix
        {
            get { return txtRightPrefix.Text; }
            set { txtRightPrefix.Text = value; }
        }

        public bool TargetGeometryPropertyEnabled
        {
            get { return chkGeometryProperty.Checked; }
            set { chkGeometryProperty.Checked = value; }
        }

        public bool LeftGeometryEnabled
        {
            get { return rdLeftGeom.Enabled; }
            set { rdLeftGeom.Enabled = value; }
        }

        public string LeftGeometryName
        {
            get { return rdLeftGeom.Text; }
            set { rdLeftGeom.Text = value; }
        }

        public bool LeftGeometryChecked
        {
            get { return rdLeftGeom.Checked; }
            set { rdLeftGeom.Checked = value; }
        }

        public bool RightGeometryEnabled
        {
            get { return rdRightGeom.Enabled; }
            set { rdRightGeom.Enabled = value; }
        }

        public string RightGeometryName
        {
            get { return rdRightGeom.Text; }
            set { rdRightGeom.Text = value; }
        }

        public bool RightGeometryChecked
        {
            get { return rdRightGeom.Checked; }
            set { rdRightGeom.Checked = value; }
        }

        private void btnAddJoin_Click(object sender, EventArgs e)
        {
            grdJoin.Rows.Add();
        }

        private void grdJoin_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDeleteJoin.Enabled = true;
        }

        private void btnDeleteJoin_Click(object sender, EventArgs e)
        {
            int rowIndex = -1;
            if (grdJoin.SelectedRows.Count == 1)
                rowIndex = grdJoin.SelectedRows[0].Index;
            else if (grdJoin.SelectedCells.Count == 1)
                rowIndex = grdJoin.SelectedCells[0].RowIndex;
            
            if (rowIndex >= 0)
                grdJoin.Rows.RemoveAt(rowIndex);
        }

        public bool DependsOnConnection(FdoToolbox.Core.Feature.FdoConnection conn)
        {
            IFdoConnectionManager connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
            FdoConnection left = connMgr.GetConnection(this.SelectedLeftConnection);
            FdoConnection right = connMgr.GetConnection(this.SelectedRightConnection);
            FdoConnection target = connMgr.GetConnection(this.SelectedTargetConnection);

            return conn == left || conn == right || conn == target;
        }

        public void CheckLeftProperties(ICollection<string> properties)
        {
            foreach (int idx in chkLeftProperties.CheckedIndices)
            {
                chkLeftProperties.SetItemChecked(idx, false);
            }

            foreach (string prop in properties)
            {
                int idx = chkLeftProperties.Items.IndexOf(prop);
                if (idx >= 0)
                    chkLeftProperties.SetItemChecked(idx, true);
            }
        }

        public void CheckRightProperties(ICollection<string> properties)
        {
            foreach (int idx in chkRightProperties.CheckedIndices)
            {
                chkRightProperties.SetItemChecked(idx, false);
            }

            foreach (string prop in properties)
            {
                int idx = chkRightProperties.Items.IndexOf(prop);
                if (idx >= 0)
                    chkRightProperties.SetItemChecked(idx, true);
            }
        }

        private void rdLeftGeom_CheckedChanged(object sender, EventArgs e)
        {
            if (rdLeftGeom.Checked)
            {
                int idx = chkLeftProperties.Items.IndexOf(rdLeftGeom.Text);
                if (idx >= 0)
                    chkLeftProperties.SetItemChecked(idx, true);
            }
        }

        private void rdRightGeom_CheckedChanged(object sender, EventArgs e)
        {
            if (rdRightGeom.Checked)
            {
                int idx = chkRightProperties.Items.IndexOf(rdRightGeom.Text);
                if (idx >= 0)
                    chkRightProperties.SetItemChecked(idx, true);
            }
        }

        private void PromptLeftFilter()
        {
            var connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            var conn = connMgr.GetConnection(this.SelectedLeftConnection);

            using (var svc = conn.CreateFeatureService())
            {
                using (ClassDefinition cd = svc.GetClassByName(this.SelectedLeftSchema, this.SelectedLeftClass))
                {
                    var expr = ExpressionEditor.EditExpression(conn, cd, null, txtLeftFilter.Text, ExpressionMode.Filter);
                    if (expr != null)
                        txtLeftFilter.Text = expr;
                }
            }
        }

        private void PromptRightFilter()
        {
            var connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            var conn = connMgr.GetConnection(this.SelectedRightConnection);

            using (var svc = conn.CreateFeatureService())
            {
                using (ClassDefinition cd = svc.GetClassByName(this.SelectedRightSchema, this.SelectedRightClass))
                {
                    var expr = ExpressionEditor.EditExpression(conn, cd, null, txtRightFilter.Text, ExpressionMode.Filter);
                    if (expr != null)
                        txtRightFilter.Text = expr;
                }
            }
        }

        public string LeftFilter
        {
            get
            {
                return txtLeftFilter.Text;
            }
            set
            {
                txtLeftFilter.Text = value;
            }
        }

        public string RightFilter
        {
            get
            {
                return txtRightFilter.Text;
            }
            set
            {
                txtRightFilter.Text = value;
            }
        }

        private void txtLeftFilter_Click(object sender, EventArgs e)
        {
            PromptLeftFilter();
        }

        private void txtRightFilter_Click(object sender, EventArgs e)
        {
            PromptRightFilter();
        }
    }
}
