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
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Controls
{
    public interface ICreateSqliteView : IViewContent
    {
        string SQLiteFile { get; }
        string FeatureSchemaDefinition { get; }
        bool CreateConnection { get; }
        string ConnectionName { get; set; }
        bool ConnectionEnabled { set; }
        bool FixIncompatibilities { get; }
    }

    public class CreateSqlitePresenter
    {
        private readonly ICreateSqliteView _view;
        private FdoConnectionManager _connMgr;

        public CreateSqlitePresenter(ICreateSqliteView view)
        {
            _view = view;
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            CheckConnect();
        }

        public bool CheckConnectionName()
        {
            return !_connMgr.NameExists(_view.ConnectionName);
        }

        public bool CreateSqlite()
        {
            if (_view.CreateConnection && string.IsNullOrEmpty(_view.ConnectionName))
            {
                _view.ShowError("Specify a connection name");
                return false;
            }

            if (ExpressUtility.CreateFlatFileDataSource("OSGeo.SQLite", _view.SQLiteFile))
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SQLite", _view.SQLiteFile);
                if (FileService.FileExists(_view.FeatureSchemaDefinition))
                {
                    conn.Open();
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.LoadSchemasFromXml(_view.FeatureSchemaDefinition, _view.FixIncompatibilities);
                    }
                }
                if (_view.CreateConnection)
                {
                    _connMgr.AddConnection(_view.ConnectionName, conn);
                }
                else
                {
                    conn.Dispose();
                }
            }
            return true;
        }

        public void CheckConnect()
        {
            if (!_view.CreateConnection)
                _view.ConnectionName = "";

            _view.ConnectionEnabled = _view.CreateConnection;
        }
    }
}
