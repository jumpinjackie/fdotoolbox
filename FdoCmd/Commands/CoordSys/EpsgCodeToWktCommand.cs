﻿#region LGPL Header
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
using OSGeo.MapGuide;
using System.Globalization;

namespace FdoCmd.Commands.CoordSys
{
    [Verb("epsg-to-wkt", HelpText = "Converts the given EPSG code to its coordinate system WKT")]
    public class EpsgCodeToWktCommand : BaseCommand
    {
        [Option("epsg", Required = true, HelpText = "The EPSG code")]
        public int Epsg { get; set; }

        public override int Execute()
        {
            using (var catalog = new CoordinateSystemCatalog())
            {
                var wkt = catalog.ConvertEpsgCodeToWkt(this.Epsg.ToString(CultureInfo.InvariantCulture));
                WriteLine(wkt);
                return (int)CommandStatus.E_OK;
            }
        }
    }
}
