#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Express.Controls
{
    [ToolboxItem(false)]
    public partial class CopySpatialContextsCtl : ViewContent, ICopySpatialContextsView
    {
        private CopySpatialContextsCtlPresenter _presenter;

        internal CopySpatialContextsCtl()
        {
            InitializeComponent();
        }

        public CopySpatialContextsCtl(string srcConnName)
            : this()
        {
            _presenter = new CopySpatialContextsCtlPresenter(this, srcConnName, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _presenter.CopySpatialContexts();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void cmbTargetConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.TargetConnectionChanged();
        }

        public string SourceConnectionName
        {
            get
            {
                return lblSrcConn.Text;
            }
            set
            {
                lblSrcConn.Text = value;
            }
        }

        public IList<string> TargetConnectionNames
        {
            set 
            {
                cmbTargetConn.DataSource = value;
            }
        }

        public string SelectedTargetConnectionName
        {
            get { return cmbTargetConn.SelectedItem.ToString(); }
        }

        public bool MultiSelect
        {
            set { lstSpatialContexts.SelectionMode = (value) ? SelectionMode.MultiSimple : SelectionMode.One; }
        }

        public IList<string> SpatialContexts
        {
            set 
            {
                lstSpatialContexts.Items.Clear();
                foreach (string s in value)
                {
                    lstSpatialContexts.Items.Add(s);
                }
            }
            get
            {
                List<string> items = new List<string>();
                foreach (object o in lstSpatialContexts.SelectedItems)
                {
                    items.Add(o.ToString());
                }
                return items;
            }
        }

        public bool Overwrite
        {
            get
            {
                return chkOverwrite.Checked;
            }
            set
            {
                chkOverwrite.Checked = value;
            }
        }

        public bool OverwriteEnabled
        {
            get
            {
                return chkOverwrite.Enabled;
            }
            set
            {
                chkOverwrite.Enabled = value;
            }
        }

        public override string Title
        {
            get { return ResourceService.GetString("TITLE_COPY_SPATIAL_CONTEXTS"); }
        }
    }
}
