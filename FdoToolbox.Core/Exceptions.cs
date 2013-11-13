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
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core
{
    [global::System.Serializable]
    public class SchemaNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SchemaNotFoundException(string name) 
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        protected SchemaNotFoundException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Thrown during any part of an ETL process
    /// </summary>
    [Serializable]
    public class FdoETLException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoETLException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected FdoETLException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoETLException"/> class.
        /// </summary>
        public FdoETLException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoETLException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public FdoETLException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoETLException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public FdoETLException(string msg, Exception inner) : base(msg, inner) 
        {
            if (inner.Data.Count > 0)
            {
                foreach (var key in inner.Data.Keys)
                {
                    this.Data[key] = inner.Data[key];
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MissingKeyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected MissingKeyException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
        /// </summary>
        public MissingKeyException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public MissingKeyException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public MissingKeyException(string msg, Exception inner) : base(msg, inner) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ParameterCountException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCountException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected ParameterCountException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCountException"/> class.
        /// </summary>
        public ParameterCountException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCountException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public ParameterCountException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCountException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ParameterCountException(string msg, Exception inner) : base(msg, inner) { }
    }

    /// <summary>
    /// Thrown when a requested feature is not supported
    /// </summary>
    [Serializable]
    public class UnsupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected UnsupportedException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedException"/> class.
        /// </summary>
        public UnsupportedException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public UnsupportedException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public UnsupportedException(string msg, Exception inner) : base(msg, inner) { }
    }

    /// <summary>
    /// Thrown when a task definition fails to be serialized or deserialized
    /// </summary>
    [Serializable]
    public class TaskLoaderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskLoaderException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected TaskLoaderException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskLoaderException"/> class.
        /// </summary>
        public TaskLoaderException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskLoaderException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public TaskLoaderException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskLoaderException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public TaskLoaderException(string msg, Exception inner) : base(msg, inner) { }
    }

    /// <summary>
    /// Thrown when task validation has failed
    /// </summary>
    [Serializable]
    public class TaskValidationException : Exception
    {
        private string[] _errors = new string[0];

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public string[] Errors { get { return _errors; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskValidationException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected TaskValidationException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskValidationException"/> class.
        /// </summary>
        public TaskValidationException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskValidationException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public TaskValidationException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskValidationException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public TaskValidationException(string msg, Exception inner) : base(msg, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskValidationException"/> class.
        /// </summary>
        /// <param name="errors">The error list.</param>
        public TaskValidationException(List<string> errors) : base() { _errors = errors.ToArray(); }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </PermissionSet>
        public override string ToString()
        {
            if (this.Errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder("The following validation errors were encountered: ");
                foreach (string str in this.Errors)
                {
                    sb.AppendFormat(" - {0}\n", str);
                }
                return sb.ToString();
            }
            else
            {
                return base.ToString();
            }
        }
    }

    /// <summary>
    /// Thrown when any operation in the FdoFeatureService fails
    /// </summary>
    [Serializable]
    public class FeatureServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureServiceException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected FeatureServiceException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureServiceException"/> class.
        /// </summary>
        public FeatureServiceException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureServiceException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public FeatureServiceException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureServiceException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public FeatureServiceException(string msg, Exception inner) : base(msg, inner) { }
    }
}