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
using FdoToolbox.Core.ETL.Operations;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Interface that abstracts the actual execution of the pipeline
    /// </summary>
    public interface IPipelineExecuter
    {
        /// <summary>
        /// Executes the specified pipeline.
        /// </summary>
        /// <param name="pipelineName">The name.</param>
        /// <param name="pipeline">The pipeline.</param>
        void Execute(string pipelineName, ICollection<IFdoOperation> pipeline);

        /// <summary>
        /// Transform the pipeline to an enumerable
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        IEnumerable<FdoRow> PipelineToEnumerable(ICollection<IFdoOperation> pipeline, IEnumerable<FdoRow> rows);

        /// <summary>
        /// Gets all errors that occured under this executer
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exception> GetAllErrors();

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }
    }
}
