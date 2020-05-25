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
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FdoCmd.Commands
{
    [Verb("execute-sql-query", HelpText = "Executes a SQL query")]
    public class ExecuteSqlCommand : ProviderConnectionCommand<ISQLCommand>
    {
        public ExecuteSqlCommand()
            : base(CommandType.CommandType_SQLCommand, "Executing SQL queries")
        { }

        [Option("sql", HelpText = "The SQL query to execute. Can be inline SQL or a path to a file containing the SQL query to execute", Required = true)]
        public string Sql { get; set; }

        [Option("format", Default = QueryFeaturesOutputFormat.CSV, HelpText = "The output format for these results")]
        public QueryFeaturesOutputFormat Format { get; set; }

        protected override int ExecuteCommand(IConnection conn, string provider, ISQLCommand cmd)
        {
            CommandStatus retCode = CommandStatus.E_OK;
            var sql = this.Sql;
            if (File.Exists(sql))
            {
                sql = File.ReadAllText(sql);
            }

            cmd.SQLStatement = sql;
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var dataValueReaders = new Dictionary<string, Func<IReader, string>>();
                    var geomNames = new List<string>();
                    var cc = reader.GetColumnCount();
                    for (int i = 0; i < cc; i++)
                    {
                        var pt = reader.GetPropertyType(i);
                        var name = reader.GetColumnName(i);
                        if (pt == OSGeo.FDO.Schema.PropertyType.PropertyType_DataProperty)
                        {
                            var dt = reader.GetColumnType(i);
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

                    var adapter = new SqlReaderAdapter(reader);
                    switch (this.Format)
                    {
                        case QueryFeaturesOutputFormat.GeoJSON:
                            PrintUtils.WriteReaderAsGeoJson(this, adapter, dataValueReaders, geomNames);
                            break;
                        case QueryFeaturesOutputFormat.CSV:
                            PrintUtils.WriteReaderAsCsv(this, adapter, dataValueReaders, geomNames);
                            break;
                        case QueryFeaturesOutputFormat.Default:
                            PrintUtils.WriteReaderDefault(this, adapter, dataValueReaders, geomNames, new List<string>());
                            break;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_SQL_EXECUTION_ERROR;
            }

            return (int)retCode;

            string QuoteValue(string s) => Format == QueryFeaturesOutputFormat.CSV ? s : "\"" + s.Replace("\"", "\\\"") + "\"";
        }
    }
}
