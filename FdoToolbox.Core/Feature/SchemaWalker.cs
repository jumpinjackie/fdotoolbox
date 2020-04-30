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
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;

namespace FdoToolbox.Core.Feature
{
    public class SchemaWalker
    {
        readonly IConnection _conn;

        public SchemaWalker(IConnection conn)
        {
            _conn = conn;
        }

        /// <summary>
        /// Gets the names of all classes for a given schema
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public List<string> GetClassNames(string schemaName)
        {
            List<string> classNames = new List<string>();
            if (SupportsPartialSchemaDiscovery())
            {
                using (IGetClassNames getnames = _conn.CreateCommand(CommandType.CommandType_GetClassNames) as IGetClassNames)
                {
                    getnames.SchemaName = schemaName;
                    OSGeo.FDO.Common.StringCollection names = getnames.Execute();
                    foreach (OSGeo.FDO.Common.StringElement sn in names)
                    {
                        //If the class name is qualified, un-qualify it
                        string className = sn.String;
                        string[] tokens = className.Split(':');
                        if (tokens.Length == 2)
                            classNames.Add(tokens[1]);
                        else
                            classNames.Add(sn.String);
                    }
                }
            }
            else
            {
                FeatureSchema schema = GetSchemaByName(schemaName);
                if (schema != null)
                {
                    foreach (ClassDefinition cd in schema.Classes)
                    {
                        classNames.Add(cd.Name);
                    }
                }
            }
            return classNames;
        }

        public List<string> GetQualifiedClassNames()
        {
            var names = new List<string>();
            if (SupportsPartialSchemaDiscovery())
            {
                var schemaNames = this.GetSchemaNames();
                foreach (var sn in schemaNames)
                {
                    var classNames = this.GetClassNames(sn);
                    foreach (var cn in classNames)
                    {
                        names.Add($"{sn}:{cn}");
                    }
                }
            }
            else
            {
                var schemas = this.DescribeSchema();
                foreach (FeatureSchema schema in schemas)
                {
                    var sn = schema.Name;
                    var classes = schema.Classes;
                    foreach (ClassDefinition klass in classes)
                    {
                        var cn = klass.Name;
                        names.Add($"{sn}:{cn}");
                    }
                }
            }
            names.Sort();
            return names;
        }

        /// <summary>
        /// Returns true if this connection supports partial schema discovery.
        /// ie. It supports IGetClassNames and IGetSchemaNames and enhanced IDescribeSchema
        /// </summary>
        /// <returns></returns>
        public bool SupportsPartialSchemaDiscovery()
        {
            var cmdCaps = _conn.CommandCapabilities;
            var cmds = cmdCaps.Commands;
            bool supportedCmds = (Array.IndexOf<int>(cmds, (int)CommandType.CommandType_GetClassNames) >= 0
                               && Array.IndexOf<int>(cmds, (int)CommandType.CommandType_GetSchemaNames) >= 0);
            using (var describe = (IDescribeSchema)_conn.CreateCommand(CommandType.CommandType_DescribeSchema))
            {
                bool supportsHint = (describe.ClassNames != null);
                return supportedCmds && supportsHint;
            }
        }

        /// <summary>
        /// Gets the names of all schemas in this connection
        /// </summary>
        /// <returns></returns>
        public List<string> GetSchemaNames()
        {
            List<string> schemaNames = new List<string>();
            if (SupportsPartialSchemaDiscovery())
            {
                using (IGetSchemaNames getnames = _conn.CreateCommand(CommandType.CommandType_GetSchemaNames) as IGetSchemaNames)
                {
                    OSGeo.FDO.Common.StringCollection names = getnames.Execute();
                    foreach (OSGeo.FDO.Common.StringElement sn in names)
                    {
                        schemaNames.Add(sn.String);
                    }
                }
            }
            else
            {
                var schemas = DescribeSchema();
                foreach (FeatureSchema fs in schemas)
                {
                    schemaNames.Add(fs.Name);
                }
            }
            return schemaNames;
        }

        /// <summary>
        /// Enumerates all the known feature schemas in the current connection.
        /// </summary>
        /// <returns></returns>
        public FeatureSchemaCollection DescribeSchema()
        {
            FeatureSchemaCollection schemas = null;
            using (IDescribeSchema describe = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                schemas = describe.Execute();
            }
            return schemas;
        }

        /// <summary>
        /// Gets a feature schema by name
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <returns>The feature schema. null if the schema was not found.</returns>
        public FeatureSchema GetSchemaByName(string schemaName)
        {
            if (string.IsNullOrEmpty(schemaName))
                return null;

            if (SupportsPartialSchemaDiscovery())
            {
                FeatureSchemaCollection schemas = null;
                using (IDescribeSchema describe = _conn.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    describe.SchemaName = schemaName;
                    schemas = describe.Execute();
                }
                if (schemas != null && schemas.Count == 1)
                    return schemas[0];
            }
            else
            {
                FeatureSchemaCollection schemas = DescribeSchema();

                foreach (FeatureSchema schema in schemas)
                {
                    if (schema.Name == schemaName)
                        return schema;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first matching class definition.
        /// </summary>
        /// <param name="className">The name of the class</param>
        /// <returns></returns>
        public ClassDefinition GetClassByName(string className)
        {
            if (string.IsNullOrEmpty(className))
                return null;

            //See if it is qualified
            var tokens = className.Split(':');
            if (tokens.Length == 2)
                return GetClassByName(tokens[0], tokens[1]);

            FeatureSchemaCollection schemas = this.DescribeSchema();
            if (schemas != null && schemas.Count > 0)
            {
                foreach (FeatureSchema sc in schemas)
                {
                    int cidx = sc.Classes.IndexOf(className);
                    if (cidx >= 0)
                        return sc.Classes[cidx];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a class definition by name
        /// </summary>
        /// <param name="schemaName">The parent schema name</param>
        /// <param name="className">The class name</param>
        /// <returns>The class definition if found. null if it wasn't</returns>
        public ClassDefinition GetClassByName(string schemaName, string className)
        {
            if (string.IsNullOrEmpty(className))
                return null;

            if (SupportsPartialSchemaDiscovery())
            {
                using (IDescribeSchema describe = _conn.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    try
                    {
                        describe.SchemaName = schemaName;
                        OSGeo.FDO.Common.StringElement el = new OSGeo.FDO.Common.StringElement(className);
                        describe.ClassNames.Add(el);
                        FeatureSchemaCollection schemas = describe.Execute();
                        if (schemas != null)
                            return schemas[0].Classes[0];
                    }
                    catch (OSGeo.FDO.Common.Exception)
                    {
                        return null;
                    }
                }
            }
            else
            {
                FeatureSchema schema = GetSchemaByName(schemaName);
                if (schema != null)
                {
                    foreach (ClassDefinition classDef in schema.Classes)
                    {
                        if (classDef.Name == className)
                            return classDef;
                    }
                }
            }
            return null;
        }
    }
}
