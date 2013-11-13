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

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// Service base class.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }
        /// <summary>
        /// Initializes the service.
        /// </summary>
        void InitializeService();
        /// <summary>
        /// Unloads the service.
        /// </summary>
        void UnloadService();

        /// <summary>
        /// Loads any persisted objects from the session directory
        /// </summary>
        void Load();
        /// <summary>
        /// Persists any managed objects to the session directory
        /// </summary>
        void Save();

        /// <summary>
        /// Occurs when the service is initialized
        /// </summary>
        event EventHandler Initialize;
        /// <summary>
        /// Occurs when the service is unloaded
        /// </summary>
        event EventHandler Unload;
    }
}
