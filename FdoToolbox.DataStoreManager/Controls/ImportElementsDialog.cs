using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using FdoToolbox.DataStoreManager.Controls.SchemaDesigner;
using OSGeo.FDO.Schema;
using ICSharpCode.Core;

namespace FdoToolbox.DataStoreManager.Controls
{
    public partial class ImportElementsDialog : Form
    {
        private ImportElementsDialog()
        {
            InitializeComponent();
        }

        private FdoDataStoreConfiguration _dataStore;
        private SchemaDesignContext _context;

        public ImportElementsDialog(FdoDataStoreConfiguration dataStore, SchemaDesignContext context)
            : this()
        {
            _dataStore = dataStore;
            _context = context;
        }

        private bool _load = true;

        protected override void OnLoad(EventArgs e)
        {
            if (!_context.IsConnected)
            {
                lstFeatureSchemas.DataSource = _dataStore.Schemas;
                lstSpatialContexts.DataSource = _dataStore.SpatialContexts;
            }
            else
            {
                var currentSchemaCount = _context.Schemas.Count;
                var importedSchemaCount = _dataStore.Schemas.Count;

                var currentScCount = _context.SpatialContexts.Count;
                var importedScCount = _context.SpatialContexts.Count;

                lstFeatureSchemas.DataSource = _dataStore.Schemas;
                lstSpatialContexts.DataSource = _dataStore.SpatialContexts;
                
                if (_context.CanHaveMultipleSchemas)
                {
                    lstFeatureSchemas.SelectionMode = SelectionMode.MultiSimple;
                }
                else
                {
                    lstFeatureSchemas.SelectionMode = SelectionMode.One;

                    lblClassImport.Visible = (currentSchemaCount > 0);
                }

                if (_context.CanHaveMultipleSpatialContexts)
                {
                    lstSpatialContexts.SelectionMode = SelectionMode.MultiSimple;
                }
                else
                {
                    lstSpatialContexts.SelectionMode = SelectionMode.One;

                    lstSpatialContexts.Enabled = (currentScCount == 0);
                }
            }

            lstFeatureSchemas.ClearSelected();
            lstSpatialContexts.ClearSelected();

            _load = false;
        }

        private void UpdateButtonState()
        {
            btnImport.Enabled = (lstSpatialContexts.SelectedItems.Count > 0 || lstFeatureSchemas.SelectedItems.Count > 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            List<string> notAdded = new List<string>();
            foreach (OSGeo.FDO.Schema.FeatureSchema fsc in lstFeatureSchemas.SelectedItems)
            {
                string [] classes = _context.AddClassesToSchema(fsc.Name, fsc.Classes);
                if (classes.Length > 0)
                {
                    foreach (var c in classes)
                    {
                        notAdded.Add(fsc.Name + ":" + c);
                    }
                }
            }

            foreach (SpatialContextInfo sc in lstSpatialContexts.SelectedItems)
            {
                _context.AddSpatialContext(sc);
            }

            if (notAdded.Count > 0)
                MessageService.ShowMessage("The following classes were not added because they already exist: " + string.Join(",", notAdded.ToArray()));

            this.DialogResult = DialogResult.OK;
        }

        private void lstFeatureSchemas_SelectedIndexChanged(object sender, EventArgs e)
        {
            lnkClearSchemas.Enabled = lstFeatureSchemas.SelectedItems.Count > 0;
            UpdateButtonState();

            if (_load)
                return;

            List<string> existing = new List<string>();
            foreach (FeatureSchema fsc in lstFeatureSchemas.SelectedItems)
            {
                if (_context.SchemaNameExists(fsc.Name))
                    existing.Add(fsc.Name);
            }

            if (existing.Count > 0)
            {
                MessageService.ShowWarning("The following schemas already exist " + string.Join(",", existing.ToArray()) + ". Classes from these selected schemas will be copied into the existing schemas");
            }
        }

        private void lstSpatialContexts_SelectedIndexChanged(object sender, EventArgs e)
        {
            lnkClearSpatialContexts.Enabled = lstSpatialContexts.SelectedItems.Count > 0;
            UpdateButtonState();
        }

        private void lnkClearSchemas_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lstFeatureSchemas.ClearSelected();
            UpdateButtonState();
        }

        private void lnkClearSpatialContexts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lstSpatialContexts.ClearSelected();
            UpdateButtonState();
        }
    }
}
