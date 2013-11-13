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
using FdoToolbox.Core.CoordinateSystems;
using System.ComponentModel;

namespace FdoToolbox.Base.Controls
{
    internal interface ICoordSysCatalogView
    {
        BindingList<CoordinateSystemDefinition> CoordSysDefinitions { set; }

        CoordinateSystemDefinition SelectedCS { get; }

        bool EditEnabled { set; }
        bool DeleteEnabled { set; }
    }

    internal class CoordSysCatalogPresenter
    {
        private readonly ICoordSysCatalogView _view;
        private FdoToolbox.Base.Services.CoordSysCatalog _catalog;
        private BindingList<CoordinateSystemDefinition> _list;

        public CoordSysCatalogPresenter(ICoordSysCatalogView view, FdoToolbox.Base.Services.CoordSysCatalog catalog)
        {
            _view = view;
            _catalog = catalog;
            _view.DeleteEnabled = false;
            _view.EditEnabled = false;
        }

        public void Init()
        {
            Refresh();
        }

        public void Refresh()
        {
            _view.CoordSysDefinitions = _list = _catalog.GetAllProjections(); 
        }

        public void AddNew(CoordinateSystemDefinition cs)
        {
            _catalog.AddProjection(cs);
        }

        public void Update(string oldName, CoordinateSystemDefinition cs)
        {
            _catalog.UpdateProjection(cs, oldName);
        }

        public void Delete(CoordinateSystemDefinition cs)
        {
            _catalog.DeleteProjection(cs);
        }

        public void CheckStatus()
        {
            _view.DeleteEnabled = (_view.SelectedCS != null);
            _view.EditEnabled = (_view.SelectedCS != null);
        }
    }
}
