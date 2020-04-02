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
using System.Text;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Connections.Capabilities;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Utility
{
    /// <summary>
    /// Helper class for FDO Expressions
    /// </summary>
    public sealed class ExpressionUtility
    {
        /// <summary>
        /// Parses the type of the expression.
        /// </summary>
        /// <param name="exprStr">The expr STR.</param>
        /// <param name="conn">The conn.</param>
        /// <returns></returns>
        public static FdoPropertyType? ParseExpressionType(string exprStr, FdoConnection conn)
        {
            Expression expr = null;
            try
            {
                expr = Expression.Parse(exprStr);
            }
            catch (OSGeo.FDO.Common.Exception)
            {
                return null;
            }

            if (expr.GetType() == typeof(Function))
            {
                Function func = expr as Function;
                FunctionDefinitionCollection funcDefs = (FunctionDefinitionCollection)conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_ExpressionFunctions);
                FunctionDefinition funcDef = null;

                //Try to get the return type
                foreach (FunctionDefinition fd in funcDefs)
                {
                    if (fd.Name == func.Name)
                    {
                        funcDef = fd;
                        break;
                    }
                }

                if (funcDef == null)
                    return null;

                switch (funcDef.ReturnPropertyType)
                {
                    case PropertyType.PropertyType_AssociationProperty:
                        return FdoPropertyType.Association;
                    case PropertyType.PropertyType_GeometricProperty:
                        return FdoPropertyType.Geometry;
                    case PropertyType.PropertyType_ObjectProperty:
                        return FdoPropertyType.Object;
                    case PropertyType.PropertyType_RasterProperty:
                        return FdoPropertyType.Raster;
                    case PropertyType.PropertyType_DataProperty:
                        {
                            return ValueConverter.GetPropertyType(funcDef.ReturnType);
                        }
                }
            }
            else if (expr.GetType() == typeof(BinaryExpression))
            {
                return FdoPropertyType.Boolean;
            }
            else if (expr.GetType() == typeof(DataValue))
            {
                DataValue dv = (DataValue)expr;
                return ValueConverter.GetPropertyType(dv.DataType);
            }
            return null;
        }
    }
}
