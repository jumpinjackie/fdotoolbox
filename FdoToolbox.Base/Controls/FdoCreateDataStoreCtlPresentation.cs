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
using System.Text;
using System.Collections.Specialized;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Connections;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoCreateDataStoreView : IViewContent
    {
        IList<FdoProviderInfo> ProviderList { set; }
        FdoProviderInfo SelectedProvider { get; }

        NameValueCollection ConnectProperties { get; }
        NameValueCollection DataStoreProperties { get; }

        void InitializeConnectGrid();
        void InitializeDataStoreGrid();

        bool CreateEnabled { set; }

        void ResetConnectGrid();
        void AddConnectProperty(DictionaryProperty p);
        void AddEnumerableConnectProperty(string name, string defaultValue, string[] values);

        void ResetDataStoreGrid();
        void AddDataStoreProperty(DictionaryProperty p);
        void AddEnumerableDataStoreProperty(string name, string defaultValue, string[] values);
    }

    internal class FdoCreateDataStorePresenter
    {
        public readonly IFdoCreateDataStoreView _view;
        private readonly IFdoConnectionManager _manager;

        public FdoCreateDataStorePresenter(IFdoCreateDataStoreView view, IFdoConnectionManager manager)
        {
            _view = view;
            _manager = manager;
        }

        public void LoadProviders()
        {
            _view.InitializeConnectGrid();
            _view.InitializeDataStoreGrid();
            _view.ProviderList = FdoFeatureService.GetProviders();
        }

        public void ProviderChanged()
        {
            FdoProviderInfo prov = _view.SelectedProvider;
            if (prov != null)
            {
                _view.ResetDataStoreGrid();

                IList<DictionaryProperty> dprops = FdoFeatureService.GetCreateDataStoreProperties(prov.Name);
                if (dprops != null)
                {
                    foreach (DictionaryProperty p in dprops)
                    {
                        if (p.Enumerable)
                        {
                            EnumerableDictionaryProperty ep = p as EnumerableDictionaryProperty;
                            if(!ep.RequiresConnection)
                                _view.AddEnumerableDataStoreProperty(ep.Name, ep.DefaultValue, ep.Values);
                        }
                        else
                        {
                            _view.AddDataStoreProperty(p);
                        }
                    }
                    _view.CreateEnabled = true;
                }
                else
                {
                    _view.ShowError("Selected provider does not support creation of data stores");
                    _view.ResetDataStoreGrid();
                    _view.ResetConnectGrid();
                    _view.CreateEnabled = false;
                    return;
                }

                if (!prov.IsFlatFile)
                {
                    _view.ResetConnectGrid();
                    IList<DictionaryProperty> cprops = FdoFeatureService.GetConnectProperties(prov.Name);
                    if (cprops != null)
                    {
                        foreach (DictionaryProperty p in cprops)
                        {
                            if (p.Enumerable)
                            {
                                EnumerableDictionaryProperty ep = p as EnumerableDictionaryProperty;
                                if(!ep.RequiresConnection)
                                    _view.AddEnumerableConnectProperty(ep.Name, ep.DefaultValue, ep.Values);
                            }
                            else
                            {
                                _view.AddConnectProperty(p);
                            }
                        }
                    }
                }
            }
        }

        public bool CreateDataStore()
        {
            FdoProviderInfo prov = _view.SelectedProvider;
            if (prov != null)
            {
                NameValueCollection dp = _view.DataStoreProperties;
                NameValueCollection cp = _view.ConnectProperties;
                FdoFeatureService.CreateDataStore(prov.Name, dp, cp);
                _view.ShowMessage(ResourceService.GetString("TITLE_CREATE_DATA_STORE"), ResourceService.GetString("MSG_DATA_STORE_CREATED"));
                return true;
            }
            return false;
        }
    }
}
