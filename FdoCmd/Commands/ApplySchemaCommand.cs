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
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using System;

namespace FdoCmd.Commands
{
    [Verb("apply-schema", HelpText = "Applies the given schema to the data store")]
    public class ApplySchemaCommand : ProviderConnectionCommand<IApplySchema>
    {
        public ApplySchemaCommand()
            : base(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema, CommandCapabilityDescriptions.ApplySchema)
        { }

        [Option("schema-file", Required = true, HelpText = "The schema file to apply")]
        public string SchemaFile { get; set; }

        [Option("fix-incompatibilities", Required = false, Default = false)]
        public bool Fix { get; set; }

        protected override int ExecuteCommand(IConnection conn, IApplySchema cmd)
        {
            CommandStatus retCode = CommandStatus.E_OK;
            var schemas = new FeatureSchemaCollection(null);
            schemas.ReadXml(this.SchemaFile);
            var sc = conn.SchemaCapabilities;
            var schemaChecker = new SchemaCapabilityChecker(sc);
            var activeSc = conn.GetActiveSpatialContext();

            foreach (FeatureSchema fs in schemas)
            {
                IncompatibleSchema incSchema;
                if (this.Fix && !schemaChecker.CanApplySchema(fs, out incSchema))
                {
                    var schema = schemaChecker.AlterSchema(fs, incSchema, () => activeSc);
                    cmd.FeatureSchema = schema;
                    cmd.Execute();
                }
                else
                {
                    cmd.FeatureSchema = fs;
                    cmd.Execute();
                }
                Console.WriteLine("Applied schema using provider: " + this.Provider);
            }
            return (int)retCode;
        }
    }
}
