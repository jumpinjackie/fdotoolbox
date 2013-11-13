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
using System.Text;
using Shp = OSGeo.FDO.Providers.SHP.Override;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class ShpPropertyDefinitionItem : PhysicalElementMappingItem<Shp.PropertyDefinition>
    {
        public ShpPropertyDefinitionItem(Shp.PropertyDefinition value)
            : base(value)
        {
            _column = new ShpColumnDefinitionItem(value.Column);
        }

        private ShpColumnDefinitionItem _column;

        public ShpColumnDefinitionItem Column
        {
            get { return _column; }
            set 
            { 
                _column = value;
                this.InternalValue.Column = value.InternalValue;
            }
        }
    }
}
