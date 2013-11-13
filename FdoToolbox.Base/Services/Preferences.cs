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
using ICSharpCode.Core;
using System.IO;

namespace FdoToolbox.Base
{
    /// <summary>
    /// A strongly-typed preferences class
    /// </summary>
    public static class Preferences
    {
        /// <summary>
        /// Group Name
        /// </summary>
        public static readonly string GroupName = "Base.AddIn.Options";



        internal static readonly string PREF_FDO_PATH = "FdoPath";
        internal static readonly string PREF_WORKING_DIR = "WorkingDirectory";
        internal static readonly string PREF_WARN_DATASET = "DataPreviewWarnLimit";
        internal static readonly string PREF_SESSION_DIR = "SessionDirectory";
        internal static readonly string PREF_LOG_PATH = "LogPath";
        internal static readonly string PREF_DATA_PREVIEW_RANDOM_COLORS = "DataPreviewRandomColors";
        internal static readonly string PREF_SCRIPT_MODULE_PATHS = "ScriptModulePaths";
        internal static readonly string PREF_SCRIPT_DEBUG = "ScriptDebug";

        static Properties properties;

        static Preferences()
        {
            properties = PropertyService.Get(GroupName, new Properties());
        }

        static Properties Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public static event PropertyChangedEventHandler PropertyChanged
        {
            add { properties.PropertyChanged += value; }
            remove { properties.PropertyChanged -= value; }
        }

        /// <summary>
        /// The path to the FDO assemblies
        /// </summary>
        public static string FdoPath
        {
            get { return properties.Get<string>(PREF_FDO_PATH, Path.Combine(FileUtility.ApplicationRootPath, "FDO")); }
            set { properties.Set(PREF_FDO_PATH, value); }
        }

        /// <summary>
        /// The working directory
        /// </summary>
        public static string WorkingDirectory
        {
            get { return properties.Get<string>(PREF_WORKING_DIR, FileUtility.ApplicationRootPath); }
            set { properties.Set(PREF_WORKING_DIR, value); }
        }

        /// <summary>
        /// The path where logs will be written
        /// </summary>
        public static string LogPath
        {
            get { return properties.Get<string>(PREF_LOG_PATH, Path.Combine(PropertyService.ConfigDirectory, "Logs")); }
            set { properties.Set(PREF_LOG_PATH, value); }
        }

        /// <summary>
        /// The path where the session data will be persisted to and loaded from
        /// </summary>
        public static string SessionDirectory
        {
            get { return properties.Get<string>(PREF_SESSION_DIR, Path.Combine(PropertyService.ConfigDirectory, "Session")); }
            set { properties.Set(PREF_SESSION_DIR, value); }
        }

        /// <summary>
        /// Determines the limit at which the Data Preview will warn you about large result sets. 
        /// </summary>
        public static int DataPreviewWarningLimit
        {
            get { return properties.Get<int>(PREF_WARN_DATASET, 1500); }
            set { properties.Set<int>(PREF_WARN_DATASET, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use a randomly generated color theme for map previews.
        /// 
        /// If false, map preview will use a monochromatic theme.
        /// </summary>
        public static bool DataPreviewRandomColors
        {
            get
            {
                return properties.Get<bool>(PREF_DATA_PREVIEW_RANDOM_COLORS, true);
            }
            set
            {
                properties.Set<bool>(PREF_DATA_PREVIEW_RANDOM_COLORS, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable debugging in the script engine
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public static bool ScriptDebug
        {
            get
            {
                return properties.Get<bool>(PREF_SCRIPT_DEBUG, false);
            }
            set
            {
                properties.Set<bool>(PREF_SCRIPT_DEBUG, value);
            }
        }

        /// <summary>
        /// Gets or sets the list of script engine module paths.
        /// </summary>
        /// <value>The list of script engine module paths.</value>
        public static string[] ScriptModulePaths
        {
            get
            {
                return properties.Get<string>(PREF_SCRIPT_MODULE_PATHS, string.Empty).Split(';');
            }
            set
            {
                properties.Set<string>(PREF_SCRIPT_MODULE_PATHS, string.Join(";", value));
            }
        }
    }
}
