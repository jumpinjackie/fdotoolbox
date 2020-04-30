#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using FdoToolbox.Base;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.DataStore;
using System.Data.Common;
using OSGeo.FDO.Commands.SpatialContext;
using System.IO;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Express.Controls
{
    public interface ICreateRdbmsView : IViewContent
    {
        string Service { get; }
        string Username { get; }
        string Password { get; }

        bool IsFdoMetadataOptional { get; }

        bool ExtentsEnabled { set; }
        bool SubmitEnabled { set; }
        bool FdoMetadataEnabled { set; }
        bool WKTEnabled { set; }

        string DataStoreName { get; }
        string SchemaFile { get; }

        SpatialContextInfo CreateDefaultSpatialContext();

        SpatialContextExtentType ExtentType { get; }
        Array AvailableExtentTypes { set; }

        bool UseFdoMetadata { get; }
        bool ConnectOnCreate { get; }
        bool FixSchema { get; }

        string CSName { get; }
        string CSWkt { get; }
        double Tolerance { get; }

        string ConnectionName { get; }

        string Provider { get; }

        string ServiceParameter { get; }
        string UsernameParameter { get; }
        string PasswordParameter { get; }
        string DataStoreParameter { get; }
        
        string FdoEnabledParameter { get; }

        double LowerLeftX { get; }
        double LowerLeftY { get; }
        double UpperRightX { get; }
        double UpperRightY { get; }
    }

    internal class CreateRdbmsPresenter
    {
        private readonly ICreateRdbmsView _view;
        private readonly IFdoConnectionManager _connMgr;

        public CreateRdbmsPresenter(ICreateRdbmsView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _connMgr = connMgr;
        }

        public bool RequiresWKT
        {
            get;
            private set;
        }

        public void Init()
        {
            var conn = new FdoConnection(_view.Provider);
            _view.AvailableExtentTypes = conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_SpatialContextTypes);
            _view.WKTEnabled = this.RequiresWKT = conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName);
        }

        public bool Create()
        {
            bool ok = true;
            FdoConnection conn = new FdoConnection(_view.Provider, GetBaseConnectionString());
            bool created = false;
            bool cleanup = true;
            try
            {
                using (var svc = conn.CreateFeatureService())
                {
                    try
                    {
                        CreateDataStore(svc);
                        created = true;
                    }
                    catch (Exception ex)
                    {
                        _view.ShowError(ex);
                        created = false;
                        ok = false;
                    }
                }

                if (created)
                {
                    //Amend the connection string to include the data store
                    var builder = new DbConnectionStringBuilder
                    {
                        ConnectionString = conn.ConnectionString
                    };
                    builder[_view.DataStoreParameter] = _view.DataStoreName;
                    conn.ConnectionString = builder.ConnectionString;
                    conn.Open();

                    using (var svc = conn.CreateFeatureService())
                    {
                        CreateDefaultSpatialContext(svc);
                    }
                }
            }
            finally
            {
                if (created)
                {
                    if (File.Exists(_view.SchemaFile))
                    {
                        ApplySchemas(conn);
                    }

                    if (_view.ConnectOnCreate)
                    {
                        _connMgr.AddConnection(_view.ConnectionName, conn);
                        cleanup = false;
                    }
                }

                if (cleanup)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            
            return ok;
        }

        private void CreateDataStore(FdoFeatureService svc)
        {
            using (var cmd = svc.CreateCommand<ICreateDataStore>(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore))
            {
                var props = cmd.DataStoreProperties;
                props.SetProperty(_view.DataStoreParameter, _view.DataStoreName);

                if (_view.IsFdoMetadataOptional)
                    props.SetProperty(_view.FdoEnabledParameter, _view.UseFdoMetadata.ToString().ToLower());

                cmd.Execute();
            }
        }

        private void CreateDefaultSpatialContext(FdoFeatureService svc)
        {
            var sc = _view.CreateDefaultSpatialContext();
            
            //You would've thought ICreateSpatialContext with updateExisting = true
            //would do the job, but noooo we have to actually destroy the existing
            //one first. Annoying!
            bool destroy = false;
            var spcs = svc.GetSpatialContexts();
            foreach (var spc in spcs)
            {
                if (spc.Name == sc.Name)
                {
                    destroy = true;
                    break;
                }
            }
            if (destroy)
                svc.DestroySpatialContext(sc.Name);

            svc.CreateSpatialContext(sc, false);
        }

        private void ApplySchemas(FdoConnection conn)
        {
            using (var svc = conn.CreateFeatureService())
            {
                using (FeatureSchemaCollection fsc = new FeatureSchemaCollection(null))
                {
                    fsc.ReadXml(_view.SchemaFile);
                    int modified = FdoSchemaUtil.SetDefaultSpatialContextAssociation(fsc, "Default");

                    List<FeatureSchema> schemas = new List<FeatureSchema>();
                    if (_view.FixSchema)
                    {
                        foreach (FeatureSchema fs in fsc)
                        {
                            IncompatibleSchema inc;
                            if (!svc.CanApplySchema(fs, out inc))
                            {
                                FeatureSchema mod = svc.AlterSchema(fs, inc);
                                schemas.Add(mod);
                            }
                            else
                            {
                                schemas.Add(fs);
                            }
                        }
                    }
                    else
                    {
                        foreach (FeatureSchema fs in fsc)
                        {
                            schemas.Add(fs);
                        }
                    }

                    foreach (FeatureSchema fs in schemas)
                    {
                        svc.ApplySchema(fs);
                    }
                }
            }
        }

        private string GetBaseConnectionString()
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder[_view.ServiceParameter] = _view.Service;
            builder[_view.UsernameParameter] = _view.Username;
            builder[_view.PasswordParameter] = _view.Password;
            return builder.ToString();
        }

        public bool CheckConnectionName()
        {
            return !_connMgr.NameExists(_view.ConnectionName);
        }

        internal bool Test()
        {
            using (var conn = new FdoConnection(_view.Provider, GetBaseConnectionString()))
            {
                if (conn.Open() != FdoConnectionState.Closed)
                {
                    conn.Close();
                    return true;
                }
            }
            return false;
        }
    }
}
