#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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

namespace FdoToolbox.Core.AppFramework
{
    /// <summary>
    /// Status codes that can be returned by any console application
    /// </summary>
    public enum CommandStatus : int
    {
        /// <summary>
        /// Operation OK, no errors encounters
        /// </summary>
        E_OK = 0,
        /// <summary>
        /// Failed to create SDF
        /// </summary>
        E_FAIL_SDF_CREATE = 1,
        /// <summary>
        /// Failed to apply schema
        /// </summary>
        E_FAIL_APPLY_SCHEMA = 2,
        /// <summary>
        /// Failed to destory datastore
        /// </summary>
        E_FAIL_DESTROY_DATASTORE = 3,
        /// <summary>
        /// Failed to connect
        /// </summary>
        E_FAIL_CONNECT = 4,
        /// <summary>
        /// Failed to serialize a feature schema
        /// </summary>
        E_FAIL_SERIALIZE_SCHEMA_XML = 5,
        /// <summary>
        /// Failed to create datastore
        /// </summary>
        E_FAIL_CREATE_DATASTORE = 6,
        /// <summary>
        /// Failed to bulk copy
        /// </summary>
        E_FAIL_BULK_COPY = 7,
        /// <summary>
        /// Task validation failed
        /// </summary>
        E_FAIL_TASK_VALIDATION = 8,
        /// <summary>
        /// Failed to create connection
        /// </summary>
        E_FAIL_CREATE_CONNECTION = 9,
        /// <summary>
        /// Failed to find intended schema
        /// </summary>
        E_FAIL_SCHEMA_NOT_FOUND = 10,
        /// <summary>
        /// Failed to find intended class
        /// </summary>
        E_FAIL_CLASS_NOT_FOUND = 11,
        /// <summary>
        /// The given capability is not supported
        /// </summary>
        E_FAIL_UNSUPPORTED_CAPABILITY = 12,
        /// <summary>
        /// The query results failed to load
        /// </summary>
        E_FAIL_LOAD_QUERY_RESULTS = 13,
        /// <summary>
        /// The task definition being attempted to load is not a recognised format
        /// </summary>
        E_FAIL_UNRECOGNISED_TASK_FORMAT = 14,
        /// <summary>
        /// Bulk Copy ran with errors logged
        /// </summary>
        E_FAIL_BULK_COPY_WITH_ERRORS = 15,
        /// <summary>
        /// Join operation ran with errors logged
        /// </summary>
        E_FAIL_JOIN_WITH_ERRORS = 16,
        /// <summary>
        /// The specified SQL statement was a SELECT statement
        /// </summary>
        E_FAIL_INVALID_SQL = 17,
        /// <summary>
        /// The specified SQL statement threw an exception during execution
        /// </summary>
        E_FAIL_SQL_EXECUTION_ERROR = 18,
        /// <summary>
        /// The specified provider does not support SQL commands
        /// </summary>
        E_FAIL_SQL_COMMAND_NOT_SUPPORTED = 19,
        /// <summary>
        /// The command was invoked with invalid or incomplete arguments
        /// </summary>
        E_FAIL_INVALID_ARGUMENTS = 20,
        /// <summary>
        /// A RunTask command was executed with a bcptask parameter specifying
        /// a copy task that doesn't exist
        /// </summary>
        E_FAIL_MISSING_BULK_COPY_TASKS = 21,
        /// <summary>
        /// An I/O error occured
        /// </summary>
        E_FAIL_IO_ERROR = 22,
        /// <summary>
        /// Unknown failure
        /// </summary>
        E_FAIL_UNKNOWN = 23,
        /// <summary>
        /// Not supported
        /// </summary>
        E_NOT_SUPPORTED = 24,
        /// <summary>
        /// No data
        /// </summary>
        E_NO_DATA
    }
}
