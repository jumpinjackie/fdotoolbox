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
using FdoToolbox.Base.Services.DragDropHandlers;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;

namespace FdoToolbox.Express.DragDropHandlers
{
    /// <summary>
    /// Drag and Drop handler for MS Access databases
    /// </summary>
    public class MsAccessFileHandler : IDragDropHandler
    {
        /// <summary>
        /// Gets a description of the action this handler will take
        /// </summary>
        /// <value></value>
        public string HandlerAction => "Create new ODBC (Access) connection";

        /// <summary>
        /// Gets the file extension this handler can handle
        /// </summary>
        /// <value></value>
        public string[] FileExtensions { get; } = { ".mdb" };

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
                conn = new FdoConnection("OSGeo.ODBC", "ConnectionString=\"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + file + "\"");
            }
            catch (Exception ex)
            {
                LoggingService.Error("Failed to load connection", ex);
                return;
            }

            string name = namer.GetDefaultConnectionName(conn.Provider, System.IO.Path.GetFileNameWithoutExtension(file));
            connMgr.AddConnection(name, conn);
        }
    }
}
