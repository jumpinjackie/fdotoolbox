namespace FdoToolbox.Base.Controls.PreferenceSheets
{
    partial class BasePreferencesCtl
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
            this.label4 = new System.Windows.Forms.Label();
            this.txtSession = new System.Windows.Forms.TextBox();
            this.txtWorking = new System.Windows.Forms.TextBox();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.txtFdoPath = new System.Windows.Forms.TextBox();
            this.btnSession = new System.Windows.Forms.Button();
            this.btnWorking = new System.Windows.Forms.Button();
            this.btnLog = new System.Windows.Forms.Button();
            this.btnFdo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Session Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Working Directory";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Log Path";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "FDO Library Path";
            // 
            // txtSession
            // 
            this.txtSession.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSession.Location = new System.Drawing.Point(163, 16);
            this.txtSession.Name = "txtSession";
            this.txtSession.Size = new System.Drawing.Size(297, 20);
            this.txtSession.TabIndex = 5;
            // 
            // txtWorking
            // 
            this.txtWorking.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWorking.Location = new System.Drawing.Point(163, 48);
            this.txtWorking.Name = "txtWorking";
            this.txtWorking.Size = new System.Drawing.Size(297, 20);
            this.txtWorking.TabIndex = 6;
            // 
            // txtLogPath
            // 
            this.txtLogPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogPath.Location = new System.Drawing.Point(163, 78);
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.Size = new System.Drawing.Size(297, 20);
            this.txtLogPath.TabIndex = 7;
            // 
            // txtFdoPath
            // 
            this.txtFdoPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFdoPath.Location = new System.Drawing.Point(163, 108);
            this.txtFdoPath.Name = "txtFdoPath";
            this.txtFdoPath.Size = new System.Drawing.Size(297, 20);
            this.txtFdoPath.TabIndex = 8;
            // 
            // btnSession
            // 
            this.btnSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSession.Location = new System.Drawing.Point(466, 14);
            this.btnSession.Name = "btnSession";
            this.btnSession.Size = new System.Drawing.Size(33, 23);
            this.btnSession.TabIndex = 10;
            this.btnSession.Text = "...";
            this.btnSession.UseVisualStyleBackColor = true;
            this.btnSession.Click += new System.EventHandler(this.btnSession_Click);
            // 
            // btnWorking
            // 
            this.btnWorking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWorking.Location = new System.Drawing.Point(466, 46);
            this.btnWorking.Name = "btnWorking";
            this.btnWorking.Size = new System.Drawing.Size(33, 23);
            this.btnWorking.TabIndex = 11;
            this.btnWorking.Text = "...";
            this.btnWorking.UseVisualStyleBackColor = true;
            this.btnWorking.Click += new System.EventHandler(this.btnWorking_Click);
            // 
            // btnLog
            // 
            this.btnLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLog.Location = new System.Drawing.Point(466, 76);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(33, 23);
            this.btnLog.TabIndex = 12;
            this.btnLog.Text = "...";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // btnFdo
            // 
            this.btnFdo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFdo.Location = new System.Drawing.Point(466, 106);
            this.btnFdo.Name = "btnFdo";
            this.btnFdo.Size = new System.Drawing.Size(33, 23);
            this.btnFdo.TabIndex = 13;
            this.btnFdo.Text = "...";
            this.btnFdo.UseVisualStyleBackColor = true;
            this.btnFdo.Click += new System.EventHandler(this.btnFdo_Click);
            // 
            // BasePreferencesCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnFdo);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.btnWorking);
            this.Controls.Add(this.btnSession);
            this.Controls.Add(this.txtFdoPath);
            this.Controls.Add(this.txtLogPath);
            this.Controls.Add(this.txtWorking);
            this.Controls.Add(this.txtSession);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "BasePreferencesCtl";
            this.Size = new System.Drawing.Size(511, 186);
            this.Load += new System.EventHandler(this.BasePreferencesCtl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSession;
        private System.Windows.Forms.TextBox txtWorking;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.TextBox txtFdoPath;
        private System.Windows.Forms.Button btnSession;
        private System.Windows.Forms.Button btnWorking;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Button btnFdo;
    }
}
