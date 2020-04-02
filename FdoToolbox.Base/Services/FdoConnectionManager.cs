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
using FdoToolbox.Core.Feature;
using FdoToolbox.Core;
using ICSharpCode.Core;

//TODO: Move exception messages to Errors.resx

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// Manages FDO connections.
    /// </summary>
    public sealed class FdoConnectionManager : IFdoConnectionManager
    {
        private Dictionary<string, FdoConnection> _ConnectionDict = new Dictionary<string, FdoConnection>();

        /// <summary>
        /// Removes all connections
        /// </summary>
        public void Clear()
        {
            List<string> names = new List<string>(GetConnectionNames());
            foreach (string name in names)
            {
                //Yes we're inlining RemoveConnection(), but we want any cancel action to cancel
                //the Clear() method
                if (_ConnectionDict.ContainsKey(name))
                {
                    ConnectionBeforeRemoveEventArgs e = new ConnectionBeforeRemoveEventArgs(name);
                    this.BeforeConnectionRemove(this, e);
                    if (e.Cancel)
                        return;

                    FdoConnection conn = _ConnectionDict[name];
                    conn.Close();
                    _ConnectionDict.Remove(name);
                    conn.Dispose();
                    if (this.ConnectionRemoved != null)
                        this.ConnectionRemoved(this, new EventArgs<string>(name));
                }
            }
        }

        /// <summary>
        /// Determines whether a connection exists (by name)
        /// </summary>
        /// <param name="name">The name of the connection</param>
        /// <returns></returns>
        public bool NameExists(string name)
        {
            if (name == null)
                return false;
            return _ConnectionDict.ContainsKey(name);
        }

        /// <summary>
        /// Adds a FDO connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="conn">The connection.</param>
        public void AddConnection(string name, FdoToolbox.Core.Feature.FdoConnection conn)
        {
            if (_ConnectionDict.ContainsKey(name))
                throw new FdoConnectionException("Unable to add connection named " + name + " to the connection manager");

            if (conn.State != FdoConnectionState.Open)
                conn.Open();

            _ConnectionDict.Add(name, conn);
            this.ConnectionAdded(this, new EventArgs<string>(name));
        }

        /// <summary>
        /// Removes the connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        public void RemoveConnection(string name)
        {
            if (_ConnectionDict.ContainsKey(name))
            {
                ConnectionBeforeRemoveEventArgs e = new ConnectionBeforeRemoveEventArgs(name);
                this.BeforeConnectionRemove(this, e);
                if (e.Cancel)
                    return;

                FdoConnection conn = _ConnectionDict[name];
                conn.Close();
                _ConnectionDict.Remove(name);
                conn.Dispose();
                if (this.ConnectionRemoved != null)
                    this.ConnectionRemoved(this, new EventArgs<string>(name));
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <returns></returns>
        public FdoToolbox.Core.Feature.FdoConnection GetConnection(string name)
        {
            FdoConnection conn = null;
            if (_ConnectionDict.ContainsKey(name))
                conn = _ConnectionDict[name];
            return conn;
        }

        /// <summary>
        /// Gets the connection names.
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetConnectionNames()
        {
            return _ConnectionDict.Keys;
        }

        /// <summary>
        /// Renames a connection.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        public void RenameConnection(string oldName, string newName)
        {
            if (!_ConnectionDict.ContainsKey(oldName))
                throw new FdoConnectionException("The connection to be renamed could not be found: " + oldName);
            if (_ConnectionDict.ContainsKey(newName))
                throw new FdoConnectionException("Cannot rename connection " + oldName + " to " + newName + " as a connection of that name already exists");

            FdoConnection conn = _ConnectionDict[oldName];
            _ConnectionDict.Remove(oldName);
            _ConnectionDict.Add(newName, conn);

            ConnectionRenameEventArgs e = new ConnectionRenameEventArgs(oldName, newName);
            this.ConnectionRenamed(this, e);
        }

        /// <summary>
        /// Determines whether this this connection can be renamed to the specified name
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <returns></returns>
        public ConnectionRenameResult CanRenameConnection(string oldName, string newName)
        {
            string reason = string.Empty;
            bool result = false;
            if (!_ConnectionDict.ContainsKey(oldName))
            {
                reason = "The connection to be renamed could not be found: " + oldName;
                result = false;
                return new ConnectionRenameResult(result, reason);
            }
            if (_ConnectionDict.ContainsKey(newName))
            {
                reason = "Cannot rename connection " + oldName + " to " + newName + " as a connection of that name already exists";
                result = false;
                return new ConnectionRenameResult(result, reason);
            }
            return new ConnectionRenameResult();
        }

        /// <summary>
        /// Occurs before a connection is removed. Subscribers have the opportunity to 
        /// cancel the removal operation by setting the Cancel property of the 
        /// <see cref="ConnectionBeforeRemoveEventArgs"/> object to true
        /// </summary>
        public event ConnectionBeforeRemoveHandler BeforeConnectionRemove = delegate { };

        /// <summary>
        /// Occurs when a connection is added
        /// </summary>
        public event ConnectionEventHandler ConnectionAdded = delegate { };

        /// <summary>
        /// Occurs when a connection is removed
        /// </summary>
        public event ConnectionEventHandler ConnectionRemoved = delegate { };

        /// <summary>
        /// Occurs when a connection is renamed
        /// </summary>
        public event ConnectionRenamedEventHandler ConnectionRenamed = delegate { };

        /// <summary>
        /// Occurs when a connection is refreshed
        /// </summary>
        public event ConnectionEventHandler ConnectionRefreshed = delegate { };

        private bool _init = false;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return _init; }
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        public void InitializeService()
        {
            LoggingService.Info("Initialized Connection Manager Service");
            _init = true;
            Initialize(this, EventArgs.Empty);
        }

        /// <summary>
        /// Unloads the service.
        /// </summary>
        public void UnloadService()
        {
            var conns = new List<FdoConnection>(_ConnectionDict.Values);
            foreach (var c in conns)
            {
                c.Close();
                c.Dispose();
            }
            _ConnectionDict.Clear();
            Unload(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when this instnace is initialized
        /// </summary>
        public event EventHandler Initialize = delegate { };

        /// <summary>
        /// Occurs when this service is unloded
        /// </summary>
        public event EventHandler Unload = delegate { };

        /// <summary>
        /// Refreshes the connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        public void RefreshConnection(string name)
        {
            if (NameExists(name))
            {
                FdoConnection conn = this.GetConnection(name);
                conn.Close();

                //HACK: FDO #660 workaround
                if (conn.Provider.ToUpper().StartsWith("OSGEO.POSTGRESQL"))
                    conn.ConnectionString = conn.ConnectionString;

                conn.Open();

                /*
                //TODO: I got a bad feeling that this may break something
                //which may rely on the old connection. Verify this.
                FdoConnection oldConn = this.GetConnection(name);
                string provider = oldConn.Provider;
                string connStr = oldConn.ConnectionString;
                oldConn.Close();
                oldConn.Dispose();

                FdoConnection newConn = new FdoConnection(provider, connStr);
                newConn.Open();
                _ConnectionDict[name] = newConn;
                */
                ConnectionRefreshed(this, new EventArgs<string>(name));
            }
        }

        /// <summary>
        /// Load all persisted connections from the session directory
        /// </summary>
        public void Load()
        {
            string path = Preferences.SessionDirectory;
            if (System.IO.Directory.Exists(path))
            {
                try
                {
                    string[] files = System.IO.Directory.GetFiles(path, "*.conn", System.IO.SearchOption.AllDirectories);
                    SortedDictionary<string, FdoConnection> connections = new SortedDictionary<string, FdoConnection>();
                    foreach (string f in files)
                    {
                        try
                        {
                            string dir = System.IO.Path.GetDirectoryName(f);
                            string name = string.Empty;
                            if (dir != path)
                            {
                                System.Diagnostics.Debug.Assert(dir.Length > path.Length && dir.Contains(path)); //Directory is child of session dir

                                string relDir = dir.Substring(path.Length + 1); //To remove the trailing slash
                                if (!relDir.EndsWith("\\"))
                                    relDir += "\\";

                                name = relDir + System.IO.Path.GetFileNameWithoutExtension(f);
                            }
                            else
                            {
                                name = System.IO.Path.GetFileNameWithoutExtension(f);
                            }
                            FdoConnection conn = FdoConnection.LoadFromFile(f, true);
                            connections.Add(name, conn);
                        }
                        catch(Exception ex)
                        {
                            LoggingService.Warn("Could not create connection from " + f + ": " + ex.Message);
                        }
                    }

                    foreach (string name in connections.Keys)
                    {
                        try
                        {
                            this.AddConnection(name, connections[name]);
                        }
                        catch (Exception ex)
                        {
                            LoggingService.Warn("Could not load connection " + name + ": " + ex.Message);
                        }
                    }
                }
                finally
                {
                    //After loading all connections, remove any directories in the session directory
                    string[] directories = System.IO.Directory.GetDirectories(path);
                    foreach (string dir in directories)
                    {
                        System.IO.Directory.Delete(dir, true);
                    }
                }
            }
        }

        /// <summary>
        /// Saves all currently loaded connections to the session directory
        /// </summary>
        public void Save()
        {
            string path = Preferences.SessionDirectory;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            else
            {
                string [] files = System.IO.Directory.GetFiles(path, "*.conn");
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }
                files = System.IO.Directory.GetFiles(path, "*.xml");
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }

                string[] directories = System.IO.Directory.GetDirectories(path);
                foreach (string dir in directories)
                {
                    System.IO.Directory.Delete(dir, true);
                }
            }

            foreach (string key in _ConnectionDict.Keys)
            {
                string file = System.IO.Path.Combine(path, key + ".conn");
                string dir = System.IO.Path.GetDirectoryName(file);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                FdoConnection conn = _ConnectionDict[key];
                conn.Save(file);
            }
        }


        /// <summary>
        /// Gets the connection by provider and connection string
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The connection string.</param>
        /// <returns></returns>
        public FdoConnection GetConnection(string provider, string connStr)
        {
            foreach (FdoConnection conn in _ConnectionDict.Values)
            {
                if (conn.Provider.StartsWith(provider) && conn.ConnectionString.ToLower() == connStr.ToLower())
                    return conn;
            }
            return null;
        }

        /// <summary>
        /// Gets the name of a connection
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns></returns>
        public string GetName(FdoConnection conn)
        {
            foreach (string key in _ConnectionDict.Keys)
            {
                if (_ConnectionDict[key] == conn)
                    return key;
            }
            return null;
        }
    }
}
