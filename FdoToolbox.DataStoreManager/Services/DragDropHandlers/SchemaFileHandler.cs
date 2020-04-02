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
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base;
using FdoToolbox.Base.Services.DragDropHandlers;
using FdoToolbox.DataStoreManager.Controls;

namespace FdoToolbox.DataStoreManager.Services.DragDropHandlers
{
    /// <summary>
    /// Drag and Drop handler for feature schema files
    /// </summary>
    public class SchemaFileHandler : IDragDropHandler
    {
        string[] extensions = { ".schema" };

        /// <summary>
        /// Gets the file extension this handler can handle
        /// </summary>
        /// <value></value>
        public string[] FileExtensions
        {
            get { return extensions; }
        }

        /// <summary>
        /// Handles the file drop
        /// </summary>
        /// <param name="file">The file being dropped</param>
        public void HandleDrop(string file)
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                //FdoSchemaDesignerCtl ctl = new FdoSchemaDesignerCtl();
                //ctl.LoadSchema(file);
                //wb.ShowContent(ctl, ViewRegion.Document);
            }
        }

        /// <summary>
        /// Gets a description of the action this handler will take
        /// </summary>
        /// <value></value>
        public string HandlerAction
        {
            get { return "Open in Standalone Schema Editor"; }
        }
    }
}
