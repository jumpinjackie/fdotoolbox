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
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using System.ComponentModel;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A text editor component
    /// </summary>
    [ToolboxItem(false)]
    public class TextEditor : ViewContent
    {
        ToolStrip toolstrip;
        TextEditorControl editor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditor"/> class.
        /// </summary>
        public TextEditor()
        {
            InitializeComponent();

            toolstrip = ToolbarService.CreateToolStrip(this, "/FdoToolbox/TextEditor/Toolbar");
            editor = new TextEditorControl
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(editor);
            this.Controls.Add(toolstrip);
        }

        private void InitializeComponent()
        {
            
        }

        /// <summary>
        /// The title of the view
        /// </summary>
        /// <value></value>
        public override string Title => ResourceService.GetString("TITLE_TEXT_EDITOR");

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return true;
        }

        /// <summary>
        /// Saves as.
        /// </summary>
        /// <returns></returns>
        public bool SaveAs()
        {
            return true;
        }
    }
}
