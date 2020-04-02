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
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A container of <see cref="IPreferenceSheet"/> instances. Each instance is contained
    /// in a tab.
    /// </summary>
    internal partial class PreferencesCtl : ViewContent, IPreferencesView
    {
        private PreferencesCtlPresenter _presenter;

        public PreferencesCtl()
        {
            InitializeComponent();
            _presenter = new PreferencesCtlPresenter(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.LoadPreferences();
            base.OnLoad(e);
        }

        public override string Title
        {
            get { return ResourceService.GetString("TITLE_PREFERENCES"); }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _presenter.SaveChanges();
            this.ShowMessage(null, ResourceService.GetString("MSG_PREFS_SAVED"));
            base.Close();
        }

        private IList<IPreferenceSheet> _sheets;

        public IList<IPreferenceSheet> Sheets
        {
            get
            {
                return _sheets;
            }
            set
            {
                _sheets = value;
                tabOptions.TabPages.Clear();
                foreach (IPreferenceSheet sh in _sheets)
                {
                    TabPage page = new TabPage(sh.Title);
                    sh.ContentControl.Dock = DockStyle.Fill;
                    page.Controls.Add(sh.ContentControl);
                    tabOptions.TabPages.Add(page);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}
