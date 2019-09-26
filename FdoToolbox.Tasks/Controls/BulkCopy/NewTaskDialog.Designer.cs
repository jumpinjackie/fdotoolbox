namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    partial class NewTaskDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbSrcClass = new System.Windows.Forms.ComboBox();
            this.cmbSrcSchema = new System.Windows.Forms.ComboBox();
            this.cmbSrcConnection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkCreate = new System.Windows.Forms.CheckBox();
            this.cmbDstClass = new System.Windows.Forms.ComboBox();
            this.cmbDstConnection = new System.Windows.Forms.ComboBox();
            this.cmbDstSchema = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnGenerateName = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbSrcClass);
            this.groupBox1.Controls.Add(this.cmbSrcSchema);
            this.groupBox1.Controls.Add(this.cmbSrcConnection);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 98);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // cmbSrcClass
            // 
            this.cmbSrcClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSrcClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcClass.FormattingEnabled = true;
            this.cmbSrcClass.Location = new System.Drawing.Point(102, 67);
            this.cmbSrcClass.Name = "cmbSrcClass";
            this.cmbSrcClass.Size = new System.Drawing.Size(196, 21);
            this.cmbSrcClass.TabIndex = 5;
            this.cmbSrcClass.SelectionChangeCommitted += new System.EventHandler(this.cmbSrcClass_SelectionChangeCommitted);
            // 
            // cmbSrcSchema
            // 
            this.cmbSrcSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSrcSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcSchema.FormattingEnabled = true;
            this.cmbSrcSchema.Location = new System.Drawing.Point(102, 40);
            this.cmbSrcSchema.Name = "cmbSrcSchema";
            this.cmbSrcSchema.Size = new System.Drawing.Size(196, 21);
            this.cmbSrcSchema.TabIndex = 4;
            this.cmbSrcSchema.SelectionChangeCommitted += new System.EventHandler(this.cmbSrcSchema_SelectionChangeCommitted);
            // 
            // cmbSrcConnection
            // 
            this.cmbSrcConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSrcConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcConnection.FormattingEnabled = true;
            this.cmbSrcConnection.Location = new System.Drawing.Point(102, 13);
            this.cmbSrcConnection.Name = "cmbSrcConnection";
            this.cmbSrcConnection.Size = new System.Drawing.Size(196, 21);
            this.cmbSrcConnection.TabIndex = 3;
            this.cmbSrcConnection.SelectionChangeCommitted += new System.EventHandler(this.cmbSrcConnection_SelectionChangeCommitted);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Feature Class";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Schema";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Connection";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkCreate);
            this.groupBox2.Controls.Add(this.cmbDstClass);
            this.groupBox2.Controls.Add(this.cmbDstConnection);
            this.groupBox2.Controls.Add(this.cmbDstSchema);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(12, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(316, 134);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target";
            // 
            // chkCreate
            // 
            this.chkCreate.AutoSize = true;
            this.chkCreate.Location = new System.Drawing.Point(102, 100);
            this.chkCreate.Name = "chkCreate";
            this.chkCreate.Size = new System.Drawing.Size(171, 17);
            this.chkCreate.TabIndex = 12;
            this.chkCreate.Text = "Create class of the same name";
            this.chkCreate.UseVisualStyleBackColor = true;
            this.chkCreate.CheckedChanged += new System.EventHandler(this.chkCreate_CheckedChanged);
            // 
            // cmbDstClass
            // 
            this.cmbDstClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDstClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDstClass.FormattingEnabled = true;
            this.cmbDstClass.Location = new System.Drawing.Point(102, 73);
            this.cmbDstClass.Name = "cmbDstClass";
            this.cmbDstClass.Size = new System.Drawing.Size(196, 21);
            this.cmbDstClass.TabIndex = 11;
            this.cmbDstClass.SelectionChangeCommitted += new System.EventHandler(this.cmbDstClass_SelectionChangeCommitted);
            // 
            // cmbDstConnection
            // 
            this.cmbDstConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDstConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDstConnection.FormattingEnabled = true;
            this.cmbDstConnection.Location = new System.Drawing.Point(102, 19);
            this.cmbDstConnection.Name = "cmbDstConnection";
            this.cmbDstConnection.Size = new System.Drawing.Size(196, 21);
            this.cmbDstConnection.TabIndex = 9;
            this.cmbDstConnection.SelectionChangeCommitted += new System.EventHandler(this.cmbDstConnection_SelectionChangeCommitted);
            // 
            // cmbDstSchema
            // 
            this.cmbDstSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDstSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDstSchema.FormattingEnabled = true;
            this.cmbDstSchema.Location = new System.Drawing.Point(102, 46);
            this.cmbDstSchema.Name = "cmbDstSchema";
            this.cmbDstSchema.Size = new System.Drawing.Size(196, 21);
            this.cmbDstSchema.TabIndex = 10;
            this.cmbDstSchema.SelectionChangeCommitted += new System.EventHandler(this.cmbDstSchema_SelectionChangeCommitted);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Connection";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Schema";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Feature Class";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(172, 285);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(253, 285);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(64, 15);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(264, 20);
            this.txtName.TabIndex = 5;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            this.txtName.Leave += new System.EventHandler(this.txtName_Leave);
            // 
            // btnGenerateName
            // 
            this.btnGenerateName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerateName.Location = new System.Drawing.Point(12, 285);
            this.btnGenerateName.Name = "btnGenerateName";
            this.btnGenerateName.Size = new System.Drawing.Size(101, 23);
            this.btnGenerateName.TabIndex = 6;
            this.btnGenerateName.Text = "Generate Name";
            this.btnGenerateName.UseVisualStyleBackColor = true;
            this.btnGenerateName.Click += new System.EventHandler(this.btnGenerateName_Click);
            // 
            // NewTaskDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(340, 321);
            this.ControlBox = false;
            this.Controls.Add(this.btnGenerateName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "NewTaskDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Task";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbSrcClass;
        private System.Windows.Forms.ComboBox cmbSrcSchema;
        private System.Windows.Forms.ComboBox cmbSrcConnection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDstClass;
        private System.Windows.Forms.ComboBox cmbDstConnection;
        private System.Windows.Forms.ComboBox cmbDstSchema;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.CheckBox chkCreate;
        private System.Windows.Forms.Button btnGenerateName;
    }
}