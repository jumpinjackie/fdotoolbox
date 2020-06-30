﻿namespace FdoToolbox.Express.Controls
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
            this.txtSourceCs = new System.Windows.Forms.TextBox();
            this.txtTargetCs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPickCS = new System.Windows.Forms.Button();
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
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "File Path";
            // 
            // txtSavePath
            // 
            this.txtSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSavePath.Location = new System.Drawing.Point(100, 41);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(246, 20);
            this.txtSavePath.TabIndex = 4;
            this.txtSavePath.TextChanged += new System.EventHandler(this.txtSavePath_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(352, 39);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(271, 195);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(352, 195);
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
            this.label3.Location = new System.Drawing.Point(16, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Coordinate System";
            // 
            // txtSourceCs
            // 
            this.txtSourceCs.Location = new System.Drawing.Point(122, 76);
            this.txtSourceCs.Multiline = true;
            this.txtSourceCs.Name = "txtSourceCs";
            this.txtSourceCs.ReadOnly = true;
            this.txtSourceCs.Size = new System.Drawing.Size(305, 48);
            this.txtSourceCs.TabIndex = 9;
            // 
            // txtTargetCs
            // 
            this.txtTargetCs.Location = new System.Drawing.Point(122, 141);
            this.txtTargetCs.Multiline = true;
            this.txtTargetCs.Name = "txtTargetCs";
            this.txtTargetCs.ReadOnly = true;
            this.txtTargetCs.Size = new System.Drawing.Size(305, 48);
            this.txtTargetCs.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Transform To";
            // 
            // btnPickCS
            // 
            this.btnPickCS.Location = new System.Drawing.Point(19, 166);
            this.btnPickCS.Name = "btnPickCS";
            this.btnPickCS.Size = new System.Drawing.Size(92, 23);
            this.btnPickCS.TabIndex = 12;
            this.btnPickCS.Text = "Pick CS";
            this.btnPickCS.UseVisualStyleBackColor = true;
            this.btnPickCS.Click += new System.EventHandler(this.btnPickCS_Click);
            // 
            // DumpFeatureClassCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnPickCS);
            this.Controls.Add(this.txtTargetCs);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSourceCs);
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
            this.Size = new System.Drawing.Size(442, 225);
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
        private System.Windows.Forms.TextBox txtSourceCs;
        private System.Windows.Forms.TextBox txtTargetCs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnPickCS;
    }
}
