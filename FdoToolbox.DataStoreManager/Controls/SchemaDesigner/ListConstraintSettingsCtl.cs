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
using OSGeo.FDO.Expression;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal partial class ListConstraintSettingsCtl : UserControl
    {
        public ListConstraintSettingsCtl()
        {
            InitializeComponent();
            CheckDeleteState();
        }

        private void lstValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDeleteState();
        }

        private void CheckDeleteState()
        {
            btnDelete.Enabled = (lstValues.SelectedItem != null);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            lstValues.Items.RemoveAt(lstValues.SelectedIndex);
            CheckDeleteState();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewValue.Text))
            {
                //TODO: How smart is this list constraint? Is a list of 
                //number strings treated as a list of numbers?
                DataValue val = new StringValue(txtNewValue.Text);
                
                txtNewValue.Text = string.Empty;
                lstValues.Items.Add(val);
            }
        }

        public DataValueCollection ListValues
        {
            get
            {
                DataValueCollection values = new DataValueCollection();
                foreach (object obj in lstValues.Items)
                {
                    values.Add((DataValue)obj);
                }
                return values;
            }
            set
            {
                lstValues.Items.Clear();
                foreach (DataValue val in value)
                {
                    lstValues.Items.Add(val);
                }
            }
        }
    }
}
