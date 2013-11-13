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
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL.Overrides
{
    /// <summary>
    /// Factory class to create <see cref="ICopySpatialContext"/> commands
    /// </summary>
    public static class CopySpatialContextOverrideFactory
    {
        private static Dictionary<string, Type> _CopySpatialContextOverrides = new Dictionary<string, Type>();

        /// <summary>
        /// Registers an override class to copy spatial contexts
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="overrideType"></param>
        public static void RegisterCopySpatialContextOverride(string providerName, Type overrideType)
        {
            if (!Array.Exists<Type>(overrideType.GetInterfaces(), delegate(Type t) { return t == typeof(ICopySpatialContext); }))
                throw new ArgumentException("The given type does not implement ICopySpatialContextOverride");

            _CopySpatialContextOverrides[providerName] = overrideType;
        }

        /// <summary>
        /// Gets the registered override object
        /// </summary>
        /// <param name="targetConn"></param>
        /// <returns></returns>
        public static ICopySpatialContext GetCopySpatialContextOverride(FdoConnection targetConn)
        {
            string providerName = targetConn.Provider;
            if (_CopySpatialContextOverrides.ContainsKey(providerName))
            {
                return (ICopySpatialContext)Activator.CreateInstance(_CopySpatialContextOverrides[providerName]);
            }
            return new CopySpatialContext();
        }

        /// <summary>
        /// Initialize with the default overrides
        /// </summary>
        static CopySpatialContextOverrideFactory()
        {
            RegisterCopySpatialContextOverride("OSGeo.MySQL", typeof(MySqlCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SHP", typeof(ShpCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SQLServerSpatial", typeof(MsSqlCopySpatialContextOverride));
        }
    }
}
