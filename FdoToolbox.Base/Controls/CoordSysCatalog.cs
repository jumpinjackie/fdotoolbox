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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A view to maintain a list of coordinate system definitions
    /// </summary>
    internal partial class CoordSysCatalog : ViewContent, IViewContent, ICoordSysCatalogView
    {
        private CoordSysCatalogPresenter _presenter;

        public CoordSysCatalog()
        {
            InitializeComponent();
            _presenter = new CoordSysCatalogPresenter(this, ServiceManager.Instance.GetService<FdoToolbox.Base.Services.CoordSysCatalog>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public override string Title => ResourceService.GetString("TITLE_COORDSYS_CATALOG");

        public CoordinateSystemDefinition SelectedCS
        {
            get
            {
                if (grdCs.SelectedRows.Count == 1)
                    return grdCs.SelectedRows[0].DataBoundItem as CoordinateSystemDefinition;
                else if (grdCs.SelectedCells.Count == 1)
                    return grdCs.Rows[grdCs.SelectedCells[0].RowIndex].DataBoundItem as CoordinateSystemDefinition;
                else
                    return null;
            }
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            txtFilter.Text = string.Empty;
            _presenter.Refresh();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = CoordinateSystemDialog.NewCoordinateSystem();
            if (cs != null)
            {
                _presenter.AddNew(cs);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = this.SelectedCS;
            if (cs != null)
            {
                string oldName = cs.Name;
                if (CoordinateSystemDialog.EditCoordinateSystem(cs))
                {
                    _presenter.Update(oldName, cs);
                    _presenter.Refresh();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = this.SelectedCS;
            if (cs != null)
            {
                _presenter.Delete(cs);
            }
        }

        private BindingList<CoordinateSystemDefinition> _bs;

        public BindingList<CoordinateSystemDefinition> CoordSysDefinitions
        {
            set 
            {
                if (_bs == null)
                    _bs = value;
                grdCs.DataSource = _bs;
            }
        }

        public bool EditEnabled
        {
            set { btnEdit.Enabled = value; }
        }

        public bool DeleteEnabled
        {
            set { btnDelete.Enabled = value; }
        }

        private void grdCs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            _presenter.CheckStatus();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            grdCs.DataSource = _bs.Where(x => x.Name == txtFilter.Text).ToArray();
        }
    }
}
