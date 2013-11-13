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
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using System.Drawing;
using SharpMap.Data;
using SharpMap.Geometries;

namespace FdoToolbox.Base.SharpMapProvider
{
    /// <summary>
    /// Represents a randmoized map theme
    /// </summary>
    public class RandomizedTheme : ITheme
    {
        private Random rand = new Random();

        private Dictionary<Geometry, Brush> _brushes = new Dictionary<Geometry, Brush>();

        /// <summary>
        /// Returns the style based on a feature
        /// </summary>
        /// <param name="attribute">Attribute to calculate color from</param>
        /// <returns>Color</returns>
        public SharpMap.Styles.IStyle GetStyle(FeatureDataRow attribute)
        {
            VectorStyle vs = new VectorStyle();

            if (!_brushes.ContainsKey(attribute.Geometry))
                _brushes[attribute.Geometry] = new SolidBrush(Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)));

            vs.Fill = _brushes[attribute.Geometry];
            vs.Outline = new Pen(Color.Black);
            vs.EnableOutline = true;
            return vs;
        }
    }
}
