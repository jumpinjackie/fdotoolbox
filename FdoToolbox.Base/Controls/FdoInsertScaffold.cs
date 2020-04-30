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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using FdoToolbox.Core;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A user interface helper to create new FDO features
    /// </summary>
    [ToolboxItem(false)]
    public partial class FdoInsertScaffold : ViewContent, IFdoInsertView
    {
        private FdoInsertScaffoldPresenter _presenter;

        internal FdoInsertScaffold()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoInsertScaffold"/> class.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="className">Name of the class.</param>
        public FdoInsertScaffold(FdoConnection conn, string className)
            : this()
        {
            _presenter = new FdoInsertScaffoldPresenter(this, conn, className);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _presenter.Cancel();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            _presenter.Insert();
        }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public string ClassName
        {
            get
            {
                return lblTargetClass.Text;
            }
            set
            {
                lblTargetClass.Text = value;
            }
        }

        /// <summary>
        /// Initializes the grid.
        /// </summary>
        public void InitializeGrid()
        {
            grdProperties.Rows.Clear();
            grdProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn
            {
                Name = "COL_NAME",
                HeaderText = "Name",
                ReadOnly = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };

            DataGridViewColumn colValue = new DataGridViewColumn
            {
                Name = "COL_VALUE",
                HeaderText = "Value",
                CellTemplate = new DataGridViewTextBoxCell(),

                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            grdProperties.Columns.Add(colName);
            grdProperties.Columns.Add(colValue);

			grdProperties.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        /// <summary>
        /// Adds the data property.
        /// </summary>
        /// <param name="dataDef">The data def.</param>
		public void AddDataProperty(DataPropertyDefinition dataDef)
		{
			AddDataProperty(dataDef, null);
		}

		/// <summary>
		/// Adds the data property.
		/// </summary>
		/// <param name="dataDef">The data def.</param>
		/// <param name="szClassHierarchy">The class name hierarchy.</param>
		public void AddDataProperty(DataPropertyDefinition dataDef, String szClassHierarchy)
        {
			if (dataDef.IsAutoGenerated || dataDef.ReadOnly)
                return;

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();

			if (!String.IsNullOrEmpty(szClassHierarchy))
				nameCell.Value = szClassHierarchy + ".";
			nameCell.Value += dataDef.Name;

            nameCell.ToolTipText = "Type: " + dataDef.DataType;
            nameCell.Tag = dataDef;

            DataGridViewCell valueCell = null;
            if (dataDef.ValueConstraint != null && dataDef.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_List)
            {
                PropertyValueConstraintList list = (dataDef.ValueConstraint as PropertyValueConstraintList);
                DataGridViewComboBoxCell cc = new DataGridViewComboBoxCell();
                List<string> values = new List<string>();
                foreach (DataValue value in list.ConstraintList)
                {
                    values.Add(value.ToString());
                }
                cc.DataSource = values;
                valueCell = cc;
            }
            else
            {
                switch (dataDef.DataType)
                {
                    case DataType.DataType_BLOB:
                        {
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell
                            {
                                MaxInputLength = dataDef.Length
                            };
                            valueCell = tc;
                        }
                        break;
                    case DataType.DataType_Boolean:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Byte:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_CLOB:
                        {
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell
                            {
                                MaxInputLength = dataDef.Length
                            };
                            valueCell = tc;
                        }
                        break;
                    case DataType.DataType_DateTime:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Decimal:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Double:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Int16:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Int32:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Int64:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Single:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_String:
                        {
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell
                            {
                                MaxInputLength = dataDef.Length
                            };
                            valueCell = tc;
                        }
                        break;
                }
            }
            valueCell.Style.BackColor = dataDef.Nullable ? Color.YellowGreen : Color.White;
            valueCell.Value = dataDef.DefaultValue;
            valueCell.ToolTipText = dataDef.Description;

            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            nameCell.ReadOnly = true;

            grdProperties.Rows.Add(row);
        }

		/// <summary>
		/// Adds the geometric property.
		/// </summary>
		/// <param name="geomDef">The geom def.</param>
		public void AddGeometricProperty(GeometricPropertyDefinition geomDef)
		{
			AddGeometricProperty(geomDef, null);
		}

		/// <summary>
		/// Adds the geometric property.
		/// </summary>
		/// <param name="geomDef">The geom def.</param>
		/// <param name="szClassHierarchy">The class name hierarchy.</param>
		public void AddGeometricProperty(GeometricPropertyDefinition geomDef, String szClassHierarchy)
		{
			if (geomDef.ReadOnly)
				return;

			DataGridViewRow row = new DataGridViewRow();
			DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();

			if (!String.IsNullOrEmpty(szClassHierarchy))
				nameCell.Value = szClassHierarchy + ":";
			nameCell.Value += geomDef.Name;

			nameCell.Tag = geomDef;

            DataGridViewCell valueCell = new DataGridViewTextBoxCell
            {
                ToolTipText = "Enter the FGF or WKB geometry text"
            };

            row.Cells.Add(nameCell);
			row.Cells.Add(valueCell);

			nameCell.ReadOnly = true;

			grdProperties.Rows.Add(row);

		}

		/// <summary>
		/// Adds the object property.
		/// </summary>
		/// <param name="objectDef">The object def.</param>
		public void AddObjectProperty(ObjectPropertyDefinition objectDef)
		{
			AddObjectProperty(objectDef, null);
		}

		/// <summary>
		/// Adds the object property.
		/// </summary>
		/// <param name="objectDef">The object def.</param>
		/// <param name="szClassHierarchy">The class name hierarchy.</param>
		public void AddObjectProperty(ObjectPropertyDefinition objectDef, String szClassHierarchy)
		{
			String szObjectName = "";
			if (!String.IsNullOrEmpty(szClassHierarchy))
				szObjectName = szClassHierarchy + ".";
			szObjectName += objectDef.Name;

			// TODO: maybe need to do some work on "objectDef.IdentityProperty" to aid interface
			foreach (PropertyDefinition pd in objectDef.Class.Properties)
			{
				switch (pd.PropertyType)
				{
					case PropertyType.PropertyType_DataProperty:
						AddDataProperty((DataPropertyDefinition)pd, szObjectName);
					break;

					// at present (FDO 3.5) SQLServer provider does not allow FeatureClass to be the child
					// class in an object property - this may be FDO definition or related to SQL Server provider
					// - if it is a generic FDO rule then this case statement will never be hit
					case PropertyType.PropertyType_GeometricProperty:
						AddGeometricProperty((GeometricPropertyDefinition)pd, szObjectName);
					break;

					// allows for cascading hierarchy - crazy man!
					case PropertyType.PropertyType_ObjectProperty:
						AddObjectProperty((ObjectPropertyDefinition)pd, szObjectName);
					break;

					// allows for cascading hierarchy - crazy man!
					case PropertyType.PropertyType_AssociationProperty:
						AddAssociationProperty((AssociationPropertyDefinition)pd, szObjectName);
					break;
				}
			}
		}

		/// <summary>
		/// Adds the association property.
		/// </summary>
		/// <param name="assocDef">The association def.</param>
		public void AddAssociationProperty(AssociationPropertyDefinition assocDef)
		{
			// TODO: validate this statement and action
			MessageBox.Show("Association Properties cannot be added at INSERT time\n\nInsert a record into the associated class with the same ID");
			// AddAssociationProperty(assocDef, null);
		}

		/// <summary>
		/// Adds the association property.
		/// </summary>
		/// <param name="assocDef">The association def.</param>
		/// <param name="szClassHierarchy">The class name hierarchy.</param>
		public void AddAssociationProperty(AssociationPropertyDefinition assocDef, String szClassHierarchy)
		{
			String szObjectName = "";
			if (!String.IsNullOrEmpty(szClassHierarchy))
				szObjectName = szClassHierarchy + ".";
			szObjectName += assocDef.Name;

			// TODO: maybe need to do some work on "objectDef.IdentityProperty" to aid interface
			foreach (PropertyDefinition pd in assocDef.AssociatedClass.Properties)
			{
				switch (pd.PropertyType)
				{
					case PropertyType.PropertyType_DataProperty:
						AddDataProperty((DataPropertyDefinition)pd, szObjectName);
					break;

					case PropertyType.PropertyType_GeometricProperty:
						AddGeometricProperty((GeometricPropertyDefinition)pd, szObjectName);
					break;

					// allows for cascading hierarchy - crazy man!
					case PropertyType.PropertyType_ObjectProperty:
						AddObjectProperty((ObjectPropertyDefinition)pd, szObjectName);
					break;

					// allows for cascading hierarchy - crazy man!
					// TODO: do NOT allow for cascading associations until reciprocol nature is
					//  better understood - else get into a loop between parent and child
					//  Possibly just need to check this (pd) does not point back to the previous!?

					// See similar comment here in "void FdoRdbmsFilterProcessor::FollowRelation(...)"
					//  \FDO\branches\3.5\Providers\GenericRdbms\Src\Fdo\Filter\FdoRdbmsFilterProcessor.cpp

					//case PropertyType.PropertyType_AssociationProperty:
					//	AddAssociationProperty((AssociationPropertyDefinition)pd, szObjectName);
					//break;
				}
			}
		}

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ValueExpression> GetValues()
        {
            Dictionary<string, ValueExpression> values = new Dictionary<string, ValueExpression>();
            foreach (DataGridViewRow row in grdProperties.Rows)
            {
                string name = row.Cells[0].Value.ToString();
                PropertyDefinition propDef = row.Cells[0].Tag as PropertyDefinition;
                if (row.Cells[1].Value != null)
                {
                    string str = row.Cells[1].Value.ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        ValueExpression expr = null;
                        if (propDef.PropertyType == PropertyType.PropertyType_DataProperty)
                        {
                            DataPropertyDefinition dp = propDef as DataPropertyDefinition;
                            switch (dp.DataType)
                            {
                                case DataType.DataType_Boolean:
                                    expr = new BooleanValue(Convert.ToBoolean(str));
                                    break;
                                case DataType.DataType_Byte:
                                    expr = new ByteValue(Convert.ToByte(str));
                                    break;
                                case DataType.DataType_DateTime:
                                    expr = new DateTimeValue(Convert.ToDateTime(str));
                                    break;
                                case DataType.DataType_Decimal:
                                    expr = new DecimalValue(Convert.ToDouble(str));
                                    break;
                                case DataType.DataType_Double:
                                    expr = new DoubleValue(Convert.ToDouble(str));
                                    break;
                                case DataType.DataType_Int16:
                                    expr = new Int16Value(Convert.ToInt16(str));
                                    break;
                                case DataType.DataType_Int32:
                                    expr = new Int32Value(Convert.ToInt32(str));
                                    break;
                                case DataType.DataType_Int64:
                                    expr = new Int64Value(Convert.ToInt64(str));
                                    break;
                                case DataType.DataType_Single:
                                    expr = new SingleValue(Convert.ToSingle(str));
                                    break;
                                case DataType.DataType_String:
                                    expr = new StringValue(str);
                                    break;
                                default:
                                    throw new NotSupportedException("Unsupported data type: " + dp.DataType);
                            }
                        }
                        else if (propDef.PropertyType == PropertyType.PropertyType_GeometricProperty)
                        {
                            FdoGeometryFactory fact = FdoGeometryFactory.Instance;
                            OSGeo.FDO.Geometry.IGeometry geom = fact.CreateGeometry(str);
                            byte[] fgf = fact.GetFgf(geom);
                            expr = new GeometryValue(fgf);
                            geom.Dispose();
                        }

                        if (expr != null)
                            values.Add(name, expr);
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use transaction].
        /// </summary>
        /// <value><c>true</c> if [use transaction]; otherwise, <c>false</c>.</value>
        public bool UseTransaction
        {
            get
            {
                return chkUseTransaction.Checked;
            }
            set
            {
                chkUseTransaction.Checked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use transaction enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use transaction enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTransactionEnabled
        {
            get
            {
                return chkUseTransaction.Enabled;
            }
            set
            {
                chkUseTransaction.Enabled = value;
            }
        }
    }
}
