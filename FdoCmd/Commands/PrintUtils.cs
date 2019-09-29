#region LGPL Header
// Copyright (C) 2019, Jackie Ng
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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Common;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FdoCmd.Commands
{
    public static class PrintUtils
    {
        public static void WritePropertyDict(BaseCommand cmd, IConnectionPropertyDictionary dict)
        {
            foreach (string name in dict.PropertyNames)
            {
                cmd.WriteLine("-> {0}", name);
                using (cmd.Indent())
                {
                    cmd.WriteLine("Localized Name: {0}", dict.GetLocalizedName(name));
                    cmd.WriteLine("Required: {0}", dict.IsPropertyRequired(name));
                    cmd.WriteLine("Protected: {0}", dict.IsPropertyProtected(name));
                    cmd.WriteLine("Enumerable: {0}", dict.IsPropertyEnumerable(name));
                    if (dict.IsPropertyEnumerable(name))
                    {
                        cmd.WriteLine("Values for property:");
                        using (cmd.Indent())
                        {
                            try
                            {
                                string[] values = dict.EnumeratePropertyValues(name);
                                foreach (string str in values)
                                {
                                    cmd.WriteLine("-> {0}", str);
                                }
                            }
                            catch (OSGeo.FDO.Common.Exception)
                            {
                                cmd.WriteError("Property values not available");
                            }
                        }
                    }
                }
            }
        }

        internal static void WriteSpatialContexts(BaseCommand cmd, ReadOnlyCollection<SpatialContextInfo> contexts)
        {
            cmd.WriteLine("Spatial Contexts in connection: {0}", contexts.Count);
            foreach (var ctx in contexts)
            {
                cmd.WriteLine("-> {0}", ctx.Name);
                using (cmd.Indent())
                {
                    cmd.WriteLine("Descriptionn: {0}", ctx.Description);
                    cmd.WriteLine("XY Tolerance: {0}", ctx.XYTolerance);
                    cmd.WriteLine("Z Tolerance: {0}", ctx.ZTolerance);
                    cmd.WriteLine("Coordinate System: {0}", ctx.CoordinateSystem);
                    cmd.WriteLine("Coordinate System WKT:");
                    if (!string.IsNullOrEmpty(ctx.CoordinateSystemWkt))
                    {
                        using (cmd.Indent())
                        {
                            cmd.WriteLine(ctx.CoordinateSystemWkt);
                        }
                    }
                    cmd.WriteLine("Extent Type: {0}", ctx.ExtentType);
                    if (ctx.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Static)
                    {
                        using (cmd.Indent())
                        {
                            cmd.WriteLine("Extent:");
                            using (cmd.Indent())
                            {
                                cmd.WriteLine(ctx.ExtentGeometryText);
                            }
                        }
                    }
                }
            }
        }

        internal static void WriteSchemaNames(BaseCommand cmd, ICollection<string> schemas)
        {
            cmd.WriteLine("Schemas in this connection: {0}", schemas.Count);
            foreach (var name in schemas)
            {
                cmd.WriteLine("-> {0}", name);
            }
        }

        internal static void WriteAttributes(BaseCommand cmd, SchemaAttributeDictionary schemaAttributeDictionary)
        {
            if (schemaAttributeDictionary.AttributeNames.Length > 0)
            {
                using (cmd.Indent())
                {
                    foreach (string name in schemaAttributeDictionary.AttributeNames)
                    {
                        cmd.WriteLine("-> {0}: {1}", name, schemaAttributeDictionary.GetAttributeValue(name));
                    }
                }
            }
        }

        internal static void WriteClassNames(BaseCommand cmd, string schemaName, ICollection<string> classNames)
        {
            cmd.WriteLine("Classes in schema ({0}): {1}", schemaName, classNames.Count);
            foreach (var name in classNames)
            {
                cmd.WriteLine("-> {0}", name);
            }
        }

        internal static void WriteClasses(BaseCommand cmd, FeatureSchema fs)
        {
            cmd.WriteLine("Classes in schema ({0}): {1}", fs.Name, fs.Classes.Count);
            foreach (ClassDefinition cd in fs.Classes)
            {
                cmd.WriteLine("-> {0} ({1})", cd.Name, cd.ClassType);
                using (cmd.Indent())
                {
                    cmd.WriteLine("Qualified Name: {0}", cd.QualifiedName);
                    cmd.WriteLine("Description: {0}", cd.Description);
                    cmd.WriteLine("Is Abstract: {0}", cd.IsAbstract);
                    cmd.WriteLine("Is Computed: {0}", cd.IsComputed);
                    if (cd.BaseClass != null)
                        cmd.WriteLine("Base Class: {0}", cd.BaseClass.Name);
                    cmd.WriteLine("Attributes:");
                    using (cmd.Indent())
                    {
                        WriteAttributes(cmd, cd.Attributes);
                    }
                    cmd.WriteLine("Properties:");
                    using (cmd.Indent())
                    {
                        WriteClassProperties(cmd, cd);
                    }
                    cmd.WriteLine("");
                }
            }
        }

        static void WriteRasterProperty(BaseCommand cmd, RasterPropertyDefinition raster)
        {
            cmd.WriteLine("Description: {0}", raster.Description);
            cmd.WriteLine("Nullable: {0}", raster.Nullable);
            cmd.WriteLine("Read-Only: {0}", raster.ReadOnly);
            cmd.WriteLine("Spatial Context Association: {0}", raster.SpatialContextAssociation);
            cmd.WriteLine("Default Image Size (X): {0}", raster.DefaultImageXSize);
            cmd.WriteLine("Default Image Size (Y): {0}", raster.DefaultImageYSize);
            if (raster.DefaultDataModel != null)
            {
                cmd.WriteLine("Raster Data Model:");
                using (cmd.Indent())
                {
                    cmd.WriteLine("Bits per-pixel: {0}", raster.DefaultDataModel.BitsPerPixel);
                    cmd.WriteLine("Default Tile Size (X): {0}", raster.DefaultDataModel.TileSizeX);
                    cmd.WriteLine("Default Tile Size (Y): {0}", raster.DefaultDataModel.TileSizeY);
                    cmd.WriteLine("Data Model Type: {0}", raster.DefaultDataModel.DataModelType);
                    cmd.WriteLine("Data Type: {0}", raster.DefaultDataModel.DataType);
                    cmd.WriteLine("Organization: {0}", raster.DefaultDataModel.Organization);
                }
            }
        }

        static void WriteObjectProperty(BaseCommand cmd, ObjectPropertyDefinition obj)
        {
            cmd.WriteLine("Object Type: {0}", obj.ObjectType);
            cmd.WriteLine("Order Type: {0}", obj.OrderType);
            if (obj.IdentityProperty != null)
                cmd.WriteLine("Identity Property: {0}", obj.IdentityProperty.Name);
            if (obj.Class != null)
                cmd.WriteLine("Class: {0}", obj.Class.Name);
        }

        static void WriteAssociationProperty(BaseCommand cmd, AssociationPropertyDefinition assoc)
        {
            if (assoc.AssociatedClass != null)
                cmd.WriteLine("Associated Class: {0}", assoc.AssociatedClass.Name);
            if (assoc.IdentityProperties != null)
            {
                cmd.WriteLine("Identity Properties:");
                using (cmd.Indent())
                {
                    foreach (DataPropertyDefinition data in assoc.IdentityProperties)
                    {
                        cmd.WriteLine("-> {0}", data.Name);
                    }
                }
            }
            cmd.WriteLine("Delete Rule: {0}", assoc.DeleteRule);
            cmd.WriteLine("Read-Only: {0}", assoc.IsReadOnly);
            cmd.WriteLine("Lock Cascade: {0}", assoc.LockCascade);
            cmd.WriteLine("Multiplicity: {0}", assoc.Multiplicity);
            cmd.WriteLine("Property Type: {0}", assoc.PropertyType);
            if (assoc.ReverseIdentityProperties != null)
            {
                cmd.WriteLine("Reverse Identity Properties:");
                using (cmd.Indent())
                {
                    foreach (DataPropertyDefinition data in assoc.ReverseIdentityProperties)
                    {
                        cmd.WriteLine("-> {0}", data.Name);
                    }
                }
            }
            cmd.WriteLine("Reverse Multiplicity: {0}", assoc.ReverseMultiplicity);
            cmd.WriteLine("Reverse Name: {0}", assoc.ReverseName);
        }

        static void WriteGeometricProperty(BaseCommand cmd, GeometricPropertyDefinition geom)
        {
            cmd.WriteLine("Has Elevation: {0}", geom.HasElevation);
            cmd.WriteLine("Has Measure: {0}", geom.HasMeasure);
            cmd.WriteLine("Read-Only: {0}", geom.ReadOnly);
            cmd.WriteLine("Geometry Types:");
            using (cmd.Indent())
            {
                cmd.WriteLines(GetGeometryTypes(geom.GeometryTypes).Select(s => $"-> {s}"));
            }
            cmd.WriteLine("Spatial Context Association: {0}", geom.SpatialContextAssociation);
        }

        static ICollection<string> GetGeometryTypes(int geometryTypes)
        {
            List<string> geomTypes = new List<string>();
            if (geometryTypes != (int)GeometryType.GeometryType_None)
            {
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_CurvePolygon);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_CurveString);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_LineString);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_MultiCurvePolygon);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_MultiCurveString);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_MultiGeometry);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_MultiLineString);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_MultiPoint);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_MultiPolygon);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_Point);
                CheckGeometryType(geometryTypes, geomTypes, GeometryType.GeometryType_Polygon);
            }
            else
            {
                geomTypes.Add(GeometryType.GeometryType_None.ToString());
            }
            return geomTypes;
        }

        private static void CheckGeometryType(int geometryTypes, List<string> geomTypes, GeometryType gtype)
        {
            if ((geometryTypes & (int)gtype) == (int)gtype)
                geomTypes.Add(gtype.ToString());
        }

        static void WriteDataProperty(BaseCommand cmd, DataPropertyDefinition data)
        {
            cmd.WriteLine("Data Type: {0}", data.DataType);
            cmd.WriteLine("Nullable: {0}", data.Nullable);
            cmd.WriteLine("Auto-Generated: {0}", data.IsAutoGenerated);
            cmd.WriteLine("Read-Only: {0}", data.ReadOnly);
            cmd.WriteLine("Length: {0}", data.Length);
            cmd.WriteLine("Precision: {0}", data.Precision);
            cmd.WriteLine("Scale: {0}", data.Scale);
        }

        internal static void WriteClassProperties(BaseCommand cmd, ClassDefinition cd)
        {
            foreach (PropertyDefinition propDef in cd.Properties)
            {
                cmd.WriteLine("-> {0}", propDef.Name);
                using (cmd.Indent())
                {
                    cmd.WriteLine("Type: {0}", propDef.PropertyType);
                    bool isIdentity = (propDef.PropertyType == PropertyType.PropertyType_DataProperty && cd.IdentityProperties.Contains((DataPropertyDefinition)propDef));
                    cmd.WriteLine("Is Identity: {0}", isIdentity);
                    cmd.WriteLine("Qualified Name: {0}", propDef.QualifiedName);
                    cmd.WriteLine("Is System: {0}", propDef.IsSystem);
                    switch (propDef.PropertyType)
                    {
                        case PropertyType.PropertyType_DataProperty:
                            WriteDataProperty(cmd, propDef as DataPropertyDefinition);
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            WriteGeometricProperty(cmd, propDef as GeometricPropertyDefinition);
                            break;
                        case PropertyType.PropertyType_AssociationProperty:
                            WriteAssociationProperty(cmd, propDef as AssociationPropertyDefinition);
                            break;
                        case PropertyType.PropertyType_ObjectProperty:
                            WriteObjectProperty(cmd, propDef as ObjectPropertyDefinition);
                            break;
                        case PropertyType.PropertyType_RasterProperty:
                            WriteRasterProperty(cmd, propDef as RasterPropertyDefinition);
                            break;
                    }
                }
            }
        }
    }
}
