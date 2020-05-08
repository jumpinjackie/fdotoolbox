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
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Common;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Raster;
using System;

namespace FdoCmd.Commands
{
    public class BBOXReader : IReader
    {
        readonly byte[] _fgf;
        readonly string _geomProp;
        bool _read = false;

        public BBOXReader(byte[] fgf, string geomProp)
        {
            _fgf = fgf;
            _geomProp = geomProp;
        }

        public void Close()
        {
            
        }

        public void Dispose()
        {
            
        }

        public bool GetBoolean(int index)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(string name)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int index)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(string name)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int index)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(string name)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int index)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(string name)
        {
            throw new NotImplementedException();
        }

        public byte[] GetGeometry(int index) => _fgf;

        public byte[] GetGeometry(string name) => _fgf;

        public short GetInt16(int index)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(string name)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int index)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(string name)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int index)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(string name)
        {
            throw new NotImplementedException();
        }

        public LOBValue GetLOB(int index)
        {
            throw new NotImplementedException();
        }

        public LOBValue GetLOB(string name)
        {
            throw new NotImplementedException();
        }

        public IStreamReader GetLOBStreamReader(int index)
        {
            throw new NotImplementedException();
        }

        public IStreamReader GetLOBStreamReader(string name)
        {
            throw new NotImplementedException();
        }

        public int GetPropertyIndex(string name)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyName(int index)
        {
            throw new NotImplementedException();
        }

        public IRaster GetRaster(int index)
        {
            throw new NotImplementedException();
        }

        public IRaster GetRaster(string name)
        {
            throw new NotImplementedException();
        }

        public float GetSingle(int index)
        {
            throw new NotImplementedException();
        }

        public float GetSingle(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int index)
        {
            throw new NotImplementedException();
        }

        public string GetString(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsNull(int index)
        {
            throw new NotImplementedException();
        }

        public bool IsNull(string name) => !(name == _geomProp);

        public bool ReadNext()
        {
            if (!_read)
            {
                _read = true;
                return true;
            }
            return false;
        }
    }
}
