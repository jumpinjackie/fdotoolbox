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

namespace FdoToolbox.Core.ETL.Enumerables
{
    using Operations;
    using System.Collections;

    /// <summary>
    /// An enumerator that will raise the events on the operation for each iterated item
    /// </summary>
    public class EventRaisingEnumerator : IEnumerable<FdoRow>, IEnumerator<FdoRow>
    {
        private readonly IFdoOperation operation;
        private readonly IEnumerable<FdoRow> inner;
        private IEnumerator<FdoRow> innerEnumerator;
        private FdoRow previous;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventRaisingEnumerator"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="inner">The innerEnumerator.</param>
        public EventRaisingEnumerator(IFdoOperation operation, IEnumerable<FdoRow> inner)
        {
            this.operation = operation;
            this.inner = inner;
        }

        ///<summary>
        ///Gets the element in the collection at the current position of the enumerator.
        ///</summary>
        ///
        ///<returns>
        ///The element in the collection at the current position of the enumerator.
        ///</returns>
        ///
        public FdoRow Current => innerEnumerator.Current;

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            innerEnumerator.Dispose();
        }

        ///<summary>
        ///Advances the enumerator to the next element of the collection.
        ///</summary>
        ///
        ///<returns>
        ///true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public bool MoveNext()
        {
            bool result = innerEnumerator.MoveNext();
            if (result)
            {
                previous = innerEnumerator.Current;
                operation.RaiseFeatureProcessed(Current);
            }
            else
                operation.RaiseFinishedProcessing();
            return result;
        }

        ///<summary>
        ///Sets the enumerator to its initial position, which is before the first element in the collection.
        ///</summary>
        ///
        ///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public void Reset()
        {
            innerEnumerator.Reset();
        }

        ///<summary>
        ///Gets the current element in the collection.
        ///</summary>
        ///
        ///<returns>
        ///The current element in the collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception><filterpriority>2</filterpriority>
        object IEnumerator.Current => innerEnumerator.Current;

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<FdoRow> IEnumerable<FdoRow>.GetEnumerator()
        {
            if(inner == null) 
                throw new InvalidOperationException("Null enumerator detected, are you trying to read from the first operation in the process?");
            innerEnumerator = inner.GetEnumerator();
            return this;
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<FdoRow>)this).GetEnumerator();
        }
    }
}
