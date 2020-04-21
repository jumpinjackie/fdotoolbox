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
using FdoToolbox.Core.Configuration;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Connections.Capabilities;
using System.Linq;

namespace FdoToolbox.Core.ETL.Specialized
{
    public class SCOverrideItem
    {
        public string OverrideScName { get; set; }

        public string CsName { get; set; }

        public string CsWkt { get; set; }

        /// <summary>
        /// If true, data will be transformed to the coordinate system specified by <see cref="CsWkt"/>, otherwise
        /// the copy will use this override merely to rewrite the destination's spatial metadata
        /// </summary>
        public bool TransformToThis { get; set; }
    }

    /// <summary>
    /// Defines the options for a <see cref="FdoClassToClassCopyProcess"/> instance
    /// </summary>
    public class FdoClassCopyOptions
    {
        private string _SourceConnectionName;
        private string _TargetConnectionName;

        internal TargetClassModificationItem PreCopyTargetModifier { get; set; }

        /// <summary>
        /// Sets any override WKTs for spatial contexts to be copied/created
        /// </summary>
        public Dictionary<string, SCOverrideItem> OverrideWkts { get; set; }

        /// <summary>
        /// Gets the name of the source connection.
        /// </summary>
        /// <value>The name of the source connection.</value>
        public string SourceConnectionName
        {
            get { return _SourceConnectionName; }
            internal set { _SourceConnectionName = value; }
        }

        /// <summary>
        /// Gets the name of the target connection.
        /// </summary>
        /// <value>The name of the target connection.</value>
        public string TargetConnectionName
        {
            get { return _TargetConnectionName; }
            internal set { _TargetConnectionName = value; }
        }

        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Gets or sets whether the source class definition should be created in the target schema 
        /// </summary>
        public bool CreateIfNotExists
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoClassCopyOptions"/> class.
        /// </summary>
        /// <param name="sourceConnectionName">Name of the source connection.</param>
        /// <param name="targetConnectionName">Name of the target connection.</param>
        /// <param name="sourceSchema">The source schema.</param>
        /// <param name="sourceClass">The source class.</param>
        /// <param name="targetSchema">The target schema.</param>
        /// <param name="targetClass">The target class.</param>
        /// <param name="targetClassOv"></param>
        public FdoClassCopyOptions(string sourceConnectionName,
                                   string targetConnectionName,
                                   string sourceSchema,
                                   string sourceClass,
                                   string targetSchema,
                                   string targetClass,
                                   string targetClassOv)
        {
            _SourceConnectionName = sourceConnectionName;
            _TargetConnectionName = targetConnectionName;
            _sourceClass = sourceClass;
            _sourceSchema = sourceSchema;
            _targetClass = targetClass;
            this.TargetClassNameOverride = targetClassOv;
            _targetSchema = targetSchema;

            _propertyMappings = new NameValueCollection();
            _expressionAliasMap = new NameValueCollection();
            _expressionMappings = new NameValueCollection();

            _rules = new Dictionary<string,FdoDataPropertyConversionRule>();
        }

        private string _sourceSchema;

        /// <summary>
        /// Gets the source schema.
        /// </summary>
        /// <value>The source schema.</value>
        public string SourceSchema
        {
            get { return _sourceSchema; }
        }

        private string _targetSchema;

        /// <summary>
        /// Gets the target schema.
        /// </summary>
        /// <value>The target schema.</value>
        public string TargetSchema
        {
            get { return _targetSchema; }
        }

        private string _sourceClass;

        /// <summary>
        /// Gets the source feature class to copy from
        /// </summary>
        public string SourceClassName
        {
            get { return _sourceClass; }
        }

        private string _targetClass;

        /// <summary>
        /// Gets the target feature class to write to
        /// </summary>
        public string TargetClassName
        {
            get { return _targetClass; }
        }

        /// <summary>
        /// If creating the target class, use the specified name here. Otherwise, fall
        /// back to <see cref="TargetClassName"/>
        /// </summary>
        public string TargetClassNameOverride
        {
            get;
            private set;
        }

        private string _SourceFilter;

