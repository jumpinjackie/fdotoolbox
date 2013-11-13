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
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls.PreferenceSheets
{
    internal partial class ScriptPreferencesCtl : UserControl, IPreferenceSheet
    {
        public ScriptPreferencesCtl()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_PREFS_SCRIPT"); }
        }

        public Control ContentControl
        {
            get { return this; }
        }

        protected override void OnLoad(EventArgs e)
        {
            this.ScriptPaths = Preferences.ScriptModulePaths;
            this.ScriptDebug = Preferences.ScriptDebug;
            base.OnLoad(e);
        }

        public string[] ScriptPaths
        {
            get 
            {
                List<string> values = new List<string>();
                foreach (object obj in lstModulePaths.Items)
                {
                    values.Add(obj.ToString());
                }
                return values.ToArray();
            }
            set 
            {
                lstModulePaths.Items.Clear();
                foreach (string path in value)
                {
                    lstModulePaths.Items.Add(path);
                }
            }
        }

        public bool ScriptDebug
        {
            get { return chkDebug.Checked; }
            set { chkDebug.Checked = value; }
        }

        public void ApplyChanges()
        {
            Preferences.ScriptDebug = this.ScriptDebug;
            Preferences.ScriptModulePaths = this.ScriptPaths;
        }

        private void lstModulePaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeletePath.Enabled = (lstModulePaths.SelectedIndex >= 0);
        }

        private void btnAddPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                if (lstModulePaths.Items.IndexOf(diag.SelectedPath) < 0)
                {
                    lstModulePaths.Items.Add(diag.SelectedPath);
                }
            }
        }
    }
}
