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
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;
using System;

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
                                    minY = env.MinX;
                                else
                                    minY = Math.Min(minY.Value, env.MinY);

                                if (!maxX.HasValue)
                                    maxX = env.MinX;
                                else
                                    maxX = Math.Min(maxX.Value, env.MaxX);

                                if (!maxY.HasValue)
                                    maxY = env.MinX;
                                else
                                    maxY = Math.Min(maxY.Value, env.MaxY);
                            }
                        }
                    }

                    if (minX.HasValue && minY.HasValue && maxX.HasValue && maxY.HasValue)
                    {
                        Console.WriteLine("Min X: " + minX.Value);
                        Console.WriteLine("Min Y: " + minY.Value);
                        Console.WriteLine("Max X: " + maxX.Value);
                        Console.WriteLine("Max Y: " + maxY.Value);
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
                if (funcs.IndexOf("SpatialExtents") >= 0)
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
                                if (dr.ReadNext())
                                {
                                    var fgf = dr.GetGeometry("bbox");
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
    }
}
