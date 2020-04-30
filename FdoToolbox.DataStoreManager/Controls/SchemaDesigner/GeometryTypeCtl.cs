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
using System.Windows.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal partial class GeometryTypeCtl : CheckedListBox
    {
        public GeometryTypeCtl()
        {
            InitializeComponent();
            //GeometricType[] gtypes = (GeometricType[])Enum.GetValues(typeof(GeometricType));
            //LoadGeometricTypes(gtypes);
            this.CheckOnClick = true;
        }

        private void LoadGeometricTypes(GeometricType[] gtypes)
        {
            this.Items.Clear();
            foreach (GeometricType gt in gtypes)
            {
                if(gt != GeometricType.GeometricType_All)
                    this.Items.Add(gt, false);
            }
        }

        public int GeometryTypes
        {
            get
            {
                GeometricType gtype = default(GeometricType);
                foreach (int idx in this.CheckedIndices)
                {
                    gtype |= (GeometricType)Enum.Parse(typeof(GeometricType), this.Items[idx].ToString());
                }
                return (int)gtype;
            }
            set
            {
                this.SelectedItems.Clear();
                if (value == (int)GeometricType.GeometricType_All)
                {
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        this.SetItemChecked(i, true);
                    }
                }
                else
                {
                    GeometricType[] gtypes = (GeometricType[])Enum.GetValues(typeof(GeometricType));
                    foreach (GeometricType gt in gtypes)
                    {
                        CheckIfSet(value, gt);
                    }
                }
            }
        }

        private void CheckIfSet(int value, GeometricType gtype)
        {
            if ((value & (int)gtype) == (int)gtype)
                this.SetItemChecked(this.Items.IndexOf(gtype), true);
        }

        /*
         * FDO trunk actually fixes this, but we'll only use it when it becomes FDO 3.5 proper
         * 
        public GeometryTypeCtl()
        {
            InitializeComponent();
            GeometryType[] gtypes = (GeometryType[])Enum.GetValues(typeof(GeometryType));
            LoadGeometryTypes(gtypes);
        }

        public GeometryTypeCtl(FdoConnection conn)
        {
            InitializeComponent();
            GeometryType[] gtypes = (GeometryType[])conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_GeometryTypes);
            LoadGeometryTypes(gtypes);
        }

        private void LoadGeometryTypes(GeometryType[] gtypes)
        {
            foreach (GeometryType gt in gtypes)
            {
                this.Items.Add(gt, false);
            }
        }

        public int GeometryTypes
        {
            get
            {
                GeometryType gtype = GeometryType.GeometryType_None;
                foreach (int idx in this.CheckedIndices)
                {
                    gtype |= (GeometryType)Enum.Parse(typeof(GeometryType), this.Items[idx].ToString());
                }
                return (int)gtype;
            }
            set
            {
                if (value != (int)GeometryType.GeometryType_None)
                {
                    if ((value & (int)GeometryType.GeometryType_CurvePolygon) == (int)GeometryType.GeometryType_CurvePolygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_CurvePolygon), true);
                    if ((value & (int)GeometryType.GeometryType_CurveString) == (int)GeometryType.GeometryType_CurveString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_CurveString), true);
                    if ((value & (int)GeometryType.GeometryType_LineString) == (int)GeometryType.GeometryType_LineString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_LineString), true);
                    if ((value & (int)GeometryType.GeometryType_MultiCurvePolygon) == (int)GeometryType.GeometryType_MultiCurvePolygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiCurvePolygon), true);
                    if ((value & (int)GeometryType.GeometryType_MultiCurveString) == (int)GeometryType.GeometryType_MultiCurveString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiCurveString), true);
                    //if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiGeometry) == (int)GeometryType.GeometryType_MultiGeometry)
                    //    this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiGeometry), true);
                    if ((value & (int)GeometryType.GeometryType_MultiLineString) == (int)GeometryType.GeometryType_MultiLineString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiLineString), true);
                    if ((value & (int)GeometryType.GeometryType_MultiPoint) == (int)GeometryType.GeometryType_MultiPoint)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiPoint), true);
                    if ((value & (int)GeometryType.GeometryType_MultiPolygon) == (int)GeometryType.GeometryType_MultiPolygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiPolygon), true);
                    if ((value & (int)GeometryType.GeometryType_Point) == (int)GeometryType.GeometryType_Point)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_Point), true);
                    if ((value & (int)GeometryType.GeometryType_Polygon) == (int)GeometryType.GeometryType_Polygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_Polygon), true);
                }
            }
        }
        */

        internal int GetPostCheckValue(ItemCheckEventArgs e)
        {
            //HACK: Someone at Microsoft is obviously on the wrong side of
            //the ballmer peak again, because the CheckedListBox does not
            //offer an ItemChecked event. So MS, please explain how on earth
            //do we listen for a change of checked values *after* an item is
            //checked/unchecked???????

            GeometricType gtype = default(GeometricType);
            if (this.CheckedIndices.Count > 0)
            {
                if (e.NewValue == CheckState.Unchecked)
                {
                    foreach (int idx in this.CheckedIndices)
                    {
                        if (idx == e.Index)
                        {
                            continue;
                        }
                        gtype |= (GeometricType)Enum.Parse(typeof(GeometricType), this.Items[idx].ToString());
                    }
                }
                else
                {
                    foreach (int idx in this.CheckedIndices)
                    {
                        gtype |= (GeometricType)Enum.Parse(typeof(GeometricType), this.Items[idx].ToString());
                    }
                    gtype |= (GeometricType)this.Items[e.Index];
                }
            }
            else
            {
                if (e.NewValue == CheckState.Checked)
                    gtype |= (GeometricType)this.Items[e.Index];
            }
            return (int)gtype;
        }
    }
}
