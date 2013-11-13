namespace FdoToolbox.Express.Controls
{
    partial class CopySpatialContextsCtl
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstSpatialContexts = new System.Windows.Forms.ListBox();
            this.cmbTargetConn = new System.Windows.Forms.ComboBox();
            this.lblSrcConn = new System.Windows.Forms.Label();
            this.chkOverwrite = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Connection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Target Connection";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Spatial Contexts to copy";
            // 
            // lstSpatialContexts
            // 
            this.lstSpatialContexts.FormattingEnabled = true;
            this.lstSpatialContexts.Location = new System.Drawing.Point(19, 96);
            this.lstSpatialContexts.Name = "lstSpatialContexts";
            this.lstSpatialContexts.Size = new System.Drawing.Size(269, 134);
            this.lstSpatialContexts.TabIndex = 3;
            // 
            // cmbTargetConn
            // 
            this.cmbTargetConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetConn.FormattingEnabled = true;
            this.cmbTargetConn.Location = new System.Drawing.Point(127, 41);
            this.cmbTargetConn.Name = "cmbTargetConn";
            this.cmbTargetConn.Size = new System.Drawing.Size(161, 21);
            this.cmbTargetConn.TabIndex = 4;
            this.cmbTargetConn.SelectedIndexChanged += new System.EventHandler(this.cmbTargetConn_SelectedIndexChanged);
            // 
            // lblSrcConn
            // 
            this.lblSrcConn.AutoSize = true;
            this.lblSrcConn.Location = new System.Drawing.Point(124, 19);
            this.lblSrcConn.Name = "lblSrcConn";
            this.lblSrcConn.Size = new System.Drawing.Size(0, 13);
            this.lblSrcConn.TabIndex = 5;
            // 
            // chkOverwrite
            // 
            this.chkOverwrite.AutoSize = true;
            this.chkOverwrite.Location = new System.Drawing.Point(19, 237);
            this.chkOverwrite.Name = "chkOverwrite";
            this.chkOverwrite.Size = new System.Drawing.Size(276, 17);
            this.chkOverwrite.TabIndex = 6;
            this.chkOverwrite.Text = "Overwrite Existing Spatial Contexts of the same name";
            this.chkOverwrite.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(127, 275);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "Copy";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(213, 275);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CopySpatialContextsCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkOverwrite);
            this.Controls.Add(this.lblSrcConn);
            this.Controls.Add(this.cmbTargetConn);
            this.Controls.Add(this.lstSpatialContexts);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CopySpatialContextsCtl";
            this.Size = new System.Drawing.Size(310, 313);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstSpatialContexts;
        private System.Windows.Forms.ComboBox cmbTargetConn;
        private System.Windows.Forms.Label lblSrcConn;
        private System.Windows.Forms.CheckBox chkOverwrite;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
