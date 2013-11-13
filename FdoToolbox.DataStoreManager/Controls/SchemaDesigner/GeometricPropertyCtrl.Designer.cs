namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    partial class GeometricPropertyCtrl
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
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkElevation = new System.Windows.Forms.CheckBox();
            this.chkMeasure = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSpatialContext = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkGeometryTypes = new FdoToolbox.DataStoreManager.Controls.SchemaDesigner.GeometryTypeCtl();
            this.SuspendLayout();
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(10, 68);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(188, 20);
            this.txtDescription.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Description";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(10, 25);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(188, 20);
            this.txtName.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name";
            // 
            // chkElevation
            // 
            this.chkElevation.AutoSize = true;
            this.chkElevation.Location = new System.Drawing.Point(10, 94);
            this.chkElevation.Name = "chkElevation";
            this.chkElevation.Size = new System.Drawing.Size(92, 17);
            this.chkElevation.TabIndex = 8;
            this.chkElevation.Text = "Has Elevation";
            this.chkElevation.UseVisualStyleBackColor = true;
            // 
            // chkMeasure
            // 
            this.chkMeasure.AutoSize = true;
            this.chkMeasure.Location = new System.Drawing.Point(10, 117);
            this.chkMeasure.Name = "chkMeasure";
            this.chkMeasure.Size = new System.Drawing.Size(89, 17);
            this.chkMeasure.TabIndex = 9;
            this.chkMeasure.Text = "Has Measure";
            this.chkMeasure.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Spatial Context Association";
            // 
            // cmbSpatialContext
            // 
            this.cmbSpatialContext.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSpatialContext.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpatialContext.FormattingEnabled = true;
            this.cmbSpatialContext.Location = new System.Drawing.Point(10, 157);
            this.cmbSpatialContext.Name = "cmbSpatialContext";
            this.cmbSpatialContext.Size = new System.Drawing.Size(188, 21);
            this.cmbSpatialContext.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Geometry Types";
            // 
            // chkGeometryTypes
            // 
            this.chkGeometryTypes.CheckOnClick = true;
            this.chkGeometryTypes.FormattingEnabled = true;
            this.chkGeometryTypes.GeometryTypes = 0;
            this.chkGeometryTypes.Items.AddRange(new object[] {
            OSGeo.FDO.Schema.GeometricType.GeometricType_Point,
            OSGeo.FDO.Schema.GeometricType.GeometricType_Curve,
            OSGeo.FDO.Schema.GeometricType.GeometricType_Surface,
            OSGeo.FDO.Schema.GeometricType.GeometricType_Solid});
            this.chkGeometryTypes.Location = new System.Drawing.Point(10, 206);
            this.chkGeometryTypes.Name = "chkGeometryTypes";
            this.chkGeometryTypes.Size = new System.Drawing.Size(188, 64);
            this.chkGeometryTypes.TabIndex = 13;
            // 
            // GeometricPropertyCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.chkGeometryTypes);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbSpatialContext);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkMeasure);
            this.Controls.Add(this.chkElevation);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Name = "GeometricPropertyCtrl";
            this.Size = new System.Drawing.Size(212, 280);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkElevation;
        private System.Windows.Forms.CheckBox chkMeasure;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbSpatialContext;
        private System.Windows.Forms.Label label4;
        private GeometryTypeCtl chkGeometryTypes;
    }
}
