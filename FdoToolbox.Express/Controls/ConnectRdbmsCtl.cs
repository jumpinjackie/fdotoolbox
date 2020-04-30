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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Express.Controls
{
    [ToolboxItem(false)]
    public partial class ConnectRdbmsCtl : ViewContent, IConnectRdbmsView
    {
        private ConnectRdbmsPresenter _presenter;

        public ConnectRdbmsCtl()
        {
            InitializeComponent();
            _presenter = new ConnectRdbmsPresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        public string Service => txtService.Text;

        public string Username => txtUsername.Text;

        public string Password => txtPassword.Text;

        public bool SubmitEnabled
        {
            set { btnOK.Enabled = value; }
        }

        public DataStoreInfo[] DataStores
        {
            set
            {
                cmbDataStore.DataSource = value;
                cmbDataStore.DisplayMember = "Description";
                if (value.Length > 0)
                    cmbDataStore.SelectedIndex = 0;
            }
        }

        public string SelectedDataStore => cmbDataStore.SelectedItem != null ? ((DataStoreInfo)cmbDataStore.SelectedItem).Name : (cmbDataStore.Text ?? string.Empty);

        public string ConnectionName => txtConnectionName.Text;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            _presenter.PendingConnect();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_presenter.Connect())
                this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public virtual string Provider => throw new NotImplementedException();

        public virtual string ServiceParameter => "Service";

        public virtual string UsernameParameter => "Username";

        public virtual string PasswordParameter => "Password";

        public virtual string DataStoreParameter => "DataStore";

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtConfigPath.Text = openFileDialog.FileName;
            }
        }

        public bool ConfigEnabled
        {
            set 
            {
                btnBrowse.Enabled = value;
                txtConfigPath.Enabled = value;
                lblConfig.Enabled = value;
            }
        }

        public string ConfigPath => txtConfigPath.Text;
    }
}
