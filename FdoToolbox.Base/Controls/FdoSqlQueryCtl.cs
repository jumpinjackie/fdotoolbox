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
using ICSharpCode.TextEditor;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A <see cref="FdoDataPreviewCtl"/> sub-view for raw SQL queries
    /// </summary>
    internal partial class FdoSqlQueryCtl : UserControl, IFdoSqlQueryView
    {
        private TextEditorControl _editor;

        public FdoSqlQueryCtl()
        {
            InitializeComponent();
            _editor = new TextEditorControl();
            _editor.Dock = DockStyle.Fill;
            _editor.SetHighlighting("SQL");
            this.Controls.Add(_editor);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string SQLString
        {
            get { return _editor.Text; }
        }

        public void FireMapPreviewStateChanged(bool enabled)
        {
            MapPreviewStateChanged(this, enabled);
        }

        public event MapPreviewStateEventHandler MapPreviewStateChanged = delegate { };

        public void SetRestrictions(FdoToolbox.Core.Feature.ICapability cap)
        {
            
        }
    }
}
