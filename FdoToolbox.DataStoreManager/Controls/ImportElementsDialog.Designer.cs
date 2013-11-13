namespace FdoToolbox.DataStoreManager.Controls
{
    partial class ImportElementsDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblClassImport = new System.Windows.Forms.Label();
            this.lnkClearSchemas = new System.Windows.Forms.LinkLabel();
            this.lstFeatureSchemas = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lnkClearSpatialContexts = new System.Windows.Forms.LinkLabel();
            this.lstSpatialContexts = new System.Windows.Forms.ListBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose the elements to import";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblClassImport);
            this.groupBox1.Controls.Add(this.lnkClearSchemas);
            this.groupBox1.Controls.Add(this.lstFeatureSchemas);
            this.groupBox1.Location = new System.Drawing.Point(16, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 122);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Feature Schemas";
            // 
            // lblClassImport
            // 
            this.lblClassImport.AutoSize = true;
            this.lblClassImport.Location = new System.Drawing.Point(90, 96);
            this.lblClassImport.Name = "lblClassImport";
            this.lblClassImport.Size = new System.Drawing.Size(344, 13);
            this.lblClassImport.TabIndex = 2;
            this.lblClassImport.Text = "Classes in the selected schema will be imported into the current schema";
            this.lblClassImport.Visible = false;
            // 
            // lnkClearSchemas
            // 
            this.lnkClearSchemas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkClearSchemas.AutoSize = true;
            this.lnkClearSchemas.Location = new System.Drawing.Point(6, 96);
            this.lnkClearSchemas.Name = "lnkClearSchemas";
            this.lnkClearSchemas.Size = new System.Drawing.Size(78, 13);
            this.lnkClearSchemas.TabIndex = 1;
            this.lnkClearSchemas.TabStop = true;
            this.lnkClearSchemas.Text = "Clear Selection";
            this.lnkClearSchemas.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearSchemas_LinkClicked);
            // 
            // lstFeatureSchemas
            // 
            this.lstFeatureSchemas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFeatureSchemas.DisplayMember = "Name";
            this.lstFeatureSchemas.FormattingEnabled = true;
            this.lstFeatureSchemas.Location = new System.Drawing.Point(7, 20);
            this.lstFeatureSchemas.Name = "lstFeatureSchemas";
            this.lstFeatureSchemas.Size = new System.Drawing.Size(433, 69);
            this.lstFeatureSchemas.TabIndex = 0;
            this.lstFeatureSchemas.SelectedIndexChanged += new System.EventHandler(this.lstFeatureSchemas_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lnkClearSpatialContexts);
            this.groupBox3.Controls.Add(this.lstSpatialContexts);
            this.groupBox3.Location = new System.Drawing.Point(16, 170);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(446, 111);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Spatial Contexts";
            // 
            // lnkClearSpatialContexts
            // 
            this.lnkClearSpatialContexts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkClearSpatialContexts.AutoSize = true;
            this.lnkClearSpatialContexts.Location = new System.Drawing.Point(6, 84);
            this.lnkClearSpatialContexts.Name = "lnkClearSpatialContexts";
            this.lnkClearSpatialContexts.Size = new System.Drawing.Size(78, 13);
            this.lnkClearSpatialContexts.TabIndex = 3;
            this.lnkClearSpatialContexts.TabStop = true;
            this.lnkClearSpatialContexts.Text = "Clear Selection";
            this.lnkClearSpatialContexts.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearSpatialContexts_LinkClicked);
            // 
            // lstSpatialContexts
            // 
            this.lstSpatialContexts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSpatialContexts.DisplayMember = "Name";
            this.lstSpatialContexts.FormattingEnabled = true;
            this.lstSpatialContexts.Location = new System.Drawing.Point(7, 19);
            this.lstSpatialContexts.Name = "lstSpatialContexts";
            this.lstSpatialContexts.Size = new System.Drawing.Size(433, 56);
            this.lstSpatialContexts.TabIndex = 2;
            this.lstSpatialContexts.SelectedIndexChanged += new System.EventHandler(this.lstSpatialContexts_SelectedIndexChanged);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(305, 288);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(387, 288);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ImportElementsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 323);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "ImportElementsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Elements";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel lnkClearSchemas;
        private System.Windows.Forms.ListBox lstFeatureSchemas;
        private System.Windows.Forms.LinkLabel lnkClearSpatialContexts;
        private System.Windows.Forms.ListBox lstSpatialContexts;
        private System.Windows.Forms.Label lblClassImport;
    }
}