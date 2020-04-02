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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Expression;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal partial class RangeConstraintSettingsCtl : UserControl
    {
        public RangeConstraintSettingsCtl()
        {
            InitializeComponent();
        }

        public DataValue MinValue
        {
            get { return (DataValue)DataValue.Parse(txtMinValue.Text); }
            set { txtMinValue.Text = value.ToString(); }
        }

        public DataValue MaxValue
        {
            get { return (DataValue)DataValue.Parse(txtMaxValue.Text); }
            set { txtMaxValue.Text = value.ToString(); }
        }

        public bool MinInclusive
        {
            get { return chkMinInclusive.Checked; }
            set { chkMinInclusive.Checked = value; }
        }

        public bool MaxInclusive
        {
            get { return chkMaxInclusive.Checked; }
            set { chkMaxInclusive.Checked = value; }
        }
    }
}
