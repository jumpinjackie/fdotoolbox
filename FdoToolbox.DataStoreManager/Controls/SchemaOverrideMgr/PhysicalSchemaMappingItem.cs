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
using System.Linq;
using System.Text;
using OSGeo.FDO.Commands.Schema;
using System.ComponentModel;
using OSGeo.FDO.Connections;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public abstract class PhysicalSchemaMappingItem<T> : PhysicalElementMappingItem<T>, IPhysicalSchemaMapping where T : PhysicalSchemaMapping
    {
        protected PhysicalSchemaMappingItem(T mapping) : base(mapping)
        {
            
        }

        public PhysicalSchemaMapping Mapping
        {
            get { return this.InternalValue; }
        }

        public virtual PhysicalSchemaMapping CreateCopy()
        {
            using (var ms = new OSGeo.FDO.Common.Io.IoMemoryStream())
            {
                //Dump internal value to xml
                using (var writer = new OSGeo.FDO.Common.Xml.XmlWriter(ms))
                {
                    this.InternalValue.WriteXml(writer, new OSGeo.FDO.Xml.XmlFlags());
                }
                ms.Reset();
                //Load into fresh object
                var sms = new OSGeo.FDO.Commands.Schema.PhysicalSchemaMappingCollection();
                sms.ReadXml(ms);
                return sms[0];
            }
        }

        /*
        /// <summary>
        /// Creates a cloned copy of this mapping
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public abstract PhysicalSchemaMapping GetMapping(OSGeo.FDO.Connections.IConnection conn);
         */
    }
}
