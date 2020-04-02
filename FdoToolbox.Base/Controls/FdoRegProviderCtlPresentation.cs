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
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoRegProviderView
    {
        string ProviderName { get; }
        string DisplayName { get; }
        string Description { get; }
        string Version { get; }
        string FdoVersion { get; }
        string LibraryPath { get; }
        bool IsManaged { get; }
    }

    internal class FdoRegProviderPresentation
    {
        private readonly IFdoRegProviderView _view;

        public FdoRegProviderPresentation(IFdoRegProviderView view)
        {
            _view = view;
        }

        public bool Register()
        {
            FdoFeatureService.RegisterProvider(
                _view.ProviderName,
                _view.DisplayName,
                _view.Description,
                _view.Version,
                _view.FdoVersion,
                _view.LibraryPath,
                _view.IsManaged);
            return true;
        }
    }
}
