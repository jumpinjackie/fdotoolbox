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
using System.Windows.Forms;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Forms
{
    internal partial class SchemaInfoDialog : Form
    {
        public SchemaInfoDialog()
        {
            InitializeComponent();
            this.Text = ResourceService.GetString("TITLE_SCHEMA_INFORMATION");
        }

        public static OSGeo.FDO.Schema.FeatureSchema NewSchema()
        {
            SchemaInfoDialog dlg = new SchemaInfoDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string name = dlg.txtName.Text;
                string desc = dlg.txtDescription.Text;
                return new OSGeo.FDO.Schema.FeatureSchema(name, desc);
            }
            return null;
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
                errorProvider1.SetError(txtName, ResourceService.GetString("ERR_FIELD_REQUIRED"));
                return;
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}