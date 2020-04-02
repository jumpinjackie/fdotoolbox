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
using FdoToolbox.Core.ETL.Operations;
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Base class for etl processes, provider registration and management
    /// services for the pipeline
    /// </summary>
    /// <typeparam name="TDerived">The type of the derived.</typeparam>
    public class EtlProcessBase<TDerived> : WithLoggingMixin
        where TDerived : EtlProcessBase<TDerived>
    {

        /// <summary>
        /// Ordered list of the operations in this process that will be added to the
        /// operations list after the initialization is completed.
        /// </summary>
        private readonly List<IFdoOperation> lastOperations = new List<IFdoOperation>();

        /// <summary>
        /// Ordered list of the operations in this process
        /// </summary>
        protected readonly List<IFdoOperation> operations = new List<IFdoOperation>();

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Registers the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public TDerived Register(IFdoOperation operation)
        {
            operations.Add(operation);
            Debug("Register {0} in {1}", operation.Name, Name);
            return (TDerived)this;
        }

        /// <summary>
        /// Registers the operation at the end of the operations queue
        /// </summary>
        /// <param name="operation">The operation.</param>
        public TDerived RegisterLast(IFdoOperation operation)
        {
            lastOperations.Add(operation);
            Debug("RegisterLast {0} in {1}", operation.Name, Name);
            return (TDerived)this;
        }

        /// <summary>
        /// Merges the last operations to the operations list.
        /// </summary>
        protected void MergeLastOperationsToOperations()
        {
            operations.AddRange(lastOperations);
        }

        /// <summary>
        /// Fires when a message is sent from this process
        /// </summary>
        public event MessageEventHandler ProcessMessage = delegate { };

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        protected void SendMessage(string msg)
        {
            ProcessMessage(this, new MessageEventArgs(msg));
        }

        /// <summary>
        /// Sends the message formatted.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void SendMessageFormatted(string format, params object[] args)
        {
            ProcessMessage(this, new MessageEventArgs(string.Format(format, args)));
        }
    }
}
