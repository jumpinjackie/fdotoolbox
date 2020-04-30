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
using System.Windows.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    public partial class ObjectPropertyCtrl : UserControl
    {
        public ObjectPropertyCtrl()
        {
            InitializeComponent();
        }

        private SchemaDesignContext _context;

        public ObjectPropertyCtrl(ObjectPropertyDefinitionDecorator p, SchemaDesignContext context, NodeUpdateHandler updater)
            : this()
        {
            _context = context;

            txtName.DataBindings.Add("Text", p, "Name");
            txtDescription.DataBindings.Add("Text", p, "Description");

            cmbObjectType.DataSource = Enum.GetValues(typeof(ObjectType));
            cmbOrderType.DataSource = Enum.GetValues(typeof(OrderType));

            cmbObjectType.DataBindings.Add("SelectedItem", p, "ObjectType");
            cmbOrderType.DataBindings.Add("SelectedItem", p, "OrderType");

            string schema = p.DecoratedObject.Parent.Parent.Name;

            cmbClass.DisplayMember = "Name";
            cmbClass.DataSource = _context.GetClasses(schema);

            EventHandler clsIndexChanged = (sender, e) =>
            {
                var cls = cmbClass.SelectedItem as ClassDefinition;
                if (cls != null)
                {
                    p.Class = cls;
                    cmbIdentityProperty.DisplayMember = "Name";

                    List<DataPropertyDefinition> dataProps = new List<DataPropertyDefinition>();
                    foreach (PropertyDefinition prop in cls.Properties)
                    {
                        if (prop.PropertyType == PropertyType.PropertyType_DataProperty)
                            dataProps.Add((DataPropertyDefinition)prop);
                    }

                    cmbIdentityProperty.DataSource = dataProps;
                    if (p.IdentityProperty != null && cls.Properties.Contains(p.IdentityProperty))
                    {
                        cmbIdentityProperty.SelectedItem = p.IdentityProperty;
                    }
                    else
                    {
                        if (cmbIdentityProperty.Items.Count == 0)
                            cmbIdentityProperty.SelectedIndex = 0;
                    }
                }
            };
            EventHandler prpIndexChanged = (sender, e) =>
            {
                var prop = cmbIdentityProperty.SelectedItem as DataPropertyDefinition;
                if (prop != null)
                {
                    p.IdentityProperty = prop;
                }
                else
                {
                    p.IdentityProperty = null;
                }
            };

            cmbClass.SelectedItem = p.Class;
            cmbClass.SelectedIndexChanged += clsIndexChanged;
            
            cmbIdentityProperty.SelectedIndexChanged += prpIndexChanged;

            clsIndexChanged(this, EventArgs.Empty);

            p.PropertyChanged += (s, evt) =>
            {
                if (evt.PropertyName == "Name")
                    updater();
            };
        }

        private void lnkClearClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cmbIdentityProperty.SelectedItem = null;
        }
    }
}
