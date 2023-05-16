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
using FdoToolbox.Core.ETL;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;
using FdoToolbox.Core.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace FdoToolbox.Tasks.Services
{
    public class TaskLoader : BaseDefinitionLoader
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The connection string.</param>
        /// <param name="configPath">The configuration path</param>
        /// <param name="name">The name that will be assigned to the connection.</param>
        /// <returns></returns>
        protected override FdoConnection CreateConnection(string provider, string connStr, string configPath, ref string name)
        {
            IFdoConnectionManager connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
            //Try to find by name first
            FdoConnection conn = null;
            conn = connMgr.GetConnection(name);
            //Named connection matches all the details
            if (conn != null)
            {
                if (conn.Provider == provider && conn.ConnectionString == connStr)
                    return conn;
            }

            //Then to find matching open connection
            foreach (string connName in connMgr.GetConnectionNames())
            {
                FdoConnection c = connMgr.GetConnection(connName);
                if (c.Provider == provider && c.ConnectionString == connStr)
                {
                    name = connName;
                    return c;
                }
            }

            //Make a new connection
            LoggingService.Info(ResourceService.GetString("INFO_REFERENCED_CONNECTION_NOT_FOUND"));
            conn = new FdoConnection(provider, connStr);
            if (!string.IsNullOrEmpty(configPath) && System.IO.File.Exists(configPath))
                conn.SetConfiguration(configPath);
            connMgr.AddConnection(name, conn);
            return conn;
        }

        /// <summary>
        /// Prepares the specified bulk copy definition (freshly deserialized) before the loading process begins
        /// </summary>
        /// <param name="def">The bulk copy definition.</param>
        protected override Dictionary<string, string> Prepare(FdoToolbox.Core.Configuration.FdoBulkCopyTaskDefinition def)
        {
            /* There is subtle precondition that would've resulted in all connection references being named to a
             * single reference, thus invalidating the whole task when loaded.
             * 
             * If the task definition has any connection names to an *already* loaded connection, a rename operation
             * could overwrite a previous rename operation. Consider:
             * 
             * Connection A) SDF_Desktop
             * Connection B) SDFConnection0
             * 
             * Loaded Connections:
             * - SDFConnection0
             * - SDFConnection1
             * 
             * If during loading, SDF_Desktop matches to SDFConnection0, and SDFConnection0 matches to SDFConnection1 the rename operations
             * would then be:
             * 
             * 1) Rename SDF_Desktop to SDFConnection0
             * 2) Rename SDF_Connection0 to SDFConnection1
             * 
             * As a result, all referenced connections will eventually be renamed to SDFConnection1, which is not what we want.
             * 
             * The solution here is to "fix" the definition by renaming the named connections to something we know is not already a loaded
             * connection. This is done regardless to ensure consistent behaviour. This method performs this solution.
             */

            string prefix = "Connection";
            int counter = 0;
            FdoConnectionManager connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();

            var nameMappings = new Dictionary<string, string>();
            var connectionsToAdd = new HashSet<string>();

            foreach (FdoConnectionEntryElement el in def.Connections)
            {
                string oldName = el.name;
                // If no connection exists or to be added under this current name, we're
                // free to use it for ourselves
                if (connMgr.GetConnection(oldName) == null && !connectionsToAdd.Contains(oldName))
                {
                    nameMappings.Add(oldName, oldName);
                    connectionsToAdd.Add(oldName);
                }
                else
                {
                    string newName = prefix + counter;
                    while (connMgr.GetConnection(newName) != null)
                    {
                        counter++;
                        newName = prefix + counter;
                    }
                    def.UpdateConnectionReferences(oldName, newName);
                    nameMappings.Add(newName, oldName);
                    connectionsToAdd.Add(newName);
                    counter++;
                }
            }

            return nameMappings;
        }
    }
}
