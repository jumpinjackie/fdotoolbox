namespace FdoToolbox.Base.Controls
{
    partial class FdoDataPreviewCtl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.queryPanel = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmbQueryMode = new System.Windows.Forms.ToolStripComboBox();
            this.btnQuery = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.sDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnInsert = new System.Windows.Forms.ToolStripButton();
            this.resultTab = new System.Windows.Forms.TabControl();
            this.TAB_GRID = new System.Windows.Forms.TabPage();
            this.grdResults = new System.Windows.Forms.DataGridView();
            this.ctxGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updateThisFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteThisFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblElapsedTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.TAB_MAP = new System.Windows.Forms.TabPage();
            this.mapCtl = new FdoToolbox.Base.Controls.FdoMapPreview();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.resultTab.SuspendLayout();
            this.TAB_GRID.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).BeginInit();
            this.ctxGridView.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.TAB_MAP.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.queryPanel);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.resultTab);
            this.splitContainer1.Size = new System.Drawing.Size(688, 480);
            this.splitContainer1.SplitterDistance = 219;
            this.splitContainer1.TabIndex = 0;
            // 
            // queryPanel
            // 
            this.queryPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.queryPanel.Location = new System.Drawing.Point(0, 25);
            this.queryPanel.Name = "queryPanel";
            this.queryPanel.Size = new System.Drawing.Size(684, 194);
            this.queryPanel.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cmbQueryMode,
            this.btnQuery,
            this.btnCancel,
            this.toolStripSeparator2,
            this.btnClear,
            this.btnSave,
            this.toolStripSeparator1,
            this.btnInsert});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(684, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(73, 22);
            this.toolStripLabel1.Text = "Query Mode";
            // 
            // cmbQueryMode
            // 
            this.cmbQueryMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQueryMode.Name = "cmbQueryMode";
            this.cmbQueryMode.Size = new System.Drawing.Size(121, 25);
            this.cmbQueryMode.SelectedIndexChanged += new System.EventHandler(this.cmbQueryMode_SelectedIndexChanged);
            // 
            // btnQuery
            // 
            this.btnQuery.Image = global::FdoToolbox.Base.Images.table_go;
            this.btnQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(67, 22);
            this.btnQuery.Text = "Execute";
            this.btnQuery.ToolTipText = "Execute the query";
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::FdoToolbox.Base.Images.cross;
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(63, 22);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Cancel the running query";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::FdoToolbox.Base.Images.table_delete;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(94, 22);
            this.btnClear.Text = "Clear Results";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sDFToolStripMenuItem,
            this.sQLiteToolStripMenuItem});
            this.btnSave.Image = global::FdoToolbox.Base.Images.disk;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 22);
            this.btnSave.Text = "Save";
            // 
            // sDFToolStripMenuItem
            // 
            this.sDFToolStripMenuItem.Image = global::FdoToolbox.Base.Images.database;
            this.sDFToolStripMenuItem.Name = "sDFToolStripMenuItem";
            this.sDFToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.sDFToolStripMenuItem.Text = "SDF";
            this.sDFToolStripMenuItem.Click += new System.EventHandler(this.saveSdf_Click);
            // 
            // sQLiteToolStripMenuItem
            // 
            this.sQLiteToolStripMenuItem.Image = global::FdoToolbox.Base.Images.database;
            this.sQLiteToolStripMenuItem.Name = "sQLiteToolStripMenuItem";
            this.sQLiteToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.sQLiteToolStripMenuItem.Text = "SQLite";
            this.sQLiteToolStripMenuItem.Click += new System.EventHandler(this.saveSQLite_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnInsert
            // 
            this.btnInsert.Image = global::FdoToolbox.Base.Images.application_form_edit;
            this.btnInsert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(93, 22);
            this.btnInsert.Text = "New Feature";
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // resultTab
            // 
            this.resultTab.Controls.Add(this.TAB_GRID);
            this.resultTab.Controls.Add(this.TAB_MAP);
            this.resultTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultTab.Location = new System.Drawing.Point(0, 0);
            this.resultTab.Name = "resultTab";
            this.resultTab.SelectedIndex = 0;
            this.resultTab.Size = new System.Drawing.Size(684, 253);
            this.resultTab.TabIndex = 0;
            // 
            // TAB_GRID
            // 
            this.TAB_GRID.Controls.Add(this.grdResults);
            this.TAB_GRID.Controls.Add(this.statusStrip1);
            this.TAB_GRID.Location = new System.Drawing.Point(4, 22);
            this.TAB_GRID.Name = "TAB_GRID";
            this.TAB_GRID.Size = new System.Drawing.Size(676, 227);
            this.TAB_GRID.TabIndex = 0;
            this.TAB_GRID.Text = "Grid View";
            this.TAB_GRID.UseVisualStyleBackColor = true;
            // 
            // grdResults
            // 
            this.grdResults.AllowUserToAddRows = false;
            this.grdResults.AllowUserToDeleteRows = false;
            this.grdResults.AllowUserToOrderColumns = true;
            this.grdResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResults.ContextMenuStrip = this.ctxGridView;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.NullValue = "<null>";
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdResults.DefaultCellStyle = dataGridViewCellStyle1;
            this.grdResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdResults.Location = new System.Drawing.Point(0, 0);
            this.grdResults.Name = "grdResults";
            this.grdResults.ReadOnly = true;
            this.grdResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdResults.Size = new System.Drawing.Size(676, 205);
            this.grdResults.TabIndex = 3;
            this.grdResults.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdResults_MouseDown);
            this.grdResults.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdResults_CellMouseDown);
            // 
            // ctxGridView
            // 
            this.ctxGridView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateThisFeatureToolStripMenuItem,
            this.deleteThisFeatureToolStripMenuItem});
            this.ctxGridView.Name = "ctxGridView";
            this.ctxGridView.Size = new System.Drawing.Size(168, 48);
            // 
            // updateThisFeatureToolStripMenuItem
            // 
            this.updateThisFeatureToolStripMenuItem.Image = global::FdoToolbox.Base.Images.application_form_edit;
            this.updateThisFeatureToolStripMenuItem.Name = "updateThisFeatureToolStripMenuItem";
            this.updateThisFeatureToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.updateThisFeatureToolStripMenuItem.Text = "Update Feature(s)";
            this.updateThisFeatureToolStripMenuItem.Click += new System.EventHandler(this.updateThisFeatureToolStripMenuItem_Click);
            // 
            // deleteThisFeatureToolStripMenuItem
            // 
            this.deleteThisFeatureToolStripMenuItem.Image = global::FdoToolbox.Base.Images.cross;
            this.deleteThisFeatureToolStripMenuItem.Name = "deleteThisFeatureToolStripMenuItem";
            this.deleteThisFeatureToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.deleteThisFeatureToolStripMenuItem.Text = "Delete Feature(s)";
            this.deleteThisFeatureToolStripMenuItem.Click += new System.EventHandler(this.deleteThisFeatureToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblMessage,
            this.lblElapsedTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 205);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(676, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblMessage
            // 
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 17);
            // 
            // lblElapsedTime
            // 
            this.lblElapsedTime.Name = "lblElapsedTime";
            this.lblElapsedTime.Size = new System.Drawing.Size(661, 17);
            this.lblElapsedTime.Spring = true;
            this.lblElapsedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TAB_MAP
            // 
            this.TAB_MAP.Controls.Add(this.mapCtl);
            this.TAB_MAP.Location = new System.Drawing.Point(4, 22);
            this.TAB_MAP.Name = "TAB_MAP";
            this.TAB_MAP.Size = new System.Drawing.Size(676, 227);
            this.TAB_MAP.TabIndex = 1;
            this.TAB_MAP.Text = "Map View";
            this.TAB_MAP.UseVisualStyleBackColor = true;
            // 
            // mapCtl
            // 
            this.mapCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapCtl.Location = new System.Drawing.Point(0, 0);
            this.mapCtl.Name = "mapCtl";
            this.mapCtl.Size = new System.Drawing.Size(676, 227);
            this.mapCtl.TabIndex = 0;
            // 
            // FdoDataPreviewCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FdoDataPreviewCtl";
            this.Size = new System.Drawing.Size(688, 480);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.resultTab.ResumeLayout(false);
            this.TAB_GRID.ResumeLayout(false);
            this.TAB_GRID.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).EndInit();
            this.ctxGridView.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.TAB_MAP.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl resultTab;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cmbQueryMode;
        private System.Windows.Forms.ToolStripButton btnQuery;
        private System.Windows.Forms.ToolStripButton btnCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton btnSave;
        private System.Windows.Forms.ToolStripMenuItem sDFToolStripMenuItem;
        private System.Windows.Forms.Panel queryPanel;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.TabPage TAB_GRID;
        private System.Windows.Forms.TabPage TAB_MAP;
        private FdoMapPreview mapCtl;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblMessage;
        private System.Windows.Forms.ToolStripStatusLabel lblElapsedTime;
        private System.Windows.Forms.DataGridView grdResults;
        private System.Windows.Forms.ToolStripMenuItem sQLiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnInsert;
        private System.Windows.Forms.ContextMenuStrip ctxGridView;
        private System.Windows.Forms.ToolStripMenuItem deleteThisFeatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateThisFeatureToolStripMenuItem;
    }
}
