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
using OSGeo.FDO.Expression;
using OSGeo.FDO.Connections.Capabilities;
using Res = FdoToolbox.Core.ResourceUtil;
using System.Diagnostics;
using OSGeo.FDO.Common.Io;

namespace FdoToolbox.Core.Utility
{
    public static class FdoSchemaUtil
    {
        public static int SetDefaultSpatialContextAssociation(FeatureSchemaCollection fsc, string name)
        {
            int modified = 0;
            foreach (FeatureSchema fs in fsc)
            {
                modified += SetDefaultSpatialContextAssociation(fs, name);
            }
            return modified;
        }

        public static int SetDefaultSpatialContextAssociation(FeatureSchema fs, string name)
        {
            int modified = 0;
            foreach (ClassDefinition cls in fs.Classes)
            {
                foreach (PropertyDefinition prop in cls.Properties)
                {
                    if (prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    {
                        GeometricPropertyDefinition geom = (GeometricPropertyDefinition)prop;
                        if (!geom.SpatialContextAssociation.Equals(name))
                        {
                            geom.SpatialContextAssociation = name;
                            modified++;
                        }
                    }
                }
            }
            return modified;
        }

        internal static PropertyDefinition CreatePropertyFromExpressionType(string exprText, ClassDefinition clsDef, FunctionDefinitionCollection functionDefs, string defaultSpatialContextName)
        {
            string name = string.Empty;
            using (var expr = Expression.Parse(exprText))
            {
                var et = expr.GetType();
                if (typeof(ComputedIdentifier).IsAssignableFrom(et))
                {
                    var subExpr = ((ComputedIdentifier)expr).Expression;
                    return CreatePropertyFromExpressionType(subExpr.ToString(), clsDef, functionDefs, defaultSpatialContextName);
                }
                else if (typeof(Identifier).IsAssignableFrom(et))
                {
                    var id = (Identifier)expr;
                    return CloneProperty(clsDef.Properties[id.Name]);
                }
                else if (typeof(Function).IsAssignableFrom(et))
                {
                    var func = (Function)expr;
                    if (functionDefs != null)
                    {
                        var fidx = functionDefs.IndexOf(func.Name);
                        if (fidx >= 0)
                        {
                            var funcDef = functionDefs[fidx];
                            switch (funcDef.ReturnPropertyType)
                            {
                                case PropertyType.PropertyType_DataProperty:
                                    {
                                        var dp = new DataPropertyDefinition(name, "")
                                        {
                                            DataType = funcDef.ReturnType,
                                            Nullable = true
                                        };
                                        if (dp.DataType == DataType.DataType_String)
                                            dp.Length = 255;

                                        return dp;
                                    }
                                    break;
                                case PropertyType.PropertyType_GeometricProperty:
                                    {
                                        var geom = new GeometricPropertyDefinition(name, "")
                                        {
                                            SpatialContextAssociation = defaultSpatialContextName,
                                            GeometryTypes = (int)GeometricType.GeometricType_All
                                        };

                                        return geom;
                                    }
                                    break;

                            }
                        }
                    }
                }
                else if (typeof(BinaryExpression).IsAssignableFrom(et))
                {
                    var dp = new DataPropertyDefinition(name, "")
                    {
                        DataType = DataType.DataType_Boolean,
                        Nullable = true
                    };

                    return dp;
                }
                else if (typeof(DataValue).IsAssignableFrom(et))
                {
                    var dv = (DataValue)expr;
                    var dp = new DataPropertyDefinition(name, "")
                    {
                        DataType = dv.DataType
                    };
                    if (dp.DataType == DataType.DataType_String)
                        dp.Length = 255;

                    dp.Nullable = true;
                    return dp;
                }
                else if (typeof(GeometryValue).IsAssignableFrom(et))
                {
                    var geom = new GeometricPropertyDefinition(name, "")
                    {
                        GeometryTypes = (int)GeometricType.GeometricType_All
                    };

                    return geom;
                }
            }
            return null;
        }

        /// <summary>
        /// Utility method to clone a feature schema
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static FeatureSchema CloneSchema(FeatureSchema fs)
        {
            return CloneSchema(fs, false);
        }

        private static ICollection<string> GetReferencedClasses(ClassDefinition clsDef)
        {
            HashSet<string> clsNames = new HashSet<string>();

            foreach (PropertyDefinition propDef in clsDef.Properties)
            {
                if (propDef.PropertyType == PropertyType.PropertyType_AssociationProperty)
                {
                    AssociationPropertyDefinition ap = (AssociationPropertyDefinition)propDef;
                    clsNames.Add(ap.AssociatedClass.QualifiedName);
                }
                else if (propDef.PropertyType == PropertyType.PropertyType_ObjectProperty)
                {
                    ObjectPropertyDefinition op = (ObjectPropertyDefinition)propDef;
                    clsNames.Add(op.Class.QualifiedName);
                }
            }

            return clsNames;
        }

        /// <summary>
        /// Utility method to clone a feature schema
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="ignoreDeleted"></param>
        /// <returns></returns>
        public static FeatureSchema CloneSchema(FeatureSchema fs, bool ignoreDeleted)
        {
            if (fs == null)
                throw new ArgumentNullException("fs");

            var fsc = new FeatureSchemaCollection(null);
            using (var ios = new IoMemoryStream())
            {
                fs.WriteXml(ios);
                ios.Reset();
                fsc.ReadXml(ios);
            }
            if (fsc.Count > 0)
            {
                var cloned = fsc[0];
                if (ignoreDeleted)
                {
                    if (ApplySchemaElementDeletions(cloned, fs))
                        cloned.AcceptChanges();
                }
                return cloned;
            }
            return null;

            bool ApplyClassElementDeletions(ClassDefinition target, ClassDefinition source)
            {
                var tProperties = target.Properties;
                var sProperties = source.Properties;

                bool bHasChanges = false;
                foreach (PropertyDefinition prop in sProperties)
                {
                    if (prop.ElementState == SchemaElementState.SchemaElementState_Deleted)
                    {
                        var tpIdx = tProperties.IndexOf(prop.Name);
                        if (tpIdx >= 0)
                        {
                            var tProp = tProperties[tpIdx];
                            tProp.Delete();
                            bHasChanges = true;
                        }
                    }
                }
                return bHasChanges;
            }

            bool ApplySchemaElementDeletions(FeatureSchema target, FeatureSchema source)
            {
                bool bHasChanges = false;

                var tClasses = target.Classes;
                var sClasses = source.Classes;

                var deleteClasses = new List<string>();
                foreach (ClassDefinition cls in sClasses)
                {
                    if (cls.ElementState == SchemaElementState.SchemaElementState_Deleted)
                        deleteClasses.Add(cls.Name);

                    var tcIdx = tClasses.IndexOf(cls.Name);
                    if (tcIdx >= 0)
                    {
                        var tCls = tClasses[tcIdx];
                        if (ApplyClassElementDeletions(tCls, cls))
                            bHasChanges = true;
                    }
                }

                foreach (var cn in deleteClasses)
                {
                    var tcIdx = tClasses.IndexOf(cn);
                    if (tcIdx >= 0)
                    {
                        var tCls = tClasses[tcIdx];
                        tCls.Delete();
                        bHasChanges = true;
                    }
                }

                return bHasChanges;
            }
        }

        /// <summary>
        /// Clones the class.
        /// </summary>
        /// <param name="cls">The CLS.</param>
        /// <returns></returns>
        public static ClassDefinition CloneClass(ClassDefinition cls)
        {
            return CloneClass(cls, false);
        }

        /// <summary>
        /// Utility method to clone a class definition
        /// </summary>
        /// <param name="cd">The class to clone.</param>
        /// <param name="ignoreDeleted">if set to <c>true</c> [ignore deleted].</param>
        /// <returns></returns>
        public static ClassDefinition CloneClass(ClassDefinition cd, bool ignoreDeleted)
        {
            ClassDefinition classDef = null;
            switch (cd.ClassType)
            {
                case ClassType.ClassType_Class:
                    {
                        Class c = new Class(cd.Name, cd.Description);
                        CopyProperties(cd.Properties, c.Properties, ignoreDeleted);
                        CopyIdentityProperties(cd.IdentityProperties, c.IdentityProperties, ignoreDeleted);
                        CopyElementAttributes(cd.Attributes, c.Attributes);
                        CopyUniqueConstraints(cd.UniqueConstraints, c);
                        classDef = c;
                    }
                    break;
                case ClassType.ClassType_FeatureClass:
                    {
                        FeatureClass sfc = (FeatureClass)cd;
                        FeatureClass fc = new FeatureClass(cd.Name, cd.Description);
                        CopyProperties(cd.Properties, fc.Properties, ignoreDeleted);
                        CopyIdentityProperties(cd.IdentityProperties, fc.IdentityProperties, ignoreDeleted);
                        if (sfc.GeometryProperty != null)
                        {
                            string geomName = sfc.GeometryProperty.Name;
                            fc.GeometryProperty = fc.Properties[fc.Properties.IndexOf(geomName)] as GeometricPropertyDefinition;
                        }
                        CopyElementAttributes(cd.Attributes, fc.Attributes);
                        CopyUniqueConstraints(cd.UniqueConstraints, fc);
                        classDef = fc;
                    }
                    break;
                default:
                    throw new UnsupportedException(Res.GetStringFormatted("ERR_UNSUPPORTED_CLONE_CLASS_TYPE", cd.ClassType));
            }
            return classDef;
        }

        /// <summary>
        /// Utility method to copy all unique constraints of a class
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void CopyUniqueConstraints(UniqueConstraintCollection source, ClassDefinition target)
        {
            if (source.Count == 0)
                return;

            var props = target.Properties;
            foreach (UniqueConstraint uniq in source)
            {
                var ucs = new UniqueConstraint();
                foreach (DataPropertyDefinition dp in uniq.Properties)
                {
                    if (props.Contains(dp.Name))
                    {
                        var prop = props[dp.Name];
                        if (prop.PropertyType == PropertyType.PropertyType_DataProperty)
                            ucs.Properties.Add((DataPropertyDefinition)prop);
                    }
                }
                target.UniqueConstraints.Add(ucs);
            }
        }

        /// <summary>
        /// Utility method to copy a schema attribute dictionary
        /// </summary>
        /// <param name="srcAttributes"></param>
        /// <param name="targetAttributes"></param>
        private static void CopyElementAttributes(SchemaAttributeDictionary srcAttributes, SchemaAttributeDictionary targetAttributes)
        {
            foreach (string attr in srcAttributes.AttributeNames)
            {
                targetAttributes.SetAttributeValue(attr, srcAttributes.GetAttributeValue(attr));
            }
        }

        /// <summary>
        /// Utility method to copy a class's identity properties
        /// </summary>
        /// <param name="srcProperties"></param>
        /// <param name="targetProperties"></param>
        private static void CopyIdentityProperties(DataPropertyDefinitionCollection srcProperties, DataPropertyDefinitionCollection targetProperties, bool ignoreDeleted)
        {
            if (ignoreDeleted)
            {
                foreach (PropertyDefinition propDef in srcProperties)
                {
                    if (propDef.ElementState != SchemaElementState.SchemaElementState_Deleted)
                        targetProperties.Add(CloneProperty(propDef) as DataPropertyDefinition);
                }
            }
            else
            {
                foreach (PropertyDefinition propDef in srcProperties)
                {
                    targetProperties.Add(CloneProperty(propDef) as DataPropertyDefinition);
                }
            }
        }

        /// <summary>
        /// Utility method to copy a property definition collection
        /// </summary>
        /// <param name="srcProperties"></param>
        /// <param name="targetProperties"></param>
        private static void CopyProperties(PropertyDefinitionCollection srcProperties, PropertyDefinitionCollection targetProperties, bool ignoreDeleted)
        {
            if (ignoreDeleted)
            {
                foreach (PropertyDefinition propDef in srcProperties)
                {
                    if (propDef.ElementState != SchemaElementState.SchemaElementState_Deleted)
                        targetProperties.Add(CloneProperty(propDef));
                }
            }
            else
            {
                foreach (PropertyDefinition propDef in srcProperties)
                {
                    targetProperties.Add(CloneProperty(propDef));
                }
            }
        }

        /// <summary>
        /// Clones a given property definition
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public static PropertyDefinition CloneProperty(PropertyDefinition pd)
        {
            PropertyDefinition propDef = null;
            switch (pd.PropertyType)
            {
                case PropertyType.PropertyType_DataProperty:
                    {
                        DataPropertyDefinition srcData = pd as DataPropertyDefinition;
                        DataPropertyDefinition dataDef = new DataPropertyDefinition(srcData.Name, srcData.Description)
                        {
                            DataType = srcData.DataType,
                            DefaultValue = srcData.DefaultValue,
                            IsAutoGenerated = srcData.IsAutoGenerated,
                            IsSystem = srcData.IsSystem,
                            Length = srcData.Length,
                            Nullable = srcData.Nullable,
                            Precision = srcData.Precision,
                            ReadOnly = srcData.ReadOnly
                        };
                        //Copy constraints
                        if (srcData.ValueConstraint != null)
                        {
                            if (srcData.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_Range)
                            {
                                PropertyValueConstraintRange range = (PropertyValueConstraintRange)srcData.ValueConstraint;
                                PropertyValueConstraintRange constraint = new PropertyValueConstraintRange(range.MinValue, range.MaxValue)
                                {
                                    MaxInclusive = range.MaxInclusive,
                                    MinInclusive = range.MinInclusive
                                };
                                dataDef.ValueConstraint = constraint;
                            }
                            else if (srcData.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_List)
                            {
                                PropertyValueConstraintList list = (PropertyValueConstraintList)srcData.ValueConstraint;
                                PropertyValueConstraintList constraint = new PropertyValueConstraintList();
                                foreach (DataValue dval in list.ConstraintList)
                                {
                                    constraint.ConstraintList.Add(dval);
                                }
                                dataDef.ValueConstraint = constraint;
                            }
                        }
                        CopyPropertyAttributes(srcData, dataDef);
                        propDef = dataDef;
                    }
                    break;
                case PropertyType.PropertyType_GeometricProperty:
                    {
                        GeometricPropertyDefinition srcData = pd as GeometricPropertyDefinition;
                        GeometricPropertyDefinition geomDef = new GeometricPropertyDefinition(srcData.Name, srcData.Description)
                        {
                            GeometryTypes = srcData.GeometryTypes,
                            HasElevation = srcData.HasElevation,
                            HasMeasure = srcData.HasMeasure,
                            IsSystem = srcData.IsSystem,
                            ReadOnly = srcData.ReadOnly,
                            SpatialContextAssociation = srcData.SpatialContextAssociation
                        };
                        CopyPropertyAttributes(srcData, geomDef);
                        propDef = geomDef;
                    }
                    break;
                case PropertyType.PropertyType_RasterProperty:
                    {
                        RasterPropertyDefinition srcData = pd as RasterPropertyDefinition;
                        RasterPropertyDefinition rastDef = new RasterPropertyDefinition(srcData.Name, srcData.Description);
                        if (srcData.DefaultDataModel != null)
                        {
                            rastDef.DefaultDataModel = new OSGeo.FDO.Raster.RasterDataModel();
                            rastDef.DefaultDataModel.BitsPerPixel = srcData.DefaultDataModel.BitsPerPixel;
                            rastDef.DefaultDataModel.DataModelType = srcData.DefaultDataModel.DataModelType;
                            rastDef.DefaultDataModel.DataType = srcData.DefaultDataModel.DataType;
                            rastDef.DefaultDataModel.Organization = srcData.DefaultDataModel.Organization;
                            rastDef.DefaultDataModel.TileSizeX = srcData.DefaultDataModel.TileSizeX;
                            rastDef.DefaultDataModel.TileSizeY = srcData.DefaultDataModel.TileSizeY;
                        }
                        rastDef.DefaultImageXSize = srcData.DefaultImageXSize;
                        rastDef.DefaultImageYSize = srcData.DefaultImageYSize;
                        rastDef.IsSystem = srcData.IsSystem;
                        rastDef.Nullable = srcData.Nullable;
                        rastDef.ReadOnly = srcData.ReadOnly;
                        rastDef.SpatialContextAssociation = srcData.SpatialContextAssociation;
                        CopyPropertyAttributes(srcData, rastDef);
                        propDef = rastDef;
                    }
                    break;
                case PropertyType.PropertyType_AssociationProperty:
                    throw new UnsupportedException(Res.GetString("ERR_UNSUPPORTED_CLONE_ASSOCIATION"));
                case PropertyType.PropertyType_ObjectProperty:
                    throw new UnsupportedException(Res.GetString("ERR_UNSUPPORTED_CLONE_OBJECT"));

            }
            return propDef;
        }

        private static void CopyPropertyAttributes(PropertyDefinition srcData, PropertyDefinition target)
        {
            //Copy attributes
            if (srcData.Attributes != null)
            {
                foreach (string attr in srcData.Attributes.AttributeNames)
                {
                    target.Attributes.SetAttributeValue(attr, srcData.Attributes.GetAttributeValue(attr));
                }
            }
        }
    }
}
