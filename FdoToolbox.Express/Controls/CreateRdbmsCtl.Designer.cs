namespace FdoToolbox.Express.Controls
{
    partial class CreateRdbmsCtl
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkFdoMetadata = new System.Windows.Forms.CheckBox();
            this.txtTolerance = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCSWkt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCSName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkAlterSchema = new System.Windows.Forms.CheckBox();
            this.chkConnect = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbExtentType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtUpperRightY = new System.Windows.Forms.TextBox();
            this.txtUpperRightX = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtLowerLeftY = new System.Windows.Forms.TextBox();
            this.txtLowerLeftX = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtSchemaFile = new System.Windows.Forms.TextBox();
            this.lblConfig = new System.Windows.Forms.Label();
            this.txtConnectionName = new System.Windows.Forms.TextBox();
            this.lblConnectionName = new System.Windows.Forms.Label();
            this.lblDataStore = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtService = new System.Windows.Forms.TextBox();
            this.lblService = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnPickCS = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnPickCS);
            this.groupBox2.Controls.Add(this.chkFdoMetadata);
            this.groupBox2.Controls.Add(this.txtTolerance);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtCSWkt);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtCSName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.chkAlterSchema);
            this.groupBox2.Controls.Add(this.chkConnect);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.btnBrowse);
            this.groupBox2.Controls.Add(this.txtSchemaFile);
            this.groupBox2.Controls.Add(this.lblConfig);
            this.groupBox2.Controls.Add(this.txtConnectionName);
            this.groupBox2.Controls.Add(this.lblConnectionName);
            this.groupBox2.Controls.Add(this.lblDataStore);
            this.groupBox2.Location = new System.Drawing.Point(3, 117);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(381, 387);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Store Options";
            // 
            // chkFdoMetadata
            // 
            this.chkFdoMetadata.AutoSize = true;
            this.chkFdoMetadata.Location = new System.Drawing.Point(20, 147);
            this.chkFdoMetadata.Name = "chkFdoMetadata";
            this.chkFdoMetadata.Size = new System.Drawing.Size(182, 17);
            this.chkFdoMetadata.TabIndex = 4;
            this.chkFdoMetadata.Text = "Create with FDO metadata tables";
            this.chkFdoMetadata.UseVisualStyleBackColor = true;
            // 
            // txtTolerance
            // 
            this.txtTolerance.Location = new System.Drawing.Point(120, 122);
            this.txtTolerance.Name = "txtTolerance";
            this.txtTolerance.Size = new System.Drawing.Size(242, 20);
            this.txtTolerance.TabIndex = 3;
            this.txtTolerance.Text = "0.0001";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Tolerance";
            // 
            // txtCSWkt
            // 
            this.txtCSWkt.Location = new System.Drawing.Point(120, 69);
            this.txtCSWkt.Multiline = true;
            this.txtCSWkt.Name = "txtCSWkt";
            this.txtCSWkt.Size = new System.Drawing.Size(242, 47);
            this.txtCSWkt.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Coord Sys WKT";
            // 
            // txtCSName
            // 
            this.txtCSName.Location = new System.Drawing.Point(120, 43);
            this.txtCSName.Name = "txtCSName";
            this.txtCSName.Size = new System.Drawing.Size(242, 20);
            this.txtCSName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Coord Sys Name";
            // 
            // chkAlterSchema
            // 
            this.chkAlterSchema.AutoSize = true;
            this.chkAlterSchema.Location = new System.Drawing.Point(214, 310);
            this.chkAlterSchema.Name = "chkAlterSchema";
            this.chkAlterSchema.Size = new System.Drawing.Size(155, 17);
            this.chkAlterSchema.TabIndex = 6;
            this.chkAlterSchema.Text = "Fix schema incompatibilities";
            this.chkAlterSchema.UseVisualStyleBackColor = true;
            // 
            // chkConnect
            // 
            this.chkConnect.AutoSize = true;
            this.chkConnect.Location = new System.Drawing.Point(20, 310);
            this.chkConnect.Name = "chkConnect";
            this.chkConnect.Size = new System.Drawing.Size(192, 17);
            this.chkConnect.TabIndex = 5;
            this.chkConnect.Text = "Once created, create a connection";
            this.chkConnect.UseVisualStyleBackColor = true;
            this.chkConnect.CheckedChanged += new System.EventHandler(this.chkConnect_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cmbExtentType);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Location = new System.Drawing.Point(6, 173);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(369, 131);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data Store Extents";
            // 
            // cmbExtentType
            // 
            this.cmbExtentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExtentType.FormattingEnabled = true;
            this.cmbExtentType.Location = new System.Drawing.Point(114, 26);
            this.cmbExtentType.Name = "cmbExtentType";
            this.cmbExtentType.Size = new System.Drawing.Size(242, 21);
            this.cmbExtentType.TabIndex = 0;
            this.cmbExtentType.SelectedIndexChanged += new System.EventHandler(this.cmbExtentType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Extent Type";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.txtUpperRightY);
            this.groupBox4.Controls.Add(this.txtUpperRightX);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Location = new System.Drawing.Point(187, 54);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(169, 71);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Upper Right";
            // 
            // txtUpperRightY
            // 
            this.txtUpperRightY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUpperRightY.Location = new System.Drawing.Point(71, 45);
            this.txtUpperRightY.Name = "txtUpperRightY";
            this.txtUpperRightY.Size = new System.Drawing.Size(83, 20);
            this.txtUpperRightY.TabIndex = 1;
            this.txtUpperRightY.Tag = "";
            this.txtUpperRightY.Text = "0";
            // 
            // txtUpperRightX
            // 
            this.txtUpperRightX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUpperRightX.Location = new System.Drawing.Point(71, 19);
            this.txtUpperRightX.Name = "txtUpperRightX";
            this.txtUpperRightX.Size = new System.Drawing.Size(83, 20);
            this.txtUpperRightX.TabIndex = 0;
            this.txtUpperRightX.Tag = "";
            this.txtUpperRightX.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "X/Long";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Y/Lat";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.txtLowerLeftY);
            this.groupBox5.Controls.Add(this.txtLowerLeftX);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(6, 54);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(175, 71);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Lower Left";
            // 
            // txtLowerLeftY
            // 
            this.txtLowerLeftY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLowerLeftY.Location = new System.Drawing.Point(77, 45);
            this.txtLowerLeftY.Name = "txtLowerLeftY";
            this.txtLowerLeftY.Size = new System.Drawing.Size(82, 20);
            this.txtLowerLeftY.TabIndex = 1;
            this.txtLowerLeftY.Tag = "";
            this.txtLowerLeftY.Text = "0";
            // 
            // txtLowerLeftX
            // 
            this.txtLowerLeftX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLowerLeftX.Location = new System.Drawing.Point(77, 19);
            this.txtLowerLeftX.Name = "txtLowerLeftX";
            this.txtLowerLeftX.Size = new System.Drawing.Size(82, 20);
            this.txtLowerLeftX.TabIndex = 0;
            this.txtLowerLeftX.Tag = "";
            this.txtLowerLeftX.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Y/Lat";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "X/Long";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(120, 17);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(242, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(334, 333);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(28, 23);
            this.btnBrowse.TabIndex = 8;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtSchemaFile
            // 
            this.txtSchemaFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSchemaFile.Location = new System.Drawing.Point(120, 335);
            this.txtSchemaFile.Name = "txtSchemaFile";
            this.txtSchemaFile.Size = new System.Drawing.Size(208, 20);
            this.txtSchemaFile.TabIndex = 7;
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Location = new System.Drawing.Point(17, 338);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(101, 13);
            this.lblConfig.TabIndex = 4;
            this.lblConfig.Text = "Feature Schema file";
            // 
            // txtConnectionName
            // 
            this.txtConnectionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnectionName.Enabled = false;
            this.txtConnectionName.Location = new System.Drawing.Point(120, 361);
            this.txtConnectionName.Name = "txtConnectionName";
            this.txtConnectionName.Size = new System.Drawing.Size(243, 20);
            this.txtConnectionName.TabIndex = 9;
            this.txtConnectionName.TextChanged += new System.EventHandler(this.txtConnectionName_TextChanged);
            // 
            // lblConnectionName
            // 
            this.lblConnectionName.AutoSize = true;
            this.lblConnectionName.Location = new System.Drawing.Point(17, 364);
            this.lblConnectionName.Name = "lblConnectionName";
            this.lblConnectionName.Size = new System.Drawing.Size(92, 13);
            this.lblConnectionName.TabIndex = 2;
            this.lblConnectionName.Text = "Connection Name";
            // 
            // lblDataStore
            // 
            this.lblDataStore.AutoSize = true;
            this.lblDataStore.Location = new System.Drawing.Point(17, 20);
            this.lblDataStore.Name = "lblDataStore";
            this.lblDataStore.Size = new System.Drawing.Size(35, 13);
            this.lblDataStore.TabIndex = 0;
            this.lblDataStore.Text = "Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.txtService);
            this.groupBox1.Controls.Add(this.lblService);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Controls.Add(this.lblUsername);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 108);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection properties";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(306, 72);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(56, 23);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(120, 74);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(180, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(120, 48);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(242, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // txtService
            // 
            this.txtService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtService.Location = new System.Drawing.Point(120, 22);
            this.txtService.Name = "txtService";
            this.txtService.Size = new System.Drawing.Size(242, 20);
            this.txtService.TabIndex = 0;
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Location = new System.Drawing.Point(17, 25);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(43, 13);
            this.lblService.TabIndex = 0;
            this.lblService.Text = "Service";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(17, 77);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(17, 51);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(309, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(228, 510);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnPickCS
            // 
            this.btnPickCS.Location = new System.Drawing.Point(20, 93);
            this.btnPickCS.Name = "btnPickCS";
            this.btnPickCS.Size = new System.Drawing.Size(75, 23);
            this.btnPickCS.TabIndex = 20;
            this.btnPickCS.Text = "Pick CS";
            this.btnPickCS.UseVisualStyleBackColor = true;
            this.btnPickCS.Click += new System.EventHandler(this.btnPickCS_Click);
            // 
            // CreateRdbmsCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "CreateRdbmsCtl";
            this.Size = new System.Drawing.Size(387, 537);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtSchemaFile;
        protected System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.TextBox txtConnectionName;
        protected System.Windows.Forms.Label lblConnectionName;
        protected System.Windows.Forms.Label lblDataStore;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtService;
        protected System.Windows.Forms.Label lblService;
        protected System.Windows.Forms.Label lblPassword;
        protected System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtUpperRightY;
        private System.Windows.Forms.TextBox txtUpperRightX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtLowerLeftY;
        private System.Windows.Forms.TextBox txtLowerLeftX;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkAlterSchema;
        private System.Windows.Forms.CheckBox chkConnect;
        protected System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCSName;
        protected System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbExtentType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkFdoMetadata;
        private System.Windows.Forms.TextBox txtTolerance;
        private System.Windows.Forms.Label label1;
        protected System.Windows.Forms.TextBox txtCSWkt;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnPickCS;
    }
}
