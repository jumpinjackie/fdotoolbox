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
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing.Design;

namespace FdoToolbox.Express.Controls.Ogr
{
    public class OgrShapeFileEditor : FileNameEditor
    {
        protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "ESRI Shape File (*.shp)|*.shp";
        }
    }

    public class OgrShapeFile : BaseOgrConnectionBuilder
    {
        public OgrShapeFile() { base.ReadOnly = true; }

        [Description("The path to the ESRI Shape File")]
        [DisplayName("Shape File Path")]
        [Editor(typeof(OgrShapeFileEditor), typeof(UITypeEditor))]
        public override string DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        [ReadOnly(true)]
        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                base.ReadOnly = value;
            }
        }
    }
}
