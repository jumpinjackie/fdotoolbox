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
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Error object for an incompatible class property
    /// </summary>
    public class IncompatibleProperty
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        private List<string> _Reasons;

        /// <summary>
        /// Gets the reasons.
        /// </summary>
        /// <value>The reasons.</value>
        public ReadOnlyCollection<string> Reasons => _Reasons.AsReadOnly();

        /// <summary>
        /// Gets the reason codes.
        /// </summary>
        /// <value>The reason codes.</value>
        public ISet<IncompatiblePropertyReason> ReasonCodes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="reason">The reason.</param>
        public IncompatibleProperty(string name, string reason)
        {
            this.Name = name;
            _Reasons = new List<string>();
            ReasonCodes = new HashSet<IncompatiblePropertyReason>();
            _Reasons.Add(reason);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleProperty"/> class.
        /// </summary>
        public IncompatibleProperty() { }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Incompatible Property: " + this.Name + Environment.NewLine);
            if (this.Reasons.Count > 0)
            {
                sb.Append("Reasons: " + Environment.NewLine);
                foreach (string str in this.Reasons)
                {
                    sb.Append(" - " + str + Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds the reason.
        /// </summary>
        /// <param name="propReason">The prop reason.</param>
        public void AddReason(string propReason)
        {
            _Reasons.Add(propReason);
        }
    }
}
