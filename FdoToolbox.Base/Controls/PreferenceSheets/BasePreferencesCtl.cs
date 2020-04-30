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
using FdoToolbox.Base.Services;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls.PreferenceSheets
{
    /// <summary>
    /// User Interface for FDO Toolbox's preferences
    /// </summary>
    internal partial class BasePreferencesCtl : UserControl, IPreferenceSheet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePreferencesCtl"/> class.
        /// </summary>
        public BasePreferencesCtl()
        {
            InitializeComponent();
        }

        private void BasePreferencesCtl_Load(object sender, EventArgs e)
        {
            txtFdoPath.Text = Preferences.FdoPath;
            txtLogPath.Text = Preferences.LogPath;
            txtSession.Text = Preferences.SessionDirectory;
            txtWorking.Text = Preferences.WorkingDirectory;
        }

        private void btnFdo_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select FDO Path");
            if (path != null)
            {
                txtFdoPath.Text = path;
            }
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select Log Path");
            if (path != null)
            {
                txtLogPath.Text = path;
            }
        }

        private void btnWorking_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select Working Directory");
            if (path != null)
            {
                txtWorking.Text = path;
            }
        }

        private void btnSession_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select Session Path");
            if (path != null)
            {
                txtSession.Text = path;
            }
        }


        /// <summary>
        /// Gets the content control.
        /// </summary>
        /// <value>The content control.</value>
        public Control ContentControl => this;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title => ResourceService.GetString("TITLE_PREFS_GENERAL");

        /// <summary>
        /// Applies the changes.
        /// </summary>
        public void ApplyChanges()
        {
            Preferences.FdoPath = txtFdoPath.Text;
            Preferences.LogPath = txtLogPath.Text;
            Preferences.SessionDirectory = txtSession.Text;
            Preferences.WorkingDirectory = txtWorking.Text;
        }
    }
}
