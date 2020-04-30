#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
using System;
using System.Collections.Generic;
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Output pipeline operation with support for SQL commands
    /// </summary>
    public class FdoSqlOutputOperation : FdoOutputOperation 
    {
        public FdoSqlOutputOperation(FdoConnection conn, string className)
            : base(conn, className)
        {
            if (!conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSQL))
            {
                throw new ArgumentException("Only providers that support SQL can be used");
            }
        }

        public FdoSqlOutputOperation(FdoConnection conn, string className, NameValueCollection propertyMappings)
            : base(conn, className, propertyMappings)
        {
            if (!conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSQL))
            {
                throw new ArgumentException("Only providers that support SQL can be used");
            }
        }

        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            base.PrepareForExecution(pipelineExecuter);
        }

        private string _sqlTpl = null;
        
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoRow obj in rows)
            {
                if (_sqlTpl == null)
                    PrepareSqlTemplate(obj);

                string sql = BuildSql(obj);
                try
                {
                    _service.ExecuteSQLNonQuery(sql);
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    ex.Data["Class/Table"] = this.ClassName;
                    ex.Data["SQL Command"] = sql;
                    RaiseFailedFeatureProcessed(obj, ex);
                }
                yield return obj;
            }
        }

        private static string EscapeValue(string value)
        {
            return value.Replace("'", "''");
        }

        string QUALIFIER_BEGIN = "[";
        string QUALIFIER_END = "]";

        string VALUE_BEGIN = "'";
        string VALUE_END = "'";

        private void PrepareSqlTemplate(FdoRow obj)
        {
            _propertyTraversalList = new List<string>();
            if (IsUsingPropertyMappings())
            {
                List<string> columns = new List<string>();
                foreach (string name in _mappings.Keys)
                {
                    columns.Add(name);
                }
                _sqlTpl = "INSERT INTO " + GetTableName() + " (" + QUALIFIER_BEGIN + string.Join(QUALIFIER_END + ", " + QUALIFIER_BEGIN, columns.ToArray());
                _sqlTpl += QUALIFIER_END + ") VALUES (";

                List<string> fmtValues = new List<string>();
                for (int i = 0; i < _mappings.Count; i++)
                {
                    fmtValues.Add("{" + i + "}");
                }
                _sqlTpl += string.Join(",", fmtValues.ToArray()) + ")";
            }
            else
            {
                foreach (string name in obj.Keys)
                {
                    _propertyTraversalList.Add(name);
                }
                _sqlTpl = "INSERT INTO " + GetTableName() + " (" + QUALIFIER_BEGIN + string.Join (QUALIFIER_END + ", " + QUALIFIER_BEGIN, _propertyTraversalList.ToArray());
                _sqlTpl += QUALIFIER_END + ") VALUES (";

                List<string> fmtValues = new List<string>();
                for (int i = 0; i < _propertyTraversalList.Count; i++)
                {
                    fmtValues.Add("{" + i + "}");
                }
                _sqlTpl += string.Join(",", fmtValues.ToArray()) + ")";
            }
        }

        private List<string> _propertyTraversalList = null;

        private string GetTableName()
        {
            return QUALIFIER_BEGIN + this.ClassName + QUALIFIER_END;
        }

        private string GetNullValue()
        {
            return "NULL";
        }

        const string ISO8601_FMT = "yyyy-MM-ddTHH:mm:ssZ";

        private string BuildSql(FdoRow obj)
        {
            if (IsUsingPropertyMappings())
            {
                string[] values = new string[_mappings.Count];
                int i = 0;
                foreach (string target in _mappings.Keys)
                {
                    string name = _mappings[target];
                    if (obj[name] != null)
                    {
                        if (obj.IsGeometryProperty(name))
                            values[i] = VALUE_BEGIN + ((IGeometry)obj[name]).Text + VALUE_END;
                        else if (obj[name].GetType() == typeof(string))
                            values[i] = VALUE_BEGIN + EscapeValue((string)obj[name]) + VALUE_END;
                        else if (obj[name].GetType() == typeof(DateTime))
                            values[i] = VALUE_BEGIN + ((DateTime)obj[name]).ToString(ISO8601_FMT) + VALUE_END;
                        else
                            values[i] = VALUE_BEGIN + obj[name].ToString() + VALUE_END;
                    }
                    else
                    {
                        values[i] = GetNullValue();
                    }
                    i++;
                }
                return string.Format(_sqlTpl, values);
            }
            else
            {
                string[] values = new string[_propertyTraversalList.Count];
                for (int i = 0; i < values.Length; i++)
                {
                    string name = _propertyTraversalList[i];
                    if (obj[name] != null)
                        values[i] = VALUE_BEGIN + obj[name].ToString() + VALUE_END;
                    else if (obj[name].GetType() == typeof(DateTime))
                        values[i] = VALUE_BEGIN + ((DateTime)obj[name]).ToString(ISO8601_FMT) + VALUE_END;
                    else
                        values[i] = GetNullValue();
                }
                return string.Format(_sqlTpl, values);
            }
        }
    }
}
