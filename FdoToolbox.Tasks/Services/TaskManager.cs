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
using FdoToolbox.Core.ETL;
using FdoToolbox.Core;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Base;
using System.IO;
using FdoToolbox.Core.Configuration;
using System.ComponentModel;

namespace FdoToolbox.Tasks.Services
{
    //TODO: Prevent orphaned tasks with dead connections by hooking to BeforeConnectionRemove event of FdoConnectionManager
    //and determining if any tasks currently in this object have references to the connection to be removed.
    public class TaskManager : IService
    {
        private bool _init = false;

        public bool IsInitialized
        {
            get { return _init; }
        }

        public void InitializeService()
        {
            _init = true;
        }

        public void UnloadService()
        {
            
        }

        public void Load()
        {
            TaskLoader ldr = new TaskLoader();
            string path = Preferences.SessionDirectory;
            if (System.IO.Directory.Exists(path))
            {
                string[] files = System.IO.Directory.GetFiles(path, "*" + TaskDefinitionHelper.BULKCOPYDEFINITION);
                foreach (string f in files)
                {
                    try
                    {
                        string name = string.Empty;
                        FdoBulkCopyOptions opt = ldr.BulkCopyFromXml(f, ref name, false);
                        FdoBulkCopy cpy = new FdoBulkCopy(opt);
                        AddTask(name, cpy);
                    }
                    catch { }
                }
                files = System.IO.Directory.GetFiles(path, "*" + TaskDefinitionHelper.JOINDEFINITION);
                foreach (string f in files)
                {
                    try
                    {
                        string name = string.Empty;
                        FdoJoinOptions opt = ldr.JoinFromXml(f, ref name, false);
                        FdoJoin join = new FdoJoin(opt);
                        AddTask(name, join);
                    }
                    catch { }
                }

                files = System.IO.Directory.GetFiles(path, "*" + TaskDefinitionHelper.SEQUENTIALPROCESS);
                foreach (string f in files)
                {
                    try
                    {
                        string prefix = Path.GetFileNameWithoutExtension(f);
                        string name = prefix;
                        int counter = 0;
                        while (this.NameExists(name))
                        {
                            counter++;
                            name = prefix + counter;
                        }
                        SequentialProcessDefinition spd = (SequentialProcessDefinition)SequentialProcessDefinition.Serializer.Deserialize(File.OpenRead(f));
                        FdoSequentialProcess proc = new FdoSequentialProcess(spd);
                        AddTask(name, proc);
                    }
                    catch { }
                }
            }
        }

        public void Save()
        {
            string path = Preferences.SessionDirectory;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            else
            {
                string [] files = System.IO.Directory.GetFiles(path, "*" + TaskDefinitionHelper.BULKCOPYDEFINITION);
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }
                files = System.IO.Directory.GetFiles(path, "*" + TaskDefinitionHelper.JOINDEFINITION);
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }
                files = System.IO.Directory.GetFiles(path, "*" + TaskDefinitionHelper.SEQUENTIALPROCESS);
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }
            }

            foreach (string key in _taskDict.Keys)
            {
                FdoSpecializedEtlProcess proc = _taskDict[key] as FdoSpecializedEtlProcess;
                if (proc != null)
                {
                    string file = System.IO.Path.Combine(path, key + proc.GetFileExtension());
                    proc.Save(file, key);
                }
            }
        }

        public event TaskBeforeRemoveEventHandler BeforeTaskRemoved = delegate { };

        public event TaskRenameEventHandler TaskRenamed = delegate { };

        public event TaskEventHandler TaskAdded = delegate { };

        public event TaskEventHandler TaskRemoved = delegate { };

        public event EventHandler Initialize = delegate { };

        public event EventHandler Unload = delegate { };

        private Dictionary<string, EtlProcess> _taskDict = new Dictionary<string, EtlProcess>();

        public void AddTask(string name, EtlProcess task)
        {
            if (_taskDict.ContainsKey(name))
                throw new ArgumentException("A task named " + name + " already exists");

            _taskDict.Add(name, task);
            TaskAdded(this, new EventArgs<string>(name));
        }

        public void RemoveTask(string name)
        {
            if (_taskDict.ContainsKey(name))
            {
                var e = new TaskBeforeRemoveEventArgs(name);
                this.BeforeTaskRemoved(this, e);
                if (e.Cancel)
                    return;

                EtlProcess proc = _taskDict[name];
                _taskDict.Remove(name);
                proc.Dispose();
                TaskRemoved(this, new EventArgs<string>(name));
            }
        }

        public void RenameTask(string oldName, string newName)
        {
            if (!_taskDict.ContainsKey(oldName))
                throw new InvalidOperationException("The task to be renamed could not be found: " + oldName);
            if (_taskDict.ContainsKey(newName))
                throw new InvalidOperationException("Cannot rename task " + oldName + " to " + newName + " as a task of that name already exists");

            EtlProcess proc = _taskDict[oldName];
            _taskDict.Remove(oldName);
            _taskDict.Add(newName, proc);
            TaskRenamed(this, new TaskRenameEventArgs(oldName, newName));
        }

        public EtlProcess GetTask(string name)
        {
            if (_taskDict.ContainsKey(name))
                return _taskDict[name];

            return null;
        }

        public bool NameExists(string name)
        {
            return _taskDict.ContainsKey(name);
        }

        public ICollection<string> GetTaskNames()
        {
            return _taskDict.Keys;
        }

        public void Clear()
        {
            List<string> names = new List<string>(GetTaskNames());
            foreach (string name in names)
            {
                this.RemoveTask(name);
            }
        }

        internal void UpdateConnectionReferences(string oldName, string newName)
        {
            foreach (string name in _taskDict.Keys)
            {
                var proc = _taskDict[name];
                if (typeof(IFdoSpecializedEtlProcess).IsAssignableFrom(proc.GetType()))
                {
                    IFdoSpecializedEtlProcess fp = (IFdoSpecializedEtlProcess)proc;
                    fp.UpdateConnectionReferences(oldName, newName);
                }
            }
        }
    }

    public delegate void TaskEventHandler(object sender, EventArgs<string> e);

    public delegate void TaskRenameEventHandler(object sender, TaskRenameEventArgs e);

    public delegate void TaskBeforeRemoveEventHandler(object sender, TaskBeforeRemoveEventArgs e);

    /// <summary>
    /// An event argument object passed when a connection is about to be renamed
    /// </summary>
    public class TaskBeforeRemoveEventArgs : CancelEventArgs
    {
        private readonly string _TaskName;

        /// <summary>
        /// Gets the name of the task to be renamed
        /// </summary>
        /// <value>The name of the task.</value>
        public string TaskName
        {
            get { return _TaskName; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionBeforeRemoveEventArgs"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TaskBeforeRemoveEventArgs(string name)
        {
            _TaskName = name;
            this.Cancel = false;
        }
    }

    public class TaskRenameEventArgs : EventArgs
    {
        private readonly string _oldName;
        private readonly string _newName;

        public string OldName
        {
            get { return _oldName; }
        }

        public string NewName
        {
            get { return _newName; }
        }

        public TaskRenameEventArgs(string oldName, string newName)
        {
            _oldName = oldName;
            _newName = newName;
        }
    }
}
