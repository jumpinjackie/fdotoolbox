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

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// This is a utility clsss that allows to treat a set of values as key, so it can be put
    /// into hash tables and retrieved easily.
    /// </summary>
    public class ObjectArrayKeys
    {
        private readonly object[] columnValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectArrayKeys"/> class.
        /// </summary>
        /// <param name="columnValues">The column values.</param>
        public ObjectArrayKeys(object[] columnValues)
        {
            this.columnValues = columnValues;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            ObjectArrayKeys other = obj as ObjectArrayKeys;
            if (other == null || other.columnValues.Length != this.columnValues.Length)
                return false;
            for (int i = 0; i < columnValues.Length; i++)
            {
                if (Equals(this.columnValues[i], other.columnValues[i]) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int result = 0;
            foreach (object value in columnValues)
            {
                if (value == null)
                    continue;
                result = 29 * result + value.GetHashCode();
            }
            return result;
        }
    }
}
