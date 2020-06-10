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
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Connections;
using FdoToolbox.Core;
using FdoToolbox.Base.Forms;
using FdoToolbox.Base.Services;
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// Generic Connect View
    /// </summary>
    internal interface IFdoConnectView : IViewContent
    {
        string ConnectionName { get; }
        IList<FdoProviderInfo> ProviderList { set; }
        FdoProviderInfo SelectedProvider { get; }
        NameValueCollection ConnectProperties { get; }
        void FlagNameError(string msg);

        void ResetGrid();
        void AddEnumerableProperty(string name, string defaultValue, string[] values);

        void AddProperty(DictionaryProperty p);

        bool ConfigEnabled { set; }
        string ConfigFile { get; }

        void FlagConfigError(string p);
    }

    /// <summary>
    /// Generic Connect presenter
    /// </summary>
    internal class FdoConnectCtlPresenter
    {
        private readonly IFdoConnectView _view;
        private readonly IFdoConnectionManager _manager;

        public FdoConnectCtlPresenter(IFdoConnectView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _manager = connMgr;
        }

        public void GetProviderList()
        {
            _view.ProviderList = FdoFeatureService.GetProviders();
        }

        public void TestConnection()
        {
            FdoProviderInfo provider = _view.SelectedProvider;
            string connStr = ExpressUtility.ConvertFromNameValueCollection(_view.ConnectProperties);

            FdoConnection conn = new FdoConnection(provider.Name, connStr);
            try
            {
                FdoConnectionState state = conn.Open();
                if (state == FdoConnectionState.Open || state == FdoConnectionState.Pending)
                {
                    _view.ShowMessage(null, "Test successful");
                    conn.Close();
                }
                else
                {
                    _view.ShowError("Connection test failed");
                }
            }
            catch (FdoException ex)
            {
                _view.ShowError(ex.InnerException.Message);
            }
            finally
            {
                conn.Dispose();
            }
        }

        private List<DictionaryProperty> _pendingProperties = new List<DictionaryProperty>();

        public void ProviderChanged()
        {
            FdoProviderInfo prov = _view.SelectedProvider;
            if (prov != null)
            {
                _view.ResetGrid();
                _pendingProperties.Clear();
                IList<DictionaryProperty> props = FdoFeatureService.GetConnectProperties(prov.Name);
                if (props != null)
                {
                    foreach (DictionaryProperty p in props)
                    {
                        if (p.Enumerable)
                        {
                            EnumerableDictionaryProperty ep = p as EnumerableDictionaryProperty;
                            if (!ep.RequiresConnection)
                                _view.AddEnumerableProperty(ep.Name, ep.DefaultValue, ep.Values);
                            else
                                _pendingProperties.Add(ep);
                        }
                        else
                        {
                            _view.AddProperty(p);
                        }
                    }
                }

                using (var conn = FeatureAccessManager.GetConnectionManager().CreateConnection(prov.Name))
                {
                    using (var connCaps = conn.ConnectionCapabilities)
                    {
                        _view.ConfigEnabled = connCaps.SupportsConfiguration();
                    }
                }
            }
        }

        public bool Connect()
        {
            if (string.IsNullOrEmpty(_view.ConnectionName))
            {
                _view.FlagNameError("Required");
                return false;
            }

            FdoConnection conn = _manager.GetConnection(_view.ConnectionName);
            if (conn != null)
            {
                _view.FlagNameError("A connection named " + _view.ConnectionName + " already exists");
                return false;
            }
            
            FdoProviderInfo provider = _view.SelectedProvider;
            //string connStr = ExpressUtility.ConvertFromNameValueCollection(_view.ConnectProperties);

            NameValueCollection cp = new NameValueCollection(_view.ConnectProperties);
            if (_pendingProperties.Count > 0)
            {
                NameValueCollection extra = new NameValueCollection();
                cp.Add(extra);
            }
            string connStr = ExpressUtility.ConvertFromNameValueCollection(cp);

            conn = new FdoConnection(provider.Name, connStr);
            if (FileService.FileExists(_view.ConfigFile))
            {
                try
                {
                    conn.SetConfiguration(_view.ConfigFile);
                }
                catch (Exception ex)
                {
                    conn.Dispose();
                    _view.FlagConfigError(ex.Message);
                    return false;
                }
            }

            try
            {
                FdoConnectionState state = conn.Open();
                if (state == FdoConnectionState.Open)
                {
                    _manager.AddConnection(_view.ConnectionName, conn);
                    return true;
                }
                else if (state == FdoConnectionState.Pending)
                {
                    //Re-query the pending parameters and re-prompt in a new dialog
                    if (_pendingProperties.Count > 0)
                    {
                        List<DictionaryProperty> pend = new List<DictionaryProperty>();
                        foreach (DictionaryProperty p in _pendingProperties)
                        {
                            pend.Add(conn.GetConnectTimeProperty(p.Name));
                        }
                        NameValueCollection extra = PendingParameterDialog.GetExtraParameters(pend);
                        //Cancelled action
                        if (extra == null)
                            return false;

                        cp.Add(extra);
                        conn.ConnectionString = ExpressUtility.ConvertFromNameValueCollection(cp);
                        if (conn.Open() == FdoConnectionState.Open)
                        {
                            _manager.AddConnection(_view.ConnectionName, conn);
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex);
                conn.Dispose();
                return false;
            }
            return false;
        }
    }
}
