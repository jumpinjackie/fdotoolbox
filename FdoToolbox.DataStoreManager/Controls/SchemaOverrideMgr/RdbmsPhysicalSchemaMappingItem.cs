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
using OSGeo.FDO.Providers.Rdbms.Override;
using System.ComponentModel;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public abstract class RdbmsPhysicalSchemaMappingItem<T> : PhysicalSchemaMappingItem<T> where T : OvPhysicalSchemaMapping
    {
        protected RdbmsPhysicalSchemaMappingItem(T mapping)
            : base(mapping)
        {
            if (this.InternalValue.AutoGeneration != null)
            {
                _autoGen = new RdbmsSchemaAutoGenerationItem(this.InternalValue.AutoGeneration);
            }
            Classes = GetClasses();
        }

        protected abstract object[] GetClasses();

        [Browsable(false)]
        public object[] Classes { get; }

        private RdbmsSchemaAutoGenerationItem _autoGen;

        public RdbmsSchemaAutoGenerationItem AutoGeneration
        {
            get
            {
                return _autoGen;
            }
            set
            {
                _autoGen = value;
                this.InternalValue.AutoGeneration = _autoGen.InternalValue;
            }
        }

        public OvGeometricColumnType GeometricColumnType
        {
            get { return this.InternalValue.GeometricColumnType; }
            set { this.InternalValue.GeometricColumnType = value; }
        }

        public OvGeometricContentType GeometricContentType
        {
            get { return this.InternalValue.GeometricContentType; }
            set { this.InternalValue.GeometricContentType = value; }
        }

        public OvTableMappingType TableMapping
        {
            get { return this.InternalValue.TableMapping; }
            set { this.InternalValue.TableMapping = value; }
        }
    }
}
