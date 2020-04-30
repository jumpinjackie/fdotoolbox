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

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// An extender interface that extends the Object Explorer with custom
    /// root nodes and context menus.
    /// </summary>
    public interface IObjectExplorerExtender
    {
        /// <summary>
        /// Extends the Object Explorer instance
        /// </summary>
        /// <param name="explorer">The Object Explorer instance.</param>
        void Decorate(IObjectExplorer explorer);
    }
}
