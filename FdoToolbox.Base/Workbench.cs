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
using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base
{
    /// <summary>
    /// Represents the main application window
    /// </summary>
    public sealed class Workbench : Form
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Workbench Instance { get; private set; }

        /// <summary>
        /// Occurs when [workbench initialized].
        /// </summary>
        public static event EventHandler WorkbenchInitialized = delegate { };

        private static bool _init = false;

        /// <summary>
        /// Initializes the workbench.
        /// </summary>
        public static void InitializeWorkbench(string title)
        {
            if (!_init)
            {
                Instance = new Workbench();
                Instance.SetTitle(title);
                _init = true;
                WorkbenchInitialized(Instance, EventArgs.Empty);
            }
        }
 
        MenuStrip menu;
        ToolStripContainer toolStripContainer;
        ToolStrip toolbar;
        DockPanel contentPanel;
        StatusStrip status;
        ToolStripStatusLabel statusLabel;
        ContextMenuStrip ctxToolbar;

        /// <summary>
        /// Gets the console.
        /// </summary>
        /// <value>The console.</value>
        public IConsole Console { get; }

        /// <summary>
        /// Gets the object explorer.
        /// </summary>
        /// <value>The object explorer.</value>
        public IObjectExplorer ObjectExplorer { get; }

        private Workbench()
        {
            InitializeComponent();

            _toolstrips = new Dictionary<string, ToolStrip>();
            _toolstripRegions = new Dictionary<string, ToolbarRegion>();

            this.Icon = ResourceService.GetIcon("FdoToolbox");

            contentPanel = new DockPanel
            {
                DocumentStyle = DocumentStyle.DockingWindow,
                Dock = DockStyle.Fill,
                DockLeftPortion = 200,
                DockBottomPortion = 150,
                DockRightPortion = 200
            };

            menu = new MenuStrip();
            MenuService.AddItemsToMenu(menu.Items, this, "/Workbench/MainMenu");

            toolStripContainer = new ToolStripContainer();
            toolStripContainer.ContentPanel.Controls.Add(contentPanel);
            toolStripContainer.Dock = DockStyle.Fill;

            this.Controls.Add(toolStripContainer);

            ctxToolbar = new ContextMenuStrip();
            toolStripContainer.TopToolStripPanel.ContextMenuStrip = ctxToolbar;
            toolStripContainer.BottomToolStripPanel.ContextMenuStrip = ctxToolbar;
            toolStripContainer.LeftToolStripPanel.ContextMenuStrip = ctxToolbar;
            toolStripContainer.RightToolStripPanel.ContextMenuStrip = ctxToolbar;

            toolbar = ToolbarService.CreateToolStrip(this, "/Workbench/Toolbar");
            AddToolbar("Base", toolbar, ToolbarRegion.Top, false);

            status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            status.Items.Add(statusLabel);

            this.Controls.Add(menu);
            this.Controls.Add(status);

            //this.IsMdiContainer = true;

            ObjectExplorer exp = new ObjectExplorer();
            ObjectExplorer = exp;

            ConsolePane console = new ConsolePane();
            Console = console;

            ShowContent(console, ViewRegion.Bottom);
            ShowContent(exp, ViewRegion.Left);
            
            // Use the Idle event to update the status of menu and toolbar items.
            Application.Idle += OnApplicationIdle;
        }

        

        private Dictionary<string, ToolStrip> _toolstrips;
        private Dictionary<string, ToolbarRegion> _toolstripRegions;

        /// <summary>
        /// Adds the toolbar.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="toolbar">The toolbar.</param>
        /// <param name="region">The region.</param>
        /// <param name="canToggleVisibility">if set to <c>true</c> [can toggle visibility].</param>
        public void AddToolbar(string name, ToolStrip toolbar, ToolbarRegion region, bool canToggleVisibility)
        {
            _toolstrips.Add(name, toolbar);
            _toolstripRegions.Add(name, region);

            if (canToggleVisibility)
            {
                ToolStripMenuItem item = new ToolStripMenuItem
                {
                    Text = name,
                    Tag = name,
                    Checked = true,
                    CheckOnClick = true
                };
                item.Click += delegate
                {
                    SetToolbarVisibility(name, item.Checked);
                };
                ctxToolbar.Items.Add(item);
            }

            switch (region)
            {
                case ToolbarRegion.Top:
                    toolStripContainer.TopToolStripPanel.Controls.Add(toolbar);
                    break;
                case ToolbarRegion.Bottom:
                    toolStripContainer.BottomToolStripPanel.Controls.Add(toolbar);
                    break;
                case ToolbarRegion.Left:
                    toolStripContainer.LeftToolStripPanel.Controls.Add(toolbar);
                    break;
                case ToolbarRegion.Right:
                    toolStripContainer.RightToolStripPanel.Controls.Add(toolbar);
                    break;
            }
        }

        /// <summary>
        /// Sets the toolbar visibility.
        /// </summary>
        /// <param name="toolbarName">Name of the toolbar.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        public void SetToolbarVisibility(string toolbarName, bool visible)
        {
            ToolStrip strip = GetToolbar(toolbarName);
            if (strip != null)
            {
                ToolbarRegion region = _toolstripRegions[toolbarName];
                if (visible)
                {
                    switch (region)
                    {
                        case ToolbarRegion.Bottom:
                            toolStripContainer.BottomToolStripPanel.Controls.Add(strip);
                            break;
                        case ToolbarRegion.Left:
                            toolStripContainer.LeftToolStripPanel.Controls.Add(strip);
                            break;
                        case ToolbarRegion.Right:
                            toolStripContainer.RightToolStripPanel.Controls.Add(strip);
                            break;
                        case ToolbarRegion.Top:
                            toolStripContainer.TopToolStripPanel.Controls.Add(strip);
                            break;
                    }
                }
                else
                {
                    switch (region)
                    {
                        case ToolbarRegion.Bottom:
                            toolStripContainer.BottomToolStripPanel.Controls.Remove(strip);
                            break;
                        case ToolbarRegion.Left:
                            toolStripContainer.LeftToolStripPanel.Controls.Remove(strip);
                            break;
                        case ToolbarRegion.Right:
                            toolStripContainer.RightToolStripPanel.Controls.Remove(strip);
                            break;
                        case ToolbarRegion.Top:
                            toolStripContainer.TopToolStripPanel.Controls.Remove(strip);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the toolbar.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ToolStrip GetToolbar(string name)
        {
            if(_toolstrips.ContainsKey(name))
                return _toolstrips[name];
            return null;
        }

        /// <summary>
        /// Gets the toolbar names.
        /// </summary>
        /// <value>The toolbar names.</value>
        public ICollection<string> ToolbarNames => _toolstrips.Keys;

        /// <summary>
        /// Sets the status label.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetStatusLabel(string text)
        {
            statusLabel.Text = text;
        }

        /// <summary>
        /// Sets the title.
        /// </summary>
        /// <param name="title">The title.</param>
        public void SetTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Shows the content.
        /// </summary>
        /// <param name="vc">The vc.</param>
        /// <param name="region">The region.</param>
        public void ShowContent(IViewContent vc, ViewRegion region)
        {
            TabManager tm = ServiceManager.Instance.GetService<TabManager>();

            DockContent content = new DockContent
            {
                TabText = vc.Title,
                Text = vc.Title,
                ToolTipText = vc.Title,
                CloseButton = vc.CanClose
            };

            switch (region)
            {
                case ViewRegion.Bottom:
                    content.DockAreas = DockAreas.DockBottom;
                    break;
                case ViewRegion.Top:
                    content.DockAreas = DockAreas.DockTop;
                    break;
                case ViewRegion.Left:
                    content.DockAreas = DockAreas.DockLeft;
                    break;
                case ViewRegion.Right:
                    content.DockAreas = DockAreas.DockRight;
                    break;
                case ViewRegion.Document:
                    content.DockAreas = DockAreas.Document;
                    break;
                case ViewRegion.Floating:
                    content.DockAreas = DockAreas.Float;
                    break;
            }

            vc.TitleChanged += delegate(object sender, EventArgs e)
            {
                content.TabText = vc.Title;
                content.Text = vc.Title;
                content.ToolTipText = vc.Title;
            };

            vc.ViewContentClosing += delegate(object sender, EventArgs e)
            {
                if(vc.CanClose)
                    content.Close();
            };

            content.ClientSize = vc.ContentControl.Size;
            content.CloseButton = vc.CanClose;

            vc.ContentControl.Dock = DockStyle.Fill;
            content.Controls.Add(vc.ContentControl);

            if (vc is IConnectionDependentView)
            {
                tm.Register((IConnectionDependentView)vc);
            }

            if (region == ViewRegion.Dialog)
            {   
                content.StartPosition = FormStartPosition.CenterParent;
                content.ShowDialog();
            }
            else
            {
                content.Show(contentPanel);
            }
        }

        void OnApplicationIdle(object sender, EventArgs e)
        {
            // Use the Idle event to update the status of menu and toolbar.
            // Depending on your application and the number of menu items with complex conditions,
            // you might want to update the status less frequently.
            UpdateMenuItemStatus();
        }

        /// <summary>Update Enabled/Visible state of items in the main menu based on conditions</summary>
        void UpdateMenuItemStatus()
        {
            foreach (ToolStripItem item in menu.Items)
            {
                if (item is IStatusUpdate)
                    (item as IStatusUpdate).UpdateStatus();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Workbench
            // 
            this.ClientSize = new System.Drawing.Size(1264, 762);
            this.Name = "Workbench";
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Invokes the method in the GUI thread context
        /// </summary>
        /// <param name="del">The delegate to invoke</param>
        /// <param name="args">The arguments</param>
        public void InvokeMethod(Delegate del, params object [] args)
        {
            this.Invoke(del, args);
        }

        /// <summary>
        /// Invokes the method in the GUI thread context
        /// </summary>
        /// <param name="del">The delegate to invoke</param>
        public void InvokeMethod(Delegate del)
        {
            this.Invoke(del);
        }

        /// <summary>
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }
                // get our current "TopMost" value (ours will always be false though)
                bool top = TopMost;
                // make our form jump to the top of everything
                TopMost = true;
                // set it back to whatever it was
                TopMost = top;
            }
            base.WndProc(ref m);
        }
    }

    /// <summary>
    /// Defines the valid regions a toolbar can reside on a workbench
    /// </summary>
    public enum ToolbarRegion
    {
        /// <summary>
        /// On the top
        /// </summary>
        Top,
        /// <summary>
        /// On the left
        /// </summary>
        Left,
        /// <summary>
        /// On the right
        /// </summary>
        Right,
        /// <summary>
        /// On the bottom
        /// </summary>
        Bottom
    }
}
