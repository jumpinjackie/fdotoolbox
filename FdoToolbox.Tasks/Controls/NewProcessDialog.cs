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

namespace FdoToolbox.Tasks.Controls
{
    public partial class NewProcessDialog : Form
    {
        public NewProcessDialog()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //Whenever more commands are added to FdoUtil.exe, they need to be added
            //here too
            cmbCommands.Items.Add("ApplySchema");
            cmbCommands.Items.Add("Destroy");
            cmbCommands.Items.Add("DumpSchema");
            cmbCommands.Items.Add("CreateFile");
            cmbCommands.Items.Add("CreateDataStore");
            cmbCommands.Items.Add("RegisterProvider");
            cmbCommands.Items.Add("UnregisterProvider");
            cmbCommands.Items.Add("BulkCopy");
            cmbCommands.Items.Add("RunTask");
            cmbCommands.Items.Add("ExecuteSql");
        }

        public string Command
        {
            get { return cmbCommands.Text; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
