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
using System.Runtime.Serialization;

namespace FdoToolbox.Base
{
    /// <summary>
    /// Represents a class of errors that can occur when creating FDO connections
    /// </summary>
    [Serializable]
    public class FdoConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnectionException"/> class.
        /// </summary>
        /// <param name="serInfo">The ser info.</param>
        /// <param name="ctx">The CTX.</param>
        protected FdoConnectionException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnectionException"/> class.
        /// </summary>
        public FdoConnectionException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnectionException"/> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public FdoConnectionException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnectionException"/> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="inner">The inner.</param>
        public FdoConnectionException(string msg, Exception inner) : base(msg, inner) { }
    }
}
