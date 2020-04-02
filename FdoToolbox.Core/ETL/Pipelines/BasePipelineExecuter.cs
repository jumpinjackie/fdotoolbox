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

namespace FdoToolbox.Core.ETL.Pipelines
{
    using Operations;

    /// <summary>
    /// Base class for pipeline executers, handles all the details and leave the actual
    /// pipeline execution to the 
    /// </summary>
    public abstract class BasePipelineExecuter : WithLoggingMixin, IPipelineExecuter
    {
        #region IPipelineExecuter Members

        /// <summary>
        /// Executes the specified pipeline.
        /// </summary>
        /// <param name="pipelineName">The name.</param>
        /// <param name="pipeline">The pipeline.</param>
        public void Execute(string pipelineName, ICollection<IFdoOperation> pipeline)
        {
            try
            {
                IEnumerable<FdoRow> enumerablePipeline = PipelineToEnumerable(pipeline, new List<FdoRow>());
                try
                {
                    DateTime start = DateTime.Now;
                    ExecutePipeline(enumerablePipeline);
                    Notice("Completed process {0} in {1}", pipelineName, DateTime.Now - start);
                }
                catch (Exception e)
                {
                    Error(e, "Failed to execute pipeline {0}", pipelineName);
                }
            }
            catch (Exception e)
            {
                Error(e, "Failed to create pipeline {0}", pipelineName);
            }

            DisposeAllOperations(pipeline);
        }

        /// <summary>
        /// Transform the pipeline to an enumerable
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="rows">The rows</param>
        /// <returns></returns>
        public virtual IEnumerable<FdoRow> PipelineToEnumerable(ICollection<IFdoOperation> pipeline, IEnumerable<FdoRow> rows)
        {
            foreach (IFdoOperation operation in pipeline)
            {
                operation.PrepareForExecution(this);
                IEnumerable<FdoRow> enumerator = operation.Execute(rows);
                rows = DecorateEnumerable(operation, enumerator);
            }
            return rows;
        }

        /// <summary>
        /// Gets all errors that occured under this executer
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> GetAllErrors()
        {
            return Errors;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return Errors.Length != 0; }
        }

        #endregion

        /// <summary>
        /// Iterates the specified enumerable.
        /// Since we use a pipeline, we need to force it to execute at some point. 
        /// We aren't really interested in the result, just in that the pipeline would execute.
        /// </summary>
        protected virtual void ExecutePipeline(IEnumerable<FdoRow> pipeline)
        {
            IEnumerator<FdoRow> enumerator = pipeline.GetEnumerator();
            bool hasMore = true;
            do
            {
                try
                {
                    hasMore = enumerator.MoveNext();
                }
                catch (Exception e)
                {
                    Error(e, "Failed to execute operation {0}", enumerator.Current);
                }
            }
            while (hasMore);
        }


        /// <summary>
        /// Destroys the pipeline.
        /// </summary>
        protected void DisposeAllOperations(ICollection<IFdoOperation> operations)
        {
            foreach (IFdoOperation operation in operations)
            {
                try
                {
                    operation.Dispose();
                }
                catch (Exception e)
                {
                    Error(e, "Failed to disposed {0}", operation.Name);
                }
            }
        }

        /// <summary>
        /// Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected abstract IEnumerable<FdoRow> DecorateEnumerable(IFdoOperation operation, IEnumerable<FdoRow> enumerator);
    }
}
