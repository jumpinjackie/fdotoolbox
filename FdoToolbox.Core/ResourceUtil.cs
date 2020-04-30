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
using System.Resources;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Utility class to access string resources
    /// </summary>
    public sealed class ResourceUtil
    {
        private static ResourceManager _resMan;

        static ResourceUtil()
        {
            _resMan = Strings.ResourceManager;
        }

        /// <summary>
        /// Gets the string resource.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal static string GetString(string key)
        {
            return _resMan.GetString(key);
        }

        /// <summary>
        /// Gets the string resource formatted.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        internal static string GetStringFormatted(string key, params object[] args)
        {
            string str = _resMan.GetString(key);
            return string.Format(str, args);
        }

        /// <summary>
        /// Gets the resource manager for this string resource bundle
        /// </summary>
        public static ResourceManager StringResourceManager => Strings.ResourceManager;
    }
}
