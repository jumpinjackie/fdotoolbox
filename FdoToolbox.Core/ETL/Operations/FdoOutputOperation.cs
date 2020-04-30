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
using OSGeo.FDO.Commands.Feature;
using System.Collections.Specialized;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Spatial;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Output pipeline operation
    /// </summary>
    public class FdoOutputOperation : FdoOperationBase
    {
        /// <summary>
        /// The output connection
        /// </summary>
        protected FdoConnection _conn;
        /// <summary>
        /// The service bound to the output connection
        /// </summary>
        protected FdoFeatureService _service;
        /// <summary>
        /// The property value mappings
        /// </summary>
        protected NameValueCollection _mappings = new NameValueCollection();

        /// <summary>
        /// The FDO Class definition we're inserting data into
        /// </summary>
        protected ClassDefinition _clsDef;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        public FdoOutputOperation(FdoConnection conn, string className)
        {
            _conn = conn;
            _service = conn.CreateFeatureService();
            this.ClassName = className;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        /// <param name="propertyMappings"></param>
        public FdoOutputOperation(FdoConnection conn, string className, NameValueCollection propertyMappings)
            : this(conn, className)
        {
            _mappings = propertyMappings;
            //_mappings = Reverse(propertyMappings);
        }

        /// <summary>
        /// The name of the feature class to write features to
        /// </summary>
        public string ClassName { get; set; }
        /*
        private NameValueCollection Reverse(NameValueCollection propertyMappings)
        {
            // The API specifies mappings are [source] -> [target], but the
            // revised implementation only works if it is [target] -> [source]
            // so we have to reverse the mappings

            NameValueCollection nvc = new NameValueCollection();
            if (propertyMappings == null || propertyMappings.Count == 0)
                return nvc;

            foreach (string srcProp in propertyMappings.Keys)
            {
                string targetProp = propertyMappings[srcProp];
                nvc.Add(targetProp, srcProp);
            }

            return nvc;
        }
        */
        protected bool IsUsingPropertyMappings()
        {
            return _mappings != null && _mappings.Count > 0;
        }

        private List<string> propertySnapshot = null;

        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            //We fetch the class def here instead of the ctor as the class in question
            //may have to be created by a pre-copy operation
            _clsDef = _service.GetClassByName(this.ClassName);
            base.PrepareForExecution(pipelineExecuter);
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            IInsert insert = null;
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                insert = service.CreateCommand<IInsert>(CommandType.CommandType_Insert);
            }
            this.Info("Set feature class to: {0}", this.ClassName);
            insert.SetFeatureClassName(this.ClassName);
            PropertyValueCollection propVals = insert.PropertyValues;

            Prepare(propVals);
            foreach (FdoRow row in rows)
            {
                Bind(row);
                insert.Prepare();
                try
                {
                    using (IFeatureReader reader = insert.Execute())
                    {
                        reader.Close();
                    }
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    ex.Data["Class/Table"] = this.ClassName;
                    RaiseFailedFeatureProcessed(row, ex);
                    RePrepare(propVals);
                }
                yield return row;
            }
            insert.Dispose();
            //yield break;
        }

        /// <summary>
        /// Temp dictionary to hold our property values.
        /// </summary>
        private Dictionary<string, LiteralValue> currentValues = new Dictionary<string, LiteralValue>();

        private void Prepare(PropertyValueCollection propVals)
        {
            propVals.Clear();
            currentValues.Clear();

            // I do not trust the long-term stability of the PropertyValueCollection
            //
            // So what we do is load it up once with LiteralValue references and manipulate these
            // outside of the collection (via a cached dictionary). We cache everything from the wrapper API 
            // that can be cached in the managed world so that we only have minimal contact with it

            // Omit read-only properties
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                ClassDefinition c = service.GetClassByName(this.ClassName);
                foreach (PropertyDefinition p in c.Properties)
                {
                    string name = p.Name;
                    PropertyValue pv = new PropertyValue(name, null);
                    if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        DataPropertyDefinition d = p as DataPropertyDefinition;
                        if (!d.ReadOnly && !d.IsAutoGenerated) 
                        {
                            DataValue dv = null;
                            switch (d.DataType)
                            {
                                case DataType.DataType_BLOB:
                                    dv = new BLOBValue();
                                    break;
                                case DataType.DataType_Boolean:
                                    dv = new BooleanValue();
                                    break;
                                case DataType.DataType_Byte:
                                    dv = new ByteValue();
                                    break;
                                case DataType.DataType_CLOB:
                                    dv = new CLOBValue();
                                    break;
                                case DataType.DataType_DateTime:
                                    dv = new DateTimeValue();
                                    break;
                                case DataType.DataType_Decimal:
                                    dv = new DecimalValue();
                                    break;
                                case DataType.DataType_Double:
                                    dv = new DoubleValue();
                                    break;
                                case DataType.DataType_Int16:
                                    dv = new Int16Value();
                                    break;
                                case DataType.DataType_Int32:
                                    dv = new Int32Value();
                                    break;
                                case DataType.DataType_Int64:
                                    dv = new Int64Value();
                                    break;
                                case DataType.DataType_Single:
                                    dv = new SingleValue();
                                    break;
                                case DataType.DataType_String:
                                    dv = new StringValue();
                                    break;
                            }
                            if (dv != null)
                            {
                                pv.Value = dv;
                                propVals.Add(pv);
                            }
                        }
                    }
                    else if (p.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    {
                        GeometricPropertyDefinition g = p as GeometricPropertyDefinition;
                        if (!g.ReadOnly)
                        {
                            GeometryValue gv = new GeometryValue();
                            pv.Value = gv;
                            propVals.Add(pv);
                        }
                    }
                }
                c.Dispose();
            }

            //Load property values into temp dictionary
            foreach (PropertyValue p in propVals)
            {
                currentValues[p.Name.Name] = p.Value as LiteralValue;
            }

            if (propertySnapshot == null)
            {
                propertySnapshot = new List<string>();
                foreach (PropertyValue p in propVals)
                {
                    propertySnapshot.Add(p.Name.Name);
                }
            }
        }

        private void RePrepare(PropertyValueCollection propVals)
        {
            //If empty go for full rebuild
            if (propVals.Count == 0)
            {
                propertySnapshot = null;
                Prepare(propVals);
                return;
            }

            //Otherwise purge from the property values any properties not 
            //in the snapshot
            List<PropertyValue> remove = new List<PropertyValue>();
            foreach (PropertyValue p in propVals)
            {
                if (!propertySnapshot.Contains(p.Name.Name))
                    remove.Add(propVals.FindItem(p.Name.Name));
            }
            if (remove.Count > 0)
            {
                foreach (PropertyValue propVal in remove)
                {
                    propVals.Remove(propVal);
                }
            }

            currentValues.Clear();
            //Re-cache property values into temp dictionary
            foreach (PropertyValue p in propVals)
            {
                currentValues[p.Name.Name] = p.Value as LiteralValue;
            }
        }

        private void Bind(FdoRow row)
        {
            //Set all property values to null and then set the proper values
            foreach (string propName in currentValues.Keys)
            {
                //Get the [target] property name. If reverse-mapped to the [source], use the 
                //mapped [source] property name. Otherwise it is assumed that [source] name
                //is the same as the [target] name
                string name = propName;
                if (_mappings[name] != null)
                    name = _mappings[name];

                LiteralValue lVal = currentValues[propName] as LiteralValue;
                if (lVal.LiteralValueType == LiteralValueType.LiteralValueType_Data)
                {
                    DataValue dVal = lVal as DataValue;
                    dVal.SetNull();

                    switch (dVal.DataType)
                    {
                        case DataType.DataType_BLOB:
                            {
                                byte [] blob = row[name] as byte[];
                                if(blob != null)
                                {
                                    (dVal as BLOBValue).Data = blob;
                                }
                            }
                            break;
                        case DataType.DataType_Boolean:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as BooleanValue).Boolean = Convert.ToBoolean(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_Byte:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as ByteValue).Byte = Convert.ToByte(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_CLOB:
                            {
                                byte[] clob = row[name] as byte[];
                                if (clob != null)
                                {
                                    (dVal as CLOBValue).Data = clob;
                                }
                            }
                            break;
                        case DataType.DataType_DateTime:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as DateTimeValue).DateTime = Convert.ToDateTime(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_Decimal:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as DecimalValue).Decimal = Convert.ToDouble(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_Double:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as DoubleValue).Double = Convert.ToDouble(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_Int16:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as Int16Value).Int16 = Convert.ToInt16(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_Int32:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as Int32Value).Int32 = Convert.ToInt32(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_Int64:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as Int64Value).Int64 = Convert.ToInt64(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_Single:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as SingleValue).Single = Convert.ToSingle(row[name]);
                                }
                            }
                            break;
                        case DataType.DataType_String:
                            {
                                if (row[name] != null)
                                {
                                    (dVal as StringValue).String = row[name].ToString();
                                }
                            }
                            break;
                    }
                }
                else
                {
                    GeometryValue gVal = lVal as GeometryValue;
                    gVal.SetNull();

                    IGeometry geom = row[name] as IGeometry;
                    if (geom != null)
                    {
                        IGeometry origGeom = geom;
                        //HACK: Just for you SQL Server 2008! 
                        if (geom.DerivedType == OSGeo.FDO.Common.GeometryType.GeometryType_Polygon ||
                            geom.DerivedType == OSGeo.FDO.Common.GeometryType.GeometryType_CurvePolygon ||
                            geom.DerivedType == OSGeo.FDO.Common.GeometryType.GeometryType_MultiCurvePolygon ||
                            geom.DerivedType == OSGeo.FDO.Common.GeometryType.GeometryType_MultiPolygon)
                        {
                            if (_conn.Provider.ToUpper().StartsWith("OSGEO.SQLSERVERSPATIAL"))
                            {
                                //This isn't the most optimal way, as the most optimal
                                //method according to RFC48 is to get the source rule and
                                //strictness and pass that to SpatialUtility.GetPolygonVertexOrderAction()
                                //along with the target rule and strictness. I don't think warping the
                                //existing API to address such a provider-specific corner case is worth it.
                                var caps = _clsDef.Capabilities;
                                var rule = caps.get_PolygonVertexOrderRule(name);
                                geom = SpatialUtility.FixPolygonVertexOrder(origGeom, rule);
                            }
                        }

                        if (geom != null)
                            gVal.Geometry = FdoGeometryFactory.Instance.GetFgf(geom);
                        else
                            gVal.Geometry = FdoGeometryFactory.Instance.GetFgf(origGeom);
                    }
                }
            }
        }
    }
}
