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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdoCmd.Commands.CoordSys
{
    [Verb("enumerate-cs", HelpText = "Enumerates coordinate systems for a given category")]
    public class EnumerateCoordinateSystemsCommand : BaseCommand
    {
        [Option("category", Required = true, HelpText = "The coordinate system category")]
        public string Category { get; set; }

        [Option("csv", HelpText = "If specified, output as CSV")]
        public bool AsCsv { get; set; }

        public override int Execute()
        {
            var csFactory = new MgCoordinateSystemFactory();
            var coordSystems = csFactory.EnumerateCoordinateSystems(this.Category);
            if (AsCsv)
                PrintUtils.WriteCoordSysEntriesAsCsv(this, coordSystems);
            else
                PrintUtils.WriteCoordSysEntries(this, coordSystems);
            return (int)CommandStatus.E_OK;
        }
    }
}
