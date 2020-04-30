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
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A view that allows the creation of FDO connection in a generic fashion
    /// </summary>
    [ToolboxItem(false)]
    public partial class FdoConnectCtl : ViewContent, IFdoConnectView
    {
        private FdoConnectCtlPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnectCtl"/> class.
        /// </summary>
        public FdoConnectCtl()
        {
            InitializeComponent();
            InitializeGrid();
            this.Title = ResourceService.GetString("TITLE_NEW_DATA_CONNECTION");
            _presenter = new FdoConnectCtlPresenter(this, ServiceManager.Instance.GetService<FdoConnectionManager>());
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            _presenter.GetProviderList();
            _presenter.ProviderChanged();
            base.OnLoad(e);
        }

        private void InitializeGrid()
        {
            grdProperties.Rows.Clear();
            grdProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn
            {
                Name = "COL_NAME",
                HeaderText = "Name",
                ReadOnly = true
            };
            DataGridViewColumn colValue = new DataGridViewColumn
            {
                Name = "COL_VALUE",
                HeaderText = "Value",

                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            grdProperties.Columns.Add(colName);
            grdProperties.Columns.Add(colValue);
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <value>The name of the connection.</value>
        public string ConnectionName => txtConnectionName.Text;

        /// <summary>
        /// Gets the selected provider.
        /// </summary>
        /// <value>The selected provider.</value>
        public FdoToolbox.Core.Feature.FdoProviderInfo SelectedProvider => cmbProvider.SelectedItem as FdoProviderInfo;

        /// <summary>
        /// Gets the connect properties.
        /// </summary>
        /// <value>The connect properties.</value>
        public System.Collections.Specialized.NameValueCollection ConnectProperties
        {
            get 
            {
                NameValueCollection props = new NameValueCollection();
                foreach (DataGridViewRow row in grdProperties.Rows)
                {
                    object n = row.Cells[0].Value;
                    object v = row.Cells[1].Value;
                    if (n != null && v != null && v.ToString().Trim().Length > 0)
                    {
                        props.Add(n.ToString(), v.ToString());
                    }
                }
                return props;
            }
        }

        /// <summary>
        /// Flags the name error.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void FlagNameError(string msg)
        {
            errorProvider1.Clear();
            errorProvider1.SetError(txtConnectionName, msg);
        }

        /// <summary>
        /// Sets the provider list.
        /// </summary>
        /// <value>The provider list.</value>
        public IList<FdoProviderInfo> ProviderList
        {
            set 
            {
                cmbProvider.DisplayMember = "DisplayName";
                cmbProvider.DataSource = value;
            }
        }

        /// <summary>
        /// Resets the grid.
        /// </summary>
        public void ResetGrid()
        {
            grdProperties.Rows.Clear();
        }

        /// <summary>
        /// Adds the enumerable property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="values">The values.</param>
        public void AddEnumerableProperty(string name, string defaultValue, string[] values)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell
            {
                Value = name
            };
            DataGridViewComboBoxCell valueCell = new DataGridViewComboBoxCell
            {
                DataSource = values,
                Value = defaultValue
            };
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdProperties.Rows.Add(row);
        }

        private void cmbProvider_SelectionChanged(object sender, EventArgs e)
        {
            using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
            {
                pwdCells.Clear();
                _presenter.ProviderChanged();
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
            {
                _presenter.TestConnection();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
            {
                if (!_presenter.Connect())
                    this.ShowError("Failed to connect");
                else
                    base.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="p">The p.</param>
        public void AddProperty(FdoToolbox.Core.Connections.DictionaryProperty p)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell
            {
                Value = p.LocalizedName
            };

            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            if (p.IsFile || p.IsPath)
            {
                valueCell.ContextMenuStrip = ctxHelper;
                valueCell.ToolTipText = "Right click for helpful options";
            }
            valueCell.Value = p.DefaultValue;

            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);
            
            grdProperties.Rows.Add(row);

            if (p.Protected)
                pwdCells.Add(valueCell);
        }

        private void grdProperties_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                grdProperties.CurrentCell = grdProperties.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void insertCurrentApplicationPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grdProperties.CurrentCell.Value = FileUtility.ApplicationRootPath;
        }

        private void insertFilePathOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile(ResourceService.GetString("TITLE_OPEN_FILE"), ResourceService.GetString("FILTER_ALL"));
            if (FileService.FileExists(file))
            {
                grdProperties.CurrentCell.Value = file;
            }
        }

        private void insertFilePathSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_FILE"), ResourceService.GetString("FILTER_ALL"));
            if (file != null)
            {
                grdProperties.CurrentCell.Value = file;
            }
        }

        private void insertDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = FileService.GetDirectory(ResourceService.GetString("TITLE_SELECT_DIRECTORY"));
            if (dir != null)
            {
                grdProperties.CurrentCell.Value = dir;
            }
        }


        /// <summary>
        /// Sets a value indicating whether [config enabled].
        /// </summary>
        /// <value><c>true</c> if [config enabled]; otherwise, <c>false</c>.</value>
        public bool ConfigEnabled
        {
            set 
            {
                txtConfiguration.Enabled = btnBrowse.Enabled = value;
            }
        }

        /// <summary>
        /// Gets the config file.
        /// </summary>
        /// <value>The config file.</value>
        public string ConfigFile => txtConfiguration.Text;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtConfiguration.Text = FileService.OpenFile(ResourceService.GetString("TITLE_LOAD_CONFIGURATION"), ResourceService.GetString("FILTER_XML_FILES"));
        }

        /// <summary>
        /// Flags the config error.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void FlagConfigError(string msg)
        {
            errorProvider1.SetError(txtConfiguration, msg);
        }

        private void grdProperties_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell cell = grdProperties.SelectedCells[0];
            if (cell != null)
            {
                TextBox t = e.Control as TextBox;
                if (t != null)
                {
                    t.UseSystemPasswordChar = IsPasswordCell(cell);
                }
            }
        }

        private List<DataGridViewCell> pwdCells = new List<DataGridViewCell>();

        private bool IsPasswordCell(DataGridViewCell cell)
        {
            return pwdCells.Contains(cell);
        }

        private void grdProperties_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = grdProperties.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell != null && IsPasswordCell(cell) && cell.Value != null)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                    Graphics g = e.Graphics;
                    g.DrawString(new string('*', cell.Value.ToString().Length), this.Font, new SolidBrush(Color.Black), e.CellBounds);
                    e.Handled = true;
                }
            }
        }
    }
}
