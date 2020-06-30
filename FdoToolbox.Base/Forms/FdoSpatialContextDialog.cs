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
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.CoordinateSystems;
using OSGeo.FDO.Schema;
using ICSharpCode.Core;
using OSGeo.FDO.Commands.SpatialContext;
using System.Diagnostics;

namespace FdoToolbox.Base.Forms
{
    public partial class FdoSpatialContextDialog : Form, IFdoSpatialContextDialogView
    {
        private FdoSpatialContextDialogPresenter _presenter;

        internal FdoSpatialContextDialog()
        {
            InitializeComponent();
        }

        public FdoSpatialContextDialog(FdoConnection conn) : this()
        {
            _presenter = new FdoSpatialContextDialogPresenter(this, conn);
        }

        public FdoSpatialContextDialog(FdoConnection conn, SpatialContextInfo sci)
            : this(conn)
        {
            _presenter.SetSpatialContext(sci);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public string Description
        {
            get
            {
                return txtDescription.Text;
            }
            set
            {
                txtDescription.Text = value;
            }
        }

        public string CoordinateSystem
        {
            get
            {
                return txtCoordSys.Text;
            }
            set
            {
                txtCoordSys.Text = value;
            }
        }

        public string CoordinateSystemWkt
        {
            get
            {
                return txtWKT.Text;
            }
            set
            {
                txtWKT.Text = value;
            }
        }

        public bool NameEnabled
        {
            get
            {
                return txtName.Enabled;
            }
            set
            {
                txtName.Enabled = value;
            }
        }

        public OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType[] ExtentTypes
        {
            set { cmbExtentType.DataSource = value; }
        }

        public OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType SelectedExtentType => (OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType)cmbExtentType.SelectedItem;

        public string LowerLeftX
        {
            get
            {
                return txtLowerLeftX.Text;
            }
            set
            {
                txtLowerLeftX.Text = value;
            }
        }

        public string LowerLeftY
        {
            get
            {
                return txtLowerLeftY.Text;
            }
            set
            {
                txtLowerLeftY.Text = value;
            }
        }

        public string UpperRightX
        {
            get
            {
                return txtUpperRightX.Text;
            }
            set
            {
                txtUpperRightX.Text = value;
            }
        }

        public string UpperRightY
        {
            get
            {
                return txtUpperRightY.Text;
            }
            set
            {
                txtUpperRightY.Text = value;
            }
        }

        private void btnComputeExtents_Click(object sender, EventArgs e)
        {
            IList<ClassDefinition> classes = FdoMultiClassPicker.GetClasses(
                ResourceService.GetString("TITLE_COMPUTE_EXTENTS"),
                ResourceService.GetString("MSG_SELECT_EXTENT_CLASSES"),
                _presenter.Connection);
            if (classes != null)
            {
                _presenter.ComputeExtents(classes);
            }
        }

        private void btnLoadCs_Click(object sender, EventArgs e)
        {
            using (var dialog = new CoordinateSystemPicker(new CoordinateSystemCatalog()))
            {
                if (dialog .ShowDialog() == DialogResult.OK)
                {
                    var selected = dialog.SelectedCoordSys;
                    if (selected != null)
                    {
                        _presenter.SetCoordinateSystem(selected);
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private bool ValidateForm()
        {
            bool valid = true;
            double dVal = default(double);
            errorProvider.Clear();
            if (string.IsNullOrEmpty(this.ContextName))
            {
                errorProvider.SetError(txtName, "Required");
                valid = false;
            }
            if (!Double.TryParse(this.XYTolerance, out dVal))
            {
                errorProvider.SetError(txtXYTolerance, "Not a double");
                valid = false;
            }
            if (!Double.TryParse(this.ZTolerance, out dVal))
            {
                errorProvider.SetError(txtZTolerance, "Not a double");
                valid = false;
            }
            //Static Extent requires the extent fields to be filled and valid
            //double numbers
            if (this.SelectedExtentType == SpatialContextExtentType.SpatialContextExtentType_Static)
            {
                ValidateExtents(ref valid, ref dVal);
            }
            else //Dynamic extents means extents are optional
            {
                //If some or all fields are filled, validate them all
                if (this.IsExtentDefined)
                {
                    ValidateExtents(ref valid, ref dVal);
                }
            }
            return valid;
        }

        private void ValidateExtents(ref bool valid, ref double dVal)
        {
            if (string.IsNullOrEmpty(this.LowerLeftX))
            {
                errorProvider.SetError(txtLowerLeftX, "Required");
                valid = false;
            }
            else if (string.IsNullOrEmpty(this.LowerLeftY))
            {
                errorProvider.SetError(txtLowerLeftY, "Required");
                valid = false;
            }
            else if (string.IsNullOrEmpty(this.UpperRightX))
            {
                errorProvider.SetError(txtUpperRightX, "Required");
                valid = false;
            }
            else if (string.IsNullOrEmpty(this.UpperRightY))
            {
                errorProvider.SetError(txtUpperRightY, "Required");
                valid = false;
            }
            else if (!Double.TryParse(this.LowerLeftX, out dVal))
            {
                errorProvider.SetError(txtLowerLeftX, "Not a double");
                valid = false;
            }
            else if (!Double.TryParse(this.LowerLeftY, out dVal))
            {
                errorProvider.SetError(txtLowerLeftY, "Not a double");
                valid = false;
            }
            else if (!Double.TryParse(this.UpperRightX, out dVal))
            {
                errorProvider.SetError(txtUpperRightX, "Not a double");
                valid = false;
            }
            else if (!Double.TryParse(this.UpperRightY, out dVal))
            {
                errorProvider.SetError(txtUpperRightY, "Not a double");
                valid = false;
            }
            else if (Convert.ToDouble(this.LowerLeftX) > Convert.ToDouble(this.UpperRightX))
            {
                errorProvider.SetError(txtLowerLeftX, "Lower Left X is greater than Upper Right X");
                errorProvider.SetError(txtUpperRightX, "Lower Left X is greater than Upper Right X");
                valid = false;
            }
            else if (Convert.ToDouble(this.LowerLeftY) > Convert.ToDouble(this.UpperRightY))
            {
                errorProvider.SetError(txtLowerLeftY, "Lower Left Y is greater than Upper Right Y");
                errorProvider.SetError(txtUpperRightY, "Lower Left Y is greater than Upper Right Y");
                valid = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


        public string XYTolerance
        {
            get
            {
                return txtXYTolerance.Text;
            }
            set
            {
                txtXYTolerance.Text = value;
            }
        }

        public string ZTolerance
        {
            get
            {
                return txtZTolerance.Text;
            }
            set
            {
                txtZTolerance.Text = value;
            }
        }

        public string ContextName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public bool IsExtentDefined => !string.IsNullOrEmpty(txtLowerLeftX.Text) &&
                    !string.IsNullOrEmpty(txtLowerLeftY.Text) &&
                    !string.IsNullOrEmpty(txtUpperRightX.Text) &&
                    !string.IsNullOrEmpty(txtUpperRightY.Text);

        public static SpatialContextInfo CreateNew(FdoConnection conn)
        {
            FdoSpatialContextDialog diag = new FdoSpatialContextDialog(conn);
            if (diag.ShowDialog() == DialogResult.OK)
            {
                SpatialContextInfo sci = new SpatialContextInfo
                {
                    Name = diag.ContextName,
                    Description = diag.Description,
                    CoordinateSystem = diag.CoordinateSystem,
                    CoordinateSystemWkt = diag.CoordinateSystemWkt,
                    XYTolerance = Convert.ToDouble(diag.XYTolerance),
                    ZTolerance = Convert.ToDouble(diag.ZTolerance),
                    ExtentType = diag.SelectedExtentType
                };
                //Only consider extent if all 4 values are defined
                if (diag.IsExtentDefined)
                {
                    string wktfmt = "POLYGON (({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))";
                    double llx = Convert.ToDouble(diag.LowerLeftX);
                    double lly = Convert.ToDouble(diag.LowerLeftY);
                    double urx = Convert.ToDouble(diag.UpperRightX);
                    double ury = Convert.ToDouble(diag.UpperRightY);
                    sci.ExtentGeometryText = string.Format(wktfmt,
                        llx, lly,
                        urx, lly,
                        urx, ury,
                        llx, ury);
                }
                return sci;
            }
            return null;
        }

        public static SpatialContextInfo Edit(FdoConnection conn, SpatialContextInfo ctx)
        {
            FdoSpatialContextDialog diag = new FdoSpatialContextDialog(conn, ctx);
            if (diag.ShowDialog() == DialogResult.OK)
            {
                SpatialContextInfo sci = new SpatialContextInfo
                {
                    Name = diag.ContextName,
                    Description = diag.Description,
                    CoordinateSystem = diag.CoordinateSystem,
                    CoordinateSystemWkt = diag.CoordinateSystemWkt,
                    XYTolerance = Convert.ToDouble(diag.XYTolerance),
                    ZTolerance = Convert.ToDouble(diag.ZTolerance),
                    ExtentType = diag.SelectedExtentType
                };
                //Only consider extent if all 4 values are defined
                if (diag.IsExtentDefined)
                {
                    string wktfmt = "POLYGON (({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))";
                    double llx = Convert.ToDouble(diag.LowerLeftX);
                    double lly = Convert.ToDouble(diag.LowerLeftY);
                    double urx = Convert.ToDouble(diag.UpperRightX);
                    double ury = Convert.ToDouble(diag.UpperRightY);
                    sci.ExtentGeometryText = string.Format(wktfmt,
                        llx, lly,
                        urx, lly,
                        urx, ury,
                        llx, ury);
                }
                return sci;
            }
            return null;
        }


        public bool ComputeExtentsEnabled
        {
            set { btnComputeExtents.Enabled = value; }
        }
    }
}
