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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Forms
{
    public partial class FdoSpatialContextBrowserDlg : Form
    {
        private FdoSpatialContextBrowserDlg()
        {
            InitializeComponent();
            this.Text = ResourceService.GetString("TITLE_SPATIAL_CONTEXT_BROWSER");
            lblMessage.Text = ResourceService.GetString("MSG_SELECT_SPATIAL_CONTEXT");
        }

        public FdoSpatialContextBrowserDlg(FdoConnection conn)
            : this()
        {
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                grdSpatialContexts.DataSource = service.GetSpatialContexts();
            }
        }

        private void grdSpatialContexts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnOK.Enabled = grdSpatialContexts.Rows.Count > 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public SpatialContextInfo SelectedSpatialContext
        {
            get
            {
                if (grdSpatialContexts.SelectedRows.Count == 1)
                    return grdSpatialContexts.SelectedRows[0].DataBoundItem as SpatialContextInfo;
                else if (grdSpatialContexts.SelectedCells.Count == 1)
                    return grdSpatialContexts.Rows[grdSpatialContexts.SelectedCells[0].RowIndex].DataBoundItem as SpatialContextInfo;
                else
                    return null;
            }
        }

        public static SpatialContextInfo GetSpatialContext(FdoConnection conn)
        {
            FdoSpatialContextBrowserDlg diag = new FdoSpatialContextBrowserDlg(conn);
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.SelectedSpatialContext;
            }
            return null;
        }
    }
}