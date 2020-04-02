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
    /// Error object for an incompatible feature class
    /// </summary>
    public class IncompatibleClass
    {
        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private List<IncompatibleProperty> _Properties;

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public ReadOnlyCollection<IncompatibleProperty> Properties
        {
            get { return _Properties.AsReadOnly(); }
        }

        private List<string> _Reasons;

        /// <summary>
        /// Gets the reasons.
        /// </summary>
        /// <value>The reasons.</value>
        public ReadOnlyCollection<string> Reasons
        {
            get { return _Reasons.AsReadOnly(); }
        }

        private ISet<IncompatibleClassReason> _ReasonCodes;

        /// <summary>
        /// Gets the reason codes.
        /// </summary>
        /// <value>The reason codes.</value>
        public ISet<IncompatibleClassReason> ReasonCodes
        {
            get { return _ReasonCodes; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleClass"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="reason">The reason.</param>
        public IncompatibleClass(string name, string reason)
        {
            this.Name = name;
            _Properties = new List<IncompatibleProperty>();
            _Reasons = new List<string>();
            _Reasons.Add(reason);
            _ReasonCodes = new HashSet<IncompatibleClassReason>();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Incompatible Class: " + this.Name + Environment.NewLine);
            if (this.Reasons.Count > 0)
            {
                sb.Append("Reasons:"+Environment.NewLine);
                foreach (string str in this.Reasons)
                {
                    sb.Append(" - " + str + Environment.NewLine);
                }
            }
            if (this.Properties.Count > 0)
            {
                sb.Append("Incompatible Properties:" + Environment.NewLine);
                foreach (IncompatibleProperty prop in this.Properties)
                {
                    sb.Append(prop.ToString() + Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds the reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public void AddReason(string reason)
        {
            _Reasons.Add(reason);
        }

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="prop">The prop.</param>
        public void AddProperty(IncompatibleProperty prop)
        {
            _Properties.Add(prop);
        }

        /// <summary>
        /// Finds the property.
        /// </summary>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        public IncompatibleProperty FindProperty(string propName)
        {
            foreach (IncompatibleProperty prop in _Properties)
            {
                if (prop.Name == propName)
                    return prop;
            }
            return null;
        }
    }
}
