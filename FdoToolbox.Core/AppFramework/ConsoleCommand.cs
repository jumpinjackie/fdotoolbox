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
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Core.AppFramework
{
    /// <summary>
    /// Base console command class. All console commands derive from
    /// this class.
    /// </summary>
    public abstract class ConsoleCommand : IConsoleCommand
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns>The CommandStatus value</returns>
        public abstract int Execute();

        private bool _IsTestOnly;

        /// <summary>
        /// If true the command should run under simulation (no changes, if any,
        /// are performed)
        /// </summary>
        public bool IsTestOnly
        {
            get { return _IsTestOnly; }
            set { _IsTestOnly = value; }
        }

        private bool _IsSilent;

        /// <summary>
        /// If true, suppresses all console output. Check the status code returned
        /// by Execute() to determine successful execution.
        /// </summary>
        public bool IsSilent
        {
            get { return _IsSilent; }
            set { _IsSilent = value; }
        }

        /// <summary>
        /// Writes a newline-terminated line to the application console
        /// </summary>
        /// <param name="str"></param>
        protected void WriteLine(string str)
        {
            if (!IsSilent)
                Console.WriteLine(str);
        }

        /// <summary>
        /// Writes a line to the application console
        /// </summary>
        /// <param name="str"></param>
        protected void Write(string str)
        {
            if (!IsSilent)
                Console.Write(str);
        }

        /// <summary>
        /// Writes a warning message to the application console
        /// </summary>
        /// <param name="str">The message</param>
        protected void WriteWarning(string str)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Yellow))
                {
                    Console.WriteLine(str);
                }
            }
        }

        /// <summary>
        /// Writes a warning message to the application console
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void WriteWarning(string format, params object[] args)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Yellow))
                {
                    Console.WriteLine(format, args);
                }
            }
        }

        /// <summary>
        /// Writes a newline-terminated line to the application console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void WriteLine(string format, params object[] args)
        {
            if (!IsSilent)
                Console.WriteLine(format, args);
        }

        /// <summary>
        /// Writes a line to the application console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Write(string format, params object[] args)
        {
            if (!IsSilent)
                Console.Write(format, args);
        }

        /// <summary>
        /// Writes an exception to the application console
        /// </summary>
        /// <param name="ex"></param>
        protected void WriteException(Exception ex)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Red))
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Writes an error to the application console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void WriteError(string format, params object[] args)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Red))
                {
                    Console.Error.WriteLine(format, args);
                }
            }
        }

        /// <summary>
        /// Writes an error to the application console
        /// </summary>
        /// <param name="str"></param>
        protected void WriteError(string str)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Red))
                {
                    Console.Error.WriteLine(str);
                }
            }
        }

        /// <summary>
        /// Repeats a string for a given number of iterations
        /// </summary>
        /// <param name="str"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        protected static string RepeatString(string str, int iterations)
        {
            string result = "";
            for (int i = 0; i < iterations; i++)
            {
                result += str;
            }
            return result;
        }
        /// <summary>
        /// Creates a new FDO connection
        /// </summary>
        /// <returns></returns>
        protected static IConnection CreateConnection(string provider, string connStr)
        {
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            conn.ConnectionString = connStr;
            return conn;
        }

        public virtual void Abort()
        {
            WriteWarning("Command Aborting");
        }
    }
}
