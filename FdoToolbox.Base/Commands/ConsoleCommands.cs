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
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Commands
{
    internal class SaveConsoleLogCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_LOG"), ResourceService.GetString("FILTER_LOG_FILE"));
                if (FileService.FileExists(file))
                {
                    System.IO.File.WriteAllText(file, wb.Console.TextContent);
                    MessageService.ShowMessage("Log saved to " + file, "Log saved");
                }
            }
        }
    }

    internal class ClearConsoleCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                wb.Console.Clear();
            }
        }
    }
}
