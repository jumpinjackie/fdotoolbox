namespace FdoToolbox.DataStoreManager.Controls
{
    partial class FdoSchemaView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FdoSchemaView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddSchema = new System.Windows.Forms.ToolStripButton();
            this.btnFix = new System.Windows.Forms.ToolStripButton();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.schemaTree = new System.Windows.Forms.TreeView();
            this.imgTree = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TAB_LOGICAL = new System.Windows.Forms.TabPage();
            this.TAB_PHYSICAL = new System.Windows.Forms.TabPage();
            this.contentPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.splitContainer1);
            this.contentPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddSchema,
            this.btnFix,
            this.btnUndo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(449, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddSchema
            // 
            this.btnAddSchema.Image = global::FdoToolbox.DataStoreManager.Images.chart_organisation_add;
            this.btnAddSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddSchema.Name = "btnAddSchema";
            this.btnAddSchema.Size = new System.Drawing.Size(94, 22);
            this.btnAddSchema.Text = "Add Schema";
            this.btnAddSchema.Click += new System.EventHandler(this.btnAddSchema_Click);
            // 
            // btnFix
            // 
            this.btnFix.Image = global::FdoToolbox.DataStoreManager.Images.wrench_orange;
            this.btnFix.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(132, 22);
            this.btnFix.Text = "Fix Incompatibilities";
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Image = global::FdoToolbox.DataStoreManager.Images.arrow_undo;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(56, 22);
            this.btnUndo.Text = "Undo";
            this.btnUndo.ToolTipText = "Undo all schema modifications";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.schemaTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(449, 267);
            this.splitContainer1.SplitterDistance = 179;
            this.splitContainer1.TabIndex = 5;
            // 
            // schemaTree
            // 
            this.schemaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schemaTree.ImageIndex = 0;
            this.schemaTree.ImageList = this.imgTree;
            this.schemaTree.Location = new System.Drawing.Point(0, 0);
            this.schemaTree.Name = "schemaTree";
            this.schemaTree.SelectedImageIndex = 0;
            this.schemaTree.Size = new System.Drawing.Size(179, 267);
            this.schemaTree.TabIndex = 4;
            this.schemaTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.schemaTree_KeyUp);
            // 
            // imgTree
            // 
            this.imgTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTree.ImageStream")));
            this.imgTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imgTree.Images.SetKeyName(0, "chart_organisation.png");
            this.imgTree.Images.SetKeyName(1, "database_table.png");
            this.imgTree.Images.SetKeyName(2, "feature_class.png");
            this.imgTree.Images.SetKeyName(3, "key.png");
            this.imgTree.Images.SetKeyName(4, "table.png");
            this.imgTree.Images.SetKeyName(5, "shape_handles.png");
            this.imgTree.Images.SetKeyName(6, "table_relationship.png");
            this.imgTree.Images.SetKeyName(7, "package.png");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TAB_LOGICAL);
            this.tabControl1.Controls.Add(this.TAB_PHYSICAL);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(266, 267);
            this.tabControl1.TabIndex = 5;
            // 
            // TAB_LOGICAL
            // 
            this.TAB_LOGICAL.Location = new System.Drawing.Point(4, 22);
            this.TAB_LOGICAL.Name = "TAB_LOGICAL";
            this.TAB_LOGICAL.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_LOGICAL.Size = new System.Drawing.Size(258, 241);
            this.TAB_LOGICAL.TabIndex = 0;
            this.TAB_LOGICAL.Text = "Logical Property";
            this.TAB_LOGICAL.UseVisualStyleBackColor = true;
            // 
            // TAB_PHYSICAL
            // 
            this.TAB_PHYSICAL.Location = new System.Drawing.Point(4, 22);
            this.TAB_PHYSICAL.Name = "TAB_PHYSICAL";
            this.TAB_PHYSICAL.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_PHYSICAL.Size = new System.Drawing.Size(258, 241);
            this.TAB_PHYSICAL.TabIndex = 1;
            this.TAB_PHYSICAL.Text = "Physical Mapping";
            this.TAB_PHYSICAL.UseVisualStyleBackColor = true;
            // 
            // FdoSchemaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.HeaderText = "Feature Schemas";
            this.Name = "FdoSchemaView";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnFix;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TAB_LOGICAL;
        private System.Windows.Forms.TabPage TAB_PHYSICAL;
        protected internal System.Windows.Forms.TreeView schemaTree;
        private System.Windows.Forms.ImageList imgTree;
        private System.Windows.Forms.ToolStripButton btnAddSchema;
        private System.Windows.Forms.ToolStripButton btnUndo;
    }
}
