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

using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Schema;
using System.Diagnostics;
using System.IO;

namespace FdoTest
{
    public static class EtlTests
    {
        static (FdoConnection source, FdoConnection target) TestETLBase(
            string testName,
            string srcProvider,
            string srcPath,
            string srcSchema,
            string srcClassName,
            string dstProvider,
            string dstExtension,
            string srcFileParam = "File",
            string dstFileParam = "File",
            string targetCsWkt = null)
        {
            Debug.WriteLine("Creating source connection");
            var conn = new FdoConnection(srcProvider, srcFileParam + "=" + srcPath);
            Debug.WriteLine("Created source connection. Testing if we can open it");
            Assert.Equal(FdoConnectionState.Open, conn.Open());
            FdoConnection targetConn = null;
            var targetPath = $"TestData/{testName}.{dstExtension}";

            Debug.WriteLine($"Deleting {targetPath} if it already exists");
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
                Debug.WriteLine($"Deleted {targetPath}");
            }

            Debug.WriteLine($"Checking source has spatial contexts");
            //The thing we're copying from must have a spatial context
            using (var ssvc = conn.CreateFeatureService())
            {
                var sourceScs = ssvc.GetSpatialContexts();
                Assert.NotEmpty(sourceScs);
            }

            Debug.WriteLine($"Creating the bulk copy");
            //Create and run the bulk copy
            using (var bcp = ExpressUtility.CreateBulkCopyForFeatureClass(conn, srcSchema, srcClassName, dstProvider, targetPath, targetCsWkt))
            {
                bcp.ProcessMessage += Bcp_ProcessMessage;
                try
                {
                    Debug.WriteLine("Executing bulk copy");
                    bcp.Execute();
                }
                finally
                {
                    bcp.ProcessMessage -= Bcp_ProcessMessage;
                }
            }

            //Basic checks

            //1. Target file exists?
            Assert.Equal(true, File.Exists(targetPath));
            //2. We can make a FDO connection to it
            targetConn = new FdoConnection(dstProvider, dstFileParam + "=" + targetPath);
            Assert.Equal(FdoConnectionState.Open, targetConn.Open());
            //3. Basic select * query returns same total
            using (var ssvc = conn.CreateFeatureService())
            using (var tsvc = targetConn.CreateFeatureService())
            {
                var sTotal = ssvc.GetFeatureCount(srcClassName, null, true);
                var tTotal = tsvc.GetFeatureCount(srcClassName, null, true);

                Assert.Equal(sTotal, tTotal);

                var tgtClass = tsvc.GetClassByName(srcClassName);
                Assert.NotNull(tgtClass);

                //4. Target has a spatial context that is referenced by the target class's geom property
                if (tgtClass is FeatureClass fc)
                {
                    var geom = fc.GeometryProperty;
                    var tsc = tsvc.GetSpatialContext(geom.SpatialContextAssociation);
                    Assert.NotNull(tsc);
                }
                else
                {
                    Assert.Fail("Expected target class to be the feature class");
                }

                //5. It is only *the* spatial context
                var targetScs = tsvc.GetSpatialContexts();
                Assert.Equal(1, targetScs.Count);
            }
            return (conn, targetConn);
        }

        private static void Bcp_ProcessMessage(object sender, FdoToolbox.Core.MessageEventArgs e)
        {
            Debug.WriteLine(e.Message);
        }

        public static void Test_ETL_SdfToSqlite()
        {
            Debug.WriteLine($"Starting test ({nameof(Test_ETL_SdfToSqlite)})");
            var (conn, dst) = TestETLBase(
                nameof(Test_ETL_SdfToSqlite),
                "OSGeo.SDF",
                "TestData/World_Countries.sdf",
                "SHP_Schema",
                "World_Countries",
                "OSGeo.SQLite",
                "sqlite");

            using (conn)
                conn.Close();
            if (dst != null)
            {
                using (dst)
                    dst.Close();
            }
        }

        public static void Test_ETL_SdfToSdf()
        {
            Debug.WriteLine($"Starting test ({nameof(Test_ETL_SdfToSdf)})");
            var (conn, dst) = TestETLBase(
                nameof(Test_ETL_SdfToSdf),
                "OSGeo.SDF",
                "TestData/World_Countries.sdf",
                "SHP_Schema",
                "World_Countries",
                "OSGeo.SQLite",
                "sdf");

            using (conn)
                conn.Close();
            if (dst != null)
            {
                using (dst)
                    dst.Close();
            }
        }
    }
}
