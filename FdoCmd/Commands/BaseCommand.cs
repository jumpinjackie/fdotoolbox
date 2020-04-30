#region LGPL Header
// Copyright (C) 2019, Jackie Ng
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
using FdoCmd.Core;
using FdoToolbox.Core.AppFramework;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Connections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FdoCmd.Commands
{
    public abstract class BaseCommand
    {
        /// <summary>
        /// If true, suppresses all console output. Check the status code returned
        /// by Execute() to determine successful execution.
        /// </summary>
        public bool IsSilent { get; set; }

        private int _indentationLevel = 0;

        class OutputIndentation : IDisposable
        {
            readonly BaseCommand _parent;

            public OutputIndentation(BaseCommand parent)
            {
                _parent = parent;
                _parent._indentationLevel++;
            }

            public void Dispose()
            {
                _parent._indentationLevel--;
            }
        }

        protected internal IDisposable Indent() => new OutputIndentation(this);

        protected static string RepeatString(string str, int iterations)
        {
            if (iterations == 0)
                return string.Empty;

            string result = "";
            for (int i = 0; i < iterations; i++)
            {
                result += str;
            }
            return result;
        }

        /// <summary>
        /// Writes a newline-terminated line to the application console disregarding the
        /// current indentation level
        /// </summary>
        /// <param name="str"></param>
        protected internal void WriteLineNoIndent(string str)
        {
            if (!IsSilent)
                Console.WriteLine(str);
        }

        /// <summary>
        /// Writes a newline-terminated line to the application console
        /// </summary>
        /// <param name="str"></param>
        protected internal void WriteLine(string str)
        {
            if (!IsSilent)
                Console.WriteLine(RepeatString("  ", _indentationLevel) + str);
        }

        /// <summary>
        /// Writes a line to the application console
        /// </summary>
        /// <param name="str"></param>
        protected internal void Write(string str)
        {
            if (!IsSilent)
                Console.Write(RepeatString("  ", _indentationLevel) + str);
        }

        /// <summary>
        /// Writes a warning message to the application console
        /// </summary>
        /// <param name="str">The message</param>
        protected internal void WriteWarning(string str)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Yellow))
                {
                    Console.WriteLine(RepeatString("  ", _indentationLevel) + str);
                }
            }
        }

        /// <summary>
        /// Writes a warning message to the application console
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected internal void WriteWarning(string format, params object[] args)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Yellow))
                {
                    Console.WriteLine(RepeatString("  ", _indentationLevel) + format, args);
                }
            }
        }

        /// <summary>
        /// Writes a newline-terminated line to the application console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected internal void WriteLines(IEnumerable<string> lines)
        {
            if (!IsSilent)
                Console.WriteLine(string.Join("\n", lines.Select(s => RepeatString("  ", _indentationLevel) + s)));
        }

        /// <summary>
        /// Writes a newline-terminated line to the application console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected internal void WriteLine(string format, params object[] args)
        {
            if (!IsSilent)
                Console.WriteLine(RepeatString("  ", _indentationLevel) + format, args);
        }

        /// <summary>
        /// Writes a line to the application console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected internal void Write(string format, params object[] args)
        {
            if (!IsSilent)
                Console.Write(RepeatString("  ", _indentationLevel) + format, args);
        }

        /// <summary>
        /// Writes an exception to the application console
        /// </summary>
        /// <param name="ex"></param>
        protected internal void WriteException(Exception ex)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Red))
                {
                    Console.Error.WriteLine(RepeatString("  ", _indentationLevel) + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Writes an error to the application console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected internal void WriteError(string format, params object[] args)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Red))
                {
                    Console.Error.WriteLine(RepeatString("  ", _indentationLevel) + format, args);
                }
            }
        }

        /// <summary>
        /// Writes an error to the application console
        /// </summary>
        /// <param name="str"></param>
        protected internal void WriteError(string str)
        {
            if (!IsSilent)
            {
                using (new TempConsoleColor(ConsoleColor.Red))
                {
                    Console.Error.WriteLine(RepeatString("  ", _indentationLevel) + str);
                }
            }
        }

        public abstract int Execute();

        protected bool HasCommand(IConnection conn, CommandType cmd, string capDesc, out int? retCode)
        {
            retCode = null;
            if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)cmd) < 0)
            {
                WriteError("This provider does not support " + capDesc);
                retCode = (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
                return false;
            }
            return true;
        }
    }
}
