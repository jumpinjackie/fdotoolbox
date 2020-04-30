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
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;
using System.Collections.Specialized;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A view that allows for the management of FDO data stores. This is mainly used in a RDBMS context
    /// </summary>
    internal partial class FdoDataStoreMgrCtl : ViewContent, IFdoDataStoreMgrView, IViewContent, IConnectionDependentView
    {
        private FdoDataStoreMgrPresenter _presenter;

        public FdoDataStoreMgrCtl()
        {
            InitializeComponent();
        }

        public FdoDataStoreMgrCtl(FdoConnection conn)
            : this()
        {
            _presenter = new FdoDataStoreMgrPresenter(this, conn);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public bool AddEnabled
        {
            set { btnCreate.Enabled = value; }
        }

        public bool DestroyEnabled
        {
            set { btnDestroy.Enabled = value; }
        }

        public IList<DataStoreInfo> DataStores
        {
            set { grdDataStores.DataSource = value; }
        }

        public override string Title => ResourceService.GetString("TITLE_DATA_STORE_MGMT");

        public string Message
        {
            set { lblMessage.Text = value; }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            NameValueCollection props = DictionaryDialog.GetParameters(ResourceService.GetString("TITLE_CREATE_DATASTORE"), FdoFeatureService.GetCreateDataStoreProperties(_presenter.Connection.Provider));
            if (props != null)
            {
                _presenter.CreateDataStore(props);
                this.ShowMessage(null, ResourceService.GetString("MSG_DATA_STORE_CREATED"));
            }
        }

        private void btnDestroy_Click(object sender, EventArgs e)
        {
            NameValueCollection props = DictionaryDialog.GetParameters(ResourceService.GetString("TITLE_DESTROY_DATASTORE"), FdoFeatureService.GetCreateDataStoreProperties(_presenter.Connection.Provider));
            if (props != null)
            {
                _presenter.DestroyDataStore(props);
                this.ShowMessage(null, ResourceService.GetString("MSG_DATA_STORE_DESTROYED"));
            }
        }

        public bool DependsOnConnection(FdoConnection conn)
        {
            return _presenter.Connection == conn;
        }
    }
}
