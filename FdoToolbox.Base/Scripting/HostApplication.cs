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
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using System.Reflection;
using System.IO;
using FdoToolbox.Base.Forms;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime.Types;
using System.Windows.Forms;

namespace FdoToolbox.Base.Scripting
{
    /// <summary>
    /// A helper class that exposes the FDO Toolbox application model and core
    /// services out for python scripts to consume.
    /// </summary>
    internal class HostApplication
    {
        internal const string SCRIPT_NAME = "App";

        internal static void InitializeScriptScope(ScriptScope scope)
        {
            PythonType pt = DynamicHelpers.GetPythonTypeFromType(typeof(HostApplication));
            scope.SetVariable(SCRIPT_NAME, pt);
        }

        /// <summary>
        /// Gets the version of this application
        /// </summary>
        /// <value>The version.</value>
        public static string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        /// Gets the connection manager.
        /// </summary>
        /// <value>The connection manager.</value>
        public static FdoConnectionManager ConnectionManager
        {
            get { return ServiceManager.Instance.GetService<FdoConnectionManager>(); }
        }

        /// <summary>
        /// Shows the message in a simple dialog box
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        public static void ShowMessage(string title, string message)
        {
            MessageService.ShowMessage(message, title);
        }

        /// <summary>
        /// Writes the specified message to the application console.
        /// </summary>
        /// <param name="msg">The message</param>
        public static void WriteLine(string msg)
        {
            LoggingService.Info(msg);
        }

        /// <summary>
        /// Creates the fdo connection.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        public static void CreateFdoFileConnection(string file, string name)
        {
            FdoConnection conn = ExpressUtility.CreateFlatFileConnection(file);
            ConnectionManager.AddConnection(name, conn);
        }

        /// <summary>
        /// Creates the fdo connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="name">The name.</param>
        public static void CreateFdoConnection(string provider, string connectionString, string name)
        {
            FdoConnection conn = new FdoConnection(provider, connectionString);
            ConnectionManager.AddConnection(name, conn);
        }

        /// <summary>
        /// Loads a saved connection, the name of the connection will be the name of the
        /// file (without the file extension)
        /// </summary>
        /// <param name="path"></param>
        public static void LoadConnection(string path)
        {
            FdoConnection conn = FdoConnection.LoadFromFile(path);
            string name = Path.GetFileNameWithoutExtension(path);
            ConnectionManager.AddConnection(name, conn);
        }

        /// <summary>
        /// Prompts the user for a currently open connection
        /// </summary>
        /// <returns>The selected FDO connection. Returns null if user cancelled or did not make a selection</returns>
        public static FdoConnection PickConnection()
        {
            string name = ConnectionPicker.GetConnectionName();
            if (!string.IsNullOrEmpty(name))
                return ConnectionManager.GetConnection(name);

            return null;
        }

        /// <summary>
        /// Invokes an open file dialog and returns the selected filename. If no 
        /// selection was made, an empty string is returned
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string GetFileForOpen(string title, string filter)
        {
            return FileService.OpenFile(title, filter);
        }

        /// <summary>
        /// Invokes a save file dialog and returns the selected filename. If no 
        /// selection was made, an empty string is returned
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string GetFileForSave(string title, string filter)
        {
            return FileService.SaveFile(title, filter);
        }

        /// <summary>
        /// Displays a prompt for user input. If user cancels, null is returned.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetInput(string title)
        {
            return null;
        }

        /// <summary>
        /// Displays a question in the form of a yes/no response.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns>true for "Yes" response. false for a "No" response</returns>
        public static bool Ask(string title, string message)
        {
            return MessageService.AskQuestion(message, title);
        }

        /// <summary>
        /// Gets the workbench.
        /// </summary>
        /// <value>The workbench.</value>
        public static Workbench Workbench
        {
            get { return Workbench.Instance; }
        }
    }
}
