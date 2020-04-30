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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Expression;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// Abstract view
    /// </summary>
    public interface IFdoInsertView : IViewContent
    {
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        string ClassName { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [use transaction].
        /// </summary>
        /// <value><c>true</c> if [use transaction]; otherwise, <c>false</c>.</value>
        bool UseTransaction { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [use transaction enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use transaction enabled]; otherwise, <c>false</c>.
        /// </value>
        bool UseTransactionEnabled { set; get; }
        /// <summary>
        /// Initializes the grid.
        /// </summary>
        void InitializeGrid();
        /// <summary>
        /// Adds the data property.
        /// </summary>
        /// <param name="dp">The dp.</param>
        void AddDataProperty(DataPropertyDefinition dp);
		/// <summary>
		/// Adds the geometric property.
		/// </summary>
		/// <param name="gp">The gp.</param>
		void AddGeometricProperty(GeometricPropertyDefinition gp);
		/// <summary>
		/// Adds the object property.
		/// </summary>
		/// <param name="gp">The gp.</param>
		void AddObjectProperty(ObjectPropertyDefinition gp);
		/// <summary>
		/// Adds the association property.
		/// </summary>
		/// <param name="gp">The gp.</param>
		void AddAssociationProperty(AssociationPropertyDefinition gp);

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, ValueExpression> GetValues();
    }

    /// <summary>
    /// Handles presentation logic
    /// </summary>
    public class FdoInsertScaffoldPresenter
    {
        private readonly IFdoInsertView _view;
        private readonly FdoConnection _conn;
        private readonly string _className;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoInsertScaffoldPresenter"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="conn">The conn.</param>
        /// <param name="className">Name of the class.</param>
        public FdoInsertScaffoldPresenter(IFdoInsertView view, FdoConnection conn, string className)
        {
            _view = view;
            _conn = conn;
            _className = className;
            _view.Title = ICSharpCode.Core.ResourceService.GetString("TITLE_INSERT_FEATURE");
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        public void Init()
        {
            _view.InitializeGrid();
            _view.UseTransactionEnabled = (_conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsTransactions));

            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                ClassDefinition cd = service.GetClassByName(_className);
                if (cd != null)
                {
                    _view.ClassName = cd.Name;
                    foreach (PropertyDefinition pd in cd.Properties)
                    {
                        switch (pd.PropertyType)
                        { 
                            case PropertyType.PropertyType_DataProperty:
                                _view.AddDataProperty((DataPropertyDefinition)pd);
                                break;
                            case PropertyType.PropertyType_GeometricProperty:
                                _view.AddGeometricProperty((GeometricPropertyDefinition)pd);
                                break;

							case PropertyType.PropertyType_ObjectProperty:
								_view.AddObjectProperty((ObjectPropertyDefinition)pd);
							break;

							case PropertyType.PropertyType_AssociationProperty:
								_view.AddAssociationProperty((AssociationPropertyDefinition)pd);
							break;
						}
                    }
                }
            }
        }

        internal void Cancel()
        {
            _view.Close();
        }

        internal void Insert()
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                service.InsertFeature(_className, _view.GetValues(), _view.UseTransaction);
            }
            _view.ShowMessage(null, "Feature Inserted into: " + _className);
            _view.Close();
        }
    }
}
