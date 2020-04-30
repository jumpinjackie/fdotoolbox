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

namespace FdoToolbox.Base.Forms
{
    internal interface IFdoMultiClassPickerView
    {
        FeatureSchemaCollection SchemaList { set; }
        FeatureSchema SelectedSchema { get; }

        ClassCollection ClassList { set; }
        IList<ClassDefinition> SelectedClasses { get; }

        string Title { set; }
        string Message { set; }
    }

    internal class FdoMultiClassPickerPresenter
    {
        private readonly IFdoMultiClassPickerView _view;

        public FdoMultiClassPickerPresenter(IFdoMultiClassPickerView view, string title, string message)
        {
            _view = view;
            _view.Title = title;
            _view.Message = message;
        }

        public void Init(FeatureSchemaCollection schemas)
        {
            _view.SchemaList = schemas;
        }

        public void SchemaChanged()
        {
            if (_view.SelectedSchema != null)
            {
                _view.ClassList = _view.SelectedSchema.Classes;
            }
        }
    }
}
