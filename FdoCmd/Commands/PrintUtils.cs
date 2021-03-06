﻿#region LGPL Header
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
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Common;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;
using OSGeo.MapGuide;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime;
using System.Text;

namespace FdoCmd.Commands
{
    public static class PrintUtils
    {
        public static IGeometry CreateExtentGeom(FgfGeometryFactory geomFactory, double minX, double minY, double maxX, double maxY)
        {
            var wkt = SpatialContextInfo.GetEnvelopeWkt(minX, minY, maxX, maxY);
            var env = geomFactory.CreateGeometry(wkt);
            return env;
        }

        static string Stringify(MgProperty prop)
        {
            switch (prop.PropertyType)
            {
                case MgPropertyType.Boolean:
                    return ((MgBooleanProperty)prop).Value.ToString();
                case MgPropertyType.Byte:
                    return ((MgByteProperty)prop).Value.ToString();
                case MgPropertyType.DateTime:
                    return ((MgDateTimeProperty)prop).Value.ToString();
                case MgPropertyType.Decimal:
                case MgPropertyType.Double:
                    return ((MgDoubleProperty)prop).Value.ToString();
                case MgPropertyType.Int16:
                    return ((MgInt16Property)prop).Value.ToString();
                case MgPropertyType.Int32:
                    return ((MgInt32Property)prop).Value.ToString();
                case MgPropertyType.Int64:
                    return ((MgInt64Property)prop).Value.ToString();
                case MgPropertyType.Single:
                    return ((MgSingleProperty)prop).Value.ToString();
                case MgPropertyType.String:
                    return ((MgStringProperty)prop).Value;
                default:
                    return string.Empty;
            }
        }

