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

using FdoToolbox.Core.Utility;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Schema;

namespace FdoTest
{
    public class SchemaTests
    {
        public static void Test_SchemaCloning()
        {
            var schemas = new FeatureSchemaCollection(null);
            using (var iof = new IoFileStream("C:/Users/User/Desktop/postgis.xml", "r"))
            {
                schemas.ReadXml(iof);
            }

            Assert.Equal(2, schemas.Count);
            var schema = schemas["parcels_local"];
            VerifyLocal(schema);

            var clonedSchema = FdoSchemaUtil.CloneSchema(schema);
            VerifyLocal(clonedSchema);

            var classes = schema.Classes;
            classes["v_parcel_groups"].Delete();
            Assert.Equal(SchemaElementState.SchemaElementState_Deleted, classes["v_parcel_groups"].ElementState);

            var clonedSchema2 = FdoSchemaUtil.CloneSchema(schema, true);
            classes = clonedSchema2.Classes;
            Assert.Equal(3, classes.Count);

            var properties = classes["parcels"].Properties;
            var pn = properties[properties.Count - 1].Name;
            properties[properties.Count - 1].Delete();

            var clonedSchema3 = FdoSchemaUtil.CloneSchema(schema, true);
            classes = clonedSchema3.Classes;
            Assert.Equal(3, classes.Count);
            properties = classes["parcels"].Properties;
            Assert.Equals(true, properties.IndexOf(pn) < 0);

            schemas = new FeatureSchemaCollection(null);
            using (var iof = new IoFileStream("C:/Users/User/Desktop/postgis.xml", "r"))
            {
                schemas.ReadXml(iof);
            }
            var newfsc = new FeatureSchemaCollection(null);
            var clone = FdoSchemaUtil.CloneSchema(schemas[0], false);
            newfsc.Add(clone);

            void VerifyLocal(FeatureSchema fs)
            {
                Assert.NotNull(fs);
                var classes = fs.Classes;
                Assert.Equal(4, classes.Count);

                Assert.Equal(true, classes.Contains("parcel_shps"));
                Assert.Equal(true, classes.Contains("parcel_shps_import"));
                Assert.Equal(true, classes.Contains("parcels"));
                Assert.Equal(true, classes.Contains("v_parcel_groups"));
            }
        }
    }
}
