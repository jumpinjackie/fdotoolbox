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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using System.Diagnostics;
using System.Collections.Specialized;
using OSGeo.FDO.Commands.DataStore;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Connections;

namespace FdoToolbox.Base.Forms
{
    internal partial class DictionaryDialog : Form
    {
        internal DictionaryDialog()
        {
            InitializeComponent();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            grdProperties.Rows.Clear();
            grdProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdProperties.Columns.Add(colName);
            grdProperties.Columns.Add(colValue);
        }

        public DictionaryDialog(IEnumerable<DictionaryProperty> dict)
            : this()
        {
            foreach (DictionaryProperty prop in dict)
            {
                string localized = prop.LocalizedName;
                bool required = prop.Required;
                bool enumerable = prop.Enumerable;
                string defaultValue = prop.DefaultValue;

                if (required)
                {
                    if (enumerable)
                    {
                        string [] values = (prop as EnumerableDictionaryProperty).Values;
                        DataGridViewRow row = AddRequiredEnumerableProperty(localized, defaultValue, values);
                        if (values.Length > 0)
                            row.Cells[1].Value = values[0];
                    }
                    else
                        AddRequiredProperty(localized, defaultValue);
                }
                else
                {
                    if (enumerable)
                    {
                        string[] values = (prop as EnumerableDictionaryProperty).Values;
                        AddOptionalEnumerableProperty(localized, defaultValue, values);
                    }
                    else
                        AddProperty(localized, defaultValue);
                }
            }
        }

        public DictionaryDialog(SchemaAttributeDictionary dict)
            : this()
        {
            foreach (string name in dict.AttributeNames)
            {
                string value = dict.GetAttributeValue(name);
                AddProperty(name, value);
            }
        }

        public DictionaryDialog(IConnectionPropertyDictionary dict)
            : this()
        {
            foreach (string name in dict.PropertyNames)
            {
                string localized = dict.GetLocalizedName(name);
                bool required = dict.IsPropertyRequired(name);
                bool enumerable = dict.IsPropertyEnumerable(name);
                string defaultValue = dict.GetPropertyDefault(name);
                
                string[] values = dict.EnumeratePropertyValues(name);
                
                if (required)
                {
                    if (enumerable)
                    {
                        DataGridViewRow row = AddRequiredEnumerableProperty(localized, defaultValue, values);
                        if(values.Length > 0)
                            row.Cells[1].Value = values[0];
                    }
                    else
                        AddRequiredProperty(localized, defaultValue);
                }
                else
                {
                    if (enumerable)
                        AddOptionalEnumerableProperty(localized, defaultValue, values);
                    else
                        AddProperty(localized, defaultValue);
                }
            }
        }

        private DataGridViewRow AddRequiredProperty(string name, string defaultValue)
        {
            //TODO: Attach a validation scheme
            return AddProperty(name, defaultValue);
        }

        private DataGridViewRow AddRequiredEnumerableProperty(string name, string defaultValue, IEnumerable<string> values)
        {
            //TODO: Attach a validation scheme
            return AddOptionalEnumerableProperty(name, defaultValue, values);
        }

        private DataGridViewRow AddOptionalEnumerableProperty(string name, string defaultValue, IEnumerable<string> values)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewComboBoxCell valueCell = new DataGridViewComboBoxCell();
            valueCell.DataSource = values;
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);
            grdProperties.Rows.Add(row);
            return row;
        }

        private DataGridViewRow AddProperty(string name, string defaultValue)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdProperties.Rows.Add(row);
            return row;
        }

        public NameValueCollection GetProperties()
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (DataGridViewRow row in grdProperties.Rows)
            {
                string name = row.Cells[0].Value.ToString();
                string value = (row.Cells[1].Value != null) ? row.Cells[1].Value.ToString() : string.Empty;
                nvc.Add(name, value);
            }
            return nvc;
        }

        public static NameValueCollection GetParameters(string title, IEnumerable<DictionaryProperty> dict)
        {
            DictionaryDialog diag = new DictionaryDialog(dict);
            diag.Text = title;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.GetProperties();
            }
            return null;
        }

        public static NameValueCollection GetParameters(string title, SchemaAttributeDictionary dict)
        {
            DictionaryDialog diag = new DictionaryDialog(dict);
            diag.Text = title;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.GetProperties();
            }
            return null;
        }

        public static NameValueCollection GetParameters(string title, IConnectionPropertyDictionary dict)
        {
            DictionaryDialog diag = new DictionaryDialog(dict);
            diag.Text = title;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.GetProperties();
            }
            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}