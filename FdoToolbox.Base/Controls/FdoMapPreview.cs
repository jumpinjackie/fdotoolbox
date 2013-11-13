#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpMap.Forms;
using SharpMap;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.SharpMapProvider;
using SharpMap.Layers;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A <see cref="FdoDataPreviewCtl"/> sub-view that provides a visual preview of the data preview query result set
    /// </summary>
    internal partial class FdoMapPreview : UserControl, IFdoMapView
    {
        private FdoMapPreviewPresenter _presenter;
        private MapImage img;

        public FdoMapPreview()
        {
            InitializeComponent();
            img = new MapImage();
            img.Map = new Map();
            img.Dock = DockStyle.Fill;
            mapContentPanel.Controls.Add(img);
            _presenter = new FdoMapPreviewPresenter(this, img);
        }

        private FdoInMemoryProvider _provider;

        public FdoInMemoryProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = new FdoInMemoryProvider();
                }
                return _provider;
            }
        }

        public FdoFeatureTable DataSource
        {
            set
            {
                this.Provider.DataSource = value;
                if (value != null && img.Map.Layers.Count == 0)
                {
                    VectorLayer layer = new VectorLayer("Preview", this.Provider);
                    layer.Style.Fill = Brushes.Transparent;
                    layer.Style.EnableOutline = true;
                    if (Preferences.DataPreviewRandomColors)
                        layer.Theme = new RandomizedTheme();
                    img.Map.Layers.Add(layer);
                }
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            _presenter.ZoomIn();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            _presenter.ZoomOut();
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            _presenter.Pan();
        }

        private void btnZoomExtents_Click(object sender, EventArgs e)
        {
            _presenter.ZoomExtents();
        }

        public bool ZoomInChecked
        {
            set { btnZoomIn.Checked = value; }
        }

        public bool ZoomOutChecked
        {
            set { btnZoomOut.Checked = value; }
        }

        public bool SelectChecked
        {
            set { btnSelect.Checked = value; }
        }

        public bool PanChecked
        {
            set { btnPan.Checked = value; }
        }

        public string StatusText
        {
            set { lblStatus.Text = value; }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            _presenter.Select();
        }

        public object SelectedFeatureData
        {
            set 
            {
                grdFeatures.DataSource = value;
                splitContainer1.Panel2Collapsed = (value == null);
                lblCount.Text = grdFeatures.Rows.Count + " features selected";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _presenter.ClearSelection();
        }
    }
}
