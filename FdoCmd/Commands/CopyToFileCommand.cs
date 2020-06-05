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
using FdoToolbox.Core;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("copy-to-file", HelpText = "Bulk copies one or more feature classes from the source connection to a target SHP/SDF/SQLite file")]
    public class CopyToFileCommand : BaseCommand
    {
        [Option("src-provider", Required = true, HelpText = "The FDO provider name for the source")]
        public string SourceProvider { get; set; }

        [Option("src-connect-params", Required = true, HelpText = "Source Connection Parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> SourceConnectParameters { get; set; }

        [Option("src-schema", HelpText = "The source schema name", Required = true)]
        public string SourceSchema { get; set; }

        [Option("src-classes", HelpText = "The source class names", Required = true)]
        public IEnumerable<string> SourceClasses { get; set; }

        [Option("src-spatial-context", HelpText = "The name of the source spatial context to copy")]
        public string SourceSpatialContext { get; set; }

        [Option("log-file", HelpText = "The path to the log file for logging errors")]
        public string LogFile { get; set; }

        [Option("flatten-geom", HelpText = "If specified, any 3D geometries will be flattened to 2D")]
        public bool FlattenGeometries { get; set; }

        [Option("dest-path", HelpText = "The file to copy data to. If a file, it must have a .sdf, .shp or .sqlite extension. If a directory, it is assumed to be a directory of .shp files to be copied to", Required = true)]
        public string DestPath { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Copy SHP feature classes to new SDF file", new CopyToFileCommand
                {
                    SourceProvider = "OSGeo.SHP",
                    SourceConnectParameters = new [] { "DefaultFileLocation", "C:\\Path\\To\\YourShapefileDirectory" },
                    DestPath = "C:\\Path\\To\\Your.sdf",
                    SourceSchema = "Default",
                    SourceClasses = new List<string> { "Parcels", "World_Countries" }
                });
            }
        }

        private string GenerateLogFileName(string prefix)
        {
            if (!string.IsNullOrEmpty(this.LogFile))
                return this.LogFile;

            var dt = DateTime.Now;
            return prefix + string.Format("{0}y{1}m{2}d{3}h{4}m{5}s", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second) + ".log";
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Copy completed");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void LogErrors(List<Exception> errors, string file)
        {
            string dir = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            base.WriteLine("Saving errors to: " + file);

            using (StreamWriter writer = new StreamWriter(file, false))
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    writer.WriteLine("------- EXCEPTION #" + (i + 1) + " -------");
                    writer.WriteLine(errors[i].ToString());
                    writer.WriteLine("------- EXCEPTION END -------");
                }
            }

            base.WriteError("Errors have been logged to {0}", file);
        }

        public override int Execute()
        {
            var (conn, srcProvider, rc) = ConnectUtils.CreateConnection(this.SourceProvider, this.SourceConnectParameters, "--src-connect-params");
            if (rc.HasValue)
                return rc.Value;

            CommandStatus retCode;
            var srcConn = FdoConnection.FromInternalConnection(conn);
            FdoConnection destConn = null;
            //Directory given, assume SHP
            if (Directory.Exists(this.DestPath))
            {
                destConn = new FdoConnection("OSGeo.SHP", "DefaultFileLocation=" + this.DestPath);
            }
            else
            {
                if (ExpressUtility.CreateFlatFileDataSource(this.DestPath))
                    destConn = ExpressUtility.CreateFlatFileConnection(this.DestPath);
                else
                    throw new FdoException("Could not create data source: " + this.DestPath);
            }

            try
            {
                srcConn.Open();
                destConn.Open();

                string srcName = "SOURCE";
                string dstName = "TARGET";

                FdoBulkCopyOptions options = new FdoBulkCopyOptions();
                options.RegisterConnection(srcName, srcConn);
                options.RegisterConnection(dstName, destConn);
                using (FdoFeatureService srcService = srcConn.CreateFeatureService())
                using (FdoFeatureService destService = destConn.CreateFeatureService())
                {
                    //See if spatial context needs to be copied to target
                    if (!string.IsNullOrEmpty(this.SourceSpatialContext))
                    {
                        SpatialContextInfo srcCtx = srcService.GetSpatialContext(this.SourceSpatialContext);
                        if (srcCtx != null)
                        {
                            Console.WriteLine("Copying spatial context: " + srcCtx.Name);
                            ExpressUtility.CopyAllSpatialContexts(new SpatialContextInfo[] { srcCtx }, destConn, true);
                        }
                    }
                    else
                    {
                        //Copy all
                        ExpressUtility.CopyAllSpatialContexts(srcConn, destConn, true);
                    }

                    FeatureSchema srcSchema = null;
                    //See if partial class list is needed
                    if (this.SourceClasses?.Any() == true)
                    {
                        WriteLine("Checking if partial schema discovery is supported: " + srcService.SupportsPartialSchemaDiscovery());

                        srcSchema = srcService.PartialDescribeSchema(this.SourceSchema, this.SourceClasses.ToList());
                    }
                    else //Full copy
                    {
                        WriteLine("No classes specified, reading full source schema");
                        srcSchema = srcService.GetSchemaByName(this.SourceSchema);
                    }

                    if (srcSchema == null)
                    {
                        WriteError("Could not find source schema: " + this.SourceSchema);
                        retCode = CommandStatus.E_FAIL_SCHEMA_NOT_FOUND;
                    }
                    else
                    {
                        WriteLine("Checking source schema for incompatibilities");
                        FeatureSchema targetSchema = null;
                        IncompatibleSchema incSchema;
                        if (destService.CanApplySchema(srcSchema, out incSchema))
                        {
                            int clsCount = srcSchema.Classes.Count;
                            WriteLine("Applying source schema (containing " + clsCount + " classes) to target");
                            destService.ApplySchema(srcSchema, null, true);
                            targetSchema = srcSchema;
                        }
                        else
                        {
                            WriteWarning("Incompatibilities were detected in source schema. Applying a modified version to target");
                            FeatureSchema fixedSchema = destService.AlterSchema(srcSchema, incSchema);
                            int clsCount = fixedSchema.Classes.Count;
                            WriteLine("Applying modified source schema (containing " + clsCount + " classes) to target");
                            destService.ApplySchema(fixedSchema, null, true);
                            targetSchema = fixedSchema;
                        }

                        //Now set class copy options
                        foreach (ClassDefinition cd in srcSchema.Classes)
                        {
                            var copt = new FdoClassCopyOptions(srcName, dstName, srcSchema.Name, cd.Name, targetSchema.Name, cd.Name, null)
                            {
                                FlattenGeometries = this.FlattenGeometries
                            };
                            options.AddClassCopyOption(copt);
                        }

                        if (this.FlattenGeometries)
                        {
                            WriteWarning("The switch -flatten has been defined. Geometries that are copied will have any Z or M coordinates removed");
                        }

                        FdoBulkCopy copy = new FdoBulkCopy(options);
                        copy.ProcessMessage += new MessageEventHandler(OnMessage);
                        copy.ProcessCompleted += new EventHandler(OnCompleted);
                        Console.WriteLine("Executing bulk copy");
                        copy.Execute();
                        List<Exception> errors = new List<Exception>(copy.GetAllErrors());
                        if (errors.Count > 0)
                        {
                            string file = GenerateLogFileName("bcp-error-");
                            LogErrors(errors, file);
                            base.WriteError("Errors were encountered during bulk copy.");
                            retCode = CommandStatus.E_FAIL_BULK_COPY_WITH_ERRORS;
                        }
                        else { retCode = CommandStatus.E_OK; }
                        retCode = CommandStatus.E_OK;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_UNKNOWN;
            }
            finally
            {
                srcConn.Dispose();
                destConn.Dispose();
            }
            return (int)retCode;
        }
    }
}
