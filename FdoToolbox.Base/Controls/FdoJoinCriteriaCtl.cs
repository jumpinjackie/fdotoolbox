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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoJoinCriteriaCtl : UserControl
    {
        public FdoJoinCriteriaCtl()
        {
            InitializeComponent();
        }

        public string ClassAlias => txtClassAlias.Text;

        public FdoConnection Connection { get; set; }

        public ClassDefinition SelectedClass { get; set; }

        private void btnRemoveJoin_Click(object sender, EventArgs e)
        {
            lstJoins.Items.RemoveAt(lstJoins.SelectedIndex);
        }

        private void btnAddJoin_Click(object sender, EventArgs e)
        {
            var cls = this.SelectedClass;
            var schema = (FeatureSchema)cls.Parent;
            using (var diag = new FdoJoinDialog(this.Connection, schema.Name, cls.Name, txtClassAlias.Text))
            {
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    lstJoins.Items.Add(diag.Criteria);
                }
            }
        }

        public IEnumerable<FdoJoinCriteriaInfo> JoinCriteria
        {
            get
            {
                foreach (FdoJoinCriteriaInfo criteria in lstJoins.Items)
                {
                    yield return criteria;
                }
            }
        }

        private void lstJoins_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = btnRemoveJoin.Enabled = lstJoins.SelectedIndex >= 0;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var criteria = (FdoJoinCriteriaInfo)lstJoins.SelectedItem;
            var cls = this.SelectedClass;
            var schema = (FeatureSchema)cls.Parent;
            using (var diag = new FdoJoinDialog(this.Connection, schema.Name, cls.Name, txtClassAlias.Text, criteria))
            {
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    var items = new System.Collections.ArrayList(lstJoins.Items);
                    lstJoins.Items.Clear();
                    lstJoins.Items.AddRange(items.ToArray());
                    btnEdit.Enabled = btnRemoveJoin.Enabled = false;
                }
            }
        }
    }
}