        internal static void WriteCoordSysEntriesAsCsv(BaseCommand cmd, IEnumerable<ICoordinateSystem> coordSystems)
        {
            if (coordSystems is null)
            {
                throw new ArgumentNullException(nameof(coordSystems));
            }

            var headers = new List<string>();
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Code)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Datum)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.DatumDescription)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Description)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Ellipsoid)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.EllipsoidDescription)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.EPSG)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Projection)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.ProjectionDescription)));
            //headers.Add(QuoteVal(nameof(ICoordinateSystem.WKT)));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Bounds) + "_MinX"));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Bounds) + "_MinY"));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Bounds) + "_MaxX"));
            headers.Add(QuoteVal(nameof(ICoordinateSystem.Bounds) + "_MaxY"));
            cmd.WriteLine(string.Join(",", headers));

            var values = new List<string>();
            foreach (var cs in coordSystems)
            {
                values.Clear();
                values.Add(QuoteVal(cs.Code));
                values.Add(QuoteVal(cs.Datum));
                values.Add(QuoteVal(cs.DatumDescription));
                values.Add(QuoteVal(cs.Description));
                values.Add(QuoteVal(cs.Ellipsoid));
                values.Add(QuoteVal(cs.EllipsoidDescription));
                values.Add(QuoteVal(cs.EPSG));
                values.Add(QuoteVal(cs.Projection));
                values.Add(QuoteVal(cs.ProjectionDescription));
                //values.Add(QuoteVal(EscapeQuotes(cs.WKT)));
                if (cs.Bounds == null) 
                {
                    values.Add(QuoteVal(string.Empty));
                    values.Add(QuoteVal(string.Empty));
                    values.Add(QuoteVal(string.Empty));
                    values.Add(QuoteVal(string.Empty));
                }
                else
                {
                    values.Add(QuoteVal(cs.Bounds.MinX.ToString(CultureInfo.InvariantCulture)));
                    values.Add(QuoteVal(cs.Bounds.MinY.ToString(CultureInfo.InvariantCulture)));
                    values.Add(QuoteVal(cs.Bounds.MaxX.ToString(CultureInfo.InvariantCulture)));
                    values.Add(QuoteVal(cs.Bounds.MaxY.ToString(CultureInfo.InvariantCulture)));
                }
                 
                cmd.WriteLine(string.Join(",", values));
            }

            string QuoteVal(string s) => "\"" + s + "\"";
            string EscapeQuotes(string s) => s.Replace("\"", "\\\"");
        }

        internal static void WriteCoordSysEntries(BaseCommand cmd, IEnumerable<ICoordinateSystem> coordSystems)
        {
            foreach (var cs in coordSystems)
            {
                cmd.WriteLine($"Code: {cs.Code}");
                using (cmd.Indent())
                {
                    cmd.WriteLine($"Datum: {cs.Datum}");
                    cmd.WriteLine($"Datum Description: {cs.DatumDescription}");
                    cmd.WriteLine($"Description: {cs.Description}");
                    cmd.WriteLine($"Ellipsoid: {cs.Ellipsoid}");
                    cmd.WriteLine($"Ellipsoid Description: {cs.EllipsoidDescription}");
                    cmd.WriteLine($"EPSG Code: {cs.EPSG}");
                    cmd.WriteLine($"Projection: {cs.Projection}");
                    cmd.WriteLine($"Projection Description: {cs.ProjectionDescription}");
                    //cmd.WriteLine($"WKT: {cs.WKT}");
                    if (cs.Bounds != null)
                        cmd.WriteLine($"Bounds: [{cs.Bounds.MinX}, {cs.Bounds.MinY}, {cs.Bounds.MaxX}, {cs.Bounds.MaxY}]");
                    else
                        cmd.WriteLine("Bounds: <null>");
                }
            }
        }

        internal static void WriteCoordSysEntry(BaseCommand cmd, MgPropertyCollection cs)
        {
            if (cs.GetCount() > 0)
            {
                var prop = cs.GetItem(0);
                cmd.WriteLine($"{prop.Name}: {Stringify(prop)}");
            }
            using (cmd.Indent())
            {
                for (int i = 1; i < cs.GetCount(); i++)
                {
                    var prop = cs.GetItem(i);
                    cmd.WriteLine($"{prop.Name}: {Stringify(prop)}");
                }
            }
        }

        public static void WritePropertyDict(BaseCommand cmd, IConnectionPropertyDictionary dict)
        {
            foreach (string name in dict.PropertyNames)
            {
                cmd.WriteLine("{0}", name);
                if (cmd is ISummarizableCommand sum && sum.Detailed)
                {
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
                                        cmd.WriteLine(str);
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
        }

        internal static void WriteReaderAsGeoJson(BaseCommand cmd,
                                                  IReader reader,
                                                  Dictionary<string, Func<IReader, string>> dataValueReaders,
                                                  List<string> geomNames)
        {
            using (var geomFactory = new FgfGeometryFactory())
            {
                cmd.WriteLine("{");
                using (cmd.Indent())
                {
                    cmd.WriteLine(@"""type"": ""FeatureCollection"",");
                    cmd.WriteLine(@"""features"": [");
                    using (cmd.Indent())
                    {
                        bool bFirstFeature = true;
                        while (reader.ReadNext())
                        {
                            if (!bFirstFeature)
                            {
                                cmd.WriteLineNoIndent(",");
                            }

                            cmd.WriteLine("{");
                            using (cmd.Indent())
                            {
                                cmd.WriteLine(@"""type"": ""Feature"",");
                                cmd.WriteLine(@"""properties"": {");
                                using (cmd.Indent())
                                {
                                    bool bFirstProperty = true;
                                    foreach (var kvp in dataValueReaders)
                                    {
                                        if (!bFirstProperty)
                                        {
                                            cmd.WriteLineNoIndent(",");
                                        }
                                        cmd.Write($@"""{kvp.Key}"": {(reader.IsNull(kvp.Key) ? "null" : kvp.Value(reader))}");
                                        bFirstProperty = false;
                                    }
                                }
                                if (geomNames.Count == 1)
                                {
                                    cmd.WriteLineNoIndent(string.Empty);
                                    cmd.WriteLine("},");
                                    cmd.Write(@"""geometry"": ");

                                    var fgf = reader.GetGeometry(geomNames[0]);
                                    using (var geom = geomFactory.CreateGeometryFromFgf(fgf))
                                    {
                                        WriteGeoJsonGeometry(cmd, geom, false, NullValue);
                                    }
                                }
                                else
                                {
                                    cmd.WriteLine("}");
                                }
                            }
                            cmd.Write("}");
                            bFirstFeature = false;
                        }
                    }
                    cmd.WriteLine("]");
                }
                cmd.WriteLine("}");
            }
        }

        internal static void WriteFeatureReaderAsGeoJson(BaseCommand cmd, IFeatureReader reader)
        {
            var clsDef = reader.GetClassDefinition();
            var clsProps = clsDef.Properties;
            var idProps = clsDef.IdentityProperties;

            var sb = new StringBuilder(2048);
            var dataValueReaders = new Dictionary<string, Func<IReader, string>>();
            var geomNames = new List<string>();

            foreach (var pd in clsProps)
            {
                if (pd is DataPropertyDefinition dp)
                {
                    string name = dp.Name;
                    switch (dp.DataType)
                    {
                        case DataType.DataType_Boolean:
                            dataValueReaders[name] = rdr => rdr.GetBoolean(name) ? "true" : "false";
                            break;
                        case DataType.DataType_Byte:
                            dataValueReaders[name] = rdr => rdr.GetByte(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_DateTime:
                            dataValueReaders[name] = rdr => QuoteValue(rdr.GetDateTime(name).ToString("o"));
                            break;
                        case DataType.DataType_Decimal:
                        case DataType.DataType_Double:
                            dataValueReaders[name] = rdr => rdr.GetDouble(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int16:
                            dataValueReaders[name] = rdr => rdr.GetInt16(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int32:
                            dataValueReaders[name] = rdr => rdr.GetInt32(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int64:
                            dataValueReaders[name] = rdr => rdr.GetInt64(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Single:
                            dataValueReaders[name] = rdr => rdr.GetSingle(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_String:
                            dataValueReaders[name] = rdr => QuoteValue(rdr.GetString(name).ToString(CultureInfo.InvariantCulture));
                            break;
                        default: //Anything else is not string representable
                            dataValueReaders[name] = rdr => string.Empty;
                            break;
                    }
                }
                else if (pd is GeometricPropertyDefinition gp)
                {
                    geomNames.Add(gp.Name);
                }
            }

            WriteReaderAsGeoJson(cmd, reader, dataValueReaders, geomNames);

            string QuoteValue(string s) => "\"" + s.Replace("\"", "\\\"") + "\"";
        }

        static bool IsWriteableToGeoJson(GeometryType gt)
        {
            switch (gt)
            {
                case GeometryType.GeometryType_Point:
                case GeometryType.GeometryType_LineString:
                case GeometryType.GeometryType_Polygon:
                case GeometryType.GeometryType_MultiPoint:
                case GeometryType.GeometryType_MultiLineString:
                case GeometryType.GeometryType_MultiPolygon:
                case GeometryType.GeometryType_MultiGeometry:
                    return true;
            }
            return false;
        }

        static string NullValue(bool trailing) => trailing ? "null," : "null";

        private static void WriteGeoJsonGeometry(BaseCommand cmd, IGeometry geom, bool trailing, Func<bool, string> unsupportedValue)
        {
            if (!IsWriteableToGeoJson(geom.DerivedType))
            {
                cmd.WriteLine(unsupportedValue(trailing));
            }
            else 
            {
                string type = geom.DerivedType == GeometryType.GeometryType_MultiGeometry
                    ? "GeometryCollection"
                    : geom.DerivedType.ToString().Substring("GeometryType_".Length);

                cmd.WriteLineNoIndent("{");
                using (cmd.Indent())
                {
                    cmd.WriteLine($@"""type"": ""{type}"",");
                    if (geom.DerivedType == GeometryType.GeometryType_MultiGeometry)
                    {
                        var mg = (IMultiGeometry)geom;
                        cmd.Write(@"""geometries"": [");
                        for (int i = 0; i < mg.Count; i++)
                        {
                            var g = mg[i];
                            WriteGeoJsonGeometry(cmd, g, i < mg.Count - 1, trailing => string.Empty);
                        }
                        cmd.Write("]");
                    }
                    cmd.Write(@"""coordinates"": ");
                    WriteGeoJsonGeometryCoordinates(cmd, geom);
                }
                if (trailing)
                    cmd.WriteLine("},");
                else
                    cmd.WriteLine("}");
            }
        }

        private static void WriteGeoJsonGeometryCoordinates(BaseCommand cmd, IGeometry geom)
        {
            switch (geom.DerivedType)
            {
                case GeometryType.GeometryType_Point:
                    WritePointCoords(cmd, (IPoint)geom, false);
                    break;
                case GeometryType.GeometryType_LineString:
                    WriteLineStringCoords(cmd, (ILineString)geom, false);
                    break;
                case GeometryType.GeometryType_Polygon:
                    WritePolygonCoords(cmd, (IPolygon)geom, false);
                    break;
                case GeometryType.GeometryType_MultiPoint:
                    WriteMultiPointCoords(cmd, (IMultiPoint)geom, false);
                    break;
                case GeometryType.GeometryType_MultiLineString:
                    WriteMultiLineStringCoords(cmd, (IMultiLineString)geom, false);
                    break;
            }
        }

        private static void WriteMultiPolygonCoords(BaseCommand cmd, IMultiPolygon geom, bool trailing)
        {
            cmd.WriteLineNoIndent("[");
            using (cmd.Indent())
            {
                for (int i = 0; i < geom.Count; i++)
                {
                    var poly = geom[i];
                    WritePolygonCoords(cmd, poly, i < geom.Count - 1);
                }
            }
            if (trailing)
                cmd.WriteLine("],");
            else
                cmd.WriteLine("]");
        }

        private static void WriteMultiLineStringCoords(BaseCommand cmd, IMultiLineString geom, bool trailing)
        {
            cmd.WriteLineNoIndent("[");
            using (cmd.Indent())
            {
                for (int i = 0; i < geom.Count; i++)
                {
                    var lstr = geom[i];
                    var pos = lstr.Positions;
                    WritePositionCollection(cmd, pos, i < geom.Count - 1);
                }
            }
            if (trailing)
                cmd.WriteLine("],");
            else
                cmd.WriteLine("]");
        }

        private static void WriteMultiPointCoords(BaseCommand cmd, IMultiPoint geom, bool trailing)
        {
            cmd.WriteLineNoIndent("[");
            using (cmd.Indent())
            {
                for (int i = 0; i < geom.Count; i++)
                {
                    var pt = geom[i];
                    var pos = pt.Position;
                    WritePosition(cmd, pos, i < geom.Count - 1);
                }
            }
            if (trailing)
                cmd.WriteLine("],");
            else
                cmd.WriteLine("]");
        }

        private static void WritePolygonCoords(BaseCommand cmd, IPolygon geom, bool trailing)
        {
            cmd.WriteLineNoIndent("[");
            using (cmd.Indent())
            {
                var extRing = geom.ExteriorRing;
                var extRingPos = extRing.Positions;
                WritePositionCollection(cmd, extRingPos, geom.InteriorRingCount > 0);

                if (geom.InteriorRingCount > 0)
                {
                    for (int i = 0; i < geom.InteriorRingCount; i++)
                    {
                        var ring = geom.GetInteriorRing(i);
                        var ringPos = ring.Positions;
                        WritePositionCollection(cmd, extRingPos, i < geom.InteriorRingCount - 1);
                    }
                }
            }
            if (trailing)
                cmd.WriteLine("],");
            else
                cmd.WriteLine("]");
        }

        private static void WritePositionCollection(BaseCommand cmd, DirectPositionCollection positions, bool trailing)
        {
            cmd.WriteLine("[");
            using (cmd.Indent())
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    var pos = positions[i];
                    WritePosition(cmd, pos, i < positions.Count - 1);
                }
            }
            if (trailing)
                cmd.WriteLine("],");
            else
                cmd.WriteLine("]");
        }

        private static void WriteLineStringCoords(BaseCommand cmd, ILineString geom, bool trailing)
        {
            var positions = geom.Positions;
            WritePositionCollection(cmd, positions, trailing);
        }

        private static void WritePosition(BaseCommand cmd, IDirectPosition pos, bool trailing)
        {
            cmd.WriteLine("[");
            using (cmd.Indent())
            {
                cmd.WriteLine(pos.X.ToString(CultureInfo.InvariantCulture) + ",");
                cmd.WriteLine(pos.Y.ToString(CultureInfo.InvariantCulture));
            }
            if (trailing)
                cmd.WriteLine("],");
            else
                cmd.WriteLine("]");
        }

        private static void WritePointCoords(BaseCommand cmd, IPoint geom, bool trailing)
        {
            var pos = geom.Position;
            WritePosition(cmd, pos, trailing);
        }

        internal static void WriteReaderAsCsv(BaseCommand cmd,
                                              IReader reader,
                                              Dictionary<string, Func<IReader, string>> dataValueReaders,
                                              ICollection<string> geomNames)
        {
            bool quote = true;
            const string DELIM = ",";

            using (var geomFactory = new FgfGeometryFactory())
            {
                //Write CSV header
                var row = new List<string>();
                foreach (var key in dataValueReaders.Keys)
                {
                    row.Add(WrapValue(key));
                }
                foreach (var g in geomNames)
                {
                    row.Add(WrapValue(g));
                }

                cmd.WriteLine(string.Join(DELIM, row));

                //Now for the actual CSV content
                while (reader.ReadNext())
                {
                    row.Clear();
                    foreach (var key in dataValueReaders.Keys)
                    {
                        if (reader.IsNull(key))
                            row.Add(string.Empty);
                        else
                            row.Add(WrapValue(dataValueReaders[key](reader)));
                    }
                    foreach (var g in geomNames)
                    {
                        if (reader.IsNull(g))
                        {
                            row.Add(string.Empty);
                        }
                        else
                        {
                            var fgf = reader.GetGeometry(g);
                            using (var geom = geomFactory.CreateGeometryFromFgf(fgf))
                            {
                                row.Add(WrapValue(geom.Text));
                            }
                        }
                    }
                    cmd.WriteLine(string.Join(DELIM, row));
                }
            }

            string WrapValue(string s) => quote ? $"\"{s}\"" : s;
        }

        internal static void WriteFeatureReaderAsCsv(BaseCommand cmd, IFeatureReader reader)
        {
            var clsDef = reader.GetClassDefinition();
            var clsProps = clsDef.Properties;
            var dataValueReaders = new Dictionary<string, Func<IReader, string>>();
            var geomNames = new HashSet<string>();

            foreach (var pd in clsProps)
            {
                if (pd is DataPropertyDefinition dp)
                {
                    string name = dp.Name;
                    switch (dp.DataType)
                    {
                        case DataType.DataType_Boolean:
                            dataValueReaders[name] = rdr => rdr.GetBoolean(name) ? "true" : "false";
                            break;
                        case DataType.DataType_Byte:
                            dataValueReaders[name] = rdr => rdr.GetByte(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_DateTime:
                            dataValueReaders[name] = rdr => rdr.GetDateTime(name).ToString("o");
                            break;
                        case DataType.DataType_Decimal:
                        case DataType.DataType_Double:
                            dataValueReaders[name] = rdr => rdr.GetDouble(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int16:
                            dataValueReaders[name] = rdr => rdr.GetInt16(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int32:
                            dataValueReaders[name] = rdr => rdr.GetInt32(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int64:
                            dataValueReaders[name] = rdr => rdr.GetInt64(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Single:
                            dataValueReaders[name] = rdr => rdr.GetSingle(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_String:
                            dataValueReaders[name] = rdr => rdr.GetString(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        default: //Anything else is not string representable
                            dataValueReaders[name] = rdr => string.Empty;
                            break;
                    }
                }
                else if (pd is GeometricPropertyDefinition gp)
                {
                    geomNames.Add(gp.Name);
                }
            }

            WriteReaderAsCsv(cmd, reader, dataValueReaders, geomNames);
        }

        internal static void WriteReaderDefault(BaseCommand cmd,
                                                IReader reader,
                                                Dictionary<string, Func<IReader, string>> dataValueReaders,
                                                ICollection<string> geomNames,
                                                List<string> idNames)
        {
            var sb = new StringBuilder(2048);
            using (var geomFactory = new FgfGeometryFactory())
            {
                while (reader.ReadNext())
                {
                    sb.Clear();
                    sb.Append("Feature");
                    if (idNames.Count > 0)
                    {
                        sb.Append("(");
                        for (int i = 0; i < idNames.Count; i++)
                        {
                            var name = idNames[i];
                            if (i > 0)
                            {
                                sb.Append(", ");
                            }
                            sb.Append(name + ": ");
                            sb.Append(dataValueReaders[name](reader));
                        }
                        sb.Append(")");
                    }
                    cmd.WriteLine(sb.ToString());
                    using (cmd.Indent())
                    {
                        foreach (var kvp in dataValueReaders)
                        {
                            if (reader.IsNull(kvp.Key))
                                cmd.WriteLine($"{kvp.Key}: (null)");
                            else
                                cmd.WriteLine($"{kvp.Key}: {kvp.Value(reader)}");
                        }
                        foreach (var gn in geomNames)
                        {
                            if (reader.IsNull(gn))
                            {
                                cmd.WriteLine($"{gn}: (null)");
                            }
                            else
                            {
                                var fgf = reader.GetGeometry(gn);
                                using (var geom = geomFactory.CreateGeometryFromFgf(fgf))
                                {
                                    cmd.WriteLine($"{gn}: {geom.Text}");
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void WriteFeatureReader(BaseCommand cmd, IFeatureReader reader)
        {
            var clsDef = reader.GetClassDefinition();
            var clsProps = clsDef.Properties;
            var idProps = clsDef.IdentityProperties;

            var dataValueReaders = new Dictionary<string, Func<IReader, string>>();
            var geomNames = new HashSet<string>();
            var idNames = new List<string>();

            foreach (DataPropertyDefinition pd in idProps)
            {
                idNames.Add(pd.Name);
            }

            foreach (var pd in clsProps)
            {
                if (pd is DataPropertyDefinition dp)
                {
                    string name = dp.Name;
                    switch (dp.DataType)
                    {
                        case DataType.DataType_Boolean:
                            dataValueReaders[name] = rdr => rdr.GetBoolean(name) ? "true" : "false";
                            break;
                        case DataType.DataType_Byte:
                            dataValueReaders[name] = rdr => rdr.GetByte(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_DateTime:
                            dataValueReaders[name] = rdr => rdr.GetDateTime(name).ToString("o");
                            break;
                        case DataType.DataType_Decimal:
                        case DataType.DataType_Double:
                            dataValueReaders[name] = rdr => rdr.GetDouble(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int16:
                            dataValueReaders[name] = rdr => rdr.GetInt16(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int32:
                            dataValueReaders[name] = rdr => rdr.GetInt32(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Int64:
                            dataValueReaders[name] = rdr => rdr.GetInt64(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_Single:
                            dataValueReaders[name] = rdr => rdr.GetSingle(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        case DataType.DataType_String:
                            dataValueReaders[name] = rdr => rdr.GetString(name).ToString(CultureInfo.InvariantCulture);
                            break;
                        default: //Anything else is not string representable
                            dataValueReaders[name] = rdr => string.Empty;
                            break;
                    }
                }
                else if (pd is GeometricPropertyDefinition gp)
                {
                    geomNames.Add(gp.Name);
                }
            }

            WriteReaderDefault(cmd, reader, dataValueReaders, geomNames, idNames);
        }

        internal static void WriteSpatialContexts(BaseCommand cmd, ReadOnlyCollection<SpatialContextInfo> contexts)
        {
            foreach (var ctx in contexts)
            {
                cmd.WriteLine(ctx.Name);
                if (cmd is ISummarizableCommand sum && sum.Detailed)
                {
                    using (cmd.Indent())
                    {
                        cmd.WriteLine("Description: {0}", ctx.Description);
                        cmd.WriteLine("XY Tolerance: {0}", ctx.XYTolerance);
                        cmd.WriteLine("Z Tolerance: {0}", ctx.ZTolerance);
                        cmd.WriteLine("Coordinate System: {0}", ctx.CoordinateSystem);
                        cmd.WriteLine("Coordinate System WKT: {0}", ctx.CoordinateSystemWkt);
                        if (!string.IsNullOrEmpty(ctx.CoordinateSystemWkt))
                        {
                            using (var catalog = new CoordinateSystemCatalog())
                            {
                                using (cmd.Indent())
                                {
                                    var csCode = catalog.ConvertWktToCoordinateSystemCode(ctx.CoordinateSystemWkt);
                                    var epsg = catalog.ConvertWktToEpsgCode(ctx.CoordinateSystemWkt);
                                    cmd.WriteLine("Coordinate System Code: {0}", csCode);
                                    cmd.WriteLine("Coordinate System EPSG Code: {0}", epsg);
                                }
                            }
                        }
                        cmd.WriteLine("Coordinate System WKT inferred: {0}", ctx.IsWktInferred);
                        cmd.WriteLine("Extent Type: {0}", ctx.ExtentType);
                        if (ctx.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Static)
                        {
                            using (cmd.Indent())
                            {
                                cmd.WriteLine("Extent: " + ctx.ExtentGeometryText);
                            }
                        }
                    }
                }
            }
        }

        internal static void WriteSchemaNames(BaseCommand cmd, ICollection<string> schemas)
        {
            foreach (var name in schemas)
            {
                cmd.WriteLine(name);
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
                cmd.WriteLines(GetGeometryTypes(geom.GeometryTypes));
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
                cmd.WriteLine("{0}", propDef.Name);
                if (cmd is ISummarizableCommand sum && sum.Detailed)
                {
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
}
