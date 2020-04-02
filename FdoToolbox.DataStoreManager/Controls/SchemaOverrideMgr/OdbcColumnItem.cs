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
using Odbc = OSGeo.FDO.Providers.Rdbms.Override.ODBC;
using System.ComponentModel;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    [TypeConverter(typeof(OdbcColumnTypeConverter))]
    public class OdbcColumnItem : RdbmsColumnItem<Odbc.OvColumn>
    {
        public OdbcColumnItem(Odbc.OvColumn value)
            : base(value)
        { }

        public OdbcColumnItem(string name)
            : base(new Odbc.OvColumn(name))
        { }
    }

    public class OdbcColumnTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((OdbcColumnItem)value).Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(OdbcColumnItem), attributes).Sort(new string[] { "Name" });
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
