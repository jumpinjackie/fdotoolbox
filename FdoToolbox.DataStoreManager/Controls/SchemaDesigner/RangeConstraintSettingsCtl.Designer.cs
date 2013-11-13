namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    partial class RangeConstraintSettingsCtl
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
            this.chkMinInclusive = new System.Windows.Forms.CheckBox();
            this.chkMaxInclusive = new System.Windows.Forms.CheckBox();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Max Value";
            // 
            // chkMinInclusive
            // 
            this.chkMinInclusive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMinInclusive.AutoSize = true;
            this.chkMinInclusive.Location = new System.Drawing.Point(218, 13);
            this.chkMinInclusive.Name = "chkMinInclusive";
            this.chkMinInclusive.Size = new System.Drawing.Size(68, 17);
            this.chkMinInclusive.TabIndex = 4;
            this.chkMinInclusive.Text = "Inclusive";
            this.chkMinInclusive.UseVisualStyleBackColor = true;
            // 
            // chkMaxInclusive
            // 
            this.chkMaxInclusive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMaxInclusive.AutoSize = true;
            this.chkMaxInclusive.Location = new System.Drawing.Point(218, 39);
            this.chkMaxInclusive.Name = "chkMaxInclusive";
            this.chkMaxInclusive.Size = new System.Drawing.Size(68, 17);
            this.chkMaxInclusive.TabIndex = 5;
            this.chkMaxInclusive.Text = "Inclusive";
            this.chkMaxInclusive.UseVisualStyleBackColor = true;
            // 
            // txtMinValue
            // 
            this.txtMinValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinValue.Location = new System.Drawing.Point(71, 10);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(141, 20);
            this.txtMinValue.TabIndex = 6;
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxValue.Location = new System.Drawing.Point(71, 37);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(141, 20);
            this.txtMaxValue.TabIndex = 7;
            // 
            // RangeConstraintSettingsCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtMaxValue);
            this.Controls.Add(this.txtMinValue);
            this.Controls.Add(this.chkMaxInclusive);
            this.Controls.Add(this.chkMinInclusive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "RangeConstraintSettingsCtl";
            this.Size = new System.Drawing.Size(289, 71);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkMinInclusive;
        private System.Windows.Forms.CheckBox chkMaxInclusive;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.TextBox txtMaxValue;
    }
}
