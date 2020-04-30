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
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    internal partial class FdoBulkDeleteCtl : ViewContent, IFdoBulkDeleteView
    {
        private FdoBulkDeleteCtlPresenter _presenter;
        private FdoConnection _conn;

        internal FdoBulkDeleteCtl()
        {
            InitializeComponent();
            this.Disposed += delegate
            {
                if (_classDef != null)
                {
                    _classDef.Dispose();
                    _classDef = null;
                }
            };
        }

        public FdoBulkDeleteCtl(FdoConnection conn, string className)
            : this()
        {
            _conn = conn;
            _presenter = new FdoBulkDeleteCtlPresenter(this, conn, className);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _presenter.Test();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _presenter.Cancel();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            _presenter.Delete();
        }

        private OSGeo.FDO.Schema.ClassDefinition _classDef;

        private void txtFilter_Click(object sender, EventArgs e)
        {
            if (_classDef == null)
            {
                using (FdoFeatureService service = _conn.CreateFeatureService())
                {
                    _classDef = service.GetClassByName(this.ClassName);
                }
            }
            if (!string.IsNullOrEmpty(this.Filter))
            {
                string filter = ExpressionEditor.EditExpression(_conn, _classDef, null, this.Filter, ExpressionMode.Filter);
                if (filter != null)
                    this.Filter = filter;
            }
            else
            {
                this.Filter = ExpressionEditor.NewExpression(_conn, _classDef, null, ExpressionMode.Filter);
            }
        }

        public string ClassName
        {
            get
            {
                return lblClass.Text;
            }
            set
            {
                lblClass.Text = value;
            }
        }

        public string Filter
        {
            get
            {
                return txtFilter.Text;
            }
            set
            {
                txtFilter.Text = value;
            }
        }


        public bool UseTransaction
        {
            get
            {
                return chkTransaction.Checked;
            }
            set
            {
                chkTransaction.Checked = value;
            }
        }

        public bool TransactionEnabled
        {
            get
            {
                return chkTransaction.Enabled;
            }
            set
            {
                chkTransaction.Enabled = value;
            }
        }
    }
}
