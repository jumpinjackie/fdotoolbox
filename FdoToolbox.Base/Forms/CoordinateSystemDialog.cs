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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Forms
{
    /// <summary>
    /// A coordinate system data entry dialog
    /// </summary>
    internal partial class CoordinateSystemDialog : Form
    {
        private readonly CoordSysCatalog catalog;

        internal CoordinateSystemDialog()
        {
            InitializeComponent();
            catalog = ServiceManager.Instance.GetService<CoordSysCatalog>();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider1.SetError(txtName, "Required");
                return;
            }
            if (string.IsNullOrEmpty(txtWKT.Text))
            {
                errorProvider1.SetError(txtWKT, "Required");
                return;
            }
            if (!_editing)
            {
                if (catalog.ProjectionExists(txtName.Text))
                {
                    errorProvider1.SetError(txtName, "A coordinate system of that name already exists");
                    return;
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private static bool _editing = false;

        public static CoordinateSystemDefinition NewCoordinateSystem()
        {
            CoordinateSystemDialog diag = new CoordinateSystemDialog();
            _editing = false;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return new CoordinateSystemDefinition(diag.txtName.Text, diag.txtDescription.Text, diag.txtWKT.Text);
            }
            return null;
        }

        public static bool EditCoordinateSystem(CoordinateSystemDefinition cs)
        {
            CoordinateSystemDialog diag = new CoordinateSystemDialog();
            _editing = true;
            diag.txtName.Text = cs.Name;
            diag.txtName.Enabled = false;
            diag.txtDescription.Text = cs.Description;
            diag.txtWKT.Text = cs.Wkt;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                cs.Name = diag.txtName.Text;
                cs.Description = diag.txtDescription.Text;
                cs.Wkt = diag.txtWKT.Text;
                return true;
            }
            return false;
        }
    }
}