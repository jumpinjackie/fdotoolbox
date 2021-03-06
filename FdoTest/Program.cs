﻿#region LGPL Header
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
using OSGeo.MapGuide;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

            var thisDir = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            var resDir = Path.Combine(thisDir, "Resources");
            FoundationApi.MgInitializeLibrary(resDir, "en");
            //Set up CS-Map
            var dictPath = Path.Combine(thisDir, "Dictionaries");
            Environment.SetEnvironmentVariable("MENTOR_DICTIONARY_PATH", dictPath);
            MgCoordinateSystemFactory fact = new MgCoordinateSystemFactory();
            MgCoordinateSystemCatalog cat = fact.GetCatalog();
            cat.SetDictionaryDir(dictPath);

            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "FDO");
            FdoAssemblyResolver.InitializeFdo(path);

            InvokeTest(Test_TestFrameworkDogfood);
            //InvokeTest(SchemaTests.Test_SchemaCloning);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_Point);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_LineString);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_Polygon);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_PolygonWithHole);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_MultiPoint);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_MultiLineString);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_MultiPolygon);
            InvokeTest(GeometryTests.Test_GeometryConverterContract_GeometryCollection);
            InvokeTest(EtlTests.Test_ETL_SdfToSdf);
            InvokeTest(EtlTests.Test_ETL_SdfToSqlite);
            InvokeTest(EtlTests.Test_ETL_SdfToSdf_WebMercator);
            InvokeTest(EtlTests.Test_ETL_SdfToSqlite_WebMercator);

            Console.WriteLine("===============================");
            Console.WriteLine("Test Summary:");
            Console.WriteLine($"{_testsRun} run, {_failedTests} failed, {_erroredTests} errored");

            FoundationApi.MgUninitializeLibrary();

            System.GC.Collect();

            return _failedTests + _erroredTests;
        }

        static void InvokeTest(Action suite)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                _testsRun++;
                Console.WriteLine($">> Executing test: {suite.Method.Name}");
                suite.Invoke();
                sw.Stop();
                Console.WriteLine($">>    Test Result: OK ({sw.ElapsedMilliseconds} ms)");
            }
            catch (AssertException ex)
            {
                sw.Stop();
                _failedTests++;
                Console.WriteLine($">>    Test Result: Failed ({sw.ElapsedMilliseconds} ms)");
                Console.WriteLine($">>    Message was:");
                Console.WriteLine(ex.Message);
                Console.WriteLine($"  {ex.Caller}");
            }
            catch
            {
                sw.Stop();
                _erroredTests++;
                Console.WriteLine($">>    Test Result: Errored ({sw.ElapsedMilliseconds} ms)");
            }
            finally
            {
                //HACK: If we don't do this we may hit an access violation when the GC claims a
                //FDO .net class proxy
                System.GC.Collect();
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
