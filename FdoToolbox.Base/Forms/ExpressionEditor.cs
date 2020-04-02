#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// https://github.com/jumpinjackie/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Connections.Capabilities;
using OSGeo.FDO.Filter;
using ICSharpCode.Core;
using OSGeo.FDO.Expression;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Base.Forms
{
    /// <summary>
    /// Defines the mode the expression editor dialog should function in
    /// </summary>
    public enum ExpressionMode
    {
        /// <summary>
        /// Editor is for editing filter expressions
        /// </summary>
        Filter,
        /// <summary>
        /// Editor is for editing aggregate expressions
        /// </summary>
        Aggregate,
        /// <summary>
        /// Editor is for editing any expressions
        /// </summary>
        Normal
    }

    /*
     * Intellisense overview:
     * 
     * The intellisense of this expression editor consists of the following parts:
     *  - An ImageListBox which is filled with auto-complete suggestions
     *  - A System.Windows.Forms.ToolTip which is shown when an auto-complete choice is highlighted (but not selected)
     * 
     * In order to invoke intellisense, we listen for the KeyUp and KeyDown events
     * on the textbox to determine what actions to take. Some actions include:
     * 
     * Key Up:
     *  - Comma: Show auto-complete with all suggestions
     *  - Quotes (Single or Double): Insert an extra quote of that type
     *  - Up/Down: Move the auto-complete selection up/down one item if the auto-complete box is visible.
     *  - Backspace: Invoke auto-complete with suggestions if there is a context buffer, otherwise hide auto-complete.
     *  - Alt + Right: Invoke auto-complete with all suggestions
     *  - Alphanumeric (no modifiers): Invoke auto-complete with suggestions
     * 
     * Key Down:
     *  - Escape: Hide auto-complete
     *  - Enter: Hide auto-complete
     * 
     * As part of the loading process, a full list of auto-complete items (functions/properties) is constructed (sorted by name)
     * Everytime intellisense is invoked, this list is queried for possible suggestions.
     * 
     * In order to determine what items to suggest, the editor builds a context buffer from the current position of the caret
     * in the textbox. The context buffer algorithm is as follows:
     * 
     *  1 - Start from caret position
     *  2 - Can we move back one char?
     *    2.1 - Get this char.
     *    2.2 - If alpha numeric, goto 2.
     *  3 - Get the string that represents the uninterrupted alphanumeric string sequence that ends at the caret position
     *  4 - Get the list of completable items that starts with this alphanumeric string
     *  5 - Add these items to the auto-complete box.
     *  6 - Show the auto-complete box
     */


    /// <summary>
    /// The expression editor dialog
    /// </summary>
    public partial class ExpressionEditor : Form
    {
        private SortedList<string, AutoCompleteItem> _autoCompleteItems = new SortedList<string, AutoCompleteItem>();

        private ImageListBox _autoBox;

        enum AutoCompleteItemType : int
        {
            Property = 0,
            Function = 1,
        }

        /// <summary>
        /// Base auto-complete item
        /// </summary>
        abstract class AutoCompleteItem
        {
            public abstract AutoCompleteItemType Type { get; }

            public abstract string Name { get; }

            public abstract string ToolTipText { get; }

            public abstract string AutoCompleteText { get; }
        }

        /// <summary>
        /// Property auto-complete item
        /// </summary>
        class PropertyItem : AutoCompleteItem
        {
            private PropertyDefinition _propDef;

            public PropertyItem(PropertyDefinition pd)
            {
                _propDef = pd;
            }

            public override AutoCompleteItemType Type
            {
                get { return AutoCompleteItemType.Property; }
            }

            public override string Name
            {
                get { return _propDef.Name; }
            }

            private string _ttText;

            public override string ToolTipText
            {
                get
                {
                    if (string.IsNullOrEmpty(_ttText))
                    {
                        if (_propDef.PropertyType == PropertyType.PropertyType_DataProperty)
                        {
                            DataPropertyDefinition dp = _propDef as DataPropertyDefinition;
                            _ttText = string.Format("Property:{1}{0}{1}Data Type: {2}", dp.QualifiedName, Environment.NewLine, EnumUtil.GetStringValue(dp.DataType));
                        }
                        else
                        {
                            _ttText = string.Format("Property:{1}{0}{1}Type: {2}", _propDef.QualifiedName, Environment.NewLine, EnumUtil.GetStringValue(_propDef.PropertyType));
                        }
                    }
                    return _ttText;
                }
            }

            public bool IsData { get { return _propDef.PropertyType == PropertyType.PropertyType_DataProperty; } }

            public override string AutoCompleteText
            {
                get { return this.Name; }
            }
        }

        /// <summary>
        /// Function auto-complete item
        /// </summary>
        class FunctionItem : AutoCompleteItem
        {
            private FunctionDefinition _func;

            public FunctionItem(FunctionDefinition fd)
            {
                _func = fd;
            }

            public override AutoCompleteItemType Type
            {
                get { return AutoCompleteItemType.Function; }
            }

            public override string Name
            {
                get { return _func.Name; }
            }

            private string _ttText;

            public override string ToolTipText
            {
                get 
                { 
                    if(string.IsNullOrEmpty(_ttText))
                        _ttText = string.Format("Function: {1}({2}){3}Description: {4}{3}Returns: {0}", GetReturnTypeString(), _func.Name, GetArgumentString(), Environment.NewLine, _func.Description);

                    return _ttText;
                }
            }

            private string _argStr;

            private string GetArgumentString()
            {
                if (string.IsNullOrEmpty(_argStr))
                {
                    List<string> tokens = new List<string>();
                    foreach (ArgumentDefinition argDef in _func.Arguments)
                    {
                        tokens.Add("[" + argDef.Name.Trim() + "]");
                    }
                    _argStr = string.Join(", ", tokens.ToArray());
                }
                return _argStr;
            }

            private string _retStr;

            private string GetReturnTypeString()
            {
                if (string.IsNullOrEmpty(_retStr))
                {
                    if (_func.ReturnPropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        _retStr = EnumUtil.GetStringValue(_func.ReturnType);
                    }
                    else
                    {
                        _retStr = EnumUtil.GetStringValue(_func.ReturnPropertyType);
                    }
                }
                return _retStr;
            }

            public override string AutoCompleteText
            {
                get 
                {
                    return this.Name + "(" + GetArgumentString() + ")";
                }
            }
        }

        class EnumUtil
        {
            public static string GetStringValue(DataType dt)
            {
                switch (dt)
                {
                    case DataType.DataType_BLOB:
                        return "BLOB";
                    case DataType.DataType_Boolean:
                        return "boolean";
                    case DataType.DataType_Byte:
                        return "byte";
                    case DataType.DataType_CLOB:
                        return "CLOB";
                    case DataType.DataType_DateTime:
                        return "DateTime";
                    case DataType.DataType_Decimal:
                        return "decimal";
                    case DataType.DataType_Double:
                        return "double";
                    case DataType.DataType_Int16:
                        return "Int16";
                    case DataType.DataType_Int32:
                        return "Int32";
                    case DataType.DataType_Int64:
                        return "Int64";
                    case DataType.DataType_Single:
                        return "single";
                    case DataType.DataType_String:
                        return "string";
                }
                return string.Empty;
            }

            public static string GetStringValue(PropertyType pt)
            {
                switch (pt)
                {
                    case PropertyType.PropertyType_DataProperty:
                        return "Data";
                    case PropertyType.PropertyType_AssociationProperty:
                        return "Assocation";
                    case PropertyType.PropertyType_GeometricProperty:
                        return "Geometry";
                    case PropertyType.PropertyType_ObjectProperty:
                        return "Object";
                    case PropertyType.PropertyType_RasterProperty:
                        return "Raster";
                }
                return string.Empty;
            }
        }

        private FdoConnection _conn;

        private ClassDefinition _ClassDef;

        private ExpressionMode _ExprMode;

        internal ExpressionEditor()
        {
            InitializeComponent();
            InitAutoComplete();
            _FunctionMenuItems = new Dictionary<FunctionCategoryType, ToolStripMenuItem>();
            _ExprMode = ExpressionMode.Filter;
            this.Text = ResourceService.GetString("TITLE_EXPRESSION_EDITOR");
        }

        private void InitAutoComplete()
        {
            _autoBox = new ImageListBox();
            _autoBox.Visible = false;
            _autoBox.ImageList = new ImageList();
            _autoBox.ImageList.Images.Add(Images.table);  //Property
            _autoBox.ImageList.Images.Add(Images.bricks); //Function
            _autoBox.DoubleClick += new EventHandler(OnAutoCompleteDoubleClick);
            _autoBox.SelectedIndexChanged += new EventHandler(OnAutoCompleteSelectedIndexChanged);
            _autoBox.KeyDown += new KeyEventHandler(OnAutoCompleteKeyDown);
            _autoBox.KeyUp += new KeyEventHandler(OnAutoCompleteKeyUp);
            _autoBox.ValueMember = "Name";
            _autoBox.Font = new Font(FontFamily.GenericMonospace, 10.0f);
            txtExpression.Controls.Add(_autoBox);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEditor"/> class.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="classDef">The class def.</param>
        /// <param name="mode">The mode.</param>
        internal ExpressionEditor(FdoConnection conn, ClassDefinition classDef, Dictionary<string, ClassDefinition> aliasedClasses, ExpressionMode mode)
            : this()
        {
            _conn = conn;
            _ClassDef = classDef;
            _aliasedClasses = aliasedClasses ?? new Dictionary<string, ClassDefinition>();
            _ExprMode = mode;
        }

        private Dictionary<string, ClassDefinition> _aliasedClasses;
        private Dictionary<FunctionCategoryType, ToolStripMenuItem> _FunctionMenuItems;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if (_conn != null)
            {
                FunctionDefinitionCollection funcs = (FunctionDefinitionCollection)_conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_ExpressionFunctions);  
                Array categories = Enum.GetValues(typeof(FunctionCategoryType));
                ConditionType[] conditions = (ConditionType[])_conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_ConditionTypes);
                DistanceOperations[] distanceOps = (DistanceOperations[])_conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_DistanceOperations);
                SpatialOperations[] spatialOps = (SpatialOperations[])_conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_SpatialOperations);
                LoadFunctionCategories(categories);
                LoadFunctionDefinitions(funcs);
                LoadProperties();
                LoadConditionTypes(conditions);
                LoadDistanceOperations(distanceOps);
                LoadSpatialOperations(spatialOps);
                ApplyView();
                splitContainer1.Panel2Collapsed = true;
            }
            txtExpression.Focus();
            txtExpression.SelectionStart = 0;
            txtExpression.ScrollToCaret();
            base.OnLoad(e);
        }

        private void ApplyView()
        {
            btnConditions.Visible = ((_ExprMode == ExpressionMode.Filter || _ExprMode == ExpressionMode.Normal) && btnConditions.DropDown.Items.Count > 0);
            btnDistance.Visible = ((_ExprMode == ExpressionMode.Filter || _ExprMode == ExpressionMode.Normal) && btnDistance.DropDown.Items.Count > 0);
            btnSpatial.Visible = ((_ExprMode == ExpressionMode.Filter || _ExprMode == ExpressionMode.Normal) && btnSpatial.DropDown.Items.Count > 0);

            if (_ExprMode == ExpressionMode.Aggregate)
            {
                foreach (FunctionCategoryType ctype in _FunctionMenuItems.Keys)
                {
                    _FunctionMenuItems[ctype].Visible = (ctype == FunctionCategoryType.FunctionCategoryType_Aggregate);
                }
            }
            //else
            //{
            //    foreach (FunctionCategoryType ctype in _FunctionMenuItems.Keys)
            //    {
            //        _FunctionMenuItems[ctype].Visible = (ctype != FunctionCategoryType.FunctionCategoryType_Aggregate);
            //    }
            //}
        }

        private void LoadSpatialOperations(SpatialOperations[] spatialOps)
        {
            btnSpatial.Visible = (spatialOps.Length > 0);
            foreach (SpatialOperations op in spatialOps)
            {
                ToolStripMenuItem item = null;
                switch (op)
                {
                    case SpatialOperations.SpatialOperations_Contains:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Contains";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> CONTAINS GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_CoveredBy:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Covered By";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> COVEREDBY GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Crosses:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Crosses";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> CROSSES GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Disjoint:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Disjoint";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> DISJOINT GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_EnvelopeIntersects:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Envelope Intersects";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> INTERSECTS GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Equals:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Equals";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> EQUALS GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Inside:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Inside";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> INSIDE GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Intersects:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Intersects";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> INTERSECTS GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Overlaps:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Overlaps";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> OVERLAPS GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Touches:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Touches";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> TOUCHES GeomFromText('<geometry text>')"); };
                        }
                        break;
                    case SpatialOperations.SpatialOperations_Within:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Within";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<geometry property> WITHIN GeomFromText('<geometry text>')"); };
                        }
                        break;
                }
                if (item != null)
                    btnSpatial.DropDown.Items.Add(item);
            }
        }

        private void LoadDistanceOperations(DistanceOperations[] distanceOps)
        {
            btnDistance.Visible = (distanceOps.Length > 0);
            foreach (DistanceOperations op in distanceOps)
            {
                ToolStripMenuItem item = null;
                switch (op)
                {
                    case DistanceOperations.DistanceOperations_Beyond:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Beyond";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<property name> BEYOND <expression> <DOUBLE|INTEGER>"); };
                        }
                        break;
                    case DistanceOperations.DistanceOperations_Within:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Within";
                            item.Tag = op;
                            item.Click += delegate(object sender, EventArgs e) { InsertText("<property name> WITHIN <expression> <DOUBLE|INTEGER>"); };
                        }
                        break;
                }
                if (item != null)
                    btnDistance.DropDown.Items.Add(item);
            }
        }

        private void LoadConditionTypes(ConditionType[] conditions)
        {
            btnConditions.Visible = (conditions.Length > 0);
            foreach (ConditionType cond in conditions)
            {
                ToolStripMenuItem item = null;
                switch (cond)
                {
                    case ConditionType.ConditionType_Comparison:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Comparison";
                            item.Tag = cond;
                            item.DropDown.Items.Add("=", null, delegate(object sender, EventArgs e) { InsertText(" <property> = <value> "); });
                            item.DropDown.Items.Add(">", null, delegate(object sender, EventArgs e) { InsertText(" <property> > <value> "); });
                            item.DropDown.Items.Add("<", null, delegate(object sender, EventArgs e) { InsertText(" <property> < <value> "); });
                            item.DropDown.Items.Add("<>", null, delegate(object sender, EventArgs e) { InsertText(" <property> <> <value> "); });
                            item.DropDown.Items.Add("<=", null, delegate(object sender, EventArgs e) { InsertText(" <property> <= <value> "); });
                            item.DropDown.Items.Add(">=", null, delegate(object sender, EventArgs e) { InsertText(" <property> >= <value> "); });
                        }
                        break;
                    case ConditionType.ConditionType_Distance:
                        {
                            btnDistance.Visible = true;
                        }
                        break;
                    case ConditionType.ConditionType_In:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "In";
                            item.Tag = cond;
                            item.Click += delegate(object sender, EventArgs e) { InsertText(" <property> IN ( <comma-separated value list> ) "); };
                        }
                        break;
                    case ConditionType.ConditionType_Like:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Like";
                            item.Tag = cond;
                            item.Click += delegate(object sender, EventArgs e) { InsertText(" <property> LIKE <pattern> "); };
                        }
                        break;
                    case ConditionType.ConditionType_Null:
                        {
                            item = new ToolStripMenuItem();
                            item.Text = item.Name = "Null";
                            item.Tag = cond;
                            item.Click += delegate(object sender, EventArgs e) { InsertText(" <property> NULL "); };
                        }
                        break;
                    case ConditionType.ConditionType_Spatial:
                        {
                            //Not supported atm
                            btnSpatial.Visible = true;
                        }
                        break;
                }
                if (item != null)
                    btnConditions.DropDown.Items.Add(item);
            }
        }

        private void LoadProperties()
        {
            List<string> dataProps = new List<string>();
            foreach (PropertyDefinition propDef in _ClassDef.Properties)
            {
                _autoCompleteItems[propDef.Name] = new PropertyItem(propDef);
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = item.Name = propDef.Name;
                switch (propDef.PropertyType)
                {
                    case PropertyType.PropertyType_AssociationProperty:
                        item.Image = ResourceService.GetBitmap("table_relationship");
                        break;
                    case PropertyType.PropertyType_DataProperty:
                        {
                            DataPropertyDefinition dataDef = (DataPropertyDefinition)propDef;
                            item.Image = ResourceService.GetBitmap("table");
                            item.ToolTipText = string.Format("Data Type: {0}\nLength: {1}", dataDef.DataType, dataDef.Length);
                            dataProps.Add(propDef.Name);
                        }
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        item.Image = ResourceService.GetBitmap("shape_handles");
                        break;
                    case PropertyType.PropertyType_ObjectProperty:
                        item.Image = ResourceService.GetBitmap("package");
                        break;
                    case PropertyType.PropertyType_RasterProperty:
                        item.Image = ResourceService.GetBitmap("image");
                        break;
                }
                item.Click += new EventHandler(property_Click);
                insertPropertyToolStripMenuItem.DropDown.Items.Add(item);
            }
            cmbProperty.DataSource = dataProps;
        }

        private void LoadFunctionDefinitions(FunctionDefinitionCollection funcs)
        {
            foreach (FunctionDefinition funcDef in funcs)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = item.Name = funcDef.Name;
                item.ToolTipText = funcDef.Description;
                item.Tag = funcDef;
                item.Click += new EventHandler(function_Click);
                _FunctionMenuItems[funcDef.FunctionCategoryType].DropDown.Items.Add(item);

                _autoCompleteItems[funcDef.Name] = new FunctionItem(funcDef);
            }
        }

        private void LoadFunctionCategories(Array categories)
        {
            foreach (object category in categories)
            {
                string name = string.Empty;
                FunctionCategoryType funcCat = (FunctionCategoryType)category;
                switch (funcCat)
                {
                    case FunctionCategoryType.FunctionCategoryType_Aggregate:
                        name = "Aggregate";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_Conversion:
                        name = "Conversion";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_Custom:
                        name = "Custom";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_Date:
                        name = "Date";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_Geometry:
                        name = "Geometry";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_Math:
                        name = "Math";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_Numeric:
                        name = "Numeric";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_String:
                        name = "String";
                        break;
                    case FunctionCategoryType.FunctionCategoryType_Unspecified:
                        name = "Unspecified";
                        break;
                }
                if (!string.IsNullOrEmpty(name))
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Name = item.Text = name;
                    _FunctionMenuItems.Add(funcCat, item);
                    btnFunctions.DropDown.Items.Add(item);
                }
            }
        }

        void property_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                string exprText = item.Name;
                InsertText(exprText);
            }
        }

        /// <summary>
        /// Inserts the given text at the text selection region or at
        /// the position of the caret (if there is no selection)
        /// </summary>
        /// <param name="exprText"></param>
        private void InsertText(string exprText)
        {
            int index = txtExpression.SelectionStart;
            if (txtExpression.SelectionLength > 0)
            {
                txtExpression.SelectedText = exprText;
                txtExpression.SelectionStart = index;
            }
            else
            {
                if (index > 0)
                {
                    string text = txtExpression.Text;
                    txtExpression.Text = text.Insert(index, exprText);
                    txtExpression.SelectionStart = index;
                }
                else
                {
                    txtExpression.Text = exprText;
                    txtExpression.SelectionStart = index;
                }
            }
        }

        void function_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                FunctionDefinition funcDef = item.Tag as FunctionDefinition;
                string funcTemplate = "{0}( {1} )";
                List<string> parameters = new List<string>();
                foreach (ArgumentDefinition argDef in funcDef.Arguments)
                {
                    parameters.Add("<"+argDef.Name+">");
                }
                string exprText = string.Format(funcTemplate, funcDef.Name, string.Join(", ", parameters.ToArray()));
                InsertText(exprText);
            }
        }

        /// <summary>
        /// Edits the expression.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="classDef">The class def.</param>
        /// <param name="expr">The expr.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public static string EditExpression(FdoConnection conn, ClassDefinition classDef, Dictionary<string, ClassDefinition> aliasedClasses, string expr, ExpressionMode mode)
        {
            ExpressionEditor dlg = new ExpressionEditor(conn, classDef, aliasedClasses, mode);
            dlg.txtExpression.Text = expr;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.txtExpression.Text.Trim();
            }
            return null;
        }

        /// <summary>
        /// Creates a new expression.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="classDef">The class def.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public static string NewExpression(FdoConnection conn, ClassDefinition classDef, Dictionary<string, ClassDefinition> aliasedClasses, ExpressionMode mode)
        {
            ExpressionEditor dlg = new ExpressionEditor(conn, classDef, aliasedClasses, mode);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.txtExpression.Text.Trim();
            }
            return null;
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (ValidateExpression())
            {
                MessageService.ShowMessage(ResourceService.GetString("MSG_VALID_EXPRESSION"), ResourceService.GetString("TITLE_VALIDATE_EXPRESSION"));
            }
        }

        private bool ValidateExpression()
        {
            bool valid = true;
            try
            {
                if (!string.IsNullOrEmpty(txtExpression.Text))
                {
                    if (_ExprMode == ExpressionMode.Filter)
                    {
                        using (Filter fl = Filter.Parse(txtExpression.Text))
                        {
                            valid = true;
                        }
                    }
                    else
                    {
                        using (Expression expr = Expression.Parse(txtExpression.Text))
                        {
                            valid = true;
                        }
                    }
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                MessageService.ShowError(ex.Message);
                valid = false;
            }
            return valid;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.ValidateExpression())
                this.DialogResult = DialogResult.OK;
        }

        private void pointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("POINT <dimensionality> (< [x y] coordinate pair>)");
        }

        private void lineStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("LINESTRING <dimensionality> (<list of [x y] coordinate pairs>)");
        }

        private void polygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("POLYGON <dimensionality> ((<list of [x y] coordinate pairs>),(<list of [x y] coordinate pairs>))");
        }

        private void curveStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("CURVESTRING <dimensionality> (<point> (<curve segment collection>))");
        }

        private void curvePolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("CURVEPOLYGON <dimensionality> (<curve string collection>)");
        }

        private void txtExpression_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKeyUp(e);
        }

        private void HandleKeyUp(KeyEventArgs e)
        {
            Keys code = e.KeyCode;
            if (code == Keys.Oemcomma || code == Keys.OemOpenBrackets)
            {
                Complete(string.Empty);
            }
            else if (code == Keys.OemQuotes)
            {
                if (e.Modifiers == Keys.Shift)  // "
                    InsertText("\"");
                else                            // '
                    InsertText("'");
                    
            }
            else if (code == Keys.D9 && e.Modifiers == Keys.Shift) // (
            {
                InsertText(")");
            }
            else if (code == Keys.Up || code == Keys.Down)
            {
                if (_autoBox.Visible)
                {
                    if (code == Keys.Up)
                    {
                        MoveAutoCompleteSelectionUp();
                    }
                    else
                    {
                        MoveAutoCompleteSelectionDown();
                    }
                }
            }
            else if (code == Keys.Back || code == Keys.Space)
            {
                string context;
                char? c = GetContextBuffer(out context);
                if (!string.IsNullOrEmpty(context))
                {
                    Complete(context);
                }
                else
                {
                    if (_autoBox.Visible)
                    {
                        _autoBox.Hide();
                        _autoCompleteTooltip.Hide(this);
                    }
                }
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Right)
            {
                string context;
                char? c = GetContextBuffer(out context);
                Complete(context);
            }
            else
            {
                if (e.Modifiers == Keys.None)
                {
                    bool alpha = (code >= Keys.A && code <= Keys.Z);
                    bool numeric = (code >= Keys.D0 && code <= Keys.D9) || (code >= Keys.NumPad0 && code <= Keys.NumPad9);
                    if (alpha || numeric || e.KeyValue == 190) //190 = .
                    {
                        string context;
                        char? c = GetContextBuffer(out context);
                        Complete(context);
                    }
                }
            }
        }

        private void txtExpression_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDown(e);
        }

        private void HandleKeyDown(KeyEventArgs e)
        {
            Keys code = e.KeyCode;
            if (code == Keys.Escape)
            {
                if (_autoBox.Visible)
                {
                    e.SuppressKeyPress = true;
                    _autoBox.Hide();
                    _autoCompleteTooltip.Hide(this);
                }
            }
            else if (code == Keys.Enter || code == Keys.Return)
            {
                if (_autoBox.Visible && _autoBox.SelectedItems.Count == 1)
                {
                    e.SuppressKeyPress = true;
                    PutAutoCompleteSuggestion();
                    _autoBox.Hide();
                    _autoCompleteTooltip.Hide(this);
                }
            }
        }

        private void MoveAutoCompleteSelectionDown()
        {
            if (_autoBox.SelectedIndex < 0)
            {
                _autoBox.SelectedIndex = 0;
            }
            else
            {
                int idx = _autoBox.SelectedIndex;
                if ((idx + 1) <= _autoBox.Items.Count - 1)
                {
                    _autoBox.SelectedIndex = idx + 1;
                }
            }
        }

        private void MoveAutoCompleteSelectionUp()
        {
            if (_autoBox.SelectedIndex < 0)
            {
                _autoBox.SelectedIndex = 0;
            }
            else
            {
                int idx = _autoBox.SelectedIndex;
                if ((idx - 1) >= 0)
                {
                    _autoBox.SelectedIndex = idx - 1;
                }
            }
        }

        private List<AutoCompleteItem> GetItemsStartingWith(string text)
        {
            List<AutoCompleteItem> ati = new List<AutoCompleteItem>();
            if (text.Length > 1 && text[text.Length - 1] == '.')
            {
                string alias = text.Substring(0, text.Length - 1);
                if (_aliasedClasses.ContainsKey(alias))
                {
                    var cls = _aliasedClasses[alias];
                    foreach (PropertyDefinition prop in cls.Properties)
                    {
                        var pi = new PropertyItem(prop);
                        ati.Add(pi);
                    }
                }
            }
            else
            {
                foreach (string key in _autoCompleteItems.Keys)
                {
                    if (key.ToLower().StartsWith(text.Trim().ToLower()))
                    {
                        ati.Add(_autoCompleteItems[key]);
                    }
                }
            }
            return ati;
        }

        void OnAutoCompleteKeyDown(object sender, KeyEventArgs e)
        {
            txtExpression.Focus();
        }

        void OnAutoCompleteKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                PutAutoCompleteSuggestion();
                _autoBox.Hide();
                _autoCompleteTooltip.Hide(this);
            }
        }

        void OnAutoCompleteSelectedIndexChanged(object sender, EventArgs e)
        {
            txtExpression.Focus();
            if (_autoBox.Visible && _autoBox.SelectedIndex >= 0 && _autoBox.Items.Count > 0)
            {
                string tt = ((_autoBox.SelectedItem as ImageListBoxItem).Tag as AutoCompleteItem).ToolTipText;
                Point pt = GetCaretPoint();
                pt.X += _autoBox.Width + 10;
                pt.Y += 65;

                _autoCompleteTooltip.Show(tt, this, pt.X, pt.Y);
            }
        }

        void OnAutoCompleteDoubleClick(object sender, EventArgs e)
        {
            PutAutoCompleteSuggestion();
            _autoBox.Hide();
            _autoCompleteTooltip.Hide(this);
        }

        private void PutAutoCompleteSuggestion()
        {
            if (_autoBox.SelectedItems.Count == 1)
            {
                int pos = txtExpression.SelectionStart;
                string context;
                char? c = GetContextBuffer(out context);

                AutoCompleteItem aci = (_autoBox.SelectedItem as ImageListBoxItem).Tag as AutoCompleteItem;

                string fullText = aci.AutoCompleteText;

                int start = pos - context.Length;
                if (context.EndsWith("."))
                    start = pos;
                int newPos = start + fullText.Length;
                int selLength = -1;
                
                //if it's a function, highlight the parameter (or the first parameter if there is multiple arguments
                if (aci.Type == AutoCompleteItemType.Function)
                {
                    newPos = start + aci.Name.Length + 1; //Position the caret just after the opening bracket
                    
                    //Has at least two arguments
                    int idx = fullText.IndexOf(",");
                    if (idx > 0)
                        selLength = idx - aci.Name.Length - 1;
                    else
                        selLength = fullText.IndexOf(")") - fullText.IndexOf("(") - 1;
                }

                string prefix = txtExpression.Text.Substring(0, start);
                string suffix = txtExpression.Text.Substring(pos, txtExpression.Text.Length - pos);

                txtExpression.Text = prefix + fullText + suffix;
                txtExpression.SelectionStart = newPos;
                if (selLength > 0)
                {
                    txtExpression.SelectionLength = selLength;
                }
                txtExpression.ScrollToCaret();
            }
        }

        private void Complete(string text)
        {
            List<AutoCompleteItem> items = GetItemsStartingWith(text);
            _autoBox.Items.Clear();

            int width = 0;
            foreach (AutoCompleteItem it in items)
            {
                ImageListBoxItem litem = new ImageListBoxItem();
                litem.Text = it.Name;
                litem.ImageIndex = (int)it.Type;
                litem.Tag = it;

                _autoBox.Items.Add(litem);
                int length = TextRenderer.MeasureText(it.Name, _autoBox.Font).Width + 30; //For icon size
                if (length > width)
                    width = length;
            }
            _autoBox.Width = width;

            if (!_autoBox.Visible)
            {
                if (_autoBox.Items.Count > 0)
                {
                    _autoBox.BringToFront();
                    _autoBox.Show();
                }
            }

            Point pt = GetCaretPoint();

            _autoBox.Location = pt;
        }

        private Point GetCaretPoint()
        {
            Point pt = txtExpression.GetPositionFromCharIndex(txtExpression.SelectionStart);
            pt.Y += (int)Math.Ceiling(this.txtExpression.Font.GetHeight()) + 2;
            pt.X += 2; // for Courier, may need a better method
            return pt;
        }

        private char? GetContextBuffer(out string buffer)
        {
            buffer = string.Empty;
            int caretPos = txtExpression.SelectionStart;
            int currentPos = caretPos;
            char? res = null;
            if (caretPos > 0)
            {
                //Walk backwards
                caretPos--;
                char c = txtExpression.Text[caretPos];
                while(Char.IsLetterOrDigit(c) || c == '.')
                {
                    caretPos--;
                    if (caretPos < 0)
                        break;

                    c = txtExpression.Text[caretPos];
                }

                if (caretPos > 0)
                {
                    res = txtExpression.Text[caretPos];
                }
                buffer = txtExpression.Text.Substring(caretPos + 1, currentPos - caretPos - 1);
            }
            return res;
        }

        private void btnGetValues_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
                splitContainer1.Panel2Collapsed = false;
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = true;
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            if (cmbProperty.SelectedItem != null)
            {
                string prop = cmbProperty.SelectedItem.ToString();

                //Try in order of support
                //
                // 1 - ISelectAggregate w/ distinct = true
                // 2 - "SELECT DISTINCT [property] FROM [table]"
                // 3 - Brute force. Prompt user for confirmation first

                using (new TempCursor(Cursors.WaitCursor))
                {
                    using (FdoFeatureService svc = _conn.CreateFeatureService())
                    {
                        bool supportsDistinct = _conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSelectDistinct);
                        if (svc.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates) && supportsDistinct)
                        {
                            //SortedList not only allows us to hackishly get set-like qualities, but we get sorting for free.
                            SortedList<string, string> values = new SortedList<string, string>();
                            FeatureAggregateOptions opts = new FeatureAggregateOptions(_ClassDef.Name);
                            opts.AddFeatureProperty(prop);
                            opts.Distinct = true;
                            using (IFdoReader reader = svc.SelectAggregates(opts))
                            {
                                while (reader.ReadNext())
                                {
                                    if (!reader.IsNull(prop))
                                    {
                                        values.Add(reader[prop].ToString(), string.Empty);
                                    }
                                }
                            }
                            lstValues.DataSource = new List<string>(values.Keys);
                            lblValueCount.Text = "(" + values.Keys.Count + ")";
                        }
                        else if (svc.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand))
                        {
                            string sql = string.Format("SELECT DISTINCT {0} FROM {1} ORDER BY {0}", prop, _ClassDef.Name);
                            List<string> values = new List<string>();
                            using (IFdoReader reader = svc.ExecuteSQLQuery(sql))
                            {
                                while (reader.ReadNext())
                                {
                                    if (!reader.IsNull(prop))
                                    {
                                        values.Add(reader[prop].ToString());
                                    }
                                }
                            }
                            lstValues.DataSource = values;
                            lblValueCount.Text = "(" + values.Count + ")";
                        }
                        else
                        {
                            if (MessageService.AskQuestion("About to fetch distinct values by brute force. Continue?", "Get Values"))
                            {
                                //SortedList not only allows us to hackishly get set-like qualities, but we get sorting for free.
                                SortedList<string, string> values = new SortedList<string, string>();

                                FeatureQueryOptions query = new FeatureQueryOptions(_ClassDef.QualifiedName);
                                query.AddFeatureProperty(prop);

                                using (IFdoReader reader = svc.SelectFeatures(query))
                                {
                                    while (reader.ReadNext())
                                    {
                                        if (!reader.IsNull(prop))
                                        {
                                            values.Add(reader[prop].ToString(), string.Empty);
                                        }
                                    }
                                }

                                lstValues.DataSource = new List<string>(values.Keys);
                                lblValueCount.Text = "(" + values.Keys.Count + ")";
                            }
                        }
                    }
                }
            }
        }

        private void lstValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstValues.SelectedItem != null)
            {
                InsertText(lstValues.SelectedItem.ToString());
            }
        }

        private void usingGeometryVisualizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeometryVisualizer vis = new GeometryVisualizer();
            if (vis.ShowDialog() == DialogResult.OK)
            {
                InsertText(vis.GeometryText);
            }
        }
    }
}