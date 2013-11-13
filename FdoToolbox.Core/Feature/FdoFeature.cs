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
using System.Data;
using OSGeo.FDO.Geometry;
using FdoToolbox.Core.Feature.RTree;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO-friendly <see cref="DataRow"/>
    /// </summary>
    public class FdoFeature : DataRow
    {
        internal FdoFeature(DataRowBuilder rb) : base(rb) { }

        /// <summary>
        /// Gets the <see cref="T:System.Data.DataTable"/> for which this row has a schema.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Data.DataTable"/> to which this row belongs.</returns>
        public new FdoFeatureTable Table
        {
            get { return base.Table as FdoFeatureTable; }
        }

        /// <summary>
        /// Gets the geometry field.
        /// </summary>
        /// <value>The geometry field.</value>
        internal string GeometryField
        {
            get { return this.Table.GeometryColumn; }
        }

        /// <summary>
        /// Gets the designated geometry.
        /// </summary>
        /// <value>The designated geometry.</value>
        public FdoGeometry DesignatedGeometry
        {
            get
            {
                if (!string.IsNullOrEmpty(this.GeometryField))
                {
                    return this[this.GeometryField] as FdoGeometry;
                }
                return null;
            }
        }

        private Rectangle _bbox;

        /// <summary>
        /// Gets the bounding box of this feature. 
        /// </summary>
        /// <value>The bounding box. If there is no designated geometry column then null is returned</value>
        internal Rectangle BoundingBox
        {
            get
            {
                if (_bbox == null)
                {
                    if (!string.IsNullOrEmpty(this.GeometryField))
                    {
                        IGeometry geom = this[this.GeometryField] as IGeometry;
                        if (geom != null)
                        {
                            IEnvelope env = geom.Envelope;
                            _bbox = new Rectangle((float)env.MinX, (float)env.MinY, (float)env.MaxX, (float)env.MaxY, (float)env.MinZ, (float)env.MaxZ);
                        }
                    }
                }
                return _bbox;
            }
        }

        /// <summary>
        /// Returns the item array for this feature. Geometry values are
        /// converted to FGF text form.
        /// </summary>
        /// <returns></returns>
        public object[] GeometriesAsText()
        {
            object[] items = this.ItemArray;
            object[] objs = new object[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                if(items[i] != null && items[i] != DBNull.Value)
                {
                    IGeometry geom = items[i] as IGeometry;
                    if (geom != null)
                    {
                        try
                        {
                            objs[i] = geom.Text;
                        }
                        catch
                        {
                            objs[i] = "INVALID GEOMETRY";
                        }
                    }
                    else
                    {
                        objs[i] = items[i];
                    }
                }
            }
            return objs;
        }
    }
}
