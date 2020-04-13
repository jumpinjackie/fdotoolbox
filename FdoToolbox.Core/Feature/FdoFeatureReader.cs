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
using System.Text;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Raster;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO reader wrapper class. Wraps the FDO IFeatureReader interface and
    /// the ADO.net IDataReader interface
    /// </summary>
    public class FdoFeatureReader : FdoReader<IFeatureReader>, IFeatureReader
    {
        private ClassDefinition _classDefinition;
        private string[] _names;
        private Type[] _types;
        private Dictionary<string, int> _ordinals;
        private int _limit = -1;
        private int _read = 0;
        private string[] _geometryNames;
        private string _defaultGeometryName;
        private Dictionary<string, FdoPropertyType> _ptypes;
        private FgfGeometryFactory _internalFactory;
        private Dictionary<string, string> _associations;

        readonly Dictionary<string, Func<object, IFeatureReader>> _fdoDataValueGetters = new Dictionary<string, Func<object, IFeatureReader>>();

        public object GetFdoDataValue(string name) => _fdoDataValueGetters.ContainsKey(name) ? _fdoDataValueGetters[name].Invoke(_internalReader) : null;

        internal FdoFeatureReader(IFeatureReader reader, int limit)
            : this(reader)
        {
            _limit = limit;
        }

        internal FdoFeatureReader(IFeatureReader reader) : base(reader)
        {
            _internalFactory = new FgfGeometryFactory();
            _classDefinition = reader.GetClassDefinition();
            _ptypes = new Dictionary<string, FdoPropertyType>();
            _associations = new Dictionary<string, string>();
            _ordinals = new Dictionary<string, int>();
            _types = new Type[_classDefinition.Properties.Count];
            _names = new string[_classDefinition.Properties.Count];
            List<string> geoms = new List<string>();
            for (int i = 0; i < _names.Length; i++)
            {
                PropertyDefinition pd = _classDefinition.Properties[i];
                _names[i] = pd.Name;
                string name = _names[i];
                _ordinals.Add(_names[i], i);
                
                DataPropertyDefinition dp = pd as DataPropertyDefinition;
                GeometricPropertyDefinition gp = pd as GeometricPropertyDefinition;
                if (dp != null)
                {
                    string pn = dp.Name;
                    switch (dp.DataType)
                    {
                        case DataType.DataType_BLOB:
                        case DataType.DataType_CLOB:
                            _fdoDataValueGetters[pn] = r =>
                            {
                                var lob = r.GetLOB(pn);
                                return lob.Data;
                            };
                            break;
                        case DataType.DataType_Boolean:
                            _fdoDataValueGetters[pn] = r => r.GetBoolean(pn);
                            break;
                        case DataType.DataType_Byte:
                            _fdoDataValueGetters[pn] = r => r.GetByte(pn);
                            break;
                        case DataType.DataType_DateTime:
                            _fdoDataValueGetters[pn] = r => r.GetDateTime(pn);
                            break;
                        case DataType.DataType_Decimal:
                        case DataType.DataType_Double:
                            _fdoDataValueGetters[pn] = r => r.GetDouble(pn);
                            break;
                        case DataType.DataType_Int16:
                            _fdoDataValueGetters[pn] = r => r.GetInt16(pn);
                            break;
                        case DataType.DataType_Int32:
                            _fdoDataValueGetters[pn] = r => r.GetInt32(pn);
                            break;
                        case DataType.DataType_Int64:
                            _fdoDataValueGetters[pn] = r => r.GetInt64(pn);
                            break;
                        case DataType.DataType_Single:
                            _fdoDataValueGetters[pn] = r => r.GetSingle(pn);
                            break;
                        case DataType.DataType_String:
                            _fdoDataValueGetters[pn] = r => r.GetString(pn);
                            break;
                    }
                    _types[i] = ExpressUtility.GetClrTypeFromFdoDataType(dp.DataType);
                    _ptypes[name] = ValueConverter.FromDataType(dp.DataType);
                }
                else if (gp != null)
                {
                    _types[i] = typeof(byte[]);
                    geoms.Add(gp.Name);
                    _ptypes[name] = FdoPropertyType.Geometry;
                    _associations[name] = gp.SpatialContextAssociation;
                }
                else if (pd.PropertyType == PropertyType.PropertyType_ObjectProperty)
                {
                    _ptypes[name] = FdoPropertyType.Object;
                }
                else if (pd.PropertyType == PropertyType.PropertyType_RasterProperty)
                {
                    _ptypes[name] = FdoPropertyType.Raster;
                    _types[i] = typeof(IRaster);
                }
                else if (pd.PropertyType == PropertyType.PropertyType_AssociationProperty)
                {
                    _ptypes[name] = FdoPropertyType.Association;
                }
            }
            _geometryNames = geoms.ToArray();
            if (_classDefinition is FeatureClass && (_classDefinition as FeatureClass).GeometryProperty != null)
                _defaultGeometryName = (_classDefinition as FeatureClass).GeometryProperty.Name;
            else if(geoms.Count > 0)
                _defaultGeometryName = geoms[0];
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _classDefinition.Dispose();
                _internalFactory.Dispose();
            }
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <value></value>
        /// <returns>The level of nesting.</returns>
        public override int Depth
        {
            get { return _internalReader.GetDepth(); }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value></value>
        /// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
        public override int FieldCount
        {
            get { return _names.Length; }
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
        /// Gets the class definition.
        /// </summary>
        /// <returns></returns>
        public ClassDefinition GetClassDefinition()
        {
            return _classDefinition;
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <returns></returns>
        public int GetDepth()
        {
            return _internalReader.GetDepth();
        }

        /// <summary>
        /// Gets the feature object.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public IFeatureReader GetFeatureObject(string propertyName)
        {
            return _internalReader.GetFeatureObject(propertyName);
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
        /// Reads the next.
        /// </summary>
        /// <returns></returns>
        public override bool ReadNext()
        {
            if (_limit > 0)
            {
                bool read = false;
                if (_limit > _read)
                {
                    read = base.ReadNext();
                    if (read)
                        _read++;
                }
                return read;
            }
            else 
            {
                return base.ReadNext();
            }
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public override bool Read()
        {
            return this.ReadNext();
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public override bool NextResult()
        {
            return this.ReadNext();
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
        /// Gets the geometry object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IGeometry GetGeometryObject(string name)
        {
            byte[] fgf = GetGeometry(name);
            return _internalFactory.CreateGeometryFromFgf(fgf);
        }

        /// <summary>
        /// Gets the spatial context association for a geometry property
        /// </summary>
        /// <param name="name">The name of the geometry property</param>
        /// <returns></returns>
        public override string GetSpatialContextAssociation(string name)
        {
            if (_associations.ContainsKey(name))
                return _associations[name];

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
            return _classDefinition.IdentityProperties.Contains(name);
        }

        /// <summary>
        /// Gets the name of the feature class that this reader originates from. If this reader was
        /// produced from a SQL or aggregate query, an empty string is returned.
        /// </summary>
        /// <returns></returns>
        public override string GetClassName()
        {
            return _classDefinition.Name;
        }


        public IFeatureReader GetFeatureObject(int index)
        {
            return _internalReader.GetFeatureObject(index);
        }
    }
}
