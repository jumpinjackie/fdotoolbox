#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
    /// Represents a base class of operations that execute only once
    /// </summary>
    public abstract class FdoSingleActionOperationBase : FdoOperationBase
    {
        public abstract void ExecuteAction();

        private int counter = 0;

        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            if (counter < 1) //reentrancy guard
            {
                ExecuteAction();
                counter++;
            }
            return rows;
        }
    }

    public delegate void ParameterlessAction();

    /// <summary>
    /// Represents a operation that only executes once. The action to be
    /// executed is represented by the <see cref="ParameterlessAction"/>
    /// delegate, which is passed in externally
    /// </summary>
    public class FdoSingleActionOperation : FdoSingleActionOperationBase
    {
        private ParameterlessAction _action;

        public FdoSingleActionOperation(ParameterlessAction action)
        {
            _action = action;
        }

        public override void ExecuteAction()
        {
            _action();
        }
    }

    /// <summary>
    /// Represents an operation that only executes once. The action to be
    /// executed is represented by the <see cref="Action{T}"/>
    /// delegate, which is passed in externally
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FdoSingleActionOperation<T> : FdoSingleActionOperationBase
    {
        private Action<T> _action;
        private T _param;

        public FdoSingleActionOperation(Action<T> action, T param)
        {
            _param = param;
            _action = action;
        }

        public override void ExecuteAction()
        {
            _action(_param);
        }
    }
}
