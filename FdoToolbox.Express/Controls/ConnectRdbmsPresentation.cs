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
using FdoToolbox.Base;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.DataStore;
using FdoToolbox.Core.Connections;

namespace FdoToolbox.Express.Controls
{
    public interface IConnectRdbmsView : IViewContent
    {
        string Service { get; }
        string Username { get; }
        string Password { get; }

        bool SubmitEnabled { set; }
        bool ConfigEnabled { set; }

        string ConfigPath { get; }

        DataStoreInfo[] DataStores { set; }
        string SelectedDataStore { get; }
        string ConnectionName { get; }

        string Provider { get; }

        string ServiceParameter { get; }
        string UsernameParameter { get; }
        string PasswordParameter { get; }
        string DataStoreParameter { get; }
    }

    public class ConnectRdbmsPresenter
    {
        private readonly IConnectRdbmsView _view;
        private readonly IFdoConnectionManager _connMgr;
        private FdoConnection _conn;

        static string[] CONFIG_PROVIDERS = { "OSGEO.MYSQL", "OSGEO.SQLSERVERSPATIAL", "OSGEO.ODBC" };

        public ConnectRdbmsPresenter(IConnectRdbmsView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _connMgr = connMgr;
            //_view.SubmitEnabled = false;

            _view.ConfigEnabled = (Array.IndexOf<string>(CONFIG_PROVIDERS, view.Provider.ToUpper()) >= 0);
        }

        private void SetDataStore(DataStoreInfo[] values)
        {
            _view.DataStores = values;
            //_view.SubmitEnabled = values.Length > 0;
        }

        private void InitConnection()
        {
            if (_conn == null)
                _conn = new FdoConnection(_view.Provider);

            if (!string.IsNullOrEmpty(_view.ConfigPath) && System.IO.File.Exists(_view.ConfigPath))
                _conn.SetConfiguration(_view.ConfigPath);
        }

        public void PendingConnect()
        {
            InitConnection();
            try
            {
                if (_conn.State != FdoConnectionState.Closed)
                    _conn.Close();

                _conn.ConnectionString = string.Format("{0}={1};{2}={3};{4}={5}", _view.ServiceParameter, _view.Service, _view.UsernameParameter, _view.Username, _view.PasswordParameter, _view.Password);
                if (_conn.Open() == FdoConnectionState.Pending)
                {
                    List<DataStoreInfo> datastores = new List<DataStoreInfo>();
                    var cmds = _conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
                    if (Array.IndexOf(cmds, CommandType.CommandType_ListDataStores) >= 0)
                    {
                        using (var svc = _conn.CreateFeatureService())
                        {
                            var stores = svc.ListDataStores(false);
                            foreach (var store in stores)
                            {
                                var ds = new DataStoreInfo(
                                    store.Name,
                                    store.Name + ((store.IsFdoEnabled) ? "" : " (*)"),
                                    store.IsFdoEnabled);

                                datastores.Add(ds);
                            }
                        }
                    }
                    else
                    {
                        var prop = _conn.GetConnectTimeProperty(_view.DataStoreParameter) as EnumerableDictionaryProperty;
                        if (prop != null)
                        {
                            foreach (string name in prop.Values)
                            {
                                datastores.Add(new DataStoreInfo(name, name, true));
                            }
                        }
                    }
                    SetDataStore(datastores.ToArray());
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }

        public bool Connect()
        {
            InitConnection();
            if (string.IsNullOrEmpty(_view.ConnectionName) || _connMgr.NameExists(_view.ConnectionName))
            {
                _view.ShowError(ResourceService.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                return false;
            }

            try
            {
                _conn.ConnectionString = string.Format("{0}={1};{2}={3};{4}={5};{6}={7}", _view.ServiceParameter, _view.Service, _view.UsernameParameter, _view.Username, _view.PasswordParameter, _view.Password, _view.DataStoreParameter, _view.SelectedDataStore);
                if (_conn.Open() == FdoConnectionState.Open)
                {
                    _connMgr.AddConnection(_view.ConnectionName, _conn);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }

            return false;
        }
    }
}
