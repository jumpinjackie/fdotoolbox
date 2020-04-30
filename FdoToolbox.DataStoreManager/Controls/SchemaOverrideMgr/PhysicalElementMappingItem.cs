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
using OSGeo.FDO.Commands.Schema;
using System.ComponentModel;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public abstract class PhysicalElementMappingItem<T> : DesignTimeWrapper<T>, FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr.IPhysicalElementMappingItem where T : PhysicalElementMapping
    {
        protected PhysicalElementMappingItem(T value)
            : base(value)
        { }

        [Description("Determines whether the name of the mapping element can be set or changed")]
        public bool CanSetName => this.InternalValue.CanSetName;

        [Description("The name of this element mapping")]
        public string Name
        {
            get { return this.InternalValue.Name; }
            set
            {
                if (this.CanSetName)
                {
                    this.InternalValue.Name = value; 
                }
            }
        }

        [Description("The qualified name of this element mapping")]
        public string QualifiedName => this.InternalValue.QualifiedName;
    }
}
