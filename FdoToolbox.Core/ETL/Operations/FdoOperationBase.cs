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
    /// Represent a single operation that can occur during the ETL process
    /// </summary>
    public abstract class FdoOperationBase : WithLoggingMixin, IFdoOperation
    {
        private IPipelineExecuter pipelineExecuter;

        /// <summary>
        /// Gets the pipeline executer
        /// </summary>
        protected IPipelineExecuter PipelineExecuter => pipelineExecuter;

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// Gets the statistics for this operation
        /// </summary>
        /// <value>The statistics.</value>
        public OperationStatistics Statistics { get; } = new OperationStatistics();

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter"></param>
        public virtual void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            this.pipelineExecuter = pipelineExecuter;
            Statistics.MarkStarted();
        }

        /// <summary>
        /// Raised when a feature has been processed
        /// </summary>
        public event FeatureProcessedEventHandler OnFeatureProcessed = delegate { };

        /// <summary>
        /// Raised when processing has completed
        /// </summary>
        public event FdoOperationEventHandler OnFinishedProcessing = delegate { };

        /// <summary>
        /// Raised when a feature has failed to be processed.
        /// </summary>
        public event FeatureFailedEventHandler OnFeatureFailed = delegate { };

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public abstract IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows);

        /// <summary>
        /// Raises the OnFeatureProcessed event
        /// </summary>
        /// <param name="row"></param>
        public void RaiseFeatureProcessed(FdoRow row)
        {
            Statistics.MarkFeatureProcessed();
            OnFeatureProcessed(this, row);
        }

        /// <summary>
        /// Raises the OnFinishedProcessing event
        /// </summary>
        public void RaiseFinishedProcessing()
        {
            Statistics.MarkFinished();
            OnFinishedProcessing(this);
        }

        /// <summary>
        /// Raises the OnFeatureFailed event
        /// </summary>
        /// <param name="row"></param>
        /// <param name="ex"></param>
        public void RaiseFailedFeatureProcessed(FdoRow row, Exception ex)
        {
            Statistics.MarkFeatureFailed();
            this.Error(ex, "Error encountered processing feature: {0}", row.ToString());
            OnFeatureFailed(this, row, ex);
        }

        /// <summary>
        /// Raises the OnFeatureFailed event
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        public void RaiseFailedReadFeature(string data, Exception ex)
        {
            Statistics.MarkFeatureFailed();
            this.Error(ex, "Error encountered reading feature: {0}", data);
            OnFeatureFailed(this, null, ex);
        }

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Exception> GetAllErrors()
        {
            return this.Errors;
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            
        }

        /// <summary>
        /// Raised when a feature batch has been processed
        /// </summary>
        public event FeatureBatchProcessedEventHandler OnBatchProcessed = delegate { };

        /// <summary>
        /// Raises the OnBatchProcessed event
        /// </summary>
        /// <param name="count"></param>
        public void RaiseBatchProcessed(int count)
        {
            Statistics.MarkFeatureProcessed(count);
            OnBatchProcessed(this, count);
        }
    }
}
