#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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

using FdoToolbox.Base.Services;
using FdoToolbox.Base.Services.DragDropHandlers;
using FdoToolbox.Core.Connections;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;
using System;
using System.Linq;

namespace FdoToolbox.Express.DragDropHandlers
{
    public class DefaultFileHandler : IDragDropHandler
    {
        public string[] FileExtensions => FileExtensionMapper.GetSupportedExtensions().ToArray();

        public string GetHandlerDescription(string extension)
        {
            return $"Create new [{FileExtensionMapper.GetExtensionDescription(extension)}] connection";
        }

        public void HandleDrop(string file)
        {
            var connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
            var namer = ServiceManager.Instance.GetService<NamingService>();
            
            try
            {
                var (conn, provider) = FileExtensionMapper.TryCreateConnection(file);
                var wrapper = FdoConnection.FromInternalConnection(conn);

                string name = namer.GetDefaultConnectionName(provider, System.IO.Path.GetFileNameWithoutExtension(file));
                connMgr.AddConnection(name, wrapper);
            }
            catch (Exception ex)
            {
                LoggingService.Error("Failed to load connection", ex);
                return;
            }
        }
    }
}
