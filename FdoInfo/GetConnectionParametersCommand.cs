#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoInfo
{
    public class GetConnectionParametersCommand : ConsoleCommand
    {
        private string _provider;

        public GetConnectionParametersCommand(string provider)
        {
            _provider = provider;
        }

        public override int Execute()
        {
            CommandStatus retCode;

            IConnection conn = null;
            try
            {
                conn = FeatureAccessManager.GetConnectionManager().CreateConnection(_provider);
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
            }

            IConnectionPropertyDictionary dict = conn.ConnectionInfo.ConnectionProperties;

            Console.WriteLine("Connection properties for: {0}", _provider);
            foreach(string name in dict.PropertyNames)
            {
                Console.WriteLine("\nProperty Name: {0}\n\n\tLocalized Name: {1}", name, dict.GetLocalizedName(name));
                Console.WriteLine("\tRequired: {0}\n\tProtected: {1}\n\tEnumerable: {2}",
                    dict.IsPropertyRequired(name),
                    dict.IsPropertyProtected(name),
                    dict.IsPropertyEnumerable(name));
                if (dict.IsPropertyEnumerable(name))
                {
                    Console.WriteLine("\tValues for property:");
                    try
                    {
                        string[] values = dict.EnumeratePropertyValues(name);
                        foreach (string str in values)
                        {
                            Console.WriteLine("\t\t- {0}", str);
                        }
                    }
                    catch (OSGeo.FDO.Common.Exception)
                    {
                        Console.Error.WriteLine("\t\tProperty values not available");
                    }
                }
            }
            retCode = CommandStatus.E_OK;
            return (int)retCode;
        }
    }
}
