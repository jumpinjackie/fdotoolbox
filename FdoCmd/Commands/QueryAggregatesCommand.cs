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
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FdoCmd.Commands
{
    [Verb("query-aggregates", HelpText = "Queries for aggregations of feature data from the given data store")]
    public class QueryAggregatesCommand : ProviderConnectionCommand<ISelectAggregates>
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

        [Option("order-by", HelpText = "An optional list of properties to order by")]
        public IEnumerable<string> OrderBy { get; set; }

        [Option("group-by", HelpText = "An optional list of properties to group by")]
        public IEnumerable<string> GroupBy { get; set; }

        [Option("having", HelpText = "An optional grouping filter")]
        public string Having { get; set; }

        [Option("distinct", HelpText = "Return distinct results")]
        public bool Distinct { get; set; }

        [Option("order-by-desc", HelpText = "If true, the query will be ordered in descending order by the specified properties. Otherwise it is ordered in ascending order")]
        public bool OrderByDesc { get; set; }

        [Option("format", Default = QueryFeaturesOutputFormat.Default, HelpText = "The output format for these results")]
        public QueryFeaturesOutputFormat Format { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Query distinct values of property (NAME) in SHP file", new QueryAggregatesCommand 
                { 
                    Provider = "OSGeo.SHP", 
                    ConnectParameters = new[] { "DefaultFileLocation", "C:\\path\\to\\MyFile.shp" },
                    Schema = "Default",
                    ClassName = "MyFeatureClass",
                    PropertyNames = new [] { "NAME" },
                    Distinct = true
                });
            }
        }

        public QueryAggregatesCommand()
           : base(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates, CommandCapabilityDescriptions.SelectAggregates)
        { }

        protected override int ExecuteCommand(IConnection conn, string provider, ISelectAggregates cmd)
        {
            using (var caps = conn.CommandCapabilities)
            {
                if (OrderBy?.Any() == true && !caps.SupportsSelectOrdering())
                {
                    WriteError("This provider does not support select ordering");
                    return (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
                }
                if (GroupBy?.Any() == true && !caps.SupportsSelectGrouping())
                {
                    WriteError("This provider does not support select grouping");
                    return (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
                }
                if (Distinct && !caps.SupportsSelectDistinct())
                {
                    WriteError("This provider does not support distinct flag for select aggregates");
                    return (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
                }
            }
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
                var (exprs, rc) = ValidateTokenPairSet("--computed-properties", Expressions);
                if (rc.HasValue)
                {
                    return rc.Value;
                }
                else
                {
                    foreach (var kvp in exprs)
                    {
                        var expr = Expression.Parse(kvp.Value);
                        var compident = new ComputedIdentifier(kvp.Key, expr);
                        props.Add(compident);
                    }
                }
            }
            if (OrderBy?.Any() == true)
            {
                var order = cmd.Ordering;
                foreach (var pn in OrderBy)
                {
                    var ident = new Identifier(pn);
                    order.Add(ident);
                }
                cmd.OrderingOption = OrderByDesc
                    ? OSGeo.FDO.Commands.OrderingOption.OrderingOption_Descending
                    : OSGeo.FDO.Commands.OrderingOption.OrderingOption_Ascending;
            }
            if (GroupBy?.Any() == true)
            {
                var grouping = cmd.Grouping;
                foreach (var pn in GroupBy)
                {
                    var ident = new Identifier(pn);
                    grouping.Add(ident);
                }
            }
            if (!string.IsNullOrEmpty(Having))
                cmd.GroupingFilter = OSGeo.FDO.Filter.Filter.Parse(Having);
            if (Distinct)
                cmd.Distinct = Distinct;

            using (var reader = cmd.Execute())
            {
                var dataValueReaders = new Dictionary<string, Func<IReader, string>>();
                var geomNames = new List<string>();
                var cc = reader.GetPropertyCount();
                for (int i = 0; i < cc; i++)
                {
                    var pt = reader.GetPropertyType(i);
                    var name = reader.GetPropertyName(i);
                    if (pt == OSGeo.FDO.Schema.PropertyType.PropertyType_DataProperty)
                    {
                        var dt = reader.GetDataType(i);
                        switch (dt)
                        {
                            case DataType.DataType_Boolean:
                                dataValueReaders[name] = rdr => rdr.GetBoolean(name) ? "true" : "false";
                                break;
                            case DataType.DataType_Byte:
                                dataValueReaders[name] = rdr => rdr.GetByte(name).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataType.DataType_DateTime:
                                dataValueReaders[name] = rdr => QuoteValue(rdr.GetDateTime(name).ToString("o"));
                                break;
                            case DataType.DataType_Decimal:
                            case DataType.DataType_Double:
                                dataValueReaders[name] = rdr => rdr.GetDouble(name).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataType.DataType_Int16:
                                dataValueReaders[name] = rdr => rdr.GetInt16(name).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataType.DataType_Int32:
                                dataValueReaders[name] = rdr => rdr.GetInt32(name).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataType.DataType_Int64:
                                dataValueReaders[name] = rdr => rdr.GetInt64(name).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataType.DataType_Single:
                                dataValueReaders[name] = rdr => rdr.GetSingle(name).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataType.DataType_String:
                                dataValueReaders[name] = rdr => QuoteValue(rdr.GetString(name).ToString(CultureInfo.InvariantCulture));
                                break;
                            default: //Anything else is not string representable
                                dataValueReaders[name] = rdr => string.Empty;
                                break;
                        }
                    }
                    else if (pt == OSGeo.FDO.Schema.PropertyType.PropertyType_GeometricProperty)
                    {
                        geomNames.Add(name);
                    }
                }

                switch (this.Format)
                {
                    case QueryFeaturesOutputFormat.GeoJSON:
                        PrintUtils.WriteReaderAsGeoJson(this, reader, dataValueReaders, geomNames);
                        break;
                    case QueryFeaturesOutputFormat.CSV:
                        PrintUtils.WriteReaderAsCsv(this, reader, dataValueReaders, geomNames);
                        break;
                    case QueryFeaturesOutputFormat.Default:
                        PrintUtils.WriteReaderDefault(this, reader, dataValueReaders, geomNames, new List<string>());
                        break;
                }
                reader.Close();
            }
            return (int)retCode;

            string QuoteValue(string s) => Format == QueryFeaturesOutputFormat.CSV ? s : "\"" + s.Replace("\"", "\\\"") + "\"";
        }
    }
}
