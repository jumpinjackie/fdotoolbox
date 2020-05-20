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
using System.Collections.Specialized;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using FdoToolbox.Core;
using ICSharpCode.Core;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Tasks.Controls
{
    public interface IFdoJoinView
    {
        string TaskName { get; set; }

        List<string> LeftConnections { set; }
        List<string> RightConnections { set; }
        List<string> TargetConnections { set; }

        List<string> LeftSchemas { set; }
        List<string> RightSchemas { set; }
        List<string> TargetSchemas { set; }

        List<string> LeftClasses { set; }
        List<string> RightClasses { set; }

        Array JoinTypes { set; }

        string SelectedLeftConnection { get; set; }
        string SelectedRightConnection { get; set; }
        string SelectedTargetConnection { get; set; }

        string SelectedLeftSchema { get; set; }
        string SelectedRightSchema { get; set; }
        string SelectedTargetSchema { get; set; }

        string SelectedLeftClass { get; set; }
        string SelectedRightClass { get; set; }
        string SelectedTargetClass { get; set; }

        string LeftPrefix { get; set; }
        string RightPrefix { get; set; }

        string LeftFilter { get; set; }
        string RightFilter { get; set; }

        FdoJoinType SelectedJoinType { get; set; }

        bool TargetGeometryPropertyEnabled { get; set; }

        /// <summary>
        /// Sets the properties or gets the checked properties
        /// </summary>
        List<string> LeftProperties { get; set; }

        /// <summary>
        /// Sets the properties or gets the checked properties
        /// </summary>
        List<string> RightProperties { get; set; }

        int BatchSize { get; set; }
        bool BatchEnabled { get; set; }

        void ClearJoins();
        void AddPropertyJoin(string left, string right);
        void RemoveJoin(string left);

        NameValueCollection GetJoinedProperties();

        bool LeftGeometryEnabled { get; set; }
        string LeftGeometryName { get; set; }
        bool LeftGeometryChecked { get; set; }
        bool RightGeometryEnabled { get; set; }
        string RightGeometryName { get; set; }
        bool RightGeometryChecked { get; set; }

        void CheckLeftProperties(ICollection<string> iCollection);

        void CheckRightProperties(ICollection<string> iCollection);
    }

    internal enum JoinSourceType
    {
        Left,
        Right,
        Target
    }

    public class FdoJoinPresenter
    {
        private readonly IFdoJoinView _view;
        private readonly IFdoConnectionManager _connMgr;
        private readonly TaskManager _taskMgr;

        public FdoJoinPresenter(IFdoJoinView view, IFdoConnectionManager connMgr, TaskManager taskMgr)
        {
            _view = view;
            _view.JoinTypes = Enum.GetValues(typeof(FdoJoinType));
            _connMgr = connMgr;
            _taskMgr = taskMgr;
        }

        private bool _init = false;

        public void Init()
        {
            _view.LeftConnections = new List<string>(_connMgr.GetConnectionNames());
            _view.RightConnections = new List<string>(_connMgr.GetConnectionNames());
            _view.TargetConnections = new List<string>(_connMgr.GetConnectionNames());

            ConnectionChanged(JoinSourceType.Left);
            ConnectionChanged(JoinSourceType.Right);
            ConnectionChanged(JoinSourceType.Target);
            
            _init = true;
            SetTargetGeometries();

            GeometryPropertyCheckChanged();
        }

        private FdoJoin _join;

        public void Init(FdoJoin join)
        {
            this.Init();
            _join = join;

            FdoJoinOptions options = join.Options;

            _view.SelectedJoinType = options.JoinType;

            _view.SelectedLeftConnection = _connMgr.GetName(options.Left.Connection);
            _view.SelectedLeftSchema = options.Left.SchemaName;
            _view.SelectedLeftClass = options.Left.ClassName;

            _view.SelectedRightConnection = _connMgr.GetName(options.Right.Connection);
            _view.SelectedRightSchema = options.Right.SchemaName;
            _view.SelectedRightClass = options.Right.ClassName;

            _view.SelectedTargetConnection = _connMgr.GetName(options.Target.Connection);
            _view.SelectedTargetSchema = options.Target.SchemaName;
            _view.SelectedTargetClass = options.Target.ClassName;

            _view.LeftPrefix = options.LeftPrefix;
            _view.RightPrefix = options.RightPrefix;
            _view.LeftFilter = options.LeftFilter;
            _view.RightFilter = options.RightFilter;
            _view.CheckLeftProperties(options.LeftProperties);
            _view.CheckRightProperties(options.RightProperties);

            if (options.BatchSize > 0)
            {
                //assert _view.BatchEnabled == true;
                _view.BatchSize = options.BatchSize;
            }

            if (!string.IsNullOrEmpty(options.GeometryProperty))
            {
                _view.TargetGeometryPropertyEnabled = true;
                if (options.Side == FdoToolbox.Core.Configuration.JoinSide.Left)
                {
                    _view.LeftGeometryChecked = true;
                    //_view.LeftGeometryEnabled = true;
                }
                else //Right
                {
                    _view.RightGeometryChecked = true;
                    //_view.RightGeometryEnabled = true;
                }
            }

            foreach (string leftProp in options.JoinPairs.Keys)
            {
                _view.AddPropertyJoin(leftProp, options.JoinPairs[leftProp]);
            }
        }

        internal FdoConnection GetConnection(JoinSourceType type)
        {
            switch (type)
            {
                case JoinSourceType.Left:
                    return _connMgr.GetConnection(_view.SelectedLeftConnection);
                case JoinSourceType.Right:
                    return _connMgr.GetConnection(_view.SelectedRightConnection);
                case JoinSourceType.Target:
                    return _connMgr.GetConnection(_view.SelectedTargetConnection);
                default:
                    return null;
            }
        }

        internal void ConnectionChanged(JoinSourceType type)
        {
            if(type != JoinSourceType.Target)
                _view.ClearJoins();

            FdoConnection conn = GetConnection(type);
            if (conn != null)
            {
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    switch (type)
                    {
                        case JoinSourceType.Left:
                            _view.LeftSchemas = service.GetSchemaNames();
                            SchemaChanged(type);
                            break;
                        case JoinSourceType.Right:
                            _view.RightSchemas = service.GetSchemaNames();
                            SchemaChanged(type);
                            break;
                        case JoinSourceType.Target:
                            _view.TargetSchemas = service.GetSchemaNames();
                            SchemaChanged(type);
                            break;
                    }
                }
            }
        }

        internal void SchemaChanged(JoinSourceType type)
        {
            if (type == JoinSourceType.Target)
                return;

            _view.ClearJoins();
            FdoConnection conn = GetConnection(type);
            if (conn != null)
            {
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    switch (type)
                    {
                        case JoinSourceType.Left:
                            _view.LeftClasses = service.GetClassNames(_view.SelectedLeftSchema);
                            ClassChanged(type);
                            break;
                        case JoinSourceType.Right:
                            _view.RightClasses = service.GetClassNames(_view.SelectedRightSchema);
                            ClassChanged(type);
                            break;
                    }
                }
            }
        }

        internal void ClassChanged(JoinSourceType type)
        {
            if(type == JoinSourceType.Target)
                return;
            
            _view.ClearJoins();

            FdoConnection conn = GetConnection(type);
            if(conn != null)
            {
                using(FdoFeatureService service = conn.CreateFeatureService())
                {
                    switch (type)
                    {
                        case JoinSourceType.Left:
                            {
                                string schema = _view.SelectedLeftSchema;
                                string className = _view.SelectedLeftClass;

                                ClassDefinition cd = service.GetClassByName(schema, className);
                                if (cd != null)
                                {
                                    List<string> properties = new List<string>();
                                    foreach(PropertyDefinition pd in cd.Properties)
                                    {
                                        properties.Add(pd.Name);
                                    }
                                    _view.LeftProperties = properties;
                                }
                            }
                            break;
                        case JoinSourceType.Right:
                            {
                                string schema = _view.SelectedRightSchema;
                                string className = _view.SelectedRightClass;

                                ClassDefinition cd = service.GetClassByName(schema, className);
                                if (cd != null)
                                {
                                    List<string> properties = new List<string>();
                                    foreach (PropertyDefinition pd in cd.Properties)
                                    {
                                        properties.Add(pd.Name);
                                    }
                                    _view.RightProperties = properties;
                                }
                            }
                            break;
                    }
                }
            }

            SetTargetGeometries();
        }

        private void SetTargetGeometries()
        {
            if (!_init)
                return;

            _view.LeftGeometryName = string.Empty;
            _view.RightGeometryName = string.Empty;
            _view.LeftGeometryEnabled = false;
            _view.RightGeometryEnabled = false;
            _view.LeftGeometryChecked = false;
            _view.RightGeometryChecked = false;

            using (FdoFeatureService leftService = GetConnection(JoinSourceType.Left).CreateFeatureService())
            using (FdoFeatureService rightService = GetConnection(JoinSourceType.Right).CreateFeatureService())
            {
                ClassDefinition leftClass = leftService.GetClassByName(_view.SelectedLeftSchema, _view.SelectedLeftClass);
                ClassDefinition rightClass = rightService.GetClassByName(_view.SelectedRightSchema, _view.SelectedRightClass);

                if (leftClass?.ClassType == ClassType.ClassType_FeatureClass)
                {
                    string geomName = ((FeatureClass)leftClass).GeometryProperty.Name;
                    //targetGeoms.Add(geomName + " (Left)", geomName);
                    //_view.LeftGeometryEnabled = true;
                    _view.LeftGeometryName = geomName;
                    _view.LeftGeometryChecked = true;
                    
                }

                if (rightClass?.ClassType == ClassType.ClassType_FeatureClass)
                {
                    string geomName = ((FeatureClass)rightClass).GeometryProperty.Name;
                    //targetGeoms.Add(geomName + " (Right)", geomName);
                    //_view.RightGeometryEnabled = true;
                    _view.RightGeometryName = geomName;

                    if (!_view.LeftGeometryChecked)
                        _view.RightGeometryChecked = true;
                }
            }
        }

        internal void SaveTask()
        {
            if (string.IsNullOrEmpty(_view.TaskName))
                throw new TaskValidationException(ResourceService.GetString("ERR_TASK_NAME_REQUIRED"));

            var leftPropNames = _view.LeftProperties;
            var rightPropNames = _view.RightProperties;
            var allPropNames = new List<string>();
            foreach (var p in leftPropNames)
            {
                var name = _view.LeftPrefix + p;
                if (allPropNames.Contains(name))
                    throw new TaskValidationException(ResourceService.GetStringFormatted("ERR_MERGED_CLASS_DUPLICATE_PROPERTY", name));

                allPropNames.Add(name);
            }
            foreach (var p in rightPropNames)
            {
                var name = _view.RightPrefix + p;
                if (allPropNames.Contains(name))
                    throw new TaskValidationException(ResourceService.GetStringFormatted("ERR_MERGED_CLASS_DUPLICATE_PROPERTY", name));

                allPropNames.Add(name);
            }

            FdoJoinOptions options = null;
            if (_join == null)
            {
                options = new FdoJoinOptions();
            }
            else
            {
                options = _join.Options;
                options.Reset();
            }
            options.SetLeft(
                _connMgr.GetConnection(_view.SelectedLeftConnection),
                _view.SelectedLeftSchema,
                _view.SelectedLeftClass);
            options.SetRight(
                _connMgr.GetConnection(_view.SelectedRightConnection),
                _view.SelectedRightSchema,
                _view.SelectedRightClass);
            options.SetTarget(
                _connMgr.GetConnection(_view.SelectedTargetConnection),
                _view.SelectedTargetSchema,
                _view.SelectedTargetClass);

            if (_view.BatchEnabled)
                options.BatchSize = _view.BatchSize;

            if (_view.TargetGeometryPropertyEnabled)
            {
                if (!string.IsNullOrEmpty(_view.LeftGeometryName) && _view.LeftGeometryChecked)
                {
                    options.GeometryProperty = _view.LeftGeometryName;
                    options.Side = FdoToolbox.Core.Configuration.JoinSide.Left;
                }
                else if (!string.IsNullOrEmpty(_view.RightGeometryName) && _view.RightGeometryChecked)
                {
                    options.GeometryProperty = _view.RightGeometryName;
                    options.Side = FdoToolbox.Core.Configuration.JoinSide.Right;
                }
            }

            options.SetJoinPairs(_view.GetJoinedProperties());
            
            options.JoinType = _view.SelectedJoinType;
            
            foreach (string leftProp in _view.LeftProperties)
            {
                options.AddLeftProperty(leftProp);
            }

            foreach (string rightProp in _view.RightProperties)
            {
                options.AddRightProperty(rightProp);
            }

            options.LeftPrefix = _view.LeftPrefix;
            options.RightPrefix = _view.RightPrefix;
            options.LeftFilter = _view.LeftFilter;
            options.RightFilter = _view.RightFilter;

            options.Validate();

            FdoJoin join = null;
            if (_join == null) //New join
            {
                join = new FdoJoin(options);
                _taskMgr.AddTask(_view.TaskName, join);
            }
            else //Join was loaded. Update its options
            {
                _join.Options = options;
            }
        }

        internal void GeometryPropertyCheckChanged()
        {
            _view.LeftGeometryEnabled = _view.TargetGeometryPropertyEnabled && !string.IsNullOrEmpty(_view.LeftGeometryName);
            _view.RightGeometryEnabled = _view.TargetGeometryPropertyEnabled && !string.IsNullOrEmpty(_view.RightGeometryName);
        }
    }
}
