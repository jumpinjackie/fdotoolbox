#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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

using OSGeo.MapGuide;
using System;

namespace FdoToolbox.Core.CoordinateSystems
{
    /// <summary>
    /// Data transfer object for Coordinate Systems
    /// </summary>
    public class CoordinateSystemDefinition : ICoordinateSystem
    {
        /// <summary>
        /// The parent category
        /// </summary>
        protected CoordinateSystemCategory m_parent;

        /// <summary>
        /// The cs code
        /// </summary>
        protected string m_code;

        /// <summary>
        /// The description
        /// </summary>
        protected string m_description;

        /// <summary>
        /// The projection
        /// </summary>
        protected string m_projection;

        /// <summary>
        /// The projection description
        /// </summary>
        protected string m_projectionDescription;

        /// <summary>
        /// The datum
        /// </summary>
        protected string m_datum;

        /// <summary>
        /// The datum description
        /// </summary>
        protected string m_datumDescription;

        /// <summary>
        /// The ellipsoid
        /// </summary>
        protected string m_ellipsoid;

        /// <summary>
        /// The ellipsoid description
        /// </summary>
        protected string m_ellipsoidDescription;

        /// <summary>
        /// The cs wkt
        /// </summary>
        protected string m_wkt = null;

        /// <summary>
        /// The epsg code
        /// </summary>
        protected string m_epsg = null;

        readonly ICoordinateSystemCatalog _catalog;

        readonly Lazy<CoordinateSystemBounds> _bounds;

        internal CoordinateSystemDefinition() { }

        internal CoordinateSystemDefinition(ICoordinateSystemCatalog catalog, MgCoordinateSystem cs)
        {
            _catalog = catalog;
            m_code = cs.CsCode;
            m_description = cs.Description;
            m_projection = cs.Projection;
            m_projectionDescription = cs.ProjectionDescription;
            m_datum = cs.Datum;
            m_datumDescription = cs.DatumDescription;
            m_ellipsoid = cs.Ellipsoid;
            m_ellipsoidDescription = cs.EllipsoidDescription;

            m_epsg = cs.GetEpsgCode().ToString();
            var b = new CoordinateSystemBounds
            {
                MinX = cs.GetMinX(),
                MinY = cs.GetMinY(),
                MaxX = cs.GetMaxX(),
                MaxY = cs.GetMaxY()
            };
            _bounds = new Lazy<CoordinateSystemBounds>(() => b);
        }

        public CoordinateSystemBounds Bounds => _bounds.Value;

        internal CoordinateSystemDefinition(ICoordinateSystemCatalog catalog, MgPropertyCollection props)
        {
            _catalog = catalog;
            int pcount = props.GetCount();
            for (int i = 0; i < pcount; i++)
            {
                var prop = props.GetItem(i);
                switch (prop.Name.ToLower())
                {
                    case "code":
                        m_code = (prop as MgStringProperty).Value;
                        break;

                    case "description":
                        m_description = (prop as MgStringProperty).Value;
                        break;

                    case "projection":
                        m_projection = (prop as MgStringProperty).Value;
                        break;

                    case "projection description":
                        m_projectionDescription = (prop as MgStringProperty).Value;
                        break;

                    case "datum":
                        m_datum = (prop as MgStringProperty).Value;
                        break;

                    case "datum description":
                        m_datumDescription = (prop as MgStringProperty).Value;
                        break;

                    case "ellipsoid":
                        m_ellipsoid = (prop as MgStringProperty).Value;
                        break;

                    case "ellipsoid description":
                        m_ellipsoidDescription = (prop as MgStringProperty).Value;
                        break;

                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty (m_code))
            {
                try
                {
                    //Fetching the coord sys by code will call the ctor with MgCoordinateSystem, which
                    //we can fetch the bounds from
                    var cs = _catalog.FindCoordinateSystemByCode(m_code);
                    _bounds = new Lazy<CoordinateSystemBounds>(() => cs.Bounds);
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code
        {
            get { return m_code; }
            set { m_code = value; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public string Projection => m_projection;

        /// <summary>
        /// Gets the projection description.
        /// </summary>
        /// <value>The projection description.</value>
        public string ProjectionDescription => m_projectionDescription;

        /// <summary>
        /// Gets the datum.
        /// </summary>
        /// <value>The datum.</value>
        public string Datum => m_datum;

        /// <summary>
        /// Gets the datum description.
        /// </summary>
        /// <value>The datum description.</value>
        public string DatumDescription => m_datumDescription;

        /// <summary>
        /// Gets the ellipsoid.
        /// </summary>
        /// <value>The ellipsoid.</value>
        public string Ellipsoid => m_ellipsoid;

        /// <summary>
        /// Gets the ellipsoid description.
        /// </summary>
        /// <value>The ellipsoid description.</value>
        public string EllipsoidDescription => m_ellipsoidDescription;

        /// <summary>
        /// Gets or sets the WKT.
        /// </summary>
        /// <value>The WKT.</value>
        public string WKT
        {
            get
            {
                if (m_wkt == null)
                    m_wkt = _catalog?.ConvertCoordinateSystemCodeToWkt(m_code);
                return m_wkt;
            }
            set
            {
                m_wkt = value;
            }
        }

        /// <summary>
        /// Gets the EPSG code
        /// </summary>
        /// <value>The EPSG code.</value>
        public string EPSG
        {
            get
            {
                if (m_epsg == null)
                {
                    if (m_code.StartsWith("EPSG:")) //NOXLATE
                    {
                        m_epsg = m_code.Substring(5);
                    }
                    else if (_catalog != null)
                    {
                        var wkt = _catalog.ConvertCoordinateSystemCodeToWkt(m_code);
                        if (!string.IsNullOrEmpty(wkt))
                        {
                            m_epsg = _catalog.ConvertWktToEpsgCode(wkt);
                        }
                        else
                        {
                            m_epsg = string.Empty;
                        }
                    }
                }
                return m_epsg;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (m_description == null && m_code == null)
                return m_wkt ?? "<null>"; //NOXLATE
            else if (m_description == null)
                return m_code;
            else if (m_code == null)
                return m_description;
            else
                return $"{m_description} ({m_code})"; //NOXLATE
        }
    }
}
