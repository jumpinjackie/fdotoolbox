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
using System.ComponentModel;

namespace FdoToolbox.Express.Controls.Odbc
{
    public class OdbcAccess : IOdbcConnectionBuilder
    {
        private string _File;

        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("The path to the Microsoft Access Database")]
        [DisplayName("MDB File Path")]
        public string File
        {
            get { return _File; }
            set { _File = value; }
        }

        private string _UserId;

        [Description("The user id to connect as")]
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        private string _Password;

        [Description("The password for the user id")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public string ToConnectionString()
        {
#if X64
            string connStr = string.Format("Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={0}", this.File);
#else
            string connStr = string.Format("Driver={{Microsoft Access Driver (*.mdb)}};Dbq={0}", this.File);
#endif
            if (!string.IsNullOrEmpty(this.UserId))
                connStr += "Uid=" + this.UserId;
            if (!string.IsNullOrEmpty(this.Password))
                connStr += "Pwd=" + this.Password;

            return connStr;
        }
    }
}
