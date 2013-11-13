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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using FdoToolbox.Core;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A control that provides the ability to perform bulk data updates
    /// </summary>
    [ToolboxItem(false)]
    public partial class FdoBulkUpdateCtl : ViewContent, IFdoBulkUpdateView
    {
        private FdoBulkUpdatePresenter _presenter;

        internal FdoBulkUpdateCtl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkUpdateCtl"/> class.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="className">Name of the class.</param>
        public FdoBulkUpdateCtl(FdoConnection conn, string className)
            : this()
        {
            _conn = conn;
            _presenter = new FdoBulkUpdatePresenter(this, conn, className);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkUpdateCtl"/> class.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="initialFilter">The initial filter.</param>
        public FdoBulkUpdateCtl(FdoConnection conn, string className, string initialFilter)
            : this(conn, className)
        {
            this.Filter = initialFilter;
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            _presenter.Update();
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

            DataGridViewColumn colEnable = new DataGridViewColumn();
            colEnable.Name = "COL_ENABLE";
            colEnable.HeaderText = "Enable";
            colEnable.CellTemplate = new DataGridViewCheckBoxCell();
            colEnable.Width = 60;

            DataGridViewColumn colNull = new DataGridViewColumn();
            colNull.Name = "COL_NULL";
            colNull.HeaderText = "Null";
            colNull.CellTemplate = new DataGridViewCheckBoxCell();
            colNull.Width = 60;

            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            colName.CellTemplate = new DataGridViewTextBoxCell();

            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";
            colValue.CellTemplate = new DataGridViewTextBoxCell();

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            grdProperties.Columns.Add(colEnable);
            grdProperties.Columns.Add(colNull);
            grdProperties.Columns.Add(colName);
            grdProperties.Columns.Add(colValue);

        }

        /// <summary>
        /// Adds the data property.
        /// </summary>
        /// <param name="dataDef">The data def.</param>
        public void AddDataProperty(DataPropertyDefinition dataDef)
        {
            if (dataDef.IsAutoGenerated || dataDef.ReadOnly)
                return;

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = dataDef.Name;
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
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell();
                            tc.MaxInputLength = dataDef.Length;
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
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell();
                            tc.MaxInputLength = dataDef.Length;
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
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell();
                            tc.MaxInputLength = dataDef.Length;
                            valueCell = tc;
                        }
                        break;
                }
            }
            valueCell.Style.BackColor = dataDef.Nullable ? Color.YellowGreen : Color.White;
            valueCell.Value = dataDef.DefaultValue;
            valueCell.ToolTipText = dataDef.Description;

            DataGridViewCheckBoxCell ecell = new DataGridViewCheckBoxCell(false);
            ecell.Value = true;
            DataGridViewCheckBoxCell ncell = new DataGridViewCheckBoxCell(false);
            ncell.Value = false;

            row.Cells.Add(ecell);
            row.Cells.Add(ncell);
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
            if (geomDef.ReadOnly)
                return;

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = geomDef.Name;
            nameCell.Tag = geomDef;

            DataGridViewCell valueCell = new DataGridViewTextBoxCell();
            valueCell.ToolTipText = "Enter the FGF or WKB geometry text";

            DataGridViewCheckBoxCell ecell = new DataGridViewCheckBoxCell(false);
            ecell.Value = true;
            DataGridViewCheckBoxCell ncell = new DataGridViewCheckBoxCell(false);
            ncell.Value = false;

            row.Cells.Add(ecell);
            row.Cells.Add(ncell);
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            nameCell.ReadOnly = true;

            grdProperties.Rows.Add(row);

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
                bool enabled = Convert.ToBoolean(row.Cells[0].Value);
                bool setNull = Convert.ToBoolean(row.Cells[1].Value);
                if (enabled)
                {
                    string name = row.Cells[2].Value.ToString();
                    PropertyDefinition propDef = row.Cells[2].Tag as PropertyDefinition;
                    if (setNull)
                    {
                        LiteralValue expr = null;
                        if (propDef.PropertyType == PropertyType.PropertyType_DataProperty)
                        {
                            DataPropertyDefinition dp = propDef as DataPropertyDefinition;
                            switch (dp.DataType)
                            {
                                case DataType.DataType_BLOB:
                                    expr = new BLOBValue();
                                    break;
                                case DataType.DataType_Boolean:
                                    expr = new BooleanValue();
                                    break;
                                case DataType.DataType_Byte:
                                    expr = new ByteValue();
                                    break;
                                case DataType.DataType_CLOB:
                                    expr = new CLOBValue();
                                    break;
                                case DataType.DataType_DateTime:
                                    expr = new DateTimeValue();
                                    break;
                                case DataType.DataType_Decimal:
                                    expr = new DecimalValue();
                                    break;
                                case DataType.DataType_Double:
                                    expr = new DoubleValue();
                                    break;
                                case DataType.DataType_Int16:
                                    expr = new Int16Value();
                                    break;
                                case DataType.DataType_Int32:
                                    expr = new Int32Value();
                                    break;
                                case DataType.DataType_Int64:
                                    expr = new Int64Value();
                                    break;
                                case DataType.DataType_Single:
                                    expr = new SingleValue();
                                    break;
                                case DataType.DataType_String:
                                    expr = new StringValue();
                                    break;
                            }
                        }
                        else if (propDef.PropertyType == PropertyType.PropertyType_GeometricProperty)
                        {
                            expr = new GeometryValue();
                        }

                        if (expr != null)
                        {
                            if (expr.LiteralValueType == LiteralValueType.LiteralValueType_Data)
                                (expr as DataValue).SetNull();
                            else
                                (expr as GeometryValue).SetNull();

                            values.Add(name, expr);
                        }
                    }
                    else
                    {
                        if (row.Cells[2].Value != null)
                        {
                            string str = row.Cells[3].Value.ToString();
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


        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter
        {
            get
            {
                return txtUpdateFilter.Text;
            }
            set
            {
                txtUpdateFilter.Text = value;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _presenter.TestUpdate();
        }

        private FdoConnection _conn;
        private OSGeo.FDO.Schema.ClassDefinition _classDef;

        private void txtUpdateFilter_Click(object sender, EventArgs e)
        {
            if (_classDef == null)
            {
                using (FdoFeatureService service = _conn.CreateFeatureService())
                {
                    _classDef = service.GetClassByName(this.ClassName);
                }
            }
            if (!string.IsNullOrEmpty(this.Filter))
            {
                string filter = ExpressionEditor.EditExpression(_conn, _classDef, null, this.Filter, ExpressionMode.Filter);
                if (filter != null)
                    this.Filter = filter;
            }
            else
            {
                this.Filter = ExpressionEditor.NewExpression(_conn, _classDef, null, ExpressionMode.Filter);
            }
        }
    }
}
