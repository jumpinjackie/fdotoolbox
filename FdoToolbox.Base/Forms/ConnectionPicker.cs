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
using System.Windows.Forms;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Forms
{
    /// <summary>
    /// A dialog to select an open connection
    /// </summary>
    public partial class ConnectionPicker : Form
    {
        internal ConnectionPicker()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            FdoConnectionManager connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            lstConnections.DataSource = new List<string>(connMgr.GetConnectionNames());
            base.OnLoad(e);
        }

        private void lstConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (lstConnections.SelectedIndex >= 0);
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionName()
        {
            ConnectionPicker dlg = new ConnectionPicker();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.lstConnections.SelectedItem.ToString();
            }
            return string.Empty;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}