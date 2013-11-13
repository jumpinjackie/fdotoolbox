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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Utility;
using NUnit.Framework;
using System.Collections.Specialized;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;
using System.IO;

namespace FdoToolbox.Core.Tests
{
    [TestFixture]
    public class ExpressUtilityTests
    {
        [Test]
        public void TestCreateFlatFile()
        {
            Assert.IsTrue(ExpressUtility.CreateFlatFileDataSource("Test.sdf"));
            Assert.IsTrue(ExpressUtility.CreateFlatFileDataSource("Test.sqlite"));
            Assert.IsTrue(ExpressUtility.CreateFlatFileDataSource("Test.db"));

            File.Delete("Test.sdf");
            File.Delete("Test.sqlite");
            File.Delete("Test.db");
        }

        [Test]
        public void TestStringToNvc()
        {
            string str = "Username=Foo;Password=Bar;Service=localhost";
            NameValueCollection nvc = ExpressUtility.ConvertFromString(str);

            Assert.AreEqual(nvc["Username"], "Foo");
            Assert.AreEqual(nvc["Password"], "Bar");
            Assert.AreEqual(nvc["Service"], "localhost");
        }

        [Test]
        public void TestNvcToString()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc["Username"] = "Foo";
            nvc["Password"] = "Bar";
            nvc["Service"] = "localhost";
            string str = ExpressUtility.ConvertFromNameValueCollection(nvc);
            Assert.AreEqual(str, "Username=Foo;Password=Bar;Service=localhost");
        }
    }
}
