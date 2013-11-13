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
using FdoToolbox.Core.AppFramework;

namespace FdoUtil
{
    public class FdoUtilApp : ConsoleApplication
    {
        public override void ParseArguments(string[] args)
        {
            string cmdName = GetArgument("-cmd", args);
            ThrowIfEmpty(cmdName, "-cmd");

            if (IsSwitchDefined("-test", args))
                this.IsTestOnly = true;

            if (IsSwitchDefined("-quiet", args))
                this.IsSilent = true;

            switch (cmdName)
            {
                case "ExecuteSql":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Descripton: {0}\nUsage: {1}",
                                "Executes a non-SELECT SQL query. The specified provider must support SQL commands",
                                "FdoUtil.exe -cmd:ExecuteSql -provider:<FDO provider> -connection:<connection string> -sql:<sql statement>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);
                        string sql = GetArgument("-sql", args);

                        ThrowIfEmpty(provider, "-provider");
                        ThrowIfEmpty(connStr, "-connection");
                        ThrowIfEmpty(sql, "-sql");

                        _Command = new ExecuteSqlCommand(provider, connStr, sql);
                    }
                    break;
                case "CreateFile":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}",
                                "Create a new flat-file data source with the option of applying a schema to it.\nThe provider is inferred from the file extension",
                                "FdoUtil.exe -cmd:CreateFile -path:<filename> [-schema:<schema file>] [-test] [-quiet]");
                            return;
                        }

                        string sdfFile = GetArgument("-path", args);
                        string schemaFile = GetArgument("-schema", args);
                        ThrowIfEmpty(sdfFile, "-path");

                        if (string.IsNullOrEmpty(schemaFile))
                            _Command = new CreateFileCommand(sdfFile);
                        else
                            _Command = new CreateFileCommand(sdfFile, schemaFile);
                    }
                    break;
                case "ApplySchema":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}",
                                "Applies a schema definition xml file to a FDO data source",
                                "FdoUtil.exe -cmd:ApplySchema -schema:<schema file> -provider:<provider name> -connection:<connection string> [-quiet]");
                            return;
                        }

                        string schemaFile = GetArgument("-file", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);

                        ThrowIfEmpty(schemaFile, "-file");
                        ThrowIfEmpty(provider, "-provider");
                        ThrowIfEmpty(connStr, "-connection");

                        _Command = new ApplySchemaCommand(provider, connStr, schemaFile);
                    }
                    break;
                case "Destroy":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}",
                                "Destroys a datastore in an FDO data source",
                                "FdoUtil.exe -cmd:Destroy -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-test] [-quiet]");
                            return;
                        }

                        string dstoreStr = GetArgument("-properties", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);

                        ThrowIfEmpty(dstoreStr, "-properties");
                        ThrowIfEmpty(provider, "-provider");

                        _Command = new DestroyCommand(provider, connStr, dstoreStr);
                    }
                    break;
                case "DumpSchema":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}",
                                "Writes a schema(s) in a FDO data store to an XML file",
                                "FdoUtil.exe -cmd:DumpSchema -file:<schema file> -provider:<provider> -connection:<connection string> [-schema:<selected schema>] [-test] [-quiet]");
                            return;
                        }

                        string file = GetArgument("-file", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);
                        string schema = GetArgument("-schema", args);

                        ThrowIfEmpty(file, "-file");
                        ThrowIfEmpty(provider, "-provider");
                        ThrowIfEmpty(connStr, "-connection");

                        if (string.IsNullOrEmpty(schema))
                            _Command = new DumpSchemaCommand(provider, connStr, file);
                        else
                            _Command = new DumpSchemaCommand(provider, connStr, file, schema);
                    }
                    break;
                case "CreateDataStore":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}",
                                "Create a new FDO data store",
                                "FdoUtil.exe -cmd:CreateDataStore -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-test] [-quiet]");
                            return;
                        }

                        string dstoreStr = GetArgument("-properties", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);

                        ThrowIfEmpty(dstoreStr, "-properties");
                        ThrowIfEmpty(provider, "-provider");

                        _Command = new CreateDataStoreCommand(provider, connStr, dstoreStr);
                    }
                    break;
                case "RegisterProvider":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}",
                                "Registers a new FDO provider",
                                "FdoUtil.exe -cmd:RegisterProvider -name:<Provider Name> -displayName:<Display Name> -description:<description> -libraryPath:<Path to provider dll> -version:<version> -fdoVersion:<fdo version> -isManaged:<true|false>");
                            return;
                        }

                        string name = GetArgument("-name", args);
                        string display = GetArgument("-displayName", args);
                        string desc = GetArgument("-description", args);
                        string lib = GetArgument("-libraryPath", args);
                        string version = GetArgument("-version", args);
                        string fdoVersion = GetArgument("-fdoVersion", args);
                        string managed = GetArgument("-isManaged", args);

                        ThrowIfEmpty(name, "-name");
                        ThrowIfEmpty(display, "-displayName");
                        ThrowIfEmpty(desc, "-description");
                        ThrowIfEmpty(lib, "-libraryPath");
                        ThrowIfEmpty(version, "-version");
                        ThrowIfEmpty(fdoVersion, "-fdoVersion");
                        ThrowIfEmpty(managed, "-isManaged");

                        _Command = new RegisterProviderCommand(
                            name,
                            display,
                            desc,
                            lib,
                            version,
                            fdoVersion,
                            Convert.ToBoolean(managed));
                    }
                    break;
                case "UnregisterProvider":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}",
                                "Un-registers a FDO provider",
                                "FdoUtil.exe -cmd:UnregisterProvider -name:<Provider Name>");
                            return;
                        }
                        string name = GetArgument("-name", args);
                        ThrowIfEmpty(name, "-name");
                        _Command = new UnregisterProviderCommand(name);
                    }
                    break;
                case "BulkCopy":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}\nNotes: {2}{3}{4}",
                                "Copies data from a FDO data source to any flat-file FDO data source",
                                "FdoUtil.exe -cmd:BulkCopy -src_provider:<provider name> -src_conn:<connection string> -dest_path:<path to file or directory> -src_schema:<source schema name> [-src_classes:<comma-separated list of class names>] [-copy_srs:<source spatial context name>] [-quiet] [-log:<named error log file>]",
                                "When -dest_path is a directory, it is assumed SHP is the output format\n",
                                "Please note that the output format is determined by file extension\n",
                                "Valid file extensions include: sdf, sqlite, db");

                            return;
                        }

                        string srcProvider = GetArgument("-src_provider", args);
                        string srcConnStr = GetArgument("-src_conn", args);
                        string destFile = GetArgument("-dest_path", args);
                        string srcSchema = GetArgument("-src_schema", args);
                        string classes = GetArgument("-src_classes", args);
                        string srcSpatialContext = GetArgument("-copy_srs", args);
                        bool flatten = IsSwitchDefined("-flatten", args);

                        ThrowIfEmpty(srcProvider, "-src_provider");
                        ThrowIfEmpty(srcConnStr, "-src_conn");
                        ThrowIfEmpty(destFile, "-dest_path");
                        ThrowIfEmpty(srcSchema, "-src_schema");

                        List<string> srcClasses = new List<string>();
                        if (!string.IsNullOrEmpty(classes))
                        {
                            string[] tokens = classes.Split(',');
                            if (tokens.Length > 0)
                            {
                                foreach (string className in tokens)
                                {
                                    srcClasses.Add(className);
                                }
                            }
                            else
                            {
                                srcClasses.Add(classes);
                            }
                        }
                        _Command = new CopyToFileCommand(srcProvider, srcConnStr, srcSchema, srcClasses, destFile, srcSpatialContext, flatten);

                        string log = GetArgument("-log", args);
                        if (!string.IsNullOrEmpty(log))
                            ((CopyToFileCommand)_Command).LogFile = log;
                    }
                    break;
                case "RunTask":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Description: {0}\nUsage: {1}\nNotes: {2}{3}{4}",
                                "Runs a pre-defined task definition",
                                "FdoUtil.exe -cmd:RunTask -task:<path to task definition> [-log:<name of error log>]",
                                "The task definition indicated by the -task parameter must be a valid\n",
                                "Bulk Copy or Join Definition\n",
                                "Valid file extensions include: BulkCopyDefinition, JoinDefinition");

                            return;
                        }

                        string taskFile = GetArgument("-task", args);
                        string subTaskName = GetArgument("-bcptask", args);
                        string[] cmdArgs = null;
                        if (!string.IsNullOrEmpty(subTaskName))
                            cmdArgs = new string[] { subTaskName };

                        ThrowIfEmpty(taskFile, "-task");

                        if (cmdArgs != null)
                            _Command = new RunTaskCommand(taskFile, cmdArgs);
                        else
                            _Command = new RunTaskCommand(taskFile);

                        string log = GetArgument("-log", args);
                        if (!string.IsNullOrEmpty(log))
                            ((RunTaskCommand)_Command).LogFile = log;
                    }
                    break;
                default:
                    throw new ArgumentException("Unknown command name: " + cmdName);
            }

            _Command.IsSilent = this.IsSilent;
            _Command.IsTestOnly = this.IsTestOnly;
        }

        public override void ShowUsage()
        {
            string usage =
@"FdoUtil.exe -cmd:<command name> [-quiet] [-test] <command parameters>
-quiet: Run in quiet mode (no console output)
-test: Simulate the command execution (note that some commands will do nothing)
<command name> can be any of the following:
 - ApplySchema
 - CreateDataStore
 - Destroy
 - DumpSchema
 - CreateFile
 - RegisterProvider
 - UnregisterProvider
 - BulkCopy
 - RunTask
 - ExecuteSql
For more information about a command type: FdoUtil.exe -cmd:<command name> -help
For more help. Consult the help file cmd_readme.txt";
            Console.WriteLine(usage);
        }
    }
}
