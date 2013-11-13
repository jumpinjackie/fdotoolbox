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
using OSGeo.FDO.Connections;
using FdoToolbox.Core;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoUtil
{
    public class DestroyCommand : ConsoleCommand
    {
        private string _dstoreStr;
        private string _connStr;
        private string _provider;

        public DestroyCommand(string provider, string connStr, string dstoreStr)
        {
            _connStr = connStr;
            _provider = provider;
            _dstoreStr = dstoreStr;
        }

        private IConnection CreateConnection()
        {
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(_provider);
            return conn;
        }

        public override int Execute()
        {
            CommandStatus retCode;
            IConnection conn = null;
            try
            {
                conn = CreateConnection();
                if (!string.IsNullOrEmpty(_connStr))
                {
                    conn.ConnectionString = _connStr;
                    conn.Open();
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_CONNECT;
                return (int)retCode;
            }

            using (conn)
            {
                using (FdoFeatureService service = new FdoFeatureService(conn))
                {
                    try
                    {
                        service.DestroyDataStore(_dstoreStr);
                        WriteLine("Data Store destroyed!");
                        retCode = CommandStatus.E_OK;
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        WriteException(ex);
                        retCode = CommandStatus.E_FAIL_DESTROY_DATASTORE;
                        return (int)retCode;
                    }
                }
                if(conn.ConnectionState != ConnectionState.ConnectionState_Closed)
                    conn.Close();
            }
            return (int)retCode;
        }
    }
}
