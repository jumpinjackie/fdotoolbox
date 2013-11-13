namespace FdoToolbox.Base.Controls
{
    partial class FdoJoinCriteriaCtl
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAddJoin = new System.Windows.Forms.Button();
            this.btnRemoveJoin = new System.Windows.Forms.Button();
            this.grpJoins = new System.Windows.Forms.GroupBox();
            this.lstJoins = new System.Windows.Forms.ListBox();
            this.txtClassAlias = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpJoins.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Enabled = false;
            this.btnEdit.Location = new System.Drawing.Point(323, 87);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 11;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAddJoin
            // 
            this.btnAddJoin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddJoin.Location = new System.Drawing.Point(323, 58);
            this.btnAddJoin.Name = "btnAddJoin";
            this.btnAddJoin.Size = new System.Drawing.Size(75, 23);
            this.btnAddJoin.TabIndex = 10;
            this.btnAddJoin.Text = "Add";
            this.btnAddJoin.UseVisualStyleBackColor = true;
            this.btnAddJoin.Click += new System.EventHandler(this.btnAddJoin_Click);
            // 
            // btnRemoveJoin
            // 
            this.btnRemoveJoin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveJoin.Enabled = false;
            this.btnRemoveJoin.Location = new System.Drawing.Point(323, 116);
            this.btnRemoveJoin.Name = "btnRemoveJoin";
            this.btnRemoveJoin.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveJoin.TabIndex = 9;
            this.btnRemoveJoin.Text = "Remove";
            this.btnRemoveJoin.UseVisualStyleBackColor = true;
            this.btnRemoveJoin.Click += new System.EventHandler(this.btnRemoveJoin_Click);
            // 
            // grpJoins
            // 
            this.grpJoins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpJoins.Controls.Add(this.lstJoins);
            this.grpJoins.Location = new System.Drawing.Point(14, 33);
            this.grpJoins.Name = "grpJoins";
            this.grpJoins.Size = new System.Drawing.Size(303, 106);
            this.grpJoins.TabIndex = 8;
            this.grpJoins.TabStop = false;
            this.grpJoins.Text = "Joins";
            // 
            // lstJoins
            // 
            this.lstJoins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstJoins.FormattingEnabled = true;
            this.lstJoins.Location = new System.Drawing.Point(3, 16);
            this.lstJoins.Name = "lstJoins";
            this.lstJoins.Size = new System.Drawing.Size(297, 82);
            this.lstJoins.TabIndex = 0;
            this.lstJoins.SelectedIndexChanged += new System.EventHandler(this.lstJoins_SelectedIndexChanged);
            // 
            // txtClassAlias
            // 
            this.txtClassAlias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClassAlias.Location = new System.Drawing.Point(122, 7);
            this.txtClassAlias.Name = "txtClassAlias";
            this.txtClassAlias.Size = new System.Drawing.Size(195, 20);
            this.txtClassAlias.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Feature Class Alias";
            // 
            // FdoJoinCriteriaCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAddJoin);
            this.Controls.Add(this.btnRemoveJoin);
            this.Controls.Add(this.grpJoins);
            this.Controls.Add(this.txtClassAlias);
            this.Controls.Add(this.label5);
            this.Name = "FdoJoinCriteriaCtl";
            this.Size = new System.Drawing.Size(410, 152);
            this.grpJoins.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAddJoin;
        private System.Windows.Forms.Button btnRemoveJoin;
        private System.Windows.Forms.GroupBox grpJoins;
        private System.Windows.Forms.ListBox lstJoins;
        private System.Windows.Forms.TextBox txtClassAlias;
        private System.Windows.Forms.Label label5;
    }
}
