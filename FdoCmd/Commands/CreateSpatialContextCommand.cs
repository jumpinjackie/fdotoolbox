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
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("create-spatial-context", HelpText = "Create a spatial context (or update wherever updates are supported by the underlying provider)")]
    public class CreateSpatialContextCommand : ProviderConnectionCommand<ICreateSpatialContext>
    {
        public CreateSpatialContextCommand()
            : base(OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext, "creating spatial contexts")
        { }

        [Option("name", Required = true, HelpText = "The name of the spatial context")]
        public string Name { get; set; }

        [Option("cs-name", Required = false, HelpText = "The name of the coordinate system")]
        public string CoordSysName { get; set; }

        [Option("cs-wkt", Required = false, HelpText = "The wkt of the coordinate system")]
        public string CoordSysWkt { get; set; }

        [Option("description", Required = true, HelpText = "The spatial context description")]
        public string Description { get; set; }

        [Option("xy-tol", Required = true, HelpText = "The XY tolerance")]
        public double XYTolerance { get; set; }

        [Option("z-tol", Required = true, HelpText = "The Z tolerance")]
        public double ZTolerance { get; set; }

        [Option("update-existing", Required = false, HelpText = "If set, will update an existing spatial context of the same name")]
        public bool UpdateExisting { get; set; }

        [Option("extent-type", Required = true, HelpText = "The type of extent for this spatial context")]
        public SpatialContextExtentType ExtentType { get; set; }

        [Option("extent", Required = false, HelpText = "The extent coordinates. Required for static extents, but ignored for dynamic extents")]
        public IEnumerable<double> Extent { get; set; }

        protected override int ExecuteCommand(IConnection conn, string provider, ICreateSpatialContext cmd)
        {
            cmd.Name = this.Name;
            cmd.CoordinateSystem = this.CoordSysName;
            cmd.CoordinateSystemWkt = this.CoordSysWkt;
            cmd.Description = this.Description;
            cmd.XYTolerance = this.XYTolerance;
            cmd.ZTolerance = this.ZTolerance;
            cmd.UpdateExisting = this.UpdateExisting;
            cmd.ExtentType = this.ExtentType;
            if (this.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static)
            {
                var bbox = (this.Extent ?? Enumerable.Empty<double>()).ToArray();
                if (bbox.Length != 4)
                {
                    WriteError("Invalid extent. Expected format: <minx> <miny> <maxx> <maxy>");
                    return (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS;
                }
                else
                {
                    // In the name of tolerance, allow for 2 x/y pairs in the wrong order
                    // and use Math.Min and Math.Max to get the right values
                    var minX = Math.Min(bbox[0], bbox[2]);
                    var minY = Math.Min(bbox[1], bbox[3]);
                    var maxX = Math.Max(bbox[0], bbox[2]);
                    var maxY = Math.Max(bbox[1], bbox[3]);

                    using (var geomFactory = new FgfGeometryFactory()) 
                    {
                        string wktfmt = "POLYGON (({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))";
                        var wkt = string.Format(wktfmt,
                            minX, minY,
                            maxX, minY,
                            maxX, maxY,
                            minX, maxY);
                        var env = geomFactory.CreateGeometry(wkt);
                        cmd.Extent = geomFactory.GetFgf(env);
                    }
                }
            }

            cmd.Execute();
            WriteLine("Created spatial context: " + this.Name);
            return (int)CommandStatus.E_OK;
        }
    }
}
