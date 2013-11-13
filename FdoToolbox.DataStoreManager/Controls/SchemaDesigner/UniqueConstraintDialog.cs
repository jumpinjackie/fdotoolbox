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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal partial class UniqueConstraintDialog : Form
    {
        private List<UniqueConstraintInfo> _constraints = new List<UniqueConstraintInfo>();
        private BindingSource bs = new BindingSource();

        public UniqueConstraintDialog()
        {
            InitializeComponent();
            lstUniqueConstraints.DataSource = bs;
            bs.DataSource = _constraints;
        }

        internal IList<UniqueConstraintInfo> Constraints
        {
            get { return _constraints; }
            set
            {
                bs.Clear();
                foreach (UniqueConstraintInfo uniq in value)
                {
                    bs.Add(uniq);
                }
            }
        }

        public IList<string> PropertyNames
        {
            set
            {
                chkProperties.Items.Clear();
                foreach (string str in value)
                {
                    chkProperties.Items.Add(str, false);
                }
            }
        }

        private void lstUniqueConstraints_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = (lstUniqueConstraints.SelectedItem != null);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstUniqueConstraints.SelectedItem != null)
            {
                bs.Remove(lstUniqueConstraints.SelectedItem);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            foreach (object o in chkProperties.CheckedItems)
            {
                names.Add(o.ToString());
            }

            UniqueConstraintInfo uniq = new UniqueConstraintInfo(names.ToArray());
            if (bs.Contains(uniq))
            {
                MessageService.ShowError("This unique constraint already exists");
            }
            else
            {
                bs.Add(uniq);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }

    internal class UniqueConstraintInfo
    {
        private string[] _names;
        private string _str;

        public UniqueConstraintInfo(string[] names)
        {
            _names = names;
            _str = string.Join(",", names);
        }

        public string[] PropertyNames
        {
            get { return _names; }
        }

        public override string ToString()
        {
            return _str;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(UniqueConstraintInfo))
            {
                return this.ToString() == (obj as UniqueConstraintInfo).ToString();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _str.GetHashCode();
        }
    }
}