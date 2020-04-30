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
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Configuration;
using FdoToolbox.Core.Feature;
using System.Xml.Serialization;
using System.IO;
using OSGeo.FDO.Filter;
using System.Xml;
using System.Collections.Specialized;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.ETL
{
    internal class FeatureSchemaCache : IDisposable
    {
        private Dictionary<string, FeatureSchemaCollection> _cache;

        public FeatureSchemaCache()
        {
            _cache = new Dictionary<string, FeatureSchemaCollection>();
        }

        public void Add(string name, FeatureSchemaCollection schemas)
        {
            _cache[name] = schemas;
        }

        public ClassDefinition GetClassByName(string name, string schemaName, string className)
        {
            if (!_cache.ContainsKey(name))
                return null;

            FeatureSchemaCollection item = _cache[name];

            int sidx = item.IndexOf(schemaName);
            if (sidx >= 0)
            {
                var classes = item[sidx].Classes;
                int cidx = classes.IndexOf(className);

                if (cidx >= 0)
                    return classes[cidx];
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var key in _cache.Keys)
            {
                _cache[key].Dispose();
            }
            _cache.Clear();
        }

        internal bool HasConnection(string connName)
        {
            return _cache.ContainsKey(connName);
        }
    }

    internal enum SchemaOrigin
    {
        Source,
        Target
    }

    internal class MultiSchemaQuery
    {
        private List<SchemaQuery> _query;

        public MultiSchemaQuery(string connName, SchemaOrigin origin)
        {
            this.ConnectionName = connName;
            this.Origin = origin;
            _query = new List<SchemaQuery>();
        }

        public SchemaOrigin Origin { get; private set; }

        public string ConnectionName { get; set; }

        public SchemaQuery TryGet(string schemaName)
        {
            foreach (var q in _query)
            {
                if (q.SchemaName == schemaName)
                    return q;
            }
            return null;
        }

        public void Add(SchemaQuery query)
        {
            _query.Add(query);
        }

        public SchemaQuery[] SchemaQueries => _query.ToArray();
    }

    internal class SchemaQuery
    {
        public SchemaQuery(string schemaName)
        {
            this.SchemaName = schemaName;
            _classes = new HashSet<string>();
        }

        private HashSet<string> _classes;

        public string SchemaName { get; set; }

        public void AddClass(string name)
        {
            _classes.Add(name);
        }

        public IEnumerable<string> ClassNames => _classes;
    }

    internal abstract class TargetClassModificationItem
    {
        public string Name { get; private set; }

        protected TargetClassModificationItem(string name) { this.Name = name; }

        private Dictionary<string, PropertyDefinition> _propsToCreate = new Dictionary<string, PropertyDefinition>();

        public void AddProperty(PropertyDefinition propDef)
        {
            _propsToCreate.Add(propDef.Name, propDef);
        }

        public ICollection<PropertyDefinition> PropertiesToCreate => _propsToCreate.Values;
    }

    internal class CreateTargetClassFromSource : TargetClassModificationItem
    {
        public string Schema { get; set; }

        public CreateTargetClassFromSource(string schema, string name) : base(name) { this.Schema = schema; }
    }

    internal class UpdateTargetClass : TargetClassModificationItem
    {
        public UpdateTargetClass(string name) : base(name) { }
    }

    /// <summary>
    /// Task definition loader base class
    /// </summary>
    public abstract class BaseDefinitionLoader
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The connection string.</param>
        /// <param name="configPath">The configuration path</param>
        /// <param name="name">The name that will be assigned to the connection.</param>
        /// <returns></returns>
        protected abstract FdoConnection CreateConnection(string provider, string connStr, string configPath, ref string name);

        /// <summary>
        /// Prepares the specified bulk copy definition (freshly deserialized) before the loading process begins
        /// </summary>
        /// <param name="def">The bulk copy definition.</param>
        /// <returns>A collection of [old name] - [new name] mappings</returns>
        protected abstract NameValueCollection Prepare(FdoBulkCopyTaskDefinition def);

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDefinitionLoader"/> class.
        /// </summary>
        protected BaseDefinitionLoader() { }

        /// <summary>
        /// Loads bulk copy options from xml
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoBulkCopyOptions BulkCopyFromXml(string file, ref string name, bool owner)
        {
            FdoBulkCopyTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
            def = (FdoBulkCopyTaskDefinition)ser.Deserialize(new StreamReader(file));
            
            return BulkCopyFromXml(def, ref name, owner);
        }

        /// <summary>
        /// Loads bulk copy options from deserialized xml
        /// </summary>
        /// <param name="def">The deserialized definition.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoBulkCopyOptions BulkCopyFromXml(FdoBulkCopyTaskDefinition def, ref string name, bool owner)
        {
            var nameMap = Prepare(def);

            // TODO/FIXME/HACK:
            //
            // The introduction of on-the-fly schema modifications before copying has introduced a
            // potential problem which will only occur when multiple copy tasks copy to the same target
            // feature class. 
            //
            // What happens is because a target will be multiple sources copying to it, if we need
            // to create/update the target class definition, the current infrastructure will be confused
            // as to which source feature class to clone from.
            //
            // A possible solution is to return the first matching name. This will cause a "create" operation
            // to be queued. We then alter the pre-modification process so that subsequent "create" operations
            // becomes "update" operations instead.
            //
            // On second thought, the current infrastructure should be smart enought to prevent this, as
            // the UI only allows:
            //
            // 1. Mapping to an existing class
            // 2. Mapping to a class of the same name (create if necessary)
            //
            // It won't be possible to map to a class that doesn't exist, as the requirement for auto-create/update
            // is that the class name will be the same.
            //
            // ie. It won't be possible to create this mapping
            //
            // a. Foo -> Foo (create if necessary)
            // b. Bar -> Foo
            // 
            // Rule 1 prevents mapping b) from happening
            // Rule 2 ensures that Foo will only ever be created once
            //
            // If multiple sources are mapped to the same target, I believe the bcp infrastructure should ensure
            // that target must already exist first.


            name = def.name;
            Dictionary<string, FdoConnection> connections = new Dictionary<string, FdoConnection>();
            Dictionary<string, string> changeConnNames = new Dictionary<string, string>();

            //TODO: Prepare() ensured that all connections have unique names that don't already exist
            //now in the effort to re-use existing connections, see which connection entries
            //already exist and can be renamed back to the old name

            foreach (FdoConnectionEntryElement entry in def.Connections)
            {
                string connName = entry.name;
                FdoConnection conn = CreateConnection(entry.provider, entry.ConnectionString, entry.configPath, ref connName);

                connections[connName] = conn;

                if (connName != entry.name)
                {
                    changeConnNames[entry.name] = connName;
                }
            }
            
            foreach (string oldName in changeConnNames.Keys)
            {
                def.UpdateConnectionReferences(oldName, changeConnNames[oldName]);
            }

            //Compile the list of classes to be queried
            Dictionary<string, MultiSchemaQuery> queries = new Dictionary<string, MultiSchemaQuery>();
            foreach (FdoCopyTaskElement task in def.CopyTasks)
            {
                MultiSchemaQuery src = null;
                MultiSchemaQuery dst = null;

                //Process source
                if (!queries.ContainsKey(task.Source.connection))
                    src = queries[task.Source.connection] = new MultiSchemaQuery(task.Source.connection, SchemaOrigin.Source);
                else
                    src = queries[task.Source.connection];

                var sq = src.TryGet(task.Source.schema);
                if (sq != null)
                {
                    sq.AddClass(task.Source.@class);
                }
                else
                {
                    sq = new SchemaQuery(task.Source.schema);
                    sq.AddClass(task.Source.@class);
                    src.Add(sq);
                }

                //Process target
                if (!queries.ContainsKey(task.Target.connection))
                    dst = queries[task.Target.connection] = new MultiSchemaQuery(task.Target.connection, SchemaOrigin.Target);
                else
                    dst = queries[task.Target.connection];

                var tq = dst.TryGet(task.Target.schema);
                if (tq != null)
                {
                    tq.AddClass(task.Target.@class);
                }
                else
                {
                    tq = new SchemaQuery(task.Target.schema);
                    tq.AddClass(task.Target.@class);
                    dst.Add(tq);
                }
            }

            List<TargetClassModificationItem> modifiers = new List<TargetClassModificationItem>();
            using (var schemaCache = new FeatureSchemaCache())
            {
                //Now populate the schema cache with source schemas
                foreach (string connName in queries.Keys)
                {
                    if (connections.ContainsKey(connName))
                    {
                        var mqry = queries[connName];
                        var conn = connections[connName];

                        FeatureSchemaCollection schemas = new FeatureSchemaCollection(null);

                        using (var svc = conn.CreateFeatureService())
                        {
                            foreach (var sq in mqry.SchemaQueries)
                            {
                                //Source schema queries are expected to fully check out so use original method
                                if (mqry.Origin == SchemaOrigin.Source)
                                {
                                    schemas.Add(svc.PartialDescribeSchema(sq.SchemaName, new List<string>(sq.ClassNames)));
                                }
                            }
                        }

                        if (schemas.Count > 0)
                            schemaCache.Add(connName, schemas);
                    }
                }

                //Now populate the schema cache with target schemas, taking note of any
                //classes that need to be created or updated.
                foreach (string connName in queries.Keys)
                {
                    if (connections.ContainsKey(connName))
                    {
                        var mqry = queries[connName];
                        var conn = connections[connName];

                        FeatureSchemaCollection schemas = new FeatureSchemaCollection(null);

                        using (var svc = conn.CreateFeatureService())
                        {
                            foreach (var sq in mqry.SchemaQueries)
                            {
                                //Source schema queries are expected to fully check out so use original method
                                if (mqry.Origin == SchemaOrigin.Target)
                                {
                                    //Because we may need to create new classes, the old method will break down on non-existent class names
                                    //So use the new method
                                    string[] notFound;
                                    var schema = svc.PartialDescribeSchema(sq.SchemaName, new List<string>(sq.ClassNames), out notFound);
                                    //if (notFound.Length > 0)
                                    //{
                                    //    //If we can't modify schemas we'll stop right here. This is caused by elements containing createIfNotExists = true
                                    //    if (!conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSchemaModification))
                                    //        throw new NotSupportedException("The connection named " + connName + " does not support schema modification. Therefore, copy tasks and property/expression mappings cannot have createIfNotExists = true");

                                    //    //This cumbersome, but we need the parent schema name of the class that we will need to copy
                                    //    string srcSchema = GetSourceSchemaForMapping(def, sq.SchemaName, notFound);

                                    //    foreach (string className in notFound)
                                    //    {
                                    //        modifiers.Add(new CreateTargetClassFromSource(srcSchema, className));
                                    //    }
                                    //}
                                    schemas.Add(schema);
                                }
                            }
                        }

                        if (schemas.Count > 0)
                            schemaCache.Add(connName, schemas);
                    }
                }

                FdoBulkCopyOptions opts = new FdoBulkCopyOptions(connections, owner);
                
                foreach (FdoCopyTaskElement task in def.CopyTasks)
                {
                    TargetClassModificationItem mod;
                    FdoClassCopyOptions copt = FdoClassCopyOptions.FromElement(task, schemaCache, connections[task.Source.connection], connections[task.Target.connection], out mod);
                    opts.AddClassCopyOption(copt);

                    if (mod != null)
                        copt.PreCopyTargetModifier = mod;
                        //opts.AddClassModifier(mod);
                }

                return opts;
            }
        }

        private static string GetSourceSchemaForMapping(FdoBulkCopyTaskDefinition def, string targetSchema, string[] classNames)
        {
            ISet<string> matches = new HashSet<string>();
            List<string> classes = new List<string>(classNames);
            foreach (var task in def.CopyTasks)
            {
                if (classes.Contains(task.Target.@class) && task.Target.schema.Equals(targetSchema))
                {
                    matches.Add(task.Source.schema);
                }
            }
            if (matches.Count > 1)
                throw new TaskValidationException("The specified class names have various parent schema names");

            if (matches.Count == 0)
                throw new TaskValidationException("Could not determine parent schema name for the given class names");

            return new List<string>(matches)[0]; //The price to pay for targeting linq-less fx 2.0
        }

        /// <summary>
        /// Loads join options from xml
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoJoinOptions JoinFromXml(string file, ref string name, bool owner)
        {
            FdoJoinTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoJoinTaskDefinition));
            def = (FdoJoinTaskDefinition)ser.Deserialize(new StreamReader(file));

            return JoinFromXml(def, ref name, owner);
        }

        /// <summary>
        /// Loads join options from xml
        /// </summary>
        /// <param name="def">The deserialized definition</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        private FdoJoinOptions JoinFromXml(FdoJoinTaskDefinition def, ref string name, bool owner)
        {
            FdoJoinOptions opts = new FdoJoinOptions(owner);
            name = def.name;
            if (def.JoinSettings.DesignatedGeometry != null)
            {
                opts.GeometryProperty = def.JoinSettings.DesignatedGeometry.Property;
                opts.Side = def.JoinSettings.DesignatedGeometry.Side;
            }
            foreach (JoinKey key in def.JoinSettings.JoinKeys)
            {
                opts.JoinPairs.Add(key.left, key.right);
            }
            string dummy = string.Empty;
            opts.JoinType = (FdoJoinType)Enum.Parse(typeof(FdoJoinType), def.JoinSettings.JoinType.ToString());
            opts.SetLeft(
                CreateConnection(def.Left.Provider, def.Left.ConnectionString, null, ref dummy),
                def.Left.FeatureSchema,
                def.Left.Class);
            foreach (string p in def.Left.PropertyList)
            {
                opts.AddLeftProperty(p);
            }
            opts.SetRight(
                CreateConnection(def.Right.Provider, def.Right.ConnectionString, null, ref dummy),
                def.Right.FeatureSchema,
                def.Right.Class);
            foreach (string p in def.Right.PropertyList)
            {
                opts.AddRightProperty(p);
            }

            opts.SetTarget(
                CreateConnection(def.Target.Provider, def.Target.ConnectionString, null, ref dummy),
                def.Target.FeatureSchema,
                def.Target.Class);

            opts.LeftPrefix = def.Left.Prefix;
            opts.RightPrefix = def.Right.Prefix;
            opts.LeftFilter = def.Left.Filter;
            opts.RightFilter = def.Right.Filter;

            return opts;
        }

        /// <summary>
        /// Saves the join options to xml
        /// </summary>
        /// <param name="opts">The opts.</param>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        public void ToXml(FdoJoinOptions opts, string name, string file)
        {
            FdoJoinTaskDefinition jdef = new FdoJoinTaskDefinition
            {
                name = name,
                JoinSettings = new FdoJoinSettings()
            };
            if (!string.IsNullOrEmpty(opts.GeometryProperty))
            {
                jdef.JoinSettings.DesignatedGeometry = new FdoDesignatedGeometry
                {
                    Property = opts.GeometryProperty,
                    Side = opts.Side
                };
            }
            List<JoinKey> keys = new List<JoinKey>();
            foreach (string left in opts.JoinPairs.Keys)
            {
                JoinKey key = new JoinKey
                {
                    left = left,
                    right = opts.JoinPairs[left]
                };
                keys.Add(key);
            }
            jdef.JoinSettings.JoinKeys = keys.ToArray();

            jdef.Left = new FdoJoinSource
            {
                Class = opts.Left.ClassName,
                ConnectionString = opts.Left.Connection.ConnectionString,
                FeatureSchema = opts.Left.SchemaName,
                Prefix = opts.LeftPrefix,
                PropertyList = new List<string>(opts.LeftProperties).ToArray(),
                Provider = opts.Left.Connection.Provider
            };

            jdef.Right = new FdoJoinSource
            {
                Class = opts.Right.ClassName,
                ConnectionString = opts.Right.Connection.ConnectionString,
                FeatureSchema = opts.Right.SchemaName,
                Prefix = opts.RightPrefix,
                PropertyList = new List<string>(opts.RightProperties).ToArray(),
                Provider = opts.Right.Connection.Provider
            };

            jdef.Target = new FdoJoinTarget
            {
                Class = opts.Target.ClassName,
                ConnectionString = opts.Target.Connection.ConnectionString,
                FeatureSchema = opts.Target.SchemaName,
                Provider = opts.Target.Connection.Provider
            };

            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;

                XmlSerializer ser = new XmlSerializer(typeof(FdoJoinTaskDefinition));
                ser.Serialize(writer, jdef);
            }
        }
    }

    /// <summary>
    /// Handler for generating connection names
    /// </summary>
    public delegate string ConnectionNameGenerationCallback(int seed);

    /// <summary>
    /// Helper class for Task Definition serialization
    /// </summary>
    public sealed class TaskDefinitionHelper
    {
        /// <summary>
        /// File extension for bulk copy definitions
        /// </summary>
        public const string BULKCOPYDEFINITION = ".BulkCopyDefinition";
        /// <summary>
        /// File extension for join definitions
        /// </summary>
        public const string JOINDEFINITION = ".JoinDefinition";
        /// <summary>
        /// File extension for sequential processes
        /// </summary>
        public const string SEQUENTIALPROCESS = ".SequentialProcess";

        /// <summary>
        /// Determines whether [the specified file] is a definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified file] is a definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefinitionFile(string file)
        {
            if (!File.Exists(file))
                return false;

            string ext = Path.GetExtension(file).ToLower();

            return (ext == SEQUENTIALPROCESS.ToLower()) || (ext == BULKCOPYDEFINITION.ToLower()) || (ext == JOINDEFINITION.ToLower());
        }

        /// <summary>
        /// Determines whether [the specified file] is a sequential process definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified file] is a sequential process definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSequentialProcess(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == SEQUENTIALPROCESS.ToLower());
        }

        /// <summary>
        /// Determines whether [the specified file] is a bulk copy definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified file] is a bulk copy definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBulkCopy(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == BULKCOPYDEFINITION.ToLower());
        }

        /// <summary>
        /// Determines whether the specified file is a join definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if the specified file is a join definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJoin(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == JOINDEFINITION.ToLower());
        }
    }

    /// <summary>
    /// Standalone task definition loader. Use this loader when using only the Core API. 
    /// Do not use this loader in the context of the FDO Toolbox application.
    /// </summary>
    public class DefinitionLoader : BaseDefinitionLoader
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="configPath">The configuration path</param>
        /// <param name="name">The name that will be assigned to the connection.</param>
        /// <returns></returns>
        protected override FdoConnection CreateConnection(string provider, string connStr, string configPath, ref string name)
        {
            var conn = new FdoConnection(provider, connStr);
            if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
                conn.SetConfiguration(configPath);
            return conn;
        }

        /// <summary>
        /// Prepares the specified bulk copy definition (freshly deserialized) before the loading process begins
        /// </summary>
        /// <param name="def">The bulk copy definition.</param>
        protected override NameValueCollection Prepare(FdoBulkCopyTaskDefinition def)
        {
            return new NameValueCollection();
        }
    }
}
