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
using FdoToolbox.Base.Services.DragDropHandlers;
using System.IO;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// Drag and Drop handler service
    /// </summary>
    public class DragDropHandlerService : IDragDropHandlerService
    {
        private Dictionary<string, List<IDragDropHandler>> _handlers;

        /// <summary>
        /// Registers a drag and drop handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler(IDragDropHandler handler)
        {
            foreach (string fileExt in handler.FileExtensions)
            {
                string ext = fileExt.ToUpper();
                if (!_handlers.ContainsKey(ext))
                    _handlers[ext] = new List<IDragDropHandler>();

                _handlers[ext].Add(handler);
            }
        }

        /// <summary>
        /// Gets the registered handlers for this particular file type
        /// </summary>
        /// <param name="file">The file being dropped</param>
        /// <returns>A list of registered handlers</returns>
        public IList<IDragDropHandler> GetHandlersForFile(string file)
        {
            string ext = Path.GetExtension(file).ToUpper();
            if (_handlers.ContainsKey(ext))
                return _handlers[ext];

            return new List<IDragDropHandler>();
        }

        private bool _init = false;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return _init; }
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        public void InitializeService()
        {
            _handlers = new Dictionary<string, List<IDragDropHandler>>();
            _init = true;
            this.Initialize(this, EventArgs.Empty);
        }

        /// <summary>
        /// Unloads the service.
        /// </summary>
        public void UnloadService()
        {
            this.Unload(this, EventArgs.Empty);            
        }

        /// <summary>
        /// Loads any persisted objects from the session directory
        /// </summary>
        public void Load()
        {
            
        }

        /// <summary>
        /// Persists any managed objects to the session directory
        /// </summary>
        public void Save()
        {
            
        }

        /// <summary>
        /// Occurs when the service is initialized
        /// </summary>
        public event EventHandler Initialize = delegate { };

        /// <summary>
        /// Occurs when the service is unloaded
        /// </summary>
        public event EventHandler Unload = delegate { };


        /// <summary>
        /// Gets the list of file extensions being handled
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetHandledExtensions()
        {
            return _handlers.Keys;
        }
    }
}
