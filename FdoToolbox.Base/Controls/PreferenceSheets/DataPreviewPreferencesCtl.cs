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
using System.Windows.Forms;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls.PreferenceSheets
{
    internal partial class DataPreviewPreferencesCtl : UserControl, IPreferenceSheet
    {
        public DataPreviewPreferencesCtl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            numLimit.Minimum = decimal.MinValue;
            numLimit.Maximum = decimal.MaxValue;
            numLimit.Value = Convert.ToDecimal(Preferences.DataPreviewWarningLimit);
            chkRandomTheme.Checked = Preferences.DataPreviewRandomColors;
            base.OnLoad(e);
        }

        public string Title => ResourceService.GetString("TITLE_PREFS_DATA_PREVIEW");

        public Control ContentControl => this;

        public void ApplyChanges()
        {
            Preferences.DataPreviewWarningLimit = Convert.ToInt32(numLimit.Value);
            Preferences.DataPreviewRandomColors = chkRandomTheme.Checked;
        }
    }
}
