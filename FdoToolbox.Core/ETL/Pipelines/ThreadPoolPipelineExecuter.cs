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

namespace FdoToolbox.Core.ETL.Pipelines
{
    using Enumerables;
    using Operations;
    using System.Threading;

    /// <summary>
    /// Execute all the actions concurrently, in the thread pool
    /// </summary>
    public class ThreadPoolPipelineExecuter : BasePipelineExecuter
    {
        /// <summary>
        /// Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<FdoRow> DecorateEnumerable(IFdoOperation operation, IEnumerable<FdoRow> enumerator)
        {
            ThreadSafeEnumerator<FdoRow> threadedEnumerator = new ThreadSafeEnumerator<FdoRow>();
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    foreach (FdoRow t in new EventRaisingEnumerator(operation, enumerator))
                    {
                        threadedEnumerator.AddItem(t);
                    }
                }
                catch (Exception e)
                {
                    Error(e, "Failed to execute operation {0}", operation);
                    threadedEnumerator.MarkAsFinished();
                }
                finally
                {
                    threadedEnumerator.MarkAsFinished();
                }
            });
            return threadedEnumerator;
        }
    }
}
