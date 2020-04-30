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

using OSGeo.FDO.Geometry;
using OSGeo.FDO.Spatial;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// <see cref="IGeometry"/> decorator object.
    /// </summary>
    public class FdoGeometry : IFdoGeometry
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoGeometry"/> class.
        /// </summary>
        /// <param name="geom">The geom.</param>
        public FdoGeometry(IGeometry geom)
        {
            InternalInstance = geom;
        }

        /// <summary>
        /// Gets the dervied type
        /// </summary>
        /// <value>The type of the derived.</value>
        public OSGeo.FDO.Common.GeometryType DerivedType => InternalInstance.DerivedType;

        /// <summary>
        /// Gets the dimensionality.
        /// </summary>
        /// <value>The dimensionality.</value>
        public int Dimensionality => InternalInstance.Dimensionality;

        /// <summary>
        /// Gets the envelope.
        /// </summary>
        /// <value>The envelope.</value>
        public IEnvelope Envelope => InternalInstance.Envelope;

        /// <summary>
        /// Gets the FGF text.
        /// </summary>
        /// <value>The text.</value>
        public string Text => InternalInstance.Text;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InternalInstance.Dispose();
        }

        /// <summary>
        /// Gets the decorated/wrapped geometry.
        /// </summary>
        /// <remarks>
        /// When passing any <see cref="IGeometry"/> instances to any FDO API, you cannot pass this decorated version
        /// you must pass the internal geometry represented by this property. Otherwise, unexpected behaviour may
        /// occur.
        /// </remarks>
        public IGeometry InternalInstance { get; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            //This is the whole reason for having a decorator. When in a DataTable, the native IGeometry's ToString() shows nothing, when it should be really showing the FGF text
            return InternalInstance.Text;
        }

        /// <summary>
        /// Determines whether this instance contains the specified envelope
        /// </summary>
        /// <param name="env">The envelope</param>
        /// <returns>
        /// 	<c>true</c> if this instance contains the specified envelope; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(IEnvelope env)
        {
            FdoGeometryFactory fact = FdoGeometryFactory.Instance;
            IGeometry geom = fact.CreateGeometry(env);
            bool contains = SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Contains, geom);
            geom.Dispose();
            return contains;
        }

        /// <summary>
        /// Determines whether this instance contains the specified point
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>
        /// 	<c>true</c> if this instance contains the specified point
        /// </returns>
        public bool Contains(double x, double y)
        {
            IPoint pt = FdoGeometryFactory.Instance.CreatePoint(FdoGeometryUtil.FDO_DIM_XY, new double[] { x, y });
            bool contains = SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Contains, pt);
            pt.Dispose();
            return contains;
        }

        /// <summary>
        /// Determines whether this instance contains the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Contains(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Contains, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance crosses the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Crosses(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Crosses, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance is disjoint with the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Disjoint(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Disjoint, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance is equal to the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Equals(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Equals, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance intersects the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Intersects(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Intersects, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance overlaps the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Overlaps(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Overlaps, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance touches the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Touches(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Touches, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance is within the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Within(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Within, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance is covered by the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool CoveredBy(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_CoveredBy, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance is inside the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool Inside(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_Inside, geom.InternalInstance);
        }

        /// <summary>
        /// Determines whether this instance's envelope intersects the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        public bool EnvelopeIntersects(IFdoGeometry geom)
        {
            return SpatialUtility.Evaluate(this.InternalInstance, OSGeo.FDO.Filter.SpatialOperations.SpatialOperations_EnvelopeIntersects, geom.InternalInstance);
        }
    }

    /// <summary>
    /// FDO Geometry interface
    /// </summary>
    public interface IFdoGeometry : IGeometry
    {
        /// <summary>
        /// Gets the decorated/wrapped geometry.
        /// </summary>
        /// <remarks>
        /// When passing any <see cref="IGeometry"/> instances to any FDO API, you cannot pass this decorated version
        /// you must pass the internal geometry represented by this property. Otherwise, unexpected behaviour may
        /// occur.
        /// </remarks>
        IGeometry InternalInstance { get; }

        /// <summary>
        /// Determines whether this instance contains the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Contains(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance crosses the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Crosses(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance is disjoint with the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Disjoint(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance is equal to the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Equals(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance intersects the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Intersects(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance overlaps the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Overlaps(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance touches the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Touches(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance is within the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Within(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance is covered by the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool CoveredBy(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance is inside the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool Inside(IFdoGeometry geom);

        /// <summary>
        /// Determines whether this instance's envelope intersects the specified geometry
        /// </summary>
        /// <param name="geom">The geometry to test against</param>
        /// <returns></returns>
        bool EnvelopeIntersects(IFdoGeometry geom);
    }
}
