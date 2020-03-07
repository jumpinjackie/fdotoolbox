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
using FdoToolbox.Core.Utility;
using System;

namespace FdoCmd.Commands
{
    [Verb("create-file", HelpText = "Creates a file-based data store. Only applies to SHP/SDF/SQLite FDO providers")]
    public class CreateFileCommand : BaseCommand
    {
        [Option("file", HelpText = "The file to create. Must end with a .shp, .sdf or .sqlite extension", Required = true)]
        public string File { get; set; }

        [Option("schema-path", HelpText = "The path to the FDO XML schema file to apply")]
        public string SchemaFile { get; set; }

        public override int Execute()
        {
            CommandStatus retCode = CommandStatus.E_OK;

            bool create = ExpressUtility.CreateFlatFileDataSource(this.File);
            if (!create)
            {
                WriteLine("Failed to create file {0}", this.File);
                retCode = CommandStatus.E_FAIL_CREATE_DATASTORE;
                return (int)retCode;
            }
            WriteLine("File {0} created", this.File);
            if (System.IO.File.Exists(this.SchemaFile))
            {
                try
                {
                    FdoConnection conn = ExpressUtility.CreateFlatFileConnection(this.File);
                    conn.Open();
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.LoadSchemasFromXml(this.SchemaFile);
                        WriteLine("Schema applied to {0}", this.File);
                    }
                    retCode = CommandStatus.E_OK;
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    retCode = CommandStatus.E_FAIL_APPLY_SCHEMA;
                }
            }
            return (int)retCode;
        }
    }
}
