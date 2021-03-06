﻿#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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
using FdoToolbox.Base.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    public partial class NewMultiTaskDialog : Form
    {
        private string[] _availNames;
        private FdoConnectionManager _connMgr;

        public NewMultiTaskDialog(string[] availableConnectionNames)
        {
            InitializeComponent();
            _availNames = availableConnectionNames;
        }

        protected override void OnLoad(EventArgs e)
        {
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            List<string> srcNames = new List<string>(_availNames);
            List<string> dstNames = new List<string>(_availNames);

            cmbSrcConn.DataSource = srcNames;
            cmbTargetConn.DataSource = dstNames;

            cmbSrcConn.SelectedIndex = 0;
            cmbTargetConn.SelectedIndex = 0;

            cmbSrcConn_SelectionChangeCommitted(this, EventArgs.Empty);
            cmbTargetConn_SelectionChangeCommitted(this, EventArgs.Empty);

            base.OnLoad(e);
        }

        private void EnableCell(DataGridViewCell dc, bool enabled)
        {
            //toggle read-only state
            dc.ReadOnly = !enabled;
            if (enabled)
            {
                //restore cell style to the default value
                dc.Style.BackColor = dc.OwningColumn.DefaultCellStyle.BackColor;
                dc.Style.ForeColor = dc.OwningColumn.DefaultCellStyle.ForeColor;
            }
            else
            {
                //gray out the cell
                dc.Style.BackColor = Color.LightGray;
                dc.Style.ForeColor = Color.DarkGray;
            }
        }

        public IEnumerable<CopyTaskDef> GetCopyTasks() => _copyTasks;

        private void btnCancel_Click(object sender, EventArgs e) => this.DialogResult = DialogResult.Cancel;

        readonly List<CopyTaskDef> _copyTasks = new List<CopyTaskDef>();

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgCopyTasks.Rows.Count == 0)
            {
                MessageBox.Show("You have not specified any copy tasks");
                return;
            }

            bool hasErrors = false;
            foreach (DataGridViewRow row in dgCopyTasks.Rows)
            {
                if (!row.IsNewRow && !ValidateRow(row))
                {
                    hasErrors = true;
                }
            }

            if (!hasErrors)
            {
                _copyTasks.Clear();
                foreach (DataGridViewRow row in dgCopyTasks.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    var name = row.Cells[nameof(this.TaskName)].Value.ToString();
                    var qcls = row.Cells[nameof(this.SourceClass)].Value.ToString();
                    var qdst = row.Cells[nameof(this.TargetClass)].Value?.ToString();
                    var autoCreate = row.Cells[nameof(this.AutoCreateClass)].Value?.ToString();
                    var autoCreateInSchema = row.Cells[nameof(this.AutoCreateInSchema)].Value?.ToString();
                    var ovTargetClass = row.Cells[nameof(this.CustomTargetClass)].Value?.ToString();

                    var tokens = qcls.Split(':');
                    var sSchema = tokens[0];
                    var sClass = tokens[1];

                    string dSchema = null;
                    string dClass = null;

                    bool bAutoCreate = (autoCreate == "true");

                    if (!string.IsNullOrEmpty(qdst))
                    {
                        tokens = qdst.Split(':');
                        dSchema = tokens[0];
                        dClass = tokens[1];
                    }
                    else
                    {
                        if (bAutoCreate)
                        {
                            dSchema = autoCreateInSchema;
                        }
                    }

                    var cdef = new CopyTaskDef
                    {
                        SourceConnectionName = cmbSrcConn.SelectedItem.ToString(),
                        TargetConnectionName = cmbTargetConn.SelectedItem.ToString(),
                        CreateIfNotExist = bAutoCreate,
                        SourceSchema = sSchema,
                        SourceClass = sClass,
                        TargetSchema = dSchema,
                        TargetClass = dClass,
                        TargetClassNameOverride = !string.IsNullOrWhiteSpace(ovTargetClass) ? ovTargetClass : null,
                        TaskName = name
                    };
                    _copyTasks.Add(cdef);
                }

                //Final checks
                var errors = new List<string>();
                /*
                var dupeTargetClasses = _copyTasks.GroupBy(t => t.EffectiveTargetClass)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToArray();
                */
                var dupeTaskNames = _copyTasks.GroupBy(t => t.TaskName)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToArray();

                //if (dupeTargetClasses.Length > 0)
                //    errors.Add($"The following target class names are specified more than once: {string.Join(", ", dupeTargetClasses)}");
                if (dupeTaskNames.Length > 0)
                    errors.Add($"The following task names are specified more than once: {string.Join(", ", dupeTaskNames)}");

                if (errors.Count > 0)
                {
                    MessageBox.Show($"The following problems were encountered. Please correct them before retrying:{Environment.NewLine}  - {string.Join(Environment.NewLine + "  - ", errors)}");
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }
        }

        readonly List<string> _srcClassNames = new List<string>();
        readonly List<string> _dstClassNames = new List<string>();
        readonly List<string> _dstSchemaNames = new List<string>();

        private void btnAcceptConnections_Click(object sender, EventArgs e)
        {
            cmbSrcConn.Enabled = false;
            cmbTargetConn.Enabled = false;
            lblHelp.Visible = false;
            btnAcceptConnections.Visible = false;

            dgCopyTasks.Visible = true;
            btnOK.Visible = true;
            btnCancel.Visible = true;
            btnRemoveRow.Visible = true;

            // Setup DG dropdowns
            var sconn = _connMgr.GetConnection(cmbSrcConn.SelectedItem.ToString());
            var tconn = _connMgr.GetConnection(cmbTargetConn.SelectedItem.ToString());
            using (var ss = sconn.CreateFeatureService())
            using (var ts = tconn.CreateFeatureService())
            {
                _srcClassNames.AddRange(ss.GetQualifiedClassNames());
                _dstClassNames.AddRange(ts.GetQualifiedClassNames());

                this.SourceClass.DataSource = _srcClassNames;
                this.TargetClass.DataSource = _dstClassNames;

                _dstSchemaNames.AddRange(ts.GetSchemaNames());
                this.AutoCreateInSchema.DataSource = _dstSchemaNames;
            }
        }

        private void CheckAcceptButtonReadiness()
        {
            if (cmbSrcConn.SelectedItem != null && cmbTargetConn.SelectedItem != null)
            {
                btnAcceptConnections.Enabled = cmbSrcConn.SelectedItem.ToString() != cmbTargetConn.SelectedItem.ToString();
            }
            else
            {
                btnAcceptConnections.Enabled = false;
            }
        }

        private void cmbSrcConn_SelectionChangeCommitted(object sender, EventArgs e) => this.CheckAcceptButtonReadiness();

        private void cmbTargetConn_SelectionChangeCommitted(object sender, EventArgs e) => this.CheckAcceptButtonReadiness();

        private void dgCopyTasks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dgCopyTasks.Rows[e.RowIndex];
            var autoCreate = row.Cells[nameof(this.AutoCreateClass)].Value?.ToString();
            if (autoCreate == "true")
            {
                EnableCell(row.Cells[nameof(this.TargetClass)], false);
            }
            else
            {
                EnableCell(row.Cells[nameof(this.TargetClass)], true);
            }
            ValidateRow(row);
        }

        private bool ValidateRow(DataGridViewRow row)
        {
            //Don't validate new rows
            if (row.IsNewRow)
                return true;

            var errors = new List<string>();
            var name = row.Cells[nameof(this.TaskName)].Value?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                errors.Add("Task name is required and must be unique");
            }
            var cls = row.Cells[nameof(this.SourceClass)].Value?.ToString();
            if (string.IsNullOrEmpty(cls))
            {
                errors.Add("You must specify a source class");
            }
            var dst = row.Cells[nameof(this.TargetClass)].Value?.ToString();
            var autoCreate = row.Cells[nameof(this.AutoCreateClass)].Value?.ToString();
            var autoCreateInSchema = row.Cells[nameof(this.AutoCreateInSchema)].Value?.ToString();
            //Did they tick auto-create?
            if (autoCreate == "true")
            {
                //Did they specify a target schema?
                if (string.IsNullOrEmpty(autoCreateInSchema))
                {
                    errors.Add("Specify the target schema to auto-create in");
                }
                else
                {
                    if (!string.IsNullOrEmpty(cls))
                    {
                        var tokens = cls.Split(':');
                        var tcn = $"{autoCreateInSchema}:{tokens[1]}";
                        if (_dstClassNames.Contains(tcn))
                        {
                            errors.Add($"Cannot auto-create class {tcn} as this class already exists in the target connection");
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(dst)) // Didn't specify target class
                {
                    errors.Add("Select a target class or tick Auto-create and specify the target schema to auto-create in");
                }
            }

            if (errors.Count > 0)
            {
                string errText = $"The row has the following errors:" + Environment.NewLine + "  - " + string.Join(Environment.NewLine + "  - ", errors);
                row.ErrorText = errText;

                return false;
            }
            else
            {
                row.ErrorText = null;
                return true;
            }
        }

        private void dgCopyTasks_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            btnOK.Enabled = true;
        }

        private void dgCopyTasks_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) => btnOK.Enabled = dgCopyTasks.Rows.Count > 0;

        private int _counter = 1;

        private void dgCopyTasks_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[nameof(this.TaskName)].Value = $"CopyTask{_counter++}";
            if (_dstSchemaNames.Count == 1)
            {
                e.Row.Cells[nameof(this.AutoCreateInSchema)].Value = _dstSchemaNames[0];
            }
            if (_dstClassNames.Count == 1)
            {
                e.Row.Cells[nameof(this.TargetClass)].Value = _dstClassNames[0];
            }
        }

        private void btnRemoveRow_Click(object sender, EventArgs e)
        {
            var toRemove = dgCopyTasks.SelectedRows;
            foreach (DataGridViewRow row in toRemove)
            {
                if (!row.IsNewRow)
                    dgCopyTasks.Rows.Remove(row);
            }
        }

        private void dgCopyTasks_SelectionChanged(object sender, EventArgs e)
        {
            btnRemoveRow.Enabled = dgCopyTasks.SelectedRows.Count >= 1;
        }
    }
}
