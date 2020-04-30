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
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;

//Type aliases to save typing
using Res = ICSharpCode.Core.ResourceService;
using Msg = ICSharpCode.Core.MessageService;
using FdoToolbox.Express.Controls;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Commands
{
    public class ConnectSdfCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string file = FileService.OpenFile(Res.GetString("TITLE_CONNECT_SDF"), Res.GetString("FILTER_SDF"));
            if (FileService.FileExists(file))
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SDF", file);
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                NamingService namer = ServiceManager.Instance.GetService<NamingService>();

                string name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), namer.GetDefaultConnectionName("OSGeo.SDF"));
                if (name == null)
                    return;
                
                while(name == string.Empty || mgr.NameExists(name))
                {
                    Msg.ShowError(Res.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                    name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), name);

                    if (name == null)
                        return;
                }
                mgr.AddConnection(name, conn);
            }
        }
    }

    public class ConnectSqliteCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string file = FileService.OpenFile(Res.GetString("TITLE_CONNECT_SQLITE"), Res.GetString("FILTER_SQLITE"));
            if (FileService.FileExists(file))
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SQLite", file);
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                NamingService namer = ServiceManager.Instance.GetService<NamingService>();

                string name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), namer.GetDefaultConnectionName("OSGeo.SQLite"));
                if (name == null)
                    return;

                while (name == string.Empty || mgr.NameExists(name))
                {
                    Msg.ShowError(Res.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                    name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), name);

                    if (name == null)
                        return;
                }
                mgr.AddConnection(name, conn);
            }
        }
    }

    public class ConnectShpCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string file = FileService.OpenFile(Res.GetString("TITLE_CONNECT_SHP"), Res.GetString("FILTER_SHP"));
            if (FileService.FileExists(file))
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SHP", file);
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                NamingService namer = ServiceManager.Instance.GetService<NamingService>();

                string name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), namer.GetDefaultConnectionName("OSGeo.SHP"));
                if (name == null)
                    return;

                while (string.IsNullOrEmpty(name) || mgr.NameExists(name))
                {
                    Msg.ShowError(Res.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                    name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), name);

                    if (name == null)
                        return;
                }
                mgr.AddConnection(name, conn);
            }
        }
    }

    public class ConnectShpDirCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string dir = FileService.GetDirectory(Res.GetString("TITLE_CONNECT_SHP_DIR"));
            if (FileService.DirectoryExists(dir))
            {
                FdoConnection conn = new FdoConnection("OSGeo.SHP", "DefaultFileLocation=" + dir);
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                NamingService namer = ServiceManager.Instance.GetService<NamingService>();

                string name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), namer.GetDefaultConnectionName("OSGeo.SHP"));
                if (name == null)
                    return;

                while (string.IsNullOrEmpty(name) || mgr.NameExists(name))
                {
                    Msg.ShowError(Res.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                    name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), name);

                    if (name == null)
                        return;
                }
                mgr.AddConnection(name, conn);
            }
        }
    }

    public class ConnectKingOracleCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ConnectKingOracleCtl ctl = new ConnectKingOracleCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectOdbcCommand : AbstractMenuCommand 
    {
        public override void Run()
        {
            ConnectOdbcCtl ctl = new ConnectOdbcCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectOgrCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ConnectOgrCtl ctl = new ConnectOgrCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectArcSdeCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ConnectArcSdeCtl ctl = new ConnectArcSdeCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectMySqlCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ConnectMySqlCtl ctl = new ConnectMySqlCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectPostGisCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ConnectPostGisCtl ctl = new ConnectPostGisCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectPostgresCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ConnectPostgresCtl ctl = new ConnectPostgresCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectSqlServerCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ConnectSqlServerCtl ctl = new ConnectSqlServerCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectAdskSqlServerCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var ctl = new ConnectAdskSqlServerCtl();
            var wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }

    public class ConnectAdskOracleCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var ctl = new ConnectAdskOracleCtl();
            var wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }
}
