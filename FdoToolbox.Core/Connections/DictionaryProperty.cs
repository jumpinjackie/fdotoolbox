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

namespace FdoToolbox.Core.Connections
{
    /// <summary>
    /// FDO connection/data store property
    /// </summary>
    public class DictionaryProperty
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the localized.
        /// </summary>
        /// <value>The name of the localized.</value>
        public string LocalizedName { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DictionaryProperty"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DictionaryProperty"/> is protected.
        /// </summary>
        /// <value><c>true</c> if protected; otherwise, <c>false</c>.</value>
        public bool Protected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DictionaryProperty"/> is enumerable.
        /// </summary>
        /// <value><c>true</c> if enumerable; otherwise, <c>false</c>.</value>
        public bool Enumerable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is file.
        /// </summary>
        /// <value><c>true</c> if this instance is file; otherwise, <c>false</c>.</value>
        public bool IsFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is path.
        /// </summary>
        /// <value><c>true</c> if this instance is path; otherwise, <c>false</c>.</value>
        public bool IsPath { get; set; }

        internal DictionaryProperty() { this.Enumerable = false; this.IsFile = false; this.IsPath = false; }
    }
}
