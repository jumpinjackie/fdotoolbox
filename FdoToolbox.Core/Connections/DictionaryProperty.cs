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

namespace FdoToolbox.Core.Connections
{
    /// <summary>
    /// FDO connection/data store property
    /// </summary>
    public class DictionaryProperty
    {
        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _LocalizedName;

        /// <summary>
        /// Gets or sets the name of the localized.
        /// </summary>
        /// <value>The name of the localized.</value>
        public string LocalizedName
        {
            get { return _LocalizedName; }
            set { _LocalizedName = value; }
        }

        private string _DefaultValue;

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue
        {
            get { return _DefaultValue; }
            set { _DefaultValue = value; }
        }

        private bool _Required;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DictionaryProperty"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required
        {
            get { return _Required; }
            set { _Required = value; }
        }

        private bool _Protected;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DictionaryProperty"/> is protected.
        /// </summary>
        /// <value><c>true</c> if protected; otherwise, <c>false</c>.</value>
        public bool Protected
        {
            get { return _Protected; }
            set { _Protected = value; }
        }

        private bool _Enumerable;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DictionaryProperty"/> is enumerable.
        /// </summary>
        /// <value><c>true</c> if enumerable; otherwise, <c>false</c>.</value>
        public bool Enumerable
        {
            get { return _Enumerable; }
            set { _Enumerable = value; }
        }

        private bool _IsFile;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is file.
        /// </summary>
        /// <value><c>true</c> if this instance is file; otherwise, <c>false</c>.</value>
        public bool IsFile
        {
            get { return _IsFile; }
            set { _IsFile = value; }
        }

        private bool _IsPath;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is path.
        /// </summary>
        /// <value><c>true</c> if this instance is path; otherwise, <c>false</c>.</value>
        public bool IsPath
        {
            get { return _IsPath; }
            set { _IsPath = value; }
        }

        internal DictionaryProperty() { this.Enumerable = false; this.IsFile = false; this.IsPath = false; }
    }
}
