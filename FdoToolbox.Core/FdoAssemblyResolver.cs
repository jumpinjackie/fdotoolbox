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
using System.Reflection;
using System.IO;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Helper class to locate FDO assemblies
    /// </summary>
    public sealed class FdoAssemblyResolver
    {
        static string[] assemblies = { "OSGeo.FDO.dll", "OSGeo.FDO.Common.dll", "OSGeo.FDO.Geometry.dll" };

        /// <summary>
        /// Checks if the given path is a valid path containing FDO assemblies
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidFdoPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            foreach (string asm in assemblies)
            {
                string asmPath = Path.Combine(path, asm);
                if (!File.Exists(asmPath))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the path where FDO assemblies will be loaded.
        /// </summary>
        /// <param name="path"></param>
        public static void InitializeFdo(string path)
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

            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
            {
                string fdoPath = path;

                //Retrieve the list of referenced assemblies in an array of AssemblyName.
                Assembly MyAssembly, objExecutingAssemblies;
                string strTempAssmbPath = "";

                objExecutingAssemblies = Assembly.GetExecutingAssembly();
                AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

                //Loop through the array of referenced assembly names.
                foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
                {
                    //Check for the assembly names that have raised the "AssemblyResolve" event.
                    if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
                    {
                        //Build the path of the assembly from where it has to be loaded.				
                        strTempAssmbPath = Path.Combine(fdoPath, args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll");
                        break;
                    }

                }
                //Load the assembly from the specified path. 					
                MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

                //Return the loaded assembly.
                return MyAssembly;
            };
        }
    }
}
