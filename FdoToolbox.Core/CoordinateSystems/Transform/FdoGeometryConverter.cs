using OSGeo.FDO.Common;
using OSGeo.FDO.Geometry;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdoToolbox.Core.CoordinateSystems.Transform
{
    /// <summary>
    /// A managed implementation of FdoSpatialGeometryConverter
    /// </summary>
    public abstract class FdoGeometryConverter : IDisposable
    {
        protected FgfGeometryFactory _geomFactory;
        private ArrayPool<double> _pool;

        /// X and Y dimensions are present.
        const int FdoDimensionality_XY = 0;
        /// Z dimension is present.
        const int FdoDimensionality_Z = 1;
        /// M ('measure') dimension is present.
        const int FdoDimensionality_M = 2;

        protected FdoGeometryConverter()
        {
            _geomFactory = new FgfGeometryFactory();
            _pool = ArrayPool<double>.Shared;
        }

        public IGeometry ConvertOrdinates(IGeometry geometry,
                                          bool applyTargetDimensionality = false,
                                          int targetDimensionality = 0,
                                          double padValueZ = 0.0,
                                          double padValueM = 0.0)
        {
            IGeometry newGeometry = null;
            if (geometry == null)
                throw new ArgumentNullException(nameof(geometry));

            var geomType = geometry.DerivedType;
            switch (geomType)
            {
                case GeometryType.GeometryType_LineString:
                    {
                        var derivedGeom = (ILineString)geometry;
                        var dimensionality = derivedGeom.Dimensionality;
                        var numPositions = derivedGeom.Count;
                        var outputDim = applyTargetDimensionality ? targetDimensionality : dimensionality;
                        var numOrdinates = numPositions * DimensionalityToNumOrdinates(outputDim);
                        var newOrdinates = _pool.Rent(numOrdinates);
                        var pos = derivedGeom.Positions;
                        var ordinates = GetOrdinates(pos, outputDim);
                        ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref newOrdinates);
                        newGeometry = _geomFactory.CreateLineString(outputDim, numOrdinates, newOrdinates);
                        _pool.Return(newOrdinates);
                    }
                    break;
                case GeometryType.GeometryType_Point:
                    {
                        var derivedGeom = (IPoint)geometry;
                        var dimensionality = derivedGeom.Dimensionality;
                        var numPositions = 1;
                        var outputDim = applyTargetDimensionality ? targetDimensionality : dimensionality;
                        var numOrdinates = numPositions * DimensionalityToNumOrdinates(outputDim);
                        var newOrdinates = _pool.Rent(numOrdinates);
                        var ordinates = GetOrdinates(derivedGeom);
                        ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref newOrdinates);
                        newGeometry = _geomFactory.CreatePoint(outputDim, newOrdinates);
                        _pool.Return(newOrdinates);
                    }
                    break;
                case GeometryType.GeometryType_Polygon:
                    {
                        var derivedGeom = (IPolygon)geometry;
                        var outputDim = applyTargetDimensionality ? targetDimensionality : derivedGeom.Dimensionality;
                        var newInteriorRings = new LinearRingCollection();

                        var ring = derivedGeom.ExteriorRing;
                        var newExteriorRing = ConvertOrdinates(ring, outputDim, padValueZ, padValueM);

                        for (int i = 0; i < derivedGeom.InteriorRingCount; i++)
                        {
                            ring = derivedGeom.GetInteriorRing(i);
                            var newInteriorRing = ConvertOrdinates(ring, outputDim, padValueZ, padValueM);
                            newInteriorRings.Add(newInteriorRing);
                        }

                        newGeometry = _geomFactory.CreatePolygon(newExteriorRing, newInteriorRings);
                    }
                    break;
                case GeometryType.GeometryType_CurveString:
                    {
                        ICurveString derivedGeom = (ICurveString)geometry;
                        var outputDim = applyTargetDimensionality ? targetDimensionality : derivedGeom.Dimensionality;
                        var csc = derivedGeom.CurveSegments;
                        var newCsc = ConvertOrdinates(csc, outputDim, padValueZ, padValueM);

                        newGeometry = _geomFactory.CreateCurveString(newCsc);
                    }
                    break;

                case GeometryType.GeometryType_CurvePolygon:
                    {
                        var derivedGeom = (ICurvePolygon)geometry;
                        var outputDim = applyTargetDimensionality ? targetDimensionality : derivedGeom.Dimensionality;
                        var newInteriorRings = new RingCollection();

                        var ring = derivedGeom.ExteriorRing;
                        var csc = ring.CurveSegments;
                        var newCsc = ConvertOrdinates(csc, outputDim, padValueZ, padValueM);
                        var newExteriorRing = _geomFactory.CreateRing(newCsc);

                        for (int i = 0; i < derivedGeom.InteriorRingCount; i++)
                        {
                            ring = derivedGeom.get_InteriorRing(i);
                            csc = ring.CurveSegments;
                            newCsc = ConvertOrdinates(csc, outputDim, padValueZ, padValueM);
                            var newInteriorRing = _geomFactory.CreateRing(newCsc);
                            newInteriorRings.Add(newInteriorRing);
                        }

                        newGeometry = _geomFactory.CreateCurvePolygon(newExteriorRing, newInteriorRings);
                    }
                    break;

                case GeometryType.GeometryType_MultiPoint:
                    {
                        var derivedGeom = (IMultiPoint)geometry;
                        var dimensionality = derivedGeom.Dimensionality;
                        var numPositions = derivedGeom.Count;
                        var outputDim = applyTargetDimensionality ? targetDimensionality : dimensionality;
                        var numOrdinates = numPositions * DimensionalityToNumOrdinates(outputDim);
                        var newOrdinates = _pool.Rent(numOrdinates);
                        var ordinates = GetOrdinates(derivedGeom);
                        ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref newOrdinates);
                        newGeometry = _geomFactory.CreateMultiPoint(outputDim, numOrdinates, newOrdinates);
                    }
                    break;
                case GeometryType.GeometryType_MultiLineString:
                    {
                        var derivedGeom = (IMultiLineString)geometry;
                        var newSubGeometries = new LineStringCollection();

                        int numSubGeometries = derivedGeom.Count;

                        for (int i = 0; i < numSubGeometries; i++)
                        {
                            var subGeom = derivedGeom[i];
                            var newSubGeometry = ConvertOrdinates(subGeom, applyTargetDimensionality, targetDimensionality);
                            newSubGeometries.Add((ILineString)newSubGeometry);
                        }
                        newGeometry = _geomFactory.CreateMultiLineString(newSubGeometries);
                    }
                    break;
                case GeometryType.GeometryType_MultiPolygon:
                    {
                        var derivedGeom = (IMultiPolygon)geometry;
                        var newSubGeometries = new PolygonCollection();

                        int numSubGeometries = derivedGeom.Count;

                        for (int i = 0; i < numSubGeometries; i++)
                        {
                            var subGeom = derivedGeom[i];
                            var newSubGeometry = ConvertOrdinates(subGeom, applyTargetDimensionality, targetDimensionality);
                            newSubGeometries.Add((IPolygon)newSubGeometry);
                        }
                        newGeometry = _geomFactory.CreateMultiPolygon(newSubGeometries);
                    }
                    break;
                case GeometryType.GeometryType_MultiCurveString:
                    {
                        var derivedGeom = (IMultiCurveString)geometry;
                        var newSubGeometries = new CurveStringCollection();

                        int numSubGeometries = derivedGeom.Count;

                        for (int i = 0; i < numSubGeometries; i++)
                        {
                            var subGeom = derivedGeom[i];
                            var newSubGeometry = ConvertOrdinates(subGeom, applyTargetDimensionality, targetDimensionality);
                            newSubGeometries.Add((ICurveString)newSubGeometry);
                        }
                        newGeometry = _geomFactory.CreateMultiCurveString(newSubGeometries);
                    }
                    break;
                case GeometryType.GeometryType_MultiCurvePolygon:
                    {
                        var derivedGeom = (IMultiCurvePolygon)geometry;
                        var newSubGeometries = new CurvePolygonCollection();

                        int numSubGeometries = derivedGeom.Count;

                        for (int i = 0; i < numSubGeometries; i++)
                        {
                            var subGeom = derivedGeom[i];
                            var newSubGeometry = ConvertOrdinates(subGeom, applyTargetDimensionality, targetDimensionality);
                            newSubGeometries.Add((ICurvePolygon)newSubGeometry);
                        }
                        newGeometry = _geomFactory.CreateMultiCurvePolygon(newSubGeometries);
                    }
                    break;
                case GeometryType.GeometryType_MultiGeometry:
                    {
                        var derivedGeom = (IMultiGeometry)geometry;
                        var newSubGeometries = new GeometryCollection();

                        int numSubGeometries = derivedGeom.Count;

                        for (int i = 0; i < numSubGeometries; i++)
                        {
                            var subGeom = derivedGeom[i];
                            var newSubGeometry = ConvertOrdinates(subGeom, applyTargetDimensionality, targetDimensionality);
                            newSubGeometries.Add((IGeometry)newSubGeometry);
                        }
                        newGeometry = _geomFactory.CreateMultiGeometry(newSubGeometries);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported geometry type: {geomType}");
            }

            return newGeometry;
        }

        public abstract void ConvertPosition(ref double x, ref double y);

        public abstract void ConvertPosition(ref double x, ref double y, ref double z);

        protected void ConvertOrdinates(int inputDim,
                                        int numPositions,
                                        /*const*/ double[] inputOrds,
                                        double padValueZ,
                                        double padValueM,
                                        int outputDim,
                                        ref double[] outputOrds)
        {
            int numInputOrds = numPositions * DimensionalityToNumOrdinates(inputDim);
            bool inputHasZ = (inputDim & FdoDimensionality_Z) != 0;
            bool inputHasM = (inputDim & FdoDimensionality_M) != 0;
            bool outputHasZ = (outputDim & FdoDimensionality_Z) != 0;
            bool outputHasM = (outputDim & FdoDimensionality_M) != 0;
            bool paddingZ = (outputHasZ && !inputHasZ);
            bool paddingM = (outputHasM && !inputHasM);

            double x, y;
            double z = padValueZ;
            double m = padValueM;

            // Assume that ordinate arrays are well-formed (no check for number of positions,
            // end of array at a position boundary, etc.).

            int ordIndex1 = 0;
            int ordIndex2 = 0;

            // Branch on input dimensionality so that we don't have to test for it for every position.
            // Performance could similarly be slightly improved by also branching
            // on output dimensionality (requires more cases).

            if (!inputHasZ && !inputHasM)
            {
                while (ordIndex1 < numInputOrds)
                {
                    x = inputOrds[ordIndex1++];
                    y = inputOrds[ordIndex1++];
                    ConvertPosition(ref x, ref y);
                    outputOrds[ordIndex2++] = x;
                    outputOrds[ordIndex2++] = y;
                    if (paddingZ)
                        outputOrds[ordIndex2++] = padValueZ;
                    if (paddingM)
                        outputOrds[ordIndex2++] = padValueM;
                }
            }
            else if (inputHasZ && !inputHasM)
            {
                while (ordIndex1 < numInputOrds)
                {
                    x = inputOrds[ordIndex1++];
                    y = inputOrds[ordIndex1++];
                    z = inputOrds[ordIndex1++];
                    ConvertPosition(ref x, ref y, ref z);
                    outputOrds[ordIndex2++] = x;
                    outputOrds[ordIndex2++] = y;
                    if (outputHasZ)
                        outputOrds[ordIndex2++] = z;
                    if (paddingM)
                        outputOrds[ordIndex2++] = padValueM;
                }
            }
            else if (!inputHasZ && inputHasM)
            {
                while (ordIndex1 < numInputOrds)
                {
                    x = inputOrds[ordIndex1++];
                    y = inputOrds[ordIndex1++];
                    m = inputOrds[ordIndex1++];
                    ConvertPosition(ref x, ref y);
                    outputOrds[ordIndex2++] = x;
                    outputOrds[ordIndex2++] = y;
                    if (paddingZ)
                        outputOrds[ordIndex2++] = padValueZ;
                    if (outputHasM)
                        outputOrds[ordIndex2++] = m;
                }
            }
            else // inputHasZ && inputHasM
            {
                while (ordIndex1 < numInputOrds)
                {
                    x = inputOrds[ordIndex1++];
                    y = inputOrds[ordIndex1++];
                    z = inputOrds[ordIndex1++];
                    m = inputOrds[ordIndex1++];
                    ConvertPosition(ref x, ref y, ref z);
                    outputOrds[ordIndex2++] = x;
                    outputOrds[ordIndex2++] = y;
                    if (outputHasZ)
                        outputOrds[ordIndex2++] = z;
                    if (outputHasM)
                        outputOrds[ordIndex2++] = m;
                }
            }
        }



        protected ILinearRing ConvertOrdinates(
            ILinearRing ring,
            int outputDim,
            double padValueZ,
            double padValueM)
        {
            int dimensionality = ring.Dimensionality;

            var numPositions = ring.Count;
            var numOrdinates = numPositions * DimensionalityToNumOrdinates(outputDim);
            var newOrdinates = _pool.Rent(numOrdinates);
            var pos = ring.Positions;
            var ordinates = GetOrdinates(pos, outputDim);
            ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref newOrdinates);
            var newRing = _geomFactory.CreateLinearRing(outputDim, numOrdinates, newOrdinates);
            _pool.Return(newOrdinates);

            return newRing;
        }

        protected CurveSegmentCollection ConvertOrdinates(
            CurveSegmentCollection csc,
            int outputDim,
            double padValueZ,
            double padValueM)
        {
            var newCsc = new CurveSegmentCollection();

            int count = csc.Count;
            for (int i = 0; i < count; i++)
            {
                var cs = csc[i];
                var newCs = ConvertOrdinates(cs, outputDim, padValueZ, padValueM);
                newCsc.Add(newCs);
            }

            return newCsc;
        }

        protected ICurveSegmentAbstract ConvertOrdinates(
            ICurveSegmentAbstract cs,
            int outputDim,
            double padValueZ,
            double padValueM)
        {
            ICurveSegmentAbstract newCs = null;

            int dimensionality = cs.Dimensionality;
            bool paddingZ = ((outputDim & FdoDimensionality_Z) != 0 && (dimensionality & FdoDimensionality_Z) == 0);
            bool paddingM = ((outputDim & FdoDimensionality_M) != 0 && (dimensionality & FdoDimensionality_M) == 0);

            var gct = cs.DerivedType;

            switch (gct)
            {
                case GeometryComponentType.GeometryComponentType_LineStringSegment:
                    {
                        var ls = (ILineStringSegment)((ICurveSegmentAbstract)cs);
                        var numPositions = ls.Count;
                        var numOrdinates = numPositions * DimensionalityToNumOrdinates(outputDim);
                        var newOrdinates = _pool.Rent(numOrdinates);
                        var pos = ls.Positions;
                        var ordinates = GetOrdinates(pos, outputDim);
                        ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref newOrdinates);
                        newCs = _geomFactory.CreateLineStringSegment(outputDim, numOrdinates, newOrdinates);
                        _pool.Return(newOrdinates);
                    }
                    break;
                case GeometryComponentType.GeometryComponentType_CircularArcSegment:
                    {
                        var arc = (ICircularArcSegment)((ICurveSegmentAbstract)cs);
                        var start = arc.StartPosition;
                        var mid = arc.MidPoint;
                        var end = arc.EndPosition;
                        var numPositions = 1;
                        var numOrdinates = DimensionalityToNumOrdinates(outputDim);
                        double[] ords = _pool.Rent(4);

                        var ordinates = GetOrdinates(start);
                        ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref ords);
                        var newStart = CreatePosition(outputDim, ords);
                        _pool.Return(ordinates);

                        ordinates = GetOrdinates(mid);
                        ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref ords);
                        var newMid = CreatePosition(outputDim, ords);
                        _pool.Return(ordinates);

                        ordinates = GetOrdinates(end);
                        ConvertOrdinates(dimensionality, numPositions, ordinates, padValueZ, padValueM, outputDim, ref ords);
                        var newEnd = CreatePosition(outputDim, ords);
                        _pool.Return(ordinates);

                        newCs = _geomFactory.CreateCircularArcSegment(newStart, newMid, newEnd);

                        _pool.Return(ords);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported geometry component type: {gct}");
            }

            return newCs;
        }

        private double[] GetOrdinates(IPoint derivedGeom) => GetOrdinates(derivedGeom.Position);

        private double[] GetOrdinates(IMultiPoint derivedGeom)
        {
            var ptCount = derivedGeom.Count;
            var numOrds = DimensionalityToNumOrdinates(derivedGeom.Dimensionality);
            var size = ptCount * numOrds;
            var ordinates = _pool.Rent(size);
            Span<double> spOrdinates = ordinates;
            
            for (int i = 0; i < ptCount; i++)
            {
                var slice = spOrdinates.Slice(i, numOrds);
                var pt = derivedGeom[i];
                var pos = pt.Position;

                FillOrdinates(slice, pos);
            }

            return ordinates;
        }

        private double[] GetOrdinates(IDirectPosition pos)
        {
            var size = DimensionalityToNumOrdinates(pos.Dimensionality);
            var ordinates = _pool.Rent(size);
            FillOrdinates(ordinates, pos);
            return ordinates;
        }

        private double[] GetOrdinates(DirectPositionCollection positions, int dim)
        {
            var numPos = positions.Count;
            var numOrds = DimensionalityToNumOrdinates(dim);
            var size = numPos * numOrds;
            var ordinates = _pool.Rent(size);
            Span<double> spOrdinates = ordinates;

            for (int i = 0; i < numPos; i++)
            {
                var slice = spOrdinates.Slice(i, numOrds);
                var pos = positions[i];
                FillOrdinates(slice, pos);
            }

            return ordinates;
        }

        /*
         
        Array format for easy reference:

                [ x, y, z|m, m ]

            XY = [x, y]
            XYZ = [x, y, z]
            XYM = [x, y, m]
            XYZM = [x, y, z, m]

         */

        private IDirectPosition CreatePosition(int dimensionality, double[] ordinates)
        {
            bool hasZ = (dimensionality & FdoDimensionality_Z) != 0;
            bool hasM = (dimensionality & FdoDimensionality_M) != 0;
            double x = ordinates[0];
            double y = ordinates[1];
            if (hasZ)
            {
                double z = ordinates[2];
                if (hasM) //XYZM
                {
                    double m = ordinates[3];
                    return _geomFactory.CreatePositionXYZM(x, y, z, m);
                }
                else //XYZ
                {
                    return _geomFactory.CreatePositionXYZ(x, y, z);
                }
            }
            else if (hasM) //XYM
            {
                double m = ordinates[2];
                return _geomFactory.CreatePositionXYM(x, y, m);
            }
            else //XY
            {
                return _geomFactory.CreatePositionXY(x, y);
            }
        }

        static void FillOrdinates(Span<double> ordinates, IDirectPosition pos)
        {
            var dimensionality = pos.Dimensionality;
            bool hasZ = (dimensionality & FdoDimensionality_Z) != 0;
            bool hasM = (dimensionality & FdoDimensionality_M) != 0;
            ordinates[0] = pos.X;
            ordinates[1] = pos.Y;

            if (hasZ)
            {
                ordinates[2] = pos.Z;
                if (hasM) //XYZM
                {
                    ordinates[3] = pos.M;
                }
            }
            else if (hasM) //XYM
            {
                ordinates[2] = pos.M;
            }
        }

        static int[] _ordinateCountTable = { 2, 3, 3, 4 };

        static int DimensionalityToNumOrdinates(int dimensionality) => _ordinateCountTable[dimensionality];

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _geomFactory.Dispose();
                    _geomFactory = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FdoGeometryConverter()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
