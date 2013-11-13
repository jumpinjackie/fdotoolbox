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
    public class OdbcSqlServer : IOdbcConnectionBuilder
    {
        private string _Server;

        [DefaultValue("(local)")]
        [Description("The named instance of the SQL server")]
        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }

        private string _Database;

        [Description("The name of the SQL server database to connect to")]
        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
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

        private bool _TrustedConnection;

        [DefaultValue(false)]
        [Description("Indicates if this is a trusted connection. The user id and password properties are ignored if this is true")]
        public bool TrustedConnection
        {
            get { return _TrustedConnection; }
            set { _TrustedConnection = value; }
        }

        public string ToConnectionString()
        {
            if (this.TrustedConnection)
                return string.Format("Driver={{SQL Server}};Server={0};Database={1};Trusted_Connection=Yes", this.Server, this.Database);
            else
                return string.Format("Driver={{SQL Server}};Server={0};Database={1};Uid={2};Pwd={3}", this.Server, this.Database, this.UserId, this.Password);
        }
    }
}
