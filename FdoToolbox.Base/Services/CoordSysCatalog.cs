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
using FdoToolbox.Core.CoordinateSystems;
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Expression;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// A simple data access object to the Coordinate System Catalog which resides
    /// in a SQLite database.
    /// </summary>
    public class CoordSysCatalog : ICoordinateSystemCatalog
    {
        const string DB_FILE = "cscatalog.sqlite";
        private string dbpath;

        private string _ConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordSysCatalog"/> class.
        /// </summary>
        public CoordSysCatalog()
        {
            dbpath = Path.Combine(FileUtility.ApplicationRootPath, DB_FILE);
            _ConnectionString = string.Format("File={0}", dbpath);
        }

        private BindingList<CoordinateSystemDefinition> _Projections;

        private IConnection CreateSqliteConnection()
        {
            var conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SQLite");
            conn.ConnectionString = _ConnectionString;
            return conn;
        }

        /// <summary>
        /// Adds a new coordinate system to the database
        /// </summary>
        /// <param name="cs"></param>
        public void AddProjection(CoordinateSystemDefinition cs)
        {
            using (var conn = CreateSqliteConnection())
            {
                conn.Open();
                using (IInsert cmd = (IInsert)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert))
                {
                    cmd.SetFeatureClassName("Projections");
                    cmd.PropertyValues.Add(new OSGeo.FDO.Commands.PropertyValue("Name", new StringValue(cs.Name)));
                    cmd.PropertyValues.Add(new OSGeo.FDO.Commands.PropertyValue("Description", new StringValue(cs.Description)));
                    cmd.PropertyValues.Add(new OSGeo.FDO.Commands.PropertyValue("WKT", new StringValue(cs.Wkt)));

                    int affected = 0;
                    using(var reader = cmd.Execute())
                    {
                        while(reader.ReadNext())
                        {
                            affected++;
                        }
                        reader.Close();
                    }

                    if (affected == 1)
                    {
                        _Projections.Add(cs);
                        LoggingService.InfoFormatted("Coordinate System {0} added to database", cs.Name);
                    }
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Updates an existing coordinate system in the database
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="oldName"></param>
        /// <returns></returns>
        public bool UpdateProjection(CoordinateSystemDefinition cs, string oldName)
        {
            using (var conn = CreateSqliteConnection())
            {
                conn.Open();
                using(IUpdate update = (IUpdate)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Update))
                {
                    update.SetFeatureClassName("Projections");
                    update.PropertyValues.Add(new OSGeo.FDO.Commands.PropertyValue("Name", new StringValue(cs.Name)));
                    update.PropertyValues.Add(new OSGeo.FDO.Commands.PropertyValue("Description", new StringValue(cs.Description)));
                    update.PropertyValues.Add(new OSGeo.FDO.Commands.PropertyValue("WKT", new StringValue(cs.Wkt)));

                    update.SetFilter("Name = '" + oldName + "'");

                    if (update.Execute() == 1)
                    {
                        LoggingService.InfoFormatted("Coordinate System {0} updated in database", oldName);
                        return true;
                    }
                }
                conn.Close();
            }
            return false;
        }

        /// <summary>
        /// Deletes a coordinate system from the database
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public bool DeleteProjection(CoordinateSystemDefinition cs)
        {
            using (var conn = CreateSqliteConnection())
            {
                conn.Open();
                using (IDelete delete = (IDelete)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Delete))
                {
                    delete.SetFeatureClassName("Projections");
                    delete.SetFilter("Name = '" + cs.Name + "'");
                    if (delete.Execute() == 1)
                    {
                        LoggingService.InfoFormatted("Coordinate System {0} deleted from database", cs.Name);
                        _Projections.Remove(cs);
                        return true;
                    }
                }
                conn.Close();
            }
            return false;
        }

        /// <summary>
        /// Gets all the coordinate systems in the database
        /// </summary>
        /// <returns></returns>
        public BindingList<CoordinateSystemDefinition> GetAllProjections()
        {
            if (_Projections != null)
                return _Projections;

            _Projections = new BindingList<CoordinateSystemDefinition>();
            using (var conn = CreateSqliteConnection())
            {
                LoggingService.InfoFormatted("Loading all Coordinate Systems from {0}", dbpath);
                conn.Open();
                string name = string.Empty;
                string desc = string.Empty;
                string wkt = string.Empty;
               
                using (ISelect select = (ISelect)conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select))
                {
                    select.SetFeatureClassName("Projections");
                    using (var reader = select.Execute())
                    {
                        while (reader.ReadNext())
                        {
                            name = reader.GetString("Name");
                            desc = reader.GetString("Description");
                            wkt = reader.GetString("WKT");
                            _Projections.Add(new CoordinateSystemDefinition(name, desc, wkt));
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }

            return _Projections;
        }

        /// <summary>
        /// Checks if a coordinate system exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ProjectionExists(string name)
        {
            if (_Projections == null)
                GetAllProjections();

            foreach (CoordinateSystemDefinition cs in _Projections)
            {
                if (cs.Name == name)
                    return true;
            }
            return false;
        }

        private bool _init = false;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return _init; }
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        public void InitializeService()
        {
            LoggingService.Info("Initialized Coordinate System Catalog Service");
            _init = true;
            Initialize(this, EventArgs.Empty);
        }

        /// <summary>
        /// Unloads the service.
        /// </summary>
        public void UnloadService()
        {
            Unload(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the service is initialized
        /// </summary>
        public event EventHandler Initialize = delegate { };

        /// <summary>
        /// Occurs when the service is unloaded
        /// </summary>
        public event EventHandler Unload = delegate { };


        /// <summary>
        /// Loads any persisted objects from the session directory
        /// </summary>
        public void Load()
        {
            
        }

        /// <summary>
        /// Persists any managed objects to the session directory
        /// </summary>
        public void Save()
        {
            
        }
    }
}
