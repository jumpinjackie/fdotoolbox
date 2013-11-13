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

namespace FdoInfo
{
    public class FdoInfoApp : ConsoleApplication
    {
        public override void ParseArguments(string[] args)
        {
            string cmdName = GetArgument("-cmd", args);
            ThrowIfEmpty(cmdName, "-cmd");

            switch (cmdName)
            {
                case "ListProviders":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Gets and displays all the registerd FDO providers");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:ListProviders");
                            return;
                        }

                        _Command = new ListProvidersCommand();
                    }
                    break;
                case "GetConnectionParameters":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Gets and displays the connection parameters for a given provider");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:GetConnectionParameters -provider:<provider name>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");
                        _Command = new GetConnectionParametersCommand(provider);
                    }
                    break;
                case "GetCreateDataStoreParameters":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Gets and displays the parameters required to create a Data Store for a given provider");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:GetCreateDataStoreParameters -provider:<provider name>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");

                        _Command = new GetCreateDataStoreParametersCommand(provider);
                    }
                    break;
                case "GetDestroyDataStoreParameters":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Gets and displays the parameters required to destroy a Data Store for a given provider");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:GetDestroyDataStoreParameters -provider:<provider name>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");

                        _Command = new GetDestroyDataStoreParametersCommand(provider);
                    }
                    break;
                case "ListDataStores":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Displays the data stores of a given connection");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:ListDataStores -provider:<provider name> -connection:<connection string> [-fdoOnly]");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");

                        string connStr = GetArgument("-connection", args);
                        ThrowIfEmpty(connStr, "-connection");

                        bool fdoOnly = IsSwitchDefined("-fdoOnly", args);

                        _Command = new ListDataStoresCommand(provider, connStr, fdoOnly);
                    }
                    break;
                case "ListBcpTasks":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Lists all available copy tasks for execution in a bulk copy definition file");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:ListBcpTasks -task:<path to bulk copy definition>");
                            return;
                        }

                        string task = GetArgument("-task", args);
                        ThrowIfEmpty(task, "-task");

                        _Command = new ListBcpTasksCommand(task);
                    }
                    break;
                case "ListSchemas":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Displays the feature schemas for a given connection");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:ListSchemas -provider:<provider name> -connection:<connection string>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");

                        string connStr = GetArgument("-connection", args);
                        ThrowIfEmpty(connStr, "-connection");

                        _Command = new ListSchemasCommand(provider, connStr);
                    }
                    break;
                case "ListClasses":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Displays the feature classes under a given feature schema");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:ListClasses -provider:<provider name> -connection:<connection string> -schema:<schema name>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");

                        string connStr = GetArgument("-connection", args);
                        ThrowIfEmpty(connStr, "-connection");

                        string schema = GetArgument("-schema", args);
                        ThrowIfEmpty(schema, "-schema");

                        _Command = new ListClassesCommand(provider, connStr, schema);
                    }
                    break;
                case "ListClassProperties":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Displays the properties under a given feature class");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:ListClassProperties -provider:<provider name> -connection:<connection string> -schema:<schema name> -class:<class name>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");

                        string connStr = GetArgument("-connection", args);
                        ThrowIfEmpty(connStr, "-connection");

                        string schema = GetArgument("-schema", args);
                        ThrowIfEmpty(schema, "-schema");

                        string className = GetArgument("-class", args);
                        ThrowIfEmpty(className, "-class");

                        _Command = new ListClassPropertiesCommand(provider, connStr, schema, className);
                    }
                    break;
                case "ListSpatialContexts":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            Console.WriteLine("Displays the defined spatial contexts in a given connection");
                            Console.WriteLine("Usage: FdoInfo.exe -cmd:ListSpatialContexts -provider:<provider name> -connection:<connection string>");
                            return;
                        }

                        string provider = GetArgument("-provider", args);
                        ThrowIfEmpty(provider, "-provider");

                        string connStr = GetArgument("-connection", args);
                        ThrowIfEmpty(connStr, "-connection");

                        _Command = new ListSpatialContextsCommand(provider, connStr);
                    }
                    break;
                default:
                    throw new ArgumentException("Undefined command: " + cmdName);
            }
        }

        public override void ShowUsage()
        {
            string msg =
@"Usage: FdoInfo.exe -cmd:<command name> <command parameters>
Valid command names include:
- GetConnectionParameters
- GetCreateDataStoreParameters
- GetDestroyDataStoreParameters
- ListBcpTasks
- ListClasses
- ListClassProperties
- ListDataStores
- ListProviders
- ListSchemas
- ListSpatialContexts
For more information about a particular command type:
FdoInfo.exe -cmd:<command name> -help";
            Console.WriteLine(msg);
        }
    }
}
