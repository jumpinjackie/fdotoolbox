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
using FdoToolbox.Express.Controls.Odbc;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core;
using OSGeo.FDO.Common.Io;

namespace FdoToolbox.Express.Controls
{
    public interface IConnectOdbcView : IViewContent
    {
        OdbcType[] OdbcTypes { set; }
        OdbcType SelectedOdbcType { get; }

        IOdbcConnectionBuilder BuilderObject { get; set; }
        string ConnectionName { get; }
        string ConfigurationFile { get; }
    }

    public class ConnectOdbcPresenter
    {
        private readonly IConnectOdbcView _view;

        private Dictionary<OdbcType, IOdbcConnectionBuilder> _builders;

        public ConnectOdbcPresenter(IConnectOdbcView view)
        {
            _view = view;
            _builders = new Dictionary<OdbcType, IOdbcConnectionBuilder>();
            _builders.Add(OdbcType.MsAccess, new OdbcAccess());
            _builders.Add(OdbcType.MsExcel, new OdbcExcel());
            _builders.Add(OdbcType.SQLServer, new OdbcSqlServer());
            _builders.Add(OdbcType.Text, new OdbcText());
            _builders.Add(OdbcType.Generic, new OdbcGeneric());
        }

        public void Init()
        {
            _view.OdbcTypes = (OdbcType[])Enum.GetValues(typeof(OdbcType));
        }

        public void OdbcTypeChanged()
        {
            OdbcType ot = _view.SelectedOdbcType;
            _view.BuilderObject = _builders[ot];
        }

        public bool Connect()
        {
            if (string.IsNullOrEmpty(_view.ConnectionName))
            {
                _view.ShowMessage(null, "Name required");
                return false;
            }

            FdoConnection conn = new FdoConnection("OSGeo.ODBC", string.Format("ConnectionString=\"{0}\"", _view.BuilderObject.ToConnectionString()));
            if (FileService.FileExists(_view.ConfigurationFile))
            {
                conn.SetConfiguration(_view.ConfigurationFile);
            }
            if (conn.Open() == FdoConnectionState.Open)
            {
                IFdoConnectionManager mgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
                mgr.AddConnection(_view.ConnectionName, conn);
                return true;
            }

            _view.ShowMessage(null, "Connection test failed");
            return false;
        }

        public void TestConnection()
        {
            FdoConnection conn = new FdoConnection("OSGeo.ODBC", string.Format("ConnectionString=\"{0}\"", _view.BuilderObject.ToConnectionString()));
            try
            {
                FdoConnectionState state = conn.Open();
                if (state == FdoConnectionState.Open)
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
        }
    }
}
