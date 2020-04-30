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
    /// Error object for an incompatible feature schema
    /// </summary>
    public class IncompatibleSchema
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        private List<IncompatibleClass> _Classes;

        /// <summary>
        /// Gets the classes.
        /// </summary>
        /// <value>The classes.</value>
        public ReadOnlyCollection<IncompatibleClass> Classes => _Classes.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleSchema"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public IncompatibleSchema(string name)
        {
            this.Name = name;
            _Classes = new List<IncompatibleClass>();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Incompatible schema: " + this.Name + Environment.NewLine);
            foreach (IncompatibleClass cls in this.Classes)
            {
                sb.Append(cls.ToString() + System.Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds the class.
        /// </summary>
        /// <param name="cls">The CLS.</param>
        public void AddClass(IncompatibleClass cls)
        {
            _Classes.Add(cls);
        }
    }
}
