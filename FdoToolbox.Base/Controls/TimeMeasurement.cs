#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.Diagnostics;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// Class to measure time elapsed between instantiation to disposal
    /// </summary>
    public class TimeMeasurement : IDisposable
    {
        private Stopwatch _sw;
        private Action<TimeSpan> onElapsed;

        public TimeMeasurement(Action<TimeSpan> onElapsed)
        {
            this.onElapsed = onElapsed;
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _sw.Stop();
            if (onElapsed != null)
                onElapsed(_sw.Elapsed);
        }
    }
}
