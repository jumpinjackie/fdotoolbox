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
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO reader wrapper class. Wraps the FDO IDataReader interface and the 
    /// ADO.net IDataReader interface
    /// </summary>
    public class FdoDataReader : FdoReader<IDataReader>, IDataReader
    {
        private string[] _names;
        private Dictionary<string, int> _ordinals;
        private Type[] _types;
        private string[] _geometryNames;
        private string _defaultGeometryName;
        private Dictionary<string, FdoPropertyType> _ptypes;

        internal FdoDataReader(IDataReader reader)
            : base(reader)
        {
            _ordinals = new Dictionary<string, int>();
            _ptypes = new Dictionary<string, FdoPropertyType>();
            _names = new string[reader.GetPropertyCount()];
            _types = new Type[reader.GetPropertyCount()];
            List<string> geoms = new List<string>();
            for (int i = 0; i < _names.Length; i++)
            {
                string name = reader.GetPropertyName(i);
                _names[i] = name;
                _ordinals.Add(name, i);

                PropertyType ptype = reader.GetPropertyType(name);
                if (ptype == PropertyType.PropertyType_DataProperty)
                {
                    _types[i] = ExpressUtility.GetClrTypeFromFdoDataType(reader.GetDataType(name));
                    _ptypes[name] = ValueConverter.FromDataType(reader.GetDataType(name));
                }
                else if (ptype == PropertyType.PropertyType_GeometricProperty)
                {
                    _types[i] = typeof(byte[]);
                    _ptypes[name] = FdoPropertyType.Geometry;
                    geoms.Add(name);
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
            if(geoms.Count > 0)
                _defaultGeometryName = geoms[0];
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
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value></value>
        /// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
        public override int FieldCount
        {
            get { return _internalReader.GetPropertyCount(); }
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
            return _types[i];
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The index of the named field.</returns>
        public override int GetOrdinal(string name)
        {
            return _ordinals[name];
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public OSGeo.FDO.Schema.DataType GetDataType(string name)
        {
            return _internalReader.GetDataType(name);
        }

        /// <summary>
        /// Gets the property count.
        /// </summary>
        /// <returns></returns>
        public int GetPropertyCount()
        {
            return _internalReader.GetPropertyCount();
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetPropertyName(int index)
        {
            return _internalReader.GetPropertyName(index);
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public OSGeo.FDO.Schema.PropertyType GetPropertyType(string name)
        {
            return _internalReader.GetPropertyType(name);
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
            return _names[i];
        }

        /// <summary>
        /// Gets the geometry properties.
        /// </summary>
        /// <value>The geometry properties.</value>
        public override string[] GeometryProperties
        {
            get { return _geometryNames; }
        }

        /// <summary>
        /// Gets the default geometry property.
        /// </summary>
        /// <value>The default geometry property.</value>
        public override string DefaultGeometryProperty
        {
            get { return _defaultGeometryName; }
        }

        /// <summary>
        /// Gets the type of the fdo property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override FdoPropertyType GetFdoPropertyType(string name)
        {
            return _ptypes[name];
        }

        /// <summary>
        /// Gets the spatial context association for a geometry property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetSpatialContextAssociation(string name)
        {
            //IDataReader does not hold a class definition, so this information is not available
            return string.Empty;
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
            //Because there is no way to infer the structure behind a data reader this will always be false.
            return false;
        }

        /// <summary>
        /// Gets the name of the feature class that this reader originates from. If this reader was
        /// produced from a SQL or aggregate query, an empty string is returned.
        /// </summary>
        /// <returns></returns>
        public override string GetClassName()
        {
            return string.Empty;
        }


        public DataType GetDataType(int index)
        {
            return _internalReader.GetDataType(index);
        }

        public PropertyType GetPropertyType(int index)
        {
            return _internalReader.GetPropertyType(index);
        }
    }
}
