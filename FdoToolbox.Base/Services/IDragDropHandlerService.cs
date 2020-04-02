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
using FdoToolbox.Base.Services.DragDropHandlers;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// A service to register custom drag and drop file handlers
    /// </summary>
    public interface IDragDropHandlerService : IService
    {
        /// <summary>
        /// Registers a drag and drop handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void RegisterHandler(IDragDropHandler handler);

        /// <summary>
        /// Gets the registered handlers for this particular file type
        /// </summary>
        /// <param name="file">The file being dropped</param>
        /// <returns>A list of registered handlers</returns>
        IList<IDragDropHandler> GetHandlersForFile(string file);

        /// <summary>
        /// Gets the list of file extensions being handled
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetHandledExtensions();
    }
}
