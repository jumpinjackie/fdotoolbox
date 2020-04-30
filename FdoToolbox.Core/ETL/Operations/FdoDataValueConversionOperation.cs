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
using System.Collections.Generic;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Expression;
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// An ETL operation that converts FDO data values
    /// </summary>
    public class FdoDataValueConversionOperation : FdoOperationBase
    {
        private Dictionary<string, FdoDataPropertyConversionRule> _rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoDataValueConversionOperation"/> class.
        /// </summary>
        /// <param name="rules">The rules.</param>
        public FdoDataValueConversionOperation(IEnumerable<FdoDataPropertyConversionRule> rules)
        {
            _rules = new Dictionary<string, FdoDataPropertyConversionRule>();
            foreach (FdoDataPropertyConversionRule rule in rules)
            {
                _rules[rule.SourceProperty] = rule;
            }
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoRow row in rows)
            {
                yield return ConvertValues(row);
            }
        }

        private FdoRow ConvertValues(FdoRow row)
        {
            foreach(string propertyName in _rules.Keys)
            {
                if (row[propertyName] != null)
                {
                    FdoDataPropertyConversionRule rule = _rules[propertyName];
                    LiteralValue old = ValueConverter.GetConvertedValue(row[propertyName]);
                    if (old.LiteralValueType == LiteralValueType.LiteralValueType_Data)
                    {
                        DataValue converted = ValueConverter.ConvertDataValue((DataValue)old, rule.TargetDataType, rule.NullOnFailure, rule.Truncate);
                        row[propertyName] = ValueConverter.GetClrValue(converted);
                        if (converted != null)
                        {
                            converted.Dispose();
                        }
                        else
                        {
                            if (!rule.NullOnFailure)
                                throw new FdoException("Converting " + old + " to " + rule.TargetDataType + " resulted in a NULL value");
                        }
                        old.Dispose();
                    }
                }
            }
            return row;
        }
    }
}
