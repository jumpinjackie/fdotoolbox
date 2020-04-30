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
using SharpMap.Data.Providers;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Geometry;
using SharpMap.Data;
using FdoToolbox.Core.Feature.RTree;
using System.Data;

namespace FdoToolbox.Base.SharpMapProvider
{
    /// <summary>
    /// An in-memory data provider for SharpMap
    /// </summary>
    internal class FdoInMemoryProvider : IProvider
    {
        private FgfGeometryFactory _geomFactory;

        public FdoInMemoryProvider() { _geomFactory = new FgfGeometryFactory(); }

        public FdoFeatureTable DataSource { get; set; }

        public List<SharpMap.Geometries.Geometry> GetGeometriesInView(SharpMap.Geometries.BoundingBox bbox)
        {
            if (DataSource == null || DataSource.Rows.Count == 0 || string.IsNullOrEmpty(DataSource.GeometryColumn))
                return new List<SharpMap.Geometries.Geometry>();

            List<SharpMap.Geometries.Geometry> geoms = new List<SharpMap.Geometries.Geometry>();
            FdoFeature[] matches = DataSource.Intersects(Converter.RectFromBoundingBox(bbox));
            foreach (FdoFeature feat in matches)
            {
                try
                {
                    geoms.Add(Converter.FromFdoGeometry(feat.DesignatedGeometry, _geomFactory));
                }
                catch { }
            }
            return geoms;
        }

        public List<uint> GetObjectIDsInView(SharpMap.Geometries.BoundingBox bbox)
        {
            return null;
        }

        public SharpMap.Geometries.Geometry GetGeometryByID(uint oid)
        {
            return null;
        }

        public void ExecuteIntersectionQuery(SharpMap.Geometries.Geometry geom, SharpMap.Data.FeatureDataSet ds)
        {
            ExecuteIntersectionQuery(geom.GetBoundingBox(), ds);
        }

        public void ExecuteIntersectionQuery(SharpMap.Geometries.BoundingBox box, SharpMap.Data.FeatureDataSet ds)
        {
            Rectangle r = new Rectangle((float)box.Left, (float)box.Bottom, (float)box.Right, (float)box.Top, (float)0.0, (float)0.0);
            FdoFeature[] matches = DataSource.Intersects(r);

            FeatureDataTable table = new FeatureDataTable();
            foreach (DataColumn col in DataSource.Columns)
            {
                table.Columns.Add(col.ColumnName, col.DataType, col.Expression);
            }

            //Filter the initial result set by inverting the operands. This weeds out non-matches on point intersection tests.
            IEnvelope env = Converter.EnvelopeFromBoundingBox(box);
            FdoGeometry poly = new FdoGeometry(Converter.CreatePolygonFromEnvelope(env));
            foreach (FdoFeature feat in matches)
            {
                FdoGeometry geom = feat.DesignatedGeometry;
                if (geom != null)
                {
                    if (geom.Contains(env) || geom.Intersects(poly))
                    {
                        FeatureDataRow row = table.NewRow();
                        bool add = true;
                        foreach (DataColumn col in DataSource.Columns)
                        {
                            if (col.ColumnName == DataSource.GeometryColumn)
                            {
                                try
                                {
                                    row.Geometry = Converter.FromFdoGeometry(geom, _geomFactory);
                                }
                                catch //Can't help you if you fail conversion.
                                {
                                    add = false;
                                }
                            }
                            else
                            {
                                row[col.ColumnName] = feat[col.ColumnName];
                            }
                        }
                        if (add)
                            table.AddRow(row);
                    }
                }
            }
            ds.Tables.Add(table);
        }

        public int GetFeatureCount()
        {
            if (DataSource != null)
                return DataSource.Rows.Count;
            else
                return 0;
        }

        public SharpMap.Data.FeatureDataRow GetFeature(uint RowID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public SharpMap.Geometries.BoundingBox GetExtents()
        {
            SharpMap.Geometries.BoundingBox bbox = null;
            if (DataSource == null || DataSource.Rows.Count == 0 || string.IsNullOrEmpty(DataSource.GeometryColumn))
                return new SharpMap.Geometries.BoundingBox(0.0, 0.0, 0.0, 0.0);
            
            foreach (FdoFeature feat in DataSource.Rows)
            {
                if (feat[DataSource.GeometryColumn] != null && feat[DataSource.GeometryColumn] != DBNull.Value)
                {
                    try
                    {
                        OSGeo.FDO.Geometry.IGeometry geom = (OSGeo.FDO.Geometry.IGeometry)feat[DataSource.GeometryColumn];
                        if (bbox != null)
                        {
                            if (geom.Envelope.MaxX > bbox.Max.X)
                                bbox.Max.X = geom.Envelope.MaxX;
                            if (geom.Envelope.MaxY > bbox.Max.Y)
                                bbox.Max.Y = geom.Envelope.MaxY;
                            if (geom.Envelope.MinX < bbox.Min.X)
                                bbox.Min.X = geom.Envelope.MinX;
                            if (geom.Envelope.MinY < bbox.Min.Y)
                                bbox.Min.Y = geom.Envelope.MinY;
                        }
                        else
                        {
                            bbox = new SharpMap.Geometries.BoundingBox(geom.Envelope.MinX, geom.Envelope.MinY, geom.Envelope.MaxX, geom.Envelope.MaxY);
                        }
                    }
                    catch { }
                }
            }
            if (bbox != null)
                return bbox;
            else
                return new SharpMap.Geometries.BoundingBox(0.0, 0.0, 0.0, 0.0);
        }

        public string ConnectionID => DataSource.TableName;

        public void Open()
        {
            
        }

        public void Close()
        {
            
        }

        public bool IsOpen => true;

        public int SRID
        {
            get
            {
                return -1;
            }
            set
            {
                
            }
        }

        public void Dispose()
        {
            _geomFactory.Dispose();
        }

        public double? GetXYTolerance()
        {
            SpatialContextInfo ctx = DataSource.ActiveSpatialContext;
            if (ctx != null)
                return ctx.XYTolerance;

            return null;
        }
    }
}
