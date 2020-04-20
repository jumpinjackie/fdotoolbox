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

using FdoToolbox.Core;
using System;
using System.IO;

namespace FdoTest
{
    class Program
    {
        static int _testsRun;
        static int _failedTests;
        static int _erroredTests;

        static int Main(string[] args)
        {
            Console.WriteLine("FDO Toolbox test runner");

            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "FDO");
            FdoAssemblyResolver.InitializeFdo(path);

            InvokeTest(Test_TestFrameworkDogfood);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_Point);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_LineString);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_Polygon);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_PolygonWithHole);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_MultiPoint);

            Console.WriteLine("===============================");
            Console.WriteLine("Test Summary:");
            Console.WriteLine($"{_testsRun} run, {_failedTests} failed, {_erroredTests} errored");

            return _failedTests + _erroredTests;
        }

        static void InvokeTest(Action suite)
        {
            try
            {
                _testsRun++;
                Console.WriteLine($">> Executing test: {suite.Method.Name}");
                suite.Invoke();
                Console.WriteLine(">>    Test Result: OK");
            }
            catch (AssertException)
            {
                _failedTests++;
                Console.WriteLine(">>    Test Result: Failed");
            }
            catch
            {
                _erroredTests++;
                Console.WriteLine(">>    Test Result: Errored");
            }
        }

        static void Test_TestFrameworkDogfood()
        {
            Assert.Equal(1, 1);
            Assert.Equal(1, 1.0);
            Assert.Equal(1.0, 1);
            Assert.Equal(1, 1.0f);
            Assert.Equal(1.0f, 1);
            try
            {
                Assert.Equal(1, 2);
            }
            catch (AssertException ex)
            {
                Assert.Equal($"Expected: {1}, Got: {2}", ex.Message);
            }
            Assert.Throws<AssertException>(() => Assert.Equal(1, 2));
            try
            {
                Assert.Throws<NotSupportedException>(() => Assert.Equal(1, 1));
            }
            catch (AssertException ex)
            {
                Assert.Equal($"Expected exception of type {typeof(NotSupportedException)} to be thrown", ex.Message);
            }
            try
            {
                Assert.Fail("This is not supposed to happen");
            }
            catch (AssertException ex)
            {
                Assert.Equal("This is not supposed to happen", ex.Message);
            }
        }
    }
}
