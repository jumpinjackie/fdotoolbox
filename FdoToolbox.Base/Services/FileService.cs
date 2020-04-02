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
using System.Windows.Forms;
using ICSharpCode.Core;
using System.IO;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// Provides file/directory browsing and selection services
    /// </summary>
    public sealed class FileService
    {
        static OpenFileDialog _openDialog = new OpenFileDialog();
        static SaveFileDialog _saveDialog = new SaveFileDialog();
        static FolderBrowserDialog _folderDialog = new FolderBrowserDialog();

        static FileService()
        {
            string path = Preferences.WorkingDirectory;
            _openDialog.InitialDirectory = path;
            _saveDialog.InitialDirectory = path;
            _folderDialog.SelectedPath = path; 
            _folderDialog.ShowNewFolderButton = true;
        }

        /// <summary>
        /// Opens the file.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string OpenFile(string title, string filter)
        {
            _openDialog.FileName = string.Empty;
            _openDialog.Title = title;
            _openDialog.Multiselect = false;
            _openDialog.Filter = filter;
            if (_openDialog.ShowDialog() == DialogResult.OK)
            {
                return _openDialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Opens the files.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string[] OpenFiles(string title, string filter)
        {
            _openDialog.Title = title;
            _openDialog.Multiselect = true;
            _openDialog.Filter = filter;
            if (_openDialog.ShowDialog() == DialogResult.OK)
            {
                return _openDialog.FileNames;
            }
            return new string[0];
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string SaveFile(string title, string filter)
        {
            _openDialog.FileName = string.Empty;
            _saveDialog.Title = title;
            _saveDialog.Filter = filter;
            if (!string.IsNullOrEmpty(_saveDialog.FileName))
            {
                try
                {
                    var dir = Path.GetDirectoryName(_saveDialog.FileName);
                    if (Directory.Exists(dir))
                        _saveDialog.InitialDirectory = dir;
                }
                catch { }
            }
            if (_saveDialog.ShowDialog() == DialogResult.OK)
            {
                return _saveDialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public static string GetDirectory(string title)
        {
            _folderDialog.Description = title;
            if (_folderDialog.ShowDialog() == DialogResult.OK)
            {
                return _folderDialog.SelectedPath;
            }
            return string.Empty;
        }

        /// <summary>
        /// Determines if the specified file exists
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static bool FileExists(string file)
        {
            return !string.IsNullOrEmpty(file) && File.Exists(file);
        }

        /// <summary>
        /// Determines if the specified directory exists
        /// </summary>
        /// <param name="dir">The directory.</param>
        /// <returns></returns>
        public static bool DirectoryExists(string dir)
        {
            return !string.IsNullOrEmpty(dir) && Directory.Exists(dir);
        }
    }
}
