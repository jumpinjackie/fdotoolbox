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
using System.Windows.Forms;
using OSGeo.FDO.Schema;
using System.Diagnostics;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    public partial class AddMappingDialog : Form
    {
        private BindingList<KeyMapping> _mappings;

        private AddMappingDialog()
        {
            InitializeComponent(); 
        }

        public AddMappingDialog(BindingList<KeyMapping> mappings, ClassDefinition acls, ClassDefinition rcls)
            : this()
        {
            _mappings = mappings;

            lblClass.Text = rcls.QualifiedName;
            lblAssocClass.Text = acls.QualifiedName;

            List<DataPropertyDefinition> aprops = new List<DataPropertyDefinition>();
            List<DataPropertyDefinition> rprops = new List<DataPropertyDefinition>();

            foreach (PropertyDefinition p in acls.Properties)
            {
                if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                    aprops.Add((DataPropertyDefinition)p);
            }

            foreach (PropertyDefinition p in rcls.Properties)
            {
                if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                    rprops.Add((DataPropertyDefinition)p);
            }

            cmbActiveProperty.DataSource = rprops;
            cmbAssociatedProperty.DataSource = aprops;

            cmbActiveProperty.SelectedIndex = 0;
            cmbAssociatedProperty.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var aprop = (DataPropertyDefinition)cmbAssociatedProperty.SelectedItem;
            var rprop = (DataPropertyDefinition)cmbActiveProperty.SelectedItem;

            Debug.Assert(aprop != null);
            Debug.Assert(rprop != null);

            var map = new KeyMapping(rprop.Name, aprop.Name);
            _mappings.Add(map);

            this.DialogResult = DialogResult.OK;
        }

        private void cmbActiveProperty_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = IsValidForSubmission();
        }

        private void cmbAssociatedProperty_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = IsValidForSubmission();
        }

        private bool IsValidForSubmission()
        {
            var aprop = (DataPropertyDefinition)cmbAssociatedProperty.SelectedItem;
            var rprop = (DataPropertyDefinition)cmbActiveProperty.SelectedItem;

            if (aprop == null || rprop == null)
                return false;

            //Verify properties to be mapped are of the same type and neither one
            //is already part of another mapping

            //FDO spec says number order and types must match. Data Types?
            if (aprop.DataType != rprop.DataType)
                return false;

            //Verify not already part of mappings
            foreach (var mp in _mappings)
            {
                if (mp.Primary == rprop.Name)
                    return false;

                if (mp.Foreign == aprop.Name)
                    return false;
            }

            return true;
        }
    }
}
