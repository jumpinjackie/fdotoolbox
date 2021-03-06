﻿#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Schema;
using FdoToolbox.Core.Feature;
using System.ComponentModel;
using FdoToolbox.Core.Utility;
using ICSharpCode.Core;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal delegate void SchemaElementEventHandler<T>(T item);
    public delegate void NodeUpdateHandler();

    public class SchemaDesignContext
    {
        private BindingList<SpatialContextInfo> _spatialContexts;

        public bool SchemasChanged
        {
            get
            {
                foreach (FeatureSchema sc in Schemas)
                {
                    if (sc.ElementState != SchemaElementState.SchemaElementState_Unchanged)
                        return true;
                }
                return false;
            }
        }

        public DataType[] SupportedDataTypes
        {
            get
            {
                if (IsConnected)
                {
                    using (var schemaCaps = Connection.SchemaCapabilities)
                    {
                        return schemaCaps.DataTypes;
                    }
                }
                else
                {
                    return (DataType[])Enum.GetValues(typeof(DataType));
                }
            }
        }

        public DataType[] SupportedAutogeneratedDataTypes
        {
            get
            {
                if (IsConnected)
                {
                    using (var schemaCaps = Connection.SchemaCapabilities)
                    {
                        return schemaCaps.SupportedAutoGeneratedTypes;
                    }
                }
                else
                {
                    return new DataType[] { DataType.DataType_Int32, DataType.DataType_Int64 };
                }
            }
        }

        public bool MappingsChanged
        {
            get;
            private set;
        }

        public bool IsConnected => this.Connection != null;

        public void EvaluateCapabilities()
        {
            //Default disconnected state
            this.CanOverrideSchemas = false;
            this.CanShowPhysicalMapping = false;
            this.CanModifyExistingSchemas = false;
            this.CanHaveMultipleSpatialContexts = true;
            this.CanHaveMultipleSchemas = true;
            this.CanDestroySpatialContexts = false;
            this.CanEditSpatialContexts = true;
            this.CanCreateSpatialContexts = true;
            this.CanHaveUniqueConstraints = true;
            this.SupportsValueConstraints = true;

            var conn = this.Connection;
            if (conn != null)
            {
                using (var connCaps = conn.ConnectionCapabilities)
                {
                    using (var cmdCaps = conn.CommandCapabilities)
                    {
                        using (var schemaCaps = conn.SchemaCapabilities)
                        {
                            var cmds = cmdCaps.Commands;

                            this.CanModifyExistingSchemas = schemaCaps.SupportsSchemaModification;
                            //this.CanOverrideSchemas = schemaCaps.SupportsSchemaOverrides;
                            //this.CanShowPhysicalMapping = Array.IndexOf(cmds, (int)OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchemaMapping) >= 0;
                            this.CanHaveMultipleSchemas = schemaCaps.SupportsMultipleSchemas;
                            this.CanHaveMultipleSpatialContexts = connCaps.SupportsMultipleSpatialContexts();
                            this.CanHaveUniqueConstraints = schemaCaps.SupportsUniqueValueConstraints;

                            this.CanDestroySpatialContexts = Array.IndexOf(cmds, (int)OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) >= 0;
                            this.CanCreateSpatialContexts = (Array.IndexOf(cmds, (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) >= 0);
                            this.CanEditSpatialContexts = _spatialContexts.Count > 0 && (Array.IndexOf(cmds, (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) >= 0);
                            this.SupportsValueConstraints =
                                schemaCaps.SupportsValueConstraintsList ||
                                schemaCaps.SupportsExclusiveValueRangeConstraints;
                        }
                    }
                }

                
            }
        }

        public SchemaDesignContext(FdoConnection conn)
        {
            _spatialContexts = new BindingList<SpatialContextInfo>();

            if (conn == null)
            {
                Schemas = new FeatureSchemaCollection(null);
                Mappings = new PhysicalSchemaMappingCollection();
            }
            else
            {
                using (var svc = conn.CreateFeatureService())
                {
                    Schemas = svc.DescribeSchema();
                    Mappings = svc.DescribeSchemaMapping(true);
                    if (Mappings == null)
                        Mappings = new PhysicalSchemaMappingCollection();

                    var spatialContexts = svc.GetSpatialContexts();
                    foreach (var sc in spatialContexts)
                    {
                        _spatialContexts.Add(sc);
                    }
                }
            }

            this.Connection = conn;

            EvaluateCapabilities();
        }

        internal void SetConfiguration(FdoDataStoreConfiguration conf)
        {
            Schemas = conf.Schemas;
            Mappings = conf.Mappings;

            if (Schemas == null)
                Schemas = new FeatureSchemaCollection(null);

            if (Mappings == null)
                Mappings = new PhysicalSchemaMappingCollection();

            _spatialContexts.Clear();
            if (conf.SpatialContexts != null && conf.SpatialContexts.Length > 0)
            {
                foreach (var sc in conf.SpatialContexts)
                {
                    _spatialContexts.Add(sc);
                }
            }
        }

        public FdoDataStoreConfiguration GetConfiguration()
        {
            return new FdoDataStoreConfiguration(
                Schemas,
                new List<SpatialContextInfo>(_spatialContexts).ToArray(),
                Mappings);
        }

        /// <summary>
        /// Indicates if schema mappings can be shown (the connection was able
        /// to retrieve a default schema mapping collection). Schema element
        /// mapping UIs will be read-only
        /// </summary>
        public bool CanShowPhysicalMapping
        {
            get;
            private set;
        }

        public bool CanHaveUniqueConstraints
        {
            get;
            private set;
        }

        public bool CanCreateSpatialContexts
        {
            get;
            private set;
        }

        public bool CanEditSpatialContexts
        {
            get;
            private set;
        }

        public bool CanDestroySpatialContexts
        {
            get;
            private set;
        }

        public bool CanModifyExistingSchemas
        {
            get;
            private set;
        }

        public bool SupportsValueConstraints
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates if the current physical schema mapping can be modified. If supported, Schema element
        /// mapping UIs will be enabled
        /// </summary>
        public bool CanOverrideSchemas
        {
            get;
            private set;
        }

        public bool CanHaveMultipleSchemas
        {
            get;
            private set;
        }

        public bool CanHaveMultipleSpatialContexts
        {
            get;
            private set;
        }

        public FdoConnection Connection
        {
            get;
            private set;
        }

        public FeatureSchemaCollection Schemas { get; private set; }

        public PhysicalSchemaMappingCollection Mappings { get; private set; }

        public BindingList<SpatialContextInfo> SpatialContexts => _spatialContexts;

        public ClassDefinition[] GetClasses(string schemaName)
        {
            var fidx = Schemas.IndexOf(schemaName);
            if (fidx >= 0)
            {
                var list = new List<ClassDefinition>();
                foreach (ClassDefinition cls in Schemas[fidx].Classes)
                {
                    list.Add(cls);
                }
                return list.ToArray();
            }
            return new ClassDefinition[0];
        }

        public ClassDefinition[] GetClassesExceptFor(string schemaName, string className)
        {
            var fidx = Schemas.IndexOf(schemaName);
            if (fidx >= 0)
            {
                var list = new List<ClassDefinition>();
                foreach (ClassDefinition cls in Schemas[fidx].Classes)
                {
                    if (cls.Name != className)
                        list.Add(cls);
                }
                return list.ToArray();
            }
            return new ClassDefinition[0];
        }

        public ClassDefinition GetClassDefinition(string schemaName, string className)
        {
            var fidx = Schemas.IndexOf(schemaName);
            if (fidx >= 0)
            {
                var classes = Schemas[fidx].Classes;
                var cidx = classes.IndexOf(className);
                if (cidx >= 0)
                {
                    return classes[cidx];
                }
            }
            return null;
        }

        private int counter = 0;

        internal string GenerateName(string prefix)
        {
            counter++;
            return prefix + counter;
        }

        internal event SchemaElementEventHandler<FeatureSchema> SchemaAdded;
        internal event SchemaElementEventHandler<ClassDefinition> ClassAdded;
        internal event SchemaElementEventHandler<ClassDefinition> ClassRemoved;
        internal event SchemaElementEventHandler<PropertyDefinition> PropertyAdded;
        internal event SchemaElementEventHandler<PropertyDefinition> PropertyRemoved;

        internal void ResetCounter()
        {
            counter = 0;
        }

        internal void AddSchema(FeatureSchema schema)
        {
            Schemas.Add(schema);
            //Broadcast

            var handler = this.SchemaAdded;
            if (handler != null)
                handler(schema);
        }

        internal void AddClass(string schema, ClassDefinition cls)
        {
            var fidx = Schemas.IndexOf(schema);
            if (fidx >= 0)
            {
                Schemas[fidx].Classes.Add(cls);
                //Broadcast
                var handler = this.ClassAdded;
                if (handler != null)
                    handler(cls);
            }
        }

        internal void AddProperty(string schema, string cls, PropertyDefinition prop)
        {
            var fidx = Schemas.IndexOf(schema);
            if (fidx >= 0)
            {
                var cidx = Schemas[fidx].Classes.IndexOf(cls);
                if (cidx >= 0)
                {
                    var cd = Schemas[fidx].Classes[cidx];
                    cd.Properties.Add(prop);
                    
                    //Broadcast
                    var handler = this.PropertyAdded;
                    if (handler != null)
                        handler(prop);
                }
            }
        }

        /*
        internal void MakeIdentity(string schema, string cls, DataPropertyDefinition prop)
        {
            var fidx = _schemas.IndexOf(schema);
            if (fidx >= 0)
            {
                var cidx = _schemas[fidx].Classes.IndexOf(cls);
                if (cidx >= 0)
                {
                    var cd = _schemas[fidx].Classes[cidx];
                    cd.IdentityProperties.Add(prop);

                    //Broadcast
                }
            }
        }*/

        internal bool FixIncompatibilities()
        {
            return false;
        }

        internal bool ClassNameExists(string schema, string name)
        {
            var fidx = Schemas.IndexOf(schema);
            if (fidx >= 0)
                return Schemas[fidx].Classes.Contains(name);

            return false;
        }

        internal bool SchemaNameExists(string name)
        {
            return Schemas.Contains(name);
        }

        internal bool PropertyNameExists(string schema, string clsName, string name)
        {
            var fidx = Schemas.IndexOf(schema);
            if (fidx >= 0)
            {
                var cidx = Schemas[fidx].Classes.IndexOf(clsName);
                if (cidx >= 0)
                    return Schemas[fidx].Classes[clsName].Properties.Contains(name);
            }
            return false;
        }

        internal void DeleteClass(ClassDefinition classDefinition)
        {
            classDefinition.Delete();
            var handler = this.ClassRemoved;
            if (handler != null)
                handler(classDefinition);
        }

        internal void DeleteProperty(PropertyDefinition propertyDefinition)
        {
            propertyDefinition.Delete();
            var handler = this.PropertyRemoved;
            if (handler != null)
                handler(propertyDefinition);
        }

        internal bool IsSupportedClass(ClassType classType)
        {
            if (this.Connection == null)
            {
                return true;
            }
            else
            {
                using (var schemaCaps=  Connection.SchemaCapabilities)
                {
                    return Array.IndexOf(schemaCaps.ClassTypes, classType) >= 0;
                }
            }
        }

        internal bool IsSupportedProperty(PropertyType propertyType)
        {
            if (this.Connection == null)
            {
                return true;
            }
            else
            {
                using (var schemaCaps = Connection.SchemaCapabilities)
                {
                    if (propertyType == PropertyType.PropertyType_AssociationProperty)
                        return schemaCaps.SupportsAssociationProperties;
                    else if (propertyType == PropertyType.PropertyType_ObjectProperty)
                        return schemaCaps.SupportsObjectProperties;
                    else if (propertyType == PropertyType.PropertyType_RasterProperty)
                        return false;
                    else
                        return true;
                }
            }
        }

        internal void AddSpatialContext(SpatialContextInfo sc)
        {
            _spatialContexts.Add(sc);
        }

        internal bool SaveSchema(string schName)
        {
            if (!this.SchemasChanged)
                return false;

            //Apply as-is. Errors applying schema is a message for the
            //user to click "Fix Incompatibilities"

            //TODO: Incorporate schema mappings. As it stands mappings are
            //read-only and are only used to dump out XML config files

            var fs = Schemas[schName];
            using (var svc = this.Connection.CreateFeatureService())
            {
                svc.ApplySchema(fs);
                return true;
            }
        }

        internal bool SaveAllSchemas()
        {
            if (!this.SchemasChanged)
                return false;

            //Apply as-is. Errors applying schema is a message for the
            //user to click "Fix Incompatibilities"

            //TODO: Incorporate schema mappings. As it stands mappings are
            //read-only and are only used to dump out XML config files

            using (var svc = this.Connection.CreateFeatureService())
            {
                svc.ApplySchemas(Schemas);
                return true;
            }
        }

        internal bool SaveSpatialContexts()
        {
            using (var svc = this.Connection.CreateFeatureService())
            {
                var contexts = svc.GetSpatialContexts();
                var existing = new Dictionary<string, SpatialContextInfo>();
                foreach (var c in contexts)
                {
                    existing[c.Name] = c;
                }

                var update = new List<SpatialContextInfo>();
                var delete = new List<SpatialContextInfo>();
                var create = new List<SpatialContextInfo>();

                var current = new Dictionary<string, SpatialContextInfo>();

                foreach (var c in _spatialContexts)
                {
                    current[c.Name] = c;
                    if (existing.ContainsKey(c.Name))
                        update.Add(c);
                    else
                        create.Add(c);
                }

                foreach (var c in existing.Values)
                {
                    if (!current.ContainsKey(c.Name))
                        delete.Add(c);
                }

                //Delete removed ones
                foreach (var c in delete)
                {
                    LoggingService.Info("Removing spatial context: " + c.Name);
                    svc.DestroySpatialContext(c);
                }
                //Create added ones
                foreach (var c in create)
                {
                    LoggingService.Info("Adding spatial context: " + c.Name);
                    svc.CreateSpatialContext(c, false);
                }
                //Update existing ones
                foreach (var c in update)
                {
                    try
                    {
                        LoggingService.Info("Updating spatial context: " + c.Name);
                        svc.CreateSpatialContext(c, true);
                    }
                    catch //Use destroy/create method
                    {
                        svc.DestroySpatialContext(c);
                        svc.CreateSpatialContext(c, false);
                    }
                }
                //Update current spatial context list
                _spatialContexts.Clear();
                foreach (var c in svc.GetSpatialContexts())
                {
                    _spatialContexts.Add(c);
                }
                this.SpatialContextsChanged = false;
                return true;
            }
        }

        internal IncompatibleSchema[] FindIncompatibilities()
        {
            var inc = new List<IncompatibleSchema>();
            using (var svc = this.Connection.CreateFeatureService())
            {
                foreach (FeatureSchema fs in Schemas)
                {
                    IncompatibleSchema ins;
                    if (!svc.CanApplySchema(fs, out ins))
                    {
                        inc.Add(ins);
                    }
                }
            }
            return inc.ToArray();
        }

        internal void UndoSchemaChanges()
        {
            foreach (FeatureSchema schema in Schemas)
            {
                schema.RejectChanges();
            }
        }

        public bool SpatialContextsChanged
        {
            get;
            private set;
        }

        internal void RemoveSpatialContext(SpatialContextInfo sc)
        {
            _spatialContexts.Remove(sc);
            this.SpatialContextsChanged = true;
        }

        internal void UpdateSpatialContext(SpatialContextInfo sc)
        {
            this.SpatialContextsChanged = true;
            SpatialContextInfo edit = null;
            foreach (var context in _spatialContexts)
            {
                if (context.Name == sc.Name)
                {
                    edit = context;
                }
            }

            if (edit != null)
            {
                edit.CoordinateSystem = sc.CoordinateSystem;
                edit.CoordinateSystemWkt = sc.CoordinateSystemWkt;
                edit.Description = sc.Description;
                edit.ExtentGeometryText = sc.ExtentGeometryText;
                edit.ExtentType = sc.ExtentType;
                edit.XYTolerance = sc.XYTolerance;
                edit.ZTolerance = sc.ZTolerance;
            }
            else
            {
                _spatialContexts.Add(sc);
            }
        }

        internal string[] AddClassesToSchema(string schemaName, ClassCollection classCollection)
        {
            List<string> notAdded = new List<string>();
            FeatureSchema fsc = null;
            var fidx = Schemas.IndexOf(schemaName);
            if (fidx >= 0)
            {
                fsc = Schemas[fidx];
            }
            else
            {
                fsc = new FeatureSchema(schemaName, "");
                AddSchema(fsc);
            }

            foreach (ClassDefinition cls in classCollection)
            {
                var cidx = fsc.Classes.IndexOf(cls.Name);
                if (cidx >= 0)
                {
                    notAdded.Add(cls.Name);
                }
                else
                {
                    ClassDefinition copy = FdoSchemaUtil.CloneClass(cls, false);
                    AddClass(fsc.Name, copy);
                }
            }

            return notAdded.ToArray();
        }

        internal string[] GetSpatialContextNames()
        {
            List<string> names = new List<string>();
            foreach (var sc in _spatialContexts)
            {
                names.Add(sc.Name);
            }
            return names.ToArray();
        }
    }
}
