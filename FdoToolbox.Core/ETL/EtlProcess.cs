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
    using Pipelines;

    /// <summary>
    /// An ETL process
    /// </summary>
    public abstract class EtlProcess : EtlProcessBase<EtlProcess>, IDisposable
    {
        /// <summary>
        /// Gets the pipeline executer.
        /// </summary>
        /// <value>The pipeline executer.</value>
        public IPipelineExecuter PipelineExecuter { get; set; } = new SingleThreadedPipelineExecuter();


        /// <summary>
        /// Gets a new partial process that we can work with
        /// </summary>
        protected static PartialProcessOperation Partial
        {
            get
            {
                PartialProcessOperation operation = new PartialProcessOperation();
                return operation;
            }
        }

        #region IDisposable Members

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IFdoOperation operation in operations)
                {
                    operation.Dispose();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Executes this process
        /// </summary>
        public virtual void Execute()
        {
            Initialize();
            MergeLastOperationsToOperations();
            RegisterToOperationsEvents();
            Notice("Starting to execute {0}", Name);
            PipelineExecuter.Execute(Name, operations);

            PostProcessing();
            OnProcessCompleted();
        }

        /// <summary>
        /// Called when the process has completed
        /// </summary>
        protected virtual void OnProcessCompleted()
        {
        }

        private void RegisterToOperationsEvents()
        {
            foreach (IFdoOperation operation in operations)
            {
                operation.OnFeatureProcessed += OnFeatureProcessed;
                operation.OnFinishedProcessing += OnFinishedProcessing;
            }
        }


        /// <summary>
        /// Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected virtual void OnFinishedProcessing(FdoOperationBase op)
        {
            Notice("Finished {0}: {1}", op.Name, op.Statistics);
        }

        /// <summary>
        /// Allow derived class to deal with custom logic after all the internal steps have been executed
        /// </summary>
        protected virtual void PostProcessing()
        {
        }

        /// <summary>
        /// Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected virtual void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % 1000 == 0)
                Info("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
            else
                Debug("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
        }

        /// <summary>
        /// Gets all errors that occured during the execution of this process
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Exception> GetAllErrors()
        {
            foreach (Exception error in Errors)
            {
                yield return error;
            }
            foreach (Exception error in PipelineExecuter.GetAllErrors())
            {
                yield return error;
            }
            foreach (IFdoOperation operation in operations)
            {
                foreach (Exception exception in operation.GetAllErrors())
                {
                    yield return exception;
                }
            }
        }
    }
}
