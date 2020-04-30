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
using System.Windows.Forms;
using ICSharpCode.Core;
using System.ComponentModel;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// The Object Explorer
    /// </summary>
    [ToolboxItem(false)]
    public partial class ObjectExplorer : ViewContent, IObjectExplorer
    {
        private ToolStrip objToolStrip;
        private TreeView objTreeView;
        private ImageList imgList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectExplorer"/> class.
        /// </summary>
        public ObjectExplorer()
        {
            imgList = new ImageList();
            InitTreeView();
            objToolStrip = ToolbarService.CreateToolStrip(this, "/ObjectExplorer/Toolbar");
            
            this.Controls.Add(objTreeView);
            this.Controls.Add(objToolStrip);
        }

        private void InitTreeView()
        {
            objTreeView = new TreeView
            {
                ShowLines = true,
                ShowNodeToolTips = true,
                ShowPlusMinus = true,
                ShowRootLines = true,
                ImageList = imgList,
                Dock = DockStyle.Fill,
                AllowDrop = true
            };
            objTreeView.DragDrop += new DragEventHandler(FileDragAndDropHandler.OnDragDrop);
            objTreeView.DragEnter += new DragEventHandler(FileDragAndDropHandler.OnDragEnter);
            objTreeView.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    objTreeView.SelectedNode = objTreeView.GetNodeAt(e.X, e.Y);
                }
            };
            objTreeView.AfterSelect += new TreeViewEventHandler(OnAfterSelect);
            objTreeView.AfterExpand += new TreeViewEventHandler(OnAfterExpand);
        }

        void OnAfterExpand(object sender, TreeViewEventArgs e)
        {
            AfterExpansion(this, e);   
        }

        void OnAfterSelect(object sender, TreeViewEventArgs e)
        {
            AfterSelection(this, e);
        }

        /// <summary>
        /// Detrmines if this view can be closed
        /// </summary>
        /// <value></value>
        public override bool CanClose => false;

        /// <summary>
        /// The title of the view
        /// </summary>
        /// <value></value>
        public override string Title => ResourceService.GetString("UI_OBJECT_EXPLORER");

        /// <summary>
        /// Registers an image resource in the Object Explorer
        /// </summary>
        /// <param name="imgResource">The img resource.</param>
        public void RegisterImage(string imgResource)
        {
            if (!imgList.Images.ContainsKey(imgResource))
                imgList.Images.Add(imgResource, ResourceService.GetBitmap(imgResource));
        }

        private Dictionary<string, ContextMenuStrip> _ContextMenus = new Dictionary<string, ContextMenuStrip>();

        /// <summary>
        /// Registers a context menu in the Object Explorer
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="addInTreePath">The add in tree path.</param>
        public void RegisterContextMenu(string nodeType, string addInTreePath)
        {
            if(_ContextMenus.ContainsKey(nodeType))
                throw new ArgumentException("A context menu has already been registered under: " + nodeType);

            _ContextMenus[nodeType] = MenuService.CreateContextMenu(this, addInTreePath);
        }

        /// <summary>
        /// Gets the selected node in the Object Explorer
        /// </summary>
        /// <returns></returns>
        public TreeNode GetSelectedNode()
        {
            return objTreeView.SelectedNode;
        }

        /// <summary>
        /// Registers the root node in the Object Explorer
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        /// <param name="imgResource">The img resource.</param>
        /// <param name="addInTreePath">The add in tree path.</param>
        public void RegisterRootNode(string name, string text, string imgResource, string addInTreePath)
        {
            if (!imgList.Images.ContainsKey(imgResource))
            {
                imgList.Images.Add(imgResource, ResourceService.GetBitmap(imgResource));
            }

            TreeNode node = new TreeNode
            {
                Name = name,
                Text = text
            };
            node.ImageKey = node.SelectedImageKey = imgResource;
            node.ContextMenuStrip = MenuService.CreateContextMenu(node, addInTreePath);
            objTreeView.Nodes.Add(node);
        }

        /// <summary>
        /// Gets the root node in the Object Explorer
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TreeNode GetRootNode(string name)
        {
            return objTreeView.Nodes[name];
        }

        /// <summary>
        /// Gets the context menu.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns></returns>
        public ContextMenuStrip GetContextMenu(string nodeType)
        {
            if (_ContextMenus.ContainsKey(nodeType))
                return _ContextMenus[nodeType];
            return null;
        }

        /// <summary>
        /// Occurs when a node has expanded in the Object Explorer
        /// </summary>
        public event TreeViewEventHandler AfterExpansion = delegate { };

        /// <summary>
        /// Occurs when a node has been selected in the Object Explorer
        /// </summary>
        public event TreeViewEventHandler AfterSelection = delegate { };
    }
}
