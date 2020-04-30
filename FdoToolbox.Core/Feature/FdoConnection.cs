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
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using Res = FdoToolbox.Core.ResourceUtil;
using FdoToolbox.Core.Connections;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Commands.Schema;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO Connection wrapper class
    /// </summary>
    public class FdoConnection : IDisposable
    {
        private ICapability _caps;
        
        /// <summary>
        /// Gets the capability object for this connection
        /// </summary>
        public ICapability Capability
        {
            get
            {
                if (_caps == null)
                    _caps = new Capability(this);
                return _caps;
            }
        }

        /// <summary>
        /// Gets the type of the data store.
        /// </summary>
        /// <value>The type of the data store.</value>
        public ProviderDatastoreType DataStoreType => InternalConnection.ConnectionInfo.ProviderDatastoreType;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnection"/> class.
        /// </summary>
        /// <param name="provider">The provider name.</param>
        public FdoConnection(string provider)
        {
            this.HasConfiguration = false;
            this.InternalConnection = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnection"/> class.
        /// </summary>
        /// <param name="provider">The provider name.</param>
        /// <param name="connectionString">The connection string.</param>
        public FdoConnection(string provider, string connectionString)
            : this(provider)
        {
            this.ConnectionString = connectionString;
        }

        public void SaveConfiguration(string file)
        {
            if (!this.HasConfiguration)
                throw new InvalidOperationException("This connection has no configuration");

            File.WriteAllText(file, _configXml);
        }

        /// <summary>
        /// Saves this connection to a file
        /// </summary>
        /// <param name="file"></param>
        public void Save(string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FdoToolbox.Core.Configuration.Connection));
            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8))
            {
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;

                FdoToolbox.Core.Configuration.Connection conn = new FdoToolbox.Core.Configuration.Connection
                {
                    Provider = this.Provider,
                    ConnectionString = this.ConnectionString
                };

                serializer.Serialize(writer, conn);
            }

            if (HasConfiguration && !string.IsNullOrEmpty(_configXml))
            {
                string dir = Path.GetDirectoryName(file);
                string baseName = Path.GetFileNameWithoutExtension(file);

                string output = Path.Combine(dir, baseName + "_Configuration.xml");

                SaveConfiguration(output);
            }
        }

        /// <summary>
        /// Creates an FDO connection from file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FdoConnection LoadFromFile(string file)
        {
            FdoToolbox.Core.Configuration.Connection c = null;
            XmlSerializer serializer = new XmlSerializer(typeof(FdoToolbox.Core.Configuration.Connection));
            using (StreamReader reader = new StreamReader(file))
            {
                c = (FdoToolbox.Core.Configuration.Connection)serializer.Deserialize(reader);
            }
            return new FdoConnection(c.Provider, c.ConnectionString);
        }

        /// <summary>
        /// Creates an FDO connection from file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tryLoadConfiguration">If true will attempt to load the configuration for this connection. The configuration file is assumed to be the same name as the connection file, except it has a _Configuration suffix and has a .xml extension</param>
        /// <returns></returns>
        public static FdoConnection LoadFromFile(string file, bool tryLoadConfiguration)
        {
            var conn = LoadFromFile(file);

            if (tryLoadConfiguration)
            {
                string dir = Path.GetDirectoryName(file);
                string baseName = Path.GetFileNameWithoutExtension(file);
                string conf = Path.Combine(dir, baseName + "_Configuration.xml");

                if (File.Exists(conf))
                    conn.SetConfiguration(conf);
            }

            return conn;
        }

        private string _connStr;

        /// <summary>
        /// Gets or sets the connection string of the underlying connection
        /// </summary>
        public string ConnectionString
        {
            get { return _connStr ?? this.InternalConnection.ConnectionString; }
            set 
            { 
                this.InternalConnection.ConnectionString = _connStr = value;
                if (!string.IsNullOrEmpty(value))
                {
                    //HACK: ODBC doesn't want to play nice
                    if (this.Provider.StartsWith("OSGeo.ODBC") || this.Provider.StartsWith("OSGeo.SQLServerSpatial"))
                    {
                        SafeConnectionString = value;
                        return;
                    }

                    List<string> safeParams = new List<string>();
                    string[] parameters = this.ConnectionString.Split(';');
                    IConnectionPropertyDictionary dict = this.InternalConnection.ConnectionInfo.ConnectionProperties;
                    foreach (string p in parameters)
                    {
                        string[] tokens = p.Split('=');
                        
                        if (!dict.IsPropertyProtected(tokens[0]))
                        {
                            safeParams.Add(p);
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < tokens[1].Length; i++)
                            {
                                sb.Append("*");
                            }
                            safeParams.Add(tokens[0] + "=" + sb.ToString());
                        }
                    }
                    SafeConnectionString = string.Join(";", safeParams.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets the connection string with the protected elements obfuscated
        /// </summary>
        public string SafeConnectionString { get; private set; } = null;

        private string _name;

        /// <summary>
        /// The name of the connection's underlying provider. This does not include the version number. Use the <see cref="ProviderQualified"/>
        /// property for the full provider name
        /// </summary>
        public string Provider
        {
            get 
            {
                if (_name == null)
                {
                    ProviderNameTokens providerName = new ProviderNameTokens(this.InternalConnection.ConnectionInfo.ProviderName);
                    string [] tokens = providerName.GetNameTokens();
                    _name = tokens[0] + "." + tokens[1];
                }
                return _name;
            }
        }

        /// <summary>
        /// The fully-qualified name of the connection's underlying provider
        /// </summary>
        public string ProviderQualified => this.InternalConnection.ConnectionInfo.ProviderName;

        /// <summary>
        /// Refreshes this connection
        /// </summary>
        public void Refresh()
        {
            Close();
            this.InternalConnection.Open();
        }

        /// <summary>
        /// The underlying FDO connection
        /// </summary>
        internal IConnection InternalConnection { get; set; }

        /// <summary>
        /// Creates a new feature service bound to this connection
        /// </summary>
        /// <returns></returns>
        public FdoFeatureService CreateFeatureService()
        {
            if (this.State == FdoConnectionState.Closed)
                this.Open();

            return new FdoFeatureService(this.InternalConnection);
        }

        /// <summary>
        /// Opens the underlying connection
        /// </summary>
        public FdoConnectionState Open()
        {
            try
            {
                if (this.InternalConnection.ConnectionState != ConnectionState.ConnectionState_Open)
                    this.InternalConnection.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex) { throw new FdoException(ex); }
            return this.State;
        }

        /// <summary>
        /// Closes the underlying connection
        /// </summary>
        public void Close()
        {
            if (this.InternalConnection.ConnectionState != ConnectionState.ConnectionState_Closed)
                this.InternalConnection.Close();
        }

        /// <summary>
        /// Disposes this connection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the current connection state
        /// </summary>
        public FdoConnectionState State
        {
            get
            {
                switch (this.InternalConnection.ConnectionState)
                {
                    case ConnectionState.ConnectionState_Busy:
                        return FdoConnectionState.Busy;
                    case ConnectionState.ConnectionState_Closed:
                        return FdoConnectionState.Closed;
                    case ConnectionState.ConnectionState_Open:
                        return FdoConnectionState.Open;
                    case ConnectionState.ConnectionState_Pending:
                        return FdoConnectionState.Pending;
                }
                throw new InvalidOperationException(Res.GetString("ERR_CONNECTION_UNKNOWN_STATE"));
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(_caps != null)
                    _caps.Dispose();
                this.Close();
                this.InternalConnection.Dispose();
            }
        }

        /// <summary>
        /// Gets the connect time property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public DictionaryProperty GetConnectTimeProperty(string name)
        {
            if (this.State != FdoConnectionState.Open && this.State != FdoConnectionState.Pending)
                throw new InvalidOperationException(Res.GetString("ERR_CONNECTION_NOT_OPEN"));

            IConnectionPropertyDictionary dict = this.InternalConnection.ConnectionInfo.ConnectionProperties;
            bool enumerable = dict.IsPropertyEnumerable(name);
            DictionaryProperty dp = null;
            if (enumerable)
            {
                EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty
                {
                    Values = dict.EnumeratePropertyValues(name)
                };
                dp = ep;
            }
            else
            {
                dp = new DictionaryProperty();
            }

            dp.Name = name;
            dp.LocalizedName = dict.GetLocalizedName(name);
            dp.DefaultValue = dict.GetPropertyDefault(name);
            dp.Protected = dict.IsPropertyProtected(name);
            dp.Required = dict.IsPropertyRequired(name);

            return dp;
        }

        private string _configXml;

        /// <summary>
        /// Sets the configuration for this connection
        /// </summary>
        /// <param name="file">The configuration file</param>
        public void SetConfiguration(string file)
        {
            if (this.State != FdoConnectionState.Closed && this.State != FdoConnectionState.Pending)
                throw new InvalidOperationException("Cannot set configuration when connection is not in a closed or pending state");

            CapabilityType cap = CapabilityType.FdoCapabilityType_SupportsConfiguration;
            if (!this.Capability.GetBooleanCapability(cap))
                throw new InvalidOperationException(ResourceUtil.GetStringFormatted("ERR_UNSUPPORTED_CAPABILITY", cap));
            IoFileStream confStream = new IoFileStream(file, "r");
            InternalConnection.Configuration = confStream;

            this.HasConfiguration = true;
            _configXml = File.ReadAllText(file);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has configuration.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has configuration; otherwise, <c>false</c>.
        /// </value>
        public bool HasConfiguration
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a blank physical schema mapping
        /// </summary>
        /// <returns></returns>
        public PhysicalSchemaMapping CreateSchemaMapping()
        {
            return InternalConnection.CreateSchemaMapping();
        }

        /// <summary>
        /// Forces the writes of any cached data to the targed datastore
        /// </summary>
        public void Flush()
        {
            InternalConnection.Flush();
        }
    }

    /// <summary>
    /// Indicates the current connection state
    /// </summary>
    public enum FdoConnectionState
    {
        /// <summary>
        /// Connection is busy
        /// </summary>
        Busy,
        /// <summary>
        /// Connection is open
        /// </summary>
        Open,
        /// <summary>
        /// Connection is closed
        /// </summary>
        Closed,
        /// <summary>
        /// Connection is pending. Additional parameters are required in order for it to be open
        /// </summary>
        Pending
    }
}
