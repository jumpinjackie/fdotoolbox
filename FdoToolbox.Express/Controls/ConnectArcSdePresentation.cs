using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base;
using FdoToolbox.Core.Connections;

namespace FdoToolbox.Express.Controls
{
    public interface IConnectArcSdeView : IViewContent
    {
        string Server { get; }
        string Instance { get; }
        string Username { get; }
        string Password { get; }

        bool DataStoreEnabled { set; }
        bool SubmitEnabled { set; }

        string[] DataStores { set; }
        string SelectedDataStore { get; }
        string ConnectionName { get; }
    }

    public class ConnectArcSdePresenter
    {
        private readonly IConnectArcSdeView _view;
        private readonly IFdoConnectionManager _connMgr;
        private FdoConnection _conn;

        public ConnectArcSdePresenter(IConnectArcSdeView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _connMgr = connMgr;
            _conn = new FdoConnection("OSGeo.ArcSDE");
            _view.DataStoreEnabled = false;
            _view.SubmitEnabled = false; 
        }

        private void SetDataStore(string[] values)
        {
            _view.DataStores = values;
            _view.DataStoreEnabled = values.Length > 0;
            _view.SubmitEnabled = values.Length > 0;
        }

        public void PendingConnect()
        {
            try
            {
                if (_conn.State != FdoConnectionState.Closed)
                    _conn.Close();

                _conn.ConnectionString = string.Format("Server={0};Instance={1};Username={2};Password={3}", _view.Server, _view.Instance, _view.Username, _view.Password);
                if (_conn.Open() == FdoConnectionState.Pending)
                {
                    EnumerableDictionaryProperty prop = (EnumerableDictionaryProperty)_conn.GetConnectTimeProperty("DataStore");
                    SetDataStore(prop.Values);
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }

        public bool Connect()
        {
            if (string.IsNullOrEmpty(_view.ConnectionName) || _connMgr.NameExists(_view.ConnectionName))
            {
                _view.ShowError(ResourceService.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                return false;
            }

            try
            {
                _conn.ConnectionString = string.Format("Server={0};Instance={1};Username={2};Password={3};DataStore={4}", _view.Server, _view.Instance, _view.Username, _view.Password, _view.SelectedDataStore);
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
