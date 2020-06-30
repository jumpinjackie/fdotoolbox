#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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
using FdoToolbox.Core.CoordinateSystems.Transform;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Common;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Raster;
using OSGeo.FDO.Schema;
using System;

namespace FdoCmd.Commands
{
    public class TransformedFeatureReader : IFeatureReader
    {
        readonly IFeatureReader _inner;
        readonly FdoGeometryTransformingConverter _xformer;

        public TransformedFeatureReader(IFeatureReader inner, string sourceWkt, string targetWkt)
        {
            _inner = inner;
            _xformer = new FdoGeometryTransformingConverter(sourceWkt, targetWkt);
        }

        public void Close()
        {
            _inner.Close();
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public bool GetBoolean(int index)
        {
            return _inner.GetBoolean(index);
        }

        public bool GetBoolean(string name)
        {
            return _inner.GetBoolean(name);
        }

        public byte GetByte(int index)
        {
            return _inner.GetByte(index);
        }

        public byte GetByte(string name)
        {
            return _inner.GetByte(name);
        }

        public ClassDefinition GetClassDefinition()
        {
            return _inner.GetClassDefinition();
        }

        public DateTime GetDateTime(int index)
        {
            return _inner.GetDateTime(index);
        }

        public DateTime GetDateTime(string name)
        {
            return _inner.GetDateTime(name);
        }

        public int GetDepth()
        {
            return _inner.GetDepth();
        }

        public double GetDouble(int index)
        {
            return _inner.GetDouble(index);
        }

        public double GetDouble(string name)
        {
            return _inner.GetDouble(name);
        }

        public IFeatureReader GetFeatureObject(int index)
        {
            return _inner.GetFeatureObject(index);
        }

        public IFeatureReader GetFeatureObject(string propertyName)
        {
            return _inner.GetFeatureObject(propertyName);
        }

        public byte[] GetGeometry(int index)
        {
            var fgf = _inner.GetGeometry(index);
            using (var geom = _xformer.CreateFromFgf(fgf))
            {
                using (var xformed = _xformer.ConvertOrdinates(geom))
                {
                    return _xformer.ToFgf(xformed);
                }
            }
        }

        public byte[] GetGeometry(string name)
        {
            var fgf = _inner.GetGeometry(name);
            using (var geom = _xformer.CreateFromFgf(fgf))
            {
                using (var xformed = _xformer.ConvertOrdinates(geom))
                {
                    return _xformer.ToFgf(xformed);
                }
            }
        }

        public short GetInt16(int index)
        {
            return _inner.GetInt16(index);
        }

        public short GetInt16(string name)
        {
            return _inner.GetInt16(name);
        }

        public int GetInt32(int index)
        {
            return _inner.GetInt32(index);
        }

        public int GetInt32(string name)
        {
            return _inner.GetInt32(name);
        }

        public long GetInt64(int index)
        {
            return _inner.GetInt64(index);
        }

        public long GetInt64(string name)
        {
            return _inner.GetInt64(name);
        }

        public LOBValue GetLOB(int index)
        {
            return _inner.GetLOB(index);
        }

        public LOBValue GetLOB(string name)
        {
            return _inner.GetLOB(name);
        }

        public IStreamReader GetLOBStreamReader(int index)
        {
            return _inner.GetLOBStreamReader(index);
        }

        public IStreamReader GetLOBStreamReader(string name)
        {
            return _inner.GetLOBStreamReader(name);
        }

        public int GetPropertyIndex(string name)
        {
            return _inner.GetPropertyIndex(name);
        }

        public string GetPropertyName(int index)
        {
            return _inner.GetPropertyName(index);
        }

        public IRaster GetRaster(int index)
        {
            return _inner.GetRaster(index);
        }

        public IRaster GetRaster(string name)
        {
            return _inner.GetRaster(name);
        }

        public float GetSingle(int index)
        {
            return _inner.GetSingle(index);
        }

        public float GetSingle(string name)
        {
            return _inner.GetSingle(name);
        }

        public string GetString(int index)
        {
            return _inner.GetString(index);
        }

        public string GetString(string name)
        {
            return _inner.GetString(name);
        }

        public bool IsNull(int index)
        {
            return _inner.IsNull(index);
        }

        public bool IsNull(string name)
        {
            return _inner.IsNull(name);
        }

        public bool ReadNext()
        {
            return _inner.ReadNext();
        }
    }
}
