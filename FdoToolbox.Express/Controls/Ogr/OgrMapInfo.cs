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
using System.Text;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace FdoToolbox.Express.Controls.Ogr
{
    public class OgrMapInfo : BaseOgrConnectionBuilder
    {
        public OgrMapInfo() { base.ReadOnly = true; }

        [Description("The path to the directory containing the MapInfo files. This directory must only contain MapInfo files!")]
        [DisplayName("MapInfo directory")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public override string DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        public override string ToConnectionString()
        {
            if (this.DataSource.EndsWith("\\"))
                return $"DataSource={this.DataSource.Substring(0, this.DataSource.Length - 1)};DefaultSchemaName={this.DefaultSchema};DataSourceEncoding={this.DataSourceEncoding};ReadOnly={this.ReadOnly.ToString().ToUpper()}";
            else
                return $"DataSource={this.DataSource};DefaultSchemaName={this.DefaultSchema};DataSourceEncoding={this.DataSourceEncoding};ReadOnly={this.ReadOnly.ToString().ToUpper()}";
        }
    }
}
