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
using System.Runtime.Serialization;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Thrown when any error is thrown by the FDO API
    /// </summary>
    [Serializable]
    public class FdoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected FdoException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        public FdoException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public FdoException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public FdoException(string msg, Exception inner) : base(msg, inner) { }
        internal FdoException(OSGeo.FDO.Common.Exception ex) : base(ex.Message, ex) { }
    }
}
