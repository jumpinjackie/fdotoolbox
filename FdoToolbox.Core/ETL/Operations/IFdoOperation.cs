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

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Pipeline operation interface
    /// </summary>
    public interface IFdoOperation : IDisposable
    {
        /// <summary>
        /// Name of the operation
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Fires when a feature is processed
        /// </summary>
        event FeatureProcessedEventHandler OnFeatureProcessed;

        /// <summary>
        /// Fires when all the features have been processed
        /// </summary>
        event FdoOperationEventHandler OnFinishedProcessing;

        /// <summary>
        /// Initializes the current instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        void PrepareForExecution(IPipelineExecuter pipelineExecuter);

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows);

        /// <summary>
        /// Raise the feature processed event
        /// </summary>
        /// <param name="row"></param>
        void RaiseFeatureProcessed(FdoRow row);

        /// <summary>
        /// Raises the finished processing event
        /// </summary>
        void RaiseFinishedProcessing();

        /// <summary>
        /// Gets all errors that occured when running this operation.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exception> GetAllErrors();
    }

    /// <summary>
    /// Event handler for tracking operation status
    /// </summary>
    public delegate void FdoOperationEventHandler(FdoOperationBase op);

    /// <summary>
    /// Event handler for processed features
    /// </summary>
    public delegate void FeatureProcessedEventHandler(FdoOperationBase op, FdoRow row);

    /// <summary>
    /// Event handler for failed features
    /// </summary>
    /// <param name="op"></param>
    /// <param name="row"></param>
    /// <param name="ex"></param>
    public delegate void FeatureFailedEventHandler(FdoOperationBase op, FdoRow row, Exception ex);

    /// <summary>
    /// Event handler for batch processed features
    /// </summary>
    /// <param name="op"></param>
    /// <param name="count"></param>
    public delegate void FeatureBatchProcessedEventHandler(FdoOperationBase op, int count);
}
