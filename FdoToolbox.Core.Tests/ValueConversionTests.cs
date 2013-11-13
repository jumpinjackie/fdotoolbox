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
using NUnit.Framework;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.Tests
{
    [TestFixture]
    public class ValueConversionTests
    {
        [Test]
        public void TestBooleanConversion()
        {
            BooleanValue b = new BooleanValue(true);
            DataValue dv = null;

            //To BLOB
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_BLOB, true, true);
            Assert.IsNull(dv);

            //To Boolean
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Boolean, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Boolean);
            Assert.AreEqual(dv, b);

            dv = null;
            //To Byte
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Byte, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Byte);
            
            //To CLOB
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_CLOB, true, true);
            Assert.IsNull(dv);

            //To DateTime
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_DateTime, true, true);
            Assert.IsNull(dv);

            //To Decimal
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Decimal, true, true);
            Assert.IsNull(dv);

            //To Double
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Double, true, true);
            Assert.IsNull(dv);

            dv = null;
            //To String
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_String, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_String);

            dv = null;
            //To Int16
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Int16, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Int16);

            dv = null;
            //To Int32
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Int32, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Int32);

            dv = null;
            //To Int64
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Int64, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Int64);
        }

        [Test]
        public void TestBLOBConversion()
        {
            Assert.Fail("Not implemented");   
        }

        [Test]
        public void TestByteConversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestCLOBConversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestDateTimeConversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestDecimalConversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestDoubleConversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestInt16Conversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestInt32Conversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestInt64Conversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestSingleConversion()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestStringConversion()
        {
            Assert.Fail("Not implemented");
        }
    }
}
