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
using FdoToolbox.Core.Configuration;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FdoCmd.Commands
{
    [Verb("copy-class", HelpText = "Copies a feature class (and its data) from a source connection to the target")]
    public class CopyClassCommand : BaseCommand
    {
        [Option("src-provider", Required = true, HelpText = "The FDO provider name for the source")]
        public string SourceProvider { get; set; }

        [Option("src-connect-params", Required = true, HelpText = "Source Connection Parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> SourceConnectParameters { get; set; }

        [Option("src-schema", Required = true, HelpText = "The source schema")]
        public string SourceSchema { get; set; }

        [Option("src-class", Required = true, HelpText = "The source class")]
        public string SourceClassName { get; set; }

        [Option("dst-provider", Required = true, HelpText = "The FDO provider name for the target")]
        public string TargetProvider { get; set; }

        [Option("dst-connect-params", Required = true, HelpText = "Target Connection Parameters. Must be in the form of: <name1> <value1> ... <nameN> <valueN>")]
        public IEnumerable<string> TargetConnectParameters { get; set; }

        [Option("dst-schema", Required = true, HelpText = "The target schema")]
        public string TargetSchema { get; set; }

        [Option("dst-class", Required = true, HelpText = "The target class")]
        public string TargetClassName { get; set; }

        [Option("log-file", Default = null, HelpText = "The path to the log file for logging errors")]
        public string LogFile { get; set; }

        [Option("filter", Default = null, HelpText = "The FDO filter to apply to the source query")]
        public string Filter { get; set; }

        [Option("flatten-geom", Default = false, HelpText = "If specified, any 3D geometries will be flattened to 2D")]
        public bool FlattenGeometries { get; set; }

        [Option("force-wkb", Default = false, HelpText = "If specified, geometries of features to be inserted are converted to WKB format")]
        public bool ForceWkb { get; set; }

        [Option("computed-properties", HelpText = "Computed properties to define")]
        public IEnumerable<string> Expressions { get; set; }

        [Option("property-mappings", HelpText = "Properties to map. Must be of the form: <src1> <target1> ... <srcN> <targetN>")]
        public IEnumerable<string> PropertyMappings { get; set; }

        [Option("delete-target", Default = false, HelpText = "If specified, data on the target class definition is deleted first before copying")]
        public bool DeleteTarget { get; set; }

        [Option("setup-only", HelpText = "If specified, only the setup portion of the Bulk Copy is run")]
        public bool BulkCopySetupOnly { get; set; }

        [Option("generate-task-only", HelpText = "If specified and a save path to the task is specified, this command will exit once the task has been saved (no setup or bulk copy is performed)")]
        public bool GenerateTaskOnly { get; set; }

        [Option("save-task-path", HelpText = "If specified, the generated Bulk Copy task will be saved to the specified path")]
        public string SaveTaskPath { get; set; }

        [Option("override-sc-name", HelpText = "The name of the source spatial context you want to override")]
        public string OverrideScName { get; set; }

        [Option("override-sc-cs", HelpText = "When creating the spatial context, use the coordinate system name instead of the source spatial context CS name")]
        public string OverrideScCoordSysName { get; set; }

        [Option("override-sc-wkt", HelpText = "When creating the spatial context, use the specified WKT instead of the source spatial context WKT")]
        public string OverrideScWkt { get; set; }

        [Option("override-sc-wkt-from-file", HelpText = "When creating the spatial context, use the given file containing the specified WKT instead of the source spatial context WKT")]
        public string OverrideScWktFromFile { get; set; }

        [Option("override-sc-target-name", HelpText = "When creating the spatial context, use the specified name instead of the source spatial context name")]
        public string OverrideScTargetName { get; set; }

        [Option("use-target-sc", HelpText = "If the target class needs to be created, associate any geometries to the given target spatial context")]
        public string UseTargetSpatialContext { get; set; }

        [Option("insert-batch-size", HelpText = "For providers that support it: The batch size to use when inserting features")]
        public int? InsertBatchSize { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Copy feature class from SHP file to SDF file", new CopyClassCommand 
                { 
                    SourceProvider = "OSGeo.SHP",
                    SourceConnectParameters = new [] { "DefaultFileLocation", "C:\\Path\\To\\YourShapefileDirectory" },
                    TargetProvider = "OSGeo.SDF",
                    TargetConnectParameters = new [] { "File", "C:\\Path\\To\\Your.sdf" },
                    SourceSchema = "Default",
                    SourceClassName = "YourFeatureClass",
                    TargetSchema = "Default",
                    TargetClassName = "YourFeatureClass"
                });
                yield return new Example("Copy feature class from SHP file to SDF file (only generate bulk copy definition)", new CopyClassCommand
                {
                    SourceProvider = "OSGeo.SHP",
                    SourceConnectParameters = new[] { "DefaultFileLocation", "C:\\Path\\To\\YourShapefileDirectory" },
                    TargetProvider = "OSGeo.SDF",
                    TargetConnectParameters = new[] { "File", "C:\\Path\\To\\Your.sdf" },
                    SourceSchema = "Default",
                    SourceClassName = "YourFeatureClass",
                    TargetSchema = "Default",
                    TargetClassName = "YourFeatureClass",
                    GenerateTaskOnly = true,
                    SaveTaskPath = "C:\\Path\\To\\Your.BulkCopyDefinition"
                });
            }
        }

        private void LogErrors(List<Exception> errors, string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                string dir = Path.GetDirectoryName(file);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                base.WriteLine("Saving errors to: " + file);

                using (var writer = new StreamWriter(file, false))
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
            else
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    WriteError("------- EXCEPTION #" + (i + 1) + " -------");
                    WriteError(errors[i].ToString());
                    WriteError("------- EXCEPTION END -------");
                }

                WriteError($"{errors.Count} errors occurred");
            }
        }

        private void TryCreateTargetDataStoreIfRequired(IConnection srcConn, IConnection dstConn, string provider)
        {
            var conni = dstConn.ConnectionInfo;
            //Must be file-based
            if (conni.ProviderDatastoreType != ProviderDatastoreType.ProviderDatastoreType_File)
            {
                return;
            }
            var props = conni.ConnectionProperties;
            string path = null;
            switch (provider)
            {
                case "OSGeo.SDF":
                case "OSGeo.SQLite":
                    path = props.GetProperty("File");
                    break;
                case "OSGeo.SHP":
                    path = props.GetProperty("DefaultFileLocation");
                    break;
            }
            if (path != null && !File.Exists(path))
            {
                WriteLine($"Creating target data store: {path}");
                if (provider != "OSGeo.SHP")
                {
                    var cmd = new CreateFileCommand
                    {
                        File = path
                    };
                    if (cmd.Execute() != (int)CommandStatus.E_OK)
                    {
                        throw new Exception("Failed to create data store");
                    }
                }
                else
                {
                    //For SHP, we use the trick of applying a schema in an empty directory to create the shp "file"
                    string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    Directory.CreateDirectory(tempDirectory);
                    try
                    {
                        var connMgr = FeatureAccessManager.GetConnectionManager();
                        var conn = connMgr.CreateConnection("OSGeo.SHP");
                        conn.ConnectionString = $"DefaultFileLocation={tempDirectory}";
                        if (conn.Open() != ConnectionState.ConnectionState_Open)
                        {
                            throw new Exception("Failed to open SHP connection");
                        }
                        using (conn)
                        {
                            var srcWalker = new SchemaWalker(srcConn);
                            var srcClass = srcWalker.GetClassByName(this.SourceSchema, this.SourceClassName);
                            var cloned = FdoSchemaUtil.CloneClass(srcClass);

                            var targetSchema = new FeatureSchema
                            {
                                Name = "Default"
                            };
                            var targetClasses = targetSchema.Classes;
                            targetClasses.Add(cloned);

                            SpatialContextInfo activeSc = null;
                            if (srcClass is FeatureClass fc)
                            {
                                var gp = fc.GeometryProperty;
                                activeSc = srcConn.GetSpatialContext(gp.SpatialContextAssociation);
                            }
                            //Nothing? Fallback to active SC
                            if (activeSc == null)
                            {
                                activeSc = srcConn.GetActiveSpatialContext();
                            }

                            using (var schemaCaps = conn.SchemaCapabilities)
                            {
                                var capsChecker = new SchemaCapabilityChecker(schemaCaps);

                                using (var svc = new FdoFeatureService(conn))
                                {
                                    svc.CreateSpatialContext(activeSc, false);
                                }

                                FeatureSchema toApply = null;
                                if (!capsChecker.CanApplySchema(targetSchema, out var incSchema))
                                    toApply = capsChecker.AlterSchema(targetSchema, incSchema, () => activeSc);
                                else
                                    toApply = targetSchema;

                                using (var apply = (IApplySchema)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema))
                                {
                                    apply.FeatureSchema = toApply;
                                    apply.Execute();
                                }
                            }

                            //Now copy the generated files out of the temp dir into the dir of the original target SHP connection
                            var targetDir = path;
                            if (path.ToLower().EndsWith(".shp"))
                            {
                                targetDir = Path.GetDirectoryName(path);
                            }

                            var files = Directory.GetFiles(tempDirectory);
                            foreach (var f in files)
                            {
                                var ext = Path.GetExtension(f);
                                var ofNoExt = Path.GetFileNameWithoutExtension(path);
                                var fn = ofNoExt + ext;

                                var targetF = Path.Combine(targetDir, fn);
                                // In the interests of safety, do not move files if they are present
                                // on the target dir. This should not be the case in general. If the user
                                // wanted to create a SHP file here, the file in question or its auxillaries
                                // should not be present in the first place
                                File.Move(f, targetF);
                            }

                            conn.Close();
                        }
                    }
                    finally
                    {
                        try
                        {
                            Directory.Delete(tempDirectory);
                        }
                        catch { }
                    }
                }
            }
        }

        public override int Execute()
        {
            var (pm, rc1) = ValidateTokenPairSet("--property-mappings", this.PropertyMappings);
            var (exprs, rc2) = ValidateTokenPairSet("--computed-properties", this.Expressions);
            var (srcConn, srcProvider, rc3) = ConnectUtils.CreateConnection(this.SourceProvider, this.SourceConnectParameters, "--src-connect-params");
            var (dstConn, dstProvider, rc4) = ConnectUtils.CreateConnection(this.TargetProvider, this.TargetConnectParameters, "--dst-connect-params");

            if (rc1.HasValue)
                return rc1.Value;
            if (rc2.HasValue)
                return rc1.Value;
            if (rc3.HasValue)
                return rc1.Value;
            if (rc4.HasValue)
                return rc1.Value;

            var pmDict = pm.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var exprDict = exprs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var bcpDef = new FdoBulkCopyTaskDefinition();

            const string SOURCE_CONN = nameof(SOURCE_CONN);
            const string TARGET_CONN = nameof(TARGET_CONN);

            //Sanity check connections
            using (srcConn)
            using (dstConn)
            {
                if (srcConn.Open() != ConnectionState.ConnectionState_Open)
                {
                    WriteError("Could not open source connection");
                    return (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
                }

                try
                {
                    TryCreateTargetDataStoreIfRequired(srcConn, dstConn, dstProvider);
                }
                catch
                {
                    return (int)CommandStatus.E_FAIL_BULK_COPY_SETUP;
                }

                if (dstConn.Open() != ConnectionState.ConnectionState_Open)
                {
                    WriteError("Could not open target connection");
                    return (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
                }

                FixUpTargetSchemaOrClassIfRequired(dstConn);

                bcpDef.Connections = new[]
                {
                    new FdoConnectionEntryElement
                    {
                        name = SOURCE_CONN,
                        provider = srcProvider,
                        ConnectionString = srcConn.ConnectionString
                    },
                    new FdoConnectionEntryElement
                    {
                        name = TARGET_CONN,
                        provider = dstProvider,
                        ConnectionString = dstConn.ConnectionString
                    },
                };

                var copyEl = new FdoCopyTaskElement
                {
                    name = this.SourceClassName,
                    createIfNotExists = true,
                    Source = new FdoCopySourceElement
                    {
                        connection = SOURCE_CONN,
                        schema = this.SourceSchema,
                        @class = this.SourceClassName
                    },
                    Target = new FdoCopyTargetElement
                    {
                        connection = TARGET_CONN,
                        schema = this.TargetSchema,
                        @class = this.TargetClassName
                    },
                    Options = new FdoCopyOptionsElement
                    {
                        DeleteTarget = this.DeleteTarget,
                        Filter = this.Filter,
                        FlattenGeometries = this.FlattenGeometries,
                        ForceWKB = this.ForceWkb,
                    }
                };

                if (this.InsertBatchSize.HasValue)
                {
                    copyEl.Options.BatchSize = $"{this.InsertBatchSize.Value}";
                }

                if (!string.IsNullOrWhiteSpace(this.UseTargetSpatialContext))
                {
                    copyEl.Options.UseTargetSpatialContext = this.UseTargetSpatialContext;
                }
                else if (!string.IsNullOrWhiteSpace(this.OverrideScName))
                {
                    var ovScWkt = this.OverrideScWkt;
                    if (!string.IsNullOrWhiteSpace(this.OverrideScWktFromFile) && File.Exists(this.OverrideScWktFromFile))
                        ovScWkt = File.ReadAllText(this.OverrideScWktFromFile);

                    if (!string.IsNullOrWhiteSpace(ovScWkt) || 
                        !string.IsNullOrWhiteSpace(this.OverrideScCoordSysName) ||
                        !string.IsNullOrWhiteSpace(this.OverrideScTargetName))
                    {
                        copyEl.Options.SpatialContextWktOverrides = new[]
                        {
                            new SpatialContextOverrideItem
                            {
                                Name = this.OverrideScName,
                                CoordinateSystemName = this.OverrideScCoordSysName,
                                CoordinateSystemWkt = ovScWkt,
                                OverrideName = this.OverrideScTargetName
                            }
                        };
                        WriteLine("Spatial context override specified");
                    }
                }

                var propMaps = new List<FdoPropertyMappingElement>();
                var exprMaps = new List<FdoExpressionMappingElement>();

                var mapProblems = new List<string>();
                var mappedTargets = new HashSet<string>();

                foreach (var exprM in exprDict)
                {
                    if (!pmDict.ContainsKey(exprM.Key))
                    {
                        mapProblems.Add($"No property mapping specified for computed property: {exprM.Key}");
                    }

                    var targetProp = pmDict[exprM.Key];
                    if (mappedTargets.Contains(targetProp))
                    {
                        mapProblems.Add($"Target property {targetProp} has already been mapped. Target properties cannot be mapped more than once");
                    }

                    exprMaps.Add(new FdoExpressionMappingElement
                    {
                        alias = exprM.Key,
                        target = targetProp,
                        createIfNotExists = true,
                        Expression = exprM.Value
                    });
                    mappedTargets.Add(targetProp);
                }

                foreach (var propM in pmDict)
                {
                    // If already covered by expression mappings, skip
                    if (exprDict.ContainsKey(propM.Key))
                    {
                        continue;
                    }

                    if (mappedTargets.Contains(propM.Value))
                    {
                        mapProblems.Add($"Target property {propM.Value} has already been mapped. Target properties cannot be mapped more than once");
                    }

                    propMaps.Add(new FdoPropertyMappingElement
                    {
                        source = propM.Key,
                        target = propM.Value,
                        createIfNotExists = true
                    });
                    mappedTargets.Add(propM.Value);
                }

                if (propMaps.Count == 0 || exprMaps.Count == 0)
                {
                    //Before we log this as an error, check if the target schema/class exists. If it
                    //doesn't interpret this as wanting to create said schema/class as part of the bcp
                    //and populate the property mapping list automatically.

                    WriteLine("No property mappings added. Seeing if we can add property mapppings throughb inference");

                    var srcWalker = new SchemaWalker(srcConn);
                    var dstWalker = new SchemaWalker(dstConn);

                    ClassDefinition scls = null;
                    ClassDefinition dcls = null;
                    if (!dstWalker.GetSchemaNames().Contains(this.TargetSchema))
                    {
                        scls = srcWalker.GetClassByName(this.SourceSchema, this.SourceClassName);
                    }
                    else if (!dstWalker.GetClassNames(this.TargetSchema).Contains(this.TargetClassName))
                    {
                        scls = srcWalker.GetClassByName(this.SourceSchema, this.SourceClassName);
                    }
                    else
                    {
                        scls = srcWalker.GetClassByName(this.SourceSchema, this.SourceClassName);
                        dcls = dstWalker.GetClassByName(this.TargetSchema, this.TargetClassName);
                    }

                    if (scls != null)
                    {
                        using (scls)
                        {
                            var sClsProps = scls.Properties;
                            if (dcls != null)
                            {
                                using (dcls)
                                {
                                    WriteLine("Adding property mappings based on common properties between source and target class");
                                    var dClsProps = dcls.Properties;
                                    var srcProps = new Dictionary<string, string>();
                                    var dstProps = new Dictionary<string, string>();

                                    foreach (PropertyDefinition pd in sClsProps)
                                    {
                                        //Skip non-data/geom props
                                        if (!(pd.PropertyType == PropertyType.PropertyType_DataProperty || pd.PropertyType == PropertyType.PropertyType_GeometricProperty))
                                            continue;

                                        srcProps.Add(pd.Name.ToUpper(), pd.Name);
                                    }
                                    foreach (PropertyDefinition pd in dClsProps)
                                    {
                                        //Skip non-data/geom props
                                        if (!(pd.PropertyType == PropertyType.PropertyType_DataProperty || pd.PropertyType == PropertyType.PropertyType_GeometricProperty))
                                            continue;

                                        //Skip auto-generated data properties
                                        if (pd is DataPropertyDefinition dp && dp.IsAutoGenerated)
                                            continue;

                                        dstProps.Add(pd.Name.ToUpper(), pd.Name);
                                    }

                                    var commonProps = srcProps.Keys.Intersect(dstProps.Keys).ToList();
                                    foreach (var cpn in commonProps)
                                    {
                                        var sn = srcProps[cpn];
                                        var dn = dstProps[cpn];

                                        propMaps.Add(new FdoPropertyMappingElement
                                        {
                                            source = sn,
                                            target = dn,
                                            createIfNotExists = true
                                        });
                                        WriteLine($"  Mapped {sn} -> {dn}");
                                    }
                                }
                            }
                            else
                            {
                                WriteLine("Adding property mappings based on source class");
                                foreach (PropertyDefinition pd in sClsProps)
                                {
                                    propMaps.Add(new FdoPropertyMappingElement
                                    {
                                        source = pd.Name,
                                        target = pd.Name,
                                        createIfNotExists = true
                                    });
                                    WriteLine($"  Mapped {pd.Name} -> {pd.Name}");
                                }
                            }
                        }
                    }
                }

                copyEl.ExpressionMappings = exprMaps.ToArray();
                copyEl.PropertyMappings = propMaps.ToArray();

                if (copyEl.PropertyMappings.Length == 0 && copyEl.ExpressionMappings.Length == 0)
                {
                    mapProblems.Add("No property mappings set in bulk copy");
                }

                if (mapProblems.Count > 0)
                {
                    WriteError("One or more problems encountered setting up property mappings:");
                    foreach (var mp in mapProblems)
                    {
                        WriteError($"  - {mp}");
                    }
                    return (int)CommandStatus.E_FAIL_BULK_COPY_SETUP;
                }

                bcpDef.CopyTasks = new[] { copyEl };
            }
            var loader = new DefinitionLoader();

            string name = null;
            using (var opts = loader.BulkCopyFromXml(bcpDef, ref name, true))
            {
                var copy = new FdoBulkCopy(opts);
                if (!string.IsNullOrEmpty(this.SaveTaskPath))
                {
                    copy.Save(this.SaveTaskPath, "BulkCopy");
                    WriteLine($"Saved bulk copy task to: {this.SaveTaskPath}");
                }

                if (this.GenerateTaskOnly)
                {
                    return (int)CommandStatus.E_OK;
                }

                copy.ProcessMessage += (s, e) =>
                {
                    WriteLine(e.Message);
                };
                copy.ProcessAborted += (s, e) =>
                {
                    WriteLine("Process Aborted");
                };
                copy.ProcessCompleted += (s, e) =>
                {
                    WriteLine("Process Completed");
                };

                copy.OnInit += (s, e) =>
                {
                    var cCopy = copy.GetSubProcessAt(0) as FdoClassToClassCopyProcess;
                    cCopy.RunSetupOnly = this.BulkCopySetupOnly;
                };
                copy.Execute();
                var errors = copy.GetAllErrors().ToList();
                if (errors.Count > 0)
                {
                    LogErrors(errors, this.LogFile);
                    return (int)CommandStatus.E_FAIL_BULK_COPY_WITH_ERRORS;
                }
                else 
                { 
                    return (int)CommandStatus.E_OK;
                }
            }
        }

        private void FixUpTargetSchemaOrClassIfRequired(IConnection dstConn)
        {
            if (this.TargetProvider == "OSGeo.SQLite")
            {
                //HACK: SQLite is hard-coded to "Default" as the schema, so adjust accordingly
                if (this.TargetSchema != "Default")
                {
                    var os = this.TargetSchema;
                    this.TargetSchema = "Default";
                    WriteWarning($"You specified ({os}) for --dst-schema. SQLite only allows for a schema named (Default) and have adjusted this value accordingly");
                }
            }
            else if (this.TargetProvider == "OSGeo.SHP")
            {
                var dstWalker = new SchemaWalker(dstConn);
                var schemaNames = dstWalker.GetSchemaNames();

                if (schemaNames[0] != this.TargetSchema)
                {
                    var os = this.TargetSchema;
                    //HACK: SHP provider is really finicky about schema name. As it's only single-schema, if the user
                    //gave a different target schema, assume they meant the name of the schema we just applied
                    this.TargetSchema = schemaNames[0];
                    WriteWarning($"You specified ({os}) for --dst-schema. SHP files only allow for one schema so we assume you actually meant ({this.TargetSchema}) and have adjusted this value accordingly");
                }

                var conni = dstConn.ConnectionInfo;
                var connp = conni.ConnectionProperties;
                var path = connp.GetProperty("DefaultFileLocation");
                if (path.ToLower().EndsWith(".shp"))
                {
                    //HACK: SHP provider is also really finicky about class name if connecting to a .shp file
                    //because if class name differs from file name our code path assumes we want to create this
                    //new class on-the-fly, which the SHP does not allow for when connecting to a single .shp file
                    //so assume the user meant to say the file name and fix accordingly
                    var fn = Path.GetFileNameWithoutExtension(path);
                    if (fn != this.TargetClassName)
                    {
                        var of = this.TargetClassName;
                        this.TargetClassName = fn;
                        WriteWarning($"You specified ({of}) for --dst-class. Single .shp file connections do not allow for creating new classes on-the-fly so we assume you actually meant ({this.SourceClassName}) and have adjusted this value accordingly");
                    }
                }
            }
        }
    }
}
