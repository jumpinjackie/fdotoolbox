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

namespace FdoToolbox.Express.Controls.Ogr
{
    public abstract class BaseOgrConnectionBuilder : IOgrConnectionBuilder
    {
        private string _DataSource;

        public virtual string DataSource
        {
            get { return _DataSource; }
            set { _DataSource = value; }
        }
	
        private bool _ReadOnly;

        [Description("Open connection as read-only")]
        [DisplayName("Read Only")]
        [DefaultValue(true)]
        public virtual bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }

        public virtual string ToConnectionString()
        {
            return string.Format("DataSource={0};ReadOnly={1}", this.DataSource, this.ReadOnly.ToString().ToUpper());
        }
    }
}
