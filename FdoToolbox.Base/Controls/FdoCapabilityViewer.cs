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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A control that can display the capabilities of a given connection
    /// </summary>
    [ToolboxItem(false)]
    public partial class FdoCapabilityViewer : ViewContent, IFdoCapabilityView
    {
        private FdoCapabilityViewerPresenter _presenter;

        internal FdoCapabilityViewer()
        {
            InitializeComponent();
            grdCaps.AutoGenerateColumns = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoCapabilityViewer"/> class.
        /// </summary>
        /// <param name="connName">Name of the conn.</param>
        public FdoCapabilityViewer(string connName)
            : this()
        {
            _presenter = new FdoCapabilityViewerPresenter(this, connName);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if (_presenter != null)
                _presenter.Init();
            base.OnLoad(e);
        }

        /// <summary>
        /// Sets the capabilities.
        /// </summary>
        /// <value>The capabilities.</value>
        public CapabilityEntry[] Capabilities
        {
            set { grdCaps.DataSource = value; }
        }
    }
}
