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
using System.Windows.Forms;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// Defines an abstract interface to the Object Explorer
    /// </summary>
    public interface IObjectExplorer : IViewContent
    {
        /// <summary>
        /// Registers the root node in the Object Explorer
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        /// <param name="imgResource">The img resource.</param>
        /// <param name="addInTreePath">The add in tree path.</param>
        void RegisterRootNode(string name, string text, string imgResource, string addInTreePath);
        /// <summary>
        /// Registers an image resource in the Object Explorer
        /// </summary>
        /// <param name="imgResource">The img resource.</param>
        void RegisterImage(string imgResource);
        /// <summary>
        /// Registers a context menu in the Object Explorer
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="addInTreePath">The add in tree path.</param>
        void RegisterContextMenu(string nodeType, string addInTreePath);
        /// <summary>
        /// Gets the selected node in the Object Explorer
        /// </summary>
        /// <returns></returns>
        TreeNode GetSelectedNode();
        /// <summary>
        /// Gets the root node in the Object Explorer
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        TreeNode GetRootNode(string name);
        /// <summary>
        /// Gets the context menu.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns></returns>
        ContextMenuStrip GetContextMenu(string nodeType);

        /// <summary>
        /// Occurs when a node has expanded in the Object Explorer
        /// </summary>
        event TreeViewEventHandler AfterExpansion;
        /// <summary>
        /// Occurs when a node has been selected in the Object Explorer
        /// </summary>
        event TreeViewEventHandler AfterSelection;
    }
}
