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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FdoToolbox.Core.Connections;
using FdoToolbox.Core.Feature.Overrides;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.DataStore;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Common.Xml;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Connections.Capabilities;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Filter;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;
using Res = FdoToolbox.Core.ResourceUtil;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// IConnection wrapper service object.
    /// </summary>
    public class FdoFeatureService : IDisposable
    {
        private static IList<FdoProviderInfo> _providerInfo;

        /// <summary>
        /// Gets the list of providers from the provider registry
        /// </summary>
        /// <returns></returns>
        public static IList<FdoProviderInfo> GetProviders()
        {
            if (_providerInfo == null)
            {
                _providerInfo = new List<FdoProviderInfo>();

                ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders();
                foreach (Provider prov in providers)
                {
                    FdoProviderInfo pi = new FdoProviderInfo
                    {
                        Description = prov.Description,
                        DisplayName = prov.DisplayName,
                        FeatureDataObjectsVersion = prov.FeatureDataObjectsVersion,
                        IsManaged = prov.IsManaged,
                        LibraryPath = prov.LibraryPath,
                        Name = prov.Name,
                        Version = prov.Version
                    };

                    IConnection conn = null;
                    try
                    {
                        conn = FeatureAccessManager.GetConnectionManager().CreateConnection(pi.Name);
                        pi.IsFlatFile = conn.ConnectionInfo.ProviderDatastoreType == ProviderDatastoreType.ProviderDatastoreType_File;
                        _providerInfo.Add(pi);
                    }
                    catch
                    { }
                    finally
                    {
                        if(conn != null)
                            conn.Dispose();
                    }
                }
            }
            return _providerInfo;
        }

        public List<string> GetQualifiedClassNames() => _walker.GetQualifiedClassNames();

        private SchemaWalker _walker;

        private static Dictionary<string, IList<DictionaryProperty>> _connectProperties = new Dictionary<string, IList<DictionaryProperty>>();
        
        /// <summary>
        /// Gets the parameters required to create a connection
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IList<DictionaryProperty> GetConnectProperties(string provider)
        {
            if (_connectProperties.ContainsKey(provider))
                return _connectProperties[provider];

            _connectProperties[provider] = new List<DictionaryProperty>();
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            IConnectionPropertyDictionary dict = conn.ConnectionInfo.ConnectionProperties;
            string [] names = dict.PropertyNames;
            foreach (string n in names)
            {
                DictionaryProperty p = null;
                if (dict.IsPropertyEnumerable(n))
                {
                    EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty();
                    try
                    {
                        ep.Values = dict.EnumeratePropertyValues(n);
                        //A bit of a hack, but if a property is enumerable, and we get nothing
                        //then we assume a connection must be established first.
                        ep.RequiresConnection = (ep.Values.Length == 0); 
                    }
                    catch
                    {
                        ep.RequiresConnection = true;
                    }
                    p = ep;
                }
                else 
                {
                    p = new DictionaryProperty();
                }
                p.IsFile = dict.IsPropertyFileName(n);
                p.IsPath = dict.IsPropertyFilePath(n);
                p.DefaultValue = dict.GetPropertyDefault(n);
                p.LocalizedName = dict.GetLocalizedName(n);
                p.Name = n;
                p.Required = dict.IsPropertyRequired(n);
                p.Protected = dict.IsPropertyProtected(n);
                _connectProperties[provider].Add(p);
            }
            dict.Dispose();
            conn.Dispose();
            return _connectProperties[provider];
        }

        private static Dictionary<string, IList<DictionaryProperty>> _createDataStoreProperties = new Dictionary<string, IList<DictionaryProperty>>();
        private static Dictionary<string, IList<DictionaryProperty>> _destroyDataStoreProperties = new Dictionary<string, IList<DictionaryProperty>>();

        /// <summary>
        /// Gets the parameters required to create a datastore
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IList<DictionaryProperty> GetCreateDataStoreProperties(string provider)
        {
            if (_createDataStoreProperties.ContainsKey(provider))
                return _createDataStoreProperties[provider];

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_CreateDataStore) >= 0)
            {
                _createDataStoreProperties[provider] = new List<DictionaryProperty>();
                using (ICreateDataStore create = conn.CreateCommand(CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                {
                    IDataStorePropertyDictionary dict = create.DataStoreProperties;
                    string[] names = dict.PropertyNames;
                    foreach (string n in names)
                    {
                        DictionaryProperty p = null;
                        if (dict.IsPropertyEnumerable(n))
                        {
                            EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty
                            {
                                Values = dict.EnumeratePropertyValues(n)
                            };
                            p = ep;
                        }
                        else
                        {
                            p = new DictionaryProperty();
                        }
                        p.IsFile = dict.IsPropertyFileName(n);
                        p.IsPath = dict.IsPropertyFilePath(n);
                        p.DefaultValue = dict.GetPropertyDefault(n);
                        p.LocalizedName = dict.GetLocalizedName(n);
                        p.Name = n;
                        p.Required = dict.IsPropertyRequired(n);
                        p.Protected = dict.IsPropertyProtected(n);
                        _createDataStoreProperties[provider].Add(p);
                    }
                    dict.Dispose();
                }
                conn.Dispose();
                return _createDataStoreProperties[provider];
            }
            return null;
        }

        /// <summary>
        /// Gets the parameters required to destory a data store
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IList<DictionaryProperty> GetDestroyDataStoreProperties(string provider)
        {
            if (_destroyDataStoreProperties.ContainsKey(provider))
                return _destroyDataStoreProperties[provider];

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_CreateDataStore) >= 0)
            {
                _destroyDataStoreProperties[provider] = new List<DictionaryProperty>();
                using (IDestroyDataStore destroy = conn.CreateCommand(CommandType.CommandType_DestroyDataStore) as IDestroyDataStore)
                {
                    IDataStorePropertyDictionary dict = destroy.DataStoreProperties;
                    string[] names = dict.PropertyNames;
                    foreach (string n in names)
                    {
                        DictionaryProperty p = null;
                        if (dict.IsPropertyEnumerable(n))
                        {
                            EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty
                            {
                                Values = dict.EnumeratePropertyValues(n)
                            };
                            p = ep;
                        }
                        else
                        {
                            p = new DictionaryProperty();
                        }
                        p.IsFile = dict.IsPropertyFileName(n);
                        p.IsPath = dict.IsPropertyFilePath(n);
                        p.DefaultValue = dict.GetPropertyDefault(n);
                        p.LocalizedName = dict.GetLocalizedName(n);
                        p.Name = n;
                        p.Required = dict.IsPropertyRequired(n);
                        p.Protected = dict.IsPropertyProtected(n);
                        _createDataStoreProperties[provider].Add(p);
                    }
                    dict.Dispose();
                }
                conn.Dispose();
                return _destroyDataStoreProperties[provider];
            }
            return null;
        }

        private FgfGeometryFactory _GeomFactory;

        /// <summary>
        /// Gets the FDO Geometry factory instance
        /// </summary>
        public FgfGeometryFactory GeometryFactory => _GeomFactory;

        readonly SchemaCapabilityChecker _schemaChecker;

        /// <summary>
        /// Constructor. The passed connection must already be open.
        /// </summary>
        /// <param name="conn"></param>
        public FdoFeatureService(IConnection conn)
        {
            if (conn.ConnectionState == ConnectionState.ConnectionState_Closed)
                throw new FeatureServiceException(Res.GetString("ERR_CONNECTION_NOT_OPEN"));
            Connection = conn;
            var sc = conn.SchemaCapabilities;
            _schemaChecker = new SchemaCapabilityChecker(sc);
            _GeomFactory = new FgfGeometryFactory();
            _walker = new SchemaWalker(conn);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~FdoFeatureService()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _GeomFactory.Dispose();
            }
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The underlying FDO connection
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Loads and applies a defined feature schema definition file into the
        /// current connection
        /// </summary>
        /// <param name="xmlFile"></param>
        public void LoadSchemasFromXml(string xmlFile)
        {
            LoadSchemasFromXml(xmlFile, true);
        }

        /// <summary>
        /// Loads and applies a defined feature schema definition file into the
        /// current connection
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="fix">If true, will fix any incompatibilities before applying</param>
        public void LoadSchemasFromXml(string xmlFile, bool fix)
        {
            FeatureSchemaCollection schemas = new FeatureSchemaCollection(null);
            schemas.ReadXml(xmlFile);
            foreach (FeatureSchema fs in schemas)
            {
                IncompatibleSchema incSchema;
                if (fix && !CanApplySchema(fs, out incSchema))
                {
                    var schema = AlterSchema(fs, incSchema);
                    ApplySchema(schema);
                }
                else
                {
                    ApplySchema(fs);
                }
            }
        }

        /// <summary>
        /// Checks if a given FDO command is supported
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool SupportsCommand(OSGeo.FDO.Commands.CommandType cmd)
        {
            return Array.IndexOf<int>(Connection.CommandCapabilities.Commands, (int)cmd) >= 0;
        }

        /// <summary>
        /// Gets the names of all schemas in this connection
        /// </summary>
        /// <returns></returns>
        public List<string> GetSchemaNames() => _walker.GetSchemaNames();

        /// <summary>
        /// Gets the names of all classes for a given schema
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public List<string> GetClassNames(string schemaName) => _walker.GetClassNames(schemaName);

        /// <summary>
        /// Gets the number of features in a given class definition.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="bruteForce">Uses a brute force counting approach if no other approaches are available</param>
        /// <returns></returns>
        public long GetFeatureCount(string className, string filter, bool bruteForce)
            => Connection.GetFeatureCount(className, filter, bruteForce);

        /// <summary>
        /// Gets the number of features in a given class definition. If the class definition is a raster
        /// feature class, it will always return 0. This does not do counting by brute force if other
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="filter"></param>
        /// <param name="bruteForce">Use the brute force approach if no other approaches are available</param>
        /// <returns></returns>
        public long GetFeatureCount(ClassDefinition classDef, string filter, bool bruteForce)
            => Connection.GetFeatureCount(classDef, filter, bruteForce);

        /// <summary>
        /// Applies a feature schema to the current connection
        /// </summary>
        /// <param name="fs">The schema to apply</param>
        public void ApplySchema(FeatureSchema fs)
        {
            ApplySchema(fs, null, false);
        }
        
        /// <summary>
        /// Applies a feature schema (with optional schema mapping) to the current connection
        /// </summary>
        /// <param name="fs">The schema to apply</param>
        /// <param name="mapping"></param>
        public void ApplySchema(FeatureSchema fs, PhysicalSchemaMapping mapping)
        {
            ApplySchema(fs, mapping, false);
        }

        /// <summary>
        /// Applies a feature schema (with optional schema mapping) to the current connection
        /// </summary>
        /// <param name="fs">The schema to apply</param>
        /// <param name="mapping"></param>
        /// <param name="ignoreStates">If true, will disregard element states in the schema to apply</param>
        public void ApplySchema(FeatureSchema fs, PhysicalSchemaMapping mapping, bool ignoreStates)
        {
            // Fix any invalid spatial context assocations
            IList<SpatialContextInfo> contexts = GetSpatialContexts();
            foreach (ClassDefinition cls in fs.Classes)
            {
                foreach (PropertyDefinition pd in cls.Properties)
                {
                    if (pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    {
                        GeometricPropertyDefinition g = pd as GeometricPropertyDefinition;
                        if (contexts.Count > 0)
                        {
                            bool found = false;
                            foreach (SpatialContextInfo sci in contexts)
                            {
                                if (sci.Name == g.SpatialContextAssociation)
                                {
                                    found = true;
                                }
                            }

                            // Set association to first one on the list if not found
                            if (!found)
                            {
                                g.SpatialContextAssociation = contexts[0].Name;
                            }
                        }
                        else //No spatial contexts found. So empty any assocations
                        {
                            g.SpatialContextAssociation = string.Empty;
                        }
                    }
                }
            }

            using (IApplySchema apply = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
            {
                apply.FeatureSchema = fs;
                apply.IgnoreStates = ignoreStates;
                if (mapping != null)
                    apply.PhysicalMapping = mapping;
                apply.Execute();
            }
        }

        /// <summary>
        /// Dumps a given feature schema (by name) to an xml file
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="xmlFile"></param>
        public void WriteSchemaToXml(string schemaName, string xmlFile)
        {
            FeatureSchema schema = GetSchemaByName(schemaName);
            if (schema != null)
            {
                using (var ios = new IoFileStream(xmlFile, "w"))
                {
                    using (var writer = new XmlWriter(ios, false, XmlWriter.LineFormat.LineFormat_Indent))
                    {
                        schema.WriteXml(writer);
                        writer.Close();
                    }
                    ios.Close();
                }
            }
            else
            {
                throw new FeatureServiceException(Res.GetStringFormatted("ERR_SCHEMA_NOT_FOUND", schemaName));
            }
        }

        /// <summary>
        /// Clones all the feature schemas in the current connection (via in-memory
        /// XML serialization)
        /// </summary>
        /// <returns></returns>
        public FeatureSchemaCollection CloneSchemas()
        {
            FeatureSchemaCollection schemas = DescribeSchema();
            FeatureSchemaCollection newSchemas = new FeatureSchemaCollection(null);
            using (IoMemoryStream stream = new IoMemoryStream())
            {
                schemas.WriteXml(stream);
                stream.Reset();
                newSchemas.ReadXml(stream);
            }
            return newSchemas;
        }

        /// <summary>
        /// Gets a feature schema by name
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <returns>The feature schema. null if the schema was not found.</returns>
        public FeatureSchema GetSchemaByName(string schemaName) => _walker.GetSchemaByName(schemaName);

        /// <summary>
        /// Enumerates all the known feature schemas in the current connection.
        /// </summary>
        /// <returns></returns>
        public FeatureSchemaCollection DescribeSchema() => _walker.DescribeSchema();

        /// <summary>
        /// Gets the first matching class definition.
        /// </summary>
        /// <param name="className">The name of the class</param>
        /// <returns></returns>
        public ClassDefinition GetClassByName(string className) => _walker.GetClassByName(className);

        /// <summary>
        /// Gets a class definition by name
        /// </summary>
        /// <param name="schemaName">The parent schema name</param>
        /// <param name="className">The class name</param>
        /// <returns>The class definition if found. null if it wasn't</returns>
        public ClassDefinition GetClassByName(string schemaName, string className) => _walker.GetClassByName(schemaName, className);

        /// <summary>
        /// Dumps all feature schemas in the current connection to an xml file.
        /// </summary>
        /// <param name="schemaFile"></param>
        public void WriteSchemaToXml(string schemaFile)
        {
            FeatureSchemaCollection schemas = DescribeSchema();
            using (var ios = new IoFileStream(schemaFile, "w"))
            {
                using (var writer = new XmlWriter(ios, false, XmlWriter.LineFormat.LineFormat_Indent))
                {
                    schemas.WriteXml(writer);
                    writer.Close();
                }
                ios.Close();
            }
        }

        /// <summary>
        /// Enumerates all spatial contexts in the current connection
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<SpatialContextInfo> GetSpatialContexts()
            => Connection.GetSpatialContexts();

        /// <summary>
        /// Gets a spatial context by name
        /// </summary>
        /// <param name="name">The name of the spatial context</param>
        /// <returns>The spatial context information if found. null if otherwise</returns>
        public SpatialContextInfo GetSpatialContext(string name) => Connection.GetSpatialContext(name);

        /// <summary>
        /// Creates a spatial context
        /// </summary>
        /// <param name="ctx">The spatial context</param>
        /// <param name="updateExisting">If true, will replace any existing spatial context of the same name</param>
        public void CreateSpatialContext(SpatialContextInfo ctx, bool updateExisting)
        {
            using (ICreateSpatialContext create = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) as ICreateSpatialContext)
            {
                IGeometry geom = null;
                create.CoordinateSystem = ctx.CoordinateSystem;
                create.CoordinateSystemWkt = ctx.CoordinateSystemWkt;
                create.Description = ctx.Description;
                create.ExtentType = ctx.ExtentType;
                if (create.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static)
                {
                    if (string.IsNullOrEmpty(ctx.ExtentGeometryText))
                        throw new FeatureServiceException("Creating a spatial context with static extents requires an extent geometry to be specified");

                    geom = _GeomFactory.CreateGeometry(ctx.ExtentGeometryText);
                    create.Extent = _GeomFactory.GetFgf(geom);
                }
                create.Name = ctx.Name;
                create.UpdateExisting = updateExisting;
                create.XYTolerance = ctx.XYTolerance;
                create.ZTolerance = ctx.ZTolerance;
                create.Execute();
                if(geom != null)
                    geom.Dispose();
            }
        }

        /// <summary>
        /// Destroys a spatial context
        /// </summary>
        /// <param name="ctx">The spatial context to destroy</param>
        public void DestroySpatialContext(SpatialContextInfo ctx)
        {
            DestroySpatialContext(ctx.Name);
        }

        /// <summary>
        /// Destroys a spatial context
        /// </summary>
        /// <param name="name">The name of the spatial context to destroy</param>
        public void DestroySpatialContext(string name)
        {
            using (IDestroySpatialContext destroy = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) as IDestroySpatialContext)
            {
                destroy.Name = name;
                destroy.Execute();
            }
        }

        /// <summary>
        /// Destroys a feature schema
        /// </summary>
        /// <param name="schemaName">The name of the schema to destroy</param>
        public void DestroySchema(string schemaName)
        {
            using (IDestroySchema destroy = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySchema) as IDestroySchema)
            {
                destroy.SchemaName = schemaName;
                destroy.Execute();
            }
        }

        /// <summary>
        /// Destroys a data store
        /// </summary>
        /// <param name="dataStoreString">A connection-string style string describing the data store parameters</param>
        public void DestroyDataStore(string dataStoreString)
        {
            NameValueCollection parameters = ExpressUtility.ConvertFromString(dataStoreString);
            using (IDestroyDataStore destroy = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroyDataStore) as IDestroyDataStore)
            {
                foreach (string key in parameters.AllKeys)
                {
                    destroy.DataStoreProperties.SetProperty(key, parameters[key]);
                }
                destroy.Execute();
            }
        }

        /// <summary>
        /// Creates a data store
        /// </summary>
        /// <param name="dataStoreString">A connection-string style string describing the data store parameters</param>
        public void CreateDataStore(string dataStoreString)
        {
            NameValueCollection parameters = ExpressUtility.ConvertFromString(dataStoreString);
            using (ICreateDataStore create = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
            {
                foreach (string key in parameters.AllKeys)
                {
                    create.DataStoreProperties.SetProperty(key, parameters[key]);
                }
                create.Execute();
            }
        }

        /// <summary>
        /// Enumerates all the datastores in the current connection
        /// </summary>
        /// <param name="onlyFdoEnabled">If true, only fdo-enabled datastores are returned</param>
        /// <returns>A list of datastores</returns>
        public ReadOnlyCollection<DataStoreInfo> ListDataStores(bool onlyFdoEnabled)
        {
            List<DataStoreInfo> stores = new List<DataStoreInfo>();
            using (IListDataStores dlist = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ListDataStores) as IListDataStores)
            {
                if (!onlyFdoEnabled)
                    dlist.IncludeNonFdoEnabledDatastores = true;

                using (IDataStoreReader reader = dlist.Execute())
                {
                    while (reader.ReadNext())
                    {
                        DataStoreInfo dinfo = new DataStoreInfo(reader);
                        stores.Add(dinfo);
                    }
                }
            }
            return new ReadOnlyCollection<DataStoreInfo>(stores);
        }

        /// <summary>
        /// Computes the minimum envelope (bounding box) for the given list
        /// of feature classes
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        public IEnvelope ComputeEnvelope(IEnumerable<ClassDefinition> classes)
        {
            double? maxx = null;
            double? maxy = null;
            double? minx = null;
            double? miny = null;

            IEnvelope computedEnvelope = null;

            //Use brute-force instead of SpatialExtents() as there
            //is no guarantee that every provider will implement that
            //expression function
            foreach (ClassDefinition classDef in classes)
            {
                if (classDef.ClassType == ClassType.ClassType_FeatureClass)
                {
                    using (ISelect select = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select) as ISelect)
                    {
                        string propertyName = ((FeatureClass)classDef).GeometryProperty.Name;
                        select.SetFeatureClassName(classDef.Name);
                        select.PropertyNames.Clear();
                        select.PropertyNames.Add((Identifier)Identifier.Parse(propertyName));
                        using (IFeatureReader reader = select.Execute())
                        {
                            while (reader.ReadNext())
                            {
                                if (!reader.IsNull(propertyName))
                                {
                                    byte[] bGeom = reader.GetGeometry(propertyName);
                                    IGeometry geom = _GeomFactory.CreateGeometryFromFgf(bGeom);
                                    IEnvelope env = geom.Envelope;
                                    if (!maxx.HasValue || env.MaxX > maxx)
                                        maxx = env.MaxX;
                                    if (!maxy.HasValue || env.MaxY > maxy)
                                        maxy = env.MaxY;
                                    if (!minx.HasValue || env.MinX < minx)
                                        minx = env.MinX;
                                    if (!miny.HasValue || env.MinY < miny)
                                        miny = env.MinY;
                                    env.Dispose();
                                    geom.Dispose();
                                }
                            }
                        }
                    }
                }
            }

            if ((maxx.HasValue) && (maxy.HasValue) && (minx.HasValue) && (miny.HasValue))
            {
                computedEnvelope = _GeomFactory.CreateEnvelopeXY(minx.Value, miny.Value, maxx.Value, maxy.Value);
            }

            return computedEnvelope;
        }

        /// <summary>
        /// Returns true if this connection supports batch insertion. Returns false if otherwise.
        /// </summary>
        /// <returns></returns>
        public bool SupportsBatchInsertion()
        {
            //HACK: This bombs on PostGIS, must be something to do with the refcounting.
            if (Connection.ConnectionInfo.ProviderName.Contains("PostGIS"))
                return false;

            bool supported = false;
            using (IInsert insert = Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert)
            {
                supported = (insert.BatchParameterValues != null);
            }
            return supported;
        }

        /// <summary>
        /// Creates a FDO command 
        /// </summary>
        /// <typeparam name="T">The FDO command reference to create. This must match the command type specified by the <paramref name="commandType"/> parameter</typeparam>
        /// <param name="commandType">The type of FDO commadn to create</param>
        /// <returns></returns>
        public T CreateCommand<T>(OSGeo.FDO.Commands.CommandType commandType) where T : class, OSGeo.FDO.Commands.ICommand
        {
            return Connection.CreateCommand(commandType) as T;
        }

        /// <summary>
        /// Alters the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="incompatibleSchema">The incompatible schema.</param>
        /// <returns></returns>
        public FeatureSchema AlterSchema(FeatureSchema schema, IncompatibleSchema incompatibleSchema)
            => _schemaChecker.AlterSchema(schema, incompatibleSchema, () => GetActiveSpatialContext());

        /// <summary>
        /// Alters the given class definition to be compatible with the current connection
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="incClass"></param>
        /// <returns></returns>
        public ClassDefinition AlterClassDefinition(ClassDefinition classDef,
                                                    IncompatibleClass incClass,
                                                    Action<GeometricPropertyDefinition, SpatialContextInfo> fixGeomSc)
            => _schemaChecker.AlterClassDefinition(classDef, incClass, () => GetActiveSpatialContext(), fixGeomSc);

        /// <summary>
        /// Returns true if the given schema can be applied to this connection
        /// Returns false if otherwise.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="incSchema"></param>
        /// <returns></returns>
        public bool CanApplySchema(FeatureSchema schema, out IncompatibleSchema incSchema)
            => _schemaChecker.CanApplySchema(schema, out incSchema);

        public bool CanApplyClass(ClassDefinition classDef, out IncompatibleClass cls)
            => _schemaChecker.CanApplyClass(classDef, out cls);

        /// <summary>
        /// Selects features from this connection according to the criteria set in the FeatureQueryOptions argument
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public FdoFeatureReader SelectFeatures(FeatureQueryOptions options)
        {
            return SelectFeatures(options, -1);
        }

        /// <summary>
        /// Selects features from this connection according to the criteria set in the FeatureQueryOptions argument
        /// </summary>
        /// <param name="options"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public FdoFeatureReader SelectFeatures(FeatureQueryOptions options, int limit)
        {
            return SelectFeatures(options, limit, false);
        }

        /// <summary>
        /// Selects features from this connection according to the criteria set in the FeatureQueryOptions argument
        /// </summary>
        /// <param name="options"></param>
        /// <param name="limit"></param>
        /// <param name="useExtendedSelectIfPossible"></param>
        /// <returns></returns>
        public FdoFeatureReader SelectFeatures(FeatureQueryOptions options, int limit, bool useExtendedSelectIfPossible)
        {
            IFeatureReader reader = null;
            var joins = options.JoinCriteria;
            if (joins.Count > 0)
            {
                IExtendedSelect extSelect = CreateCommand<IExtendedSelect>(CommandType.CommandType_ExtendedSelect);
                using (extSelect)
                {
                    SetSelectOptions(options, extSelect);
                    if (options.OrderBy.Count == 1)
                        extSelect.SetOrderingOption(options.OrderBy[0], options.OrderOption);

                    if (!string.IsNullOrEmpty(options.ClassAlias))
                        extSelect.Alias = options.ClassAlias;
                    var joinCriteria = extSelect.JoinCriteria;
                    foreach (var join in joins)
                    {
                        joinCriteria.Add(join.AsJoinCriteria());
                    }
                    reader = extSelect.Execute();
                }
            }
            else
            {
                try
                {
                    if (useExtendedSelectIfPossible)
                    {
                        IExtendedSelect select = CreateCommand<IExtendedSelect>(CommandType.CommandType_ExtendedSelect);
                        using (select)
                        {
                            SetSelectOptions(options, select);
                            if (options.OrderBy.Count == 1)
                            {
                                var ordering = select.Ordering;
                                var ident = new Identifier(options.OrderBy[0]);
                                ordering.Add(ident);
                                select.SetOrderingOption(options.OrderBy[0], options.OrderOption);
                            }

                            reader = select.ExecuteScrollable();
                        }
                    }
                }
                catch (OSGeo.FDO.Common.Exception)
                {
                    
                }
                
                if (reader == null)
                {
                    ISelect select = CreateCommand<ISelect>(CommandType.CommandType_Select);
                    using (select)
                    {
                        SetSelectOptions(options, select);
                        reader = select.Execute();
                    }
                }
            }
            if(limit > 0)
                return new FdoFeatureReader(reader, limit);
            else
                return new FdoFeatureReader(reader);
        }

        /// <summary>
        /// Selects groups of features from this connection and applies filters to each of the groups 
        /// according to the criteria set in the FeatureAggregateOptions argument
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public FdoDataReader SelectAggregates(FeatureAggregateOptions options)
        {
            if (!SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_SELECT_AGGREGATE"));

            IDataReader reader = null;
            ISelectAggregates select = CreateCommand<ISelectAggregates>(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates);
            using (select)
            {
                SetSelectOptions(options, select);
                //HACK: Only set distinct if true, so we don't upset the GDAL provider
                if (options.Distinct)
                    select.Distinct = true;
                if(!string.IsNullOrEmpty(options.GroupFilter))
                    select.GroupingFilter = Filter.Parse(options.GroupFilter);
                foreach (string propName in options.GroupByProperties)
                {
                    select.Grouping.Add((Identifier)Identifier.Parse(propName));
                }
                if (options.JoinCriteria.Count > 0)
                {
                    select.Alias = options.ClassAlias;
                    var joinCriteria = select.JoinCriteria;
                    foreach (var criteria in options.JoinCriteria)
                    {
                        var fjc = criteria.AsJoinCriteria();
                        joinCriteria.Add(fjc);
                    }
                }
                reader = select.Execute();
            }
            return new FdoDataReader(reader);
        }

        private static void SetSelectOptions(FeatureQueryOptions options, IBaseSelect select)
        {
            select.SetFeatureClassName(options.ClassName);

            if (options.IsFilterSet)
                select.Filter = Filter.Parse(options.Filter);

            if (options.PropertyList.Count > 0)
            {
                select.PropertyNames.Clear();
                foreach (string propName in options.PropertyList)
                {
                    var id = new Identifier(propName);
                    select.PropertyNames.Add(id);
                }
            }

            if (options.ComputedProperties.Count > 0)
            {
                foreach (string alias in options.ComputedProperties.Keys)
                {
                    select.PropertyNames.Add(new ComputedIdentifier(alias, options.ComputedProperties[alias]));
                }
            }

            if (options.OrderBy.Count > 0 && !(select is IExtendedSelect))
            {
                foreach (string propertyName in options.OrderBy)
                {
                    select.Ordering.Add(new Identifier(propertyName));
                }
                select.OrderingOption = options.OrderOption;
            }
        }
        
        /// <summary>
        /// Executes a SQL statement on this connection.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <returns>A <see cref="FdoSqlReader"/> containing the results of the query. If this is not a SQL SELECT query, it will return a reader containing the number of results affected</returns>
        public FdoSqlReader ExecuteSQLQuery(string sql)
        {
            if (!SupportsCommand(CommandType.CommandType_SQLCommand))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_SQL"));

            ISQLDataReader reader = null;
            ISQLCommand cmd = CreateCommand<ISQLCommand>(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand);
            bool isSelect = sql.ToUpper().Trim().StartsWith("SELECT");
            FdoSqlReader sqlReader = null;
            using (cmd)
            {
                cmd.SQLStatement = sql;
                if (isSelect)
                {
                    reader = cmd.ExecuteReader();
                    sqlReader = new FdoSqlReader(reader);
                }
                else
                {
                    int affected = cmd.ExecuteNonQuery();
                    sqlReader = new FdoSqlNonQueryReader(affected);
                }
            }
            return sqlReader;
        }

        /// <summary>
        /// Executes SQL statements NOT including SELECT statements. 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSQLNonQuery(string sql)
        {
            if (!SupportsCommand(CommandType.CommandType_SQLCommand))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_SQL"));

            int result = default(int);
            ISQLCommand cmd = CreateCommand<ISQLCommand>(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand);
            using (cmd)
            {
                cmd.SQLStatement = sql;
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

        /// <summary>
        /// Inserts a new feature into the given feature class
        /// </summary>
        /// <param name="className"></param>
        /// <param name="propertyValues"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public int InsertFeature(string className, PropertyValueCollection propertyValues, bool useTransaction)
        {
            if (!SupportsCommand(CommandType.CommandType_Insert))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_INSERT"));

            bool useTrans = (useTransaction && this.Connection.ConnectionCapabilities.SupportsTransactions());
            int inserted = 0;

            IInsert insert = CreateCommand<IInsert>(CommandType.CommandType_Insert);
            using (insert)
            {
                insert.SetFeatureClassName(className);
                foreach (PropertyValue pv in propertyValues)
                {
                    insert.PropertyValues.Add(pv);
                }
                if (useTrans)
                {
                    ITransaction trans = this.Connection.BeginTransaction();
                    using (trans)
                    {
                        try
                        {
                            using (IFeatureReader reader = insert.Execute())
                            {
                                while (reader.ReadNext()) { inserted++; }
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (IFeatureReader reader = insert.Execute())
                        {
                            while (reader.ReadNext()) { inserted++; }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                    }
                }
            }
            return inserted;
        }
        
        /// <summary>
        /// Deletes features from a feature class. The delete operation will run as-is, without a transaction.
        /// </summary>
        /// <param name="className">The feature class</param>
        /// <param name="filter">The filter that determines which features will be deleted</param>
        /// <returns></returns>
        public int DeleteFeatures(string className, string filter)
        {
            return DeleteFeatures(className, filter, false);
        }

        /// <summary>
        /// Deletes features from a feature class
        /// </summary>
        /// <param name="className">The feature class</param>
        /// <param name="filter">The filter that determines which features will be deleted</param>
        /// <param name="useTransaction">If true, the delete operation will run in a transaction. Will not use transactions if false or not supported</param>
        /// <returns></returns>
        public int DeleteFeatures(string className, string filter, bool useTransaction)
        {
            if (!SupportsCommand(CommandType.CommandType_Delete))
                throw new FeatureServiceException(Res.GetStringFormatted("ERR_UNSUPPORTED_CMD", CommandType.CommandType_Delete));

            bool useTrans = (useTransaction && this.Connection.ConnectionCapabilities.SupportsTransactions());

            int deleted = 0;

            using (IDelete del = CreateCommand<IDelete>(CommandType.CommandType_Delete))
            {
                del.SetFeatureClassName(className);
                if(!string.IsNullOrEmpty(filter))
                    del.SetFilter(filter);

                if (useTrans)
                {
                    using (ITransaction trans = this.Connection.BeginTransaction())
                    {
                        deleted = del.Execute();
                        trans.Commit();
                    }
                }
                else
                {
                    deleted = del.Execute();
                }
            }

            return deleted;
        }

        /// <summary>
        /// Updates the features.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="values">The values.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="useTransaction">if set to <c>true</c> [use transaction].</param>
        /// <returns></returns>
        public int UpdateFeatures(string className, Dictionary<string, ValueExpression> values, string filter, bool useTransaction)
        {
            if (values == null || values.Count == 0)
                return 0;

            if (!SupportsCommand(CommandType.CommandType_Update))
                throw new FeatureServiceException(Res.GetStringFormatted("ERR_UNSUPPORTED_CMD", CommandType.CommandType_Update));

            int updated = 0;
            bool useTrans = (useTransaction && this.Connection.ConnectionCapabilities.SupportsTransactions());
            using (IUpdate update = CreateCommand<IUpdate>(CommandType.CommandType_Update))
            {
                update.SetFeatureClassName(className);
                if (!string.IsNullOrEmpty(filter))
                    update.SetFilter(filter);
                foreach (string propName in values.Keys)
                {
                    update.PropertyValues.Add(new PropertyValue(propName, values[propName]));
                }
                if (useTrans)
                {
                    ITransaction trans = this.Connection.BeginTransaction();
                    using (trans)
                    {
                        try
                        {
                            updated = update.Execute();
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new FeatureServiceException(Res.GetString("ERR_UPDATE"), ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        updated = update.Execute();
                    }
                    catch (Exception ex)
                    {
                        throw new FeatureServiceException(Res.GetString("ERR_UPDATE"), ex);
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// Inserts a new feature into the given feature class
        /// </summary>
        /// <param name="className"></param>
        /// <param name="values"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public int InsertFeature(string className, Dictionary<string, ValueExpression> values, bool useTransaction)
        {
            if (!SupportsCommand(CommandType.CommandType_Insert))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_INSERT"));

            bool useTrans = (useTransaction && this.Connection.ConnectionCapabilities.SupportsTransactions());
            int inserted = 0;

            IInsert insert = CreateCommand<IInsert>(CommandType.CommandType_Insert);
            using(insert)
            {
                insert.SetFeatureClassName(className);
                foreach(string propName in values.Keys)
                {
                    insert.PropertyValues.Add(new PropertyValue(propName, values[propName]));
                }
                if (useTrans)
                {
                    ITransaction trans = this.Connection.BeginTransaction();
                    using (trans)
                    {
                        try
                        {
                            using (IFeatureReader reader = insert.Execute())
                            {
                                while (reader.ReadNext()) { inserted++; }
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (IFeatureReader reader = insert.Execute())
                        {
                            while (reader.ReadNext()) { inserted++; }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Creates a data store
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="dataStoreProperties"></param>
        /// <param name="connProperties"></param>
        public static void CreateDataStore(string providerName, NameValueCollection dataStoreProperties, NameValueCollection connProperties)
        {
            if (connProperties == null)
                throw new ArgumentNullException("connProperties");

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(providerName);
            using (conn)
            {
                FdoException fex = null;
                try
                {
                    if (connProperties != null && connProperties.Count > 0)
                    {
                        foreach (string key in connProperties.Keys)
                        {
                            if (!string.IsNullOrEmpty(connProperties[key]))
                                conn.ConnectionInfo.ConnectionProperties.SetProperty(key, connProperties[key]);
                        }
                        conn.Open();
                    }

                    using (ICreateDataStore cmd = conn.CreateCommand(CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                    {
                        foreach (string key in dataStoreProperties.Keys)
                        {
                            if (!string.IsNullOrEmpty(dataStoreProperties[key]))
                                cmd.DataStoreProperties.SetProperty(key, dataStoreProperties[key]);
                        }
                        cmd.Execute();
                    }
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    fex = new FdoException(ex);
                }
                finally
                {
                    if (conn.ConnectionState == ConnectionState.ConnectionState_Open)
                        conn.Close();
                }

                if (fex != null)
                    throw fex;
            }
        }

        /// <summary>
        /// Returns true if this connection supports partial schema discovery.
        /// ie. It supports IGetClassNames and IGetSchemaNames and enhanced IDescribeSchema
        /// </summary>
        /// <returns></returns>
        public bool SupportsPartialSchemaDiscovery()
        {
            int[] cmds = this.Connection.CommandCapabilities.Commands;
            bool supportedCmds = (Array.IndexOf<int>(cmds, (int)CommandType.CommandType_GetClassNames) >= 0
                               && Array.IndexOf<int>(cmds, (int)CommandType.CommandType_GetSchemaNames) >= 0);
            using (IDescribeSchema describe = Connection.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                bool supportsHint = (describe.ClassNames != null);
                return supportedCmds && supportsHint;
            }
        }

        /// <summary>
        /// Removes an FDO provider from the provider registry
        /// </summary>
        /// <param name="prov"></param>
        public static void UnregisterProvider(string prov)
        {
            FeatureAccessManager.GetProviderRegistry().UnregisterProvider(prov);
            _providerInfo = null; //Invalidate
        }

        /// <summary>
        /// Adds a new FDO provider to the provider registry
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="description"></param>
        /// <param name="version"></param>
        /// <param name="fdoVersion"></param>
        /// <param name="libraryPath"></param>
        /// <param name="managed"></param>
        public static void RegisterProvider(string name, string displayName, string description, string version, string fdoVersion, string libraryPath, bool managed)
        {
            FeatureAccessManager.GetProviderRegistry().RegisterProvider(name, displayName, description, version, fdoVersion, libraryPath, managed);
            _providerInfo = null; //Invalidate
        }

        /// <summary>
        /// Applies the given feature schemas to the current connection
        /// </summary>
        /// <param name="schemas"></param>
        public void ApplySchemas(FeatureSchemaCollection schemas)
        {
            ApplySchemas(schemas, true);
        }

        /// <summary>
        /// Applies the given feature schemas to the current connection
        /// </summary>
        /// <param name="schemas"></param>
        /// <param name="fix"></param>
        public void ApplySchemas(FeatureSchemaCollection schemas, bool fix)
        {
            foreach (FeatureSchema s in schemas)
            {
                IncompatibleSchema incSchema;
                if (fix && !CanApplySchema(s, out incSchema))
                {
                    var schema = AlterSchema(s, incSchema);
                    this.ApplySchema(schema);
                }
                else
                {
                    this.ApplySchema(s);
                }
            }
        }

        /// <summary>
        /// Gets the active spatial context in this connection
        /// </summary>
        /// <returns></returns>
        public SpatialContextInfo GetActiveSpatialContext() => Connection.GetActiveSpatialContext();

        /// <summary>
        /// Returns a feature schema containing the given sub-set of classes. This also returns
        /// a list of classes that did not exist.
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="classes"></param>
        /// <param name="classesNotFound">An array of class name that were not found</param>
        /// <returns></returns>
        public FeatureSchema PartialDescribeSchema(string schemaName, List<string> classes, out string[] classesNotFound)
        {
            classesNotFound = new string[0];
            var notFound = new HashSet<string>();
            if (SupportsPartialSchemaDiscovery())
            {
                List<string> classesToQuery = new List<string>();

                //Compile a list of legit classes to query against the master list provided by IGetClassNames
                using (IGetClassNames getclasses = Connection.CreateCommand(CommandType.CommandType_GetClassNames) as IGetClassNames)
                {
                    getclasses.SchemaName = schemaName;
                    using (var names = getclasses.Execute())
                    {
                        foreach (OSGeo.FDO.Common.StringElement el in names)
                        {
                            if (classes.Contains(el.String))
                                classesToQuery.Add(el.String);
                        }
                    }
                }

                //Now weed out specified classes that don't exist
                foreach (var name in classes)
                {
                    if (!classesToQuery.Contains(name))
                        notFound.Add(name);
                }

                classesNotFound = notFound.ToArray();

                //Use new API
                //
                //Bug?: SQL Server appears to disregard the schema hint on a multi-schema database, our picking and trimming ensures
                //the calling application doesn't see this problem.
                using (IDescribeSchema describe = Connection.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    describe.SchemaName = schemaName;
                    foreach (string cls in classesToQuery)
                    {
                        describe.ClassNames.Add(new OSGeo.FDO.Common.StringElement(cls));
                    }
                    FeatureSchemaCollection schemas = describe.Execute();
                    if (schemas != null)
                    {
                        foreach (FeatureSchema schema in schemas)
                        {
                            if (schema.Name == schemaName)
                            {
                                TrimSchema(classes, notFound, schema);
                                classesNotFound = notFound.ToArray();
                                return schema;
                            }
                        }
                    }
                }
            }
            else
            {
                //Use old approach, full schema and rip out classes not in
                //the list
                FeatureSchema schema = GetSchemaByName(schemaName);
                if (schema != null)
                {
                    TrimSchema(classes, notFound, schema);
                    classesNotFound = notFound.ToArray();
                    return schema;
                }
            }

            throw new SchemaNotFoundException(schemaName);
        }

        private static void TrimSchema(List<string> classes, ICollection<string> notFound, FeatureSchema schema)
        {
            List<string> clsRemove = new List<string>();
            foreach (ClassDefinition cd in schema.Classes)
            {
                if (!classes.Contains(cd.Name))
                    clsRemove.Add(cd.Name);
            }
            foreach (string cls in clsRemove)
            {
                schema.Classes.RemoveAt(schema.Classes.IndexOf(cls));
            }
            //Add classes that don't exist
            foreach (string cls in classes)
            {
                var cidx = schema.Classes.IndexOf(cls);
                if (cidx < 0)
                    notFound.Add(cls);
            }
        }

        /// <summary>
        /// Returns a feature schema containing the given sub-set of classes
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        /// <exception cref="SchemaNotFoundException">Thrown if no schema of the specified name exists</exception>
        public FeatureSchema PartialDescribeSchema(string schemaName, List<string> classes)
        {
            if (SupportsPartialSchemaDiscovery())
            {
                //Use new API
                using (IDescribeSchema describe = Connection.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    describe.SchemaName = schemaName;
                    foreach (string cls in classes)
                    {
                        describe.ClassNames.Add(new OSGeo.FDO.Common.StringElement(cls));
                    }
                    FeatureSchemaCollection schemas = describe.Execute();
                    if (schemas != null && schemas.Count == 1)
                        return schemas[0];
                }
            }
            else
            {
                //Use old approach, full schema and rip out classes not in
                //the list
                FeatureSchema schema = GetSchemaByName(schemaName);
                if (schema != null)
                {
                    List<string> clsRemove = new List<string>();
                    foreach (ClassDefinition cd in schema.Classes)
                    {
                        if (!classes.Contains(cd.Name))
                            clsRemove.Add(cd.Name);
                    }
                    foreach (string cls in clsRemove)
                    {
                        schema.Classes.RemoveAt(schema.Classes.IndexOf(cls));
                    }
                    return schema;
                }
            }

            throw new SchemaNotFoundException(schemaName);
        }

        public PhysicalSchemaMappingCollection DescribeSchemaMapping(bool includeDefaults)
        {
            return DescribeSchemaMapping(null, includeDefaults);
        }

        public PhysicalSchemaMappingCollection DescribeSchemaMapping(string schemaName, bool includeDefaults)
        {
            var cmds = Connection.CommandCapabilities.Commands;
            if (Array.IndexOf<int>(cmds, (int)CommandType.CommandType_DescribeSchemaMapping) >= 0)
            {
                using (var cmd = (IDescribeSchemaMapping)Connection.CreateCommand(CommandType.CommandType_DescribeSchemaMapping))
                {
                    if (!string.IsNullOrEmpty(schemaName))
                        cmd.SchemaName = schemaName;

                    cmd.IncludeDefaults = includeDefaults;

                    return cmd.Execute();
                }
            }
            else
            {
                return null;
            }
        }
    }
}
