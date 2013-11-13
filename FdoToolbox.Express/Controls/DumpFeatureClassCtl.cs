#region LGPL Header
// Copyright (C) 2011, Jackie Ng
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
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Base;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Express.Controls
{
    public partial class DumpFeatureClassCtl : ViewContent
    {
        private FdoConnection _source;
        private string _schemaName;
        private string _className;

        public DumpFeatureClassCtl(FdoConnection source, string schemaName, string className)
        {
            InitializeComponent();
            this.Title = "Dump Feature Class";
            _source = source;
            _schemaName = schemaName;
            _className = className;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSavePath.Text))
            {
                MessageService.ShowError("Please specify a file name to save to");
                return;
            }
            string provider = rdSdf.Checked ? "OSGeo.SDF" : "OSGeo.SQLite";

            using (FdoBulkCopy bcp = ExpressUtility.CreateBulkCopy(_source, _schemaName, _className, provider, txtSavePath.Text))
            {
                EtlProcessCtl ctl = new EtlProcessCtl(bcp);
                Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
                this.Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var save = new SaveFileDialog())
            {
                if (rdSdf.Checked)
                    save.Filter = Strings.FILTER_SDF;
                else if (rdSqlite.Checked)
                    save.Filter = Strings.FILTER_SQLITE;

                if (save.ShowDialog() == DialogResult.OK)
                {
                    txtSavePath.Text = save.FileName;
                }
            }
        }
    }
}
