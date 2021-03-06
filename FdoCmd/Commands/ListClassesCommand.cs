﻿#region LGPL Header
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
using CommandLine.Text;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;
using System.Collections.Generic;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("list-classes", HelpText = "Lists classes for the given schema")]
    public class ListClassesCommand : ProviderConnectionCommand
    {
        [Option("schema", Required = false, HelpText = "The schema name to list classes of. If not specified, all classes will be listed")]
        public string Schema { get; set; }

        [Option("qualified", Required = false, HelpText = "If set, class names outputted will be qualified")]
        public bool Qualified { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("List classes of SHP file", new ListClassesCommand
                {
                    Provider = "OSGeo.SHP",
                    ConnectParameters = new [] { "DefaultFileLocation", "C:\\Path\\To\\MyShapefiles" },
                    Schema = "Default"
                });
            }
        }

        protected override int ExecuteConnection(IConnection conn, string provider)
        {
            var walker = new SchemaWalker(conn);
            var classNames = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.Schema))
            {
                classNames.AddRange(walker.GetClassNames(this.Schema).Select(cn =>
                {
                    if (Qualified)
                        return this.Schema + ":" + cn;
                    else
                        return cn;
                }));
            }
            else
            {
                var schemaNames = walker.GetSchemaNames();
                foreach (var sn in schemaNames)
                {
                    classNames.AddRange(walker.GetClassNames(sn).Select(cn =>
                    {
                        if (Qualified)
                            return sn + ":" + cn;
                        else
                            return cn;
                    }));
                }
            }

            foreach (var cn in classNames)
            {
                WriteLine(cn);
            }

            /*
            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                if (SummaryOnly)
                {
                    var names = service.GetClassNames(this.Schema);
                    PrintUtils.WriteClassNames(this, this.Schema, names);
                }
                else
                {
                    var fs = service.GetSchemaByName(this.Schema);
                    if (fs != null)
                    {
                        using (fs)
                        {
                            PrintUtils.WriteClasses(this, fs);
                        }
                    }
                    else
                    {
                        WriteError("Could not find schema: {0}", this.Schema);
                        return (int)CommandStatus.E_FAIL_SCHEMA_NOT_FOUND;
                    }
                }
            }
            */
            return (int)CommandStatus.E_OK;
        }
    }
}
