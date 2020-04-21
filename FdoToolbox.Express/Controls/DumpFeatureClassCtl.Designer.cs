namespace FdoToolbox.Express.Controls
{
    partial class DumpFeatureClassCtl
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
            this.label1 = new System.Windows.Forms.Label();
            this.rdSdf = new System.Windows.Forms.RadioButton();
            this.rdSqlite = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTargetCsWkt = new System.Windows.Forms.TextBox();
            this.btnPickTargetCs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dump to:";
            // 
            // rdSdf
            // 
            this.rdSdf.AutoSize = true;
            this.rdSdf.Checked = true;
            this.rdSdf.Location = new System.Drawing.Point(100, 18);
            this.rdSdf.Name = "rdSdf";
            this.rdSdf.Size = new System.Drawing.Size(46, 17);
            this.rdSdf.TabIndex = 1;
            this.rdSdf.TabStop = true;
            this.rdSdf.Text = "SDF";
            this.rdSdf.UseVisualStyleBackColor = true;
            // 
            // rdSqlite
            // 
            this.rdSqlite.AutoSize = true;
            this.rdSqlite.Location = new System.Drawing.Point(165, 18);
            this.rdSqlite.Name = "rdSqlite";
            this.rdSqlite.Size = new System.Drawing.Size(57, 17);
            this.rdSqlite.TabIndex = 2;
            this.rdSqlite.TabStop = true;
            this.rdSqlite.Text = "SQLite";
            this.rdSqlite.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "File Path";
            // 
            // txtSavePath
            // 
            this.txtSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSavePath.Location = new System.Drawing.Point(102, 73);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(246, 20);
            this.txtSavePath.TabIndex = 4;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(354, 71);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(273, 100);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(354, 100);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Transform to";
            // 
            // txtTargetCsWkt
            // 
            this.txtTargetCsWkt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetCsWkt.Location = new System.Drawing.Point(100, 45);
            this.txtTargetCsWkt.Name = "txtTargetCsWkt";
            this.txtTargetCsWkt.ReadOnly = true;
            this.txtTargetCsWkt.Size = new System.Drawing.Size(296, 20);
            this.txtTargetCsWkt.TabIndex = 9;
            // 
            // btnPickTargetCs
            // 
            this.btnPickTargetCs.Location = new System.Drawing.Point(402, 43);
            this.btnPickTargetCs.Name = "btnPickTargetCs";
            this.btnPickTargetCs.Size = new System.Drawing.Size(27, 23);
            this.btnPickTargetCs.TabIndex = 10;
            this.btnPickTargetCs.Text = "...";
            this.btnPickTargetCs.UseVisualStyleBackColor = true;
            this.btnPickTargetCs.Click += new System.EventHandler(this.btnPickTargetCs_Click);
            // 
            // DumpFeatureClassCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnPickTargetCs);
            this.Controls.Add(this.txtTargetCsWkt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rdSqlite);
            this.Controls.Add(this.rdSdf);
            this.Controls.Add(this.label1);
            this.Name = "DumpFeatureClassCtl";
            this.Size = new System.Drawing.Size(442, 139);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdSdf;
        private System.Windows.Forms.RadioButton rdSqlite;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTargetCsWkt;
        private System.Windows.Forms.Button btnPickTargetCs;
    }
}
