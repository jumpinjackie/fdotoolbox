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
    public class ListClassesCommand : ConsoleCommand
    {
        private string _schema;
        private string _provider;
        private string _connstr;

        public ListClassesCommand(string provider, string connStr, string schemaName)
        {
            _schema = schemaName;
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
                FeatureSchema fs = service.GetSchemaByName(_schema);
                if (fs != null)
                {
                    using (fs)
                    {
                        Console.WriteLine("\nClasses in schema {0}: {1}\n", fs.Name, fs.Classes.Count);
                        foreach (ClassDefinition cd in fs.Classes)
                        {
                            Console.WriteLine("Name: {0} ({1})\n\n\tQualified Name: {2}", cd.Name, cd.ClassType, cd.QualifiedName);
                            Console.WriteLine("\tDescription: {0}", cd.Description);
                            Console.WriteLine("\tIs Abstract: {0}\n\tIs Computed: {1}", cd.IsAbstract, cd.IsComputed);
                            if (cd.BaseClass != null)
                                Console.WriteLine("\tBase Class: {0}", cd.BaseClass.Name);
                            Console.WriteLine("\tAttributes:");
                            WriteAttributes(cd.Attributes);
                            Console.WriteLine("");
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine("Could not find schema: {0}", _schema);
                    return (int)CommandStatus.E_FAIL_SCHEMA_NOT_FOUND;
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
