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
using FdoToolbox.Express.Controls.Ogr;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Controls
{
    public interface IConnectOgrView : IViewContent
    {
        OgrType[] OgrTypes { set; }
        OgrType SelectedOgrType { get; }

        IOgrConnectionBuilder BuilderObject { get; set; }
        string ConnectionName { get; }
    }

    public class ConnectOgrPresenter
    {
        private readonly IConnectOgrView _view;

        private Dictionary<OgrType, IOgrConnectionBuilder> _builders;

        public ConnectOgrPresenter(IConnectOgrView view)
        {
            _view = view;
            _builders = new Dictionary<OgrType, IOgrConnectionBuilder>();

            _builders.Add(OgrType.Generic, new OgrGeneric());

            _builders.Add(OgrType.ArcCoverage, new OgrArcCoverage());
            _builders.Add(OgrType.AtlasBna, new OgrAtlasBna());
            _builders.Add(OgrType.CSV, new OgrCsv());
            _builders.Add(OgrType.DGN, new OgrMicrostation());
            _builders.Add(OgrType.EsriPGB, new OgrEsriPgb());
            _builders.Add(OgrType.GeoJSON, new OgrGeoJson());
            _builders.Add(OgrType.MapInfo, new OgrMapInfo());
            _builders.Add(OgrType.S57, new OgrS57());
            _builders.Add(OgrType.ShapeFile, new OgrShapeFile());
            _builders.Add(OgrType.Virtual, new OgrVirtual());
        }

        public void Init()
        {
            _view.OgrTypes = (OgrType[])Enum.GetValues(typeof(OgrType));
        }

        public void OgrTypeChanged()
        {
            OgrType ot = _view.SelectedOgrType;
            _view.BuilderObject = _builders[ot];
        }

        public bool Connect()
        {
            if (string.IsNullOrEmpty(_view.ConnectionName))
            {
                _view.ShowMessage(null, "Name required");
                return false;
            }

            FdoConnection conn = new FdoConnection("OSGeo.OGR", _view.BuilderObject.ToConnectionString());
            try
            {
                if (conn.Open() == FdoConnectionState.Open)
                {
                    IFdoConnectionManager mgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
                    mgr.AddConnection(_view.ConnectionName, conn);
                    return true;
                }
            }
            catch (FdoException ex)
            {
                _view.ShowError(ex);
            }
            return false;
        }

        public void TestConnection()
        {
            FdoConnection conn = new FdoConnection("OSGeo.OGR", _view.BuilderObject.ToConnectionString());
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
