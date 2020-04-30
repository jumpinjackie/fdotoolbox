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
using System.ComponentModel;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.CoordinateSystems;

namespace FdoToolbox.Base.Forms
{
    internal partial class CoordinateSystemBrowserDialog : Form, ICoordinateSystemBrowserView
    {
        private CoordinateSystemBrowserDialogPresenter _presenter;

        internal CoordinateSystemBrowserDialog()
        {
            InitializeComponent();
            _presenter = new CoordinateSystemBrowserDialogPresenter(this, ServiceManager.Instance.GetService<ICoordinateSystemCatalog>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        private void grdContexts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _presenter.CoordinateSystemSelected();
        }

        public BindingList<CoordinateSystemDefinition> CoordinateSystems
        {
            set { grdContexts.DataSource = value; }
        }

        public CoordinateSystemDefinition SelectedCS
        {
            get 
            {
                CoordinateSystemDefinition cs = null;
                if (grdContexts.SelectedRows.Count == 1)
                    cs = grdContexts.SelectedRows[0].DataBoundItem as CoordinateSystemDefinition;
                else if (grdContexts.SelectedCells.Count == 1)
                    cs = grdContexts.Rows[grdContexts.SelectedCells[0].RowIndex].DataBoundItem as CoordinateSystemDefinition;
                return cs;
            }
        }

        public bool OkEnabled
        {
            set { btnOK.Enabled = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public static CoordinateSystemDefinition GetCoordinateSystem()
        {
            CoordinateSystemBrowserDialog diag = new CoordinateSystemBrowserDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.SelectedCS;
            }
            return null;
        }
    }
}