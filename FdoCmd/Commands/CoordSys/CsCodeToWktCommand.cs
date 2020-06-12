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
using System;

namespace FdoCmd.Commands.CoordSys
{
    [Verb("cs-code-to-wkt", HelpText = "Converts the given mentor CS code to WKT")]
    public class CsCodeToWktCommand : BaseCommand
    {
        [Option("code", Required = true, HelpText = "The mentor CS code")]
        public string Code { get; set; }

        public override int Execute()
        {
            var csFactory = new MgCoordinateSystemFactory();
            var wkt = csFactory.ConvertCoordinateSystemCodeToWkt(this.Code);
            WriteLine(wkt);
            return (int)CommandStatus.E_OK;
        }
    }
}
