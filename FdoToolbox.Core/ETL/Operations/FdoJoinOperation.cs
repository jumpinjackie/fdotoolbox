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

namespace FdoToolbox.Core.ETL.Operations
{
    using Enumerables;
    using FdoToolbox.Core.Configuration;

    /// <summary>
    /// Perform a join between two sources
    /// </summary>
    public abstract class FdoJoinOperation : FdoOperationBase
    {
        private readonly PartialProcessOperation left = new PartialProcessOperation();
        private readonly PartialProcessOperation right = new PartialProcessOperation();
        private JoinType jointype;
        private string[] leftColumns;
        private string[] rightColumns;
        private Dictionary<FdoRow, object> rightRowsWereMatched = new Dictionary<FdoRow, object>();
        private Dictionary<ObjectArrayKeys, List<FdoRow>> rightRowsByJoinKey = new Dictionary<ObjectArrayKeys, List<FdoRow>>();

        /// <summary>
        /// Sets the right part of the join
        /// </summary>
        /// <value>The right.</value>
        public FdoJoinOperation Right(IFdoOperation value)
        {
            right.Register(value);
            return this;
        }


        /// <summary>
        /// Sets the left part of the join
        /// </summary>
        /// <value>The left.</value>
        public FdoJoinOperation Left(IFdoOperation value)
        {
            left.Register(value);
            return this;
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="ignored">Ignored rows</param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> ignored)
        {
            PrepareForJoin();

            IEnumerable<FdoRow> rightEnumerable = GetRightEnumerable();

            IEnumerable<FdoRow> execute = left.Execute(null);
            foreach (FdoRow leftRow in new EventRaisingEnumerator(left, execute))
            {
                ObjectArrayKeys key = leftRow.CreateKey(leftColumns);
                List<FdoRow> rightRows;
                if (this.rightRowsByJoinKey.TryGetValue(key, out rightRows))
                {
                    foreach (FdoRow rightRow in rightRows)
                    {
                        rightRowsWereMatched[rightRow] = null;
                        yield return MergeRows(leftRow, rightRow);
                    }
                }
                else if ((jointype & JoinType.Left) != 0)
                {
                    FdoRow emptyRow = new FdoRow();
                    yield return MergeRows(leftRow, emptyRow);
                }
                else
                {
                    LeftOrphanRow(leftRow);
                }
            }
            foreach (FdoRow rightRow in rightEnumerable)
            {
                if (rightRowsWereMatched.ContainsKey(rightRow))
                    continue;
                FdoRow emptyRow = new FdoRow();
                if ((jointype & JoinType.Right) != 0)
                    yield return MergeRows(emptyRow, rightRow);
                else
                    RightOrphanRow(rightRow);
            }
        }

        private void PrepareForJoin()
        {
            Initialize();

            if(left == null)
                throw new InvalidOperationException("Left branch of a join cannot be null");
            if(right == null) 
                throw new InvalidOperationException("Right branch of a join cannot be null");

            SetupJoinConditions();

            if(leftColumns == null)
                throw new InvalidOperationException("You must setup the left columns");
            if(rightColumns == null)
                throw new InvalidOperationException("You must setup the right columns");
        }

        private IEnumerable<FdoRow> GetRightEnumerable()
        {
            IEnumerable<FdoRow> rightEnumerable = new CachingEnumerable<FdoRow>(
                new EventRaisingEnumerator(right, right.Execute(null))
                );
            foreach (FdoRow row in rightEnumerable)
            {
                ObjectArrayKeys key = row.CreateKey(rightColumns);
                List<FdoRow> rowsForKey;
                if (this.rightRowsByJoinKey.TryGetValue(key, out rowsForKey) == false)
                {
                    this.rightRowsByJoinKey[key] = rowsForKey = new List<FdoRow>();
                }
                rowsForKey.Add(row);
            }
            return rightEnumerable;
        }

        /// <summary>
        /// Called when a row on the right side was filtered by
        /// the join condition, allow a derived class to perform 
        /// logic associated to that, such as logging
        /// </summary>
        protected virtual void RightOrphanRow(FdoRow row)
        {

        }

        /// <summary>
        /// Called when a row on the left side was filtered by
        /// the join condition, allow a derived class to perform 
        /// logic associated to that, such as logging
        /// </summary>
        /// <param name="row">The row.</param>
        protected virtual void LeftOrphanRow(FdoRow row)
        {

        }

        /// <summary>
        /// Merges the two rows into a single row
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract FdoRow MergeRows(FdoRow leftRow, FdoRow rightRow);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Setups the join conditions.
        /// </summary>
        protected abstract void SetupJoinConditions();

        /// <summary>
        /// Create an inner join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder InnerJoin => new JoinBuilder(this, JoinType.Inner);

        /// <summary>
        /// Create a left outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder LeftJoin => new JoinBuilder(this, JoinType.Left);


        /// <summary>
        /// Create a right outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder RightJoin => new JoinBuilder(this, JoinType.Right);


        /// <summary>
        /// Create a full outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder FullOuterJoin => new JoinBuilder(this, JoinType.Full);


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                left.Dispose();
                right.Dispose();
            }
        }


        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            left.PrepareForExecution(pipelineExecuter);
            right.PrepareForExecution(pipelineExecuter);
        }

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors()
        {
            foreach (Exception error in left.GetAllErrors())
            {
                yield return error;
            }
            foreach (Exception error in right.GetAllErrors())
            {
                yield return error;
            }
        }

        /// <summary>
        /// Fluent interface to create joins
        /// </summary>
        public class JoinBuilder
        {
            private readonly FdoJoinOperation parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="JoinBuilder"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="joinType">Type of the join.</param>
            public JoinBuilder(FdoJoinOperation parent, JoinType joinType)
            {
                this.parent = parent;
                parent.jointype = joinType;
            }

            /// <summary>
            /// Setup the left side of the join
            /// </summary>
            /// <param name="columns">The columns.</param>
            /// <returns></returns>
            public JoinBuilder Left(params string[] columns)
            {
                parent.leftColumns = columns;
                return this;
            }

            /// <summary>
            /// Setup the right side of the join
            /// </summary>
            /// <param name="columns">The columns.</param>
            /// <returns></returns>
            public JoinBuilder Right(params string[] columns)
            {
                parent.rightColumns = columns;
                return this;
            }
        }
    }
}
