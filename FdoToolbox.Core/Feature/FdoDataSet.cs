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
using System.Data;
using System.Globalization;
using System.ComponentModel;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Represents an in-memory cache of spatial data
    /// </summary>
    [ToolboxItem(false)]
    public class FdoDataSet : DataSet
    {
        /// <summary>
		/// Initializes a new instance of the FeatureDataSet class.
		/// </summary>
        public FdoDataSet()
		{
			this.InitClass();
			System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
			//this.Tables.CollectionChanged += schemaChangedHandler;
			this.Relations.CollectionChanged += schemaChangedHandler;
			this.InitClass();
		}

        /// <summary>
        /// Gets the collection of tables contained within this data set
        /// </summary>
        public new FdoFeatureTableCollection Tables { get; private set; }

        /// <summary>
        /// Clones this data set
        /// </summary>
        /// <returns></returns>
        public new FdoDataSet Clone()
        {
            FdoDataSet ds = ((FdoDataSet)base.Clone());
            return ds;
        }

        private void InitClass()
        {
            Tables = new FdoFeatureTableCollection();
            //this.DataSetName = "FeatureDataSet";
            this.Prefix = "";
            this.Namespace = "http://tempuri.org/FeatureDataSet.xsd";
            this.Locale = CultureInfo.CurrentCulture;
            this.CaseSensitive = false;
            this.EnforceConstraints = true;
        }

        private void SchemaChanged(object sender, CollectionChangeEventArgs e)
        {
        }
    }

    /// <summary>
    /// A <see cref="FdoFeatureTable"/> collection
    /// </summary>
    public class FdoFeatureTableCollection : List<FdoFeatureTable> { }
}
