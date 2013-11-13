#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using FdoToolbox.Base;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.DataStoreManager.Controls;

namespace FdoToolbox.DataStoreManager.Commands
{
    internal class EditDataStoreCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 1)
            {
                string name = node.Name;
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(name);

                if (conn != null)
                {
                    var ctl = new FdoDataStoreCtrl(conn);
                    ctl.DataStoreChanged += delegate
                    {
                        mgr.RefreshConnection(name);
                    };
                    wb.ShowContent(ctl, ViewRegion.Document);
                }
            }
        }
    }
}
