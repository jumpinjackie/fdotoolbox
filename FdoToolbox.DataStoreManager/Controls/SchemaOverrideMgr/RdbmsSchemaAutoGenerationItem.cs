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
using Rdbms = OSGeo.FDO.Providers.Rdbms.Override;
using OSGeo.FDO.Common;

namespace FdoToolbox.DataStoreManager.Controls.SchemaOverrideMgr
{
    public class RdbmsSchemaAutoGenerationItem : NotifyPropertyChanged
    {
        private Rdbms.OvSchemaAutoGeneration _autoGen;
        private string[] _genTableList;

        internal RdbmsSchemaAutoGenerationItem(Rdbms.OvSchemaAutoGeneration autoGen)
        {
            _autoGen = autoGen;
            List<string> tables = new List<string>();
            foreach (StringElement el in _autoGen.GenTableList)
            {
                tables.Add(el.String);
            }
            _genTableList = tables.ToArray();
        }

        internal Rdbms.OvSchemaAutoGeneration InternalValue { get { return _autoGen; } }

        public string[] GenTableList
        {
            get
            {
                return _genTableList;
            }
        }

        public string GenTablePrefix
        {
            get { return _autoGen.GenTablePrefix; }
            set { _autoGen.GenTablePrefix = value; }
        }

        public int MaxSampleRows 
        {
            get { return _autoGen.MaxSampleRows; }
            set { _autoGen.MaxSampleRows = value; }
        }

        public bool RemoveTablePrefix
        {
            get { return _autoGen.RemoveTablePrefix; }
            set { _autoGen.RemoveTablePrefix = value; }
        }
    }
}
