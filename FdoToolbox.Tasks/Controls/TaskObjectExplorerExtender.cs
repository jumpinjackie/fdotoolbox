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
using FdoToolbox.Base.Controls;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using System.Windows.Forms;
using FdoToolbox.Core.ETL.Specialized;
using ICSharpCode.Core;

namespace FdoToolbox.Tasks.Controls
{
    public class TaskObjectExplorerExtender : IObjectExplorerExtender
    {
        private string RootNodeName = "NODE_TASKS";

        const string PATH_TASKS = "/ObjectExplorer/ContextMenus/Tasks";
        const string PATH_SELECTED_TASK = "/ObjectExplorer/ContextMenus/SelectedTask";

        const string NODE_TASK = "NODE_TASK";

        const string IMG_TASK = "application_go";
        const string IMG_COPY = "table_go";
        const string IMG_JOIN = "table_relationship";
        const string IMG_SEQPROC = "application_double";

        private IObjectExplorer _explorer;
        private TaskManager _taskMgr;
        private FdoConnectionManager _connMgr;

        public void Decorate(IObjectExplorer explorer)
        {
            _explorer = explorer;
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            _connMgr.BeforeConnectionRemove += new ConnectionBeforeRemoveHandler(OnBeforeConnectionRemove);
            _connMgr.ConnectionRenamed += new ConnectionRenamedEventHandler(OnConnectionRenamed);
            _taskMgr = ServiceManager.Instance.GetService<TaskManager>();
            _taskMgr.TaskAdded += new TaskEventHandler(OnTaskAdded);
            _taskMgr.TaskRemoved += new TaskEventHandler(OnTaskRemoved);
            _taskMgr.TaskRenamed += new TaskRenameEventHandler(OnTaskRenamed);

            _explorer.RegisterImage(IMG_TASK);
            _explorer.RegisterImage(IMG_JOIN);
            _explorer.RegisterImage(IMG_SEQPROC);

            _explorer.RegisterRootNode(RootNodeName, "Tasks", IMG_TASK, PATH_TASKS);
            _explorer.RegisterContextMenu(NODE_TASK, PATH_SELECTED_TASK);
        }

        void OnConnectionRenamed(object sender, ConnectionRenameEventArgs e)
        {
            _taskMgr.UpdateConnectionReferences(e.OldName, e.NewName);
        }

        void OnBeforeConnectionRemove(object sender, ConnectionBeforeRemoveEventArgs e)
        {
            foreach (var name in _taskMgr.GetTaskNames())
            {
                var task = _taskMgr.GetTask(name);
                if (typeof(FdoBulkCopy).IsAssignableFrom(task.GetType()))
                {
                    var bcp = (FdoBulkCopy)task;
                    var names = new List<string>(bcp.Options.ConnectionNames);
                    if (names.Contains(e.ConnectionName))
                    {
                        MessageService.ShowMessage("Cannot remove connection as the task (" + name + ") depends on this connection");
                        e.Cancel = true;
                        return;
                    }
                }
                else if (typeof(FdoJoin).IsAssignableFrom(task.GetType()))
                {
                    var join = (FdoJoin)task;
                    var conn = _connMgr.GetConnection(e.ConnectionName);

                    var opts = join.Options;
                    if (opts.Left.Connection == conn ||
                        opts.Right.Connection == conn ||
                        opts.Target.Connection == conn)
                    {
                        MessageService.ShowMessage("Cannot remove connection as the task (" + name + ") depends on this connection");
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        void OnTaskRenamed(object sender, TaskRenameEventArgs e)
        {
            TreeNode node = _explorer.GetRootNode(RootNodeName).Nodes[e.OldName];
            node.Name = node.Text = e.NewName;
        }

        void OnTaskRemoved(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            _explorer.GetRootNode(RootNodeName).Nodes.RemoveByKey(e.Data);
        }

        void OnTaskAdded(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            string name = e.Data;
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = IMG_TASK;
            node.ContextMenuStrip = _explorer.GetContextMenu(NODE_TASK);

            TreeNode root = _explorer.GetRootNode(RootNodeName);
            root.Nodes.Add(node);
            node.Expand();
            root.Expand();
        }
    }
}
