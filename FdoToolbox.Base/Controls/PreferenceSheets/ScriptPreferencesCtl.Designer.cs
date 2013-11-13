namespace FdoToolbox.Base.Controls.PreferenceSheets
{
    partial class ScriptPreferencesCtl
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
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstModulePaths = new System.Windows.Forms.ListBox();
            this.btnAddPath = new System.Windows.Forms.Button();
            this.btnDeletePath = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enable Debugging";
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(151, 26);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(15, 14);
            this.chkDebug.TabIndex = 1;
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Script Module Paths";
            // 
            // lstModulePaths
            // 
            this.lstModulePaths.FormattingEnabled = true;
            this.lstModulePaths.Location = new System.Drawing.Point(151, 60);
            this.lstModulePaths.Name = "lstModulePaths";
            this.lstModulePaths.Size = new System.Drawing.Size(254, 95);
            this.lstModulePaths.TabIndex = 3;
            this.lstModulePaths.SelectedIndexChanged += new System.EventHandler(this.lstModulePaths_SelectedIndexChanged);
            // 
            // btnAddPath
            // 
            this.btnAddPath.Location = new System.Drawing.Point(151, 162);
            this.btnAddPath.Name = "btnAddPath";
            this.btnAddPath.Size = new System.Drawing.Size(75, 23);
            this.btnAddPath.TabIndex = 4;
            this.btnAddPath.Text = "Add";
            this.btnAddPath.UseVisualStyleBackColor = true;
            this.btnAddPath.Click += new System.EventHandler(this.btnAddPath_Click);
            // 
            // btnDeletePath
            // 
            this.btnDeletePath.Enabled = false;
            this.btnDeletePath.Location = new System.Drawing.Point(232, 162);
            this.btnDeletePath.Name = "btnDeletePath";
            this.btnDeletePath.Size = new System.Drawing.Size(75, 23);
            this.btnDeletePath.TabIndex = 5;
            this.btnDeletePath.Text = "Delete";
            this.btnDeletePath.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 285);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(341, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Note: You will have to restart this application for changes to take effect";
            // 
            // ScriptPreferencesCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDeletePath);
            this.Controls.Add(this.btnAddPath);
            this.Controls.Add(this.lstModulePaths);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkDebug);
            this.Controls.Add(this.label1);
            this.Name = "ScriptPreferencesCtl";
            this.Size = new System.Drawing.Size(560, 326);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstModulePaths;
        private System.Windows.Forms.Button btnAddPath;
        private System.Windows.Forms.Button btnDeletePath;
        private System.Windows.Forms.Label label3;
    }
}
