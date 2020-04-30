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
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Expression;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("query-features", HelpText = "Queries features from the given data store")]
    public class QueryFeaturesCommand : ProviderConnectionCommand<ISelect>
    {
        [Option("schema", HelpText = "The schema name")]
        public string Schema { get; set; }

        [Option("class", Required = true, HelpText = "The class name")]
        public string ClassName { get; set; }

        [Option("filter", HelpText = "The class name")]
        public string Filter { get; set; }

        [Option("properties", HelpText = "An optional list of property names to include in the query result. If not specified, all properties are included")]
        public IEnumerable<string> PropertyNames { get; set; }

        [Option("computed-properties", HelpText = "An optional list of computed properties. Must be of the form: <name1> <expr1> ... <nameN> <exprN>")]
        public IEnumerable<string> Expressions { get; set; }

        [Option("geojson", HelpText = "If set, the result is outputted as GeoJSON")]
        public bool GeoJson { get; set; }

        public QueryFeaturesCommand()
            : base(OSGeo.FDO.Commands.CommandType.CommandType_Select, CommandCapabilityDescriptions.Select)
        { }

        protected override int ExecuteCommand(IConnection conn, ISelect cmd)
        {
            CommandStatus retCode = CommandStatus.E_OK;
            if (!string.IsNullOrEmpty(Schema))
                cmd.SetFeatureClassName($"{Schema}:{ClassName}");
            else
                cmd.SetFeatureClassName(ClassName);

            if (!string.IsNullOrEmpty(Filter))
                cmd.SetFilter(Filter);

            var props = cmd.PropertyNames;
            if (PropertyNames?.Any() == true)
            {
                foreach (var pn in PropertyNames)
                {
                    var ident = new Identifier(pn);
                    props.Add(ident);
                }
            }
            if (Expressions?.Any() == true)
            {
                var exprs = Expressions.ToList();
                if ((exprs.Count % 2) != 0)
                {
                    Console.Error.WriteLine("Incorrect computed-properties format. Expected: <name1> <expr1> ... <nameN> <exprN>");
                    retCode = CommandStatus.E_FAIL_INVALID_ARGUMENTS;
                    return (int)retCode;
                }
                else
                {
                    for (int i = 0; i < exprs.Count; i += 2)
                    {
                        var name = exprs[i];
                        var expr = Expression.Parse(exprs[i + 1]);

                        var compident = new ComputedIdentifier(name, expr);
                        props.Add(compident);
                    }
                }
            }

            using (var reader = cmd.Execute())
            {
                if (GeoJson)
                    PrintUtils.WriteFeatureReaderAsGeoJson(this, reader);
                else
                    PrintUtils.WriteFeatureReader(this, reader);
                reader.Close();
            }
            return (int)retCode;
        }
    }
}
