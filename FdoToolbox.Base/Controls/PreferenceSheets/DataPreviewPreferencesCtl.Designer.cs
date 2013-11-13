namespace FdoToolbox.Base.Controls.PreferenceSheets
{
    partial class DataPreviewPreferencesCtl
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
            this.numLimit = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.chkRandomTheme = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // numLimit
            // 
            this.numLimit.Location = new System.Drawing.Point(96, 18);
            this.numLimit.Name = "numLimit";
            this.numLimit.Size = new System.Drawing.Size(120, 20);
            this.numLimit.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Warning Limit";
            // 
            // chkRandomTheme
            // 
            this.chkRandomTheme.AutoSize = true;
            this.chkRandomTheme.Location = new System.Drawing.Point(12, 53);
            this.chkRandomTheme.Name = "chkRandomTheme";
            this.chkRandomTheme.Size = new System.Drawing.Size(175, 17);
            this.chkRandomTheme.TabIndex = 12;
            this.chkRandomTheme.Text = "Use Randomized Color Themes";
            this.chkRandomTheme.UseVisualStyleBackColor = true;
            // 
            // DataPreviewPreferencesCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkRandomTheme);
            this.Controls.Add(this.numLimit);
            this.Controls.Add(this.label5);
            this.Name = "DataPreviewPreferencesCtl";
            this.Size = new System.Drawing.Size(403, 150);
            ((System.ComponentModel.ISupportInitialize)(this.numLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numLimit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkRandomTheme;
    }
}
