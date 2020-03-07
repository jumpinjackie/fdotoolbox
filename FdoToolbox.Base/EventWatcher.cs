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
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core;

namespace FdoToolbox.Base
{
    /// <summary>
    /// Helper class to log common events to the console
    /// </summary>
    internal sealed class EventWatcher
    {
        public static void Initialize()
        {
            FdoConnectionManager manager = ServiceManager.Instance.GetService<FdoConnectionManager>();
            NamingService namer = ServiceManager.Instance.GetService<NamingService>();
            manager.ConnectionAdded += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Connection added: {0}", e.Data);
            };
            manager.ConnectionRemoved += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Connection removed: {0}", e.Data);
                if (manager.GetConnectionNames().Count == 0)
                    namer.ResetCounter();
            };
            manager.ConnectionRenamed += delegate(object sender, ConnectionRenameEventArgs e)
            {
                LoggingService.InfoFormatted("Connection {0} renamed to {1}", e.OldName, e.NewName);
            };
            manager.ConnectionRefreshed += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Connection {0} refreshed", e.Data);
            };
        }
    }
}
