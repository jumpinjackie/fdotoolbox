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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Configuration;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Defines the options for a <see cref="FdoBulkCopy"/> instance
    /// </summary>
    public class FdoBulkCopyOptions : IDisposable
    {
        private Dictionary<string, FdoConnection> _connections;

        private List<FdoClassCopyOptions> _copyOptions;

        private bool _owner;

        private List<string> _ownerOfConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkCopyOptions"/> class.
        /// </summary>
        public FdoBulkCopyOptions()
        {
            _copyOptions = new List<FdoClassCopyOptions>();
            _connections = new Dictionary<string, FdoConnection>();
            _owner = false;
            _ownerOfConnection = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkCopyOptions"/> class.
        /// Used by ExpressUtility
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <param name="owner">if set to <c>true</c>, this object owns the connections within and will dispose and cleanup these connections when done.</param>
        internal FdoBulkCopyOptions(Dictionary<string, FdoConnection> connections, bool owner)
        {
            _copyOptions = new List<FdoClassCopyOptions>();
            _connections = connections;
            _owner = owner;
            _ownerOfConnection = new List<string>();
        }

        /// <summary>
        /// Sets the specified named connection as the owner. This connection will be disposed and cleaned up when done
        /// </summary>
        /// <param name="name"></param>
        internal void MarkOwnerOfConnection(string name)
        {
            _ownerOfConnection.Add(name);
        }

        /// <summary>
        /// Gets the connection names.
        /// </summary>
        /// <value>The connection names.</value>
        public ICollection<string> ConnectionNames => _connections.Keys;

        /// <summary>
        /// Registers the connection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="conn">The conn.</param>
        public void RegisterConnection(string name, FdoConnection conn)
        {
            if (_connections.ContainsKey(name))
                throw new ArgumentException("A connection named " + name + " already exists");

            _connections[name] = conn;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FdoConnection GetConnection(string name)
        {
            if (!_connections.ContainsKey(name))
                throw new ArgumentException("A connection named " + name + " was not found");

            return _connections[name];
        }

        /// <summary>
        /// Adds the class copy option.
        /// </summary>
        /// <param name="copt">The copt.</param>
        public void AddClassCopyOption(FdoClassCopyOptions copt)
        {
            copt.Parent = this;
            _copyOptions.Add(copt);
        }

        /// <summary>
        /// Gets the class copy options.
        /// </summary>
        /// <value>The class copy options.</value>
        public ICollection<FdoClassCopyOptions> ClassCopyOptions => _copyOptions;

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            foreach (FdoClassCopyOptions copt in this.ClassCopyOptions)
            {
                FdoConnection src = GetConnection(copt.SourceConnectionName);
                FdoConnection dst = GetConnection(copt.TargetConnectionName);

                if (src == null)
                    throw new TaskValidationException("The specified source connection name does not exist");

                if (dst == null)
                    throw new TaskValidationException("The specified target connection name does not exist");

                using (FdoFeatureService srcSvc = src.CreateFeatureService())
                using (FdoFeatureService dstSvc = dst.CreateFeatureService())
                {
                    ClassDefinition srcCls = srcSvc.GetClassByName(copt.SourceSchema, copt.SourceClassName);
                    ClassDefinition dstCls = dstSvc.GetClassByName(copt.TargetSchema, copt.TargetClassName);

                    if (srcCls == null)
                        throw new TaskValidationException("The specified source feature class does not exist");

                    if (dstCls == null)
                        throw new TaskValidationException("The specified target feature class does not exist");
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_owner)
            {
                foreach (FdoConnection conn in _connections.Values)
                {
                    if (conn.State != FdoConnectionState.Closed)
                        conn.Close();
                    conn.Dispose();
                }
                _connections.Clear();
            }
            else
            {
                foreach (string name in _ownerOfConnection)
                {
                    var conn = _connections[name];
                    if (conn.State != FdoConnectionState.Closed)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        //internal void AddClassModifier(TargetClassModificationItem moditem)
        //{
        //    var copt = FindByTargetClassName(moditem.Name);
        //    if (copt != null)
        //    {
        //        copt.PreCopyTargetModifier = moditem;
        //    }
        //}

        //private FdoClassCopyOptions FindByTargetClassName(string name)
        //{
        //    foreach (var copt in _copyOptions)
        //    {
        //        if (copt.TargetClassName == name)
        //            return copt;
        //    }

        //    return null;
        //}

        internal void UpdateConnectionReferences(string oldName, string newName)
        {
            if (_connections.ContainsKey(oldName))
            {
                var conn = _connections[oldName];
                _connections.Remove(oldName);
                _connections[newName] = conn;
            }

            foreach (var copt in _copyOptions)
            {
                if (copt.SourceConnectionName.Equals(oldName))
                    copt.SourceConnectionName = newName;

                if (copt.TargetConnectionName.Equals(oldName))
                    copt.TargetConnectionName = newName;
            }
        }
    }
}
