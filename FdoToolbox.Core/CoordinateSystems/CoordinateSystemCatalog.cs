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
using System.Collections.Generic;
using System.Linq;

namespace FdoToolbox.Core.CoordinateSystems
{
    /// <summary>
    /// The default implementation of <see cref="ICoordinateSystemCatalog"/>
    /// </summary>
    public class CoordinateSystemCatalog : ICoordinateSystemCatalog
    {
        private string m_coordLib = null;
        internal MgCoordinateSystemFactory _csFactory;
        private bool disposedValue;

        public CoordinateSystemCatalog()
        {
            _csFactory = new MgCoordinateSystemFactory();
            _categories = new Lazy<ICoordinateSystemCategory[]>(() => 
            {
                var c = _csFactory.EnumerateCategories();
                CoordinateSystemCategory[] data = null;
                try
                {
                    data = new CoordinateSystemCategory[c.GetCount()];
                    for (int i = 0; i < c.GetCount(); i++)
                    {
                        data[i] = new CoordinateSystemCategory(this, c.GetItem(i));
                    }
                }
                finally
                {
                    c.Dispose();
                }
                return data;
            });
        }

        readonly Lazy<ICoordinateSystemCategory[]> _categories;

        public ICoordinateSystemCategory[] Categories => _categories.Value;

        public string ConvertCoordinateSystemCodeToWkt(string coordcode) => _csFactory.ConvertCoordinateSystemCodeToWkt(coordcode);
        
        public string ConvertEpsgCodeToWkt(string epsg) => _csFactory.ConvertEpsgCodeToWkt(int.Parse(epsg));
        
        public string ConvertWktToCoordinateSystemCode(string wkt) => _csFactory.ConvertWktToCoordinateSystemCode(wkt);
        
        public string ConvertWktToEpsgCode(string wkt) => _csFactory.ConvertWktToEpsgCode(wkt).ToString();
        
        public bool IsValid(string wkt) => _csFactory.IsValid(wkt);

        public string LibraryName
        {
            get
            {
                if (m_coordLib == null)
                    m_coordLib = _csFactory.GetBaseLibrary();
                return m_coordLib;
            }
        }

        public bool IsLoaded => _categories.IsValueCreated;

        public IEnumerable<ICoordinateSystem> EnumerateCoordinateSystems(string category)
        {
            var cat = this.Categories.FirstOrDefault(csc => csc.Name == category);
            if (cat != null)
            {
                var bp = _csFactory.EnumerateCoordinateSystems(category);
                try
                {
                    for (int i = 0; i < bp.GetCount(); i++)
                    {
                        var props = bp.GetItem(i);
                        try
                        {
                            yield return new CoordinateSystemDefinition(this, props);
                        }
                        finally
                        {
                            props.Dispose();
                        }
                    }
                }
                finally
                {
                    bp.Dispose();
                }
            }
        }

        public ICoordinateSystem CreateFromWkt(string wkt)
        {
            try
            {
                var cs = _csFactory.Create(wkt);
                return new CoordinateSystemDefinition(this, cs);
            }
            catch
            {
                return null;
            }
        }

        public ICoordinateSystem CreateEmptyCoordinateSystem() => new CoordinateSystemDefinition();

        public ICoordinateSystem CreateArbitraryCoordinateSystem(string wkt) => new CoordinateSystemDefinition { WKT = wkt };

        public IEnumerable<ICoordinateSystem> AllCoordinateSystems => Categories.SelectMany(cat => cat.Items);

        public ICoordinateSystem FindCoordinateSystemByCode(string coordcode)
        {
            if (!string.IsNullOrEmpty(coordcode))
            {
                var cs = _csFactory.CreateFromCode(coordcode);
                if (cs != null)
                {
                    try
                    {
                        return new CoordinateSystemDefinition(this, cs);
                    }
                    finally
                    {
                        cs.Dispose();
                    }
                }
            }
            return null;
        }

        public ICoordinateSystem FindCoordinateSystemByEpsgCode(string epsg)
        {
            if (!string.IsNullOrEmpty(epsg) && int.TryParse(epsg, out var code))
            {
                var wkt = _csFactory.ConvertEpsgCodeToWkt(code);
                if (!string.IsNullOrEmpty(wkt))
                {
                    var cs = _csFactory.Create(wkt);
                    if (cs != null)
                    {
                        try
                        {
                            return new CoordinateSystemDefinition(this, cs);
                        }
                        finally
                        {
                            cs.Dispose();
                        }
                    }
                }
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _csFactory.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CoordinateSystemCatalog()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
