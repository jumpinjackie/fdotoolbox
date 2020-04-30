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
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing.Design;

namespace FdoToolbox.Express.Controls.Ogr
{
    public class OgrS57Editor : FileNameEditor
    {
        //TODO: Find out what the extensions are to look for
    }

    public class OgrS57 : BaseOgrConnectionBuilder
    {
        public OgrS57() { base.ReadOnly = true; }

        [Description("The path to the S-57 files")]
        [DisplayName("S-57 file path")]
        [Editor(typeof(OgrS57Editor), typeof(UITypeEditor))]
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
