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
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;
using System.Collections.Generic;
using System.Globalization;

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

        bool WKTEnabled { get; set; }

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

        public FdoConnection Connection { get; }

        public FdoSpatialContextDialogPresenter(IFdoSpatialContextDialogView view, FdoConnection conn)
        {
            _view = view;
            Connection = conn;
        }

        public void Init()
        {
            if (Connection != null)
            {
                using (var connCaps = Connection.ConnectionCapabilities)
                {
                    _view.ExtentTypes = connCaps.SpatialContextTypes;
                    _view.WKTEnabled = connCaps.SupportsCSysWKTFromCSysName();
                }
            }
            else
            {
                //Can only have static extents (for XML serialization purposes)
                _view.ExtentTypes = new SpatialContextExtentType[] { SpatialContextExtentType.SpatialContextExtentType_Static };
                _view.WKTEnabled = true;
                _view.ComputeExtentsEnabled = false;
            }
        }

        public void ComputeExtents(IEnumerable<ClassDefinition> classes)
        {
            using (FdoFeatureService service = Connection.CreateFeatureService())
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

        public void SetCoordinateSystem(ICoordinateSystem cs, string provider)
        {
            if (cs != null)
            {
                if (_view.NameEnabled)
                    _view.ContextName = cs.Code;

                _view.Description = cs.Description;
                _view.CoordinateSystem = SpatialContextInfo.NominateCsName(cs, provider);
                if (_view.WKTEnabled)
                    _view.CoordinateSystemWkt = cs.WKT;

                var bounds = cs.Bounds;
                if (bounds != null)
                {
                    _view.LowerLeftX = bounds.MinX.ToString(CultureInfo.InvariantCulture);
                    _view.LowerLeftY = bounds.MinY.ToString(CultureInfo.InvariantCulture);
                    _view.UpperRightX = bounds.MaxX.ToString(CultureInfo.InvariantCulture);
                    _view.UpperRightY = bounds.MaxY.ToString(CultureInfo.InvariantCulture);
                }
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
                using (FdoFeatureService service = Connection.CreateFeatureService())
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
