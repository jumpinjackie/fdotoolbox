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

using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoBulkDeleteView : IViewContent
    {
        string ClassName { get; set; }
        string Filter { get; set; }
        bool UseTransaction { get; set; }
        bool TransactionEnabled { get; set; }
    }

    internal class FdoBulkDeleteCtlPresenter
    {
        private readonly IFdoBulkDeleteView _view;
        private readonly FdoConnection _conn;
        private readonly string _className;

        public FdoBulkDeleteCtlPresenter(IFdoBulkDeleteView view, FdoConnection conn, string className)
        {
            _view = view;
            _conn = conn;
            _className = className;
            _view.ClassName = className;
            _view.Title = ICSharpCode.Core.ResourceService.GetString("TITLE_BULK_DELETE");
            using (var connCaps = _conn.ConnectionCapabilities)
            {
                _view.TransactionEnabled = connCaps.SupportsTransactions();
            }
        }

        internal void Cancel()
        {
            _view.Close();
        }

        internal void Delete()
        {
            if (_view.Confirm("Bulk Delete", "Bulk deletes can be very lengthy. Are you sure you want to do this?"))
            {
                using (FdoFeatureService service = _conn.CreateFeatureService())
                {
                    int deleted = service.DeleteFeatures(_className, _view.Filter.Trim(), _view.UseTransaction);
                    _view.ShowMessage(null, deleted + " feature(s) deleted");
                }

                using (var connCaps = _conn.ConnectionCapabilities)
                {
                    if (connCaps.SupportsFlush())
                    {
                        _conn.Flush();
                    }
                }

                _view.Close();
            }
        }

        internal void Test()
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                long count = service.GetFeatureCount(_className, _view.Filter.Trim(), true);
                _view.ShowMessage(null, count + " feature(s) would be deleted");
            }
        }
    }
}
