#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Common.Xml;
using OSGeo.FDO.Xml;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Encapsulates all aspects of a FDO data store. This is essentially an
    /// object representation of a FDO XML configuration document.
    /// </summary>
    public class FdoDataStoreConfiguration
    {
        /// <summary>
        /// Gets or sets the feature schemas in this data store
        /// </summary>
        public FeatureSchemaCollection Schemas { get; set; }

        /// <summary>
        /// Gets or sets the spatial contexts in this data store
        /// </summary>
        public SpatialContextInfo[] SpatialContexts { get; set; }

        /// <summary>
        /// Gets or sets the logical to physical schema mappings in this data store
        /// </summary>
        public PhysicalSchemaMappingCollection Mappings { get; set; }

        private FdoDataStoreConfiguration() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schemas"></param>
        /// <param name="contexts"></param>
        /// <param name="mappings"></param>
        public FdoDataStoreConfiguration(FeatureSchemaCollection schemas, SpatialContextInfo[] contexts, PhysicalSchemaMappingCollection mappings)
        {
            this.Schemas = schemas;
            this.SpatialContexts = contexts;
            this.Mappings = mappings;
        }

        /// <summary>
        /// Saves this out to a FDO XML configuration document
        /// </summary>
        /// <param name="xmlFile"></param>
        public void Save(string xmlFile, bool applySchemaDeletions)
        {
            if (applySchemaDeletions)
            {
                // There's no .AcceptChanges() on the feature schema collection level, because the
                // deleted schema element state on a FDO feature schema is generally meant to be processed
                // by the FDO command implementing IApplySchema, so we have to remove such marked schemas
                // ourselves from the schema collection if we find any
                var toRemove = new List<FeatureSchema>();
                foreach (FeatureSchema schema in this.Schemas)
                {
                    if (schema.ElementState == SchemaElementState.SchemaElementState_Deleted)
                    {
                        toRemove.Add(schema);
                    }
                }
                foreach (var removeMe in toRemove)
                {
                    this.Schemas.Remove(removeMe);
                    removeMe.Dispose();
                }
            }

            using (var fact = new FgfGeometryFactory())
            using (var ws = new IoFileStream(xmlFile, "w"))
            {
                using (var writer = new XmlWriter(ws, true, XmlWriter.LineFormat.LineFormat_Indent))
                {
                    //Write spatial contexts
                    if (this.SpatialContexts != null && this.SpatialContexts.Length > 0)
                    {
                        var flags = new XmlSpatialContextFlags();
                        using (var scWriter = new XmlSpatialContextWriter(writer))
                        {
                            foreach (var sc in this.SpatialContexts)
                            {
                                //XmlSpatialContextWriter forbids writing dynamically calculated extents
                                if (sc.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Dynamic)
                                    continue;

                                scWriter.CoordinateSystem = sc.CoordinateSystem;
                                scWriter.CoordinateSystemWkt = sc.CoordinateSystemWkt;
                                scWriter.Description = sc.Description;
                                
                                scWriter.ExtentType = sc.ExtentType;

                                using (var geom = fact.CreateGeometry(sc.ExtentGeometryText))
                                {
                                    byte[] fgf = fact.GetFgf(geom);
                                    scWriter.Extent = fgf;
                                }

                                scWriter.Name = sc.Name;
                                scWriter.XYTolerance = sc.XYTolerance;
                                scWriter.ZTolerance = sc.ZTolerance;

                                scWriter.WriteSpatialContext();
                            }
                        }
                    }

                    

                    //Write logical schema
                    if (this.Schemas != null)
                    {
                        var schemas = CloneSchemas(this.Schemas);

                        schemas.WriteXml(writer);
                    }

                    //Write physical mappings
                    if (this.Mappings != null)
                    {
                        this.Mappings.WriteXml(writer);
                    }

                    writer.Close();
                }
                ws.Close();
            }
        }

        private FeatureSchemaCollection CloneSchemas(FeatureSchemaCollection featureSchemaCollection)
        {
            var schemas = new FeatureSchemaCollection(null);
            using (var ios = new IoMemoryStream())
            {
                featureSchemaCollection.WriteXml(ios);
                ios.Reset();
                schemas.ReadXml(ios);
            }
            return schemas;
        }

        /// <summary>
        /// Loads a FDO XML configuration document
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public static FdoDataStoreConfiguration FromFile(string xmlFile)
        {
            using (var fact = new FgfGeometryFactory())
            using (var ios = new IoFileStream(xmlFile, "r"))
            {
                using (var reader = new XmlReader(ios))
                {
                    List<SpatialContextInfo> contexts = new List<SpatialContextInfo>();
                    using (var scReader = new XmlSpatialContextReader(reader))
                    {
                        while (scReader.ReadNext())
                        {
                            var sc = new SpatialContextInfo
                            {
                                CoordinateSystem = scReader.GetCoordinateSystem(),
                                CoordinateSystemWkt = scReader.GetCoordinateSystemWkt(),
                                Description = scReader.GetDescription(),
                                ExtentType = scReader.GetExtentType()
                            };
                            if (sc.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Static)
                            {
                                using (var geom = fact.CreateGeometryFromFgf(scReader.GetExtent()))
                                {
                                    sc.ExtentGeometryText = geom.Text;
                                }
                            }
                            sc.IsActive = scReader.IsActive();
                            sc.Name = scReader.GetName();
                            sc.XYTolerance = scReader.GetXYTolerance();
                            sc.ZTolerance = scReader.GetZTolerance();

                            contexts.Add(sc);
                        }
                    }

                    ios.Reset();

                    var schemas = new FeatureSchemaCollection(null);
                    schemas.ReadXml(ios);

                    ios.Reset();

                    var mappings = new PhysicalSchemaMappingCollection();
                    mappings.ReadXml(ios);

                    ios.Close();

                    return new FdoDataStoreConfiguration(schemas, contexts.ToArray(), mappings);
                }
            }
        }
    }
}
