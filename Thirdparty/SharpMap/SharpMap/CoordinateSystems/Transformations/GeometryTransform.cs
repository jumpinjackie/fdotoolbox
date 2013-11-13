// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using SharpMap.Geometries;

namespace SharpMap.CoordinateSystems.Transformations
{
	/// <summary>
	/// Helper class for transforming <see cref="SharpMap.Geometries.Geometry"/>
	/// </summary>
	public class GeometryTransform
	{
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.BoundingBox"/>.
		/// </summary>
		/// <param name="box">BoundingBox to transform</param>
		/// <param name="transform">Math Transform</param>
		/// <returns>Transformed object</returns>
		public static BoundingBox TransformBox(BoundingBox box, IMathTransform transform)
		{
			if (box == null)
				return null;
			SharpMap.Geometries.Point[] corners = new Point[4];
			corners[0] = transform.Transform(box.Min); //LL
			corners[1] = transform.Transform(box.Max); //UR
			corners[2] = transform.Transform(new Point(box.Min.X, box.Max.Y)); //UL
			corners[3] = transform.Transform(new Point(box.Max.X, box.Min.Y)); //LR

			BoundingBox result = corners[0].GetBoundingBox();
			for (int i = 1; i < 4; i++)
				result = result.Join(corners[i].GetBoundingBox());
			return result;
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.Geometry"/>.
		/// </summary>
		/// <param name="g">Geometry to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed Geometry</returns>
		public static Geometry TransformGeometry(Geometry g, IMathTransform transform)
		{
			if (g==null)
				return null;
			else if (g is Point)
				return TransformPoint(g as Point, transform);
			else if (g is LineString)
				return TransformLineString(g as LineString, transform);
			else if (g is Polygon)
				return TransformPolygon(g as Polygon, transform);
			else if (g is MultiPoint)
				return TransformMultiPoint(g as MultiPoint, transform);
			else if (g is MultiLineString)
				return TransformMultiLineString(g as MultiLineString, transform);
			else if (g is MultiPolygon)
				return TransformMultiPolygon(g as MultiPolygon, transform);
			else
				throw new ArgumentException("Could not transform geometry type '" + g.GetType().ToString() +"'");
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.Point"/>.
		/// </summary>
		/// <param name="p">Point to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed Point</returns>
		public static Point TransformPoint(Point p, IMathTransform transform)
		{
			try { return transform.Transform(p); }
			catch { return null; }
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.LineString"/>.
		/// </summary>
		/// <param name="l">LineString to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed LineString</returns>
		public static LineString TransformLineString(LineString l, IMathTransform transform)
		{
			try { return new LineString(transform.TransformList(l.Vertices)); }
			catch { return null; }
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.LinearRing"/>.
		/// </summary>
		/// <param name="r">LinearRing to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed LinearRing</returns>
		public static LinearRing TransformLinearRing(LinearRing r, IMathTransform transform)
		{
			try { return new LinearRing(transform.TransformList(r.Vertices)); }
			catch { return null; }
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.Polygon"/>.
		/// </summary>
		/// <param name="p">Polygon to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed Polygon</returns>
		public static Polygon TransformPolygon(Polygon p, IMathTransform transform)
		{
			Polygon pOut = new Polygon(TransformLinearRing(p.ExteriorRing, transform));
			pOut.InteriorRings = new List<LinearRing>(p.InteriorRings.Count); //Pre-inialize array size for better performance
			for (int i = 0; i < p.InteriorRings.Count; i++)
				pOut.InteriorRings.Add(TransformLinearRing(p.InteriorRings[i], transform));
			return pOut;
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.MultiPoint"/>.
		/// </summary>
		/// <param name="points">MultiPoint to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed MultiPoint</returns>
		public static MultiPoint TransformMultiPoint(MultiPoint points, IMathTransform transform)
		{
			MultiPoint pOut = new MultiPoint();
			pOut.Points = transform.TransformList(points.Points);			
			return pOut;
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.MultiLineString"/>.
		/// </summary>
		/// <param name="lines">MultiLineString to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed MultiLineString</returns>
		public static MultiLineString TransformMultiLineString(MultiLineString lines, IMathTransform transform)
		{
			MultiLineString lOut = new MultiLineString();
			lOut.LineStrings = new List<LineString>(lines.LineStrings.Count); //Pre-inialize array size for better performance
			for (int i = 0; i < lines.LineStrings.Count;i++ )
				lOut.LineStrings.Add(TransformLineString(lines[i], transform));
			return lOut;
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.MultiPolygon"/>.
		/// </summary>
		/// <param name="polys">MultiPolygon to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed MultiPolygon</returns>
		public static MultiPolygon TransformMultiPolygon(MultiPolygon polys, IMathTransform transform)
		{
			MultiPolygon pOut = new MultiPolygon();
			pOut.Polygons = new List<Polygon>(polys.Polygons.Count); //Pre-inialize array size for better performance
			for (int i = 0; i < polys.NumGeometries; i++)
				pOut.Polygons.Add(TransformPolygon(polys[i], transform));
			return pOut;
		}
		/// <summary>
		/// Transforms a <see cref="SharpMap.Geometries.GeometryCollection"/>.
		/// </summary>
		/// <param name="geoms">GeometryCollection to transform</param>
		/// <param name="transform">MathTransform</param>
		/// <returns>Transformed GeometryCollection</returns>
		public static GeometryCollection TransformGeometryCollection(GeometryCollection geoms, IMathTransform transform)
		{
			GeometryCollection gOut = new GeometryCollection();
			gOut.Collection = new List<Geometry>(geoms.Collection.Count); //Pre-inialize array size for better performance
			for (int i = 0; i < geoms.Collection.Count;i++ )
				gOut.Collection.Add(TransformGeometry(geoms[i], transform));
			return gOut;
		}
	}
}
