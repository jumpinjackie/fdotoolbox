namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    partial class SpatialContextOverridesDialog
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
            this.dgvSpatialContextOverrides = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SCOverride = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SCName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SCCoordSysName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SCWkt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSpatialContextOverrides)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSpatialContextOverrides
            // 
            this.dgvSpatialContextOverrides.AllowUserToAddRows = false;
            this.dgvSpatialContextOverrides.AllowUserToDeleteRows = false;
            this.dgvSpatialContextOverrides.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSpatialContextOverrides.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSpatialContextOverrides.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SCOverride,
            this.SCName,
            this.SCCoordSysName,
            this.SCWkt});
            this.dgvSpatialContextOverrides.Location = new System.Drawing.Point(12, 45);
            this.dgvSpatialContextOverrides.Name = "dgvSpatialContextOverrides";
            this.dgvSpatialContextOverrides.Size = new System.Drawing.Size(753, 203);
            this.dgvSpatialContextOverrides.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(735, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "The following spatial contexts were found on the source connection. Specify overr" +
    "ides by ticking the override column and specifying the replacement WKT";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(609, 254);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(690, 254);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SCOverride
            // 
            this.SCOverride.DataPropertyName = "Override";
            this.SCOverride.HeaderText = "Override?";
            this.SCOverride.Name = "SCOverride";
            // 
            // SCName
            // 
            this.SCName.DataPropertyName = "Name";
            this.SCName.HeaderText = "Name";
            this.SCName.Name = "SCName";
            this.SCName.Width = 150;
            // 
            // SCCoordSysName
            // 
            this.SCCoordSysName.DataPropertyName = "CsName";
            this.SCCoordSysName.HeaderText = "CS Name";
            this.SCCoordSysName.Name = "SCCoordSysName";
            this.SCCoordSysName.Width = 150;
            // 
            // SCWkt
            // 
            this.SCWkt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SCWkt.DataPropertyName = "WKT";
            this.SCWkt.HeaderText = "CS WKT";
            this.SCWkt.Name = "SCWkt";
            // 
            // SpatialContextOverridesDialog
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(777, 289);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvSpatialContextOverrides);
            this.Name = "SpatialContextOverridesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Spatial Context Overrides";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSpatialContextOverrides)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSpatialContextOverrides;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SCOverride;
        private System.Windows.Forms.DataGridViewTextBoxColumn SCName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SCCoordSysName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SCWkt;
    }
}