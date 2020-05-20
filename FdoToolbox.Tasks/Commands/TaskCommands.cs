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
using ICSharpCode.Core;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using FdoToolbox.Base;
using FdoToolbox.Core.ETL;
using System.Windows.Forms;

using Res = ICSharpCode.Core.ResourceService;
using Msg = ICSharpCode.Core.MessageService;
using FdoToolbox.Tasks.Controls;
using FdoToolbox.Base.Controls;
using FdoToolbox.Core.ETL.Specialized;
using System.IO;
using System.ComponentModel;
using FdoToolbox.Core.Configuration;

namespace FdoToolbox.Tasks.Commands
{
    public class ExecuteTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();
            string name = Workbench.Instance.ObjectExplorer.GetSelectedNode().Name;
            EtlProcess proc = mgr.GetTask(name);
            FdoBulkCopy bcp = proc as FdoBulkCopy;
            CancelEventHandler ch = new CancelEventHandler(OnBeforeExecuteBulkCopy);
            if (bcp != null)
            {   
                bcp.BeforeExecute += ch;
            }

            if (proc != null)
            {
                IFdoSpecializedEtlProcess spec = proc as IFdoSpecializedEtlProcess;
                if (spec != null)
                {
                    EtlProcessCtl ctl = new EtlProcessCtl(spec);
                    Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
                    if (bcp != null)
                    {
                        bcp.BeforeExecute -= ch;
                    }
                }
                else
                {
                    MessageService.ShowError(ResourceService.GetString("ERR_CANNOT_EXECUTE_UNSPECIALIZED_ETL_PROCESS"));
                }
            }
        }

        private void OnBeforeExecuteBulkCopy(object sender, CancelEventArgs e)
        {

            //    Workbench wb = Workbench.Instance;
            //    MethodInvoker invoker = new MethodInvoker(delegate
            //    {
            //        TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();
            //        string name = wb.ObjectExplorer.GetSelectedNode().Name;
            //        EtlProcess proc = mgr.GetTask(name);
            //        FdoBulkCopy bcp = proc as FdoBulkCopy;
            //        if (bcp != null)
            //        {
            //            FdoBulkCopyOptions opts = bcp.Options;
            //            using (FdoFeatureService srcService = FdoConnectionUtil.CreateFeatureService(opts.SourceConnection))
            //            using (FdoFeatureService destService = FdoConnectionUtil.CreateFeatureService(opts.TargetConnection))
            //            {
            //                FeatureSchema srcSchema = srcService.GetSchemaByName(opts.SourceSchema);
            //                if (opts.ApplySchemaToTarget && srcSchema != null)
            //                {
            //                    IncompatibleSchema incSchema = null;
            //                    if (!destService.CanApplySchema(srcSchema, out incSchema))
            //                    {
            //                        bool attemptAlter = WrappedMessageBox.Confirm("Incompatible Schema", "The source schema has incompatible elements:\n\n" + incSchema.ToString() + "\nThe source schema will be altered to be compatible with the target connection. Proceed?", MessageBoxText.YesNo);
            //                        if (attemptAlter)
            //                        {
            //                            try
            //                            {
            //                                FeatureSchema alteredSchema = destService.AlterSchema(srcSchema, incSchema);
            //                                if (alteredSchema != null)
            //                                {
            //                                    opts.AlterSchema = true;
            //                                }
            //                            }
            //                            catch (Exception ex)
            //                            {
            //                                MessageService.ShowError(ex);
            //                                LoggingService.Error("Alter schema error", ex);
            //                                e.Cancel = true;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            e.Cancel = true;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    });
            //    wb.Invoke(invoker);
        }
    }

    public class RenameTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode taskName = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();

            string name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_TASK"), Res.GetString("PROMPT_ENTER_NEW_TASK_NAME"), taskName.Name);
            if (name == null)
                return;

            while (name == string.Empty || mgr.NameExists(name))
            {
                name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_TASK"), Res.GetString("PROMPT_ENTER_NEW_TASK_NAME"), taskName.Name);
                if (name == null)
                    return;
            }

            mgr.RenameTask(taskName.Name, name);
        }
    }

    public class DeleteTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode taskNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();

            mgr.RemoveTask(taskNode.Name);
        }
    }

    public class LoadTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();
            TaskLoader ldr = new TaskLoader();
            string file = FileService.OpenFile(ResourceService.GetString("TITLE_LOAD_TASK"), ResourceService.GetString("FILTER_TASK_DEFINITION"));
            if (FileService.FileExists(file))
            {
                using (new TempCursor(Cursors.WaitCursor))
                {
                    LoggingService.Info(ResourceService.GetString("LOADING_TASK_DEFINITION_WAIT"));
                    if (TaskDefinitionHelper.IsBulkCopy(file))
                    {
                        string name = string.Empty;
                        FdoBulkCopyOptions opt = ldr.BulkCopyFromXml(file, ref name, false);
                        FdoBulkCopy cpy = new FdoBulkCopy(opt);
                        mgr.AddTask(name, cpy);
                    }
                    else if (TaskDefinitionHelper.IsJoin(file))
                    {
                        string name = string.Empty;
                        FdoJoinOptions opt = ldr.JoinFromXml(file, ref name, false);
                        FdoJoin join = new FdoJoin(opt);
                        mgr.AddTask(name, join);
                    }
                }
            }
        }
    }

    public class EditTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode taskNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();
            //EtlProcess proc = mgr.GetTask(taskNode.Name);
            FdoSpecializedEtlProcess proc = mgr.GetTask(taskNode.Name) as FdoSpecializedEtlProcess;
            if (proc != null)
            {
                if (proc is FdoBulkCopy)
                {
                    FdoBulkCopy copy = proc as FdoBulkCopy;
                    FdoBulkCopyCtl ctl = new FdoBulkCopyCtl(taskNode.Name, copy);
                    Workbench.Instance.ShowContent(ctl, ViewRegion.Document);
                }
                else if (proc is FdoJoin)
                {
                    FdoJoin join = proc as FdoJoin;
                    FdoJoinCtl ctl = new FdoJoinCtl(taskNode.Name, join);
                    Workbench.Instance.ShowContent(ctl, ViewRegion.Document);
                }
                else
                {
                    MessageService.ShowError(ResourceService.GetString("ERR_NO_EDITOR_FOR_TASK"));
                }
            }
        }
    }

    public class SaveTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode taskNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();
            //EtlProcess proc = mgr.GetTask(taskNode.Name);
            FdoSpecializedEtlProcess proc = mgr.GetTask(taskNode.Name) as FdoSpecializedEtlProcess;
            if (proc != null && proc.CanSave)
            {
                try
                {
                    string filter = string.Format("{0} (*{1})|*{1}", proc.GetDescription(), proc.GetFileExtension());
                    string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_TASK"), filter);
                    if (!string.IsNullOrEmpty(file))
                    {
                        proc.Save(file, taskNode.Name);
                        MessageService.ShowMessage(ResourceService.GetStringFormatted("MSG_TASK_SAVED", file));
                    }
                }
                catch (Exception ex)
                {
                    MessageService.ShowError(ex.Message);
                }
            }
        }
    }

    public class RemoveAllTasksCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TaskManager tm = ServiceManager.Instance.GetService<TaskManager>();
            tm.Clear();
        }
    }
}
