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
using OSGeo.FDO.Schema;
using System.Data;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Feature
{
    //
    // This should really derive from FdoReader<T>, but since ISQLDataReader does
    // not derive from IReader, that is not possible. Until that is fixed in FDO itself
    // we have no choice but to duplicate the FdoReader implementation, ugh!
    // 
    // http://trac.osgeo.org/fdo/ticket/359
    // 
    // Update 14/9/2008: All adapter classes implement IFdoReader. This does not fix the original
    // issue, but it does give us an interface that we can work with any reader adapter class.
    //

    /// <summary>
    /// FDO reader wrapper class. Wraps the FDO ISQLDataReader interface and the 
    /// ADO.net IDataReader interface
    /// </summary>
    public class FdoSqlReader : ISQLDataReader, IFdoReader
    {
        private Dictionary<string, int> _ordinals;
        private string[] _names;
        private Type[] _types;
        private ISQLDataReader _internalReader;
        private string[] _geometryNames;
        private string _defaultGeometryName;
        private Dictionary<string, FdoPropertyType> _ptypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoSqlReader"/> class.
        /// </summary>
        protected FdoSqlReader() { }

        internal FdoSqlReader(ISQLDataReader reader)
        {
            _internalReader = reader;
            _ordinals = new Dictionary<string, int>();
            int count = reader.GetColumnCount();
            _types = new Type[count];
            _names = new string[count];
            _ptypes = new Dictionary<string, FdoPropertyType>();
            List<string> geoms = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string name = _internalReader.GetColumnName(i);
                _names[i] = name;
                _ordinals.Add(name, i);

                PropertyType ptype = _internalReader.GetPropertyType(name);
                if (ptype == PropertyType.PropertyType_DataProperty)
                {
                    _types[i] = ExpressUtility.GetClrTypeFromFdoDataType(_internalReader.GetColumnType(name));
                    _ptypes[name] = ValueConverter.FromDataType(_internalReader.GetColumnType(name));
                }
                else if (ptype == PropertyType.PropertyType_GeometricProperty)
                {
                    _types[i] = typeof(byte[]);
                    geoms.Add(name);
                    _ptypes[name] = FdoPropertyType.Geometry;
                }
                else if (ptype == PropertyType.PropertyType_AssociationProperty)
                {
                    _ptypes[name] = FdoPropertyType.Association;
                }
                else if (ptype == PropertyType.PropertyType_ObjectProperty)
                {
                    _ptypes[name] = FdoPropertyType.Object;
                }
                else if (ptype == PropertyType.PropertyType_RasterProperty)
                {
                    _ptypes[name] = FdoPropertyType.Raster;
                }
            }
            _geometryNames = geoms.ToArray();
            if (geoms.Count > 0)
                _defaultGeometryName = geoms[0];
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <value></value>
        /// <returns>The level of nesting.</returns>
        public virtual int Depth
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
        public virtual System.Data.DataTable GetSchemaTable()
        {
            System.Data.DataTable schemaTable = new System.Data.DataTable();

            schemaTable.Columns.Add("ColumnName", typeof(String));
            schemaTable.Columns.Add("ColumnOrdinal", typeof(Int32));
            schemaTable.Columns.Add("ColumnSize", typeof(Int32));
            schemaTable.Columns.Add("NumericPrecision", typeof(Int32));
            schemaTable.Columns.Add("NumericScale", typeof(Int32));
            schemaTable.Columns.Add("IsUnique", typeof(Boolean));
            schemaTable.Columns.Add("IsKey", typeof(Boolean));
            schemaTable.Columns.Add("BaseCatalogName", typeof(String));
            schemaTable.Columns.Add("BaseColumnName", typeof(String));
            schemaTable.Columns.Add("BaseSchemaName", typeof(String));
            schemaTable.Columns.Add("BaseTableName", typeof(String));
            schemaTable.Columns.Add("DataType", typeof(Type));
            schemaTable.Columns.Add("AllowDBNull", typeof(Boolean));
            schemaTable.Columns.Add("ProviderType", typeof(Int32));
            schemaTable.Columns.Add("IsAliased", typeof(Boolean));
            schemaTable.Columns.Add("IsExpression", typeof(Boolean));
            schemaTable.Columns.Add("IsIdentity", typeof(Boolean));
            schemaTable.Columns.Add("IsAutoIncrement", typeof(Boolean));
            schemaTable.Columns.Add("IsRowVersion", typeof(Boolean));
            schemaTable.Columns.Add("IsHidden", typeof(Boolean));
            schemaTable.Columns.Add("IsLong", typeof(Boolean));
            schemaTable.Columns.Add("IsReadOnly", typeof(Boolean));

            schemaTable.BeginLoadData();
            for (int i = 0; i < this.FieldCount; i++)
            {
                System.Data.DataRow schemaRow = schemaTable.NewRow();

                schemaRow["ColumnName"] = GetName(i);
                schemaRow["ColumnOrdinal"] = i;
                schemaRow["ColumnSize"] = -1;
                schemaRow["NumericPrecision"] = 0;
                schemaRow["NumericScale"] = 0;
                schemaRow["IsUnique"] = false;
                schemaRow["IsKey"] = false;
                schemaRow["BaseCatalogName"] = "";
                schemaRow["BaseColumnName"] = GetName(i);
                schemaRow["BaseSchemaName"] = "";
                schemaRow["BaseTableName"] = "";
                schemaRow["DataType"] = GetFieldType(i);
                schemaRow["AllowDBNull"] = true;
                schemaRow["ProviderType"] = 0;
                schemaRow["IsAliased"] = false;
                schemaRow["IsExpression"] = false;
                schemaRow["IsIdentity"] = false;
                schemaRow["IsAutoIncrement"] = false;
                schemaRow["IsRowVersion"] = false;
                schemaRow["IsHidden"] = false;
                schemaRow["IsLong"] = false;
                schemaRow["IsReadOnly"] = false;

                schemaTable.Rows.Add(schemaRow);
                schemaRow.AcceptChanges();

            }
            schemaTable.EndLoadData();

            return schemaTable;
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value></value>
        /// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
        public virtual int FieldCount
        {
            get { return _internalReader.GetColumnCount(); }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual Type GetFieldType(int i)
        {
            return _types[i];
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The index of the named field.</returns>
        public virtual int GetOrdinal(string name)
        {
            return _ordinals[name];   
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <returns></returns>
        public virtual int GetColumnCount()
        {
            return _internalReader.GetColumnCount();
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public virtual string GetColumnName(int index)
        {
            return _internalReader.GetColumnName(index);
        }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual OSGeo.FDO.Schema.DataType GetColumnType(string name)
        {
            return _internalReader.GetColumnType(name);
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual OSGeo.FDO.Schema.PropertyType GetPropertyType(string name)
        {
            return _internalReader.GetPropertyType(name);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close()
        {
            _internalReader.Close();
        }

        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual bool GetBoolean(string name)
        {
            return _internalReader.GetBoolean(name);
        }

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual byte GetByte(string name)
        {
            return _internalReader.GetByte(name);
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(string name)
        {
            return _internalReader.GetDateTime(name);
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual double GetDouble(string name)
        {
            return _internalReader.GetDouble(name);
        }

        /// <summary>
        /// Gets the geometry.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual byte[] GetGeometry(string name)
        {
            return _internalReader.GetGeometry(name);
        }

        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual short GetInt16(string name)
        {
            return _internalReader.GetInt16(name);
        }

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual int GetInt32(string name)
        {
            return _internalReader.GetInt32(name);
        }

        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual long GetInt64(string name)
        {
            return _internalReader.GetInt64(name);
        }

        /// <summary>
        /// Gets the LOB.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual OSGeo.FDO.Expression.LOBValue GetLOB(string name)
        {
            return _internalReader.GetLOB(name);
        }

        /// <summary>
        /// Gets the LOB stream reader.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual OSGeo.FDO.Common.IStreamReader GetLOBStreamReader(string name)
        {
            return _internalReader.GetLOBStreamReader(name);
        }

        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual float GetSingle(string name)
        {
            return _internalReader.GetSingle(name);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual string GetString(string name)
        {
            return _internalReader.GetString(name);
        }

        /// <summary>
        /// Determines whether the specified property name is null.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified name is null; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsNull(string name)
        {
            return _internalReader.IsNull(name);
        }

        /// <summary>
        /// Reads the next feature/row
        /// </summary>
        /// <returns></returns>
        public virtual bool ReadNext()
        {
            //HACK
            bool read = false;
            try
            {
                read = _internalReader.ReadNext();
            }
            catch
            {
                read = false;
            }
            return read;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _internalReader.Dispose();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <value></value>
        /// <returns>true if the data reader is closed; otherwise, false.</returns>
        public virtual bool IsClosed
        {
            get { return false; }
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public virtual bool NextResult()
        {
            return this.ReadNext();
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public virtual bool Read()
        {
            return this.ReadNext();
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <value></value>
        /// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
        public virtual int RecordsAffected
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual bool GetBoolean(int i)
        {
            return _internalReader.GetBoolean(i);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual byte GetByte(int i)
        {
            return _internalReader.GetByte(i);
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
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual char GetChar(int i)
        {
            throw new Exception("The method or operation is not implemented.");
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
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader"/> for the specified column ordinal.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/>.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual System.Data.IDataReader GetData(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual string GetDataTypeName(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual DateTime GetDateTime(int i)
        {
            return _internalReader.GetDateTime(i);
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(_internalReader.GetDouble(i));
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual double GetDouble(int i)
        {
            return _internalReader.GetDouble(i);
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual float GetFloat(int i)
        {
            return _internalReader.GetSingle(i);
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The GUID value of the specified field.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual Guid GetGuid(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual short GetInt16(int i)
        {
            return _internalReader.GetInt16(i);
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual int GetInt32(int i)
        {
            return _internalReader.GetInt32(i);
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual long GetInt64(int i)
        {
            return _internalReader.GetInt64(i);
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual string GetName(int i)
        {
            return _internalReader.GetColumnName(i);
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The string value of the specified field.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual string GetString(int i)
        {
            return _internalReader.GetString(i);
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Object"/> which will contain the field value upon return.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual object GetValue(int i)
        {
            Type t = GetFieldType(i);
            string name = GetName(i);
            if (t == typeof(bool))
                return GetBoolean(name);
            else if (t == typeof(byte))
                return GetByte(name);
            else if (t == typeof(DateTime))
                return GetDateTime(name);
            else if (t == typeof(decimal))
                return GetDecimal(i);
            else if (t == typeof(double))
                return GetDouble(name);
            else if (t == typeof(short))
                return GetInt16(name);
            else if (t == typeof(int))
                return GetInt32(name);
            else if (t == typeof(long))
                return GetInt64(name);
            else if (t == typeof(float))
                return GetSingle(name);
            else if (t == typeof(string))
                return GetString(name);
            return DBNull.Value;
        }

        /// <summary>
        /// Gets all the attribute fields in the collection for the current record.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object"/> to copy the attribute fields into.</param>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object"/> in the array.
        /// </returns>
        public virtual int GetValues(object[] values)
        {
            int numToFill = System.Math.Min(values.Length, this.FieldCount);
            for (int i = 0; i < numToFill; i++)
            {
                string name = GetName(i);
                if (!IsNull(name))
                    values[i] = this[i];
                else
                    values[i] = DBNull.Value;
            }
            return numToFill;
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
        public virtual bool IsDBNull(int i)
        {
            return _internalReader.IsNull(i);
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public virtual object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified i.
        /// </summary>
        /// <value></value>
        public virtual object this[int i]
        {
            get { return GetValue(i); }
        }

        /// <summary>
        /// Gets the geometry properties.
        /// </summary>
        /// <value>The geometry properties.</value>
        public virtual string[] GeometryProperties
        {
            get { return _geometryNames; }
        }

        /// <summary>
        /// Gets the default geometry property.
        /// </summary>
        /// <value>The default geometry property.</value>
        public virtual string DefaultGeometryProperty
        {
            get { return _defaultGeometryName; }
        }

        /// <summary>
        /// Gets the type of the fdo property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual FdoPropertyType GetFdoPropertyType(string name)
        {
            return _ptypes[name];
        }

        /// <summary>
        /// Gets the spatial context association for a geometry property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetSpatialContextAssociation(string name)
        {
            //ISQLReader does not hold a class definition so this information is not available
            return string.Empty;
        }

        /// <summary>
        /// Determines whether the specified property name is an identity property
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property is an identity property; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsIdentity(string name)
        {
            //Because there is no way to infer the structure behind a SQL reader this will always be false.
            //If FDO RFC 33 gets ratified and implemented, we may be able to change this
            return false;
        }

        /// <summary>
        /// Gets the name of the feature class that this reader originates from. If this reader was
        /// produced from a SQL or aggregate query, an empty string is returned.
        /// </summary>
        /// <returns></returns>
        public virtual string GetClassName()
        {
            return string.Empty;
        }


        public int GetColumnIndex(string name)
        {
            return _internalReader.GetColumnIndex(name);
        }

        public DataType GetColumnType(int index)
        {
            return _internalReader.GetColumnType(index);
        }

        public byte[] GetGeometry(int index)
        {
            return _internalReader.GetGeometry(index);
        }

        public OSGeo.FDO.Expression.LOBValue GetLOB(int index)
        {
            return _internalReader.GetLOB(index);
        }

        public OSGeo.FDO.Common.IStreamReader GetLOBStreamReader(int index)
        {
            return _internalReader.GetLOBStreamReader(index);
        }

        public PropertyType GetPropertyType(int index)
        {
            return _internalReader.GetPropertyType(index);
        }

        public float GetSingle(int index)
        {
            return _internalReader.GetSingle(index);
        }

        public bool IsNull(int index)
        {
            return _internalReader.IsNull(index);
        }
    }
}
