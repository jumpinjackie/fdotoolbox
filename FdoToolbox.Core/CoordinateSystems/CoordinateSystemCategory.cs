#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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
using System.Linq;

namespace FdoToolbox.Core.CoordinateSystems
{
    public class CoordinateSystemCategory : ICoordinateSystemCategory
    {
        readonly Lazy<ICoordinateSystem[]> _coordinateSystems;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystemCategory"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        internal CoordinateSystemCategory(ICoordinateSystemCatalog parent, string name)
        {
            Name = name;
            _coordinateSystems = new Lazy<ICoordinateSystem[]>(() => parent.EnumerateCoordinateSystems(name).ToArray());
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets an array of all coordinate systems in this category
        /// </summary>
        public ICoordinateSystem[] Items => _coordinateSystems.Value;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() => Name;
    }
}
