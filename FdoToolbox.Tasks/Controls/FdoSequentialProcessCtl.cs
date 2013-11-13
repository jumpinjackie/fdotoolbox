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
using FdoToolbox.Core.Configuration;
using FdoToolbox.Base.Controls;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoSequentialProcessCtl : ViewContent
    {
        public FdoSequentialProcessCtl()
        {
            InitializeComponent();
            _taskMgr = ServiceManager.Instance.GetService<TaskManager>();
            if (_taskMgr != null)
            {
                this.Disposed += new EventHandler(OnDisposed);
                _taskMgr.BeforeTaskRemoved += OnBeforeTaskRemoved;
            }
        }

        private SequentialProcessDefinition _def;

        private string _taskName;

        public FdoSequentialProcessCtl(string name, SequentialProcessDefinition def)
            : this()
        {
            _def = def;
            _taskName = name;
            txtName.Text = name;
            txtName.ReadOnly = true;
        }

        private TaskManager _taskMgr;

        protected override void OnLoad(EventArgs e)
        {
            
            if (_def != null && _def.Operations != null)
            {
                foreach (var op in _def.Operations)
                {
                    lstProcesses.Items.Add(op);
                }
            }
        }

        void OnDisposed(object sender, EventArgs e)
        {
            if (_taskMgr != null)
            {
                _taskMgr.BeforeTaskRemoved -= OnBeforeTaskRemoved;
            }
        }

        void OnBeforeTaskRemoved(object sender, TaskBeforeRemoveEventArgs e)
        {
            if (e.TaskName == _taskName && _taskMgr.GetTask(e.TaskName) != null)
            {
                MessageService.ShowMessage("This editor for this task is still open. Close that editor first");
                e.Cancel = true;
            }
        }

        private void lstProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnMoveUp.Enabled = btnMoveDown.Enabled = btnDeleteProcess.Enabled = lstProcesses.SelectedItem != null;
            
            if (lstProcesses.SelectedItem != null)
            {
                propGrid.SelectedObject = lstProcesses.SelectedItem;
            }
        }

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            var diag = new NewProcessDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                var op = new SequentialOperation() { Command = diag.Command };
                lstProcesses.Items.Add(op);
                propGrid.SelectedObject = op;
            }
        }

        private void btnDeleteProcess_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItem != null)
            {
                lstProcesses.Items.Remove(lstProcesses.SelectedItem);
                propGrid.SelectedObject = null;
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItem != null)
            {
                if (lstProcesses.SelectedIndex > 0)
                {
                    var idx = lstProcesses.SelectedIndex - 1;
                    var obj = lstProcesses.SelectedItem;
                    lstProcesses.Items.Remove(obj);
                    lstProcesses.Items.Insert(idx, obj);
                    lstProcesses.SelectedItem = obj;
                }
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItem != null)
            {
                if (lstProcesses.SelectedIndex < lstProcesses.Items.Count - 1)
                {
                    var idx = lstProcesses.SelectedIndex + 1;
                    var obj = lstProcesses.SelectedItem;
                    lstProcesses.Items.Remove(obj);
                    lstProcesses.Items.Insert(idx, obj);
                    lstProcesses.SelectedItem = obj;
                }
            }
        }

        public override string Title
        {
            get
            {
                return "Sequential Process";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (new TempCursor(Cursors.WaitCursor))
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    MessageService.ShowError("Name required");
                    return;
                }

                TaskManager tmgr = ServiceManager.Instance.GetService<TaskManager>();
                LoggingService.Info("Updating loaded task. Please wait.");
                List<SequentialOperation> ops = new List<SequentialOperation>();
                foreach (var obj in lstProcesses.Items)
                {
                    ops.Add((SequentialOperation)obj);
                }
                if (_def == null) //is new
                {
                    _def = new SequentialProcessDefinition();
                    foreach(var op in ops)
                    {
                        _def.AddOperation(op);
                    }
                    var proc = new FdoSequentialProcess(_def);
                    tmgr.AddTask(txtName.Text, proc);
                }
                else
                {
                    _def.ClearOperations();
                    foreach (var op in ops)
                    {
                        _def.AddOperation(op);
                    }
                }
                this.Close();
            }
        }
    }
}
