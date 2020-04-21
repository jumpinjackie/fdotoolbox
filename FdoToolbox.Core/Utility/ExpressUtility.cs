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
using System.Collections.Specialized;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using System.Resources;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.DataStore;
using System.IO;
using FdoToolbox.Core.ETL.Specialized;
using System.Data;
using System.Collections.ObjectModel;
using FdoToolbox.Core.ETL.Overrides;
using FdoToolbox.Core.ETL;

namespace FdoToolbox.Core.Utility
{
    /// <summary>
    /// Utility class for common FDO tasks and functionality
    /// </summary>
    public sealed class ExpressUtility
    {
        /// <summary>
        /// Converts a specially formatted string to a NameValueCollection
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static NameValueCollection ConvertFromString(string str)
        {
            NameValueCollection param = new NameValueCollection();
            if (!string.IsNullOrEmpty(str))
            {
                string[] parameters = str.Split(';');
                if (parameters.Length > 0)
                {
                    foreach (string p in parameters)
                    {
                        string[] pair = p.Split('=');
                        param.Add(pair[0], pair[1]);
                    }
                }
                else
                {
                    string[] pair = str.Split('=');
                    param.Add(pair[0], pair[1]);
                }
            }
            return param;
        }

        /// <summary>
        /// Converts a <see cref="DataPropertyDefinition"/> into a <see cref="DataColumn"/>
        /// </summary>
        /// <param name="dp">The data property definition</param>
        /// <returns>The converted data column</returns>
        public static DataColumn GetDataColumnForProperty(DataPropertyDefinition dp)
        {
            DataColumn col = new DataColumn(dp.Name, GetClrTypeFromFdoDataType(dp.DataType));
            col.AllowDBNull = dp.Nullable;
            col.AutoIncrement = dp.IsAutoGenerated;
            col.ReadOnly = dp.ReadOnly;
            if (dp.DataType == DataType.DataType_String)
            {
                if (!string.IsNullOrEmpty(dp.DefaultValue))
                    col.DefaultValue = dp.DefaultValue;

                if (dp.Length > 0)
                    col.MaxLength = dp.Length;
            }
            return col;
        }

        const int DEFAULT_STRING_LENGTH = 255;

        /// <summary>
        /// Converts a <see cref="DataColumn"/> object into a <see cref="DataPropertyDefinition"/>
        /// </summary>
        /// <param name="col">The data column</param>
        /// <returns>The converted data property definition</returns>
        public static DataPropertyDefinition GetDataPropertyForColumn(DataColumn col)
        {
            DataPropertyDefinition dp = new DataPropertyDefinition(col.ColumnName, string.Empty);
            dp.DataType = GetFdoDataTypeFromClrType(col.DataType);
            dp.Nullable = col.AllowDBNull;
            dp.IsAutoGenerated = col.AutoIncrement;
            dp.ReadOnly = col.ReadOnly;
            if (dp.DataType == DataType.DataType_String)
            {
                if (col.MaxLength > 0)
                    dp.Length = col.MaxLength;
                else
                    dp.Length = DEFAULT_STRING_LENGTH;
            }
            return dp;
        }

        /// <summary>
        /// Gets the FDO data type for a CLR type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DataType GetFdoDataTypeFromClrType(Type t)
        {
            //No CLOB
            if (t == typeof(byte[]))
                return DataType.DataType_BLOB;
            else if (t == typeof(bool))
                return DataType.DataType_Boolean;
            else if (t == typeof(byte))
                return DataType.DataType_Byte;
            else if (t == typeof(DateTime))
                return DataType.DataType_DateTime;
            else if (t == typeof(decimal))
                return DataType.DataType_Decimal;
            else if (t == typeof(double))
                return DataType.DataType_Double;
            else if (t == typeof(short))
                return DataType.DataType_Int16;
            else if (t == typeof(int))
                return DataType.DataType_Int32;
            else if (t == typeof(long))
                return DataType.DataType_Int64;
            else if (t == typeof(float))
                return DataType.DataType_Single;
            else if (t == typeof(string))
                return DataType.DataType_String;
            else
                throw new ArgumentException(ResourceUtil.GetStringFormatted("ERR_NO_CORRESPONDING_DATA_TYPE", t));
        }

