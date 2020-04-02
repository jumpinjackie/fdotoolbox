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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    public partial class GeometricPropertyCtrl : UserControl
    {
        public GeometricPropertyCtrl()
        {
            InitializeComponent();
        }

        private SchemaDesignContext _context;

        public GeometricPropertyCtrl(GeometricPropertyDefinitionDecorator p, SchemaDesignContext context, NodeUpdateHandler updater)
            : this()
        {
            _context = context;

            txtName.DataBindings.Add("Text", p, "Name");
            txtDescription.DataBindings.Add("Text", p, "Description");
            chkElevation.DataBindings.Add("Checked", p, "HasElevation");
            chkMeasure.DataBindings.Add("Checked", p, "HasMeasure");

            cmbSpatialContext.DisplayMember = "Name";
            var scNames = _context.GetSpatialContextNames();
            cmbSpatialContext.DataSource = scNames;
            //Assign current association if defined
            if (!string.IsNullOrEmpty(p.SpatialContextAssociation))
            {
                cmbSpatialContext.SelectedItem = p.SpatialContextAssociation;
            }
            //Setup event handler that will update the model
            EventHandler scChanged = (s, e) =>
            {
                if (cmbSpatialContext.SelectedItem != null)
                    p.SpatialContextAssociation = cmbSpatialContext.SelectedItem.ToString();
            };
            //Wire it up
            cmbSpatialContext.SelectedIndexChanged += scChanged;
            //If spatial contexts available and this property hasn't been assigned one, assign
            //it to the first available spatial context name
            if (string.IsNullOrEmpty(p.SpatialContextAssociation) && scNames.Length > 0)
            {
                cmbSpatialContext.SelectedIndex = 0;
                scChanged(this, EventArgs.Empty);
            }

            chkGeometryTypes.GeometryTypes = p.GeometryTypes;

            //Now wire up change listener
            chkGeometryTypes.ItemCheck += (s, e) =>
            {
                p.GeometryTypes = chkGeometryTypes.GetPostCheckValue(e);
            };

            p.PropertyChanged += (s, evt) =>
            {
                if (evt.PropertyName == "Name")
                    updater();
            };
        }
    }
}
