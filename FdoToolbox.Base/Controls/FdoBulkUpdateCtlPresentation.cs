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
using OSGeo.FDO.Expression;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoBulkUpdateView : IViewContent
    {
        string ClassName { set; get; }
        bool UseTransaction { set; get; }
        bool UseTransactionEnabled { set; get; }
        void InitializeGrid();
        void AddDataProperty(DataPropertyDefinition dp);
        void AddGeometricProperty(GeometricPropertyDefinition gp);
        string Filter { set; get; }
        Dictionary<string, ValueExpression> GetValues();
    }

    internal class FdoBulkUpdatePresenter
    {
        private readonly IFdoBulkUpdateView _view;
        private readonly FdoConnection _conn;
        private readonly string _className;

        public FdoBulkUpdatePresenter(IFdoBulkUpdateView view, FdoConnection conn, string className)
        {
            _view = view;
            _conn = conn;
            _className = className;
            _view.Title = ICSharpCode.Core.ResourceService.GetString("TITLE_BULK_UPDATE_FEATURE");
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
                                _view.AddDataProperty((DataPropertyDefinition)pd);
                                break;
                            case PropertyType.PropertyType_GeometricProperty:
                                _view.AddGeometricProperty((GeometricPropertyDefinition)pd);
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
            if (_view.Confirm("Bulk Update", "Bulk updates can be very lengthy. Are you sure you want to do this?"))
            {
                using (FdoFeatureService service = _conn.CreateFeatureService())
                {
                    int updated = service.UpdateFeatures(_className, _view.GetValues(), _view.Filter, _view.UseTransaction);
                    _view.ShowMessage(null, updated + " feature(s) updated");
                }

                if (_conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsFlush))
                {
                    _conn.Flush();
                }

                _view.Close();
            }
        }

        internal void TestUpdate()
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                long count = service.GetFeatureCount(_className, _view.Filter, true);
                _view.ShowMessage(null, count + " feature(s) would be updated");
            }
        }
    }
}