        /// <summary>
        /// Gets or sets the filter to apply to the source class query
        /// </summary>
        public string SourceFilter
        {
            get { return _SourceFilter; }
            set { _SourceFilter = value; }
        }

        private bool _DeleteTarget;

        /// <summary>
        /// Determines if the data in the target feature class should be 
        /// deleted before commencing copying.
        /// </summary>
        public bool DeleteTarget
        {
            get { return _DeleteTarget; }
            set { _DeleteTarget = value; }
        }

        private FdoBulkCopyOptions _Parent;

        /// <summary>
        /// Gets the bulk copy options
        /// </summary>
        public FdoBulkCopyOptions Parent
        {
            get { return _Parent; }
            internal set { _Parent = value; }
        }

        private NameValueCollection _propertyMappings;
        private NameValueCollection _expressionMappings;
        private NameValueCollection _expressionAliasMap;

        /// <summary>
        /// Gets the list of source property names. Use this to get the mapped (target)
        /// property name. If this is empty, then all source properties will be used
        /// as target properties
        /// </summary>
        public string[] SourcePropertyNames
        {
            get { return _propertyMappings.AllKeys; }
        }

        /// <summary>
        /// Gets the list of source expression aliases.
        /// </summary>
        /// <value>The source aliases.</value>
        public string[] SourceAliases
        {
            get { return _expressionAliasMap.AllKeys; }
        }

        /// <summary>
        /// Adds a source to target property mapping.
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="targetProperty"></param>
        public void AddPropertyMapping(string sourceProperty, string targetProperty)
        {
            _propertyMappings[sourceProperty] = targetProperty;
        }

        private List<string> _checkSourceProperties = new List<string>();

        /// <summary>
        /// Adds a source property to check if it needs to be created in the target class definition
        /// </summary>
        /// <param name="sourceProperty"></param>
        public void AddSourcePropertyToCheck(string sourceProperty)
        {
            _checkSourceProperties.Add(sourceProperty);
        }

        /// <summary>
        /// Gets an array of source property names that need to be checked in the target class to see
        /// whether they need to be created or not.
        /// </summary>
        public string[] CheckSourceProperties
        {
            get { return _checkSourceProperties.ToArray(); }
        }

        private int _BatchSize;

        /// <summary>
        /// Gets or sets the batch size. If greater than zero, a batch insert operation
        /// will be used instead of a regular insert operation (if supported by the
        /// target connection)
        /// </summary>
        public int BatchSize
        {
            get { return _BatchSize; }
            set { _BatchSize = value; }
        }

        private bool _ForceWkb;

        /// <summary>
        /// Gets or sets a value indicating whether to force the input geometries to be 
        /// WKB compliant. If necessary, the geometry will be flattened (stripped of Z and
        /// M components)
        /// </summary>
        public bool ForceWkb
        {
            get { return _ForceWkb; }
            set { _ForceWkb = value; }
        }

        private bool _FlattenGeometries;

        /// <summary>
        /// Gets or sets a value indicating whether to strip the Z and M components of all
        /// geometries
        /// </summary>
        /// <value><c>true</c> if [flatten geometries]; otherwise, <c>false</c>.</value>
        public bool FlattenGeometries
        {
            get { return _FlattenGeometries; }
            set { _FlattenGeometries = value; }
        }

