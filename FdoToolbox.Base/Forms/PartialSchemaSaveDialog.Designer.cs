namespace FdoToolbox.Base.Forms
{
    partial class PartialSchemaSaveDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartialSchemaSaveDialog));
            this.rdXml = new System.Windows.Forms.RadioButton();
            this.rdFile = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeSchema = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtXml = new System.Windows.Forms.TextBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnBrowseXml = new System.Windows.Forms.Button();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnCheckNone = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdXml
            // 
            this.rdXml.AutoSize = true;
            this.rdXml.Checked = true;
            this.rdXml.Location = new System.Drawing.Point(24, 26);
            this.rdXml.Name = "rdXml";
            this.rdXml.Size = new System.Drawing.Size(62, 17);
            this.rdXml.TabIndex = 0;
            this.rdXml.TabStop = true;
            this.rdXml.Text = "As XML";
            this.rdXml.UseVisualStyleBackColor = true;
            this.rdXml.CheckedChanged += new System.EventHandler(this.rdXml_CheckedChanged);
            // 
            // rdFile
            // 
            this.rdFile.AutoSize = true;
            this.rdFile.Location = new System.Drawing.Point(24, 49);
            this.rdFile.Name = "rdFile";
            this.rdFile.Size = new System.Drawing.Size(56, 17);
            this.rdFile.TabIndex = 1;
            this.rdFile.Text = "As File";
            this.rdFile.UseVisualStyleBackColor = true;
            this.rdFile.CheckedChanged += new System.EventHandler(this.rdFile_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.treeSchema);
            this.groupBox1.Location = new System.Drawing.Point(13, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(525, 297);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Schema";
            // 
            // treeSchema
            // 
            this.treeSchema.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeSchema.CheckBoxes = true;
            this.treeSchema.ImageIndex = 0;
            this.treeSchema.ImageList = this.imageList1;
            this.treeSchema.Location = new System.Drawing.Point(11, 20);
            this.treeSchema.Name = "treeSchema";
            this.treeSchema.SelectedImageIndex = 0;
            this.treeSchema.Size = new System.Drawing.Size(512, 271);
            this.treeSchema.TabIndex = 0;
            this.treeSchema.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeSchema_AfterCheck);
            this.treeSchema.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeSchema_BeforeCheck);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "database_table.png");
            this.imageList1.Images.SetKeyName(1, "feature_class.png");
            this.imageList1.Images.SetKeyName(2, "key.png");
            this.imageList1.Images.SetKeyName(3, "table.png");
            this.imageList1.Images.SetKeyName(4, "shape_handles.png");
            this.imageList1.Images.SetKeyName(5, "package.png");
            this.imageList1.Images.SetKeyName(6, "table_relationship.png");
            this.imageList1.Images.SetKeyName(7, "image.png");
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(381, 388);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(463, 388);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtXml
            // 
            this.txtXml.Location = new System.Drawing.Point(102, 25);
            this.txtXml.Name = "txtXml";
            this.txtXml.ReadOnly = true;
            this.txtXml.Size = new System.Drawing.Size(399, 20);
            this.txtXml.TabIndex = 5;
            this.txtXml.TextChanged += new System.EventHandler(this.txtXml_TextChanged);
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(102, 48);
            this.txtFile.Name = "txtFile";
            this.txtFile.ReadOnly = true;
            this.txtFile.Size = new System.Drawing.Size(399, 20);
            this.txtFile.TabIndex = 6;
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            // 
            // btnBrowseXml
            // 
            this.btnBrowseXml.Location = new System.Drawing.Point(507, 23);
            this.btnBrowseXml.Name = "btnBrowseXml";
            this.btnBrowseXml.Size = new System.Drawing.Size(31, 23);
            this.btnBrowseXml.TabIndex = 7;
            this.btnBrowseXml.Text = "...";
            this.btnBrowseXml.UseVisualStyleBackColor = true;
            this.btnBrowseXml.Click += new System.EventHandler(this.btnBrowseXml_Click);
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Location = new System.Drawing.Point(507, 46);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(31, 23);
            this.btnBrowseFile.TabIndex = 8;
            this.btnBrowseFile.Text = "...";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(13, 388);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnCheckAll.TabIndex = 9;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnCheckNone
            // 
            this.btnCheckNone.Location = new System.Drawing.Point(94, 388);
            this.btnCheckNone.Name = "btnCheckNone";
            this.btnCheckNone.Size = new System.Drawing.Size(75, 23);
            this.btnCheckNone.TabIndex = 10;
            this.btnCheckNone.Text = "Check None";
            this.btnCheckNone.UseVisualStyleBackColor = true;
            this.btnCheckNone.Click += new System.EventHandler(this.btnCheckNone_Click);
            // 
            // PartialSchemaSaveDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 423);
            this.ControlBox = false;
            this.Controls.Add(this.btnCheckNone);
            this.Controls.Add(this.btnCheckAll);
            this.Controls.Add(this.btnBrowseFile);
            this.Controls.Add(this.btnBrowseXml);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.txtXml);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rdFile);
            this.Controls.Add(this.rdXml);
            this.Name = "PartialSchemaSaveDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save Schema";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdXml;
        private System.Windows.Forms.RadioButton rdFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeSchema;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtXml;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnBrowseXml;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnCheckNone;
    }
}