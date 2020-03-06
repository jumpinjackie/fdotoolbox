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
using FdoToolbox.Base.Controls;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

using Msg = ICSharpCode.Core.MessageService;
using Res = ICSharpCode.Core.ResourceService;
using Log = ICSharpCode.Core.LoggingService;
using OSGeo.FDO.Schema;
using FdoToolbox.Base.Forms;
using System.Collections.Specialized;
using FdoToolbox.Core;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Xml;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Base.Commands
{
    internal class RunSqlCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            FdoConnection conn = mgr.GetConnection(connNode.Name);
            if (!conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSQL))
            {
                MessageService.ShowError(Res.GetString("ERR_UNSUPPORTED_SQL"));
            }
            else
            {
                var dlg = new FdoSqlCommandDialog(conn);
                dlg.ShowDialog();
            }
        }
    }

    internal class ExportDataStoreXmlCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.SaveFile(Res.GetString("TITLE_EXPORT_DATASTORE_XML"), Res.GetString("FILTER_XML_FILES"));
            if (!string.IsNullOrEmpty(path))
            {
                TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);

                using (new TempCursor(Cursors.WaitCursor))
                {
                    using (var svc = conn.CreateFeatureService())
                    {
                        var scs = new List<SpatialContextInfo>(svc.GetSpatialContexts()).ToArray();
                        var schemas = svc.DescribeSchema();
                        var mappings = svc.DescribeSchemaMapping(true);

                        var dstore = new FdoDataStoreConfiguration(schemas, scs, mappings);
                        dstore.Save(path);

                        Log.InfoFormatted("Connection saved to: {0}", path);
                    }
                }
            }
        }
    }

    internal class RemoveAllConnectionsCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            mgr.Clear();
        }
    }

    internal class RefreshConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
            {
                mgr.RefreshConnection(connNode.Name);
            }
        }
    }

    internal class RemoveConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            mgr.RemoveConnection(connNode.Name);
        }
    }

    internal class RenameConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();

            string name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), connNode.Name);
            if (name == null)
                return;

            while (name == string.Empty || mgr.NameExists(name))
            {
                name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), connNode.Name);
                if (name == null)
                    return;
            }

            mgr.RenameConnection(connNode.Name, name);
        }
    }

    internal class SaveConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.SaveFile(Res.GetString("TITLE_SAVE_CONNECTION"), Res.GetString("FILTER_CONNECTION_FILE"));
            if (!string.IsNullOrEmpty(path))
            {
                TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                conn.Save(path);
                Log.InfoFormatted("Connection saved to: {0}", path);
            }
        }
    }

    internal class LoadConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.OpenFile(Res.GetString("TITLE_LOAD_CONNECTION"), Res.GetString("FILTER_CONNECTION_FILE"));
            if (FileService.FileExists(path))
            {
                FdoConnection conn = FdoConnection.LoadFromFile(path);
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();

                string name = string.Empty;
                name = Msg.ShowInputBox(Res.GetString("TITLE_NEW_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), name);
                if (name == null)
                    return;

                while (name == string.Empty || mgr.NameExists(name))
                {
                    name = Msg.ShowInputBox(Res.GetString("TITLE_NEW_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), name);
                    if (name == null)
                        return;
                }

                using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
                {
                    mgr.AddConnection(name, conn);
                }
            }
        }
    }

    internal class SaveSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode node = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            if (node.Level == 2) //Schema
            {
                TreeNode schemaNode = node;
                TreeNode connNode = node.Parent;
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    FeatureSchema schema = service.GetSchemaByName(schemaNode.Name);
                    if (schema != null)
                    {
                        PartialSchemaSaveDialog dialog = new PartialSchemaSaveDialog(schema);
                        dialog.ShowDialog();
                    }
                }
            }


            //string path = FileService.SaveFile(Res.GetString("TITLE_SAVE_SCHEMA"), Res.GetString("FILTER_SCHEMA_FILE"));
            //if (!string.IsNullOrEmpty(path))
            //{
            //    TreeNode node = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            //    if (node.Level == 1) //Connection
            //    {
            //        TreeNode connNode = node;
            //        FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            //        FdoConnection conn = mgr.GetConnection(connNode.Name);
            //        using (FdoFeatureService service = conn.CreateFeatureService())
            //        {
            //            service.WriteSchemaToXml(path);
            //            Log.InfoFormatted(Res.GetString("LOG_SCHEMA_SAVED"), path);
            //        }
            //    }
            //    else if (node.Level == 2) //Schema
            //    {
            //        TreeNode schemaNode = node;
            //        TreeNode connNode = node.Parent;
            //        FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            //        FdoConnection conn = mgr.GetConnection(connNode.Name);
            //        using (FdoFeatureService service = conn.CreateFeatureService())
            //        {
            //            service.WriteSchemaToXml(schemaNode.Name, path);
            //            Log.InfoFormatted(Res.GetString("LOG_SCHEMA_SAVED_2"), connNode.Name, path);
            //        }
            //    }
            //}
        }
    }

    internal class DeleteSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode schemaNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            if (schemaNode.Level == 2 && MessageService.AskQuestion("Are you sure you want to delete this schema?"))
            {
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(schemaNode.Parent.Name);
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    try
                    {
                        service.DestroySchema(schemaNode.Name);
                        Msg.ShowMessage(Res.GetString("MSG_SCHEMA_DELETED"), Res.GetString("TITLE_DELETE_SCHEMA"));
                        Log.InfoFormatted(Res.GetString("LOG_SCHEMA_DELETED"), schemaNode.Name, schemaNode.Parent.Name);
                        mgr.RefreshConnection(schemaNode.Parent.Name);
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        Msg.ShowError(ex);
                    }
                }
            }
        }
    }

    internal class DataPreviewCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                TreeNode connNode = wb.ObjectExplorer.GetSelectedNode();
                while (connNode.Level > 1)
                    connNode = connNode.Parent;
                
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                FdoDataPreviewCtl ctl = null;
                if (node.Level > 1) //Class node
                    ctl = new FdoDataPreviewCtl(conn, connNode.Name, node.Parent.Name, node.Name);
                else
                    ctl = new FdoDataPreviewCtl(conn, connNode.Name);
                
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }

    internal class DataPreviewFirstThousandCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                TreeNode connNode = wb.ObjectExplorer.GetSelectedNode();
                while (connNode.Level > 1)
                    connNode = connNode.Parent;

                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                FdoDataPreviewCtl ctl = null;
                if (node.Level > 1) //Class node
                    ctl = new FdoDataPreviewCtl(conn, connNode.Name, node.Parent.Name, node.Name);
                else
                    ctl = new FdoDataPreviewCtl(conn, connNode.Name);

                ctl.SetInitialPreviewLimit(1000);

                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }

    internal class ManageDataStoresCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                if (node.Level == 1)
                {
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(node.Name);

                    FdoDataStoreMgrCtl ctl = new FdoDataStoreMgrCtl(conn);
                    
                    wb.ShowContent(ctl, ViewRegion.Document);
                }
            }
        }
    }

    internal class ConfigureConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            /*
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 1)
            {
                string name = node.Name;
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(name);

                if (conn != null)
                {

                }
            }*/
            MessageService.ShowError("Not Implemented");
        }
    }

    internal class InsertFeatureCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 3)
            {
                string name = node.Name;
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(node.Parent.Parent.Name);
                FdoInsertScaffold ctl = new FdoInsertScaffold(conn, name);
                wb.ShowContent(ctl, ViewRegion.Dialog);
            }
        }
    }

    internal class BulkDeleteCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 3)
            {
                if (MessageService.AskQuestion("This is a dangerous operation. One false filter could cause irreversible data loss. Do you want to continue?"))
                {
                    string name = node.Name;
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(node.Parent.Parent.Name);

                    FdoBulkDeleteCtl ctl = new FdoBulkDeleteCtl(conn, node.Name);
                    wb.ShowContent(ctl, ViewRegion.Dialog);
                }
            }
        }
    }

    internal class BulkUpdateCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 3)
            {
                if (MessageService.AskQuestion("This is a dangerous operation. One false filter could cause irreversible data changes. Do you want to continue?"))
                {
                    string name = node.Name;
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(node.Parent.Parent.Name);

                    FdoBulkUpdateCtl ctl = new FdoBulkUpdateCtl(conn, node.Name);
                    wb.ShowContent(ctl, ViewRegion.Dialog);
                }
            }
        }
    }

    internal class ViewCapabilitiesCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 1)
            {
                FdoCapabilityViewer viewer = new FdoCapabilityViewer(node.Name);
                wb.ShowContent(viewer, ViewRegion.Document);
            }
        }
    }
}
