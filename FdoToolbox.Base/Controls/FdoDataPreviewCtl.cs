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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A view that allows the query and preview of FDO feature data
    /// </summary>
    internal partial class FdoDataPreviewCtl : ViewContent, IFdoDataPreviewView, IConnectionDependentView
    {
        private FdoDataPreviewPresenter _presenter;

        internal FdoDataPreviewCtl()
        {
            InitializeComponent();
            ImageList list = new ImageList();
            list.Images.Add(ResourceService.GetBitmap("table"));
            list.Images.Add(ResourceService.GetBitmap("map"));
            resultTab.ImageList = list;
            TAB_GRID.ImageIndex = 0;
            TAB_MAP.ImageIndex = 1;
        }

        private string _connName;

        public FdoDataPreviewCtl(FdoConnection conn, string connName) : this()
        {
            _connName = connName;
            _presenter = new FdoDataPreviewPresenter(this, conn);
        }

        private string _initSchema;
        private string _initClass;

        public FdoDataPreviewCtl(FdoConnection conn, string connName, string initialSchema, string initialClass)
            : this(conn, connName)
        {
            _initSchema = initialSchema;
            _initClass = initialClass;
        }

        private int? _initPreviewLimit = null;
        private bool _autoExecOnLoad = false;

        public void SetInitialPreviewLimit(int limit)
        {
            _initPreviewLimit = limit;
            _autoExecOnLoad = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init(_initSchema, _initClass);
            base.OnLoad(e);
            if (_autoExecOnLoad && _initPreviewLimit.HasValue)
            {
                _presenter.SetInitialPreviewLimit(_initPreviewLimit.Value);
                cmbQueryMode.SelectedItem = QueryMode.Standard;
                _presenter.ExecuteQuery();
            }
        }

        public override string Title
        {
            get { return ResourceService.GetString("TITLE_DATA_PREVIEW") + " - " + _connName; }
        }

        public List<QueryMode> QueryModes
        {
            set 
            { 
                cmbQueryMode.ComboBox.Items.Clear();
                foreach (QueryMode mode in value)
                {
                    cmbQueryMode.ComboBox.Items.Add(mode);
                }
                if (value.Count > 0)
                {
                    cmbQueryMode.SelectedIndex = 0;
                }
            }
        }

        public QueryMode SelectedQueryMode
        {
            get { return (QueryMode)cmbQueryMode.ComboBox.SelectedItem; }
        }

        public IQuerySubView QueryView
        {
            get
            {
                if (queryPanel.Controls.Count > 0)
                    return (IQuerySubView)queryPanel.Controls[0];
                return null;
            }
            set 
            {
                queryPanel.Controls.Clear();
                value.ContentControl.Dock = DockStyle.Fill;
                queryPanel.Controls.Add(value.ContentControl);
            }
        }

        private void cmbQueryMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.QueryModeChanged();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            _presenter.ExecuteQuery();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _presenter.CancelCurrentQuery();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _presenter.Clear();
        }

        public bool CancelEnabled
        {
            get
            {
                return btnCancel.Enabled;
            }
            set
            {
                btnCancel.Enabled = value;
            }
        }

        public bool ExecuteEnabled
        {
            get
            {
                return btnQuery.Enabled;
            }
            set
            {
                btnQuery.Enabled = value;
            }
        }

        public string ElapsedMessage
        {
            set 
            { 
                lblElapsedTime.Text = value;
            }
        }

        public string StatusMessage
        {
            set 
            { 
                lblMessage.Text = value; 
            }
        }

        public bool ClearEnabled
        {
            get
            {
                return btnClear.Enabled;
            }
            set
            {
                btnClear.Enabled = value;
            }
        }

        private FdoFeatureTable _table;

        public FdoFeatureTable ResultTable
        {
            get
            {
                return _table;
            }
            set
            {
                if (value == null)
                {
                    _table = null;
                    grdResults.DataSource = null;
                    grdResults.Columns.Clear();
                    grdResults.Rows.Clear();
                    lblElapsedTime.Text = string.Empty;
                }
                else
                {
                    _table = value;
                    grdResults.DataSource = _table;
                    mapCtl.DataSource = _table;
                }
            }
        }

        public bool MapEnabled
        {
            set
            {
                if (value)
                {
                    if (!resultTab.TabPages.Contains(TAB_MAP))
                        resultTab.TabPages.Add(TAB_MAP);
                }
                else
                {
                    resultTab.TabPages.Remove(TAB_MAP);
                }
            }
        }

        public void DisplayError(Exception exception)
        {
            this.ShowError(exception);
        }

        private void saveSdf_Click(object sender, EventArgs e)
        {
            FdoFeatureTable table = this.ResultTable;
            if (table == null || table.Rows.Count == 0)
            {
                this.ShowError(ResourceService.GetString("ERR_NO_RESULT_TABLE_TO_SAVE"));
                return;
            }
            string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_QUERY_RESULT"), ResourceService.GetString("FILTER_SDF_FILE"));
            SaveQueryResult(table, file);
        }

        private void saveSQLite_Click(object sender, EventArgs e)
        {
            FdoFeatureTable table = this.ResultTable;
            if (table == null || table.Rows.Count == 0)
            {
                this.ShowError(ResourceService.GetString("ERR_NO_RESULT_TABLE_TO_SAVE"));
                return;
            }
            string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_QUERY_RESULT"), ResourceService.GetString("FILTER_SQLITE"));
            SaveQueryResult(table, file);
        }

        private static void SaveQueryResult(FdoFeatureTable table, string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                //Ask for class name
                if (string.IsNullOrEmpty(table.TableName))
                {
                    string name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_SAVE_QUERY_AS"), ResourceService.GetString("MSG_SAVE_QUERY_AS"), "QueryResult");
                    while (name != null && name.Trim() == string.Empty)
                        name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_SAVE_QUERY_AS"), ResourceService.GetString("MSG_SAVE_QUERY_AS"), "QueryResult");

                    if (name == null)
                        return;

                    table.TableName = name;
                }

                using (TableToFlatFile proc = new TableToFlatFile(table, file))
                {
                    EtlProcessCtl ctl = new EtlProcessCtl(proc);
                    Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
                }
            }
        }

        public bool DependsOnConnection(FdoConnection conn)
        {
            return _presenter.ConnectionMatch(conn);
        }

        public void SetBusyCursor(bool busy)
        {
            this.Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            _presenter.DoInsert();
        }


        public bool InsertEnabled
        {
            get
            {
                return btnInsert.Enabled;
            }
            set
            {
                btnInsert.Enabled = value;
            }
        }

        public bool DeleteEnabled
        {
            get
            {
                return deleteThisFeatureToolStripMenuItem.Enabled;
            }
            set
            {
                deleteThisFeatureToolStripMenuItem.Enabled = value;
            }
        }

        private void deleteThisFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (grdResults.SelectedRows.Count == 1)
            {
                if (this.Confirm("Delete Feature", "Are you sure you want to delete this feature?"))
                {
                    FdoFeature feat = (FdoFeature)((grdResults.SelectedRows[0].DataBoundItem as DataRowView).Row);
                    _presenter.DoDelete(feat);
                }
            }
            else if (grdResults.SelectedRows.Count > 1)
            {
                if (this.Confirm("Delete Feature", "Are you sure you want to delete these features?"))
                {
                    List<FdoFeature> features = new List<FdoFeature>();
                    foreach (DataGridViewRow row in grdResults.SelectedRows)
                    {
                        features.Add((FdoFeature)(row.DataBoundItem as DataRowView).Row);
                    }
                    _presenter.DoDelete(features.ToArray());
                }
            }
        }

        private void updateThisFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (grdResults.SelectedRows.Count == 1)
            {
                FdoFeature feat = (FdoFeature)((grdResults.SelectedRows[0].DataBoundItem as DataRowView).Row);
                _presenter.DoUpdate(feat);
            }
            else if (grdResults.SelectedRows.Count > 1)
            {
                List<FdoFeature> features = new List<FdoFeature>();
                foreach (DataGridViewRow row in grdResults.SelectedRows)
                {
                    features.Add((FdoFeature)(row.DataBoundItem as DataRowView).Row);
                }
                string filter = _presenter.GenerateFilter(features.ToArray());
                FdoBulkUpdateCtl ctl = new FdoBulkUpdateCtl(_presenter.Connection, _presenter.SelectedClassName, filter);
                Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
            }
        }

        private void grdResults_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            //{
            //    System.Windows.Forms.DataGridView.HitTestInfo hit = grdResults.HitTest(e.X, e.Y);
            //    if (hit.Type == DataGridViewHitTestType.Cell && hit.ColumnIndex >= 0 && hit.RowIndex >= 0)
            //    {
            //        grdResults.ClearSelection();
            //        grdResults.Rows[hit.RowIndex].Selected = true;
            //    }
            //}
        }

        private void grdResults_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if(e.RowIndex >= 0 && e.ColumnIndex >= 0)
            //    grdResults.CurrentCell = grdResults.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }


        public bool UpdateEnabled
        {
            get
            {
                return updateThisFeatureToolStripMenuItem.Enabled;
            }
            set
            {
                updateThisFeatureToolStripMenuItem.Enabled = value;
            }
        }
    }
}
