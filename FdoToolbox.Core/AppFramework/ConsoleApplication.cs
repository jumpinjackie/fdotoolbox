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
using OSGeo.FDO.Commands;
using System.IO;

namespace FdoToolbox.Core.AppFramework
{
    /// <summary>
    /// Base application object for console applications
    /// </summary>
    public abstract class ConsoleApplication : IDisposable
    {
        /// <summary>
        /// The command to be executed
        /// </summary>
        protected IConsoleCommand _Command;

        /// <summary>
        /// Asks the specified question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns></returns>
        public bool Ask(string question)
        {
            Console.WriteLine("{0} [y/n]?", question);
            ConsoleKeyInfo input = Console.ReadKey();
            while (input.Key != ConsoleKey.Y && input.Key != ConsoleKey.N)
            {
                Console.WriteLine("Unknown response. Try again.");
                Console.WriteLine("{0} [y/n]?", question);
            }
            return input.Key == ConsoleKey.Y;
        }

        /// <summary>
        /// Throws an ArgumentException if the given parameter value is empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        protected static void ThrowIfEmpty(string value, string parameter)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Missing required parameter: " + parameter);
        }

        /// <summary>
        /// Parse application-specific arguments.
        /// </summary>
        /// <param name="args">The array of commandline arguments</param>
        public abstract void ParseArguments(string[] args);

        /// <summary>
        /// Display usage information for this application
        /// </summary>
        public abstract void ShowUsage();

        /// <summary>
        /// Run the application
        /// </summary>
        /// <param name="args">The array of commandline arguments</param>
        public virtual void Run(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
            try
            {
                ParseArguments(args);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ShowUsage();
                Environment.ExitCode = (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS;
                return;
            }

#if DEBUG
            if (_Command != null)
                Console.WriteLine("Silent: {0}\nTest: {1}", _Command.IsSilent, _Command.IsTestOnly);
#endif

            int retCode = (int)CommandStatus.E_OK;
            if (_Command != null)
            {
                try
                {
                    retCode = _Command.Execute();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                    retCode = (int)CommandStatus.E_FAIL_UNKNOWN;
                }
            }
#if DEBUG
            Console.WriteLine("Status: {0}", retCode);
#endif
            System.Environment.ExitCode = retCode;
        }

        void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_Command != null)
            {
                _Command.Abort();
            }
        }

        /// <summary>
        /// Verifies the file name exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The file name.</returns>
        protected string CheckFile(string fileName)
        {
            string file = fileName;
            if (!File.Exists(file))
            {
                if (!Path.IsPathRooted(file))
                    file = Path.Combine(this.AppPath, file);

                if (!File.Exists(file))
                    throw new ArgumentException("Unable to find file: " + fileName);
            }
            return file;
        }

        /// <summary>
        /// Current working directory path of the application
        /// </summary>
        public string AppPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        private bool _IsTest;

        /// <summary>
        /// Is this application being run in simulation mode?
        /// </summary>
        public bool IsTestOnly
        {
            get { return _IsTest; }
            set { _IsTest = value; }
        }

        private bool _IsSilent;

        /// <summary>
        /// Is this application running silent? (no console output)
        /// </summary>
        public bool IsSilent
        {
            get { return _IsSilent; }
            set { _IsSilent = value; }
        }


        /// <summary>
        /// Gets an argument value with the given prefix. Arguments follow the
        /// convention [prefix]:[value]
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="args"></param>
        /// <returns>The argument value if found, otherwise null</returns>
        protected static string GetArgument(string prefix, string[] args)
        {
            if (args.Length == 0)
                return null;

            foreach (string arg in args)
            {
                if (arg.StartsWith(prefix))
                {
                    string argument = arg.Substring(arg.IndexOf(":") + 1);
                    return argument;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a given parameter switch was defined
        /// </summary>
        /// <param name="strSwitch"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected static bool IsSwitchDefined(string strSwitch, string[] args)
        {
            if (args.Length == 0)
                return false;

            foreach (string arg in args)
            {
                if (arg == strSwitch || arg.StartsWith(strSwitch))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            
        }
    }
}
