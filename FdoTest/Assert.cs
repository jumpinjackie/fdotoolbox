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

using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public AssertException(string message, string caller)
            : base(message) 
        {
            this.Caller = caller;
        }
        public AssertException(string message, Exception inner) : base(message, inner) { }

        public string Caller { get; }
        protected AssertException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public static class Assert
    {
        static string FormatCaller(string member, string sourcePath, int lineNum)
            => $"at {member} ({sourcePath}, line: {lineNum})";

        public static void Throws<TException>(
            Action action,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) where TException : Exception
        {
            try
            {
                action.Invoke();
                Assert.Fail($"Expected exception of type {typeof(TException)} to be thrown", memberName, sourceFilePath, sourceLineNumber);
            }
            catch (TException)
            {

            }
        }

        public static void Fail(
            string failMessage,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) 
            => throw new AssertException(failMessage, FormatCaller(memberName, sourceFilePath, sourceLineNumber));

        internal static void NotNull(
            object obj,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (obj == null)
                throw new AssertException("Expected given object to not be null", FormatCaller(memberName, sourceFilePath, sourceLineNumber));
        }

        public static void Equal<T>(
            T expected,
            T value,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
            => Equal(expected, value, $"Expected: {expected}, Got: {value}", memberName, sourceFilePath, sourceLineNumber);

        public static void Equal<T>(
            T expected,
            T value,
            string failMessage,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, value))
                throw new AssertException(failMessage, FormatCaller(memberName, sourceFilePath, sourceLineNumber));
        }

        internal static void NotEmpty<T>(
            IEnumerable<T> sourceScs,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!sourceScs.Any())
                throw new AssertException("Expected a non-empty sequence/collection", FormatCaller(memberName, sourceFilePath, sourceLineNumber));
        }
    }
}
