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
using SharpMap.Forms;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoMapView
    {
        bool ZoomInChecked { set; }
        bool ZoomOutChecked { set; }
        bool SelectChecked { set; }
        bool PanChecked { set; }
        string StatusText { set; }
        object SelectedFeatureData { set; }
    }

    internal class FdoMapPreviewPresenter
    {
        private readonly IFdoMapView _view;
        private readonly MapImage _mapImage;

        public FdoMapPreviewPresenter(IFdoMapView view, MapImage mimg)
        {
            _view = view;
            _mapImage = mimg;
            _mapImage.MouseMove += new MapImage.MouseEventHandler(MapMouseMove);
            _mapImage.MapQueried += new MapImage.MapQueryHandler(OnMapQueried);

            switch (_mapImage.ActiveTool)
            {
                case MapImage.Tools.Pan:
                    _view.PanChecked = true;
                    break;
                case MapImage.Tools.Query:
                    _view.SelectChecked = true;
                    break;
                case MapImage.Tools.ZoomIn:
                    _view.ZoomInChecked = true;
                    break;
                case MapImage.Tools.ZoomOut:
                    _view.ZoomOutChecked = true;
                    break;
            }
        }

        void OnMapQueried(SharpMap.Data.FeatureDataTable data)
        {
            _view.SelectedFeatureData = (data.Count > 0) ? data : null;
        }

        void MapMouseMove(SharpMap.Geometries.Point WorldPos, System.Windows.Forms.MouseEventArgs ImagePos)
        {
            _view.StatusText = string.Format("X: {0} Y: {1}", WorldPos.X, WorldPos.Y);
        }

        private void ChangeTool(SharpMap.Forms.MapImage.Tools tool)
        {
            _mapImage.ActiveTool = tool;

            _view.SelectChecked = false;
            _view.ZoomInChecked = false;
            _view.ZoomOutChecked = false;
            _view.PanChecked = false;

            switch (tool)
            {
                case SharpMap.Forms.MapImage.Tools.Pan:
                    _view.PanChecked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.ZoomIn:
                    _view.ZoomInChecked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.ZoomOut:
                    _view.ZoomOutChecked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.Query:
                    _view.SelectChecked = true;
                    break;
            }
        }

        private bool HasLayers()
        {
            return _mapImage.Map != null && _mapImage.Map.Layers.Count > 0;
        }

        public void ZoomIn()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.ZoomIn);
        }

        public void ZoomOut()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.ZoomOut);
        }

        public void Pan()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.Pan);
        }

        public void Select()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.Query);
        }

        public void ZoomExtents()
        {
            if (HasLayers())
            {
                _mapImage.Map.ZoomToExtents();
                _mapImage.Refresh();
            }
        }

        public void ClearSelection()
        {
            _view.SelectedFeatureData = null;
        }
    }
}
