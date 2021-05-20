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
using System.Data;
using System.ComponentModel;
using System.Collections;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Feature
{
    using RTree;

    /// <summary>
    /// A FDO-friendly DataTable
    /// </summary>
    public class FdoFeatureTable : DataTable, IEnumerable
    {
        private RTree<FdoFeature> _tree;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoFeatureTable"/> class.
        /// </summary>
        public FdoFeatureTable()
            : base()
        {
            this.InitClass();
            _tree = new RTree<FdoFeature>();
        }

        /// <summary>
        /// Gets the bounding box of this feature table
        /// </summary>
        /// <value>The bounding box.</value>
        public Rectangle BoundingBox => _tree.getBounds();

        /// <summary>
        /// Raised when the table requests more information about a spatial context association
        /// </summary>
        public event FdoSpatialContextRequestEventHandler RequestSpatialContext = delegate { };

        private List<SpatialContextInfo> _spatialContexts = new List<SpatialContextInfo>();

        /// <summary>
        /// Adds a spatial context.
        /// </summary>
        /// <param name="ctx">The context.</param>
        public void AddSpatialContext(SpatialContextInfo ctx)
        {
            _spatialContexts.Add(ctx);
        }

        /// <summary>
        /// Gets a spatial context by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public SpatialContextInfo GetSpatialContext(string name)
        {
            return _spatialContexts.Find(delegate(SpatialContextInfo s) { return s.Name == name; });
        }

        /// <summary>
        /// Gets the spatial contexts attached to this table
        /// </summary>
        /// <value>The spatial contexts.</value>
        public ICollection<SpatialContextInfo> SpatialContexts => _spatialContexts;

        /// <summary>
        /// Gets the active spatial context. If no active spatial contexts are found, the first one is returned.
        /// If no spatial contexts are found, null is returned.
        /// </summary>
        /// <value>The active spatial context.</value>
        public SpatialContextInfo ActiveSpatialContext
        {
            get
            {
                SpatialContextInfo c = _spatialContexts.Find(delegate(SpatialContextInfo s) { return s.IsActive; });
                if (c != null)
                    return c;
                else if (_spatialContexts.Count > 0)
                    return _spatialContexts[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoFeatureTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public FdoFeatureTable(DataTable table)
            : base(table.TableName)
        {
            if (table.DataSet != null)
            {
                if ((table.CaseSensitive != table.DataSet.CaseSensitive))
                    this.CaseSensitive = table.CaseSensitive;
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString()))
                    this.Locale = table.Locale;
                if ((table.Namespace != table.DataSet.Namespace))
                    this.Namespace = table.Namespace;

                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
                this.DisplayExpression = table.DisplayExpression;
            }
        }

        /// <summary>
        /// Gets the number of rows (features) in this table
        /// </summary>
        [Browsable(true)]
        public int Count => base.Rows.Count;

        /// <summary>
        /// Gets the row (feature) at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FdoFeature this[int index]
        {
            get { return (FdoFeature)base.Rows[index]; }
        }

        /// <summary>
        /// Adds a row (feature) to this table
        /// </summary>
        /// <param name="feature"></param>
        public void AddRow(FdoFeature feature)
        {
            base.Rows.Add(feature);
            if (feature.BoundingBox != null && !(feature.DesignatedGeometry?.IsEmpty() == true))
            {
                _tree.Add(feature.BoundingBox, feature);
            }
        }

        /// <summary>
        /// Returns a list of features that intersect with the specified rectangle.
        /// </summary>
        /// <param name="r">The intersection rectangle.</param>
        /// <returns></returns>
        public FdoFeature[] Intersects(Rectangle r)
        {
            return _tree.Intersects(r).ToArray();
        }

        /// <summary>
        /// Returns a list of features that contains the specified rectangle.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public FdoFeature[] Contains(Rectangle r)
        {
            return _tree.Contains(r).ToArray();
        }

        /// <summary>
        /// Returns a list of features near the specified point
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public FdoFeature[] GetFeaturesNear(Point pt, float distance)
        {
            return _tree.Nearest(pt, distance).ToArray();
        }

        /// <summary>
        /// Returns an enumerator for enumerating that rows of this table
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return base.Rows.GetEnumerator();
        }

        /// <summary>
        /// Clones this table
        /// </summary>
        /// <returns></returns>
        public new FdoFeatureTable Clone()
        {
            FdoFeatureTable tbl = ((FdoFeatureTable)(base.Clone()));
            tbl.InitVars();
            return tbl;
        }

        /// <summary>
        /// Occurs after a FdoFeature has been change successfully
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureChanged = delegate { };

        /// <summary>
        /// Occurs when a FdoFeature is changing
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureChanging = delegate { };

        /// <summary>
        /// Occurs when a FdoFeature has been deleted
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureDeleting = delegate { };

        /// <summary>
        /// Occurs when a FdoFeature is about to be deleted
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureDeleted = delegate { };

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <returns></returns>
        protected override DataTable CreateInstance()
        {
            return new FdoFeatureTable();
        }

        internal void InitVars()
        {

        }

        private void InitClass()
        {

        }

        /// <summary>
        /// Creates a new feature with the same schema as the table
        /// </summary>
        /// <returns></returns>
        public new FdoFeature NewRow()
        {
            return (FdoFeature)base.NewRow();
        }

        /// <summary>
        /// Creates a new Feature with the same schema as the table based on a DataRow builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new FdoFeature(builder);
        }
        
        /// <summary>
        /// Gets the row type
        /// </summary>
        /// <returns></returns>
        protected override Type GetRowType()
        {
            return typeof(FdoFeature);
        }

        /// <summary>
        /// Removes the row (feature) from the table
        /// </summary>
        /// <param name="row"></param>
        public void RemoveRow(FdoFeature row)
        {
            base.Rows.Remove(row);
            if(row.BoundingBox != null)
                _tree.Delete(row.BoundingBox, row);
        }

        /// <summary>
        /// Raises feature changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            base.OnRowChanged(e);
            this.FeatureChanged(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        /// <summary>
        /// Raises feature changing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            base.OnRowChanging(e);
            this.FeatureChanging(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        /// <summary>
        /// Raises feature deleted
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);
            this.FeatureDeleted(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        /// <summary>
        /// Raises feature deleting
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleting(DataRowChangeEventArgs e)
        {
            base.OnRowDeleting(e);
            this.FeatureDeleting(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        /// <summary>
        /// The default geometry column for this table. If null or empty then this table has no geometries
        /// </summary>
        public string GeometryColumn { get; set; }

        /// <summary>
        /// Initializes the table from a reader
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="origClass">The original class definition</param>
        /// <param name="ignorePk">If true, will not convert the identity properties of the class definition to primary keys</param>
        public void InitTable(IFdoReader reader, ClassDefinition origClass, bool ignorePk)
        {
            this.Columns.Clear();
            this.TableName = reader.GetClassName();
            string[] geometries = reader.GeometryProperties;
            GeometryColumn = reader.DefaultGeometryProperty;
            List<DataColumn> pk = new List<DataColumn>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                FdoPropertyType ptype = reader.GetFdoPropertyType(name);
                if (ptype == FdoPropertyType.Raster)
                {
                    //Raster's can't conceivably be previewed in tabular form, so omit it
                }
                else if (ptype == FdoPropertyType.Object)
                {
					// because the original code used an iterator over the reader.FieldCount
					// it is a bit of a hack to get another definition of the class and check
					// we are at the right property with a name comparison
					// TODO: code review to see if this can be implemented better
					FdoFeatureReader readerFeature = (FdoFeatureReader)reader;
					ClassDefinition classDef = readerFeature.GetClassDefinition();

					foreach (PropertyDefinition propertyDef in classDef.Properties)
					{
						// only looking for the right association
						if ((PropertyType.PropertyType_ObjectProperty == propertyDef.PropertyType)
							&& (propertyDef.Name.Equals(name)))
						{
							ObjectPropertyDefinition objectProp = (ObjectPropertyDefinition)propertyDef;
							ClassDefinition classDefObject = objectProp.Class;

							// TODO: this should be an iterative function?
							foreach (PropertyDefinition propertyDefObject in classDefObject.Properties)
							{
								String nameObject = propertyDefObject.Name;

								// TODO: find a better way to convert types
								Type type = "".GetType();
								// TODO: handle data and iterative associations and objects
								if (PropertyType.PropertyType_DataProperty == propertyDefObject.PropertyType)
								{
									DataPropertyDefinition datapropertyDefObject = (DataPropertyDefinition)propertyDefObject;

									DataType ptAssoc = datapropertyDefObject.DataType;
									switch (ptAssoc)
									{
										case DataType.DataType_String:
											type = "".GetType();
										break;
									}

									DataColumn dc = this.Columns.Add(name + "." + nameObject, type);
								}
								else if (PropertyType.PropertyType_GeometricProperty == propertyDefObject.PropertyType)
								{
									type = typeof(FdoGeometry);
									DataColumn dc = this.Columns.Add(name + "." + nameObject, type);
								}
							}
						}
					}
				}
                else if (ptype == FdoPropertyType.Association)
                {
					// because the original code used an iterator over the reader.FieldCount
					// it is a bit of a hack to get another definition of the class and check
					// we are at the right property with a name comparison
					// TODO: code review to see if this can be implemented better
					FdoFeatureReader readerFeature = (FdoFeatureReader)reader;
					ClassDefinition classDef = readerFeature.GetClassDefinition();

					foreach (PropertyDefinition propertyDef in classDef.Properties)
					{
						// only looking for the right association
						if ((PropertyType.PropertyType_AssociationProperty == propertyDef.PropertyType)
							&& (propertyDef.Name.Equals(name)))
						{
							AssociationPropertyDefinition assocProp = (AssociationPropertyDefinition)propertyDef;
							ClassDefinition classDefAssoc = assocProp.AssociatedClass;

							// TODO: this should be an iterative function?
							foreach (PropertyDefinition propertyDefAssoc in classDefAssoc.Properties)
							{
								String nameAssoc = propertyDefAssoc.Name;

								// TODO: find a better way to convert types
								Type type = typeof(String);
								// TODO: handle data and iterative associations and objects
								if (PropertyType.PropertyType_DataProperty == propertyDefAssoc.PropertyType)
								{
									DataPropertyDefinition datapropertyDefAssoc = (DataPropertyDefinition)propertyDefAssoc;

									DataType ptAssoc = datapropertyDefAssoc.DataType;
									switch (ptAssoc)
									{
										case DataType.DataType_String:
											type = typeof(String);
										break;
									}

									DataColumn dc = this.Columns.Add(name + "." + nameAssoc, type);
								}
								else if (PropertyType.PropertyType_GeometricProperty == propertyDefAssoc.PropertyType)
								{
									type = typeof(FdoGeometry);
									DataColumn dc = this.Columns.Add(name + "." + nameAssoc, type);
								}
							}
						}
					}
				}
                else
                {
                    Type type = reader.GetFieldType(i);
                    if (Array.IndexOf<string>(geometries, name) >= 0)
                    {
                        type = typeof(FdoGeometry);
                        string assoc = reader.GetSpatialContextAssociation(name);
                        if (!string.IsNullOrEmpty(assoc))
                            this.RequestSpatialContext(this, assoc);
                    }
                    DataColumn dc = this.Columns.Add(name, type);

                    if (origClass != null)
                    {
                        //HACK: If this query is the result of the join, the returned Class Definition may have some
                        //fubar constraints applied to it. So use the original Class Definition
                        var idProps = origClass.IdentityProperties;
                        if (ptype != FdoPropertyType.Geometry && idProps.Contains(name))
                        {
                            pk.Add(dc);
                        }
                    }
                    else
                    {
                        if (ptype != FdoPropertyType.Geometry && reader.IsIdentity(name))
                        {
                            pk.Add(dc);
                        }
                    }
                }
            }
            if (pk.Count > 0 && !ignorePk)
            {
                this.PrimaryKey = pk.ToArray();
            }
        }

        /// <summary>
        /// Creates a <see cref="ClassDefinition"/> from this instance
        /// </summary>
        /// <param name="createAutoGeneratedId">If true, will add an auto-generated id property to this class definition</param>
        /// <returns>The class definition</returns>
        public ClassDefinition CreateClassDefinition(bool createAutoGeneratedId)
        {
            ClassDefinition cd = null;
            if (!string.IsNullOrEmpty(this.GeometryColumn))
            {
                FeatureClass fc = new FeatureClass(this.TableName, string.Empty);
                GeometricPropertyDefinition gp = new GeometricPropertyDefinition(this.GeometryColumn, string.Empty);
                fc.Properties.Add(gp);
                fc.GeometryProperty = gp;
                cd = fc;
            }
            else
            {
                cd = new Class(this.TableName, string.Empty);
            }

            if (createAutoGeneratedId)
            {
                int num = 1;
                string name = "AutoID";
                string genName = name + num;
                string theName = string.Empty;
                if (this.Columns[name] != null)
                {
                    while (this.Columns[genName] != null)
                    {
                        genName = name + (num++);
                    }
                    theName = genName;
                }
                else
                {
                    theName = name;
                }

                DataPropertyDefinition id = new DataPropertyDefinition(theName, string.Empty)
                {
                    IsAutoGenerated = true,
                    DataType = DataType.DataType_Int32
                };
                cd.Properties.Add(id);
                cd.IdentityProperties.Add(id);
            }

            //Now process columns
            foreach (DataColumn dc in this.Columns)
            {
                if (dc.ColumnName != this.GeometryColumn)
                {
                    DataPropertyDefinition dp = ExpressUtility.GetDataPropertyForColumn(dc);
                    cd.Properties.Add(dp);
                }
            }
            return cd;
        }
    }
}
