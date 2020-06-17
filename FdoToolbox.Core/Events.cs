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
using FdoToolbox.Core.Feature;
using System.Data;
using System.Diagnostics;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Represents the method that will handle the RowChanging, RowChanged, RowDeleting and RowDeleted events of a 
    /// FdoFeatureTable
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FdoFeatureChangeEventHandler(object sender, FdoFeatureChangeEventArgs e);

    /// <summary>
    /// Represents a method that will handle requests for a spatial context
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="name"></param>
    public delegate void FdoSpatialContextRequestEventHandler(object sender, string name);

    /// <summary>
    /// Occures after a Feature has been changed
    /// </summary>
    [DebuggerStepThrough]
    public class FdoFeatureChangeEventArgs : EventArgs
    {
        private DataRowAction _action;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="action"></param>
        public FdoFeatureChangeEventArgs(FdoFeature feature, DataRowAction action)
        {
            Feature = feature;
            _action = action;
        }

        /// <summary>
        /// Gets the feature upon which an action has occured
        /// </summary>
        public FdoFeature Feature { get; }

        /// <summary>
        /// Gets the action that has occured on a FdoFeature
        /// </summary>
        public DataRowAction Action => _action;
    }

    /// <summary>
    /// Generic event object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// The event data
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public EventArgs(T data) { Data = data; }
    }

    /// <summary>
    /// Message event object
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// The message
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public MessageEventArgs(string message)
        {
            this.Message = message;
        }
    }

    #region Procedures

    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc();
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0>(A0 a0);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1>(A0 a0, A1 a1);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1, A2>(A0 a0, A1 a1, A2 a2);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1, A2, A3>(A0 a0, A1 a1, A2 a2, A3 a3);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1, A2, A3, A4>(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1, A2, A3, A4, A5>(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1, A2, A3, A4, A5, A6>(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1, A2, A3, A4, A5, A6, A7>(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7);
    /// <summary>
    /// Common delegate definition
    /// </summary>
    public delegate void Proc<A0, A1, A2, A3, A4, A5, A6, A7, A8>(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8);

    #endregion
}
