#region LGPL Header
// Copyright (C) 2011, Jackie Ng
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
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Services;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Controls
{
    public interface IConnectKingOracleView : IViewContent
    {
        string ConnectionName { get; }

        string Username { get; }
        string Password { get; }
        string Service { get; }

        string OracleSchema { get; }
        string KingFdoClass { get; }
        string SdeSchema { get; }
    }

    internal class ConnectKingOraclePresenter
    {
        private readonly IConnectKingOracleView _view;

        public ConnectKingOraclePresenter(IConnectKingOracleView view)
        {
            _view = view;
        }

        public bool Connect()
        {
            var builder = new System.Data.Common.DbConnectionStringBuilder();
            builder["Username"] = _view.Username;
            builder["Password"] = _view.Password;
            builder["Service"] = _view.Service;
            if (!string.IsNullOrEmpty(_view.OracleSchema))
                builder["OracleSchema"] = _view.OracleSchema;
            if (!string.IsNullOrEmpty(_view.KingFdoClass))
                builder["KingFdoClass"] = _view.KingFdoClass;
            if (!string.IsNullOrEmpty(_view.SdeSchema))
                builder["SDE Schema"] = _view.SdeSchema;
            FdoConnection conn = new FdoConnection("OSGeo.KingOracle", builder.ToString());
            if (conn.Open() == FdoConnectionState.Open)
            {
                IFdoConnectionManager mgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
                mgr.AddConnection(_view.ConnectionName, conn);
                return true;
            }

            _view.ShowMessage(null, "Connection failed");
            return false;
        }
    }
}
