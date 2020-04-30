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
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A view that allows for un-registration of FDO providers from the provider registry
    /// </summary>
    internal partial class FdoUnregProviderCtl : ViewContent, IViewContent, IFdoUnregProviderView
    {
        private FdoUnregProviderPresenter _presenter;

        public FdoUnregProviderCtl()
        {
            InitializeComponent();
            _presenter = new FdoUnregProviderPresenter(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.GetProviders();
            base.OnLoad(e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        public override string Title => ResourceService.GetString("TITLE_UNREGISTER_PROVIDER");

        private void lstProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.SelectionChanged();
        }

        public IList<FdoToolbox.Core.Feature.FdoProviderInfo> ProviderList
        {
            set 
            {
                lstProviders.DisplayMember = "DisplayName";
                lstProviders.DataSource = value; 
            }
        }

        public IList<string> SelectedProviders
        {
            get 
            {
                IList<string> names = new List<string>();
                foreach (object obj in lstProviders.SelectedItems)
                {
                    names.Add((obj as FdoProviderInfo).Name);
                }
                return names;
            }
        }

        public bool UnregEnabled
        {
            set { btnUnregister.Enabled = value; }
        }

        private void btnUnregister_Click(object sender, EventArgs e)
        {
            if (_presenter.Unregister())
            {
                this.ShowMessage(ResourceService.GetString("TITLE_UNREGISTER_PROVIDER"), ResourceService.GetString("MSG_PROVIDER_UNREGISTERED"));
                _presenter.GetProviders();
            }
        }
    }
}
