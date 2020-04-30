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
using FdoToolbox.Base.Controls;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// Manages connection dependent view content
    /// </summary>
    public class TabManager : IService
    {
        private IFdoConnectionManager connMgr;
        private List<IConnectionDependentView> _tabs;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Initializes the service.
        /// </summary>
        public void InitializeService()
        {
            _tabs = new List<IConnectionDependentView>();
            //Can't get instance here otherwise it will cause a recurive loop resulting in a stack overflow.
            //So do it after the ServiceManager has been initialized
            ServiceManager.ServiceManagerInitialized += delegate
            {
                connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
                connMgr.BeforeConnectionRemove += new ConnectionBeforeRemoveHandler(OnBeforeRemoveConnection);
            };
            IsInitialized = true;
            Initialize(this, EventArgs.Empty);
        }

        void OnBeforeRemoveConnection(object sender, ConnectionBeforeRemoveEventArgs e)
        {
            FdoConnection conn = connMgr.GetConnection(e.ConnectionName);
            //Get all views that depend on connection
            List<IConnectionDependentView> matches = _tabs.FindAll(delegate(IConnectionDependentView c) { return c.DependsOnConnection(conn); });
            if (matches.Count > 0)
            {
                //Don't close then cancel
                if (!MessageService.AskQuestion(ResourceService.GetStringFormatted("QUESTION_CLOSE_TABS", e.ConnectionName)))
                {
                    e.Cancel = true;
                    return;
                }
                else //Otherwise remove from watch list and close the view
                {
                    foreach (IConnectionDependentView view in matches)
                    {
                        _tabs.Remove(view);
                        view.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Unloads the service.
        /// </summary>
        public void UnloadService()
        {
            _tabs.Clear();
            Unload(this, EventArgs.Empty);
        }

        /// <summary>
        /// Loads any persisted objects from the session directory
        /// </summary>
        public void Load()
        {
            
        }

        /// <summary>
        /// Persists any managed objects to the session directory
        /// </summary>
        public void Save()
        {
            
        }

        /// <summary>
        /// Registers the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void Register(IConnectionDependentView view)
        {
            _tabs.Add(view);
            //If view was not closed programmatically, ViewContentClosing won't fire.
            //So hook onto the Disposed event to remove the view from the watch list.
            view.Disposed += delegate
            {
                _tabs.Remove(view);
            };
        }

        /// <summary>
        /// Occurs when the service is initialized
        /// </summary>
        public event EventHandler Initialize = delegate { };

        /// <summary>
        /// Occurs when the service is unloaded
        /// </summary>
        public event EventHandler Unload = delegate { };
    }
}
