#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.DataStoreManager.Controls
{
    /// <summary>
    /// A simple collapsible panel with basic properties for configuring header color/font
    /// and content color. 
    /// 
    /// This control works best when Dock = Top and any content below is also Dock = Top and this
    /// control was built with these assumptions in place.
    /// 
    /// Note that there is no designer support for this control (ie. Drag and drop does not do what
    /// you would hope it would do). The way to use this control is to derive from this class and add your custom 
    /// content there. Also this class does not appear in the VS Toolbox, thus you must apply ToolboxItemAttribute(true)
    /// on your derived classes if you want to make the control available for design.
    /// </summary>
    public partial class CollapsiblePanel : UserControl
    {
        protected CollapsiblePanel()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            btnCollapse.Enabled = !this.Collapsed;
            btnExpand.Enabled = this.Collapsed;
        }

        [Category("Collapsible Panel Header")]
        public Color HeaderBackgroundColor
        {
            get { return headerPanel.BackColor; }
            set { headerPanel.BackColor = value; }
        }

        [Category("Collapsible Panel Header")]
        public string HeaderText
        {
            get { return lblHeaderText.Text; }
            set { lblHeaderText.Text = value; }
        }

        [Category("Collapsible Panel Header")]
        public Font HeaderFont
        {
            get { return lblHeaderText.Font; }
            set { lblHeaderText.Font = value; }
        }

        [Category("Collapsible Panel Content")]
        public Color ContentBackgroundColor
        {
            get { return contentPanel.BackColor; }
            set { contentPanel.BackColor = value; }
        }

        [DefaultValue(true)]
        [Category("Collapsible Panel")]
        public bool CanCollapse
        {
            get { return btnCollapse.Visible; }
            set { btnCollapse.Visible = value; }
        }

        [DefaultValue(true)]
        [Category("Collapsible Panel")]
        public bool CanExpand
        {
            get { return btnExpand.Visible; }
            set { btnExpand.Visible = value; }
        }

        private bool _collapsed;

        [DefaultValue(false)]
        [Category("Collapsible Panel")]
        public bool Collapsed
        {
            get
            {
                return _collapsed;
            }
            set
            {
                _collapsed = value;
                if (value)
                {
                    if (contentPanel.Height > 0)
                    {
                        restoreHeight = contentPanel.Height;
                        this.Height -= restoreHeight;
                    }
                }
                else
                {
                    if (contentPanel.Height < restoreHeight)
                    {
                        this.Height += restoreHeight;
                    }
                }
                btnCollapse.Enabled = !_collapsed;
                btnExpand.Enabled = _collapsed;
            }
        }

        private int restoreHeight;

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            this.Collapsed = true;
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            this.Collapsed = false;
        }
    }
}
