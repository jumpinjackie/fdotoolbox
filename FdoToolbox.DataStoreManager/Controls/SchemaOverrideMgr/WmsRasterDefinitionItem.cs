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
using Wms = OSGeo.FDO.Providers.WMS.Override;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class WmsRasterDefinitionItem : PhysicalElementMappingItem<Wms.OvRasterDefinition>
    {
        public WmsRasterDefinitionItem(Wms.OvRasterDefinition value)
            : base(value)
        {
            List<WmsLayerDefinitionItem> layers = new List<WmsLayerDefinitionItem>();
            foreach (Wms.OvLayerDefinition layer in value.Layers)
            {
                layers.Add(new WmsLayerDefinitionItem(layer));
            }
            _layers = layers.ToArray();
        }

        public string BackgroundColor
        {
            get { return this.InternalValue.BackgroundColor; }
            set { this.InternalValue.BackgroundColor = value; }
        }

        public string ElevationDimension
        {
            get { return this.InternalValue.ElevationDimension; }
            set { this.InternalValue.ElevationDimension = value; }
        }

        public string ImageFormat
        {
            get { return this.InternalValue.ImageFormat; }
            set { this.InternalValue.ImageFormat = value; }
        }

        private WmsLayerDefinitionItem[] _layers;

        public WmsLayerDefinitionItem[] Layers
        {
            get
            {
                return _layers;
            }
        }

        public string SpatialContextName
        {
            get { return this.InternalValue.SpatialContextName; }
            set { this.InternalValue.SpatialContextName = value; }
        }

        public string TimeDimension
        {
            get { return this.InternalValue.TimeDimension; }
            set { this.InternalValue.TimeDimension = value; }
        }

        public bool Transparent
        {
            get { return this.InternalValue.Transparent; }
            set { this.InternalValue.Transparent = value; }
        }

        public bool UseTileCache
        {
            get { return this.InternalValue.UseTileCache; }
            set { this.InternalValue.UseTileCache = value; }
        }
    }
}