        /// <summary>
        /// Adds the source expression.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="targetProp">The target property.</param>
        public void AddSourceExpression(string alias, string expression, string targetProp)
        {
            _expressionAliasMap[alias] = expression;
            _expressionMappings[alias] = targetProp;
        }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <param name="srcProp">The source prop.</param>
        /// <returns></returns>
        public string GetTargetProperty(string srcProp)
        {
            return _propertyMappings[srcProp];
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public string GetExpression(string alias)
        {
            return _expressionAliasMap[alias];
        }

        /// <summary>
        /// Gets the target property for alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public string GetTargetPropertyForAlias(string alias)
        {
            if (_expressionAliasMap[alias] != null)
                return _expressionMappings[alias];

            return null;
        }

        private Dictionary<string, FdoDataPropertyConversionRule> _rules;

        /// <summary>
        /// Gets the conversion rules.
        /// </summary>
        /// <value>The conversion rules.</value>
        public ICollection<FdoDataPropertyConversionRule> ConversionRules
        {
            get { return _rules.Values; }
        }

        /// <summary>
        /// Adds a data conversion rule.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="rule">The rule.</param>
        public void AddDataConversionRule(string name, FdoDataPropertyConversionRule rule)
        {
            _rules.Add(name, rule);
        }

        /// <summary>
        /// Gets the data conversion rule.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FdoDataPropertyConversionRule GetDataConversionRule(string name)
        {
            if (_rules.ContainsKey(name))
                return _rules[name];
            return null;
        }

        internal static FdoClassCopyOptions FromElement(FdoCopyTaskElement el, FeatureSchemaCache cache, FdoConnection sourceConn, FdoConnection targetConn, out TargetClassModificationItem mod)
        {
            mod = null;
            if (!cache.HasConnection(el.Source.connection))
                throw new TaskLoaderException("The referenced source connection is not defined");

            if (!cache.HasConnection(el.Target.connection))
                throw new TaskLoaderException("The referenced target connection is not defined");

            var opts = new FdoClassCopyOptions(el.Source.connection, el.Target.connection, el.Source.schema, el.Source.@class, el.Target.schema, el.Target.@class, el.Target.createAs);
            opts.DeleteTarget = el.Options.DeleteTarget;
            opts.SourceFilter = el.Options.Filter;
            if (!el.Options.FlattenGeometriesSpecified)
                opts.FlattenGeometries = false;
            else
                opts.FlattenGeometries = el.Options.FlattenGeometries;

            if (!el.Options.ForceWKBSpecified)
                opts.ForceWkb = false;
            else
                opts.ForceWkb = el.Options.ForceWKB;

            opts.OverrideWkts = el.Options.SpatialContextWktOverrides?.ToDictionary(item => item.Name, item => new SCOverrideItem
            {
                OverrideScName = item.OverrideName,
                CsName = item.CoordinateSystemName,
                CsWkt = item.CoordinateSystemWkt
            }) ?? new Dictionary<string, SCOverrideItem>();

            if (!string.IsNullOrEmpty(el.Options.BatchSize))
                opts.BatchSize = Convert.ToInt32(el.Options.BatchSize);
            opts.Name = el.name;
            opts.CreateIfNotExists = el.createIfNotExists;
            opts.TargetClassNameOverride = el.Target.createAs;

            ClassDefinition srcClass = cache.GetClassByName(el.Source.connection, el.Source.schema, el.Source.@class);
            ClassDefinition dstClass = cache.GetClassByName(el.Target.connection, el.Target.schema, el.Target.@class);

            if (!el.createIfNotExists && dstClass == null)
                throw new InvalidOperationException("Target class " + el.Target.@class + " does not exist and the createIfNotExist option is false");

            SpatialContextInfo defaultSc = null;
            FunctionDefinitionCollection availableFunctions = (FunctionDefinitionCollection)sourceConn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_ExpressionFunctions);

            using (var svc = targetConn.CreateFeatureService())
            {
                defaultSc = svc.GetActiveSpatialContext();
            }

            if (dstClass != null)
            {
                foreach (FdoPropertyMappingElement propMap in el.PropertyMappings)
                {
                    if (srcClass.Properties.IndexOf(propMap.source) < 0)
                        throw new TaskLoaderException("The property mapping (" + propMap.source + " -> " + propMap.target + ") in task (" + el.name + ") contains a source property not found in the source class definition (" + el.Source.@class + ")");

                    //Add to list of properties to check for
                    if (propMap.createIfNotExists && dstClass.Properties.IndexOf(propMap.target) < 0)
                    {
                        if (mod == null)
                            mod = new UpdateTargetClass(dstClass.Name);

                        opts.AddSourcePropertyToCheck(propMap.source);

                        //Clone copy of source property of same name
                        var srcProp = srcClass.Properties[srcClass.Properties.IndexOf(propMap.source)];
                        srcProp = FdoSchemaUtil.CloneProperty(srcProp);
                        mod.AddProperty(srcProp);
                    }
                    else
                    {
                        if (dstClass.Properties.IndexOf(propMap.target) < 0)
                            throw new TaskLoaderException("The property mapping (" + propMap.source + " -> " + propMap.target + ") in task (" + el.name + ") contains a target property not found in the target class definition (" + el.Target.@class + ")");

                        PropertyDefinition sp = srcClass.Properties[propMap.source];
                        PropertyDefinition tp = dstClass.Properties[propMap.target];

                        if (sp.PropertyType != tp.PropertyType)
                            throw new TaskLoaderException("The properties in the mapping (" + propMap.source + " -> " + propMap.target + ") are of different types");

                        //if (sp.PropertyType != PropertyType.PropertyType_DataProperty)
                        //    throw new TaskLoaderException("One or more properties in the mapping (" + propMap.source + " -> " + propMap.target + ") is not a data property");

                        DataPropertyDefinition sdp = sp as DataPropertyDefinition;
                        DataPropertyDefinition tdp = tp as DataPropertyDefinition;

                        opts.AddPropertyMapping(propMap.source, propMap.target);

                        //Property mapping is between two data properties
                        if (sdp != null && tdp != null)
                        {
                            //Types not equal, so add a conversion rule
                            if (sdp.DataType != tdp.DataType)
                            {
                                FdoDataPropertyConversionRule rule = new FdoDataPropertyConversionRule(
                                    propMap.source,
                                    propMap.target,
                                    sdp.DataType,
                                    tdp.DataType,
                                    propMap.nullOnFailedConversion,
                                    propMap.truncate);
                                opts.AddDataConversionRule(propMap.source, rule);
                            }
                        }
                    }
                }

                //
                foreach (FdoExpressionMappingElement exprMap in el.ExpressionMappings)
                {
                    if (string.IsNullOrEmpty(exprMap.target))
                        continue;

                    opts.AddSourceExpression(exprMap.alias, exprMap.Expression, exprMap.target);
                    //Add to list of properties to check for
                    if (exprMap.createIfNotExists)
                    {
                        //Class exists but property doesn't
                        if (dstClass.Properties.IndexOf(exprMap.target) < 0)
                        {
                            if (mod == null)
                            {
                                mod = new UpdateTargetClass(el.Target.@class);
                            }

                            var prop = FdoSchemaUtil.CreatePropertyFromExpressionType(exprMap.Expression, srcClass, availableFunctions, defaultSc.Name);
                            if (prop == null)
                            {
                                throw new InvalidOperationException("Could not derive a property definition from the expression: " + exprMap.Expression);
                            }

                            prop.Name = exprMap.target;
                            mod.AddProperty(prop);
                        }
                    }
                    else //Conversion rules can only apply if both properties exist.
                    {
                        FdoPropertyType? pt = ExpressionUtility.ParseExpressionType(exprMap.Expression, sourceConn);
                        if (pt.HasValue)
                        {
                            DataType? srcDt = ValueConverter.GetDataType(pt.Value);
                            if (srcDt.HasValue)
                            {
                                PropertyDefinition tp = dstClass.Properties[exprMap.target];
                                DataPropertyDefinition tdp = tp as DataPropertyDefinition;
                                if (tdp != null)
                                {
                                    if (srcDt.Value != tdp.DataType)
                                    {
                                        FdoDataPropertyConversionRule rule = new FdoDataPropertyConversionRule(
                                            exprMap.alias,
                                            exprMap.target,
                                            srcDt.Value,
                                            tdp.DataType,
                                            exprMap.nullOnFailedConversion,
                                            exprMap.truncate);
                                        opts.AddDataConversionRule(exprMap.alias, rule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else //class doesn't exist
            {
                mod = new CreateTargetClassFromSource(el.Source.schema, el.Target.@class);

                foreach (var propMap in el.PropertyMappings)
                {
                    opts.AddPropertyMapping(propMap.source, propMap.target);

                    if (propMap.createIfNotExists)
                    {
                        opts.AddSourcePropertyToCheck(propMap.source);
                    }
                }

                foreach (var exprMap in el.ExpressionMappings)
                {
                    opts.AddSourceExpression(exprMap.alias, exprMap.Expression, exprMap.target);

                    if (exprMap.createIfNotExists)
                        opts.AddSourcePropertyToCheck(exprMap.alias);

                    var prop = FdoSchemaUtil.CreatePropertyFromExpressionType(exprMap.Expression, srcClass, availableFunctions, defaultSc.Name);
                    if (prop == null)
                    {
                        throw new InvalidOperationException("Could not derive a property definition from the expression: " + exprMap.Expression);
                    }
                    prop.Name = exprMap.target;
                    mod.AddProperty(prop);
                }
            }

            return opts;
        }

        internal FdoCopyTaskElement ToElement()
        {
            FdoCopyTaskElement el = new FdoCopyTaskElement();
            el.name = this.Name;
            el.createIfNotExists = this.CreateIfNotExists;
            el.Options = new FdoCopyOptionsElement();
            el.Options.DeleteTarget = this.DeleteTarget;
            el.Options.Filter = this.SourceFilter;
            el.Options.FlattenGeometries = this.FlattenGeometries;
            el.Options.FlattenGeometriesSpecified = true;
            el.Options.ForceWKB = this.ForceWkb;
            el.Options.ForceWKBSpecified = true;
            el.Options.SpatialContextWktOverrides = this.OverrideWkts.Select(kvp => new SpatialContextOverrideItem
            {
                Name = kvp.Key,
                OverrideName = kvp.Value.OverrideScName,
                CoordinateSystemName = kvp.Value.CsName,
                CoordinateSystemWkt = kvp.Value.CsWkt
            }).ToArray();

            if (this.BatchSize > 0)
                el.Options.BatchSize = this.BatchSize.ToString();

            el.Source = new FdoCopySourceElement();
            
            el.Source.connection = this.SourceConnectionName;
            el.Source.schema = this.SourceSchema;
            el.Source.@class = this.SourceClassName;

            el.Target = new FdoCopyTargetElement();

            el.Target.connection = this.TargetConnectionName;
            el.Target.schema = this.TargetSchema;
            el.Target.@class = this.TargetClassName;
            el.Target.createAs = this.TargetClassNameOverride;

            List<FdoPropertyMappingElement> propMappings = new List<FdoPropertyMappingElement>();
            List<FdoExpressionMappingElement> exprMappings = new List<FdoExpressionMappingElement>();

            List<string> check = new List<string>(this.CheckSourceProperties);
            Dictionary<string, FdoPropertyMappingElement> convRules = new Dictionary<string, FdoPropertyMappingElement>();
            foreach (FdoDataPropertyConversionRule rule in this.ConversionRules)
            {
                FdoPropertyMappingElement map = new FdoPropertyMappingElement();
                //map.sourceDataType = rule.SourceDataType.ToString();
                //map.targetDataType = rule.TargetDataType.ToString();
                map.nullOnFailedConversion = rule.NullOnFailure;
                map.truncate = rule.Truncate;
                map.source = rule.SourceProperty;
                map.target = rule.TargetProperty;
                if (check.Contains(map.source))
                    map.createIfNotExists = true;

                convRules.Add(map.source, map);
            }

            foreach (string prop in this.SourcePropertyNames)
            {
                if (convRules.ContainsKey(prop))
                {
                    propMappings.Add(convRules[prop]);
                }
                else 
                {
                    FdoPropertyMappingElement map = new FdoPropertyMappingElement();
                    map.source = prop;
                    map.target = this.GetTargetProperty(prop);
                    if (check.Contains(map.source))
                        map.createIfNotExists = true;

                    propMappings.Add(map);
                }
            }

            foreach (string alias in this.SourceAliases)
            {
                FdoExpressionMappingElement map = new FdoExpressionMappingElement();
                map.alias = alias;
                map.Expression = this.GetExpression(alias);
                map.target = this.GetTargetPropertyForAlias(alias);
                if (check.Contains(map.alias))
                    map.createIfNotExists = true;

                exprMappings.Add(map);
            }

            el.PropertyMappings = propMappings.ToArray();
            el.ExpressionMappings = exprMappings.ToArray();

            return el;
        }
    }
}