        /// <summary>
        /// Gets the CLR type from a FDO data type
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Type GetClrTypeFromFdoDataType(DataType dt)
        {
            Type t = null;
            switch (dt)
            {
                case DataType.DataType_BLOB:
                    t = typeof(byte[]);
                    break;
                case DataType.DataType_Boolean:
                    t = typeof(bool);
                    break;
                case DataType.DataType_Byte:
                    t = typeof(byte);
                    break;
                case DataType.DataType_CLOB:
                    t = typeof(byte[]);
                    break;
                case DataType.DataType_DateTime:
                    t = typeof(DateTime);
                    break;
                case DataType.DataType_Decimal:
                    t = typeof(double); //FDO coerces decimals to doubles (otherwise why is there not a GetDecimal() method in FdoIReader?)
                    break;
                case DataType.DataType_Double:
                    t = typeof(double);
                    break;
                case DataType.DataType_Int16:
                    t = typeof(short);
                    break;
                case DataType.DataType_Int32:
                    t = typeof(int);
                    break;
                case DataType.DataType_Int64:
                    t = typeof(long);
                    break;
                case DataType.DataType_Single:
                    t = typeof(float);
                    break;
                case DataType.DataType_String:
                    t = typeof(string);
                    break;
            }
            return t;
        }

        /// <summary>
        /// Converts a NameValueCollection to a connection string style string
        /// </summary>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        public static string ConvertFromNameValueCollection(NameValueCollection nameValueCollection)
        {
            string str = string.Empty;
            foreach (string key in nameValueCollection.Keys)
            {
                if(str == string.Empty)
                    str += key + "=" + nameValueCollection[key];
                else
                    str += ";" + key + "=" + nameValueCollection[key];
            }
            return str;
        }

        /// <summary>
        /// Creates a connection to a FDO provider. The FDO provider must be a flat-file provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FdoConnection CreateFlatFileConnection(string provider, string file)
        {
            FdoConnection conn = null;
            if (provider.StartsWith("OSGeo.SDF"))
            {
                conn = new FdoConnection(provider, string.Format("File={0}", file));
            }
            else if (provider.StartsWith("OSGeo.SQLite"))
            {
                conn = new FdoConnection(provider, string.Format("File={0};UseFdoMetadata=TRUE", file));
            }
            else if (provider.StartsWith("OSGeo.SHP"))
            {
                conn = new FdoConnection(provider, string.Format("DefaultFileLocation={0}", file));
            }
            else
            {
                throw new InvalidOperationException(); //ERR_UNSUPPORTED_FLAT_FILE_PROVIDER
            }
            return conn;
        }

        /// <summary>
        /// Creates a FDO data source. The provider must be a flat-file provider
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool CreateFlatFileDataSource(string provider, string path)
        {
            return CreateFlatFileDataSource(provider, path, true);
        }

