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

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Specialized ETL process interface
    /// </summary>
    public interface IFdoSpecializedEtlProcess
    {
        /// <summary>
        /// Executes the process
        /// </summary>
        void Execute();
        
        /// <summary>
        /// Fires when a feature has been processed
        /// </summary>
        event FeatureCountEventHandler FeatureProcessed;

        /// <summary>
        /// Fires when a feature
        /// </summary>
        event MessageEventHandler ProcessMessage;

        /// <summary>
        /// Casts this to a <see cref="EtlProcess"/>
        /// </summary>
        /// <returns></returns>
        EtlProcess ToEtlProcess();

        /// <summary>
        /// Updates affected connection references. Does nothing if the process
        /// does not hold connection name references
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        void UpdateConnectionReferences(string oldName, string newName);
    }

    /// <summary>
    /// Event handler for counting features processed
    /// </summary>
    public delegate void FeatureCountEventHandler(object sender, FeatureCountEventArgs e);

    /// <summary>
    /// Event object for counting features processed
    /// </summary>
    public class FeatureCountEventArgs
    {
        /// <summary>
        /// The number of features
        /// </summary>
        public readonly int Features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCountEventArgs"/> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public FeatureCountEventArgs(int features)
        {
            this.Features = features;
        }
    }

    /// <summary>
    /// Event handler for sending messages
    /// </summary>
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);
}
