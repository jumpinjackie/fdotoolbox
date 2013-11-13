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
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoUtil
{
    public class CreateDataStoreCommand : ConsoleCommand
    {
        private string _dstoreStr;
        private string _connStr;
        private string _provider;

        public CreateDataStoreCommand(string provider, string connStr, string dstoreStr)
        {
            _connStr = connStr;
            _provider = provider;
            _dstoreStr = dstoreStr;
        }

        public override int Execute()
        {
            CommandStatus retCode;
            FdoConnection conn = null;
            try
            {
                conn = new FdoConnection(_provider, _connStr);
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_CONNECT;
                return (int)retCode;
            }

            using (conn)
            {
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    try
                    {
                        service.CreateDataStore(_dstoreStr);
                        WriteLine("Data Store Created!");
                        retCode = CommandStatus.E_OK;
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        WriteException(ex);
                        retCode = CommandStatus.E_FAIL_CREATE_DATASTORE;
                        return (int)retCode;
                    }
                }
                if (conn.State != FdoConnectionState.Closed)
                    conn.Close();
            }
            return (int)retCode;
        }
    }
}
