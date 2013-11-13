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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Expression;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoUpdateView : IViewContent
    {
        string ClassName { set; get; }
        string UpdateFilter { set; get; }
        bool UseTransaction { set; get; }
        bool UseTransactionEnabled { set; get; }
        void InitializeGrid();

		void AddDataProperty(DataPropertyDefinition dp, object value);
		void AddDataProperty(DataPropertyDefinition dp, object value, String szClassHierarchy);

		void AddGeometricProperty(GeometricPropertyDefinition gp, string fgft);
		void AddGeometricProperty(GeometricPropertyDefinition gp, string fgft, String szClassHierarchy);

        Dictionary<string, ValueExpression> GetValues();
    }

    internal class FdoUpdateScaffoldPresenter
    {
        private readonly IFdoUpdateView _view;
        private readonly FdoConnection _conn;
        private readonly string _className;
        private readonly string _filter;
        private readonly FdoFeature _feature;

        public FdoUpdateScaffoldPresenter(IFdoUpdateView view, FdoFeature feat, FdoConnection conn, string filter)
        {
            _view = view;
            _conn = conn;
            _className = feat.Table.TableName;
            _view.Title = ICSharpCode.Core.ResourceService.GetString("TITLE_UPDATE_FEATURE");
            _view.UpdateFilter = filter;
            _filter = filter;
            _feature = feat;
        }

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
                                _view.AddDataProperty((DataPropertyDefinition)pd, _feature[pd.Name]);
                                break;

                            case PropertyType.PropertyType_GeometricProperty:
								if (null != (_feature[pd.Name] as FdoGeometry))	// else throws exeption with the ".Text" null reference
									_view.AddGeometricProperty((GeometricPropertyDefinition)pd, (_feature[pd.Name] as FdoGeometry).Text);
                            break;

							case PropertyType.PropertyType_AssociationProperty:
								AssociationPropertyDefinition pdAssoc = (AssociationPropertyDefinition)pd;

				                ClassDefinition cdAssoc = service.GetClassByName(pdAssoc.AssociatedClass.Name);
								foreach (PropertyDefinition pdThis in cdAssoc.Properties)
								{
									String szPropertyName = pd.Name + "." + pdThis.Name;

									// TODO: iterate associations and objects
									switch (pdThis.PropertyType)
									{
										case PropertyType.PropertyType_DataProperty:
											// TODO: fix this issue of reverse association
											// at the moment can get geom from parent to child - but throws error from child to parent
											try
											{
												_view.AddDataProperty((DataPropertyDefinition)pdThis, _feature[szPropertyName], pd.Name);
											}
											catch (Exception exThis)
											{
												// System.Windows.Forms.MessageBox.Show("Cannot Edit Association Property\n\n" + exThis.Message);
											}
										break;

										case PropertyType.PropertyType_GeometricProperty:
											// TODO: fix this issue of reverse association
											// at the moment can get geom from parent to child - but throws error from child to parent
											try
											{
												if (null != (_feature[szPropertyName] as FdoGeometry))	// else throws exeption with the ".Text" null reference
													_view.AddGeometricProperty((GeometricPropertyDefinition)pdThis, (_feature[szPropertyName] as FdoGeometry).Text, pd.Name);
											}
											catch (Exception exThis)
											{
												System.Windows.Forms.MessageBox.Show("Cannot Edit Association Geometry\n\n" + exThis.Message);
											}
										break;
									}
								}
							break;

							case PropertyType.PropertyType_ObjectProperty:
								ObjectPropertyDefinition pdObject = (ObjectPropertyDefinition)pd;
								
								ClassDefinition cdObject = service.GetClassByName(pdObject.Class.Name);
								foreach (PropertyDefinition pdThis in cdObject.Properties)
								{
									String szPropertyName = pd.Name + "." + pdThis.Name;

									// TODO: iterate associations and objects
									switch (pdThis.PropertyType)
									{
										case PropertyType.PropertyType_DataProperty:
											_view.AddDataProperty((DataPropertyDefinition)pdThis, _feature[szPropertyName], pd.Name);
										break;

										case PropertyType.PropertyType_GeometricProperty:
											if (null != (_feature[szPropertyName] as FdoGeometry))	// else throws exeption with the ".Text" null reference
												_view.AddGeometricProperty((GeometricPropertyDefinition)pdThis, (_feature[szPropertyName] as FdoGeometry).Text, pd.Name);
										break;
									}
								}
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

        internal void Update()
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                int updated = service.UpdateFeatures(_className, _view.GetValues(), _filter, _view.UseTransaction);
                if (updated > 0)
                    _view.ShowMessage(null, updated + " feature(s) updated");
                else
                    _view.ShowMessage(null, "No features updated");
                _view.Close();
            }
        }
    }
}
