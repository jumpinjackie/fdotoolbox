#region LGPL Header
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
using CommandLine;
using FdoToolbox.Core.AppFramework;
using OSGeo.FDO.ClientServices;
using System;

namespace FdoCmd.Commands
{
    [Verb("get-providers", HelpText = "Gets all registered FDO providers")]
    public class ListProvidersCommand : BaseCommand
    {
        [Option("summary", Required = false, Default = false, HelpText = "Only dislpay FDO provider names")]
        public bool SummaryOnly { get; set; }

        public override int Execute()
        {
            try
            {
                var providers = FeatureAccessManager.GetProviderRegistry().GetProviders();
                using (providers)
                {
                    if (SummaryOnly)
                    {
                        foreach (Provider provider in providers)
                        {
                            WriteLine("-> {0}", provider.Name);
                        }
                    }
                    else
                    {
                        foreach (Provider provider in providers)
                        {
                            WriteLine("-> {0}", provider.Name);
                            using (Indent())
                            {
                                WriteLine("Display Name: {0}", provider.DisplayName);
                                WriteLine("Library Path: {0}", provider.LibraryPath);
                                WriteLine("Version: {0}", provider.Version);
                                WriteLine("FDO Version: {0}", provider.FeatureDataObjectsVersion);
                                WriteLine("Is Managed: {0}", provider.IsManaged);
                                WriteLine("Description: {0}", provider.Description);
                            }
                        }
                    }
                }
                return (int)CommandStatus.E_OK;
            }
            catch (Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_UNKNOWN;
            }
        }
    }
}
