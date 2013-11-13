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
using System.Text;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.DataStore;
using System.Collections.Specialized;
using FdoToolbox.Base.Forms;
using ICSharpCode.Core;
using System.Collections.ObjectModel;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoDataStoreMgrView
    {
        bool AddEnabled { set; }
        bool DestroyEnabled { set; }
        IList<DataStoreInfo> DataStores { set; }
        string Message { set; }
    }

    internal class FdoDataStoreMgrPresenter
    {
        private readonly IFdoDataStoreMgrView _view;
        private FdoConnection _conn;

        public FdoConnection Connection
        {
            get { return _conn; }
        }

        public FdoDataStoreMgrPresenter(IFdoDataStoreMgrView view, FdoConnection conn)
        {
            _view = view;
            _conn = conn;
            _view.Message = ResourceService.GetString("MSG_LISTING_DATA_STORES");
        }

        public void Init()
        {
            GetDataStores();
        }

        private void GetDataStores()
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                ReadOnlyCollection<DataStoreInfo> dstores = service.ListDataStores(true);
                _view.DataStores = dstores;
                _view.DestroyEnabled = (dstores.Count > 0) && canDestroy;
            }
        }

        private bool canDestroy;
        private bool canAdd;

        private void ToggleUI()
        {
            Array cmds = _conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
            _view.AddEnabled = canAdd = Array.IndexOf(cmds, CommandType.CommandType_CreateDataStore) >= 0;
            _view.DestroyEnabled = canDestroy = Array.IndexOf(cmds, CommandType.CommandType_DestroyDataStore) >= 0;
        }

        public void DestroyDataStore(NameValueCollection props)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                using (IDestroyDataStore destroy = service.CreateCommand<IDestroyDataStore>(CommandType.CommandType_DestroyDataStore))
                {
                    foreach (string key in props.AllKeys)
                    {
                        destroy.DataStoreProperties.SetProperty(key, props[key]);
                    }
                    destroy.Execute();
                }
            }
        }

        public void CreateDataStore(NameValueCollection props)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                using (ICreateDataStore create = service.CreateCommand<ICreateDataStore>(CommandType.CommandType_CreateDataStore))
                {
                    foreach (string key in props.AllKeys)
                    {
                        create.DataStoreProperties.SetProperty(key, props[key]);
                    }
                    create.Execute();
                }
            }
        }
    }
}
