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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Geometry;
using FdoToolbox.Core.CoordinateSystems;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Forms
{
    internal interface IFdoSpatialContextDialogView
    {
        string ContextName { get; set;}
        string Description { get; set; }
        string CoordinateSystem { get; set; }
        string CoordinateSystemWkt { get; set; }

        string XYTolerance { get; set; }
        string ZTolerance { get; set; }

        bool NameEnabled { get; set; }
        bool IsExtentDefined { get; }
        bool ComputeExtentsEnabled { set; }
        
        SpatialContextExtentType[] ExtentTypes { set; }
        SpatialContextExtentType SelectedExtentType { get; }

        string LowerLeftX { get; set; }
        string LowerLeftY { get; set; }
        string UpperRightX { get; set; }
        string UpperRightY { get; set; }
    }

    internal class FdoSpatialContextDialogPresenter
    {
        private readonly IFdoSpatialContextDialogView _view;
        private FdoConnection _conn;

        public FdoConnection Connection
        {
            get { return _conn; }
        }

        public FdoSpatialContextDialogPresenter(IFdoSpatialContextDialogView view, FdoConnection conn)
        {
            _view = view;
            _conn = conn;
        }

        public void Init()
        {
            if (_conn != null)
            {
                _view.ExtentTypes = (SpatialContextExtentType[])_conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_SpatialContextTypes);
            }
            else
            {
                //Can only have static extents (for XML serialization purposes)
                _view.ExtentTypes = new SpatialContextExtentType[] { SpatialContextExtentType.SpatialContextExtentType_Static };
                _view.ComputeExtentsEnabled = false;
            }
        }

        public void ComputeExtents(IEnumerable<ClassDefinition> classes)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                IEnvelope env = service.ComputeEnvelope(classes);
                if (env != null)
                {
                    _view.LowerLeftX = env.MinX.ToString();
                    _view.LowerLeftY = env.MinY.ToString();
                    _view.UpperRightX = env.MaxX.ToString();
                    _view.UpperRightY = env.MaxY.ToString();
                }
            }
        }

        public void SetCoordinateSystem(CoordinateSystemDefinition cs)
        {
            if (cs != null)
            {
                if (_view.NameEnabled)
                    _view.ContextName = cs.Name;

                _view.Description = cs.Description;
                _view.CoordinateSystem = cs.Wkt;
                _view.CoordinateSystemWkt = cs.Wkt;
            }
        }

        public void SetSpatialContext(SpatialContextInfo sci)
        {
            _view.NameEnabled = false;
            _view.ContextName = sci.Name;
            _view.CoordinateSystem = sci.CoordinateSystem;
            _view.CoordinateSystemWkt = sci.CoordinateSystemWkt;
            _view.Description = sci.Description;
            if (!string.IsNullOrEmpty(sci.ExtentGeometryText))
            {
                using (FdoFeatureService service = _conn.CreateFeatureService())
                {
                    using (IGeometry geom = service.GeometryFactory.CreateGeometry(sci.ExtentGeometryText))
                    {
                        _view.LowerLeftX = geom.Envelope.MinX.ToString();
                        _view.UpperRightX = geom.Envelope.MaxX.ToString();
                        _view.LowerLeftY = geom.Envelope.MinY.ToString();
                        _view.UpperRightY = geom.Envelope.MaxY.ToString();
                    }
                }
            }
            _view.XYTolerance = sci.XYTolerance.ToString();
            _view.ZTolerance = sci.ZTolerance.ToString();
        }
    }
}
