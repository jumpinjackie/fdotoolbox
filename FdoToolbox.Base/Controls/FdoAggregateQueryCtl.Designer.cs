namespace FdoToolbox.Base.Controls
{
    partial class FdoAggregateQueryCtl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSchema = new System.Windows.Forms.ComboBox();
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lstComputed = new System.Windows.Forms.ListView();
            this.COL_NAME = new System.Windows.Forms.ColumnHeader();
            this.COL_EXPR = new System.Windows.Forms.ColumnHeader();
            this.chkProperties = new System.Windows.Forms.CheckedListBox();
            this.ctxProperties = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unSelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label6 = new System.Windows.Forms.Label();
            this.numLimit = new System.Windows.Forms.NumericUpDown();
            this.tabQueryOptions = new System.Windows.Forms.TabControl();
            this.TAB_PROPERTIES = new System.Windows.Forms.TabPage();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.TAB_COMPUTED = new System.Windows.Forms.TabPage();
            this.lblComputedHint = new System.Windows.Forms.Label();
            this.btnEditComputed = new System.Windows.Forms.Button();
            this.btnAddComputed = new System.Windows.Forms.Button();
            this.TAB_ORDERING = new System.Windows.Forms.TabPage();
            this.cmbOrderingOption = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAddOrderBy = new System.Windows.Forms.Button();
            this.btnRemoveOrderBy = new System.Windows.Forms.Button();
            this.lstOrderBy = new System.Windows.Forms.ListBox();
            this.lstOrderableProperties = new System.Windows.Forms.ListBox();
            this.TAB_GROUPING = new System.Windows.Forms.TabPage();
            this.btnGroupFilter = new System.Windows.Forms.Button();
            this.txtGroupFilter = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRemoveGroupBy = new System.Windows.Forms.Button();
            this.lstGroupBy = new System.Windows.Forms.ListBox();
            this.lstGroupableProperties = new System.Windows.Forms.ListBox();
            this.btnAddGroupBy = new System.Windows.Forms.Button();
            this.chkDistinct = new System.Windows.Forms.CheckBox();
            this.TAB_JOINS = new System.Windows.Forms.TabPage();
            this.joinCriteriaCtrl = new FdoToolbox.Base.Controls.FdoJoinCriteriaCtl();
            this.ctxProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLimit)).BeginInit();
            this.tabQueryOptions.SuspendLayout();
            this.TAB_PROPERTIES.SuspendLayout();
            this.TAB_COMPUTED.SuspendLayout();
            this.TAB_ORDERING.SuspendLayout();
            this.TAB_GROUPING.SuspendLayout();
            this.TAB_JOINS.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Schema";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Class";
            // 
            // cmbSchema
            // 
            this.cmbSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSchema.FormattingEnabled = true;
            this.cmbSchema.Location = new System.Drawing.Point(66, 13);
            this.cmbSchema.Name = "cmbSchema";
            this.cmbSchema.Size = new System.Drawing.Size(185, 21);
            this.cmbSchema.TabIndex = 2;
            this.cmbSchema.SelectedIndexChanged += new System.EventHandler(this.cmbSchema_SelectionChangeCommitted);
            // 
            // cmbClass
            // 
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Location = new System.Drawing.Point(66, 40);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(185, 21);
            this.cmbClass.TabIndex = 3;
            this.cmbClass.SelectedIndexChanged += new System.EventHandler(this.cmbClass_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Filter";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(66, 67);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(185, 78);
            this.txtFilter.TabIndex = 5;
            this.txtFilter.Click += new System.EventHandler(this.txtFilter_Enter);
            // 
            // lstComputed
            // 
            this.lstComputed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstComputed.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.COL_NAME,
            this.COL_EXPR});
            this.lstComputed.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstComputed.LabelEdit = true;
            this.lstComputed.Location = new System.Drawing.Point(3, 3);
            this.lstComputed.MultiSelect = false;
            this.lstComputed.Name = "lstComputed";
            this.lstComputed.Size = new System.Drawing.Size(354, 101);
            this.lstComputed.TabIndex = 8;
            this.lstComputed.UseCompatibleStateImageBehavior = false;
            this.lstComputed.View = System.Windows.Forms.View.Details;
            this.lstComputed.SelectedIndexChanged += new System.EventHandler(this.lstComputed_SelectedIndexChanged);
            // 
            // COL_NAME
            // 
            this.COL_NAME.Text = "Alias";
            this.COL_NAME.Width = 90;
            // 
            // COL_EXPR
            // 
            this.COL_EXPR.Text = "Expression";
            this.COL_EXPR.Width = 197;
            // 
            // chkProperties
            // 
            this.chkProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProperties.CheckOnClick = true;
            this.chkProperties.ContextMenuStrip = this.ctxProperties;
            this.chkProperties.FormattingEnabled = true;
            this.chkProperties.Location = new System.Drawing.Point(3, 2);
            this.chkProperties.Name = "chkProperties";
            this.chkProperties.Size = new System.Drawing.Size(354, 79);
            this.chkProperties.TabIndex = 9;
            // 
            // ctxProperties
            // 
            this.ctxProperties.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.unSelectAllToolStripMenuItem});
            this.ctxProperties.Name = "ctxProperties";
            this.ctxProperties.Size = new System.Drawing.Size(143, 48);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            // 
            // unSelectAllToolStripMenuItem
            // 
            this.unSelectAllToolStripMenuItem.Name = "unSelectAllToolStripMenuItem";
            this.unSelectAllToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.unSelectAllToolStripMenuItem.Text = "Un-Select All";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Limit";
            // 
            // numLimit
            // 
            this.numLimit.Location = new System.Drawing.Point(66, 151);
            this.numLimit.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numLimit.Name = "numLimit";
            this.numLimit.Size = new System.Drawing.Size(108, 20);
            this.numLimit.TabIndex = 16;
            // 
            // tabQueryOptions
            // 
            this.tabQueryOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabQueryOptions.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabQueryOptions.Controls.Add(this.TAB_PROPERTIES);
            this.tabQueryOptions.Controls.Add(this.TAB_COMPUTED);
            this.tabQueryOptions.Controls.Add(this.TAB_ORDERING);
            this.tabQueryOptions.Controls.Add(this.TAB_GROUPING);
            this.tabQueryOptions.Controls.Add(this.TAB_JOINS);
            this.tabQueryOptions.Location = new System.Drawing.Point(274, 13);
            this.tabQueryOptions.Multiline = true;
            this.tabQueryOptions.Name = "tabQueryOptions";
            this.tabQueryOptions.SelectedIndex = 0;
            this.tabQueryOptions.ShowToolTips = true;
            this.tabQueryOptions.Size = new System.Drawing.Size(368, 172);
            this.tabQueryOptions.TabIndex = 17;
            // 
            // TAB_PROPERTIES
            // 
            this.TAB_PROPERTIES.Controls.Add(this.btnSelectNone);
            this.TAB_PROPERTIES.Controls.Add(this.btnSelectAll);
            this.TAB_PROPERTIES.Controls.Add(this.chkProperties);
            this.TAB_PROPERTIES.Location = new System.Drawing.Point(4, 49);
            this.TAB_PROPERTIES.Name = "TAB_PROPERTIES";
            this.TAB_PROPERTIES.Size = new System.Drawing.Size(360, 119);
            this.TAB_PROPERTIES.TabIndex = 2;
            this.TAB_PROPERTIES.Text = "Selected Properties";
            this.TAB_PROPERTIES.UseVisualStyleBackColor = true;
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectNone.Location = new System.Drawing.Point(84, 89);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(75, 23);
            this.btnSelectNone.TabIndex = 11;
            this.btnSelectNone.Text = "Select None";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAll.Location = new System.Drawing.Point(3, 89);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 10;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // TAB_COMPUTED
            // 
            this.TAB_COMPUTED.Controls.Add(this.lblComputedHint);
            this.TAB_COMPUTED.Controls.Add(this.btnEditComputed);
            this.TAB_COMPUTED.Controls.Add(this.btnAddComputed);
            this.TAB_COMPUTED.Controls.Add(this.lstComputed);
            this.TAB_COMPUTED.Location = new System.Drawing.Point(4, 25);
            this.TAB_COMPUTED.Name = "TAB_COMPUTED";
            this.TAB_COMPUTED.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_COMPUTED.Size = new System.Drawing.Size(360, 143);
            this.TAB_COMPUTED.TabIndex = 0;
            this.TAB_COMPUTED.Text = "Computed Properties";
            this.TAB_COMPUTED.UseVisualStyleBackColor = true;
            // 
            // lblComputedHint
            // 
            this.lblComputedHint.AutoSize = true;
            this.lblComputedHint.Location = new System.Drawing.Point(147, 115);
            this.lblComputedHint.Name = "lblComputedHint";
            this.lblComputedHint.Size = new System.Drawing.Size(188, 13);
            this.lblComputedHint.TabIndex = 11;
            this.lblComputedHint.Text = "Double click on the alias label to edit it";
            // 
            // btnEditComputed
            // 
            this.btnEditComputed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditComputed.Enabled = false;
            this.btnEditComputed.Location = new System.Drawing.Point(69, 110);
            this.btnEditComputed.Name = "btnEditComputed";
            this.btnEditComputed.Size = new System.Drawing.Size(61, 23);
            this.btnEditComputed.TabIndex = 10;
            this.btnEditComputed.Text = "Edit";
            this.btnEditComputed.UseVisualStyleBackColor = true;
            this.btnEditComputed.Click += new System.EventHandler(this.btnEditComputed_Click);
            // 
            // btnAddComputed
            // 
            this.btnAddComputed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddComputed.Location = new System.Drawing.Point(3, 110);
            this.btnAddComputed.Name = "btnAddComputed";
            this.btnAddComputed.Size = new System.Drawing.Size(60, 23);
            this.btnAddComputed.TabIndex = 9;
            this.btnAddComputed.Text = "Add";
            this.btnAddComputed.UseVisualStyleBackColor = true;
            this.btnAddComputed.Click += new System.EventHandler(this.btnAddComputed_Click);
            // 
            // TAB_ORDERING
            // 
            this.TAB_ORDERING.Controls.Add(this.cmbOrderingOption);
            this.TAB_ORDERING.Controls.Add(this.label4);
            this.TAB_ORDERING.Controls.Add(this.btnAddOrderBy);
            this.TAB_ORDERING.Controls.Add(this.btnRemoveOrderBy);
            this.TAB_ORDERING.Controls.Add(this.lstOrderBy);
            this.TAB_ORDERING.Controls.Add(this.lstOrderableProperties);
            this.TAB_ORDERING.Location = new System.Drawing.Point(4, 25);
            this.TAB_ORDERING.Name = "TAB_ORDERING";
            this.TAB_ORDERING.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_ORDERING.Size = new System.Drawing.Size(360, 143);
            this.TAB_ORDERING.TabIndex = 1;
            this.TAB_ORDERING.Text = "Ordering";
            this.TAB_ORDERING.UseVisualStyleBackColor = true;
            // 
            // cmbOrderingOption
            // 
            this.cmbOrderingOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOrderingOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrderingOption.FormattingEnabled = true;
            this.cmbOrderingOption.Location = new System.Drawing.Point(93, 112);
            this.cmbOrderingOption.Name = "cmbOrderingOption";
            this.cmbOrderingOption.Size = new System.Drawing.Size(261, 21);
            this.cmbOrderingOption.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Ordering Option";
            // 
            // btnAddOrderBy
            // 
            this.btnAddOrderBy.Location = new System.Drawing.Point(132, 22);
            this.btnAddOrderBy.Name = "btnAddOrderBy";
            this.btnAddOrderBy.Size = new System.Drawing.Size(49, 23);
            this.btnAddOrderBy.TabIndex = 3;
            this.btnAddOrderBy.Text = ">>";
            this.btnAddOrderBy.UseVisualStyleBackColor = true;
            this.btnAddOrderBy.Click += new System.EventHandler(this.btnAddOrderBy_Click);
            // 
            // btnRemoveOrderBy
            // 
            this.btnRemoveOrderBy.Location = new System.Drawing.Point(132, 51);
            this.btnRemoveOrderBy.Name = "btnRemoveOrderBy";
            this.btnRemoveOrderBy.Size = new System.Drawing.Size(49, 23);
            this.btnRemoveOrderBy.TabIndex = 2;
            this.btnRemoveOrderBy.Text = "<<";
            this.btnRemoveOrderBy.UseVisualStyleBackColor = true;
            this.btnRemoveOrderBy.Click += new System.EventHandler(this.btnRemoveOrderBy_Click);
            // 
            // lstOrderBy
            // 
            this.lstOrderBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstOrderBy.FormattingEnabled = true;
            this.lstOrderBy.Location = new System.Drawing.Point(187, 6);
            this.lstOrderBy.Name = "lstOrderBy";
            this.lstOrderBy.Size = new System.Drawing.Size(167, 95);
            this.lstOrderBy.TabIndex = 1;
            // 
            // lstOrderableProperties
            // 
            this.lstOrderableProperties.FormattingEnabled = true;
            this.lstOrderableProperties.Location = new System.Drawing.Point(6, 6);
            this.lstOrderableProperties.Name = "lstOrderableProperties";
            this.lstOrderableProperties.Size = new System.Drawing.Size(120, 95);
            this.lstOrderableProperties.TabIndex = 0;
            // 
            // TAB_GROUPING
            // 
            this.TAB_GROUPING.Controls.Add(this.btnGroupFilter);
            this.TAB_GROUPING.Controls.Add(this.txtGroupFilter);
            this.TAB_GROUPING.Controls.Add(this.label5);
            this.TAB_GROUPING.Controls.Add(this.btnRemoveGroupBy);
            this.TAB_GROUPING.Controls.Add(this.lstGroupBy);
            this.TAB_GROUPING.Controls.Add(this.lstGroupableProperties);
            this.TAB_GROUPING.Controls.Add(this.btnAddGroupBy);
            this.TAB_GROUPING.Location = new System.Drawing.Point(4, 25);
            this.TAB_GROUPING.Name = "TAB_GROUPING";
            this.TAB_GROUPING.Size = new System.Drawing.Size(360, 143);
            this.TAB_GROUPING.TabIndex = 3;
            this.TAB_GROUPING.Text = "Grouping";
            this.TAB_GROUPING.UseVisualStyleBackColor = true;
            // 
            // btnGroupFilter
            // 
            this.btnGroupFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGroupFilter.Location = new System.Drawing.Point(327, 111);
            this.btnGroupFilter.Name = "btnGroupFilter";
            this.btnGroupFilter.Size = new System.Drawing.Size(29, 23);
            this.btnGroupFilter.TabIndex = 6;
            this.btnGroupFilter.Text = "...";
            this.btnGroupFilter.UseVisualStyleBackColor = true;
            this.btnGroupFilter.Click += new System.EventHandler(this.btnGroupFilter_Click);
            // 
            // txtGroupFilter
            // 
            this.txtGroupFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGroupFilter.Location = new System.Drawing.Point(72, 113);
            this.txtGroupFilter.Name = "txtGroupFilter";
            this.txtGroupFilter.Size = new System.Drawing.Size(249, 20);
            this.txtGroupFilter.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Group Filter";
            // 
            // btnRemoveGroupBy
            // 
            this.btnRemoveGroupBy.Location = new System.Drawing.Point(132, 51);
            this.btnRemoveGroupBy.Name = "btnRemoveGroupBy";
            this.btnRemoveGroupBy.Size = new System.Drawing.Size(49, 23);
            this.btnRemoveGroupBy.TabIndex = 3;
            this.btnRemoveGroupBy.Text = "<<";
            this.btnRemoveGroupBy.UseVisualStyleBackColor = true;
            this.btnRemoveGroupBy.Click += new System.EventHandler(this.btnRemoveGroupBy_Click);
            // 
            // lstGroupBy
            // 
            this.lstGroupBy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstGroupBy.FormattingEnabled = true;
            this.lstGroupBy.Location = new System.Drawing.Point(187, 6);
            this.lstGroupBy.Name = "lstGroupBy";
            this.lstGroupBy.Size = new System.Drawing.Size(167, 95);
            this.lstGroupBy.TabIndex = 1;
            // 
            // lstGroupableProperties
            // 
            this.lstGroupableProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstGroupableProperties.FormattingEnabled = true;
            this.lstGroupableProperties.Location = new System.Drawing.Point(6, 6);
            this.lstGroupableProperties.Name = "lstGroupableProperties";
            this.lstGroupableProperties.Size = new System.Drawing.Size(120, 95);
            this.lstGroupableProperties.TabIndex = 0;
            // 
            // btnAddGroupBy
            // 
            this.btnAddGroupBy.Location = new System.Drawing.Point(132, 22);
            this.btnAddGroupBy.Name = "btnAddGroupBy";
            this.btnAddGroupBy.Size = new System.Drawing.Size(49, 23);
            this.btnAddGroupBy.TabIndex = 2;
            this.btnAddGroupBy.Text = ">>";
            this.btnAddGroupBy.UseVisualStyleBackColor = true;
            this.btnAddGroupBy.Click += new System.EventHandler(this.btnAddGroupBy_Click);
            // 
            // chkDistinct
            // 
            this.chkDistinct.AutoSize = true;
            this.chkDistinct.Location = new System.Drawing.Point(190, 152);
            this.chkDistinct.Name = "chkDistinct";
            this.chkDistinct.Size = new System.Drawing.Size(61, 17);
            this.chkDistinct.TabIndex = 0;
            this.chkDistinct.Text = "Distinct";
            this.chkDistinct.UseVisualStyleBackColor = true;
            // 
            // TAB_JOINS
            // 
            this.TAB_JOINS.Controls.Add(this.joinCriteriaCtrl);
            this.TAB_JOINS.Location = new System.Drawing.Point(4, 49);
            this.TAB_JOINS.Name = "TAB_JOINS";
            this.TAB_JOINS.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_JOINS.Size = new System.Drawing.Size(360, 119);
            this.TAB_JOINS.TabIndex = 4;
            this.TAB_JOINS.Text = "Joins";
            this.TAB_JOINS.UseVisualStyleBackColor = true;
            // 
            // joinCriteiraCtrl
            // 
            this.joinCriteriaCtrl.Connection = null;
            this.joinCriteriaCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.joinCriteriaCtrl.Location = new System.Drawing.Point(3, 3);
            this.joinCriteriaCtrl.Name = "joinCriteiraCtrl";
            this.joinCriteriaCtrl.SelectedClass = null;
            this.joinCriteriaCtrl.Size = new System.Drawing.Size(354, 113);
            this.joinCriteriaCtrl.TabIndex = 0;
            // 
            // FdoAggregateQueryCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkDistinct);
            this.Controls.Add(this.tabQueryOptions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbClass);
            this.Controls.Add(this.numLimit);
            this.Controls.Add(this.cmbSchema);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label1);
            this.Name = "FdoAggregateQueryCtl";
            this.Size = new System.Drawing.Size(656, 205);
            this.ctxProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numLimit)).EndInit();
            this.tabQueryOptions.ResumeLayout(false);
            this.TAB_PROPERTIES.ResumeLayout(false);
            this.TAB_COMPUTED.ResumeLayout(false);
            this.TAB_COMPUTED.PerformLayout();
            this.TAB_ORDERING.ResumeLayout(false);
            this.TAB_ORDERING.PerformLayout();
            this.TAB_GROUPING.ResumeLayout(false);
            this.TAB_GROUPING.PerformLayout();
            this.TAB_JOINS.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader COL_NAME;
        private System.Windows.Forms.ColumnHeader COL_EXPR;
        private System.Windows.Forms.ContextMenuStrip ctxProperties;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unSelectAllToolStripMenuItem;
        protected System.Windows.Forms.ComboBox cmbSchema;
        protected System.Windows.Forms.ComboBox cmbClass;
        protected System.Windows.Forms.TextBox txtFilter;
        protected System.Windows.Forms.ListView lstComputed;
        protected System.Windows.Forms.CheckedListBox chkProperties;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numLimit;
        private System.Windows.Forms.TabControl tabQueryOptions;
        private System.Windows.Forms.TabPage TAB_COMPUTED;
        private System.Windows.Forms.TabPage TAB_ORDERING;
        private System.Windows.Forms.TabPage TAB_PROPERTIES;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnEditComputed;
        private System.Windows.Forms.Button btnAddComputed;
        private System.Windows.Forms.ComboBox cmbOrderingOption;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAddOrderBy;
        private System.Windows.Forms.Button btnRemoveOrderBy;
        private System.Windows.Forms.ListBox lstOrderBy;
        private System.Windows.Forms.ListBox lstOrderableProperties;
        private System.Windows.Forms.Label lblComputedHint;
        private System.Windows.Forms.TabPage TAB_GROUPING;
        private System.Windows.Forms.CheckBox chkDistinct;
        private System.Windows.Forms.Button btnRemoveGroupBy;
        private System.Windows.Forms.Button btnAddGroupBy;
        private System.Windows.Forms.ListBox lstGroupBy;
        private System.Windows.Forms.ListBox lstGroupableProperties;
        private System.Windows.Forms.Button btnGroupFilter;
        private System.Windows.Forms.TextBox txtGroupFilter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage TAB_JOINS;
        private FdoJoinCriteriaCtl joinCriteriaCtrl;
    }
}
