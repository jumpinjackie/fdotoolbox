#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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

namespace FdoTest
{
    // Why are we inventing our own test framework and not using a battle-tested 
    // unit test framework like nUnit or xUnit?
    //
    // Because our code under test has bindings to unmanaged code and rather than
    // wasting time/resources to figure out how to get such code to properly run in
    // their respective runners without errors around missing dlls, we could just
    // re-create the core primitives of any test framework (Assert, etc) and just
    // roll our own test runner with the right unmanaged dll setup

    [Serializable]
    public class AssertException : Exception
    {
        public AssertException() { }
        public AssertException(string message) : base(message) { }
        public AssertException(string message, Exception inner) : base(message, inner) { }
        protected AssertException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public static class Assert
    {
        public static void Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action.Invoke();
                Assert.Fail($"Expected exception of type {typeof(TException)} to be thrown");
            }
            catch (TException)
            {

            }
        }

        public static void Fail(string failMessage) => throw new AssertException(failMessage);

        public static void Equal<T>(T expected, T value)
            => Equal(expected, value, $"Expected: {expected}, Got: {value}");

        public static void Equal<T>(T expected, T value, string failMessage)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, value))
                throw new AssertException(failMessage);
        }
    }
}
