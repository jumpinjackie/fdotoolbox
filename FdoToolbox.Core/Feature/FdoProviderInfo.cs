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
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO provider information class
    /// </summary>
    public class FdoProviderInfo
    {
        private string _DisplayName;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return _DisplayName; }
            internal set { _DisplayName = value; }
        }
        private string _Description;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _Description; }
            internal set { _Description = value; }
        }
        private string _FeatureDataObjectsVersion;

        /// <summary>
        /// Gets or sets the feature data objects version.
        /// </summary>
        /// <value>The feature data objects version.</value>
        public string FeatureDataObjectsVersion
        {
            get { return _FeatureDataObjectsVersion; }
            internal set { _FeatureDataObjectsVersion = value; }
        }
        private bool _IsManaged;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is managed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is managed; otherwise, <c>false</c>.
        /// </value>
        public bool IsManaged
        {
            get { return _IsManaged; }
            internal set { _IsManaged = value; }
        }
        private string _LibraryPath;

        /// <summary>
        /// Gets or sets the library path.
        /// </summary>
        /// <value>The library path.</value>
        public string LibraryPath
        {
            get { return _LibraryPath; }
            internal set { _LibraryPath = value; }
        }
        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            internal set { _Name = value; }
        }
        private string _Version;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version
        {
            get { return _Version; }
            internal set { _Version = value; }
        }

        private bool _IsFlatFile;

        /// <summary>
        /// Gets or sets a value indicating whether this provider is a flat file provider.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is flat file; otherwise, <c>false</c>.
        /// </value>
        public bool IsFlatFile
        {
            get { return _IsFlatFile; }
            internal set { _IsFlatFile = value; }
        }
	

        internal FdoProviderInfo()
        {
        }
    }
}
