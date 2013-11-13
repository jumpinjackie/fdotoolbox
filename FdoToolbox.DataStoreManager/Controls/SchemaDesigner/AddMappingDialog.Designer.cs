namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    partial class AddMappingDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblClass = new System.Windows.Forms.Label();
            this.lblAssocClass = new System.Windows.Forms.Label();
            this.cmbActiveProperty = new System.Windows.Forms.ComboBox();
            this.cmbAssociatedProperty = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Class";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(282, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Associated Class";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Class Property";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(282, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Associated Class Property";
            // 
            // lblClass
            // 
            this.lblClass.AutoEllipsis = true;
            this.lblClass.Location = new System.Drawing.Point(98, 13);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(178, 23);
            this.lblClass.TabIndex = 4;
            this.lblClass.Text = "label5";
            // 
            // lblAssocClass
            // 
            this.lblAssocClass.AutoEllipsis = true;
            this.lblAssocClass.Location = new System.Drawing.Point(422, 13);
            this.lblAssocClass.Name = "lblAssocClass";
            this.lblAssocClass.Size = new System.Drawing.Size(185, 23);
            this.lblAssocClass.TabIndex = 5;
            this.lblAssocClass.Text = "label6";
            // 
            // cmbActiveProperty
            // 
            this.cmbActiveProperty.DisplayMember = "Name";
            this.cmbActiveProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActiveProperty.FormattingEnabled = true;
            this.cmbActiveProperty.Location = new System.Drawing.Point(101, 51);
            this.cmbActiveProperty.Name = "cmbActiveProperty";
            this.cmbActiveProperty.Size = new System.Drawing.Size(175, 21);
            this.cmbActiveProperty.TabIndex = 6;
            this.cmbActiveProperty.SelectedIndexChanged += new System.EventHandler(this.cmbActiveProperty_SelectedIndexChanged);
            // 
            // cmbAssociatedProperty
            // 
            this.cmbAssociatedProperty.DisplayMember = "Name";
            this.cmbAssociatedProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAssociatedProperty.FormattingEnabled = true;
            this.cmbAssociatedProperty.Location = new System.Drawing.Point(425, 51);
            this.cmbAssociatedProperty.Name = "cmbAssociatedProperty";
            this.cmbAssociatedProperty.Size = new System.Drawing.Size(182, 21);
            this.cmbAssociatedProperty.TabIndex = 7;
            this.cmbAssociatedProperty.SelectedIndexChanged += new System.EventHandler(this.cmbAssociatedProperty_SelectedIndexChanged);
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(451, 93);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(532, 93);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AddMappingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 128);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cmbAssociatedProperty);
            this.Controls.Add(this.cmbActiveProperty);
            this.Controls.Add(this.lblAssocClass);
            this.Controls.Add(this.lblClass);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AddMappingDialog";
            this.Text = "Add Property Mapping";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.Label lblAssocClass;
        private System.Windows.Forms.ComboBox cmbActiveProperty;
        private System.Windows.Forms.ComboBox cmbAssociatedProperty;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}