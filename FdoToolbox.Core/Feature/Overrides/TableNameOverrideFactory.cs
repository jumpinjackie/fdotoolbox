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
using System.Collections.Generic;
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Core.Feature.Overrides
{
    /// <summary>
    /// A factory class that provides <see cref="ITableNameOverride"/> implementations
    /// </summary>
    public sealed class TableNameOverrideFactory 
    {
        static Dictionary<string, ITableNameOverride> _overrides;

        static TableNameOverrideFactory()
        {
            _overrides = new Dictionary<string, ITableNameOverride>();
            ITableNameOverride oracle = new OracleTableNameOverride();
            _overrides["King.Oracle".ToUpper()] = oracle;
            _overrides["OSGeo.KingOracle".ToUpper()] = oracle;
        }

        /// <summary>
        /// Gets the table name override.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static ITableNameOverride GetTableNameOverride(string provider)
        {
            if (string.IsNullOrEmpty(provider))
                return null;

            string name = provider;

            try
            {
                ProviderNameTokens tok = new ProviderNameTokens(provider);
                string[] tokens = tok.GetNameTokens();
                name = tokens[0] + "." + tokens[1];
            }
            catch { }

            name = name.ToUpper();

            if (_overrides.ContainsKey(name))
                return _overrides[name];

            return null;
        }
    }
}
