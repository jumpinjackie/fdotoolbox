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
using CommandLine;
using FdoCmd.Commands;
using FdoToolbox.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FdoCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "FDO");
            FdoAssemblyResolver.InitializeFdo(path);

            var commandTypes = Assembly.GetExecutingAssembly()
                                       .GetTypes()
                                       .Where(t => typeof(BaseCommand).IsAssignableFrom(t) && t.GetCustomAttribute<VerbAttribute>() != null && !t.IsAbstract)
                                       .ToArray();

            Parser.Default
                .ParseArguments(args, commandTypes)
                .WithParsed(opts =>
                {
                    Environment.ExitCode = ((BaseCommand)opts).Execute();
                });
        }
    }
}
