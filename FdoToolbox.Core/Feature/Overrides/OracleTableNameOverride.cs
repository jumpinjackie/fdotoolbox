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

namespace FdoToolbox.Core.Feature.Overrides
{
    /// <summary>
    /// Table name override for King.Oracle
    /// 
    /// King.Oracle interprets class names as [user]~[table]~[geometry column], this override extracts the
    /// [table] component
    /// </summary>
    public class OracleTableNameOverride : ITableNameOverride
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        public string GetTableName(string fullName)
        {
            string[] tokens = fullName.Split('~');
            if (tokens.Length == 3)
                return tokens[1];
            return fullName;
        }
    }
}
