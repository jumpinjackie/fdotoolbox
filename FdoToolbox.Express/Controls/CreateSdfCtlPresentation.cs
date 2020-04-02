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
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Controls
{
    public interface ICreateSdfView : IViewContent
    {
        string SdfFile { get; }
        string FeatureSchemaDefinition { get; }
        bool CreateConnection { get; }
        string ConnectionName { get; set; }
        bool ConnectionEnabled { set; }
        bool FixIncompatibilities { get; }
    }

    public class CreateSdfPresenter
    {
        private readonly ICreateSdfView _view;
        private FdoConnectionManager _connMgr;

        public CreateSdfPresenter(ICreateSdfView view)
        {
            _view = view;
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            CheckConnect();
        }

        public bool CheckConnectionName()
        {
            return !_connMgr.NameExists(_view.ConnectionName);
        }

        public bool CreateSdf()
        {
            if (_view.CreateConnection && string.IsNullOrEmpty(_view.ConnectionName))
            {
                _view.ShowError("Specify a connection name");
                return false;
            }

            if (ExpressUtility.CreateFlatFileDataSource("OSGeo.SDF", _view.SdfFile))
            {
                FdoDataStoreConfiguration dstore = null;
                if (FileService.FileExists(_view.FeatureSchemaDefinition))
                {
                    dstore = FdoDataStoreConfiguration.FromFile(_view.FeatureSchemaDefinition);
                    
                    //SDF only permits the following:
                    // 1 feature schema
                    // 1 spatial context

                    if (dstore.Schemas.Count > 1)
                    {
                        _view.ShowError("Multiple schemas were found in this document. SDF only allows 1 feature schema");
                        return false;
                    }
                    if (dstore.SpatialContexts.Length > 1)
                    {
                        _view.ShowError("Multiple spatial contexts were found in this doucment. SDF only allows 1 spatial context");
                        return false;
                    }
                }
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SDF", _view.SdfFile);
                if (dstore != null)
                {
                    using (var svc = conn.CreateFeatureService())
                    {
                        if (dstore.SpatialContexts.Length == 1)
                        {
                            //Overwrite existing spatial context if it exists
                            var sc = dstore.SpatialContexts[0];
                            var asc = svc.GetActiveSpatialContext();
                            if (asc != null)
                                sc.Name = asc.Name;

                            svc.CreateSpatialContext(sc, (asc != null));
                        }

                        var schema = dstore.Schemas[0];
                        if (_view.FixIncompatibilities)
                        {
                            IncompatibleSchema incS;
                            if (!svc.CanApplySchema(schema, out incS))
                            {
                                schema = svc.AlterSchema(schema, incS);
                            }
                        }
                        svc.ApplySchema(schema);
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
