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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Utility class to perform conversion to/from/between FDO and CLR data types
    /// </summary>
    public sealed class ValueConverter
    {
        /// <summary>
        /// Converts a FDO data type to a FDO property type
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static FdoPropertyType FromDataType(DataType dt)
        {
            //return (FdoPropertyType)Enum.Parse(typeof(FdoPropertyType), dt.ToString());
            switch (dt)
            {
                case DataType.DataType_BLOB:
                    return FdoPropertyType.BLOB;
                case DataType.DataType_Boolean:
                    return FdoPropertyType.Boolean;
                case DataType.DataType_Byte:
                    return FdoPropertyType.Byte;
                case DataType.DataType_CLOB:
                    return FdoPropertyType.CLOB;
                case DataType.DataType_DateTime:
                    return FdoPropertyType.DateTime;
                case DataType.DataType_Decimal:
                    return FdoPropertyType.Double; //FDO coerces decimals to doubles (otherwise, why is there not a GetDecimal() method in FdoIReader?)
                case DataType.DataType_Double:
                    return FdoPropertyType.Double;
                case DataType.DataType_Int16:
                    return FdoPropertyType.Int16;
                case DataType.DataType_Int32:
                    return FdoPropertyType.Int32;
                case DataType.DataType_Int64:
                    return FdoPropertyType.Int64;
                case DataType.DataType_Single:
                    return FdoPropertyType.Single;
                case DataType.DataType_String:
                    return FdoPropertyType.String;
                default:
                    throw new ArgumentException("dt");
            }
        }

        /// <summary>
        /// Gets the CLR value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static object GetClrValue(DataValue val)
        {
            if (val == null || val.IsNull())
                return null;

            switch (val.DataType)
            {
                case DataType.DataType_BLOB:
                    return ((BLOBValue)val).Data;
                case DataType.DataType_Boolean:
                    return ((BooleanValue)val).Boolean;
                case DataType.DataType_Byte:
                    return ((ByteValue)val).Byte;
                case DataType.DataType_CLOB:
                    return ((CLOBValue)val).Data;
                case DataType.DataType_DateTime:
                    return ((DateTimeValue)val).DateTime;
                case DataType.DataType_Decimal:
                    return ((DecimalValue)val).Decimal;
                case DataType.DataType_Double:
                    return ((DoubleValue)val).Double;
                case DataType.DataType_Int16:
                    return ((Int16Value)val).Int16;
                case DataType.DataType_Int32:
                    return ((Int32Value)val).Int32;
                case DataType.DataType_Int64:
                    return ((Int64Value)val).Int64;
                case DataType.DataType_Single:
                    return ((SingleValue)val).Single;
                case DataType.DataType_String:
                    return ((StringValue)val).String;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the data type
        /// </summary>
        /// <param name="pt">The <see cref="FdoPropertyType"/></param>
        /// <returns></returns>
        public static DataType? GetDataType(FdoPropertyType pt)
        {
            switch (pt)
            {
                case FdoPropertyType.BLOB:
                    return DataType.DataType_BLOB;
                case FdoPropertyType.Boolean:
                    return DataType.DataType_Boolean;
                case FdoPropertyType.Byte:
                    return DataType.DataType_Byte;
                case FdoPropertyType.CLOB:
                    return DataType.DataType_CLOB;
                case FdoPropertyType.DateTime:
                    return DataType.DataType_DateTime;
                case FdoPropertyType.Decimal:
                    return DataType.DataType_Decimal;
                case FdoPropertyType.Double:
                    return DataType.DataType_Double;
                case FdoPropertyType.Int16:
                    return DataType.DataType_Int16;
                case FdoPropertyType.Int32:
                    return DataType.DataType_Int32;
                case FdoPropertyType.Int64:
                    return DataType.DataType_Int64;
                case FdoPropertyType.Single:
                    return DataType.DataType_Single;
                case FdoPropertyType.String:
                    return DataType.DataType_String;
            }
            return null;
        }

        /// <summary>
        /// Gets the property type
        /// </summary>
        /// <param name="dt">The data type</param>
        /// <returns></returns>
        public static FdoPropertyType GetPropertyType(DataType dt)
        {
            switch (dt)
            {
                case DataType.DataType_BLOB:
                    return FdoPropertyType.BLOB;
                case DataType.DataType_Boolean:
                    return FdoPropertyType.Boolean;
                case DataType.DataType_Byte:
                    return FdoPropertyType.Byte;
                case DataType.DataType_CLOB:
                    return FdoPropertyType.CLOB;
                case DataType.DataType_DateTime:
                    return FdoPropertyType.DateTime;
                case DataType.DataType_Decimal:
                    return FdoPropertyType.Decimal;
                case DataType.DataType_Double:
                    return FdoPropertyType.Double;
                case DataType.DataType_Int16:
                    return FdoPropertyType.Int16;
                case DataType.DataType_Int32:
                    return FdoPropertyType.Int32;
                case DataType.DataType_Int64:
                    return FdoPropertyType.Int64;
                case DataType.DataType_Single:
                    return FdoPropertyType.Single;
                case DataType.DataType_String:
                    return FdoPropertyType.String;
            }
            throw new ArgumentException("dt");
        }

        public static LiteralValue GetConvertedValue(object value, DataType? expectedDataType)
        {
            var lv = GetConvertedValue(value);
            if (lv.LiteralValueType == LiteralValueType.LiteralValueType_Data)
            {
                var dv = (DataValue)lv;
                if (dv != null && dv.DataType != expectedDataType)
                {
                    //"Wash" the value back through FDO, which provides DataValue conversion support
                    //TODO: Surface conversion settings up to the caller, but for now assume defaults
                    switch (expectedDataType)
                    {
                        case DataType.DataType_BLOB:
                            return new BLOBValue(dv);
                        case DataType.DataType_Boolean:
                            return new BooleanValue(dv);
                        case DataType.DataType_Byte:
                            return new ByteValue(dv);
                        case DataType.DataType_CLOB:
                            return new CLOBValue(dv);
                        case DataType.DataType_DateTime:
                            return new DateTimeValue(dv);
                        case DataType.DataType_Decimal:
                            return new DecimalValue(dv);
                        case DataType.DataType_Double:
                            return new DoubleValue(dv);
                        case DataType.DataType_Int16:
                            return new Int16Value(dv);
                        case DataType.DataType_Int32:
                            return new Int32Value(dv);
                        case DataType.DataType_Int64:
                            return new Int64Value(dv);
                        case DataType.DataType_Single:
                            return new SingleValue(dv);
                        case DataType.DataType_String:
                            return new StringValue(dv);
                    }
                }
            }
            return lv;
        }

        /// <summary>
        /// Gets a FDO literal value type from a CLR type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LiteralValue GetConvertedValue(object value)
        {
            Type t = value.GetType();
            if (t == typeof(byte[]))
            {
                //BLOB
                //CLOB
                return new BLOBValue((byte[])value);
            }
            if (t == typeof(bool))
            {
                return new BooleanValue((bool)value);
            }
            if (t == typeof(byte))
            {
                return new ByteValue((byte)value);
            }
            if (t == typeof(DateTime))
            { 
                return new DateTimeValue((DateTime)value);
            }
            if (t == typeof(Decimal))
            {
                return new DoubleValue(Convert.ToDouble(value));
            }
            if (t == typeof(Double))
            {
                return new DoubleValue((double)value);
            }
            if (t == typeof(Int16))
            {
                return new Int16Value((Int16)value);
            }
            if (t == typeof(Int32))
            {
                return new Int32Value((Int32)value);
            }
            if (t == typeof(Int64))
            {
                return new Int64Value((Int64)value);
            }
            if (t == typeof(Single))
            {
                return new SingleValue((Single)value);
            }
            if (t == typeof(String))
            {
                return new StringValue((String)value);
            }

            IGeometry geom = value as IGeometry;
            if (geom != null)
            {
                return new GeometryValue(FdoGeometryFactory.Instance.GetFgf(geom));
            }

            return null;
        }

        /// <summary>
        /// Converts a source DataValue to a DataValue of the given type. Incompatible
        /// values are converted to null and values outside the valid range are truncated
        /// </summary>
        /// <param name="src">The source data value</param>
        /// <param name="dataType">The data type to convert to</param>
        /// <returns>The converted data value</returns>
        public static DataValue ConvertDataValue(DataValue src, DataType dataType)
        {
            return ConvertDataValue(src, dataType, true, true);
        }

        /// <summary>
        /// Converts a source DataValue to a DataValue of the given type
        /// </summary>
        /// <param name="src">The source data value</param>
        /// <param name="dataType">The data type to convert to</param>
        /// <param name="nullIfIncompatible">Determines what happens if the source and destination types are incompatible</param>
        /// <param name="truncate">Determines what happens if the value is outside the valid range for the destination type (e.g. convert 1000000 from FdoInt32Value to FdoInt16Value). Applicable only when both source and destination types are one of Boolean, Byte, Decimal, Double, Int16, Int32, Int64 or Single</param>
        /// <returns>The converted data value</returns>
        public static DataValue ConvertDataValue(DataValue src, DataType dataType, bool nullIfIncompatible, bool truncate)
        {
            //Calling code should not be passing in null data value in the first place
            if (src == null || src.IsNull())
                throw new ArgumentNullException("src");

            if (src.DataType == dataType)
                return src;

            //Get this out of the way, throw if source cannot be converted to target and not
            //using null coercion.
            if (!nullIfIncompatible && !IsConvertible(src.DataType, dataType))
                throw new FdoException("Unable to convert source data type <" + src.DataType + "> to <" + dataType + ">");

            //assert nullIfIncompatible == true
            switch (src.DataType)
            {
                case DataType.DataType_BLOB:
                    {
                        return null;
                    }
                case DataType.DataType_Boolean:
                    {
                        return ConvertBoolean((BooleanValue)src, dataType);
                    }
                case DataType.DataType_Byte:
                    {
                        return ConvertByte((ByteValue)src, dataType);
                    }
                case DataType.DataType_CLOB:
                    {
                        return null;
                    }
                case DataType.DataType_DateTime:
                    {
                        return ConvertDateTime((DateTimeValue)src, dataType);
                    }
                case DataType.DataType_Decimal:
                    {
                        return ConvertDecimal((DecimalValue)src, dataType, truncate);
                    }
                case DataType.DataType_Double:
                    {
                        return ConvertDouble((DoubleValue)src, dataType, truncate);
                    }
                case DataType.DataType_Int16:
                    {
                        return ConvertInt16((Int16Value)src, dataType, truncate);
                    }
                case DataType.DataType_Int32:
                    {
                        return ConvertInt32((Int32Value)src, dataType, truncate);
                    }
                case DataType.DataType_Int64:
                    {
                        return ConvertInt64((Int64Value)src, dataType, truncate);
                    }
                case DataType.DataType_Single:
                    {
                        return ConvertSingle((SingleValue)src, dataType, truncate);
                    }
                case DataType.DataType_String:
                    {
                        return ConvertString((StringValue)src, dataType, truncate);
                    }
            }

            return null;
        }

        private static DataValue ConvertDouble(DoubleValue doubleValue, DataType dataType, bool truncate)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    return new StringValue(doubleValue.Double.ToString());
                case DataType.DataType_Single:
                    {
                        double d = doubleValue.Double;
                        try
                        {
                            float f = Convert.ToSingle(d);
                            return new SingleValue(f);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > float.MaxValue)
                                    return new SingleValue(float.MaxValue);
                                else if (d < float.MinValue)
                                    return new SingleValue(float.MinValue);
                                else
                                    return new SingleValue((float)d);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_Decimal:
                    {
                        return new DecimalValue(doubleValue.Double);
                    }
                case DataType.DataType_Int16:
                    {
                        double d = doubleValue.Double;
                        try
                        {
                            short f = Convert.ToInt16(d);
                            return new Int16Value(f);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > short.MaxValue)
                                    return new Int16Value(short.MaxValue);
                                else if (d < short.MinValue)
                                    return new Int16Value(short.MinValue);
                                else
                                    return new Int16Value((short)d);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_Int32:
                    {
                        double d = doubleValue.Double;
                        try
                        {
                            int f = Convert.ToInt32(d);
                            return new Int32Value(f);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > int.MaxValue)
                                    return new Int32Value(int.MaxValue);
                                else if (d < int.MinValue)
                                    return new Int32Value(int.MinValue);
                                else
                                    return new Int32Value((int)d);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_Int64:
                    {
                        double d = doubleValue.Double;
                        try
                        {
                            long f = Convert.ToInt64(d);
                            return new Int64Value(f);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > long.MaxValue)
                                    return new Int64Value(long.MaxValue);
                                else if (d < long.MinValue)
                                    return new Int64Value(long.MinValue);
                                else
                                    return new Int64Value((long)d);
                            }
                        }
                        return null;
                    }
                default:
                    return null;
            }
        }

        private static DataValue ConvertInt16(Int16Value int16Value, DataType dataType, bool truncate)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    return new StringValue(int16Value.Int16.ToString());
                case DataType.DataType_Single:
                    return new SingleValue(Convert.ToSingle(int16Value.Int16));
                case DataType.DataType_Double:
                    return new DoubleValue(Convert.ToDouble(int16Value.Int16));
                case DataType.DataType_Decimal:
                    return new DecimalValue(Convert.ToDouble(int16Value.Int16));
                case DataType.DataType_Int32:
                    return new Int32Value(Convert.ToInt32(int16Value.Int16));
                case DataType.DataType_Int64:
                    return new Int64Value(Convert.ToInt64(int16Value.Int16));
                default:
                    return null;
            }
        }

        private static DataValue ConvertInt32(Int32Value int32Value, DataType dataType, bool truncate)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    {
                        return new StringValue(int32Value.Int32.ToString());
                    }
                case DataType.DataType_Single:
                    {
                        int i = int32Value.Int32;
                        try
                        {
                            float f = Convert.ToSingle(i);
                            return new SingleValue(f);
                        }
                        catch (OverflowException)
                        {
                            if (i > float.MaxValue)
                                return new SingleValue(float.MaxValue);
                            else if (i < float.MinValue)
                                return new SingleValue(float.MinValue);
                            else
                                return new SingleValue((float)i);
                        }
                    }
                case DataType.DataType_Double:
                    {
                        int i = int32Value.Int32;
                        try
                        {
                            double d = Convert.ToDouble(i);
                            return new DoubleValue(d);
                        }
                        catch (OverflowException)
                        {
                            if (i > double.MaxValue)
                                return new DoubleValue(double.MaxValue);
                            else if (i < double.MinValue)
                                return new DoubleValue(double.MinValue);
                            else
                                return new DoubleValue((double)i);
                        }
                    }
                case DataType.DataType_Decimal:
                    {
                        return new DecimalValue(Convert.ToDouble(int32Value.Int32));
                    }
                case DataType.DataType_Int16:
                    {
                        int i = int32Value.Int32;
                        try
                        {
                            short f = Convert.ToInt16(i);
                            return new Int16Value(f);
                        }
                        catch (OverflowException)
                        {
                            if (i > short.MaxValue)
                                return new Int16Value(short.MaxValue);
                            else if (i < short.MinValue)
                                return new Int16Value(short.MinValue);
                            else
                                return new Int16Value((short)i);
                        }
                    }
                case DataType.DataType_Int64:
                    {
                        return new Int64Value(Convert.ToInt64(int32Value.Int32));
                    }
                default:
                    return null;
            }
        }

        private static DataValue ConvertInt64(Int64Value int64Value, DataType dataType, bool truncate)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    {
                        return new StringValue(int64Value.Int64.ToString());
                    }
                case DataType.DataType_Single:
                    {
                        long l = int64Value.Int64;
                        try
                        {
                            float f = Convert.ToSingle(l);
                            return new SingleValue(f);
                        }
                        catch (OverflowException)
                        {
                            if (l > float.MaxValue)
                                return new SingleValue(float.MaxValue);
                            else if (l < float.MinValue)
                                return new SingleValue(float.MinValue);
                            else
                                return new SingleValue((float)l);
                        }
                    }
                case DataType.DataType_Double:
                    {
                        long l = int64Value.Int64;
                        try
                        {
                            double d = Convert.ToDouble(l);
                            return new DoubleValue(d);
                        }
                        catch (OverflowException)
                        {
                            if (l > double.MaxValue)
                                return new DoubleValue(double.MaxValue);
                            else if (l < double.MinValue)
                                return new DoubleValue(double.MinValue);
                            else
                                return new DoubleValue((double)l);
                        }
                    }
                case DataType.DataType_Decimal:
                    {
                        if (truncate)
                            return new DecimalValue(Convert.ToDouble(int64Value.Int64));
                        else
                            return null;
                    }
                case DataType.DataType_Int16:
                    {
                        long l = int64Value.Int64;
                        try
                        {
                            short d = Convert.ToInt16(l);
                            return new Int16Value(d);
                        }
                        catch (OverflowException)
                        {
                            if (l > short.MaxValue)
                                return new Int16Value(short.MaxValue);
                            else if (l < short.MinValue)
                                return new Int16Value(short.MinValue);
                            else
                                return new Int16Value((short)l);
                        }
                    }
                case DataType.DataType_Int32:
                    {
                        long l = int64Value.Int64;
                        try
                        {
                            int d = Convert.ToInt32(l);
                            return new Int32Value(d);
                        }
                        catch (OverflowException)
                        {
                            if (l > int.MaxValue)
                                return new Int32Value(int.MaxValue);
                            else if (l < int.MinValue)
                                return new Int32Value(int.MinValue);
                            else
                                return new Int32Value((int)l);
                        }
                    }
                default:
                    return null;
            }
        }
        
        private static DataValue ConvertSingle(SingleValue singleValue, DataType dataType, bool truncate)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    {
                        return new StringValue(singleValue.ToString());
                    }
                case DataType.DataType_Double:
                    {
                        return new DoubleValue(Convert.ToDouble(singleValue.Single));
                    }
                case DataType.DataType_Decimal:
                    {
                        return new DecimalValue(Convert.ToDouble(singleValue.Single));
                    }
                case DataType.DataType_Int16:
                    {
                        float f = singleValue.Single;
                        try
                        {
                            short sh = Convert.ToInt16(f);
                            return new Int16Value(sh);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (f > (float)short.MaxValue)
                                    return new Int16Value(short.MaxValue);
                                else if (f < (float)short.MinValue)
                                    return new Int16Value(short.MinValue);
                                else
                                    return new Int16Value((short)f);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_Int32:
                    {
                        if (truncate)
                            return new Int32Value(Convert.ToInt32(singleValue.Single));
                        else
                            return null;
                    }
                case DataType.DataType_Int64:
                    {
                        if (truncate)
                            return new Int64Value(Convert.ToInt64(singleValue.Single));
                        else
                            return null;
                    }
                default:
                    return null;
            }
        }

        private static DataValue ConvertString(StringValue stringValue, DataType dataType, bool truncate)
        {
            switch (dataType)
            {
                case DataType.DataType_Boolean:
                    {
                        string val = stringValue.String.ToLower();
                        if (val == true.ToString() || val == false.ToString())
                            return new BooleanValue(Convert.ToBoolean(val));
                        else
                            return null;
                    }
                case DataType.DataType_Byte:
                    {
                        byte b;
                        if (byte.TryParse(stringValue.String, out b))
                            return new ByteValue(b);
                        else
                            return null;
                    }
                case DataType.DataType_DateTime:
                    {
                        DateTime dt;
                        if (DateTime.TryParse(stringValue.String, out dt))
                            return new DateTimeValue(dt);
                        else
                            return null;
                    }
                case DataType.DataType_Decimal:
                    {
                        double d;
                        if (double.TryParse(stringValue.String, out d))
                            return new DecimalValue(d);
                        else
                            return null;
                    }
                case DataType.DataType_Double:
                    {
                        double d;
                        if (double.TryParse(stringValue.String, out d))
                            return new DoubleValue(d);
                        else
                            return null;
                    }
                case DataType.DataType_Int16:
                    {
                        short s;
                        if (short.TryParse(stringValue.String, out s))
                        {
                            return new Int16Value(s);
                        }
                        else
                        {
                            //Try as double first before we bail
                            double d;
                            if (double.TryParse(stringValue.String, out d))
                                return new Int16Value(Convert.ToInt16(d));
                            return null;
                        }
                    }
                case DataType.DataType_Int32:
                    {
                        int i;
                        if (int.TryParse(stringValue.String, out i))
                        {
                            //Try as double first before we bail
                            double d;
                            if (double.TryParse(stringValue.String, out d))
                                return new Int32Value(Convert.ToInt32(d));
                            return null;
                        }
                        else
                        {
                            return null;
                        }
                    }
                case DataType.DataType_Int64:
                    {
                        long l;
                        if (long.TryParse(stringValue.String, out l))
                        {
                            return new Int64Value(l);
                        }
                        else
                        {
                            //Try as double first before we bail
                            double d;
                            if (double.TryParse(stringValue.String, out d))
                                return new Int64Value(Convert.ToInt64(d));
                            return null;
                        }
                    }
                case DataType.DataType_Single:
                    {
                        float f;
                        if (float.TryParse(stringValue.String, out f))
                            return new SingleValue(f);
                        else
                            return null;
                    }
                default:
                    return null;
            }
        }

        private static DataValue ConvertDecimal(DecimalValue src, DataType dataType, bool truncate)
        {
            switch (dataType)
            {
                case DataType.DataType_Byte:
                    {
                        double d = src.Decimal;
                        try
                        {
                            byte b = Convert.ToByte(d);
                            return new ByteValue(b);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > (double)byte.MaxValue)
                                    return new ByteValue(byte.MaxValue);
                                else if (d < (double)byte.MinValue)
                                    return new ByteValue(byte.MinValue);
                                else
                                    return new ByteValue((byte)d);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_String:
                    return new StringValue(src.Decimal.ToString());
                case DataType.DataType_Single:
                    {
                        double d = src.Decimal;
                        try
                        {
                            float f = Convert.ToSingle(d);
                            return new SingleValue(f);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > (double)float.MaxValue)
                                    return new SingleValue(float.MaxValue);
                                else if (d < (double)float.MinValue)
                                    return new SingleValue(float.MinValue);
                                else
                                    return new SingleValue((float)d);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_Double:
                    return new DoubleValue(src.Decimal);
                case DataType.DataType_Int16:
                    {
                        double d = src.Decimal;
                        try
                        {
                            short s = Convert.ToInt16(d);
                            return new Int16Value(s);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > (double)Int16.MaxValue)
                                    return new Int16Value(Int16.MaxValue);
                                else if (d < (double)Int16.MinValue)
                                    return new Int16Value(Int16.MinValue);
                                else
                                    return new Int16Value((short)d);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_Int32:
                    {
                        double d = src.Decimal;
                        try
                        {
                            int i = Convert.ToInt32(d);
                            return new Int32Value(i);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > (double)Int32.MaxValue)
                                    return new Int32Value(Int32.MaxValue);
                                else if (d < (double)Int32.MinValue)
                                    return new Int32Value(Int32.MinValue);
                                else
                                    return new Int32Value((int)d);
                            }
                        }
                        return null;
                    }
                case DataType.DataType_Int64:
                    {
                        double d = src.Decimal;
                        try
                        {
                            long l = Convert.ToInt64(d);
                            return new Int64Value(l);
                        }
                        catch (OverflowException)
                        {
                            if (truncate)
                            {
                                if (d > (double)Int64.MaxValue)
                                    return new Int64Value(Int64.MaxValue);
                                else if (d < (double)Int16.MinValue)
                                    return new Int64Value(Int64.MinValue);
                                else
                                    return new Int64Value((long)d);
                            }
                        }
                        return null;
                    }
                default:
                    return null;
            }
        }

        private static DataValue ConvertDateTime(DateTimeValue src, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    return new StringValue(src.DateTime.ToString());
                default:
                    return null;
            }
        }

        private static DataValue ConvertByte(ByteValue src, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    return new StringValue(src.Byte.ToString());
                case DataType.DataType_Int16:
                    return new Int16Value(Convert.ToInt16(src.Byte));
                case DataType.DataType_Int32:
                    return new Int32Value(Convert.ToInt32(src.Byte));
                case DataType.DataType_Int64:
                    return new Int64Value(Convert.ToInt64(src.Byte));
                case DataType.DataType_Single:
                    return new SingleValue(Convert.ToSingle(src.Byte));
                case DataType.DataType_Decimal:
                    return new DecimalValue(Convert.ToDouble(src.Byte));
                case DataType.DataType_Double:
                    return new DoubleValue(Convert.ToDouble(src.Byte));
                default:
                    return null;
            }
        }

        private static DataValue ConvertBoolean(BooleanValue src, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.DataType_String:
                    return new StringValue(src.Boolean.ToString());
                case DataType.DataType_Byte:
                    return new ByteValue(Convert.ToByte(src.Boolean));
                case DataType.DataType_Int16:
                    return new Int16Value(Convert.ToInt16(src.Boolean));
                case DataType.DataType_Int32:
                    return new Int32Value(Convert.ToInt32(src.Boolean));
                case DataType.DataType_Int64:
                    return new Int64Value(Convert.ToInt64(src.Boolean));
                default:
                    return null;
            }
        }

        /// <summary>
        /// Determines if one data type can be converted to another
        /// </summary>
        /// <param name="src">The source data type</param>
        /// <param name="dest">The target data type</param>
        /// <returns>True if the data types are convertible. False otherwise</returns>
        public static bool IsConvertible(DataType src, DataType dest)
        {
            if (src == dest)
                return true;

            switch (src)
            {
                case DataType.DataType_BLOB:
                    {
                        return false;
                    }
                case DataType.DataType_Boolean:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Boolean:
                            case DataType.DataType_String:
                            case DataType.DataType_Byte:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_Byte:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Byte:
                            case DataType.DataType_String:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                            case DataType.DataType_Single:
                            case DataType.DataType_Decimal:
                            case DataType.DataType_Double:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_CLOB:
                    {
                        return false;
                    }
                case DataType.DataType_DateTime:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_DateTime:
                            case DataType.DataType_String:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_Decimal:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Byte:
                            case DataType.DataType_Decimal:
                            case DataType.DataType_String:
                            case DataType.DataType_Single:
                            case DataType.DataType_Double:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_Double:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Byte:
                            case DataType.DataType_Decimal:
                            case DataType.DataType_String:
                            case DataType.DataType_Single:
                            case DataType.DataType_Double:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_Int16:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Byte:
                            case DataType.DataType_Decimal:
                            case DataType.DataType_String:
                            case DataType.DataType_Single:
                            case DataType.DataType_Double:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_Int32:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Byte:
                            case DataType.DataType_Decimal:
                            case DataType.DataType_String:
                            case DataType.DataType_Single:
                            case DataType.DataType_Double:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_Int64:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Byte:
                            case DataType.DataType_Decimal:
                            case DataType.DataType_String:
                            case DataType.DataType_Single:
                            case DataType.DataType_Double:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_Single:
                    {
                        switch (dest)
                        {
                            case DataType.DataType_Byte:
                            case DataType.DataType_Decimal:
                            case DataType.DataType_String:
                            case DataType.DataType_Single:
                            case DataType.DataType_Double:
                            case DataType.DataType_Int16:
                            case DataType.DataType_Int32:
                            case DataType.DataType_Int64:
                                return true;
                            default:
                                return false;
                        }
                    }
                case DataType.DataType_String:
                    {
                        switch (dest)
                        { 
                            case DataType.DataType_BLOB:
                            case DataType.DataType_CLOB:
                                return false;
                            default:
                                return true;
                        }
                        
                    }
            }

            return false;
        }
    }
}
