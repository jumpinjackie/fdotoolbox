namespace FdoToolbox.Base.Scripting
{
    partial class ScriptManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptManager));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnUnload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRun = new System.Windows.Forms.ToolStripButton();
            this.btnReload = new System.Windows.Forms.ToolStripButton();
            this.treeScripts = new System.Windows.Forms.TreeView();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "script.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnUnload,
            this.toolStripSeparator1,
            this.btnRun,
            this.btnReload});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(198, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOpen
            // 
            this.btnOpen.Image = global::FdoToolbox.Base.Images.folder;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(50, 22);
            this.btnOpen.Text = "Load";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnUnload
            // 
            this.btnUnload.Enabled = false;
            this.btnUnload.Image = global::FdoToolbox.Base.Images.cross;
            this.btnUnload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(60, 22);
            this.btnUnload.Text = "Unload";
            this.btnUnload.Visible = false;
            this.btnUnload.Click += new System.EventHandler(this.btnUnload_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Image = global::FdoToolbox.Base.Images.application_go;
            this.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(46, 22);
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnReload
            // 
            this.btnReload.Enabled = false;
            this.btnReload.Image = global::FdoToolbox.Base.Images.page_refresh;
            this.btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(60, 20);
            this.btnReload.Text = "Reload";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // treeScripts
            // 
            this.treeScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeScripts.ImageIndex = 0;
            this.treeScripts.ImageList = this.imageList1;
            this.treeScripts.Location = new System.Drawing.Point(0, 25);
            this.treeScripts.Name = "treeScripts";
            this.treeScripts.SelectedImageIndex = 0;
            this.treeScripts.Size = new System.Drawing.Size(198, 340);
            this.treeScripts.TabIndex = 1;
            this.treeScripts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeScripts_AfterSelect);
            // 
            // ScriptManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.treeScripts);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ScriptManager";
            this.Size = new System.Drawing.Size(198, 365);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnRun;
        private System.Windows.Forms.TreeView treeScripts;
        private System.Windows.Forms.ToolStripButton btnUnload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnReload;
    }
}
