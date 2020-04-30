#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// https://github.com/jumpinjackie/fdotoolbox-addins, jumpinjackie@gmail.com
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
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// This is a helper class that aids in resolving assemblies that are outside
    /// of the application's directory or the Global Assembly Cache.
    /// </summary>
    /// <remarks>
    /// This class is designed to be used from within an add-in context. Do not 
    /// use <see cref="FdoToolbox.Core.FdoAssemblyResolver"/> from within an add-in context. Only 
    /// use <see cref="FdoToolbox.Core.FdoAssemblyResolver"/> when using the Core API from a standalone 
    /// context (outside of FDO Toolbox)
    /// </remarks>
    internal sealed class AddInAssemblyResolver
    {
        static AddInAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(OnAssemblyResolve);
        }

        static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string asmName = args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
            if (assemblyPaths.ContainsKey(asmName))
            {
                return Assembly.LoadFrom(Path.Combine(assemblyPaths[asmName], asmName));
            }
            return null;
        }

        static Dictionary<string, string> assemblyPaths = new Dictionary<string, string>();

        /// <summary>
        /// Registers the libraries. Use this method if your add-in references additional
        /// libraries that are not part of the FDO Toolbox installation.
        /// 
        /// Failure to do this will result in FileLoadExceptions when attempting to load
        /// a referenced assembly that is not part of the FDO Toolbox installation.
        /// </summary>
        /// <param name="path">The path where the additional assemblies are located</param>
        /// <param name="assemblies">The assemblies to register</param>
        public static void RegisterLibraries(string path, params string[] assemblies)
        {
            //As of FDO 3.4 final, it somehow fails when the unmanaged FDO dlls are not the application path
            //
            //This prepends the given path to the PATH enviornment variable in the scope of this
            //process so the system knows where to look for the unmanaged FDO dlls. Because our FDO path
            //is at the beginning, this should avoid any conflicts if there are any other FDO paths in the
            //environment variable since our FDO path will be loaded first.
            string envPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
            if (!envPath.Contains(path))
            {
                envPath = path + ";" + envPath;
                Environment.SetEnvironmentVariable("PATH", envPath);
            }
            
            foreach (string asm in assemblies)
            {
                if(!assemblyPaths.ContainsKey(asm))
                    assemblyPaths.Add(asm, path);
            }
        }
    }
}
