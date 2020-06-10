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
using System.Collections.Generic;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using ICSharpCode.Core;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Controls
{
    public interface ICopySpatialContextsView : IViewContent
    {
        string SourceConnectionName { get; set; }
        IList<string> TargetConnectionNames { set; }
        string SelectedTargetConnectionName { get; }
        bool MultiSelect { set; }
        IList<string> SpatialContexts { get; set; }
        bool Overwrite { get; set; }
        bool OverwriteEnabled { get; set; }
    }

    public class CopySpatialContextsCtlPresenter
    {
        private readonly ICopySpatialContextsView _view;
        private readonly IFdoConnectionManager _connMgr;

        public CopySpatialContextsCtlPresenter(ICopySpatialContextsView view, string sourceConnName, IFdoConnectionManager connMgr)
        {
            _view = view;
            _view.SourceConnectionName = sourceConnName;
            _connMgr = connMgr;
        }

        public void Init()
        {
            List<string> names = new List<string>(_connMgr.GetConnectionNames());
            names.Remove(_view.SourceConnectionName);

            _view.TargetConnectionNames = names;
            FdoConnection conn = _connMgr.GetConnection(_view.SourceConnectionName);

            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                ICollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                List<string> scn = new List<string>();
                foreach (SpatialContextInfo sc in contexts)
                {
                    scn.Add(sc.Name);
                }
                _view.SpatialContexts = scn;
            }
        }

        public void TargetConnectionChanged()
        {
            string name = _view.SelectedTargetConnectionName;
            FdoConnection conn = _connMgr.GetConnection(name);
            using (var connCaps = conn.ConnectionCapabilities)
            {
                bool supportsMultiSc = connCaps.SupportsMultipleSpatialContexts();
                if (!supportsMultiSc)
                {
                    _view.OverwriteEnabled = false;
                    _view.Overwrite = true;
                    _view.MultiSelect = false;
                }
                else
                {
                    _view.OverwriteEnabled = true;
                    _view.MultiSelect = true;
                }
            }
        }

        public void CopySpatialContexts()
        {
            List<SpatialContextInfo> contexts = new List<SpatialContextInfo>();
            FdoConnection srcConn = _connMgr.GetConnection(_view.SourceConnectionName);
            FdoConnection targetConn = _connMgr.GetConnection(_view.SelectedTargetConnectionName);

            using (FdoFeatureService service = srcConn.CreateFeatureService())
            {
                foreach (string ctxName in _view.SpatialContexts)
                {
                    SpatialContextInfo sc = service.GetSpatialContext(ctxName);
                    if (sc != null)
                    {
                        contexts.Add(sc);
                    }
                }
            }

            if (contexts.Count == 0)
            {
                _view.ShowMessage(null, ResourceService.GetString("MSG_NO_SPATIAL_CONTEXTS_COPIED"));
                return;
            }

            ExpressUtility.CopyAllSpatialContexts(contexts, targetConn, _view.Overwrite);
            _view.ShowMessage(null, ResourceService.GetString("MSG_SPATIAL_CONTEXTS_COPIED"));
            _view.Close();
        }
    }
}
