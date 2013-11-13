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
using System.Collections;
using ICSharpCode.Core;

namespace FdoToolbox.Base
{
    ///// <summary>
    ///// Interface for classes that are able to open a file and create a <see cref="IViewContent"/> for it.
    ///// </summary>
    //public interface IDisplayBinding
    //{
    //    /// <summary>
    //    /// Loads the file and opens a <see cref="IViewContent"/>.
    //    /// When this method returns <c>null</c>, the display binding cannot handle the file type.
    //    /// </summary>
    //    IViewContent OpenFile(string fileName);
    //}

    //public static class DisplayBindingManager
    //{
    //    static ArrayList items;

    //    public static IViewContent CreateViewContent(string fileName)
    //    {
    //        if (items == null)
    //        {
    //            items = AddInTree.BuildItems("/Workspace/DisplayBindings", null, true);
    //        }
    //        foreach (IDisplayBinding binding in items)
    //        {
    //            IViewContent content = binding.OpenFile(fileName);
    //            if (content != null)
    //            {
    //                return content;
    //            }
    //        }
    //        return null;
    //    }
    //}
}
