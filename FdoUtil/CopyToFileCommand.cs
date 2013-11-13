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
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;
using System.IO;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core;
using FdoToolbox.Core.ETL.Specialized;
using OSGeo.FDO.Schema;

namespace FdoUtil
{
    public class CopyToFileCommand : ConsoleCommand
    {
        private string _srcProvider;
        private string _srcConnStr;
        private string _srcSchema;
        private List<string> _srcClasses;
        private string _destPath;
        private string _srcSpatialContext;
        private bool _flatten;

        public CopyToFileCommand(string srcProvider, string srcConnStr, string srcSchema, List<string> srcClasses, string destPath, string srcSpatialContext, bool flatten)
        {
            _srcProvider = srcProvider;
            _srcConnStr = srcConnStr;
            _srcSchema = srcSchema;
            _srcClasses = srcClasses;
            _destPath = destPath;
            _srcSpatialContext = srcSpatialContext;
            _flatten = flatten;
        }

        public string LogFile
        {
            get;
            set;
        }

        private string GenerateLogFileName(string prefix)
        {
            if (!string.IsNullOrEmpty(this.LogFile))
                return this.LogFile;

            var dt = DateTime.Now;
            return prefix + string.Format("{0}y{1}m{2}d{3}h{4}m{5}s", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second) + ".log";
        }

        public override int Execute()
        {
            CommandStatus retCode;

            FdoConnection srcConn = new FdoConnection(_srcProvider, _srcConnStr);
            FdoConnection destConn = null;
            //Directory given, assume SHP
            if (Directory.Exists(_destPath))
            {
                destConn = new FdoConnection("OSGeo.SHP", "DefaultFileLocation=" + _destPath);
            }
            else
            {
                if (ExpressUtility.CreateFlatFileDataSource(_destPath))
                    destConn = ExpressUtility.CreateFlatFileConnection(_destPath);
                else
                    throw new FdoException("Could not create data source: " + _destPath);
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
                    if (!string.IsNullOrEmpty(_srcSpatialContext))
                    {
                        SpatialContextInfo srcCtx = srcService.GetSpatialContext(_srcSpatialContext);
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
                    if (_srcClasses.Count > 0)
                    {
                        WriteLine("Checking if partial schema discovery is supported: " + srcService.SupportsPartialSchemaDiscovery());

                        srcSchema = srcService.PartialDescribeSchema(_srcSchema, _srcClasses);
                    }
                    else //Full copy
                    {
                        WriteLine("No classes specified, reading full source schema");
                        srcSchema = srcService.GetSchemaByName(_srcSchema);
                    }

                    if (srcSchema == null)
                    {
                        WriteError("Could not find source schema: " + _srcSchema);
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
                            WriteLine("Applying source schema (containing " +  clsCount + " classes) to target");
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
                            FdoClassCopyOptions copt = new FdoClassCopyOptions(srcName, dstName, srcSchema.Name, cd.Name, targetSchema.Name, cd.Name);
                            copt.FlattenGeometries = _flatten;
                            options.AddClassCopyOption(copt);
                        }

                        if (_flatten)
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

        void OnCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Copy completed");
        }

        void OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
