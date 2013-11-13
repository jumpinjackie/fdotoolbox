namespace FdoToolbox.Base.Controls
{
    partial class FdoCapabilityViewer
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
            this.grdCaps = new System.Windows.Forms.DataGridView();
            this.COL_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COL_TYPE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COL_VALUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdCaps)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Capabilities";
            // 
            // grdCaps
            // 
            this.grdCaps.AllowUserToAddRows = false;
            this.grdCaps.AllowUserToDeleteRows = false;
            this.grdCaps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdCaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdCaps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.COL_NAME,
            this.COL_TYPE,
            this.COL_VALUE});
            this.grdCaps.Location = new System.Drawing.Point(23, 45);
            this.grdCaps.Name = "grdCaps";
            this.grdCaps.ReadOnly = true;
            this.grdCaps.RowHeadersVisible = false;
            this.grdCaps.Size = new System.Drawing.Size(534, 402);
            this.grdCaps.TabIndex = 1;
            // 
            // COL_NAME
            // 
            this.COL_NAME.DataPropertyName = "Name";
            this.COL_NAME.HeaderText = "Name";
            this.COL_NAME.Name = "COL_NAME";
            this.COL_NAME.ReadOnly = true;
            // 
            // COL_TYPE
            // 
            this.COL_TYPE.DataPropertyName = "Type";
            this.COL_TYPE.HeaderText = "Type";
            this.COL_TYPE.Name = "COL_TYPE";
            this.COL_TYPE.ReadOnly = true;
            // 
            // COL_VALUE
            // 
            this.COL_VALUE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.COL_VALUE.DataPropertyName = "Value";
            this.COL_VALUE.HeaderText = "Value";
            this.COL_VALUE.Name = "COL_VALUE";
            this.COL_VALUE.ReadOnly = true;
            // 
            // FdoCapabilityViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grdCaps);
            this.Controls.Add(this.label1);
            this.Name = "FdoCapabilityViewer";
            this.Size = new System.Drawing.Size(584, 476);
            ((System.ComponentModel.ISupportInitialize)(this.grdCaps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView grdCaps;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_TYPE;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_VALUE;
    }
}
