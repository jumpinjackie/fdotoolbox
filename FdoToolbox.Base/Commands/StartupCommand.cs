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
using ICSharpCode.Core;
using System.Resources;
using System.Reflection;
using FdoToolbox.Base.Services;
using Res = ICSharpCode.Core.ResourceService;
using Msg = ICSharpCode.Core.MessageService;
using FdoToolbox.Core;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base.Services.DragDropHandlers;
using System.IO;
using System.Windows.Forms;

namespace FdoToolbox.Base.Commands
{
    /// <summary>
    /// The application startup command
    /// </summary>
    public class StartupCommand : AbstractCommand
    {
        /// <summary>
        /// Invokes the command.
        /// </summary>
        public override void Run()
        {
            EventWatcher.Initialize();
            ServiceManager svcMgr = ServiceManager.Instance;
            
            Res.RegisterNeutralStrings(FdoToolbox.Base.Strings.ResourceManager);
            Res.RegisterNeutralImages(FdoToolbox.Base.Images.ResourceManager);
            Res.RegisterNeutralStrings(ResourceUtil.StringResourceManager);

            Workbench.WorkbenchInitialized += delegate
            {
                Workbench wb = Workbench.Instance;
                List<IObjectExplorerExtender> extenders = AddInTree.BuildItems<IObjectExplorerExtender>("/ObjectExplorer/Extenders", this);
                if (extenders != null)
                {
                    foreach (IObjectExplorerExtender dec in extenders)
                    {
                        dec.Decorate(wb.ObjectExplorer);
                    }
                }

                svcMgr.RestoreSession();
                Msg.MainForm = wb;

                //Find and register drag and drop handlers
                List<IDragDropHandler> handlers = AddInTree.BuildItems<IDragDropHandler>("/FdoToolbox/DragDropHandlers", this);
                if (handlers != null)
                {
                    IDragDropHandlerService handlerSvc = ServiceManager.Instance.GetService<IDragDropHandlerService>();
                    foreach (IDragDropHandler h in handlers)
                    {
                        handlerSvc.RegisterHandler(h);
                    }
                }

                wb.FormClosing += delegate
                {
                    svcMgr.UnloadAllServices();
                };
            };
        }
    }
}
