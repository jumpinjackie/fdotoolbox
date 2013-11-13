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
using FdoToolbox.Core.AppFramework;

namespace FdoUtil
{
    public class RegisterProviderCommand : ConsoleCommand
    {
        private string _name;
        private string _displayName;
        private string _description;
        private string _libraryPath;
        private string _version;
        private string _fdoVersion;
        private bool _isManaged;

        public RegisterProviderCommand(string name, string displayName, string description, string libraryPath, string version, string fdoVersion, bool isManaged)
        {
            _name = name;
            _displayName = displayName;
            _description = description;
            _libraryPath = libraryPath;
            _version = version;
            _fdoVersion = fdoVersion;
            _isManaged = isManaged;
        }

        public override int Execute()
        {
            FeatureAccessManager.GetProviderRegistry().RegisterProvider(
                _name,
                _displayName,
                _description,
                _version,
                _fdoVersion,
                _libraryPath,
                _isManaged);
            WriteLine("New provider registered: {0}", _name);
            return (int)CommandStatus.E_OK;
        }
    }
}
