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
using FdoToolbox.Base;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Express.Controls
{
    [ToolboxItem(false)]
    public partial class ConnectOdbcCtl : ViewContent, IConnectOdbcView
    {
        private ConnectOdbcPresenter _presenter;

        public ConnectOdbcCtl()
        {
            InitializeComponent();
            _presenter = new ConnectOdbcPresenter(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public FdoToolbox.Express.Controls.Odbc.OdbcType[] OdbcTypes
        {
            set { cmbDataType.DataSource = value; }
        }

        public FdoToolbox.Express.Controls.Odbc.OdbcType SelectedOdbcType
        {
            get { return (FdoToolbox.Express.Controls.Odbc.OdbcType)cmbDataType.SelectedItem; }
        }

        public FdoToolbox.Express.Controls.Odbc.IOdbcConnectionBuilder BuilderObject
        {
            get
            {
                return (FdoToolbox.Express.Controls.Odbc.IOdbcConnectionBuilder)propGrid.SelectedObject;
            }
            set
            {
                propGrid.SelectedObject = value;
            }
        }

        public override string Title
        {
            get { return ResourceService.GetString("TITLE_CONNECT_ODBC"); }
        }

        private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.OdbcTypeChanged();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _presenter.TestConnection();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_presenter.Connect())
                base.Close();
        }

        public string ConnectionName
        {
            get { return txtName.Text; }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtConfiguration.Text = FileService.OpenFile(ResourceService.GetString("TITLE_LOAD_CONFIGURATION"), ResourceService.GetString("FILTER_XML_FILES"));
        }
        
        public string ConfigurationFile
        {
            get { return txtConfiguration.Text; }
        }
    }
}
