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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Filter;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Configuration;
using System.Xml.Serialization;
using System.IO;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Controls the <see cref="FdoJoin"/> operation
    /// </summary>
    public class FdoJoinOptions : IDisposable
    {
        private bool _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoJoinOptions"/> class.
        /// </summary>
        /// <param name="owner">if set to <c>true</c> this will own the underlying connections and dispose of them when it itself is disposed</param>
        public FdoJoinOptions(bool owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoJoinOptions"/> class.
        /// </summary>
        public FdoJoinOptions()
            : this(false)
        {
        }

        /// <summary>
        /// Gets or sets the batch size for batched inserts. Only applies if the
        /// target connection supports batch insertion.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the side of the join the designated geometry properties comes from
        /// </summary>
        /// <value>The side.</value>
        public JoinSide Side { get; set; }

        /// <summary>
        /// Gets or sets the type of join operation
        /// </summary>
        public FdoJoinType JoinType { get; set; }

        /// <summary>
        /// Gets the left join source
        /// </summary>
        public FdoSource Left { get; private set; }

        /// <summary>
        /// Gets the right join source
        /// </summary>
        public FdoSource Right { get; private set; }

        /// <summary>
        /// Gets the join target
        /// </summary>
        public FdoSource Target { get; private set; }

        /// <summary>
        /// Gets or sets the filter for the left source
        /// </summary>
        public string LeftFilter { get; set; }

        /// <summary>
        /// Gets or sets the filter for the right source
        /// </summary>
        public string RightFilter { get; set; }

        /// <summary>
        /// Gets or sets the left column prefix that is applied in the event of a name collision in the merged feature
        /// </summary>
        public string LeftPrefix { get; set; }

        /// <summary>
        /// Gets or sets the right column prefix that is applied in the event of a name collision in the merged feature
        /// </summary>
        public string RightPrefix { get; set; }

        private IList<string> _LeftProperties = new List<string>();

        /// <summary>
        /// Gets the property collection for the left side of the join
        /// </summary>
        public ICollection<string> LeftProperties => _LeftProperties;

        private IList<string> _RightProperties = new List<string>();

        /// <summary>
        /// Gets the property collection for the right side of the join
        /// </summary>
        public ICollection<string> RightProperties => _RightProperties;

        /// <summary>
        /// Adds a property to the left side of the join
        /// </summary>
        /// <param name="propertyName"></param>
        public void AddLeftProperty(string propertyName)
        {
            _LeftProperties.Add(propertyName);
        }

        /// <summary>
        /// Adds a property to the right side of the join
        /// </summary>
        /// <param name="propertyName"></param>
        public void AddRightProperty(string propertyName)
        {
            _RightProperties.Add(propertyName);
        }

        /// <summary>
        /// Gets the join pair collection
        /// </summary>
        public NameValueCollection JoinPairs { get; } = new NameValueCollection();

        /// <summary>
        /// Sets the join pair collection
        /// </summary>
        /// <param name="pairs"></param>
        public void SetJoinPairs(NameValueCollection pairs)
        {
            JoinPairs.Clear();
            JoinPairs.Add(pairs);
        }

        /// <summary>
        /// Sets the left source of the join
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetLeft(FdoConnection conn, string schemaName, string className)
        {
            Left = new FdoSource(conn, schemaName, className);
        }

        /// <summary>
        /// Sets the right source of the join
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetRight(FdoConnection conn, string schemaName, string className)
        {
            Right = new FdoSource(conn, schemaName, className);
        }

        /// <summary>
        /// Sets the target of the join
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetTarget(FdoConnection conn, string schemaName, string className)
        {
            Target = new FdoSource(conn, schemaName, className);
        }

        /// <summary>
        /// Creates the query for the left side of the join
        /// </summary>
        /// <returns></returns>
        internal FeatureQueryOptions CreateLeftQuery()
        {
            FeatureQueryOptions qry = new FeatureQueryOptions(Left.ClassName);
            return qry;
        }

        /// <summary>
        /// Creates the query for the right side of the join
        /// </summary>
        /// <returns></returns>
        internal FeatureQueryOptions CreateRightQuery()
        {
            FeatureQueryOptions qry = new FeatureQueryOptions(Right.ClassName);
            return qry;
        }

        /// <summary>
        /// Gets or sets the geometry property which will be the designated geometry
        /// property on the joined feature class. If a prefix has been specified, the
        /// geometry property must also be prefixed.
        /// </summary>
        public string GeometryProperty { get; set; }

        /// <summary>
        /// Validates these options
        /// </summary>
        public void Validate()
        {
            if (Left == null)
                throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_LEFT_UNDEFINED"));

            if (Right == null)
                throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_RIGHT_UNDEFINED"));

            if (Target == null)
                throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_TARGET_UNDEFINED"));

            if (string.IsNullOrEmpty(Target.ClassName))
                throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_TARGET_CLASS_UNDEFINED"));

            if (this.JoinPairs.Count == 0)
                throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_KEYS_UNDEFINED"));

            int count = this.LeftProperties.Count + this.RightProperties.Count;

            if (string.IsNullOrEmpty(LeftPrefix) && string.IsNullOrEmpty(RightPrefix))
            {
                ISet<string> set = new HashSet<string>();
                foreach (var prop in this.LeftProperties)
                    set.Add(prop);
                foreach (var prop in this.RightProperties)
                    set.Add(prop);

                //If all properties are unique then the counts should be the same
                if (set.Count < count)
                    throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_PROPERTY_NAME_COLLISION"));
            }

            //Verify left source filter checks out
            if (!string.IsNullOrEmpty(this.LeftFilter))
            {
                try
                {
                    using (var filter = Filter.Parse(this.LeftFilter)) { }
                }
                catch
                {
                    throw new TaskValidationException(ResourceUtil.GetStringFormatted("ERR_INVALID_LEFT_FILTER", this.LeftFilter));
                }
            }

            //Verify right source filter checks out
            if (!string.IsNullOrEmpty(this.RightFilter))
            {
                try
                {
                    using (var filter = Filter.Parse(this.RightFilter)) { }
                }
                catch
                {
                    throw new TaskValidationException(ResourceUtil.GetStringFormatted("ERR_INVALID_RIGHT_FILTER", this.LeftFilter));
                }
            }

            //Create target class. The schema must already exist, but the class must *not* already exist.
            using (FdoFeatureService service = this.Target.Connection.CreateFeatureService())
            {
                if (!service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema))
                    throw new TaskValidationException(ResourceUtil.GetStringFormatted("ERR_UNSUPPORTED_CMD", OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema));

                //Get target schema
                FeatureSchema schema = service.GetSchemaByName(this.Target.SchemaName);
                if (schema == null)
                    throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_SCHEMA_NOT_FOUND"));

                //Check target class does not exist
                int cidx = schema.Classes.IndexOf(this.Target.ClassName);
                if (cidx >= 0)
                    throw new TaskValidationException(ResourceUtil.GetString("ERR_JOIN_TARGET_EXISTS"));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_owner)
            {
                Left.Connection.Dispose();
                Right.Connection.Dispose();
                Target.Connection.Dispose();
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Left = null;
            Right = null;
            Target = null;
            _owner = false;
            _LeftProperties.Clear();
            _RightProperties.Clear();
        }
    }

    /// <summary>
    /// Defines a FDO source
    /// </summary>
    public class FdoSource
    {
        /// <summary>
        /// The connection for this source
        /// </summary>
        public FdoConnection Connection { get; set; }

        /// <summary>
        /// The schema name
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// The class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schema"></param>
        /// <param name="className"></param>
        public FdoSource(FdoConnection conn, string schema, string className)
        {
            Connection = conn;
            SchemaName = schema;
            ClassName = className;
        }
    }

    /// <summary>
    /// Defines the possible join types
    /// </summary>
    [Flags]
    public enum FdoJoinType
    {
        /// <summary>
        /// Inner join, only matching objects from both sides are merged
        /// </summary>
        Inner,
        /// <summary>
        /// Left join, left side objects are merged with right side objects regardless of whether the right side object exists or not
        /// </summary>
        Left,
        /*
        /// <summary>
        /// Right join, right side objects are merged with left side objects regardless of whether the left side object exists or not
        /// </summary>
        Right,
        /// <summary>
        /// Full join, both sides of the join are merged regardless of whether either side exists or not
        /// </summary>
        Full = Left | Right,
         */
    }
}
