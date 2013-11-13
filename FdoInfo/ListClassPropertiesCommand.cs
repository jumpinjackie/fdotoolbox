#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Common;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoInfo
{
    public class ListClassPropertiesCommand : ConsoleCommand
    {
        private string _schema;
        private string _class;
        private string _provider;
        private string _connstr;

        public ListClassPropertiesCommand(string provider, string connStr, string schema, string className)
        {
            _schema = schema;
            _class = className;
            _provider = provider;
            _connstr = connStr;
        }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = CreateConnection(_provider, _connstr);
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }

            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                ClassDefinition cd = service.GetClassByName(_schema, _class);
                if (cd == null)
                {
                    Console.Error.WriteLine("Class {0} not found in schema {1}", _class, _schema);
                    return (int)CommandStatus.E_FAIL_CLASS_NOT_FOUND;
                }
                else
                {
                    using (cd)
                    {
                        foreach (PropertyDefinition propDef in cd.Properties)
                        {
                            bool isIdentity = (propDef.PropertyType == PropertyType.PropertyType_DataProperty && cd.IdentityProperties.Contains((DataPropertyDefinition)propDef));
                            Console.WriteLine("\nName: {0} ({1}) {2}\n", propDef.Name, propDef.PropertyType, isIdentity ? "(IDENTITY)" : "");
                            Console.WriteLine("\tQualified Name: {0}\n\tIs System: {1}", propDef.QualifiedName, propDef.IsSystem);
                            switch (propDef.PropertyType)
                            {
                                case PropertyType.PropertyType_DataProperty:
                                    WriteDataProperty(propDef as DataPropertyDefinition);
                                    break;
                                case PropertyType.PropertyType_GeometricProperty:
                                    WriteGeometricProperty(propDef as GeometricPropertyDefinition);
                                    break;
                                case PropertyType.PropertyType_AssociationProperty:
                                    WriteAssociationProperty(propDef as AssociationPropertyDefinition);
                                    break;
                                case PropertyType.PropertyType_ObjectProperty:
                                    WriteObjectProperty(propDef as ObjectPropertyDefinition);
                                    break;
                                case PropertyType.PropertyType_RasterProperty:
                                    WriteRasterProperty(propDef as RasterPropertyDefinition);
                                    break;
                            }
                        }
                    }
                }
            }

            conn.Close();
            return (int)CommandStatus.E_OK;
        }

        private void WriteRasterProperty(RasterPropertyDefinition raster)
        {
            Console.WriteLine("\tDescription: {0}", raster.Description);
            Console.WriteLine("\tNullable: {0}\n\tRead-Only: {1}", raster.Nullable, raster.ReadOnly);
            Console.WriteLine("\tSpatial Context Association: {0}", raster.SpatialContextAssociation);
            Console.WriteLine("\tDefault Image Size (X): {0}\n\tDefault Image Size (Y): {1}", raster.DefaultImageXSize, raster.DefaultImageYSize);
            if (raster.DefaultDataModel != null)
            {
                Console.WriteLine("\tRaster Data Model:");
                Console.WriteLine("\t\tBits per-pixel: {0}", raster.DefaultDataModel.BitsPerPixel);
                Console.WriteLine("\t\tDefault Tile Size (X): {0}\n\t\tDefault Tile Size (Y): {1}",
                    raster.DefaultDataModel.TileSizeX,
                    raster.DefaultDataModel.TileSizeY);
                Console.WriteLine("\t\tData Model Type: {0}", raster.DefaultDataModel.DataModelType);
                Console.WriteLine("\t\tData Type: {0}", raster.DefaultDataModel.DataType);
                Console.WriteLine("\t\tOrganization: {0}", raster.DefaultDataModel.Organization);
            }
        }

        private void WriteObjectProperty(ObjectPropertyDefinition obj)
        {
            Console.WriteLine("\tObject Type: {0}\n\tOrder Type: {1}", obj.ObjectType, obj.OrderType);
            if (obj.IdentityProperty != null)
                Console.WriteLine("\tIdentity Property: {0}", obj.IdentityProperty.Name);
            if (obj.Class != null)
                Console.WriteLine("\tClass: {0}", obj.Class.Name);
        }

        private void WriteAssociationProperty(AssociationPropertyDefinition assoc)
        {
            if(assoc.AssociatedClass != null)
                Console.WriteLine("\tAssociated Class: {0}", assoc.AssociatedClass.Name);
            if (assoc.IdentityProperties != null)
            {
                Console.WriteLine("\tIdentity Properties:");
                foreach (DataPropertyDefinition data in assoc.IdentityProperties)
                {
                    Console.WriteLine("\t\t- {0}", data.Name);
                }
            }
            Console.WriteLine("\tDelete Rule: {0}", assoc.DeleteRule);
            Console.WriteLine("\tRead-Only: {0}\n\tLock Cascade: {1}", assoc.IsReadOnly, assoc.LockCascade);
            Console.WriteLine("\tMultiplicity: {0}", assoc.Multiplicity);
            Console.WriteLine("\tProperty Type: {0}", assoc.PropertyType);
            if (assoc.ReverseIdentityProperties != null)
            {
                Console.WriteLine("\tReverse Identity Properties:");
                foreach (DataPropertyDefinition data in assoc.ReverseIdentityProperties)
                {
                    Console.WriteLine("\t\t- {0}", data.Name);
                }
            }
            Console.WriteLine("\tReverse Multiplicity: {0}\n\tReverse Name: {1}", assoc.ReverseMultiplicity, assoc.ReverseName);
        }

        private void WriteGeometricProperty(GeometricPropertyDefinition geom)
        {
            Console.WriteLine("\tHas Elevation: {0}\n\tHas Measure: {1}\n\tRead-Only: {2}", geom.HasElevation, geom.HasMeasure, geom.ReadOnly);
            Console.WriteLine("\tGeometry Types: \n\t\t- {0}", GetGeometryTypes(geom.GeometryTypes));
            Console.WriteLine("\tSpatial Context Association: {0}", geom.SpatialContextAssociation);
        }

        private string GetGeometryTypes(int geometryTypes)
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
            return string.Join("\n\t\t- ", geomTypes.ToArray());
        }

        private static void CheckGeometryType(int geometryTypes, List<string> geomTypes, GeometryType gtype)
        {
            if ((geometryTypes & (int)gtype) == (int)gtype)
                geomTypes.Add(gtype.ToString());
        }

        private void WriteDataProperty(DataPropertyDefinition data)
        {
            Console.WriteLine("\tData Type: {0}", data.DataType);
            Console.WriteLine("\tNullable: {0}\n\tAuto-Generated: {1}\n\tRead-Only: {2}", data.Nullable, data.IsAutoGenerated, data.ReadOnly);
            Console.WriteLine("\tLength: {0}\n\tPrecision: {1}\n\tScale: {2}", data.Length, data.Precision, data.Scale);
        }
    }
}
