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
using FdoToolbox.Base.Controls;
using FdoToolbox.Base.Services;
using System.IO;
using ICSharpCode.Core;

// TODO: Load scripts in own AppDomain, so they can be un-loaded

namespace FdoToolbox.Base.Scripting
{
    /// <summary>
    /// The application script manager view
    /// </summary>
    [ToolboxItem(false)]
    public partial class ScriptManager : ViewContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptManager"/> class.
        /// </summary>
        public ScriptManager()
        {
            InitializeComponent();
            this.Title = ResourceService.GetString("CMD_ScriptManager");
        }

        /// <summary>
        /// Detrmines if this view can be closed
        /// </summary>
        /// <value></value>
        public override bool CanClose
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            InitScriptItems();
            ScriptingEngine.Instance.ScriptLoaded += new ScriptEventHandler(OnScriptLoaded);
            base.OnLoad(e);
        }

        void OnScriptLoaded(ApplicationScript script)
        {
            AddScriptItem(script.Path);
        }

        void OnScriptUnloaded(ApplicationScript script)
        {
            treeScripts.Nodes.RemoveByKey(script.Path);
            UpdateButtonStates();
        }

        private void AddScriptItem(string path)
        {
            TreeNode node = new TreeNode();
            node.Name = path;
            node.Text = Path.GetFileName(path);
            node.Tag = path;
            treeScripts.Nodes.Add(node);
            UpdateButtonStates();
        }

        private void InitScriptItems()
        {
            ICollection<string> scriptPaths = ScriptingEngine.Instance.LoadedScripts;
            foreach (string scr in scriptPaths)
            {
                AddScriptItem(scr);
            }
        }

        /// <summary>
        /// Gets the selected script.
        /// </summary>
        /// <value>The selected script.</value>
        public string SelectedScript
        {
            get
            {
                if (treeScripts.SelectedNode != null) 
                {
                    return treeScripts.SelectedNode.Tag.ToString();
                }
                return string.Empty;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile("Load Script", "IronPython Scripts (*.py)|*.py");
            if (!string.IsNullOrEmpty(file))
            {
                ScriptingEngine.Instance.LoadScript(file);
            }
        }

        private void treeScripts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            btnUnload.Enabled = btnRun.Enabled = btnReload.Enabled = (treeScripts.SelectedNode != null);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string script = this.SelectedScript;
            if (!string.IsNullOrEmpty(script))
            {
                ScriptingEngine.Instance.InvokeLoadedScript(script);
            }
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            string script = this.SelectedScript;
            if (!string.IsNullOrEmpty(script))
            {
                ScriptingEngine.Instance.UnloadScript(script);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            string script = this.SelectedScript;
            if (!string.IsNullOrEmpty(script))
            {
                ScriptingEngine.Instance.RecompileScript(script);
                LoggingService.Info("Script re-loaded: " + script);
            }
        }
    }
}
