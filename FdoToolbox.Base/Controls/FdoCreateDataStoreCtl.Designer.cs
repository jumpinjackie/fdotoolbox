namespace FdoToolbox.Base.Controls
{
    partial class FdoCreateDataStoreCtl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbProvider = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grdDataStoreProperties = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.grdConnectionProperties = new System.Windows.Forms.DataGridView();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ctxHelper = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertCurrentApplicationPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertFilePathOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertFilePathSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataStoreProperties)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdConnectionProperties)).BeginInit();
            this.ctxHelper.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbProvider);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(484, 51);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // cmbProvider
            // 
            this.cmbProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProvider.FormattingEnabled = true;
            this.cmbProvider.Location = new System.Drawing.Point(94, 19);
            this.cmbProvider.Name = "cmbProvider";
            this.cmbProvider.Size = new System.Drawing.Size(381, 21);
            this.cmbProvider.TabIndex = 1;
            this.cmbProvider.SelectionChangeCommitted += new System.EventHandler(this.cmbProvider_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FDO Provider";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.grdDataStoreProperties);
            this.groupBox2.Location = new System.Drawing.Point(3, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(484, 119);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Store Properties";
            // 
            // grdDataStoreProperties
            // 
            this.grdDataStoreProperties.AllowUserToAddRows = false;
            this.grdDataStoreProperties.AllowUserToDeleteRows = false;
            this.grdDataStoreProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdDataStoreProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDataStoreProperties.Location = new System.Drawing.Point(6, 19);
            this.grdDataStoreProperties.Name = "grdDataStoreProperties";
            this.grdDataStoreProperties.Size = new System.Drawing.Size(469, 94);
            this.grdDataStoreProperties.TabIndex = 0;
            this.grdDataStoreProperties.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdDataStoreProperties_CellMouseDown);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.grdConnectionProperties);
            this.groupBox3.Location = new System.Drawing.Point(3, 186);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(484, 130);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Connection Properties";
            // 
            // grdConnectionProperties
            // 
            this.grdConnectionProperties.AllowUserToAddRows = false;
            this.grdConnectionProperties.AllowUserToDeleteRows = false;
            this.grdConnectionProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdConnectionProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdConnectionProperties.Location = new System.Drawing.Point(6, 20);
            this.grdConnectionProperties.Name = "grdConnectionProperties";
            this.grdConnectionProperties.Size = new System.Drawing.Size(469, 104);
            this.grdConnectionProperties.TabIndex = 0;
            this.grdConnectionProperties.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdConnectionProperties_CellMouseDown);
            this.grdConnectionProperties.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.grdConnectionProperties_CellPainting);
            this.grdConnectionProperties.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.grdConnectionProperties_EditingControlShowing);
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Location = new System.Drawing.Point(322, 322);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(403, 322);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ctxHelper
            // 
            this.ctxHelper.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertCurrentApplicationPathToolStripMenuItem,
            this.insertFilePathOpenToolStripMenuItem,
            this.insertFilePathSaveToolStripMenuItem,
            this.insertDirectoryToolStripMenuItem});
            this.ctxHelper.Name = "ctxHelper";
            this.ctxHelper.Size = new System.Drawing.Size(235, 92);
            // 
            // insertCurrentApplicationPathToolStripMenuItem
            // 
            this.insertCurrentApplicationPathToolStripMenuItem.Name = "insertCurrentApplicationPathToolStripMenuItem";
            this.insertCurrentApplicationPathToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.insertCurrentApplicationPathToolStripMenuItem.Text = "Insert Current Application Path";
            this.insertCurrentApplicationPathToolStripMenuItem.Click += new System.EventHandler(this.insertCurrentApplicationPathToolStripMenuItem_Click);
            // 
            // insertFilePathOpenToolStripMenuItem
            // 
            this.insertFilePathOpenToolStripMenuItem.Name = "insertFilePathOpenToolStripMenuItem";
            this.insertFilePathOpenToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.insertFilePathOpenToolStripMenuItem.Text = "Insert File Path (Open)";
            this.insertFilePathOpenToolStripMenuItem.Click += new System.EventHandler(this.insertFilePathOpenToolStripMenuItem_Click);
            // 
            // insertFilePathSaveToolStripMenuItem
            // 
            this.insertFilePathSaveToolStripMenuItem.Name = "insertFilePathSaveToolStripMenuItem";
            this.insertFilePathSaveToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.insertFilePathSaveToolStripMenuItem.Text = "Insert File Path (Save)";
            this.insertFilePathSaveToolStripMenuItem.Click += new System.EventHandler(this.insertFilePathSaveToolStripMenuItem_Click);
            // 
            // insertDirectoryToolStripMenuItem
            // 
            this.insertDirectoryToolStripMenuItem.Name = "insertDirectoryToolStripMenuItem";
            this.insertDirectoryToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.insertDirectoryToolStripMenuItem.Text = "Insert Directory";
            this.insertDirectoryToolStripMenuItem.Click += new System.EventHandler(this.insertDirectoryToolStripMenuItem_Click);
            // 
            // FdoCreateDataStoreCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FdoCreateDataStoreCtl";
            this.Size = new System.Drawing.Size(490, 355);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDataStoreProperties)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdConnectionProperties)).EndInit();
            this.ctxHelper.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView grdDataStoreProperties;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView grdConnectionProperties;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbProvider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip ctxHelper;
        private System.Windows.Forms.ToolStripMenuItem insertCurrentApplicationPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertFilePathOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertFilePathSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertDirectoryToolStripMenuItem;
    }
}
