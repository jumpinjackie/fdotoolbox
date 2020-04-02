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
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Connections;

namespace FdoToolbox.Express.Controls.Ogr
{
    public class OgrEsriPgbEditor : UITypeEditor
    {
        private ListBox box = new ListBox();

        private IWindowsFormsEditorService editorService;

        public OgrEsriPgbEditor()
        {
            box.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);

            //Since ESRI PGB is basically a ODBC DSN, we get a free builder by just using
            //the DataSourceName property from the ODBC FDO Provider
            IList<DictionaryProperty> props = FdoFeatureService.GetConnectProperties("OSGeo.ODBC");
            EnumerableDictionaryProperty dataSource = null;

            foreach (DictionaryProperty dp in props)
            {
                if (dp.Enumerable && dp.Name == "DataSourceName")
                {
                    dataSource = (EnumerableDictionaryProperty)dp;
                }
            }

            if (dataSource != null)
            {
                box.SelectionMode = SelectionMode.One;
                box.DataSource = dataSource.Values;
            }
        }

        void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            editorService.CloseDropDown();
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                if (editorService == null)
                    editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                editorService.DropDownControl(box);

                if (box.SelectedItem != null)
                {
                    return box.SelectedItem;
                }
            }
            return base.EditValue(context, provider, value);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    public class OgrEsriPgb : BaseOgrConnectionBuilder
    {
        [Description("The path to the ESRI Personal Geodatabase")]
        [DisplayName("ESRI PGB Path")]
        [Editor(typeof(OgrEsriPgbEditor), typeof(UITypeEditor))]
        public override string DataSource
        {
            get { return "PGeo:" + base.DataSource; }
            set { base.DataSource = value; }
        }
    }
}
