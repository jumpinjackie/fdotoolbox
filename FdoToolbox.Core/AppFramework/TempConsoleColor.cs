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

namespace FdoToolbox.Core.AppFramework
{
    /// <summary>
    /// A helper class to set the text color of the console. All text written to the console 
    /// will be of the specified color until this object is disposed of.
    /// </summary>
    public class TempConsoleColor : IDisposable
    {
        private ConsoleColor _oldColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TempConsoleColor"/> class.
        /// </summary>
        /// <param name="c">The c.</param>
        public TempConsoleColor(ConsoleColor c)
        {
            _oldColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Console.ForegroundColor = _oldColor;
        }
    }
}
