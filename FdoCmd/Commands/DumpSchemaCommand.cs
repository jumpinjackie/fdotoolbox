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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;

namespace FdoCmd.Commands
{
    [Verb("dump-schema", HelpText = "Dumps the specified schema for the given FDO connection")]
    public class DumpSchemaCommand : ProviderConnectionCommand
    {
        public string SchemaName { get; set; }

        public string SchemaFile { get; set; }

        protected override int ExecuteConnection(IConnection conn)
        {
            CommandStatus retCode;
            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                try
                {
                    if (string.IsNullOrEmpty(this.SchemaName))
                        service.WriteSchemaToXml(this.SchemaFile);
                    else
                        service.WriteSchemaToXml(this.SchemaName, this.SchemaFile);
                    WriteLine("Schema(s) written to {0}", this.SchemaFile);
                    retCode = CommandStatus.E_OK;
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    WriteException(ex);
                    retCode = CommandStatus.E_FAIL_SERIALIZE_SCHEMA_XML;
                }
            }
            return (int)retCode;
        }
    }
}
