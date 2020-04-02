namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    partial class NewMultiTaskDialog
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
            this.dgCopyTasks = new System.Windows.Forms.DataGridView();
            this.TaskName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceClass = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TargetClass = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AutoCreateClass = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AutoCreateInSchema = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmbSrcConn = new System.Windows.Forms.ComboBox();
            this.cmbTargetConn = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAcceptConnections = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblHelp = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgCopyTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // dgCopyTasks
            // 
            this.dgCopyTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgCopyTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCopyTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TaskName,
            this.SourceClass,
            this.TargetClass,
            this.AutoCreateClass,
            this.AutoCreateInSchema});
            this.dgCopyTasks.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgCopyTasks.Location = new System.Drawing.Point(12, 116);
            this.dgCopyTasks.Name = "dgCopyTasks";
            this.dgCopyTasks.Size = new System.Drawing.Size(852, 283);
            this.dgCopyTasks.TabIndex = 0;
            this.dgCopyTasks.Visible = false;
            this.dgCopyTasks.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgCopyTasks_CellValueChanged);
            this.dgCopyTasks.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgCopyTasks_DefaultValuesNeeded);
            this.dgCopyTasks.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgCopyTasks_RowsAdded);
            this.dgCopyTasks.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgCopyTasks_RowsRemoved);
            // 
            // TaskName
            // 
            this.TaskName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TaskName.HeaderText = "Name";
            this.TaskName.Name = "TaskName";
            // 
            // SourceClass
            // 
            this.SourceClass.HeaderText = "Source Class";
            this.SourceClass.Name = "SourceClass";
            this.SourceClass.Width = 200;
            // 
            // TargetClass
            // 
            this.TargetClass.HeaderText = "Target Class";
            this.TargetClass.Name = "TargetClass";
            this.TargetClass.Width = 200;
            // 
            // AutoCreateClass
            // 
            this.AutoCreateClass.FalseValue = "false";
            this.AutoCreateClass.HeaderText = "Auto-create class";
            this.AutoCreateClass.Name = "AutoCreateClass";
            this.AutoCreateClass.ToolTipText = "Auto-ceate the class of the same name";
            this.AutoCreateClass.TrueValue = "true";
            // 
            // AutoCreateInSchema
            // 
            this.AutoCreateInSchema.HeaderText = "Auto-create in schema";
            this.AutoCreateInSchema.Name = "AutoCreateInSchema";
            this.AutoCreateInSchema.ToolTipText = "The target schema to auto-create the class in";
            this.AutoCreateInSchema.Width = 150;
            // 
            // cmbSrcConn
            // 
            this.cmbSrcConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSrcConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcConn.FormattingEnabled = true;
            this.cmbSrcConn.Location = new System.Drawing.Point(153, 12);
            this.cmbSrcConn.Name = "cmbSrcConn";
            this.cmbSrcConn.Size = new System.Drawing.Size(711, 21);
            this.cmbSrcConn.TabIndex = 1;
            this.cmbSrcConn.SelectionChangeCommitted += new System.EventHandler(this.cmbSrcConn_SelectionChangeCommitted);
            // 
            // cmbTargetConn
            // 
            this.cmbTargetConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTargetConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetConn.FormattingEnabled = true;
            this.cmbTargetConn.Location = new System.Drawing.Point(153, 40);
            this.cmbTargetConn.Name = "cmbTargetConn";
            this.cmbTargetConn.Size = new System.Drawing.Size(711, 21);
            this.cmbTargetConn.TabIndex = 2;
            this.cmbTargetConn.SelectionChangeCommitted += new System.EventHandler(this.cmbTargetConn_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Source Connection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Target Connection";
            // 
            // btnAcceptConnections
            // 
            this.btnAcceptConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAcceptConnections.Enabled = false;
            this.btnAcceptConnections.Location = new System.Drawing.Point(789, 67);
            this.btnAcceptConnections.Name = "btnAcceptConnections";
            this.btnAcceptConnections.Size = new System.Drawing.Size(75, 23);
            this.btnAcceptConnections.TabIndex = 5;
            this.btnAcceptConnections.Text = "Accept";
            this.btnAcceptConnections.UseVisualStyleBackColor = true;
            this.btnAcceptConnections.Click += new System.EventHandler(this.btnAcceptConnections_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(708, 405);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(789, 405);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.Location = new System.Drawing.Point(163, 72);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(300, 13);
            this.lblHelp.TabIndex = 8;
            this.lblHelp.Text = "Select source and target connections above and click Accept";
            // 
            // NewMultiTaskDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(876, 440);
            this.ControlBox = false;
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAcceptConnections);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbTargetConn);
            this.Controls.Add(this.cmbSrcConn);
            this.Controls.Add(this.dgCopyTasks);
            this.Name = "NewMultiTaskDialog";
            this.Text = "Add Copy Tasks";
            ((System.ComponentModel.ISupportInitialize)(this.dgCopyTasks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgCopyTasks;
        private System.Windows.Forms.ComboBox cmbSrcConn;
        private System.Windows.Forms.ComboBox cmbTargetConn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAcceptConnections;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskName;
        private System.Windows.Forms.DataGridViewComboBoxColumn SourceClass;
        private System.Windows.Forms.DataGridViewComboBoxColumn TargetClass;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AutoCreateClass;
        private System.Windows.Forms.DataGridViewComboBoxColumn AutoCreateInSchema;
    }
}