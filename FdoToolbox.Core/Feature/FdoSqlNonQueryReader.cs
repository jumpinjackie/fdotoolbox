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
using OSGeo.FDO.Commands.SQL;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// A SQL reader that contains the result of a non-SELECT SQL query
    /// </summary>
    public class FdoSqlNonQueryReader : FdoSqlReader
    {
        private string className = "SQLQueryResult";
        private string field = "RecordsAffected";
        private int affected;
        private bool hasRead;
        private bool isClosed;

        internal FdoSqlNonQueryReader(int affected) : base()
        {
            this.affected = affected;
            this.hasRead = false;
            this.isClosed = false;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            this.isClosed = true;
        }

        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override bool GetBoolean(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override byte GetByte(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <returns></returns>
        public override int GetColumnCount()
        {
            return 1;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override string GetColumnName(int index)
        {
            if (index == 0)
                return field;
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override OSGeo.FDO.Schema.DataType GetColumnType(string name)
        {
            return OSGeo.FDO.Schema.DataType.DataType_Int64;
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override DateTime GetDateTime(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override double GetDouble(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the geometry.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override byte[] GetGeometry(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override short GetInt16(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override int GetInt32(string name)
        {
            return this.affected;
        }

        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override long GetInt64(string name)
        {
            return this.affected;
        }

        /// <summary>
        /// Gets the LOB.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override OSGeo.FDO.Expression.LOBValue GetLOB(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the LOB stream reader.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override OSGeo.FDO.Common.IStreamReader GetLOBStreamReader(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override OSGeo.FDO.Schema.PropertyType GetPropertyType(string name)
        {
            if (name == field)
                return OSGeo.FDO.Schema.PropertyType.PropertyType_DataProperty;
            else
                throw new ArgumentException(name);
        }

        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override float GetSingle(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string GetString(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the specified name is null.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified name is null; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsNull(string name)
        {
            if (field == name)
                return false;

            throw new ArgumentException(name);
        }

        /// <summary>
        /// Reads the next.
        /// </summary>
        /// <returns></returns>
        public override bool ReadNext()
        {
            if (!hasRead)
            {
                hasRead = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            
        }

        /// <summary>
        /// Gets the type of the fdo property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override FdoPropertyType GetFdoPropertyType(string name)
        {
            if (name == field)
                return FdoPropertyType.Int64;
            throw new ArgumentException(name);
        }

        private string[] geomProps = { };

        /// <summary>
        /// Gets the geometry properties.
        /// </summary>
        /// <value>The geometry properties.</value>
        public override string[] GeometryProperties
        {
            get { return geomProps; }
        }

        /// <summary>
        /// Gets the default geometry property.
        /// </summary>
        /// <value>The default geometry property.</value>
        public override string DefaultGeometryProperty
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the spatial context association for a geometry property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetSpatialContextAssociation(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the specified property name is an identity property
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property is an identity property; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsIdentity(string name)
        {
            return false;
        }

        /// <summary>
        /// Gets the name of the feature class that this reader originates from. If this reader was
        /// produced from a SQL or aggregate query, an empty string is returned.
        /// </summary>
        /// <returns></returns>
        public override string GetClassName()
        {
            return this.className;
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <value></value>
        /// <returns>The level of nesting.</returns>
        public override int Depth
        {
            get { return -1; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable"/> that describes the column metadata of the <see cref="T:System.Data.IDataReader"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable"/> that describes the column metadata.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.IDataReader"/> is closed. </exception>
        public override System.Data.DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <value></value>
        /// <returns>true if the data reader is closed; otherwise, false.</returns>
        public override bool IsClosed
        {
            get { return this.isClosed; }
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public override bool NextResult()
        {
            return ReadNext();
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public override bool Read()
        {
            return ReadNext();
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <value></value>
        /// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
        public override int RecordsAffected
        {
            get { return affected; }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value></value>
        /// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
        public override int FieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override bool GetBoolean(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override byte GetByte(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override char GetChar(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of characters read.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader"/> for the specified column ordinal.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/>.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override System.Data.IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override string GetDataTypeName(int i)
        {
            return GetFieldType(i).Name;
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override DateTime GetDateTime(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override decimal GetDecimal(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override double GetDouble(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override Type GetFieldType(int i)
        {
            if (i == 0)
                return typeof(int);

            throw new ArgumentException("i");
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override float GetFloat(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The GUID value of the specified field.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override short GetInt16(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override int GetInt32(int i)
        {
            if (i == 0)
                return affected;

            throw new ArgumentException("i");
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override long GetInt64(int i)
        {
            if (i == 0)
                return affected;

            throw new ArgumentException("i");
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override string GetName(int i)
        {
            if (i == 0)
                return field;

            throw new ArgumentException("i");
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The index of the named field.</returns>
        public override int GetOrdinal(string name)
        {
            if (name == field)
                return 0;

            throw new ArgumentException(name);
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The string value of the specified field.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override string GetString(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Object"/> which will contain the field value upon return.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override object GetValue(int i)
        {
            if (i == 0)
                return affected;

            throw new ArgumentException("i");
        }

        /// <summary>
        /// Gets all the attribute fields in the collection for the current record.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object"/> to copy the attribute fields into.</param>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object"/> in the array.
        /// </returns>
        public override int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public override bool IsDBNull(int i)
        {
            return IsNull(GetName(i));
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public override object this[string name]
        {
            get {
                return GetValue(GetOrdinal(name));
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified i.
        /// </summary>
        /// <value></value>
        public override object this[int i]
        {
            get { return GetValue(i); }
        }
    }
}
