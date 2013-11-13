// Copyright 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.CoordinateSystems.Transformations
{
	/// <summary>
	/// Transformation for applying 
	/// </summary>
	internal class DatumTransform : MathTransform
	{
		protected IMathTransform _inverse;
		private Wgs84ConversionInfo _ToWgs94;
		double[] v;
		private bool _isInverse = false;
		public DatumTransform(Wgs84ConversionInfo towgs84) : this(towgs84,false)
		{
		}
		private DatumTransform(Wgs84ConversionInfo towgs84, bool isInverse)
		{
			_ToWgs94 = towgs84;
			v = _ToWgs94.GetAffineTransform();
			_isInverse = isInverse;
		}
		public override string WKT
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override string XML
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override IMathTransform Inverse()
		{
			if (_inverse == null)
				_inverse = new DatumTransform(_ToWgs94,!_isInverse);
			return _inverse;
		}

		private SharpMap.Geometries.Point3D Apply(SharpMap.Geometries.Point3D p)
		{
			return new SharpMap.Geometries.Point3D(
				v[0] * p.X - v[3] * p.Y + v[2] * p.Z + v[4],
				v[3] * p.X + v[0] * p.Y - v[1] * p.Z + v[5],
			   -v[2] * p.X + v[1] * p.Y + v[0] * p.Z + v[6]);
			//double[,] v = GetAffineTransform();
			//return new SharpMap.Geometries.Point3D(
			//    v[0, 0] * p.X + v[0, 1] * p.Y + v[0, 2] * p.Z + v[0, 3],
			//    v[1, 0] * p.X + v[1, 1] * p.Y + v[1, 2] * p.Z + v[1, 3],
			//    v[2, 0] * p.X + v[2, 1] * p.Y + v[2, 2] * p.Z + v[2, 3]);
		}

		private SharpMap.Geometries.Point3D ApplyInverted(SharpMap.Geometries.Point3D p)
		{
			return new SharpMap.Geometries.Point3D(
				v[0] * p.X + v[3] * p.Y - v[2] * p.Z - v[4],
			   -v[3] * p.X + v[0] * p.Y + v[1] * p.Z - v[5],
			    v[2] * p.X - v[1] * p.Y + v[0] * p.Z - v[6]);
			//double[,] v = GetAffineTransform();
			//return new SharpMap.Geometries.Point3D(
			//    v[0, 0] * p.X + v[0, 1] * p.Y + v[0, 2] * p.Z + v[0, 3],
			//    v[1, 0] * p.X + v[1, 1] * p.Y + v[1, 2] * p.Z + v[1, 3],
			//    v[2, 0] * p.X + v[2, 1] * p.Y + v[2, 2] * p.Z + v[2, 3]);
		}

		public override SharpMap.Geometries.Point Transform(SharpMap.Geometries.Point point)
		{
			if (!(point is SharpMap.Geometries.Point3D))
				throw new ArgumentException("Datum transformation requires a 3D point");
			if (!_isInverse)
				return Apply(point as SharpMap.Geometries.Point3D);
			else
				return ApplyInverted(point as SharpMap.Geometries.Point3D);
		}

		public override List<SharpMap.Geometries.Point> TransformList(List<SharpMap.Geometries.Point> points)
		{
			List<SharpMap.Geometries.Point> pnts = new List<SharpMap.Geometries.Point>(points.Count);
			foreach(SharpMap.Geometries.Point p in points)
				pnts.Add(Transform(p));
			return pnts;
		}

		public override void Invert()
		{
			_isInverse = !_isInverse;
		}
	}
}
