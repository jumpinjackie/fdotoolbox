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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal partial class ValueConstraintDialog : Form
    {
        internal ValueConstraintDialog()
        {
            InitializeComponent();
            cmbConstraintType.DataSource = Enum.GetValues(typeof(PropertyValueConstraintType));
            cmbConstraintType_SelectedIndexChanged(null, null);
        }

        public ValueConstraintDialog(FdoConnection conn)
        {
            InitializeComponent();
            bool supportsList = conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsValueConstraintsList);
            bool supportsRange = conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints);

            if (supportsList)
                cmbConstraintType.Items.Add(PropertyValueConstraintType.PropertyValueConstraintType_List);
            if (supportsRange)
                cmbConstraintType.Items.Add(PropertyValueConstraintType.PropertyValueConstraintType_Range);

            if (cmbConstraintType.Items.Count > 0)
                cmbConstraintType.SelectedIndex = 0;
            //cmbConstraintType_SelectedIndexChanged(null, null);
        }

        public static OSGeo.FDO.Schema.PropertyValueConstraint GetConstraint()
        {
            ValueConstraintDialog dlg = new ValueConstraintDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.Constraint;
            }
            return null;
        }

        public static OSGeo.FDO.Schema.PropertyValueConstraint GetConstraint(PropertyValueConstraint constraint)
        {
            ValueConstraintDialog dlg = new ValueConstraintDialog();
            dlg.Constraint = constraint;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.Constraint;
            }
            return null;
        }

        public static OSGeo.FDO.Schema.PropertyValueConstraint GetConstraint(PropertyValueConstraint constraint, FdoConnection conn)
        {
            ValueConstraintDialog dlg = new ValueConstraintDialog(conn);
            dlg.Constraint = constraint;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.Constraint;
            }
            return null;
        }

        public static OSGeo.FDO.Schema.PropertyValueConstraint GetConstraint(FdoConnection conn)
        {
            ValueConstraintDialog dlg = new ValueConstraintDialog(conn);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.Constraint;
            }
            return null;
        }

        private void cmbConstraintType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyValueConstraintType ctype = (PropertyValueConstraintType)cmbConstraintType.SelectedItem;
            switch (ctype)
            {
                case PropertyValueConstraintType.PropertyValueConstraintType_Range:
                    {
                        RangeConstraintSettingsCtl ctl = new RangeConstraintSettingsCtl();
                        ctl.Dock = DockStyle.Fill;
                        grpSettings.Controls.Clear();
                        grpSettings.Controls.Add(ctl);
                    }
                    break;
                case PropertyValueConstraintType.PropertyValueConstraintType_List:
                    {
                        ListConstraintSettingsCtl ctl = new ListConstraintSettingsCtl();
                        ctl.Dock = DockStyle.Fill;
                        grpSettings.Controls.Clear();
                        grpSettings.Controls.Add(ctl);
                    }
                    break;
            }
        }

        public PropertyValueConstraint Constraint
        {
            get
            {
                PropertyValueConstraintType ctype = (PropertyValueConstraintType)cmbConstraintType.SelectedItem;
                switch (ctype)
                {
                    case PropertyValueConstraintType.PropertyValueConstraintType_List:
                        {
                            ListConstraintSettingsCtl ctl = grpSettings.Controls[0] as ListConstraintSettingsCtl;
                            PropertyValueConstraintList list = new PropertyValueConstraintList();
                            DataValueCollection values = ctl.ListValues;
                            foreach (DataValue dv in values)
                            {
                                list.ConstraintList.Add(dv);
                            }
                            return list;
                        }
                    case PropertyValueConstraintType.PropertyValueConstraintType_Range:
                        {
                            RangeConstraintSettingsCtl ctl = grpSettings.Controls[0] as RangeConstraintSettingsCtl;
                            PropertyValueConstraintRange range = new PropertyValueConstraintRange();
                            range.MaxValue = ctl.MaxValue;
                            range.MinValue = ctl.MinValue;
                            range.MinInclusive = ctl.MinInclusive;
                            range.MaxInclusive = ctl.MaxInclusive;
                            return range;
                        }
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    cmbConstraintType.SelectedItem = value.ConstraintType;
                    cmbConstraintType_SelectedIndexChanged(null,null);
                    switch (value.ConstraintType)
                    {
                        case PropertyValueConstraintType.PropertyValueConstraintType_List:
                            {
                                PropertyValueConstraintList list = (value as PropertyValueConstraintList);
                                ListConstraintSettingsCtl ctl = grpSettings.Controls[0] as ListConstraintSettingsCtl;
                                ctl.ListValues = list.ConstraintList;
                            }
                            break;
                        case PropertyValueConstraintType.PropertyValueConstraintType_Range:
                            {
                                PropertyValueConstraintRange range = (value as PropertyValueConstraintRange);
                                RangeConstraintSettingsCtl ctl = grpSettings.Controls[0] as RangeConstraintSettingsCtl;
                                ctl.MaxInclusive = range.MaxInclusive;
                                ctl.MinInclusive = range.MinInclusive;
                                ctl.MaxValue = range.MaxValue;
                                ctl.MinValue = range.MinValue;
                            }
                            break;
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}