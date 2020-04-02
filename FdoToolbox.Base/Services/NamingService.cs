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
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// A service that provides name generation for FDO connections
    /// </summary>
    public class NamingService : IService
    {
        private bool _init = false;

        private Dictionary<string, string> _namePrefixes;
        private Dictionary<string, int> _counter;

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
            _namePrefixes = new Dictionary<string, string>();
            _counter = new Dictionary<string, int>();
            
            _init = true;
            Initialize(this, EventArgs.Empty);
        }

        /// <summary>
        /// Unloads the service.
        /// </summary>
        public void UnloadService()
        {
            Unload(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the preferred name prefix for a given provider
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="prefix">The prefix.</param>
        public void SetPreferredNamePrefix(string provider, string prefix)
        {
            _namePrefixes[provider] = prefix;
        }

        /// <summary>
        /// Resets the connection counter.
        /// </summary>
        public void ResetCounter()
        {
            List<string> keys = new List<string>(_counter.Keys);
            foreach (string k in keys)
            {
                _counter[k] = 0;
            }
        }

        private FdoConnectionManager _manager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public string GetDefaultConnectionName(string provider)
        {
            return GetDefaultConnectionName(provider, string.Empty);
        }

        /// <summary>
        /// Gets the default name of the connection based on its provider
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetDefaultConnectionName(string provider, string file)
        {
            if (!_namePrefixes.ContainsKey(provider))
            {
                using (var pnt = new ProviderNameTokens(provider))
                {
                    SetPreferredNamePrefix(provider, pnt.GetLocalName() + "_");
                }
            }

            if (!_counter.ContainsKey(provider))
                _counter[provider] = 0;

            if (_manager == null)
                _manager = ServiceManager.Instance.GetService<FdoConnectionManager>();

            string name = "";
            if (!string.IsNullOrEmpty(file)) //Try [PROVIDER]_[FileName] first before adding numbers
                name = _namePrefixes[provider] + file;
            else 
                name = _namePrefixes[provider] + file + _counter[provider];
            while (_manager.NameExists(name))
            {
                _counter[provider]++;
                name = _namePrefixes[provider] + file + _counter[provider];
            }
            return name;
        }

        /// <summary>
        /// Occurs when the service is initialized
        /// </summary>
        public event EventHandler Initialize = delegate { };

        /// <summary>
        /// Occurs when the service is unloaded
        /// </summary>
        public event EventHandler Unload = delegate { };


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
    }
}
