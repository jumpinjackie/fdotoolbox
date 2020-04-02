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
using FdoToolbox.Core.Feature;
using FdoToolbox.DataStoreManager.Controls.SchemaDesigner;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.DataStoreManager.Controls
{
    public partial class FdoSpatialContextView : CollapsiblePanel
    {
        public FdoSpatialContextView()
        {
            InitializeComponent();
        }

        private SchemaDesignContext _context;

        internal SchemaDesignContext Context
        {
            get { return _context; }
            set
            {
                //Stupid WinForms designer!
                if (value == null)
                    return;

                _context = value;
                
                grdSpatialContexts.DataSource = _context.SpatialContexts;
                btnAdd.Enabled = (_context.CanHaveMultipleSpatialContexts && _context.SpatialContexts.Count == 0);
            }
        }

        internal bool CanDelete
        {
            get { return grdSpatialContexts.SelectedRows.Count == 1 && _context.CanDestroySpatialContexts; }
        }

        public event EventHandler UpdateState;

        private void FlagUpdateState()
        {
            var handler = this.UpdateState;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var sc = FdoSpatialContextDialog.CreateNew(_context.Connection);
            if (sc != null)
            {
                _context.AddSpatialContext(sc);
                FlagUpdateState();
            }
        }

        internal void EvaluateCommandStates()
        {
            btnAdd.Enabled = _context.CanCreateSpatialContexts;
            btnEdit.Enabled = _context.CanEditSpatialContexts;
            btnDelete.Enabled = _context.CanDestroySpatialContexts;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (grdSpatialContexts.SelectedRows.Count == 1)
            {
                var sc = (SpatialContextInfo)grdSpatialContexts.SelectedRows[0].DataBoundItem;
                sc = FdoSpatialContextDialog.Edit(_context.Connection, sc);
                if (sc != null)
                {
                    _context.UpdateSpatialContext(sc);
                    FlagUpdateState();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (grdSpatialContexts.SelectedRows.Count == 1)
            {
                var sc = (SpatialContextInfo)grdSpatialContexts.SelectedRows[0].DataBoundItem;
                if (sc != null)
                {
                    _context.RemoveSpatialContext(sc);
                }
            }
        }

        private void grdSpatialContexts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            grdSpatialContexts.ClearSelection();
            if (e.RowIndex >= 0)
            {
                grdSpatialContexts.Rows[e.RowIndex].Selected = true;
            }

            btnEdit.Enabled = grdSpatialContexts.SelectedRows.Count == 1 && _context.CanEditSpatialContexts;
        }
    }
}
