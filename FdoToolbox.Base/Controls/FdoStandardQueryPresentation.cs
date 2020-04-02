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

using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands;
using System.Windows.Forms;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoStandardQueryView : IQuerySubView
    {
        string[] SchemaList { set; }
        ClassDescriptor[] ClassList { set; }

        string SelectedSchema { get; }
        ClassDescriptor SelectedClass { get; }

        ClassDefinition SelectedClassDefinition { get; }

        IList<string> PropertyList { set; }
        
        IList<ComputedProperty> ComputedProperties { get; set; }

        string Filter { get; set; }
        int Limit { get; set; }

        FeatureQueryOptions QueryObject { get; }
        IList<string> SelectPropertyList { get; }
        IList<string> OrderByList { get; }

        string SelectedOrderByProperty { get; }
        string SelectedOrderByPropertyToAdd { get; }

        void AddOrderBy(string prop);
        void RemoveOrderBy(string prop);

        bool OrderingEnabled { get; set; }

        bool UseExtendedSelectForOrdering { get; set; }
    }

    internal class FdoStandardQueryPresenter
    {
        private IFdoStandardQueryView _view;
        private FdoConnection _conn;
        private FdoFeatureService _service;
        private SchemaWalker _walker;

        public FdoStandardQueryPresenter(IFdoStandardQueryView view, FdoConnection conn)
        {
            _view = view;
            _conn = conn;
            _service = _conn.CreateFeatureService();
            _view.UseExtendedSelectForOrdering = false;
            bool bExtended = Array.IndexOf(conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList), CommandType.CommandType_ExtendedSelect) >= 0;
            _view.OrderingEnabled = conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSelectOrdering) || bExtended;
            _view.UseExtendedSelectForOrdering = bExtended;
            _walker = SchemaWalker.GetWalker(conn);
        }

        public void GetSchemas()
        {
            _view.SchemaList = _walker.GetSchemaNames();
        }

        public void SchemaChanged()
        {
            if (_view.SelectedSchema != null)
            {
                _view.ClassList = _walker.GetClassNames(_view.SelectedSchema);
            }
        }

        public ClassDefinition SelectedClass { get; private set; }

        public void ClassChanged()
        {
            if (_view.SelectedClass != null)
            {
                var cls = _walker.GetClassDefinition(_view.SelectedSchema, _view.SelectedClass.ClassName);
                this.SelectedClass = cls;
                List<string> p = new List<string>();
                foreach (PropertyDefinition pd in cls.Properties)
                {
                    if (pd.PropertyType == PropertyType.PropertyType_DataProperty || pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
						p.Add(pd.Name);
					else if (pd.PropertyType == PropertyType.PropertyType_ObjectProperty)
					{
						String szPrefix = pd.Name;
						ObjectPropertyDefinition pdo = (ObjectPropertyDefinition)pd;

						// TODO: make this an iterative loop
						// only processing one sub-set for now
		                foreach (PropertyDefinition pdSub in pdo.Class.Properties)
						{
							if (pdSub.PropertyType == PropertyType.PropertyType_DataProperty || pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
								p.Add(szPrefix + "." + pdSub.Name);

							// TODO: process objects and associations
						}
					}
					else if (pd.PropertyType == PropertyType.PropertyType_AssociationProperty)
					{
						String szPrefix = pd.Name;
						AssociationPropertyDefinition pda = (AssociationPropertyDefinition)pd;

						// TODO: make this an iterative loop
						// only processing one sub-set for now
						foreach (PropertyDefinition pdSub in pda.AssociatedClass.Properties)
						{
							if (pdSub.PropertyType == PropertyType.PropertyType_DataProperty || pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
								p.Add(szPrefix + "." + pdSub.Name);

							// TODO: process objects and associations
						}
					}
                }
                _view.PropertyList = p;
                _view.FireMapPreviewStateChanged(cls.ClassType == ClassType.ClassType_FeatureClass);
            }
        }

        public void RemoveOrderByProperty()
        {
            string prop = _view.SelectedOrderByProperty;
            if (prop != null)
                _view.RemoveOrderBy(prop);
        }

        public void AddOrderByProperty()
        {
            string prop = _view.SelectedOrderByPropertyToAdd;
            if (prop != null)
            {
                if (_view.UseExtendedSelectForOrdering)
                {
                    if (_view.OrderByList.Count == 0)
                        _view.AddOrderBy(prop);
                    else
                        MessageBox.Show("You can only add one property");
                }
                else
                {
                    if (!_view.OrderByList.Contains(prop))
                        _view.AddOrderBy(prop);
                }
            }
        }
    }
}
