namespace FdoToolbox.Base.Forms
{
    partial class ExpressionEditor
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
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnFunctions = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnConditions = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnDistance = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnSpatial = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnValidate = new System.Windows.Forms.ToolStripButton();
            this.btnGetValues = new System.Windows.Forms.ToolStripButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ctxInsert = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertGeometryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polygonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.curveStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.curvePolygonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.exprPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtExpression = new System.Windows.Forms.RichTextBox();
            this.lblValueCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnHide = new System.Windows.Forms.Button();
            this.btnFetch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lstValues = new System.Windows.Forms.ListBox();
            this.cmbProperty = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this._autoCompleteTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.usingGeometryVisualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.ctxInsert.SuspendLayout();
            this.panel1.SuspendLayout();
            this.exprPanel.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFunctions,
            this.btnConditions,
            this.btnDistance,
            this.btnSpatial,
            this.toolStripSeparator1,
            this.btnValidate,
            this.btnGetValues});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(637, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnFunctions
            // 
            this.btnFunctions.Image = global::FdoToolbox.Base.Images.bricks;
            this.btnFunctions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFunctions.Name = "btnFunctions";
            this.btnFunctions.Size = new System.Drawing.Size(82, 22);
            this.btnFunctions.Text = "Functions";
            // 
            // btnConditions
            // 
            this.btnConditions.Image = global::FdoToolbox.Base.Images.add;
            this.btnConditions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConditions.Name = "btnConditions";
            this.btnConditions.Size = new System.Drawing.Size(86, 22);
            this.btnConditions.Text = "Conditions";
            // 
            // btnDistance
            // 
            this.btnDistance.Image = global::FdoToolbox.Base.Images.arrow_right;
            this.btnDistance.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDistance.Name = "btnDistance";
            this.btnDistance.Size = new System.Drawing.Size(77, 22);
            this.btnDistance.Text = "Distance";
            // 
            // btnSpatial
            // 
            this.btnSpatial.Image = global::FdoToolbox.Base.Images.map;
            this.btnSpatial.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpatial.Name = "btnSpatial";
            this.btnSpatial.Size = new System.Drawing.Size(68, 22);
            this.btnSpatial.Text = "Spatial";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnValidate
            // 
            this.btnValidate.Image = global::FdoToolbox.Base.Images.accept;
            this.btnValidate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(65, 22);
            this.btnValidate.Text = "Validate";
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnGetValues
            // 
            this.btnGetValues.Image = global::FdoToolbox.Base.Images.application_go;
            this.btnGetValues.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGetValues.Name = "btnGetValues";
            this.btnGetValues.Size = new System.Drawing.Size(78, 22);
            this.btnGetValues.Text = "Get Values";
            this.btnGetValues.Click += new System.EventHandler(this.btnGetValues_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(468, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(549, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ctxInsert
            // 
            this.ctxInsert.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertPropertyToolStripMenuItem,
            this.insertGeometryToolStripMenuItem});
            this.ctxInsert.Name = "ctxInsert";
            this.ctxInsert.Size = new System.Drawing.Size(154, 70);
            // 
            // insertPropertyToolStripMenuItem
            // 
            this.insertPropertyToolStripMenuItem.Image = global::FdoToolbox.Base.Images.table;
            this.insertPropertyToolStripMenuItem.Name = "insertPropertyToolStripMenuItem";
            this.insertPropertyToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.insertPropertyToolStripMenuItem.Text = "Insert Property";
            // 
            // insertGeometryToolStripMenuItem
            // 
            this.insertGeometryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usingGeometryVisualizerToolStripMenuItem,
            this.toolStripSeparator2,
            this.pointToolStripMenuItem,
            this.lineStringToolStripMenuItem,
            this.polygonToolStripMenuItem,
            this.curveStringToolStripMenuItem,
            this.curvePolygonToolStripMenuItem});
            this.insertGeometryToolStripMenuItem.Image = global::FdoToolbox.Base.Images.shape_handles;
            this.insertGeometryToolStripMenuItem.Name = "insertGeometryToolStripMenuItem";
            this.insertGeometryToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.insertGeometryToolStripMenuItem.Text = "Insert Geometry";
            // 
            // pointToolStripMenuItem
            // 
            this.pointToolStripMenuItem.Name = "pointToolStripMenuItem";
            this.pointToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.pointToolStripMenuItem.Text = "Point";
            this.pointToolStripMenuItem.Click += new System.EventHandler(this.pointToolStripMenuItem_Click);
            // 
            // lineStringToolStripMenuItem
            // 
            this.lineStringToolStripMenuItem.Name = "lineStringToolStripMenuItem";
            this.lineStringToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.lineStringToolStripMenuItem.Text = "LineString";
            this.lineStringToolStripMenuItem.Click += new System.EventHandler(this.lineStringToolStripMenuItem_Click);
            // 
            // polygonToolStripMenuItem
            // 
            this.polygonToolStripMenuItem.Name = "polygonToolStripMenuItem";
            this.polygonToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.polygonToolStripMenuItem.Text = "Polygon";
            this.polygonToolStripMenuItem.Click += new System.EventHandler(this.polygonToolStripMenuItem_Click);
            // 
            // curveStringToolStripMenuItem
            // 
            this.curveStringToolStripMenuItem.Name = "curveStringToolStripMenuItem";
            this.curveStringToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.curveStringToolStripMenuItem.Text = "CurveString";
            this.curveStringToolStripMenuItem.Click += new System.EventHandler(this.curveStringToolStripMenuItem_Click);
            // 
            // curvePolygonToolStripMenuItem
            // 
            this.curvePolygonToolStripMenuItem.Name = "curvePolygonToolStripMenuItem";
            this.curvePolygonToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.curvePolygonToolStripMenuItem.Text = "CurvePolygon";
            this.curvePolygonToolStripMenuItem.Click += new System.EventHandler(this.curvePolygonToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 271);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(637, 49);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Press Alt + Right to invoke auto-complete at any time";
            // 
            // exprPanel
            // 
            this.exprPanel.Controls.Add(this.splitContainer1);
            this.exprPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exprPanel.Location = new System.Drawing.Point(0, 25);
            this.exprPanel.Name = "exprPanel";
            this.exprPanel.Size = new System.Drawing.Size(637, 246);
            this.exprPanel.TabIndex = 4;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtExpression);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblValueCount);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.btnHide);
            this.splitContainer1.Panel2.Controls.Add(this.btnFetch);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.lstValues);
            this.splitContainer1.Panel2.Controls.Add(this.cmbProperty);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(637, 246);
            this.splitContainer1.SplitterDistance = 437;
            this.splitContainer1.TabIndex = 6;
            // 
            // txtExpression
            // 
            this.txtExpression.ContextMenuStrip = this.ctxInsert;
            this.txtExpression.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExpression.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExpression.Location = new System.Drawing.Point(0, 0);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(437, 246);
            this.txtExpression.TabIndex = 5;
            this.txtExpression.Text = "";
            this.txtExpression.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtExpression_KeyDown);
            this.txtExpression.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtExpression_KeyUp);
            // 
            // lblValueCount
            // 
            this.lblValueCount.AutoSize = true;
            this.lblValueCount.Location = new System.Drawing.Point(55, 44);
            this.lblValueCount.Name = "lblValueCount";
            this.lblValueCount.Size = new System.Drawing.Size(0, 13);
            this.lblValueCount.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(16, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(167, 46);
            this.label4.TabIndex = 6;
            this.label4.Text = "Double click a value to insert that value at the current text position";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnHide
            // 
            this.btnHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHide.Location = new System.Drawing.Point(108, 171);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(75, 23);
            this.btnHide.TabIndex = 5;
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // btnFetch
            // 
            this.btnFetch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFetch.Location = new System.Drawing.Point(26, 171);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(75, 23);
            this.btnFetch.TabIndex = 4;
            this.btnFetch.Text = "Fetch";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Values";
            // 
            // lstValues
            // 
            this.lstValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstValues.FormattingEnabled = true;
            this.lstValues.Location = new System.Drawing.Point(16, 70);
            this.lstValues.Name = "lstValues";
            this.lstValues.Size = new System.Drawing.Size(168, 95);
            this.lstValues.TabIndex = 2;
            this.lstValues.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstValues_MouseDoubleClick);
            // 
            // cmbProperty
            // 
            this.cmbProperty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProperty.FormattingEnabled = true;
            this.cmbProperty.Location = new System.Drawing.Point(16, 20);
            this.cmbProperty.Name = "cmbProperty";
            this.cmbProperty.Size = new System.Drawing.Size(169, 21);
            this.cmbProperty.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Property Name";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(194, 6);
            // 
            // usingGeometryVisualizerToolStripMenuItem
            // 
            this.usingGeometryVisualizerToolStripMenuItem.Name = "usingGeometryVisualizerToolStripMenuItem";
            this.usingGeometryVisualizerToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.usingGeometryVisualizerToolStripMenuItem.Text = "Using Geometry Visualizer";
            this.usingGeometryVisualizerToolStripMenuItem.Click += new System.EventHandler(this.usingGeometryVisualizerToolStripMenuItem_Click);
            // 
            // ExpressionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 320);
            this.ControlBox = false;
            this.Controls.Add(this.exprPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ExpressionEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Expression Editor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ctxInsert.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.exprPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton btnFunctions;
        private System.Windows.Forms.ToolStripDropDownButton btnConditions;
        private System.Windows.Forms.ToolStripDropDownButton btnSpatial;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnValidate;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip ctxInsert;
        private System.Windows.Forms.ToolStripDropDownButton btnDistance;
        private System.Windows.Forms.ToolStripMenuItem insertPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertGeometryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem polygonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem curveStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem curvePolygonToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel exprPanel;
        private System.Windows.Forms.RichTextBox txtExpression;
        private System.Windows.Forms.ToolTip _autoCompleteTooltip;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton btnGetValues;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstValues;
        private System.Windows.Forms.ComboBox cmbProperty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblValueCount;
        private System.Windows.Forms.ToolStripMenuItem usingGeometryVisualizerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}