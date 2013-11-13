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
using log4net.Appender;
using log4net.Repository.Hierarchy;
using log4net;
using System.IO;
using log4net.Core;
using log4net.Config;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// The application console
    /// </summary>
    internal partial class ConsolePane : ViewContent, IViewContent, IConsole
    {
        private ToolStrip toolStrip1;
        private RichTextBox txtConsole;
        private TextBox inputBox;

        public ConsolePane()
        {
            inputBox = new TextBox();
            txtConsole = new RichTextBox();
            txtConsole.Font = new Font(FontFamily.GenericMonospace, 8.0f);
            toolStrip1 = ToolbarService.CreateToolStrip(this, "/AppConsole/Toolbar");
            
            txtConsole.Dock = DockStyle.Fill;
            inputBox.Dock = DockStyle.Bottom;

            this.Controls.Add(txtConsole);
            this.Controls.Add(inputBox);
            this.Controls.Add(toolStrip1);
        }

        protected override void OnLoad(EventArgs e)
        {
            TextBoxAppender.SetTextBox(txtConsole);
            //HACK to force a write of buffered logs
            LoggingService.Info("Console Initialized"); 
            base.OnLoad(e);
        }

        public override string Title
        {
            get { return ResourceService.GetString("UI_CONSOLE"); }
        }

        public override bool CanClose
        {
            get { return false; }
        }

        public bool Save()
        {
            return false;
        }

        public bool SaveAs()
        {
            return false;
        }

        public string TextContent
        {
            get { return txtConsole.Text; }
        }

        public void Clear()
        {
            txtConsole.Clear();
        }
    }
}
