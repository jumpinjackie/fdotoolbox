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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Forms
{
    public partial class FdoSqlCommandDialog : Form
    {
        private FdoSqlCommandDialog()
        {
            InitializeComponent();
        }

        private FdoConnection _conn;

        public FdoSqlCommandDialog(FdoConnection conn)
            : this()
        {
            _conn = conn;
        }

        protected override void OnLoad(EventArgs e)
        {
            txtSql.SetHighlighting("SQL");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            btnExecute.Enabled = btnClose.Enabled = false;
            txtAffected.Clear();
            txtAffected.AppendText("Executing SQL");
            this.Cursor = Cursors.WaitCursor;
            bgExecutor.RunWorkerAsync(txtSql.Text);
        }

        private void bgExecutor_DoWork(object sender, DoWorkEventArgs e)
        {
            string sql = e.Argument.ToString();

            using (var svc = _conn.CreateFeatureService())
            {
                int affected = svc.ExecuteSQLNonQuery(sql);
                e.Result = affected;
            }
        }

        private void bgExecutor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                txtAffected.AppendText(Environment.NewLine + "ERROR: " + e.Error.Message);
            }
            else
            {
                txtAffected.AppendText(string.Format("{0}{1} objects affected", Environment.NewLine, e.Result));
            }
            this.Cursor = Cursors.Default;
            btnExecute.Enabled = btnClose.Enabled = true;
        }
    }
}
