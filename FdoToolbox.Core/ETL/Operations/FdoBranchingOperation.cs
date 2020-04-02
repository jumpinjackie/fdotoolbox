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

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class FdoBranchingOperation : FdoOperationBase
    {
        private readonly List<IFdoOperation> operations = new List<IFdoOperation>();

        /// <summary>
        /// Adds the specified operation to this branching operation
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public FdoBranchingOperation Add(IFdoOperation op)
        {
            operations.Add(op);
            return this;
        }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter"></param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            base.PrepareForExecution(pipelineExecuter);
            foreach (IFdoOperation op in operations)
            {
                op.PrepareForExecution(pipelineExecuter);
            }
        }

        /// <summary>
        /// Executes this operation, sending the input of this operation
        /// to all its child operations
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            List<FdoRow> copiedRows = new List<FdoRow>(rows);
            foreach (IFdoOperation operation in operations)
            {
                List<FdoRow> cloned = copiedRows.ConvertAll<FdoRow>(delegate(FdoRow row)
                {
                    return row.Clone();
                });
                IEnumerable<FdoRow> enumerable = operation.Execute(cloned);
                if (enumerable == null)
                    continue;
                IEnumerator<FdoRow> enumerator = enumerable.GetEnumerator();
#pragma warning disable 642
                while (enumerator.MoveNext()) ;
#pragma warning restore 642
            }
            yield break;
        }
    }
}
