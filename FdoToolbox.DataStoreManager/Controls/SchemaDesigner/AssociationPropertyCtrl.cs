#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.Windows.Forms;
using System.Diagnostics;
using OSGeo.FDO.Schema;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    // NOTE: The main difference between the new Association Property Editor and the old one
    // is that we use a key-value mapping to ensure that identity properties and reverse
    // identity properties are in sync. The UI is designed to ensure valid property mappings
    // (ie. They are of the same data type, and can't map a property that is already in
    // another mapping)

    public partial class AssociationPropertyCtrl : UserControl
    {
        public AssociationPropertyCtrl()
        {
            InitializeComponent();
        }

        private SchemaDesignContext _context;

        private BindingList<KeyMapping> _mappings;

        private AssociationPropertyDefinitionDecorator _ap;

        private bool bubbleAssocClassChanged = false;

        public AssociationPropertyCtrl(AssociationPropertyDefinitionDecorator p, SchemaDesignContext context, NodeUpdateHandler updater)
            : this()
        {
            _context = context;
            _mappings = new BindingList<KeyMapping>();
            _ap = p;

            txtName.DataBindings.Add("Text", p, "Name");
            txtDescription.DataBindings.Add("Text", p, "Description");

            string schema = p.DecoratedObject.Parent.Parent.Name;

            cmbDeleteRule.DataSource = Enum.GetValues(typeof(DeleteRule));
            cmbDeleteRule.DataBindings.Add("SelectedItem", p, "DeleteRule");

            var mappings = _ap.GetMappings();

            cmbAssociatedClass.DisplayMember = "Name";
            cmbAssociatedClass.DataSource = _context.GetClasses(schema);
            cmbAssociatedClass.DataBindings.Add("SelectedItem", p, "AssociatedClass");

            txtMultiplicity.DataBindings.Add("Text", p, "Multiplicity");
            chkReadOnly.DataBindings.Add("Checked", p, "IsReadOnly");
            chkLockCascade.DataBindings.Add("Checked", p, "LockCascade");

            txtReverseName.DataBindings.Add("Text", p, "ReverseName");
            txtRevMultiplicity.DataBindings.Add("Text", p, "ReverseMultiplicity");

           
            foreach (var map in mappings)
            {
                _mappings.Add(map);
            }

            grdMappings.DataSource = _mappings;

            _mappings.ListChanged += new ListChangedEventHandler(OnMappingsChanged);

            p.PropertyChanged += (s, evt) =>
            {
                if (evt.PropertyName == "Name")
                    updater();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            bubbleAssocClassChanged = true;
        }

        void OnMappingsChanged(object sender, ListChangedEventArgs e)
        {
            if (!bubble)
                return;

            var ids = _ap.IdentityProperties;
            var rids = _ap.ReverseIdentityProperties;
            var cls = _ap.AssociatedClass;

            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                var map = _mappings[e.NewIndex];
                _ap.AddKeyMapping(map);
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                var map = _mappings[e.NewIndex];

                var pidx = ids.IndexOf(map.Primary);
                var ridx = rids.IndexOf(map.Foreign);

                Debug.Assert(pidx == ridx);

                _ap.RemoveKeyMapping(pidx);
            }
        }

        private bool bubble = true;

        private void cmbAssociatedClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!bubbleAssocClassChanged)
                return;

            bubble = false;
            _ap.ResetMappings();
            _mappings.Clear();
            bubble = true;
        }

        private void lnkAdd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var acls = _ap.AssociatedClass;
            var rcls = (ClassDefinition)_ap.DecoratedObject.Parent;

            if (acls == null)
            {
                MessageBox.Show("Please set the associated class first");
                return;
            }

            var aprops = new List<DataPropertyDefinition>();
            foreach (PropertyDefinition p in acls.Properties)
            {
                if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                    aprops.Add((DataPropertyDefinition)p);
            }
            var rprops = new List<DataPropertyDefinition>();
            foreach (PropertyDefinition p in rcls.Properties)
            {
                if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                    rprops.Add((DataPropertyDefinition)p);
            }

            if (aprops.Count == 0)
            {
                MessageBox.Show("Associated class has no data properties to map");
                return;
            }

            if (rprops.Count == 0)
            {
                MessageBox.Show("This class has no data properties to map");
                return;
            }

            var dlg = new AddMappingDialog(_mappings, acls, rcls);
            dlg.ShowDialog();
        }

        private void lnkDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (grdMappings.SelectedRows.Count == 1)
            {
                var map = (KeyMapping)grdMappings.SelectedRows[0].DataBoundItem;
                _mappings.Remove(map);
            }
        }

        private void grdMappings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            grdMappings.ClearSelection();
            if (e.RowIndex >= 0)
            {
                grdMappings.Rows[e.RowIndex].Selected = true;
            }

            lnkDelete.Enabled = grdMappings.SelectedRows.Count == 1;
        }
    }

    public class KeyMapping
    {
        public string Primary { get; set; }
        public string Foreign { get; set; }

        public KeyMapping(string primary, string foreign)
        {
            this.Primary = primary;
            this.Foreign = foreign;
        }
    }
}
