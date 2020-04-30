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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;

namespace FdoCmd.Commands
{
    [Verb("list-schemas", HelpText = "Lists schemas for the given connection")]
    public class ListSchemasCommand : ProviderConnectionCommand
    {
        protected override int ExecuteConnection(IConnection conn)
        {
            var walker = new SchemaWalker(conn);
            var schemaNames = walker.GetSchemaNames();
            foreach (var scn in schemaNames)
            {
                WriteLine(scn);
            }
            /*
            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                if (SummaryOnly)
                {
                    var schemas = service.GetSchemaNames();
                    PrintUtils.WriteSchemaNames(this, schemas);
                }
                else
                {
                    using (FeatureSchemaCollection schemas = service.DescribeSchema())
                    {
                        WriteLine("Schemas in this connection: {0}", schemas.Count);
                        foreach (FeatureSchema fs in schemas)
                        {
                            WriteLine("-> {0}", fs.Name);
                            using (Indent())
                            {
                                WriteLine("Qualified Name: {0}", fs.QualifiedName);
                                WriteLine("Description: {0}", fs.Description);
                                WriteLine("Attributes:");
                                using (Indent())
                                {
                                    PrintUtils.WriteAttributes(this, fs.Attributes);
                                }
                            }
                        }
                    }
                }
            }
            */
            return (int)CommandStatus.E_OK;
        }
    }
}
