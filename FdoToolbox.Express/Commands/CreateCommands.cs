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
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;
using FdoToolbox.Express.Controls;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Commands
{
    public class CreateMySqlCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            var ctl = new CreateMySqlCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class CreatePostgresCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            var ctl = new CreatePostgresCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class CreateSqlServerCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            var ctl = new CreateSqlServerCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class CreateSdfCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            CreateSdfCtl ctl = new CreateSdfCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class CreateSqliteCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            CreateSqliteCtl ctl = new CreateSqliteCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class CreateShpCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            CreateShpCtl ctl = new CreateShpCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }
}
