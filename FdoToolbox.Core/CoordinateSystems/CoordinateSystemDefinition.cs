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

namespace FdoToolbox.Core.CoordinateSystems
{
    /// <summary>
    /// Data transfer object for Coordinate Systems
    /// </summary>
    public class CoordinateSystemDefinition
    {
        private string _Name;

        /// <summary>
        /// The user-defined name of the coordinate system
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Description;

        /// <summary>
        /// The user-defined description of the coordinate system
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Wkt;

        /// <summary>
        /// The Well Known Text representation of the coordinate system
        /// </summary>
        public string Wkt
        {
            get { return _Wkt; }
            set { _Wkt = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="wkt"></param>
        public CoordinateSystemDefinition(string name, string description, string wkt)
        {
            this.Name = name;
            this.Description = description;
            this.Wkt = wkt;
        }
    }
}
