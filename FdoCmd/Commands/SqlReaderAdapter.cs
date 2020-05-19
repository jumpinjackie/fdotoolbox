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
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Common;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Raster;
using System;

namespace FdoCmd.Commands
{
    public class SqlReaderAdapter : IReader
    {
        readonly ISQLDataReader _reader;

        public SqlReaderAdapter(ISQLDataReader reader)
        {
            _reader = reader;
        }

        public void Close() => _reader.Close();

        public void Dispose() => _reader.Dispose();

        public bool GetBoolean(int index) => _reader.GetBoolean(index);

        public bool GetBoolean(string name) => _reader.GetBoolean(name);

        public byte GetByte(int index) => _reader.GetByte(index);

        public byte GetByte(string name) => _reader.GetByte(name);

        public DateTime GetDateTime(int index) => _reader.GetDateTime(index);

        public DateTime GetDateTime(string name) => _reader.GetDateTime(name);

        public double GetDouble(int index) => _reader.GetDouble(index);

        public double GetDouble(string name) => _reader.GetDouble(name);

        public byte[] GetGeometry(int index) => _reader.GetGeometry(index);

        public byte[] GetGeometry(string name) => _reader.GetGeometry(name);

        public short GetInt16(int index) => _reader.GetInt16(index);

        public short GetInt16(string name) => _reader.GetInt16(name);

        public int GetInt32(int index) => _reader.GetInt32(index);

        public int GetInt32(string name) => _reader.GetInt32(name);

        public long GetInt64(int index) => _reader.GetInt64(index);

        public long GetInt64(string name) => _reader.GetInt64(name);

        public LOBValue GetLOB(int index) => _reader.GetLOB(index);

        public LOBValue GetLOB(string name) => _reader.GetLOB(name);

        public IStreamReader GetLOBStreamReader(int index) => _reader.GetLOBStreamReader(index);

        public IStreamReader GetLOBStreamReader(string name) => _reader.GetLOBStreamReader(name);

        public int GetPropertyIndex(string name) => _reader.GetColumnIndex(name);

        public string GetPropertyName(int index) => _reader.GetColumnName(index);

        public IRaster GetRaster(int index) => throw new NotImplementedException();

        public IRaster GetRaster(string name) => throw new NotImplementedException();

        public float GetSingle(int index) => _reader.GetSingle(index);

        public float GetSingle(string name) => _reader.GetSingle(name);

        public string GetString(int index) => _reader.GetString(index);

        public string GetString(string name) => _reader.GetString(name);

        public bool IsNull(int index) => _reader.IsNull(index);

        public bool IsNull(string name) => _reader.IsNull(name);

        public bool ReadNext() => _reader.ReadNext();
    }
}
