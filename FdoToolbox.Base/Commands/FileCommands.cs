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

using ICSharpCode.Core;
using FdoToolbox.Base.Controls;
using System.Diagnostics;

namespace FdoToolbox.Base.Commands
{
    internal class ExitCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                wb.Close();
            }
        }
    }

    internal class OpenFdoDirectoryCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Process.Start(Preferences.FdoPath);
        }
    }

    internal class OpenLogsDirectoryCommand : AbstractMenuCommand
    {
        public override void Run()
        {
			if (System.IO.Directory.Exists(Preferences.LogPath))
				Process.Start(Preferences.LogPath);
			else
			{
				// TODO: put this as WrappedMessageBox with system string
				System.Windows.Forms.MessageBox.Show("Log Path Does Not Exist!");
			}
        }
    }

    internal class OpenSessionDirectoryCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Process.Start(Preferences.SessionDirectory);
        }
    }

    internal class ConnectCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoConnectCtl ctl = new FdoConnectCtl();
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }

    internal class CreateDataStoreCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoCreateDataStoreCtl ctl = new FdoCreateDataStoreCtl();
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }
}
