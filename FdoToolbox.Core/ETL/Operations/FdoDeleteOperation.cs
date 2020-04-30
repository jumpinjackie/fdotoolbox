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
using OSGeo.FDO.Commands.Feature;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// A delete ETL operation
    /// </summary>
    public class FdoDeleteOperation : FdoSingleActionOperationBase
    {
        private FdoConnection _conn;
        private string _className;
        private string _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoDeleteOperation"/> class.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="className">Name of the class.</param>
        public FdoDeleteOperation(FdoConnection conn, string className)
            : this(conn, className, string.Empty) 
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoDeleteOperation"/> class.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="deleteFilter">The delete filter.</param>
        public FdoDeleteOperation(FdoConnection conn, string className, string deleteFilter)
        {
            _conn = conn;
            _className = className;
            _filter = deleteFilter;
        }

        public override void ExecuteAction()
        {
            using (FdoFeatureService svc = _conn.CreateFeatureService())
            {
                using (IDelete del = svc.CreateCommand<IDelete>(OSGeo.FDO.Commands.CommandType.CommandType_Delete))
                {
                    try
                    {
                        del.SetFeatureClassName(_className);
                        if (!string.IsNullOrEmpty(_filter))
                        {
                            del.SetFilter(_filter);
                            Info("Deleting everything from class " + _className + " with filter: " + _filter);
                        }
                        else
                        {
                            Info("Deleting everything from class: " + _className);
                        }
                        int result = del.Execute();
                        Info(result + " features deleted from class: " + _className);
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        Error(ex, "Error occured executing delete");
                    }
                }
            }
        }
    }
}
