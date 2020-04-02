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
using Rdbms = OSGeo.FDO.Providers.Rdbms.Override;
using System.ComponentModel;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public abstract class RdbmsGeometricPropertyDefinitionItem<T> : RdbmsPropertyDefinitionItem<T>, FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr.IRdbmsGeometricPropertyDefinitionItem where T : Rdbms.OvGeometricPropertyDefinition
    {
        protected RdbmsGeometricPropertyDefinitionItem(T value)
            : base(value)
        { 
            
        }

        [Description("The geometric column type")]
        public Rdbms.OvGeometricColumnType GeometricColumnType
        {
            get { return this.InternalValue.GeometricColumnType; }
            set { this.InternalValue.GeometricColumnType = value; }
        }

        [Description("The geometric content type")]
        public Rdbms.OvGeometricContentType GeometricContentType
        {
            get { return this.InternalValue.GeometricContentType; }
            set { this.InternalValue.GeometricContentType = value; }
        }

        [Description("The X column name")]
        public string XColumnName
        {
            get { return this.InternalValue.XColumnName; }
            set { this.InternalValue.XColumnName = value; }
        }

        [Description("The Y column name")]
        public string YColumnName
        {
            get { return this.InternalValue.YColumnName; }
            set { this.InternalValue.YColumnName = value; }
        }

        [Description("The Z column name")]
        public string ZColumnName
        {
            get { return this.InternalValue.ZColumnName; }
            set { this.InternalValue.ZColumnName = value; }
        }
    }
}
