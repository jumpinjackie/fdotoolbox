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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using FdoToolbox.DataStoreManager.Controls.SchemaDesigner;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Utility;
using FdoToolbox.Base.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.DataStoreManager.Controls
{
    public partial class FdoDataStoreCtrl : ViewContent
    {
        private Color _defaultButtonColor;

        public FdoDataStoreCtrl()
        {
            InitializeComponent();
            this.Title = ResourceService.GetString("TITLE_DATA_STORE");
            _defaultButtonColor = btnApply.BackColor;
        }

        private SchemaDesignContext _context;

        private FdoConnection _conn;

        public FdoDataStoreCtrl(FdoConnection conn) : this()
        {
            _conn = conn;
        }

        protected override void OnLoad(EventArgs e)
        {
            _context = new SchemaDesignContext(_conn);

            if (!_context.IsConnected)
                this.Title = ResourceService.GetString("TITLE_DATA_STORE_STANDALONE");

            schemaView.UpdateState += new EventHandler(OnUpdateState);
            spatialContextView.UpdateState += new EventHandler(OnUpdateState);

            schemaView.Context = _context;
            spatialContextView.Context = _context;

            EvaluateCommandStates();

            base.OnLoad(e);
        }

        void HighlightApplyButton()
        {
            btnApply.BackColor = Color.OrangeRed;
        }

        void ResetApplyButton()
        {
            btnApply.BackColor = _defaultButtonColor;
        }

        void OnUpdateState(object sender, EventArgs e)
        {
            EvaluateCommandStates();
        }

        void EvaluateCommandStates()
        {
            _context.EvaluateCapabilities();

            btnApply.Enabled = true;
            btnSaveAllSchemas.Enabled = true;
            btnSaveEverything.Enabled = true;
            btnSaveSpatialContexts.Enabled = true;
            btnSaveSelectedSchema.Enabled = true;

            //If we're not connected, there's nothing to apply
            btnApply.Enabled = _context.IsConnected;

            if (!_context.IsConnected)
                return;

            btnSaveAllSchemas.Enabled = _context.Schemas.Count > 0 && _context.SchemasChanged;
            btnSaveSelectedSchema.Enabled = !string.IsNullOrEmpty(schemaView.GetSelectedSchema()) && _context.SchemasChanged;
            btnSaveSpatialContexts.Enabled = _context.SpatialContexts.Count > 0;

            //Any one of the above will do
            btnSaveEverything.Enabled = (btnSaveAllSchemas.Enabled || btnSaveSelectedSchema.Enabled || btnSaveSpatialContexts.Enabled);

            if (_context.SchemasChanged && btnApply.Enabled)
            {
                HighlightApplyButton();
            }
            else
            {
                ResetApplyButton();
            }

            spatialContextView.EvaluateCommandStates();
        }

        private void btnSaveXmlConfig_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile("Save XML Configuration", "XML Files (*.xml)|*.xml");
            if (!string.IsNullOrEmpty(file))
            {
                var conf = _context.GetConfiguration();
                conf.Save(file);
                MessageService.ShowMessageFormatted("Configuration saved to {0}", file);
                LoggingService.Info("Configuration Saved");
            }
        }

        private void btnSaveSdf_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile("Export to SDF", "Autodesk SDF (*.sdf)|*.sdf");
            if (!string.IsNullOrEmpty(file))
            {
                //SDF only allows
                // 1 feature schema
                // 1 spatial context
                if (ExportToFile("OSGeo.SDF", file, 1, 1))
                {
                    if (Confirm("Export", "Successfully exported to: " + file + ". Connect to it?"))
                    {
                        var connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                        var nameSvc = ServiceManager.Instance.GetService<NamingService>();

                        var name = nameSvc.GetDefaultConnectionName("OSGeo.SDF", System.IO.Path.GetFileNameWithoutExtension(file));
                        var conn = ExpressUtility.CreateFlatFileConnection(file);

                        connMgr.AddConnection(name, conn);
                    }
                }
            }
        }

        private void btnSaveSqlite_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile("Export to SQLite", "SQLite databases (*.sqlite)|*.sqlite|SQLite databases (*.db)|*.db");
            if (!string.IsNullOrEmpty(file))
            {
                //SQLite only allows
                // 1 feature schema
                // any number spatial contexts
                if (ExportToFile("OSGeo.SQLite", file, 1, -1))
                {
                    if (Confirm("Export", "Successfully exported to: " + file + ". Connect to it?"))
                    {
                        var connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                        var nameSvc = ServiceManager.Instance.GetService<NamingService>();

                        var name = nameSvc.GetDefaultConnectionName("OSGeo.SQLite", System.IO.Path.GetFileNameWithoutExtension(file));
                        var conn = ExpressUtility.CreateFlatFileConnection(file);

                        connMgr.AddConnection(name, conn);
                    }
                }
            }
        }

        private bool ExportToFile(string provider, string file, int maxSchemas, int maxSpatialContexts)
        {
            var conf = _context.GetConfiguration();
            if (maxSchemas > 0 && conf.Schemas.Count > maxSchemas)
            {
                ShowError("This provider only allows " + maxSchemas + " feature schema(s)");
                return false;
            }
            if (maxSpatialContexts > 0 && conf.SpatialContexts.Length > maxSpatialContexts)
            {
                ShowError("This provider only allows " + maxSpatialContexts + " spatial context(s)");
                return false;
            }

            if (ExpressUtility.CreateFlatFileDataSource(provider, file))
            {
                var conn = ExpressUtility.CreateFlatFileConnection(provider, file);
                var schema = conf.Schemas[0];
                schema = FdoSchemaUtil.CloneSchema(schema, true);
                using (var svc = conn.CreateFeatureService())
                {
                    IncompatibleSchema incs;
                    if (!svc.CanApplySchema(schema, out incs))
                    {
                        string msg = "This schema has incompatibilities. Attempt to fix it? " + incs.ToString();
                        if (!WrappedMessageBox.Confirm("Incompatible schema", msg, MessageBoxText.YesNo))
                        {
                            return false;
                        }
                        schema = svc.AlterSchema(schema, incs);
                    }

                    using (var cur = new TempCursor(Cursors.WaitCursor))
                    {
                        foreach (var sc in conf.SpatialContexts)
                        {
                            svc.CreateSpatialContext(sc, true);
                        }

                        //Only for SQLite, but this is general enough
                        var fscs = svc.DescribeSchema();
                        if (fscs.Count == 1)
                        {
                            var fsc = fscs[0];
                            foreach (ClassDefinition cls in schema.Classes)
                            {
                                var klass = FdoSchemaUtil.CloneClass(cls);
                                fsc.Classes.Add(klass);
                            }
                            schema = fsc;
                        }
                        svc.ApplySchema(schema);
                    }
                }
                conn.Close();
            }
            return true;
        }

        private void btnSaveEverything_Click(object sender, EventArgs e)
        {
            using (var cur = new TempCursor(Cursors.WaitCursor))
            {
                ResetApplyButton();
                if (_context.SaveSpatialContexts() && _context.SaveAllSchemas())
                {
                    MessageService.ShowMessage("Changes have been saved");
                    schemaView.Reset();
                }
            }
        }

        private void btnSaveSpatialContexts_Click(object sender, EventArgs e)
        {
            using (var cur = new TempCursor(Cursors.WaitCursor))
            {
                ResetApplyButton();
                if (_context.SaveSpatialContexts())
                {
                    MessageService.ShowMessage("Spatial Context changes have been saved");
                    EvaluateCommandStates();
                }
            }
        }

        private void btnSaveAllSchemas_Click(object sender, EventArgs e)
        {
            using (var cur = new TempCursor(Cursors.WaitCursor))
            {
                try
                {
                    ResetApplyButton();
                    if (_context.SaveAllSchemas())
                    {
                        MessageService.ShowMessage("Schemas saved");
                        schemaView.Reset();
                    }
                }
                catch (Exception ex)
                {
                    MessageService.ShowError(ex);
                }
            }
        }

        private void btnSaveSelectedSchema_Click(object sender, EventArgs e)
        {
            using (var cur = new TempCursor(Cursors.WaitCursor))
            {
                ResetApplyButton();
                string schName = schemaView.GetSelectedSchema();
                if (!string.IsNullOrEmpty(schName))
                {
                    try
                    {
                        if (_context.SaveSchema(schName))
                        {
                            MessageService.ShowMessage("Schema saved");
                            schemaView.Reset();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowError(ex);
                    }
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile("Open XML Configuration", "XML files (*.xml)|*.xml");
            if (!string.IsNullOrEmpty(file))
            {
                var conf = FdoDataStoreConfiguration.FromFile(file);
                if (!_context.IsConnected)
                {
                    using (var cur = new TempCursor(Cursors.WaitCursor))
                    {
                        _context.SetConfiguration(conf);
                        schemaView.Reset();
                    }
                }
                else
                {
                    //Prompt for elements to import 
                    var impDiag = new ImportElementsDialog(conf, _context);
                    impDiag.ShowDialog();
                }
            }
        }

        public event EventHandler DataStoreChanged;

        private void OnDataStoreChanged()
        {
            var handler = this.DataStoreChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
