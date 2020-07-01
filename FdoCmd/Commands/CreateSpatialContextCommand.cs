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
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Geometry;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("create-spatial-context", HelpText = "Create a spatial context (or update wherever updates are supported by the underlying provider)")]
    public class CreateSpatialContextCommand : ProviderConnectionCommand<ICreateSpatialContext>
    {
        public CreateSpatialContextCommand()
            : base(OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext, "creating spatial contexts")
        { }

        [Option("name", HelpText = "The name of the spatial context")]
        public string Name { get; set; }

        [Option("cs-name", HelpText = "The name of the coordinate system")]
        public string CoordSysName { get; set; }

        [Option("cs-wkt", HelpText = "The wkt of the coordinate system")]
        public string CoordSysWkt { get; set; }

        [Option("cs-wkt-from-file", HelpText = "The path to the file containing the wkt of the coordinate system")]
        public string CoordSysWktFromFile { get; set; }

        [Option("description", HelpText = "The spatial context description")]
        public string Description { get; set; }

        [Option("xy-tol", Required = true, HelpText = "The XY tolerance")]
        public double XYTolerance { get; set; }

        [Option("z-tol", Required = true, HelpText = "The Z tolerance")]
        public double ZTolerance { get; set; }

        [Option("update-existing", HelpText = "If set, will update an existing spatial context of the same name")]
        public bool UpdateExisting { get; set; }

        [Option("from-epsg", HelpText = "If set, will resolve the coordinate system from the given EPSG code and use the information within as the basis of the spatial context to create")]
        public int? FromEpsg { get; set; }

        [Option("from-code", HelpText = "If set, will resolve the coordinate system from the given EPSG code and use the information within as the basis of the spatial context to create")]
        public string FromCode { get; set; }

        [Option("extent-type", HelpText = "The type of extent for this spatial context")]
        public SpatialContextExtentType ExtentType { get; set; }

        [Option("extent", Required = false, HelpText = "The extent coordinates. Required for static extents, but ignored for dynamic extents")]
        public IEnumerable<double> Extent { get; set; }

        protected override int ExecuteCommand(IConnection conn, string provider, ICreateSpatialContext cmd)
        {
            var sci = new SpatialContextInfo();
            sci.ExtentType = this.ExtentType;
            using (var connCaps = conn.ConnectionCapabilities)
            {
                if (this.FromEpsg.HasValue)
                {
                    using (var catalog = new CoordinateSystemCatalog())
                    {
                        var cs = catalog.FindCoordinateSystemByEpsgCode($"{this.FromEpsg.Value}");
                        if (cs != null)
                        {
                            sci.ApplyFrom(cs);
                            // Either SQL Server is lying about the SupportsCSysWKTFromCSysName capability
                            // or I am completely misunderstanding the intent of this capability. Either way,
                            // if we pre-filled the SC properties from a resolved CS, discard the WKT as it
                            // runs interference in SRID resolution in SQL Server as the CS code is sufficient
                            if (provider.ToUpper().Contains("SQLSERVERSPATIAL"))
                                sci.CoordinateSystemWkt = null;

                            // The inferred extent is useless if this provider doesn't support static extents
                            if (sci.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static && !connCaps.SpatialContextTypes.Contains(sci.ExtentType))
                            {
                                sci.ExtentType = SpatialContextExtentType.SpatialContextExtentType_Dynamic;
                                sci.ExtentGeometryText = null;
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(this.FromCode))
                {
                    using (var catalog = new CoordinateSystemCatalog())
                    {
                        var cs = catalog.FindCoordinateSystemByCode(this.FromCode);
                        if (cs != null)
                        {
                            sci.ApplyFrom(cs);
                            // Either SQL Server is lying about the SupportsCSysWKTFromCSysName capability
                            // or I am completely misunderstanding the intent of this capability. Either way,
                            // if we pre-filled the SC properties from a resolved CS, discard the WKT as it
                            // runs interference in SRID resolution in SQL Server as the CS code is sufficient
                            if (provider.ToUpper().Contains("SQLSERVERSPATIAL"))
                                sci.CoordinateSystemWkt = null;

                            // The inferred extent is useless if this provider doesn't support static extents
                            if (sci.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static && !connCaps.SpatialContextTypes.Contains(sci.ExtentType))
                            {
                                sci.ExtentType = SpatialContextExtentType.SpatialContextExtentType_Dynamic;
                                sci.ExtentGeometryText = null;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.Name))
                    sci.Name = this.Name;
                if (!string.IsNullOrWhiteSpace(this.CoordSysName))
                    sci.CoordinateSystem = this.CoordSysName;
                if (!string.IsNullOrWhiteSpace(this.CoordSysWktFromFile) && File.Exists(this.CoordSysWktFromFile))
                    sci.CoordinateSystemWkt = File.ReadAllText(this.CoordSysWktFromFile);
                else if (!string.IsNullOrWhiteSpace(this.CoordSysWkt))
                    sci.CoordinateSystemWkt = this.CoordSysWkt;

                if (!string.IsNullOrWhiteSpace(this.Description))
                    sci.Description = this.Description;
                sci.XYTolerance = this.XYTolerance;
                sci.ZTolerance = this.ZTolerance;

                if (this.Extent?.Any() == true && this.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static)
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

                        sci.ExtentGeometryText = SpatialContextInfo.GetEnvelopeWkt(minX, minY, maxX, maxY);
                    }
                }

                //Final checks before we apply the SC to the command
                if (string.IsNullOrWhiteSpace(sci.Name))
                {
                    WriteError("Missing required spatial context name. Did you forget to specify --name or --from-code or --from-epsg?");
                    return (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS;
                }
                if (string.IsNullOrWhiteSpace(sci.Description))
                {
                    WriteError("Missing required spatial context description. Did you forget to specify --description or --from-code or --from-epsg?");
                    return (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS;
                }
                if (!connCaps.SpatialContextTypes.Contains(sci.ExtentType))
                {
                    WriteError("This provider does not support the extent type: " + sci.ExtentType);
                    return (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
                }
            }

            //All good, apply the SC
            sci.ApplyTo(cmd);

            cmd.UpdateExisting = this.UpdateExisting;

            cmd.Execute();
            WriteLine("Created spatial context: " + sci.Name);
            return (int)CommandStatus.E_OK;
        }
    }
}
