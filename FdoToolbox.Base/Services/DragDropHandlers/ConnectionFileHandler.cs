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
using System.Text;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Services.DragDropHandlers
{
    /// <summary>
    /// Drag and drop handler for saved connections
    /// </summary>
    public class ConnectionFileHandler : IDragDropHandler
    {
        /// <summary>
        /// Gets a description of the action this handler will take
        /// </summary>
        /// <value></value>
        public string HandlerAction
        {
            get { return "Create new connection"; }
        }

        string [] extensions = { ".conn" };

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
            IFdoConnectionManager connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
            NamingService namer = ServiceManager.Instance.GetService<NamingService>();
            FdoConnection conn = null;
            try
            {
                conn = FdoConnection.LoadFromFile(file, true);
            }
            catch (Exception ex)
            {
                LoggingService.Error("Failed to load connection", ex);
                return;
            }

            string name = namer.GetDefaultConnectionName(conn.Provider);
            connMgr.AddConnection(name, conn);
        }
    }
}
