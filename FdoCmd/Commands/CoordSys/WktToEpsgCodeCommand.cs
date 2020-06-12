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
using OSGeo.MapGuide;
using System.Globalization;
using System.IO;

namespace FdoCmd.Commands.CoordSys
{
    [Verb("wkt-to-epsg", HelpText = "Converts the given coordinate system WKT to its EPSG code")]
    public class WktToEpsgCodeCommand : BaseCommand
    {
        [Option("wkt", Required = true, HelpText = "The coordinate system WKT")]
        public string Wkt { get; set; }

        public override int Execute()
        {
            string wkt = this.Wkt;
            //If it's a file path, read its contents instead
            if (File.Exists(wkt))
                wkt = File.ReadAllText(wkt);

            var csFactory = new MgCoordinateSystemFactory();
            var epsg = csFactory.ConvertWktToEpsgCode(wkt);
            WriteLine(epsg.ToString(CultureInfo.InvariantCulture));
            return (int)CommandStatus.E_OK;
        }
    }
}