        /// <summary>
        /// Creates a FDO data source. The provider must be a flat-file provider
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="path">The path.</param>
        /// <param name="deleteIfExists">if set to <c>true</c> deletes the specified file if it exists.</param>
        /// <returns></returns>
        public static bool CreateFlatFileDataSource(string provider, string path, bool deleteIfExists)
        {
            bool result = false;
            bool sdf = provider.StartsWith("OSGeo.SDF");
            bool sqlite = provider.StartsWith("OSGeo.SQLite");

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            if (conn.ConnectionInfo.ProviderDatastoreType != ProviderDatastoreType.ProviderDatastoreType_File)
                return false; //ERR_NOT_FLAT_FILE

            string pName = GetFileParameter(provider);
            if (string.IsNullOrEmpty(pName))
                return false; //ERR_FILE_PARAMETER_UNKNOWN

            if (deleteIfExists && File.Exists(path))
                File.Delete(path);

            using (conn)
            {
                using (ICreateDataStore cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                {
                    try
                    {
                        var dsprops = cmd.DataStoreProperties;
                        dsprops.SetProperty(pName, path);
                        //Ensures that FDO logical schemas being applied to SQLite are mostly intact.
                        //See FDO trac ticket #739 for details.
                        if (sqlite)
                            dsprops.SetProperty("UseFdoMetadata", "TRUE");
                        cmd.Execute();
                        result = true;
                    }
                    catch (OSGeo.FDO.Common.Exception)
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        private static string GetFileParameter(string provider)
        {
            if (provider.StartsWith("OSGeo.SDF"))
                return "File";
            else if (provider.StartsWith("OSGeo.SQLite"))
                return "File";

            return null;
        }

        /// <summary>
        /// Creates a connection to a FDO provider. The FDO provider must be a flat-file provider.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public static FdoConnection CreateFlatFileConnection(string sourceFile)
        {
            string provider = GetProviderFromFileExtension(sourceFile);
            if (provider != null)
                return CreateFlatFileConnection(provider, sourceFile);
            return null;
        }

        /// <summary>
        /// Infers the FDO provider name from the file's extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetProviderFromFileExtension(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".sdf")
                return "OSGeo.SDF";
            else if (ext == ".shp")
                return "OSGeo.SHP";
            else if (ext == ".db" || ext == ".sqlite" || ext == ".slt")
                return "OSGeo.SQLite";
            return null;
        }

        /// <summary>
        /// Creates a FDO data source. The provider must be a flat-file provider
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static bool CreateFlatFileDataSource(string file)
        {
            return CreateFlatFileDataSource(file, true);
        }

        /// <summary>
        /// Creates a FDO data source. The provider must be a flat-file provider
        /// </summary>
        /// <param name="file">The file</param>
        /// <param name="deleteIfExists">if set to <c>true</c> will delete the file if it already exists.</param>
        /// <returns></returns>
        public static bool CreateFlatFileDataSource(string file, bool deleteIfExists)
        {
            string provider = GetProviderFromFileExtension(file);
            if (provider != null)
                return CreateFlatFileDataSource(provider, file, deleteIfExists);
            return false;
        }

        /// <summary>
        /// Creates a FDO bulk copy task. The target file will be created as part of 
        /// this method call. If the target path is a directory, it is assumed that
        /// SHP files are to be created and copied to.
        /// </summary>
        /// <param name="sourceFile">The path to the source file.</param>
        /// <param name="targetPath">
        /// The path to the target file/directory. If it is a directory, it is assumed
        /// that SHP files are to be created and copied to.
        /// </param>
        /// <param name="copySpatialContexts">If true, will also copy spatial contexts</param>
        /// <param name="fixIncompatibleSchema">If true, will try to fix the source schema to make it compatible with the target connection. If false, an exception will be thrown</param>
        /// <param name="flattenGeometries">If true, will strip all Z and M coordinates from all geometries that are copied</param>
        /// <returns></returns>
        public static FdoBulkCopy CreateBulkCopy(string sourceFile, string targetPath, bool copySpatialContexts, bool fixIncompatibleSchema, bool flattenGeometries)
        {
            FdoBulkCopyOptions options = null;
            FdoConnection source = null;
            FdoConnection target = null;

            try
            {
                //Is a directory. Implies a SHP connection
                if (IsShp(targetPath))
                {
                    //SHP doesn't actually support CreateDataStore. We use the following technique:
                    // - Connect to base directory
                    // - Clone source schema and apply to SHP connection.
                    // - A SHP file and related files are created for each feature class.
                    string shpdir = Directory.Exists(targetPath) ? targetPath : Path.GetDirectoryName(targetPath);
                    source = CreateFlatFileConnection(sourceFile);
                    target = new FdoConnection("OSGeo.SHP", "DefaultFileLocation=" + shpdir);

                    source.Open();

                    //Verify source has only classes with single geometry storage and only one geometry
                    using (FdoFeatureService svc = source.CreateFeatureService())
                    {
                        using (FeatureSchemaCollection schemas = svc.DescribeSchema())
                        {
                            foreach (FeatureSchema sch in schemas)
                            {
                                foreach (ClassDefinition cd in sch.Classes)
                                {
                                    int geomProps = 0;
                                    foreach (PropertyDefinition pd in cd.Properties)
                                    {
                                        if (pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
                                        {
                                            GeometricPropertyDefinition gp = pd as GeometricPropertyDefinition;
                                            GeometricType[] types = FdoGeometryUtil.GetGeometricTypes(gp.GeometryTypes);
                                            if (types.Length != 1 || (types.Length == 1 && types[0] == GeometricType.GeometricType_All))
                                                throw new FdoETLException(string.Format("Source file cannot be copied to a SHP file. {0}:{1}.{2} has more than one geometry storage type", sch.Name, cd.Name, pd.Name));
                                            geomProps++;
                                        }
                                    }
                                    if (geomProps > 1)
                                        throw new FdoETLException("Source file cannot be copied to a SHP file. One or more feature classes have more than one geometry property");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!CreateFlatFileDataSource(targetPath))
                        throw new FdoException("Unable to create data source on: " + targetPath);
                    source = CreateFlatFileConnection(sourceFile);
                    target = CreateFlatFileConnection(targetPath);
                }

                //Source and target connections may have been opened before this point
                if (source.State == FdoConnectionState.Closed)
                    source.Open();

                if (target.State == FdoConnectionState.Closed)
                    target.Open();

                string srcName = "SOURCE";
                string dstName = "TARGET";

                Dictionary<string, FdoConnection> connections = new Dictionary<string, FdoConnection>();
                connections.Add(srcName, source);
                connections.Add(dstName, target);

                options = new FdoBulkCopyOptions(connections, true);

                if (copySpatialContexts)
                {
                    CopyAllSpatialContexts(source, target, true);
                }

                using (FdoFeatureService srcService = source.CreateFeatureService())
                using (FdoFeatureService destService = target.CreateFeatureService())
                {
                    FeatureSchemaCollection schemas = srcService.DescribeSchema();
                    //Assume single-schema
                    FeatureSchema fs = schemas[0];
                    //Clone and apply to target
                    FeatureSchema targetSchema = FdoSchemaUtil.CloneSchema(fs);
                    IncompatibleSchema incSchema;
                    string sourceSchemaName = fs.Name;
                    string targetSchemaName = string.Empty;

                    // If flattening geometries, make sure this is reflected in the output schema
                    if (flattenGeometries)
                    {
                        foreach (ClassDefinition cd in targetSchema.Classes)
                        {
                            if (cd.ClassType == ClassType.ClassType_FeatureClass)
                            {
                                FeatureClass fc = (FeatureClass)cd;
                                fc.GeometryProperty.HasElevation = false;
                                fc.GeometryProperty.HasMeasure = false;
                            }
                        }
                    }

                    bool canApply = destService.CanApplySchema(targetSchema, out incSchema);
                    if (canApply)
                    {
                        destService.ApplySchema(targetSchema);
                        targetSchemaName = targetSchema.Name;
                    }
                    else
                    {
                        if (fixIncompatibleSchema)
                        {
                            FeatureSchema fixedSchema = destService.AlterSchema(targetSchema, incSchema);
                            destService.ApplySchema(fixedSchema);
                            targetSchemaName = fixedSchema.Name;
                        }
                        else
                        {
                            throw new Exception(incSchema.ToString());
                        }
                    }

                    //Copy all classes
                    foreach (ClassDefinition cd in fs.Classes)
                    {
                        var copt = new FdoClassCopyOptions(srcName, dstName, sourceSchemaName, cd.Name, targetSchemaName, cd.Name, null);
                        copt.Name = "Copy source to target [" + cd.Name + "]";
                        copt.FlattenGeometries = flattenGeometries;
                        options.AddClassCopyOption(copt);

                        //Flick on batch support if we can
                        if (destService.SupportsBatchInsertion())
                            copt.BatchSize = 300; //Madness? THIS IS SPARTA!
                    }
                }
            }
            catch (Exception)
            {
                if (source != null)
                    source.Dispose();
                if (target != null)
                    target.Dispose();

                throw;
            }
            return new FdoBulkCopy(options);
        }

        /// <summary>
        /// Copies all spatial contexts from one connection to another. If the target connection
        /// only supports one spatial context, then the active spatial context is copied across.
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts, otherwise it will add them. This value is ignored if the target connection does not support multiple spatial contexts</param>
        public static void CopyAllSpatialContexts(FdoConnection source, FdoConnection target, bool overwrite)
        {
            ICopySpatialContext copy = CopySpatialContextOverrideFactory.GetCopySpatialContextOverride(target);
            copy.Execute(source, target, overwrite);
        }

        /// <summary>
        /// Copies all spatial contexts from one connection to another. If the target connection
        /// only supports one spatial context, then the active spatial context is copied across.
        /// </summary>
        /// <param name="spatialContexts">The spatial contexts.</param>
        /// <param name="target">The target.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public static void CopyAllSpatialContexts(ICollection<SpatialContextInfo> spatialContexts, FdoConnection target, bool overwrite)
        {
            ICopySpatialContext copy = CopySpatialContextOverrideFactory.GetCopySpatialContextOverride(target);
            copy.Execute(spatialContexts, target, overwrite);
        }

        /// <summary>
        /// Gets all class names from the specified flat-file data source
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public static string[] GetClassNames(string sourceFile)
        {
            List<string> classnames = new List<string>();
            FdoConnection source = null;
            try
            {
                source = CreateFlatFileConnection(sourceFile);
                source.Open();
                using (FdoFeatureService svc = source.CreateFeatureService())
                {
                    using (FeatureSchemaCollection schemas = svc.DescribeSchema())
                    {
                        foreach (FeatureSchema sch in schemas)
                        {
                            foreach (ClassDefinition cd in sch.Classes)
                            {
                                classnames.Add(cd.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (source != null)
                    source.Dispose();
            }
            return classnames.ToArray();
        }


        /// <summary>
        /// Determines whether the specified target path is a valid SHP provider file path
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        /// <returns>
        /// 	<c>true</c> if the specified target path is SHP; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsShp(string targetPath)
        {
            return Directory.Exists(targetPath) || targetPath.EndsWith(".shp");
        }

        /// <summary>
        /// Determines whether the specified class has a raster property.
        /// </summary>
        /// <param name="cls">The class</param>
        /// <returns>
        /// 	<c>true</c> if the specified class has a raster property; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasRaster(ClassDefinition cls)
        {
            foreach (PropertyDefinition p in cls.Properties)
            {
                if (p.PropertyType == PropertyType.PropertyType_RasterProperty)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the related files of a Shape File
        /// </summary>
        /// <param name="shapeFile">The shape file.</param>
        /// <returns></returns>
        public static string[] GetRelatedFiles(string shapeFile)
        {
            string file = shapeFile.ToUpper();
            if (file.EndsWith(".SHP"))
            {
                return new string[4] 
                {
                    file.Substring(0, file.Length - 3) + "CPG",
                    file.Substring(0, file.Length - 3) + "DBF",
                    file.Substring(0, file.Length - 3) + "SHX",
                    file.Substring(0, file.Length - 3) + "PRJ"
                };
            }
            return new string[0];
        }

        /// <summary>
        /// Utility method to create a feature class dump bulk copy
        /// </summary>
        /// <param name="source"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="provider"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static FdoBulkCopy CreateBulkCopy(FdoConnection source, string schemaName, string className, string provider, string savePath, string transformToTargetCsWkt)
        {
            if (!ExpressUtility.CreateFlatFileDataSource(provider, savePath))
                throw new FdoException("Could not create " + savePath);

            ClassDefinition srcClass = null;
            using (var svc = source.CreateFeatureService())
            {
                srcClass = svc.GetClassByName(schemaName, className);
            }

            //Apply a copy of the source class to target
            ClassDefinition clone = FdoSchemaUtil.CloneClass(srcClass, true);
            FeatureSchema fs = null;

            FdoConnection target = ExpressUtility.CreateFlatFileConnection(provider, savePath);
            using (var svc = target.CreateFeatureService())
            {
                var schemas = svc.DescribeSchema();
                if (schemas != null && schemas.Count == 1)
                    fs = schemas[0];

                if (fs == null)
                    fs = new FeatureSchema("Default", "");

                var classes = fs.Classes;
                classes.Add(clone);

                svc.ApplySchema(fs, null, true);
            }

            //Setup mappings
            var mappings = new NameValueCollection();
            foreach (PropertyDefinition prop in srcClass.Properties)
            {
                if (prop.PropertyType == PropertyType.PropertyType_DataProperty ||
                    prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                {
                    mappings.Add(prop.Name, prop.Name);
                }
            }

            //Compile query
            var query = new FeatureQueryOptions(className);

            var bcp = CreateBulkCopy(source, target, schemaName, query, fs.Name, clone.Name, mappings, transformToTargetCsWkt);

            //The target connection needs to be cleaned up when done
            bcp.Options.MarkOwnerOfConnection("TARGET");

            return bcp;
        }

        /// <summary>
        /// Utility method to create a bulk copy operation from
        /// one class to another
        /// </summary>
        /// <param name="sourceConn"></param>
        /// <param name="targetConn"></param>
        /// <param name="srcSchemaName"></param>
        /// <param name="srcQuery"></param>
        /// <param name="targetSchemaName"></param>
        /// <param name="targetClassName"></param>
        /// <param name="propertyMapping"></param>
        /// <param name="transformToTargetCsWkt"></param>
        /// <returns></returns>
        public static FdoBulkCopy CreateBulkCopy(
            FdoConnection sourceConn, 
            FdoConnection targetConn, 
            string srcSchemaName,
            FeatureQueryOptions srcQuery, 
            string targetSchemaName,
            string targetClassName, 
            NameValueCollection propertyMapping,
            string transformToTargetCsWkt)
        {
            var dict = new Dictionary<string, FdoConnection>();
            dict["SOURCE"] = sourceConn;
            dict["TARGET"] = targetConn;

            var opts = new FdoBulkCopyOptions(dict, false);
            var copt = new FdoClassCopyOptions(
                "SOURCE",
                "TARGET",
                srcSchemaName,
                srcQuery.ClassName,
                targetSchemaName,
                targetClassName,
                null);

            if (!string.IsNullOrEmpty(transformToTargetCsWkt))
            {
                using (var srcService = sourceConn.CreateFeatureService())
                {
                    var cls = srcService.GetClassByName(srcSchemaName, srcQuery.ClassName) as FeatureClass;
                    if (cls != null)
                    {
                        var geomProp = cls.GeometryProperty;
                        if (geomProp != null)
                        {
                            var sci = srcService.GetSpatialContext(geomProp.SpatialContextAssociation);
                            if (sci != null)
                            {
                                copt.OverrideWkts = new Dictionary<string, SCOverrideItem>();
                                copt.OverrideWkts[sci.Name] = new SCOverrideItem
                                {
                                    CsName = sci.CoordinateSystem,
                                    CsWkt = transformToTargetCsWkt,
                                    OverrideScName = sci.CoordinateSystem,
                                    TransformToThis = true
                                };
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(srcQuery.Filter))
                copt.SourceFilter = srcQuery.Filter;

            foreach (string p in propertyMapping.Keys)
            {
                copt.AddPropertyMapping(p, propertyMapping[p]);
            }

            copt.FlattenGeometries = true;
            copt.ForceWkb = true;

            opts.AddClassCopyOption(copt);
            return new FdoBulkCopy(opts, 100);
        }
    }
}
