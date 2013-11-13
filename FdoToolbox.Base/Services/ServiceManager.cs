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
using ICSharpCode.Core;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// Initializes services defined in all addins and handles requests for services.
    /// </summary>
    public class ServiceManager
    {
        private static ServiceManager instance = null;

        private List<IService> _services = new List<IService>();
        private Dictionary<Type, IService> _serviceDict = new Dictionary<Type, IService>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ServiceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceManager();
                    ServiceManagerInitialized(instance, EventArgs.Empty);
                }
                return instance;
            }
        }

        /// <summary>
        /// Occurs when [service manager initialized].
        /// </summary>
        public static event EventHandler ServiceManagerInitialized = delegate { };

        private ServiceManager() { InitializeServicesSubsystem("/FdoToolbox/Services"); }

        internal void InitializeServicesSubsystem(string servicePath)
        {
            List<IService> services = AddInTree.BuildItems<IService>(servicePath, null, false);
            if (services != null && services.Count > 0)
                AddServices(services);

            foreach (IService service in _services)
            {
                if (!service.IsInitialized)
                {
                    service.InitializeService();
                }
            }
        }

        internal void RestoreSession()
        {
            foreach (IService service in _services)
            {
                service.Load();
            }
        }

        internal void UnloadAllServices()
        {
            foreach (IService service in _services)
            {
                service.Save();
                service.UnloadService();
            }
        }

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <param name="service">The service.</param>
        protected void AddService(IService service)
        {
            _services.Add(service);
        }

        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">The services.</param>
        protected void AddServices(IEnumerable<IService> services)
        {
            _services.AddRange(services);
        }

        /// <summary>
        /// Requests a specific service. May return null if service is not found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : class, IService
        {
            Type serviceType = typeof(T);
            if (_serviceDict.ContainsKey(serviceType))
                return _serviceDict[serviceType] as T;

            foreach (IService service in _services)
            {
                if (serviceType.IsInstanceOfType(service))
                {
                    _serviceDict[serviceType] = service;
                    return service as T;
                }
            }
            return null;
        }
    }
}
