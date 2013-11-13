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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoInfo
{
    public class ListSchemasCommand : ConsoleCommand
    {
        private string _provider;
        private string _connstr;

        public ListSchemasCommand(string provider, string connStr)
        {
            _provider = provider;
            _connstr = connStr;
        }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = CreateConnection(_provider, _connstr);
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }

            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                using (FeatureSchemaCollection schemas = service.DescribeSchema())
                {
                    Console.WriteLine("\nSchemas in this connection: {0}", schemas.Count);
                    foreach (FeatureSchema fs in schemas)
                    {
                        Console.WriteLine("\nName: {0}\n", fs.Name);
                        Console.WriteLine("\tQualified Name: {0}", fs.QualifiedName);
                        Console.WriteLine("\tAttributes:");
                        WriteAttributes(fs.Attributes);
                    }
                }
            }

            conn.Close();
            return (int)CommandStatus.E_OK;
        }

        private void WriteAttributes(SchemaAttributeDictionary schemaAttributeDictionary)
        {
            if (schemaAttributeDictionary.AttributeNames.Length > 0)
            {
                foreach (string name in schemaAttributeDictionary.AttributeNames)
                {
                    Console.WriteLine("\t\t- {0} : {1}", name, schemaAttributeDictionary.GetAttributeValue(name));
                }
            }
            else
            {
                Console.WriteLine("\t\tNone");
            }
        }
    }
}
