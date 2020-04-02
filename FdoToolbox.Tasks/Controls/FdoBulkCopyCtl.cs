using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.Tasks.Controls.BulkCopy;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base;
using OSGeo.FDO.Schema;
using System.Diagnostics;
using FdoToolbox.Core.ETL.Specialized;
using ICSharpCode.Core;
using FdoToolbox.Base.Forms;
using FdoToolbox.Core.Configuration;
using System.Collections.Specialized;
using FdoToolbox.Tasks.Services;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoBulkCopyCtl : ViewContent
    {
        private FdoConnectionManager _connMgr;

        public FdoBulkCopyCtl()
        {
            InitializeComponent();
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
        }

        public FdoBulkCopyCtl(string name, FdoBulkCopy task)
            : this()
        {
            Load(task.Options, name);
            txtName.ReadOnly = true;
            btnSave.Enabled = true;
        }

        public override string Title
        {
            get
            {
                return ResourceService.GetString("TITLE_BULK_COPY_SETTINGS") + ": " + txtName.Text; 
            }
        }

        protected override void OnConnectionRenamed(object sender, ConnectionRenameEventArgs e)
        {
            int idx = GetConnectionIndex(e.OldName);
            if (idx >= 0)
            {
                grdConnections.Rows[idx].Cells[0].Value = e.NewName;
                UpdateConnectionReferences(e.OldName, e.NewName);
            }
        }

        private void UpdateConnectionReferences(string oldName, string newName)
        {
            foreach (var task in _tasks.Values)
            {
                if (task.SourceConnectionName.Equals(oldName))
                    task.SourceConnectionName = newName;

                if (task.TargetConnectionName.Equals(oldName))
                    task.TargetConnectionName = newName;
            }
        }

        protected override void OnBeforeConnectionRemove(object sender, ConnectionBeforeRemoveEventArgs e)
        {
            if (ConnectionAdded(e.ConnectionName))
            {
                //TODO: Prompt to remove the referenced connection and any copy tasks that use
                //this connection
                MessageService.ShowMessage("This bulk copy task depends on the current connection");
                e.Cancel = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            PopulateConnections();
            base.OnLoad(e);
        }

        private void PopulateConnections()
        {
            foreach (string name in _connMgr.GetConnectionNames())
            {
                string connName = name; //Can't bind to iter variable
                var item = btnAddConnection.DropDown.Items.Add(connName, null, delegate(object sender, EventArgs e)
                {
                    if (!ConnectionAdded(connName))
                        AddParticipatingConnection(connName);
                    else
                        MessageService.ShowMessage("Connection " + connName + " already added");
                });
                item.Name = name;
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            foreach (string name in _connMgr.GetConnectionNames())
            {
                if (!ConnectionAdded(name))
                    AddParticipatingConnection(name);
            }
        }

        private bool ConnectionAdded(string name)
        {
            return GetConnectionIndex(name) >= 0;
        }

        private int GetConnectionIndex(string name)
        {
            foreach (DataGridViewRow row in grdConnections.Rows)
            {
                if (name.Equals(row.Cells[0].Value))
                    return row.Index;
            }
            return -1;
        }

        protected override void OnConnectionAdded(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            string name = e.Data;
            btnAddConnection.DropDown.Items.Add(name, null, delegate(object s, EventArgs evt)
            {
                AddParticipatingConnection(name);
            });
        }

        protected override void OnConnectionRemoved(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            string name = e.Data;
            btnAddConnection.DropDown.Items.RemoveByKey(name);
        }

        private void AddParticipatingConnection(string name)
        {
            FdoConnection conn = _connMgr.GetConnection(name);
            if (conn != null)
            {
                grdConnections.Rows.Add(name, conn.Provider, conn.SafeConnectionString, conn.ConnectionString);
            }
        }

        private CopyTaskNodeDecorator AddNewTask(TreeNode root, string srcConnName, string srcSchema, string srcClass, string dstConnName, string dstSchema, string dstClass, string taskName, bool createIfNotExists)
        {
            return new CopyTaskNodeDecorator(root, srcConnName, srcSchema, srcClass, dstConnName, dstSchema, dstClass, taskName, createIfNotExists);
        }

        private string[] GetAvailableConnectionNames()
        {
            List<string> values = new List<string>();
            foreach (DataGridViewRow row in grdConnections.Rows)
            {
                values.Add(row.Cells[0].Value.ToString());
            }
            return values.ToArray();
        }

        private SortedList<int, CopyTaskNodeDecorator> _tasks = new SortedList<int, CopyTaskNodeDecorator>();

        private void btnAddMultipleTasks_Click(object sender, EventArgs e)
        {
            if (GetAvailableConnectionNames().Length == 0)
            {
                MessageService.ShowMessage("Add some participating connections first", "No connections");
                return;
            }
            var dlg = new NewMultiTaskDialog(GetAvailableConnectionNames());
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TreeNode root = mTreeView.Nodes[0];
                foreach (var cdef in dlg.GetCopyTasks())
                {
                    CopyTaskNodeDecorator task = AddNewTask(
                                               root,
                                               cdef.SourceConnectionName,
                                               cdef.SourceSchema,
                                               cdef.SourceClass,
                                               cdef.TargetConnectionName,
                                               cdef.TargetSchema,
                                               cdef.TargetClass,
                                               cdef.TaskName,
                                               cdef.CreateIfNotExist);

                    if (cdef.CreateIfNotExist)
                        task.PropertyMappings.OnAutoMap(this, EventArgs.Empty);

                    _tasks[task.DecoratedNode.Index] = task;
                }

                
                root.Expand();

                btnSave.Enabled = (root.Nodes.Count > 0);
            }
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            if (GetAvailableConnectionNames().Length == 0)
            {
                MessageService.ShowMessage("Add some participating connections first", "No connections");
                return;
            }

            var dlg = new NewTaskDialog(GetAvailableConnectionNames());
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TreeNode root = mTreeView.Nodes[0];
                CopyTaskNodeDecorator task = AddNewTask(
                                               root,
                                               dlg.SourceConnectionName,
                                               dlg.SourceSchema,
                                               dlg.SourceClass,
                                               dlg.TargetConnectionName,
                                               dlg.TargetSchema,
                                               dlg.TargetClass,
                                               dlg.TaskName,
                                               dlg.CreateIfNotExist);

                if (dlg.CreateIfNotExist)
                    task.PropertyMappings.OnAutoMap(this, EventArgs.Empty);

                _tasks[task.DecoratedNode.Index] = task;
                root.Expand();

                btnSave.Enabled = (root.Nodes.Count > 0);
            }
        }

        const int NODE_LEVEL_TASK = 1;

        private void mTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Level)
            {
                case NODE_LEVEL_TASK:
                    btnRemoveTask.Enabled = true;
                    break;
                default:
                    btnRemoveTask.Enabled = false;
                    break;
            }
        }

        private void btnRemoveTask_Click(object sender, EventArgs e)
        {
            TreeNode node = mTreeView.SelectedNode;
            if (node.Level == NODE_LEVEL_TASK)
            {
                mTreeView.Nodes.Remove(node);
                _tasks.Remove(node.Index);
            }
        }

        //Right-click TreeView hack
        private void mTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mTreeView.SelectedNode = mTreeView.GetNodeAt(e.X, e.Y);
            }
        }

        public bool IsNew
        {
            get { return !txtName.ReadOnly; }
            set { txtName.ReadOnly = !value; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mTreeView.Nodes[0].Nodes.Count == 0)
            {
                MessageService.ShowMessage("Please define at least one copy task");
                return;
            }

            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageService.ShowMessage("Please specify a name for this task");
                return;
            }

            TaskManager tmgr = ServiceManager.Instance.GetService<TaskManager>();

            if (IsNew && tmgr.GetTask(txtName.Text) != null)
            {
                MessageService.ShowMessage("A task named " + txtName.Text + " already exists. Please specify a name for this task");
                return;
            }

            //This could take a while...
            using (new TempCursor(Cursors.WaitCursor))
            {
                LoggingService.Info("Updating loaded task. Please wait.");
                TaskLoader loader = new TaskLoader();
                if (IsNew)
                {
                    string name = string.Empty;
                    FdoBulkCopyOptions opts = loader.BulkCopyFromXml(Save(), ref name, false);
                    FdoBulkCopy bcp = new FdoBulkCopy(opts);
                    tmgr.AddTask(name, bcp);
                    this.Close();
                }
                else
                {
                    FdoBulkCopy bcp = tmgr.GetTask(txtName.Text) as FdoBulkCopy;
                    if (bcp == null)
                    {
                        MessageService.ShowMessage("This named task is not a bulk copy task or could not find the named task to update");
                        return;
                    }
                    string name = string.Empty;
                    FdoBulkCopyOptions opts = loader.BulkCopyFromXml(Save(), ref name, false);
                    Debug.Assert(name == txtName.Text); //unchanged

                    //Update options
                    bcp.Options = opts;
                    MessageService.ShowMessage("Task updated. To save to disk, right-click the task object and choose: " + ResourceService.GetString("CMD_SaveTask"));
                    this.Close();
                }
            }
        }

        private FdoBulkCopyTaskDefinition Save()
        {
            FdoBulkCopyTaskDefinition def = new FdoBulkCopyTaskDefinition();
            def.name = txtName.Text;
            List<FdoConnectionEntryElement> conns = new List<FdoConnectionEntryElement>();
            foreach (DataGridViewRow row in grdConnections.Rows)
            {
                FdoConnectionEntryElement entry = new FdoConnectionEntryElement();
                entry.name = row.Cells[0].Value.ToString();
                entry.provider = row.Cells[1].Value.ToString();
                entry.ConnectionString = row.Cells[3].Value.ToString();
                conns.Add(entry);
            }
            List<FdoCopyTaskElement> tasks = new List<FdoCopyTaskElement>();
            foreach (CopyTaskNodeDecorator dec in _tasks.Values)
            {
                FdoCopyTaskElement task = new FdoCopyTaskElement();
                task.name = dec.Name;
                task.createIfNotExists = dec.CreateIfNotExists;

                task.Source = new FdoCopySourceElement();
                task.Target = new FdoCopyTargetElement();
                task.Options = new FdoCopyOptionsElement();
                List<FdoPropertyMappingElement> pmaps = new List<FdoPropertyMappingElement>();
                List<FdoExpressionMappingElement> emaps = new List<FdoExpressionMappingElement>();
                
                //Source
                task.Source.@class = dec.SourceClassName;
                task.Source.connection = dec.SourceConnectionName;
                task.Source.schema = dec.SourceSchemaName;

                //Target
                task.Target.@class = dec.TargetClassName;
                task.Target.connection = dec.TargetConnectionName;
                task.Target.schema = dec.TargetSchemaName;

                //Options
                task.Options.BatchSize = dec.Options.BatchSize.ToString();
                task.Options.FlattenGeometries = dec.Options.Flatten;
                task.Options.FlattenGeometriesSpecified = true;
                task.Options.DeleteTarget = dec.Options.Delete;
                task.Options.Filter = dec.Options.SourceFilter;
                task.Options.ForceWKB = dec.Options.ForceWkb;
                task.Options.ForceWKBSpecified = true;

                //Property Mappings
                NameValueCollection mappings = dec.PropertyMappings.GetPropertyMappings();
                foreach (string srcProp in mappings.Keys)
                {
                    string dstProp = mappings[srcProp];
                    FdoPropertyMappingElement p = new FdoPropertyMappingElement();
                    p.source = srcProp;
                    p.target = dstProp;

                    PropertyConversionNodeDecorator conv = dec.PropertyMappings.GetConversionRule(p.source);
                    p.nullOnFailedConversion = conv.NullOnFailedConversion;
                    p.truncate = conv.Truncate;
                    p.createIfNotExists = conv.CreateIfNotExists;

                    pmaps.Add(p);
                }

                foreach (string alias in dec.ExpressionMappings.GetAliases())
                {
                    FdoExpressionMappingElement e = new FdoExpressionMappingElement();
                    e.alias = alias;
                    ExpressionMappingInfo exMap = dec.ExpressionMappings.GetMapping(alias);
                    e.Expression = exMap.Expression;
                    e.target = exMap.TargetProperty;

                    PropertyConversionNodeDecorator conv = dec.ExpressionMappings.GetConversionRule(e.alias);
                    e.nullOnFailedConversion = conv.NullOnFailedConversion;
                    e.truncate = conv.Truncate;
                    e.createIfNotExists = conv.CreateIfNotExists;

                    emaps.Add(e);
                }

                task.PropertyMappings = pmaps.ToArray();
                task.ExpressionMappings = emaps.ToArray();

                tasks.Add(task);
            }
            def.Connections = conns.ToArray();
            def.CopyTasks = tasks.ToArray();
            return def;
        }

        private void Load(FdoBulkCopyOptions def, string name)
        {
            txtName.Text = name;

            grdConnections.Rows.Clear();

            foreach (string connName in def.ConnectionNames)
            {
                this.AddParticipatingConnection(connName);
            }

            TreeNode root = mTreeView.Nodes[0];
            foreach (FdoClassCopyOptions task in def.ClassCopyOptions)
            {
                //Init w/ defaults
                CopyTaskNodeDecorator dec = AddNewTask(
                                               root,
                                               task.SourceConnectionName,
                                               task.SourceSchema,
                                               task.SourceClassName,
                                               task.TargetConnectionName,
                                               task.TargetSchema,
                                               task.TargetClassName,
                                               task.Name,
                                               task.CreateIfNotExists);

                _tasks[dec.DecoratedNode.Index] = dec;
                root.Expand();

                btnSave.Enabled = (root.Nodes.Count > 0);

                //Options
                dec.Options.BatchSize = task.BatchSize;
                dec.Options.Delete = task.DeleteTarget;
                dec.Options.SourceFilter = task.SourceFilter;
                dec.Options.Flatten = task.FlattenGeometries;

                var checkProps = new List<string>(task.CheckSourceProperties);
                //Property Mappings
                foreach (string srcProp in task.SourcePropertyNames)
                {
                    string dstProp = task.GetTargetProperty(srcProp);
                    bool createIfNotExists = checkProps.Contains(srcProp);
                    try
                    {
                        dec.PropertyMappings.MapProperty(srcProp, dstProp, createIfNotExists);
                    }
                    catch (MappingException ex)
                    {
                        LoggingService.Info("Skipping mapping: " + srcProp + " => " + dstProp + " (" + ex.Message + ")");
                    }
                    FdoDataPropertyConversionRule rule = task.GetDataConversionRule(srcProp);
                    PropertyConversionNodeDecorator cd = dec.PropertyMappings.GetConversionRule(srcProp);
                    if (rule != null)
                    {
                        cd.NullOnFailedConversion = rule.NullOnFailure;
                        cd.Truncate = rule.Truncate;
                    }
                }

                //Expression Mappings
                foreach (string alias in task.SourceAliases)
                {
                    string expr = task.GetExpression(alias);
                    string dstProp = task.GetTargetPropertyForAlias(alias);
                    bool createIfNotExists = checkProps.Contains(alias);
                    dec.ExpressionMappings.AddExpression(alias, expr);
                    dec.ExpressionMappings.MapExpression(alias, dstProp, createIfNotExists);

                    FdoDataPropertyConversionRule rule = task.GetDataConversionRule(alias);
                    PropertyConversionNodeDecorator cd = dec.ExpressionMappings.GetConversionRule(alias);

                    if (rule != null)
                    {
                        cd.NullOnFailedConversion = rule.NullOnFailure;
                        cd.Truncate = rule.Truncate;
                    }
                }
            }
        }

        private void OnBeforeExecuteBulkCopy(object sender, CancelEventArgs e) { }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var loader = new TaskLoader();
            string name = string.Empty;
            var opts = loader.BulkCopyFromXml(Save(), ref name, false);
            var bcp = new FdoBulkCopy(opts);
            CancelEventHandler ch = new CancelEventHandler(OnBeforeExecuteBulkCopy);
            bcp.BeforeExecute += ch;

            IFdoSpecializedEtlProcess spec = bcp as IFdoSpecializedEtlProcess;
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

    internal class ExpressionMappingInfo
    {
    	public string Expression;
    	public string TargetProperty;
    }
    
    [global::System.Serializable]
    public class MappingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public MappingException() { }
        public MappingException(string message) : base(message) { }
        public MappingException(string message, Exception inner) : base(message, inner) { }
        protected MappingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
