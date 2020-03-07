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
using OSGeo.FDO.ClientServices;

namespace FdoCmd.Commands
{
    [Verb("register-provider", HelpText = "Register a FDO provider")]
    public class RegisterProviderCommand : BaseCommand
    {
        [Option("name", HelpText =  "The name of the FDO provider", Required = true)]
        public string Name { get; set; }

        [Option("display-name", HelpText = "The name of the FDO provider", Required = true)]
        public string DisplayName { get; set; }

        [Option("description", HelpText = "The description of the FDO provider", Required = true)]
        public string Description { get; set; }

        [Option("version", HelpText = "The version of the FDO provider", Required = true)]
        public string Version { get; set; }

        [Option("fdo-version", HelpText = "The FDO version of the FDO provider", Required = true)]
        public string FdoVersion { get; set; }

        [Option("path", HelpText = "The path of the FDO provider library", Required = true)]
        public string LibraryPath { get; set; }

        [Option("is-managed")]
        public bool IsManaged { get; set; }

        public override int Execute()
        {
            FeatureAccessManager.GetProviderRegistry().RegisterProvider(
                this.Name,
                this.DisplayName,
                this.Description,
                this.Version,
                this.FdoVersion,
                this.LibraryPath,
                this.IsManaged);
            WriteLine("New provider registered: {0}", this.Name);
            return (int)CommandStatus.E_OK;
        }
    }
}
