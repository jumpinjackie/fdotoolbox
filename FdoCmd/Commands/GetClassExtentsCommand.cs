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
using CommandLine;
using CommandLine.Text;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FdoCmd.Commands
{
    [Verb("get-class-extent", HelpText = "Gets the extent of the given feature class")]
    public class GetClassExtentsCommand : ProviderConnectionCommand
    {
        [Option("schema", HelpText = "The schema name")]
        public string Schema { get; set; }

        [Option("class", Required = true, HelpText = "The class name")]
        public string ClassName { get; set; }

        [Option("filter", HelpText = "An optional FDO filter")]
        public string Filter { get; set; }

        [Option("geojson", HelpText = "Output the extent as GeoJSON")]
        public bool GeoJson { get; set; }

        [Option("force-raw-spin", Default = false, HelpText = "Forces the use of raw spinning the feature reader to obtain the extent")]
        public bool ForceRawSpin { get; set; }

        [Option("transform-to-code", Required = false, HelpText = "Transform geometry data to the projection indicated by the given mentor code")]
        public string TransformToCode { get; set; }

        [Option("transform-to-epsg", Required = false, HelpText = "Transform geometry data to the projection indicated by the given epsg code")]
        public int? TransformToEpsg { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Get extents of a SHP feature class", new GetClassExtentsCommand
                {
                    FilePath = "C:\\Path\\To\\MyShapefile.shp",
                    Schema = "Default",
                    ClassName = "MyFeatureClass"
                });
            }
        }

        private void ApplySelect(IBaseSelect cmd)
        {
            if (!string.IsNullOrEmpty(Schema))
                cmd.SetFeatureClassName($"{Schema}:{ClassName}");
            else
                cmd.SetFeatureClassName(ClassName);

            if (!string.IsNullOrEmpty(Filter))
                cmd.SetFilter(Filter);
        }

        private int RawSpinExtent(IConnection conn, string geomProp, FgfGeometryFactory geomFactory)
        {
            using (var sel = (ISelect)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select))
            {
                ApplySelect(sel);
                var props = sel.PropertyNames;
                var ident = new Identifier(geomProp);
                props.Add(ident);

                using (var fr = sel.Execute())
                {
                    double? minX = null;
                    double? minY = null;
                    double? maxX = null;
                    double? maxY = null;

                    while (fr.ReadNext())
                    {
                        if (!fr.IsNull(geomProp))
                        {
                            var fgf = fr.GetGeometry(geomProp);
                            using (var geom = geomFactory.CreateGeometryFromFgf(fgf))
                            {
                                var env = geom.Envelope;
                                if (!minX.HasValue)
                                    minX = env.MinX;
                                else
                                    minX = Math.Min(minX.Value, env.MinX);

                                if (!minY.HasValue)
                                    minY = env.MinY;
                                else
                                    minY = Math.Min(minY.Value, env.MinY);

                                if (!maxX.HasValue)
                                    maxX = env.MaxX;
                                else
                                    maxX = Math.Max(maxX.Value, env.MaxX);

                                if (!maxY.HasValue)
                                    maxY = env.MaxY;
                                else
                                    maxY = Math.Max(maxY.Value, env.MaxY);
                            }
                        }
                    }

                    if (minX.HasValue && minY.HasValue && maxX.HasValue && maxY.HasValue)
                    {
                        if (GeoJson)
                        {
                            using (var geom = PrintUtils.CreateExtentGeom(geomFactory, minX.Value, minY.Value, maxX.Value, maxY.Value))
                            {
                                var fgf = geomFactory.GetFgf(geom);
                                var rdr = new BBOXReader(fgf, "bbox");
                                PrintUtils.WriteReaderAsGeoJson(this, rdr, new Dictionary<string, Func<IReader, string>>(), new List<string> { "bbox" });
                            }
                        }
                        else
                        {
                            Console.WriteLine("Min X: " + minX.Value);
                            Console.WriteLine("Min Y: " + minY.Value);
                            Console.WriteLine("Max X: " + maxX.Value);
                            Console.WriteLine("Max Y: " + maxY.Value);
                        }
                        return (int)CommandStatus.E_OK;
                    }
                }
            }
            WriteError("No extent found");
            return (int)CommandStatus.E_NO_DATA;
        }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            var walker = new SchemaWalker(conn);
            var clsDef = !string.IsNullOrEmpty(this.Schema)
                ? walker.GetClassByName(this.Schema, this.ClassName)
                : walker.GetClassByName(this.ClassName);
            if (clsDef == null)
            {
                WriteError("Could not find class: " + this.ClassName);
                return (int)CommandStatus.E_FAIL_CLASS_NOT_FOUND;
            }
            string geomProp = null;
            if (clsDef is FeatureClass fc)
            {
                var gp = fc.GeometryProperty;
                geomProp = gp.Name;
            }
            else
            {
                WriteError("Class is not a feature class");
                return (int)CommandStatus.E_NOT_SUPPORTED;
            }

            if (string.IsNullOrEmpty(geomProp))
            {
                WriteError("Feature class has no geometry property");
                return (int)CommandStatus.E_NOT_SUPPORTED;
            }

            var exprCaps = conn.ExpressionCapabilities;
            var funcs = exprCaps.Functions;
            using (var geomFactory = new FgfGeometryFactory())
            {
                if (!this.ForceRawSpin && funcs.IndexOf("SpatialExtents") >= 0)
                {
                    if (HasCommand(conn, OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates, "selecting aggregates", out var _))
                    {
                        using (var selAgg = (ISelectAggregates)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
                        {
                            ApplySelect(selAgg);
                            var props = selAgg.PropertyNames;
                            var cexpr = Expression.Parse($"SpatialExtents({geomProp})");
                            var ci = new ComputedIdentifier("bbox", cexpr);
                            props.Add(ci);
                            using (var dr = selAgg.Execute())
                            {
                                var readerToOutput = dr;
                                if (!string.IsNullOrWhiteSpace(this.TransformToCode) || this.TransformToEpsg.HasValue)
                                {
                                    using (var catalog = new CoordinateSystemCatalog())
                                    {
                                        var sourceWkt = conn.GetSpatialContext(this.Schema, this.ClassName)?.CoordinateSystemWkt;
                                        if (!string.IsNullOrWhiteSpace(sourceWkt))
                                        {
                                            string targetWkt = null;
                                            if (!string.IsNullOrWhiteSpace(this.TransformToCode))
                                                targetWkt = catalog.ConvertCoordinateSystemCodeToWkt(this.TransformToCode);
                                            else if (this.TransformToEpsg.HasValue)
                                                targetWkt = catalog.ConvertEpsgCodeToWkt(this.TransformToEpsg.Value.ToString(CultureInfo.InvariantCulture));

                                            if (!string.IsNullOrWhiteSpace(targetWkt))
                                            {
                                                readerToOutput = new TransformedDataReader(dr, sourceWkt, targetWkt);
                                            }
                                        }
                                    }
                                }

                                if (GeoJson)
                                {
                                    PrintUtils.WriteReaderAsGeoJson(this, readerToOutput, new Dictionary<string, Func<IReader, string>>(), new List<string> { "bbox" });
                                    return (int)CommandStatus.E_OK;
                                }
                                else
                                {
                                    if (readerToOutput.ReadNext())
                                    {
                                        var fgf = readerToOutput.GetGeometry("bbox");
                                        using (var geom = geomFactory.CreateGeometryFromFgf(fgf))
                                        {
                                            var env = geom.Envelope;
                                            Console.WriteLine("Min X: " + env.MinX);
                                            Console.WriteLine("Min Y: " + env.MinY);
                                            Console.WriteLine("Max X: " + env.MaxX);
                                            Console.WriteLine("Max Y: " + env.MaxY);
                                            return (int)CommandStatus.E_OK;
                                        }
                                    }
                                    else
                                    {
                                        WriteError("No extent found");
                                        return (int)CommandStatus.E_NO_DATA;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return RawSpinExtent(conn, geomProp, geomFactory);
                    }
                }
                else
                {
                    return RawSpinExtent(conn, geomProp, geomFactory);
                }
            }
        }

        private void OutputGeoJson(double minX, double minY, double maxX, double maxY)
        {
            
        }
    }
}
