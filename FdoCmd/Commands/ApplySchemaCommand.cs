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
using CommandLine.Text;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

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

        [Option("rename-schemas", HelpText = "A series of schema names to be renamed before applying. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> SchemaNameRemappings { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Apply schema for SDF file (explicit connection)", new ApplySchemaCommand { Provider = "OSGeo.SDF", ConnectParameters = new[] { "File", "C:\\path\\to\\MyFile.sdf" }, SchemaFile = "C:\\path\\to\\MySchema.xml" });
                yield return new Example("Apply schema for SDF file (inferred file path)", new ApplySchemaCommand { FilePath = "C:\\path\\to\\MyFile.sdf", SchemaFile = "C:\\path\\to\\MySchema.xml" });
                yield return new Example("Apply schema for SDF file and rename schema name from file", new ApplySchemaCommand { FilePath = "C:\\path\\to\\MyFile.sdf", SchemaFile = "C:\\path\\to\\MySchema.xml", SchemaNameRemappings = new[] { "SHP_Schema", "SDF_Schema"  } });
            }
        }

        protected override int ExecuteCommand(IConnection conn, string provider, IApplySchema cmd)
        {
            var (renames, rc) = ValidateTokenPairSet("--rename-schemas", this.SchemaNameRemappings);
            if (rc.HasValue)
            {
                return rc.Value;
            }
            var renameDict = renames.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            CommandStatus retCode = CommandStatus.E_OK;
            var schemas = new FeatureSchemaCollection(null);
            schemas.ReadXml(this.SchemaFile);

            if (renameDict.Count > 0)
            {
                foreach (FeatureSchema schema in schemas)
                {
                    if (renameDict.ContainsKey(schema.Name))
                    {
                        var oldName = schema.Name;
                        schema.Name = renameDict[oldName];
                        Console.WriteLine($"Renaming schema before apply: {oldName} -> {schema.Name}");
                    }
                }
            }

            var sc = conn.SchemaCapabilities;
            var schemaChecker = new SchemaCapabilityChecker(sc);
            var schemaWalker = new SchemaWalker(conn);
            var activeSc = conn.GetActiveSpatialContext();

            foreach (FeatureSchema fs in schemas)
            {
                FeatureSchema toApply = null;
                IncompatibleSchema incSchema;
                if (this.Fix && !schemaChecker.CanApplySchema(fs, out incSchema))
                {
                    var schema = schemaChecker.AlterSchema(fs, incSchema, () => activeSc);
                    toApply = schema;
                }
                else
                {
                    toApply = fs;
                }

                // See if a source schema of the same name already exists
                var sourceSchema = schemaWalker.GetSchemaByName(fs.Name);
                if (sourceSchema == null) //If not, great! Apply as is
                {
                    cmd.FeatureSchema = toApply;
                }
                else // Otherwise, alter the fetched source
                {
                    sourceSchema.ApplyChangesFrom(toApply, Console.WriteLine);
                    cmd.FeatureSchema = sourceSchema;
                }

                cmd.Execute();
                Console.WriteLine("Applied schema using provider: " + provider);
            }
            return (int)retCode;
        }
    }
}
