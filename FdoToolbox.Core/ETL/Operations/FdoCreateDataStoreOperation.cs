#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.Collections.Specialized;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.DataStore;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Represents an operation that executes an FDO create data store command
    /// </summary>
    public class FdoCreateDataStoreOperation : FdoOperationBase
    {
        private string _provider;
        private string _pendingConnStr;
        private NameValueCollection _properties;

        public FdoCreateDataStoreOperation(string provider, NameValueCollection properties, string pendingConnectionString)
        {
            _provider = provider;
            _pendingConnStr = pendingConnectionString;
            _properties = properties;
        }

        private int counter = 0;
        
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            if (counter < 1) //Shouldn't be reentrant, but just play it safe.
            {
                Info("Creating a connection to: " + _provider);
                using (var conn = FeatureAccessManager.GetConnectionManager().CreateConnection(_provider))
                {
                    if (!string.IsNullOrEmpty(_pendingConnStr))
                    {
                        conn.ConnectionString = _pendingConnStr;
                        conn.Open();
                    }

                    using (var create = (ICreateDataStore)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore))
                    {
                        var props = create.DataStoreProperties;
                        foreach (string key in _properties.Keys)
                        {
                            string name = key;
                            string value = _properties[key];
                            Info("Setting property: " + name + " = " + value);
                            props.SetProperty(name, value);
                        }
                        create.Execute();
                        Info("Data Store created");
                    }

                    if (conn.ConnectionState != OSGeo.FDO.Connections.ConnectionState.ConnectionState_Closed)
                    {
                        conn.Close();
                    }
                }
                counter++;
            }
            return rows;
        }
    }
}
