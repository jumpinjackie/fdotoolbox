#region LGPL Header
// Copyright (C) 2011, Jackie Ng
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
using System.Windows.Forms;
using FdoToolbox.Base.Forms;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoJoinDialog : Form
    {
        private FdoConnection _conn;

        private string _primarySchemaName;
        private string _primaryClassName;
        private string _primaryClassAlias;

        public FdoJoinDialog(FdoConnection conn, string primarySchemaName, string primaryClassName, string primaryClassAlias)
        {
            InitializeComponent();
            _conn = conn;
            _primarySchemaName = primarySchemaName;
            _primaryClassName = primaryClassName;
            _primaryClassAlias = primaryClassAlias;
        }

        public FdoJoinDialog(FdoConnection conn, string primarySchemaName, string primaryClassName, string primaryClassAlias, FdoJoinCriteriaInfo criteria)
            : this(conn, primarySchemaName, primaryClassName, primaryClassAlias)
        {
            this.Criteria = criteria;
        }

        public FdoJoinCriteriaInfo Criteria
        {
            get;
            private set;
        }

        protected override void OnLoad(EventArgs e)
        {
            cmbJoinType.DataSource = _conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_JoinTypes);
            using (var svc = _conn.CreateFeatureService())
            {
                cmbSchema.DataSource = svc.DescribeSchema();
                cmbSchema.SelectedIndex = 0;
            }

            if (this.Criteria != null)
            {
                foreach (FeatureSchema fs in (FeatureSchemaCollection)cmbSchema.DataSource)
                {
                    if (fs.Name == this.Criteria.JoinSchema)
                    {
                        cmbSchema.SelectedIndex = cmbSchema.Items.IndexOf(fs);
                        foreach (ClassDefinition cls in fs.Classes)
                        {
                            if (cls.Name == this.Criteria.JoinClass)
                            {
                                cmbClass.SelectedIndex = cmbClass.Items.IndexOf(cls);

                                txtJoinClassAlias.Text = this.Criteria.JoinClassAlias;
                                txtJoinFilter.Text = this.Criteria.JoinFilter;
                                txtPrefix.Text = this.Criteria.JoinPrefix;

                                cmbJoinType.SelectedItem = this.Criteria.JoinType;
                            }
                        }
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            if (this.Criteria != null)
            {
                this.Criteria.JoinPrefix = txtPrefix.Text;
                this.Criteria.JoinSchema = ((FeatureSchema)cmbSchema.SelectedItem).Name;
                this.Criteria.JoinClass = ((ClassDefinition)cmbClass.SelectedItem).Name;
                this.Criteria.JoinClassAlias = txtJoinClassAlias.Text;
                this.Criteria.JoinFilter = txtJoinFilter.Text;
                this.Criteria.JoinType = ((OSGeo.FDO.Expression.JoinType)cmbJoinType.SelectedItem);
            }
            else
            {
                this.Criteria = new FdoJoinCriteriaInfo()
                {
                    JoinPrefix = txtPrefix.Text,
                    JoinSchema = ((FeatureSchema)cmbSchema.SelectedItem).Name,
                    JoinClass = ((ClassDefinition)cmbClass.SelectedItem).Name,
                    JoinClassAlias = txtJoinClassAlias.Text,
                    JoinFilter = txtJoinFilter.Text,
                    JoinType = ((OSGeo.FDO.Expression.JoinType)cmbJoinType.SelectedItem)
                };
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            var cls = cmbClass.SelectedItem as ClassDefinition;
            if (cls != null)
            {
                var aliasedClasses = new Dictionary<string, ClassDefinition>();
                aliasedClasses.Add(txtJoinClassAlias.Text, cls);

                bool found = false;
                foreach (FeatureSchema fs in (FeatureSchemaCollection)cmbSchema.DataSource)
                {
                    if (fs.Name == _primarySchemaName)
                    {
                        foreach (ClassDefinition cd in fs.Classes)
                        {
                            if (cd.Name == _primaryClassName)
                            {
                                aliasedClasses.Add(_primaryClassAlias, cd);
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found)
                        break;
                }

                string expr = ExpressionEditor.EditExpression(_conn, cls, aliasedClasses, txtJoinFilter.Text, ExpressionMode.Filter);
                if (expr != null)
                    txtJoinFilter.Text = expr;
            }
        }

        private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            var schema = cmbSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                cmbClass.DataSource = schema.Classes;
                cmbClass.SelectedIndex = 0;
            }
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = cmbClass.SelectedIndex >= 0;
        }
    }
}
