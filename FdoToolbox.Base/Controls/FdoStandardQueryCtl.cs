#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A <see cref="FdoDataPreviewCtl"/> sub-view for standard FDO queries
    /// </summary>
    internal partial class FdoStandardQueryCtl : UserControl, IFdoStandardQueryView
    {
        private FdoStandardQueryPresenter _presenter;

        internal FdoStandardQueryCtl()
        {
            InitializeComponent();
            cmbOrderingOption.DataSource = Enum.GetValues(typeof(OSGeo.FDO.Commands.OrderingOption));
        }

        private FdoConnection _conn;

        public FdoStandardQueryCtl(FdoConnection conn) : this()
        {
            _conn = conn;
            _presenter = new FdoStandardQueryPresenter(this, conn);
            joinCriteriaCtrl.Connection = conn;
        }

        private string _initSchema;
        private string _initClass;

        public FdoStandardQueryCtl(FdoConnection conn, string initSchema, string initClass)
            : this(conn)
        {
            _initSchema = initSchema;
            _initClass = initClass;
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.GetSchemas();
            if (!string.IsNullOrEmpty(_initSchema))
            {
                int idx = cmbSchema.FindString(_initSchema);
                if (idx >= 0)
                    cmbSchema.SelectedIndex = idx;

                if (!string.IsNullOrEmpty(_initClass))
                {
                    idx = cmbClass.FindString(_initClass);
                    if (idx >= 0)
                        cmbClass.SelectedIndex = idx;
                }
            }
            base.OnLoad(e);
        }

        public string[] SchemaList
        {
            set { cmbSchema.DataSource = value; }
        }

        public ClassDescriptor[] ClassList
        {
            set { cmbClass.DisplayMember = "ClassName"; cmbClass.DataSource = value; }
        }

        public string SelectedSchema
        {
            get { return cmbSchema.SelectedItem != null ? cmbSchema.SelectedItem.ToString() : null; }
        }

        public ClassDescriptor SelectedClass
        {
            get { return cmbClass.SelectedItem as ClassDescriptor; }
        }

        public IList<string> PropertyList
        {
            set
            {
                List<string> ps = new List<string>(value);
                List<string> po = new List<string>(value);

                chkProperties.Items.Clear();
                foreach (string p in ps)
                {
                    chkProperties.Items.Add(p, true);
                }

                lstOrderableProperties.Items.Clear();
                foreach (string p in po)
                {
                    lstOrderableProperties.Items.Add(p);
                }
            }
        }

        public IList<string> SelectPropertyList
        {
            get
            {
                IList<string> p = new List<string>();
                foreach (object o in chkProperties.CheckedItems)
                {
                    p.Add(o.ToString());
                }
                return p;
            }
        }

        public IList<string> AllClassProperties
        {
            get
            {
                IList<string> p = new List<string>();
                foreach (object o in chkProperties.Items)
                {
                    p.Add(o.ToString());
                }
                return p;
            }
        }

        public IList<string> OrderByList
        {
            get
            {
                IList<string> p = new List<string>();
                foreach (object o in lstOrderBy.Items)
                {
                    p.Add(o.ToString());
                }
                return p;
            }
        }

        public IList<ComputedProperty> ComputedProperties
        {
            get
            {
                List<ComputedProperty> p = new List<ComputedProperty>();
                foreach (ListViewItem item in lstComputed.Items)
                {
                    string alias = item.SubItems[0].Text;
                    string expr = item.SubItems[1].Text;
                    p.Add(new ComputedProperty(alias, expr));
                }
                return p;
            }
            set
            {
                lstComputed.Items.Clear();
                foreach (ComputedProperty p in value)
                {
                    ListViewItem item = new ListViewItem(new string[]{p.Alias, p.Expression});
                    lstComputed.Items.Add(item);
                }
            }
        }

        public string Filter
        {
            get
            {
                return txtFilter.Text.Trim();
            }
            set
            {
                txtFilter.Text = value;
            }
        }

        public Control ContentControl
        {
            get { return this; }
        }

        private void cmbSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SchemaChanged();
        }

        private void cmbClass_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ClassChanged();
            joinCriteriaCtrl.SelectedClass = _presenter.SelectedClass;
        }

        private void txtFilter_Enter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Filter))
            {
                string filter = ExpressionEditor.EditExpression(_conn, _presenter.SelectedClass, null, this.Filter, ExpressionMode.Filter);
                if (filter != null)
                    this.Filter = filter;
            }
            else
            {
                this.Filter = ExpressionEditor.NewExpression(_conn, _presenter.SelectedClass, null, ExpressionMode.Filter);
            }
        }

        public int Limit
        {
            get { return Convert.ToInt32(numLimit.Value); }
        }

        public OSGeo.FDO.Commands.OrderingOption Ordering
        {
            get { return (OSGeo.FDO.Commands.OrderingOption)cmbOrderingOption.SelectedItem; }
        }

        public FeatureQueryOptions QueryObject
        {
            get 
            {
                FeatureQueryOptions options = new FeatureQueryOptions(this.SelectedClass.QualifiedName);
                if (!string.IsNullOrEmpty(txtFilter.Text))
                    options.Filter = txtFilter.Text;

                IList<string> sp = this.SelectPropertyList;
                IList<string> op = this.OrderByList;
                IList<ComputedProperty> cp = this.ComputedProperties;
                bool joinVisible = tabQueryOptions.Contains(TAB_JOINS);

                string classAlias = "";
                if (joinVisible)
                {
                    classAlias = joinCriteriaCtrl.ClassAlias;
                    options.ClassAlias = classAlias;
                }

                if (sp.Count > 0)
                {
                    foreach (var prop in sp)
                    {
                        if (joinVisible && !string.IsNullOrEmpty(classAlias))
                            options.AddFeatureProperty(classAlias + "." + prop);
                        else
                            options.AddFeatureProperty(prop);
                    }
                }
                else
                {
                    if (joinVisible && !string.IsNullOrEmpty(classAlias))
                    {
                        foreach (var prop in this.AllClassProperties)
                        {
                            //[alias].[propertyName]
                            options.AddFeatureProperty(classAlias + "." + prop);
                        }
                    }
                }

                if (cp.Count > 0)
                {
                    foreach (ComputedProperty p in cp)
                    {
                        options.AddComputedProperty(p.Alias, p.Expression);
                    }
                }

                if (op.Count > 0)
                {
                    options.SetOrderingOption(op, this.Ordering);
                }

                if (joinVisible)
                {
                    using (var svc = _conn.CreateFeatureService())
                    {
                        foreach (FdoJoinCriteriaInfo join in joinCriteriaCtrl.JoinCriteria)
                        {
                            options.AddJoinCriteria(join);
                            ClassDefinition clsDef = svc.GetClassByName(join.JoinSchema, join.JoinClass);
                            foreach (PropertyDefinition prop in clsDef.Properties)
                            {
                                if (prop.PropertyType == PropertyType.PropertyType_DataProperty ||
                                    prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                                {
                                    if (string.IsNullOrEmpty(join.JoinPrefix))
                                        options.AddFeatureProperty(join.JoinClassAlias + "." + prop.Name);
                                    else
                                        options.AddComputedProperty(join.JoinPrefix + prop.Name, join.JoinClassAlias + "." + prop.Name);
                                }
                            }
                        }
                    }
                }

                return options;
            }
        }

        private void btnRemoveOrderBy_Click(object sender, EventArgs e)
        {
            _presenter.RemoveOrderByProperty();
        }

        private void btnAddOrderBy_Click(object sender, EventArgs e)
        {
            _presenter.AddOrderByProperty();
        }

        public string SelectedOrderByProperty
        {
            get 
            {
                if(lstOrderBy.SelectedItem != null)
                    return lstOrderBy.SelectedItem.ToString();
                return null;
            }
        }

        public string SelectedOrderByPropertyToAdd
        {
            get 
            { 
                if(lstOrderableProperties.SelectedItem != null)
                    return lstOrderableProperties.SelectedItem.ToString();
                return null;
            }
        }

        public void AddOrderBy(string prop)
        {
            lstOrderBy.Items.Add(prop);
        }

        public void RemoveOrderBy(string prop)
        {
            lstOrderBy.Items.Remove(prop);
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            CheckAllProperties(true);
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            CheckAllProperties(false);
        }

        private void CheckAllProperties(bool isChecked)
        {
            for (int i = 0; i < chkProperties.Items.Count; i++)
            {
                chkProperties.SetItemChecked(i, isChecked);
            }
        }

        private void btnAddComputed_Click(object sender, EventArgs e)
        {
            string expr = ExpressionEditor.NewExpression(_conn, _presenter.SelectedClass, null, ExpressionMode.Normal);
            if (expr != null)
            {
                string name = "";
                //Test to see if it is a computed identiifer
                using (var exp = OSGeo.FDO.Expression.Expression.Parse(expr))
                {
                    var comp = exp as OSGeo.FDO.Expression.ComputedIdentifier;
                    if (comp != null)
                    {
                        name = GetExpressionAlias(comp.Name);
                        expr = comp.Expression.ToString();
                    }
                    else
                    {
                        name = GetExpressionAlias("Expr0");
                    }
                }
                lstComputed.Items.Add(new ListViewItem(new string[] { name, expr }));
            }
        }

        private void btnEditComputed_Click(object sender, EventArgs e)
        {
            ListViewItem item = lstComputed.SelectedItems[0];
            string alias = item.SubItems[0].Text;
            string exprText = "";
            using(var expr = OSGeo.FDO.Expression.Expression.Parse(item.SubItems[1].Text))
            {
                using (var comp = new OSGeo.FDO.Expression.ComputedIdentifier(alias, expr))
                {
                    exprText = comp.ToString();
                }
            }
            exprText = ExpressionEditor.EditExpression(_conn, _presenter.SelectedClass, null, exprText, ExpressionMode.Normal);
            if (exprText != null)
            {
                //Test to see if it is a computed identifier
                using (var expr = OSGeo.FDO.Expression.Expression.Parse(exprText))
                {
                    var comp = expr as OSGeo.FDO.Expression.ComputedIdentifier;
                    if (comp != null)
                    {
                        exprText = comp.Expression.ToString();
                        alias = GetExpressionAlias(comp.Name);
                    }
                    else
                    {
                        alias = GetExpressionAlias("Expr0");
                    }
                }
                item.SubItems[0].Text = alias;
                item.SubItems[1].Text = exprText;
            }
        }

        private string GetExpressionAlias(string alias)
        {
            //This may already exist, if so use default name
            int count = 0;
            while (lstComputed.Items.Cast<ListViewItem>().Where(x => x.SubItems[0].Text.ToUpper() == alias.ToUpper()).Any())
            {
                alias = "Expr" + count;
                count++;
            }
            return alias;
        }

        private void lstComputed_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEditComputed.Enabled = lstComputed.SelectedItems.Count > 0;
        }

        public bool OrderingEnabled
        {
            get
            {
                return tabQueryOptions.TabPages.Contains(TAB_ORDERING);
            }
            set
            {
                if (value)
                {
                    if (!tabQueryOptions.TabPages.Contains(TAB_ORDERING))
                        tabQueryOptions.TabPages.Add(TAB_ORDERING);
                }
                else
                {
                    tabQueryOptions.TabPages.Remove(TAB_ORDERING);
                }
            }
        }

        private bool _useExtendedSelect;

        public bool UseExtendedSelectForOrdering
        {
            get
            {
                return _useExtendedSelect;
            }
            set
            {
                _useExtendedSelect = value;
            }
        }

        public void FireMapPreviewStateChanged(bool enabled)
        {
            MapPreviewStateChanged(this, enabled);
        }

        public event MapPreviewStateEventHandler MapPreviewStateChanged = delegate { };

        public void SetRestrictions(ICapability cap)
        {
            bool bExtended = Array.IndexOf(cap.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList), OSGeo.FDO.Commands.CommandType.CommandType_ExtendedSelect) >= 0;

            if (!cap.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSelectOrdering) && !bExtended)
                tabQueryOptions.TabPages.Remove(TAB_ORDERING);

            if (!cap.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsJoins))
                tabQueryOptions.TabPages.Remove(TAB_JOINS);
        }

        public ClassDefinition SelectedClassDefinition
        {
            get { return _presenter.SelectedClass; }
        }
    }
}
