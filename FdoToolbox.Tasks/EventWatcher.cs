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
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using FdoToolbox.Core;
using ICSharpCode.Core;

namespace FdoToolbox.Tasks
{
    /// <summary>
    /// Task service activity logger
    /// </summary>
    public sealed class EventWatcher
    {
        public static void Initialize()
        {
            TaskManager tm = ServiceManager.Instance.GetService<TaskManager>();
            tm.TaskAdded += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Task added: {0}", e.Data);
            };
            tm.TaskRemoved += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Task removed: {0}", e.Data);
            };
            tm.TaskRenamed += delegate(object sender, TaskRenameEventArgs e)
            {
                LoggingService.InfoFormatted("Task {0} renamed to {1}", e.OldName, e.NewName);
            };
        }
    }
}
