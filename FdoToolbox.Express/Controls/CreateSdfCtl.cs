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
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Base;
using FdoToolbox.Base.Controls;

//TODO: Attach validation.

namespace FdoToolbox.Express.Controls
{
    [ToolboxItem(false)]
    public partial class CreateSdfCtl : ViewContent, ICreateSdfView
    {
        private CreateSdfPresenter _presenter;

        public CreateSdfCtl()
        {
            InitializeComponent();
            _presenter = new CreateSdfPresenter(this);
        }

        private void chkConnect_CheckedChanged(object sender, EventArgs e)
        {
            _presenter.CheckConnect();
        }

        private void btnSdf_Click(object sender, EventArgs e)
        {
            txtSdfFile.Text = FileService.SaveFile(ResourceService.GetString("TITLE_CREATE_SDF"), ResourceService.GetString("FILTER_SDF"));
        }

        private void btnSchema_Click(object sender, EventArgs e)
        {
            txtFeatureSchema.Text = FileService.OpenFile(ResourceService.GetString("TITLE_LOAD_SCHEMA"), ResourceService.GetString("FILTER_SCHEMA_FILE"));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool ok = false;
            using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
            {
                ok = _presenter.CheckConnectionName() && _presenter.CreateSdf();
            }
            if (ok)
            {
                this.ShowMessage(ResourceService.GetString("TITLE_CREATE_SDF"), ResourceService.GetString("MSG_SDF_CREATED"));
                base.Close();
            }
        }

        public override string Title => ResourceService.GetString("TITLE_CREATE_SDF");

        public string SdfFile => txtSdfFile.Text;

        public string FeatureSchemaDefinition => txtFeatureSchema.Text;

        public bool CreateConnection => chkConnect.Checked;

        public bool FixIncompatibilities => chkAlterSchema.Checked;

        public bool ConnectionEnabled
        {
            set { txtConnectionName.Enabled = value; }
        }

        public string ConnectionName
        {
            get { return txtConnectionName.Text; }
            set { txtConnectionName.Text = value; }
        }
    }
}
