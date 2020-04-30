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
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Data;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Commands;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Geometry;
using System.Collections.Specialized;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Represent a virtual row. This differs from a <see cref="FdoFeature"/> in that this is used 
    /// strictly for ETL purposes. Any geometries are stored as <see cref="IGeometry"/> objects instead
    /// of their raw FGF binary form.
    /// </summary>
    [DebuggerDisplay("Count = {items.Count}")]
    [DebuggerTypeProxy(typeof(FdoToolbox.Core.ETL.QuackingDictionary.QuackingDictionaryDebugView))]
    [Serializable]
    public class FdoRow : QuackingDictionary, IDisposable
    {
        static readonly Dictionary<Type, List<PropertyInfo>> propertiesCache = new Dictionary<Type, List<PropertyInfo>>();
        static readonly Dictionary<Type, List<FieldInfo>> fieldsCache = new Dictionary<Type, List<FieldInfo>>();

        private HashSet<string> readOnlyProperties = new HashSet<string>();

        internal static readonly IEqualityComparer DefaultComparer = StringComparer.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoRow"/> class.
        /// </summary>
        internal FdoRow()
            : base(new Hashtable(DefaultComparer))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoRow"/> class.
        /// </summary>
        /// <param name="itemsToClone">The items to clone.</param>
        private FdoRow(IDictionary itemsToClone)
            : base(new Hashtable(itemsToClone, DefaultComparer))
        {
        }

        /// <summary>
        /// Marks a property as read only.
        /// </summary>
        /// <param name="property">The property name</param>
        public void MarkReadOnly(string property)
        {
            readOnlyProperties.Add(property);
        }

        /// <summary>
        /// Determines whether a specified property is read only
        /// </summary>
        /// <param name="property">The property name</param>
        /// <returns>
        /// 	<c>true</c> if the specified property is read only; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPropertyReadOnly(string property)
        {
            return readOnlyProperties.Contains(property);
        }

        /// <summary>
        /// Creates a copy of the given source, erasing whatever is in the row currently.
        /// </summary>
        /// <param name="source">The source row.</param>
        public void Copy(IDictionary source)
        {
            items = new Hashtable(source, DefaultComparer);
        }

        /// <summary>
        /// Gets the columns in this row.
        /// </summary>
        /// <value>The columns.</value>
        public IEnumerable<string> Columns
        {
            get
            {
                //We likely would want to change the row when iterating on the columns, so we
                //want to make sure that we send a copy, to avoid enumeration modified exception
                foreach (string column in new ArrayList(items.Keys))
                {
                    yield return column;
                }
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public FdoRow Clone()
        {
            FdoRow row = new FdoRow(this);
            return row;
        }

        /// <summary>
        /// Creates a key from the current row, suitable for use in hashtables
        /// </summary>
        public ObjectArrayKeys CreateKey()
        {
            return CreateKey(null);
        }

        /// <summary>
        /// Creates a key that allow to do full or partial indexing on a row
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public ObjectArrayKeys CreateKey(params string[] columns)
        {
            object[] array = new object[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                array[i] = items[columns[i]];
            }
            return new ObjectArrayKeys(array);
        }

        /// <summary>
        /// Copy all the public properties and fields of an object to the row
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static FdoRow FromObject(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            FdoRow row = new FdoRow();
            foreach (PropertyInfo property in GetProperties(obj))
            {
                row[property.Name] = property.GetValue(obj, new object[0]);
            }
            foreach (FieldInfo field in GetFields(obj))
            {
                row[field.Name] = field.GetValue(obj);
            }
            return row;
        }

        private static List<PropertyInfo> GetProperties(object obj)
        {
            List<PropertyInfo> properties;
            if (propertiesCache.TryGetValue(obj.GetType(), out properties))
                return properties;

            properties = new List<PropertyInfo>();
            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (property.CanRead == false || property.GetIndexParameters().Length > 0)
                    continue;
                properties.Add(property);
            }
            propertiesCache[obj.GetType()] = properties;
            return properties;
        }

        private static List<FieldInfo> GetFields(object obj)
        {
            List<FieldInfo> fields;
            if (fieldsCache.TryGetValue(obj.GetType(), out fields))
                return fields;

            fields = new List<FieldInfo>();
            foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                fields.Add(fieldInfo);
            }
            fieldsCache[obj.GetType()] = fields;
            return fields;
        }

        /// <summary>
        /// Generate a row from a data reader
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static FdoRow FromReader(IDataReader reader)
        {
            FdoRow row = new FdoRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if(!reader.IsDBNull(i))
                    row[reader.GetName(i)] = reader.GetValue(i);
            }
            return row;
        }

        /// <summary>
        /// Generate a row from a Feature Reader
        /// </summary>
        /// <param name="reader">The feature reader</param>
        /// <param name="readOnlyProperties">A list of properties to mark as read-only</param>
        /// <returns></returns>
        public static FdoRow FromFeatureReader(FdoFeatureReader reader, ICollection<string> readOnlyProperties)
        {
            FdoRow row = new FdoRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                if (!reader.IsNull(name))
                {
                    if (name == reader.DefaultGeometryProperty)
                    {
                        row.DefaultGeometryProperty = reader.DefaultGeometryProperty;
                        row.AddGeometry(name, reader.GetGeometryObject(name));
                    }
                    else if (Array.IndexOf<string>(reader.GeometryProperties, name) >= 0)
                    {
                        row.AddGeometry(name, reader.GetGeometryObject(name));
                    }
                    else
                    {
                        row[name] = reader.GetFdoDataValue(name);
                    }

                    if (readOnlyProperties.Contains(name))
                    {
                        row.MarkReadOnly(name);
                    }
                }
            }
            return row;
        }

        /// <summary>
        /// Create a new object of <typeparamref name="T"/> and set all
        /// the matching fields/properties on it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToObject<T>()
        {
            return (T)ToObject(typeof(T));
        }

        /// <summary>
        /// Create a new object of <param name="type"/> and set all
        /// the matching fields/properties on it.
        /// </summary>
        public object ToObject(Type type)
        {
            object instance = Activator.CreateInstance(type);
            foreach (PropertyInfo info in GetProperties(instance))
            {
                if (items.Contains(info.Name) && info.CanWrite)
                    info.SetValue(instance, items[info.Name], null);
            }
            foreach (FieldInfo info in GetFields(instance))
            {
                if (items.Contains(info.Name))
                    info.SetValue(instance, items[info.Name]);
            }
            return instance;
        }

        /// <summary>
        /// Converts this feature to a parameter value collection.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public ParameterValueCollection ToParameterValueCollection(string prefix)
        {
            return ToParameterValueCollection(prefix, null, null);
        }

        /// <summary>
        /// Converts this feature to a parameter value collection.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="excludeProperties">The list of properties to exclude.</param>
        /// <returns></returns>
        public ParameterValueCollection ToParameterValueCollection(string prefix, ICollection<string> excludeProperties)
        {
            return ToParameterValueCollection(prefix, null, excludeProperties);
        }

        /// <summary>
        /// Converts this feature to a parameter value collection.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="excludeProperties">The list of properties to exclude.</param>
        /// <returns></returns>
        public ParameterValueCollection ToParameterValueCollection(string prefix, NameValueCollection mappings, ICollection<string> excludeProperties)
        {
            ParameterValueCollection values = new ParameterValueCollection();
            if (mappings == null)
            {
                foreach (string col in this.Columns)
                {
                    //No excluded properties or property not in exclusion list
                    if (excludeProperties == null || excludeProperties.Count == 0 || !excludeProperties.Contains(col))
                    {
                        //Omit null values
                        if (this[col] != null && this[col] != DBNull.Value)
                        {
                            if (!IsGeometryProperty(col))
                            {
                                LiteralValue dv = ValueConverter.GetConvertedValue(this[col]);
                                if (dv != null)
                                {
                                    ParameterValue pv = new ParameterValue(prefix + col, dv);
                                    values.Add(pv);
                                }
                            }
                            else
                            {
                                IGeometry geom = this[col] as IGeometry;
                                if (geom != null)
                                {
                                    ParameterValue pv = new ParameterValue(prefix + col, new GeometryValue(FdoGeometryFactory.Instance.GetFgf(geom)));
                                    values.Add(pv);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (string col in this.Columns)
                {
                    //No excluded properties or property not in exclusion list
                    if (excludeProperties == null || excludeProperties.Count == 0 || !excludeProperties.Contains(col))
                    {
                        //Omit null and un-mapped values
                        if (mappings[col] != null && this[col] != null && this[col] != DBNull.Value)
                        {
                            if (!IsGeometryProperty(col))
                            {
                                LiteralValue dv = ValueConverter.GetConvertedValue(this[col]);
                                if (dv != null)
                                {
                                    ParameterValue pv = new ParameterValue(prefix + col, dv);
                                    values.Add(pv);
                                }
                            }
                            else
                            {
                                IGeometry geom = this[col] as IGeometry;
                                if (geom != null)
                                {
                                    ParameterValue pv = new ParameterValue(prefix + col, new GeometryValue(FdoGeometryFactory.Instance.GetFgf(geom)));
                                    values.Add(pv);
                                }
                            }
                        }
                    }
                }
            }
            return values;
        }

        private List<string> _geometryProperties = new List<string>();

        /// <summary>
        /// Adds a geometry value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal void AddGeometry(string name, object value)
        {
            _geometryProperties.Add(name);
            this[name] = value;
        }

        /// <summary>
        /// Determines whether [is geometry property] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [is geometry property] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsGeometryProperty(string name)
        {
            return _geometryProperties.Contains(name);
        }

        /// <summary>
        /// The default geometry of this row
        /// </summary>
        public string DefaultGeometryProperty { get; internal set; }

        /// <summary>
        /// The geometry object for this feature row
        /// </summary>
        public IGeometry Geometry
        {
            get
            {
                if (DefaultGeometryProperty != null)
                    return this[DefaultGeometryProperty] as IGeometry;
                return null;
            }
            internal set
            {
                if (string.IsNullOrEmpty(DefaultGeometryProperty))
                    throw new InvalidOperationException("No default geometry property name defined");

                this[DefaultGeometryProperty] = value;
            }
        }

        /// <summary>
        /// Creates a new <see cref="FdoRow"/> from a <see cref="FdoFeature"/> instance
        /// </summary>
        /// <param name="feat">The <see cref="FdoFeature"/> instance</param>
        /// <returns>A new <see cref="FdoRow"/> instance</returns>
        public static FdoRow FromFeatureRow(FdoFeature feat)
        {
            if (feat.Table == null)
                throw new InvalidOperationException(ResourceUtil.GetString("ERR_FEATURE_ROW_HAS_NO_PARENT_TABLE"));

            FdoRow row = new FdoRow();
            foreach (DataColumn dc in feat.Table.Columns)
            {
                if (feat[dc] != null && feat[dc] != DBNull.Value)
                {
                    FdoGeometry geom = feat[dc] as FdoGeometry;
                    if (geom != null)
                    {
                        if (feat.Table.GeometryColumn == dc.ColumnName)
                        {
                            row.AddGeometry(dc.ColumnName, geom.InternalInstance);
                            row.DefaultGeometryProperty = dc.ColumnName;
                        }
                        else
                        {
                            row.AddGeometry(dc.ColumnName, geom.InternalInstance);
                        }
                    }
                    else
                    {
                        row[dc.ColumnName] = feat[dc];
                    }
                    if (dc.ReadOnly)
                    {
                        row.MarkReadOnly(dc.ColumnName);
                    }
                }
            }
            return row;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Clear the geometry name list
                _geometryProperties.Clear();
                //Dispose all IDisposable instances. Everything else is CLR objects
                foreach (var col in this.Columns)
                {
                    var disp = this[col] as IDisposable;
                    if (disp != null)
                        disp.Dispose();
                }
                //Clear the property dictionary
                items.Clear();
            }
        }

        ~FdoRow()
        {
            Dispose(false);
        }
    }
}
