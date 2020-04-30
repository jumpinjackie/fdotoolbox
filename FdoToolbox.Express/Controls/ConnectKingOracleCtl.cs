#region LGPL Header
// Copyright (C) 2011, Jackie Ng
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
using ICSharpCode.Core;

namespace FdoToolbox.Express.Controls
{
    public partial class ConnectKingOracleCtl : ViewContent, IConnectKingOracleView
    {
        private ConnectKingOraclePresenter _presenter;

        public ConnectKingOracleCtl()
        {
            InitializeComponent();
            _presenter = new ConnectKingOraclePresenter(this);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_presenter.Connect())
                this.Close();
        }

        public override string Title => ResourceService.GetString("TITLE_CONNECT_KINGORACLE");

        #region IConnectKingOracleView Members

        public string Username => txtUsername.Text;

        public string Password => txtPassword.Text;

        public string Service => txtService.Text;

        public string OracleSchema => txtOracleSchema.Text;

        public string KingFdoClass => txtKingFdoClass.Text;

        public string SdeSchema => txtSdeSchema.Text;

        public string ConnectionName => txtConnectionName.Text;

        #endregion
    }
}
