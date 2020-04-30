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
using FdoToolbox.Core;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// A generic object manager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ManagerBase<T> where T : class
    {
        /// <summary>
        /// The internal dictionary of managed objects
        /// </summary>
        protected Dictionary<string, T> _dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerBase&lt;T&gt;"/> class.
        /// </summary>
        protected ManagerBase()
        {
            _dict = new Dictionary<string, T>();
        }

        /// <summary>
        /// Gets the object names
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetNames()
        {
            return _dict.Keys;
        }

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="value">The object.</param>
        public virtual void Add(string name, T value)
        {
            if (_dict.ContainsKey(name))
                throw new FdoConnectionException("Unable to add object named " + name + " to the manager");

            _dict.Add(name, value);
            ObjectAdded(this, new EventArgs<string>(name));
        }

        /// <summary>
        /// Gets the specified object.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <returns></returns>
        public virtual T Get(string name)
        {
            if (_dict.ContainsKey(name))
                return _dict[name];
            return null;
        }

        /// <summary>
        /// Renames a specified object
        /// </summary>
        /// <param name="oldName">The current name of the specified object.</param>
        /// <param name="newName">The new name of the specified object.</param>
        public virtual void Rename(string oldName, string newName)
        {
            if(!_dict.ContainsKey(oldName))
                throw new InvalidOperationException("The object to be renamed could not be found: " + oldName);
            if (_dict.ContainsKey(newName))
                throw new InvalidOperationException("Cannot rename object " + oldName + " to " + newName + " as an object of that name already exists");

            T value = _dict[oldName];
            _dict.Remove(oldName);
            _dict.Add(newName, value);

            ManagerObjectRenameArgs e = new ManagerObjectRenameArgs(oldName, newName);
            ObjectRenamed(this, e);
        }

        /// <summary>
        /// Removes an object by name
        /// </summary>
        /// <param name="name">The name of the object.</param>
        public virtual void Remove(string name)
        {
            if (_dict.ContainsKey(name))
            {
                ManagerObjectBeforeRemoveEventArgs e = new ManagerObjectBeforeRemoveEventArgs(name);
                BeforeRemove(this, e);
                if (e.Cancel)
                    return;

                _dict.Remove(name);
                ObjectRemoved(this, new EventArgs<string>(name));
            }
        }

        /// <summary>
        /// Determines if an object exists (by name)
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <returns></returns>
        public virtual bool NameExists(string name)
        {
            return _dict.ContainsKey(name);
        }

        /// <summary>
        /// Removes all objects
        /// </summary>
        public virtual void Clear()
        {
            List<string> names = new List<string>(GetNames());
            foreach (string name in names)
            {
                this.Remove(name);
            }
        }

        /// <summary>
        /// Occurs before an object is removed. Subscribers have the opportunity to 
        /// cancel the removal operation by setting the Cancel property of the 
        /// <see cref="ManagerObjectBeforeRemoveEventArgs"/> object to true
        /// </summary>
        public event ManagerObjectBeforeRemoveEventHandler BeforeRemove = delegate { };
        /// <summary>
        /// Occurs when an object is added
        /// </summary>
        public event ManagerObjectEventHandler ObjectAdded = delegate { };
        /// <summary>
        /// Occurs when an object is removed
        /// </summary>
        public event ManagerObjectEventHandler ObjectRemoved = delegate { };
        /// <summary>
        /// Occurs when an object is renamed
        /// </summary>
        public event ManagerObjectRenamedEventHandler ObjectRenamed = delegate { };
    }

    /// <summary>
    /// Represents a method that is called before an object is to be renamed
    /// </summary>
    public delegate void ManagerObjectBeforeRemoveEventHandler(object sender, ManagerObjectBeforeRemoveEventArgs e);
    /// <summary>
    /// Represents a method that is called when an action has been performed on an object
    /// </summary>
    public delegate void ManagerObjectEventHandler(object sender, EventArgs<string> e);
    /// <summary>
    /// Represents a method that is called when an object has been renamed
    /// </summary>
    public delegate void ManagerObjectRenamedEventHandler(object sender, ManagerObjectRenameArgs e);

    /// <summary>
    /// An event object passed when a managed object is renamed
    /// </summary>
    public class ManagerObjectBeforeRemoveEventArgs
    {
        /// <summary>
        /// The name of the object
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ManagerObjectBeforeRemoveEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerObjectBeforeRemoveEventArgs"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ManagerObjectBeforeRemoveEventArgs(string name)
        {
            this.Name = name;
        }
    }

    /// <summary>
    /// An event object passed when a managed object is renamed
    /// </summary>
    public class ManagerObjectRenameArgs
    {
        /// <summary>
        /// The old name of the object
        /// </summary>
        public readonly string OldName;
        /// <summary>
        /// The new name of the object
        /// </summary>
        public readonly string NewName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerObjectRenameArgs"/> class.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        public ManagerObjectRenameArgs(string oldName, string newName)
        {
            this.OldName = oldName;
            this.NewName = newName;
        }
    }
}
