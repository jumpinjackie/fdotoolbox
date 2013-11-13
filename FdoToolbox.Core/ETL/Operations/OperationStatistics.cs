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
using System.Threading;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Contains the statistics for an operation
    /// </summary>
    public class OperationStatistics
    {
        private DateTime? start;
        private DateTime? end;
        private long outputtedRows = 0;
        private long failedRows = 0;

        /// <summary>
        /// Gets number of the outputted rows.
        /// </summary>
        /// <value>The processed rows.</value>
        public long OutputtedRows
        {
            get { return outputtedRows; }
        }

        /// <summary>
        /// Gets number of the failed rows
        /// </summary>
        public long FailedRows
        {
            get { return failedRows; }
        }

        /// <summary>
        /// Gets the duration this operation has executed
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration
        {
            get
            {
                if (start == null || end == null)
                    return new TimeSpan();

                return end.Value - start.Value;
            }
        }

        /// <summary>
        /// Mark the start time
        /// </summary>
        public void MarkStarted()
        {
            start = DateTime.Now;
        }

        /// <summary>
        /// Mark the end time
        /// </summary>
        public void MarkFinished()
        {
            end = DateTime.Now;
        }

        /// <summary>
        /// Marks a processed row.
        /// </summary>
        public void MarkFeatureProcessed()
        {
            Interlocked.Increment(ref outputtedRows);
        }

        /// <summary>
        /// Marks a failed row
        /// </summary>
        public void MarkFeatureFailed()
        {
            Interlocked.Increment(ref failedRows);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return OutputtedRows + " features in " + Duration + ". " + FailedRows + " features failed to be processed.";
        }

        /// <summary>
        /// Adds to the count of the output rows.
        /// </summary>
        /// <param name="rowProcessed">The row processed.</param>
        public void AddOutputRows(long rowProcessed)
        {
            Interlocked.Increment(ref outputtedRows);
        }

        /// <summary>
        /// Marks the feature batch processed.
        /// </summary>
        /// <param name="count">The count.</param>
        public void MarkFeatureProcessed(int count)
        {
            Interlocked.Add(ref outputtedRows, count);
        }
    }
}
