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
using System.ComponentModel;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Filter;
using System.Data;
using ICSharpCode.Core;
using System.Diagnostics;
using System.Timers;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Base.Controls
{
    internal enum QueryMode
    {
        Standard,
        ExtendedSelect,
        Aggregate,
        SQL
    }

    internal interface IFdoDataPreviewView : IViewContent
    {
        List<QueryMode> QueryModes { set; }
        QueryMode SelectedQueryMode { get; }

        IQuerySubView QueryView { get; set; }

        bool CancelEnabled { get; set; }
        bool ClearEnabled { get; set; }
        bool ExecuteEnabled { get; set; }
        bool InsertEnabled { get; set; }
        bool DeleteEnabled { get; set; }
        bool UpdateEnabled { get; set; }

        string StatusMessage { set; }
        string ElapsedMessage { set; }
        FdoFeatureTable ResultTable { set; get; }

        bool MapEnabled { set; }

        void DisplayError(Exception exception);

        void SetBusyCursor(bool busy);
    }

    internal class FdoDataPreviewPresenter
    {
        private readonly IFdoDataPreviewView _view;
        private FdoConnection _connection;
        private FdoFeatureService _service;
        private BackgroundWorker _queryWorker;

        private Dictionary<QueryMode, IQuerySubView> _queryViews;
        private Timer _timer;
        private DateTime _queryStart;

        internal void SetInitialPreviewLimit(int limit)
        {
            ((IFdoStandardQueryView)_queryViews[QueryMode.Standard]).Limit = limit;
        }

        private bool insertSupported;
        private bool updateSupported;
        private bool deleteSupported;

        internal FdoConnection Connection
        {
            get { return _connection; }
        }

        internal string SelectedClassName
        {
            get
            {
                IFdoStandardQueryView qv = _view.QueryView as IFdoStandardQueryView;
                if(qv == null)
                    return string.Empty;
                return qv.SelectedClass.ClassName;
            }
        }

        internal string SelectedClassNameQualified
        {
            get
            {
                IFdoStandardQueryView qv = _view.QueryView as IFdoStandardQueryView;
                if (qv == null)
                    return string.Empty;
                return qv.SelectedClass.QualifiedName;
            }
        }

        public FdoDataPreviewPresenter(IFdoDataPreviewView view, FdoConnection conn)
        {
            _timer = new Timer();
            _view = view;
            _connection = conn;
            _service = conn.CreateFeatureService();
            _queryViews = new Dictionary<QueryMode, IQuerySubView>();
            _queryWorker = new BackgroundWorker();
            _queryWorker.WorkerReportsProgress = true;
            _queryWorker.WorkerSupportsCancellation = true;
            _queryWorker.DoWork += new DoWorkEventHandler(DoWork);
            //_queryWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            _queryWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            _view.ElapsedMessage = string.Empty;
            _view.CancelEnabled = false;
            _view.ExecuteEnabled = true;
            
            insertSupported = (Array.IndexOf(conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList), OSGeo.FDO.Commands.CommandType.CommandType_Insert) >= 0);
            updateSupported = (Array.IndexOf(conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList), OSGeo.FDO.Commands.CommandType.CommandType_Update) >= 0);
            deleteSupported = (Array.IndexOf(conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList), OSGeo.FDO.Commands.CommandType.CommandType_Delete) >= 0);

            _view.DeleteEnabled = deleteSupported;
            _view.UpdateEnabled = updateSupported;

            _timer.Interval = 1000;
            _timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan ts = e.SignalTime.Subtract(_queryStart);
            _view.ElapsedMessage = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _view.SetBusyCursor(false);
            _timer.Stop();
            _view.CancelEnabled = false;
            _view.ClearEnabled = true;
            _view.ExecuteEnabled = true;

            if (e.Error != null)
            {
                _view.DisplayError(e.Error);
            }
            else
            {
                FdoFeatureTable result = null;
                if (e.Cancelled)
                    result = _cancelResult;
                else
                    result = e.Result as FdoFeatureTable;

                if (result != null)
                {
                    _view.ResultTable = result;
                    if (e.Cancelled)
                        _view.StatusMessage = string.Format("Query cancelled. Returned {0} results", result.Rows.Count);
                    else
                        _view.StatusMessage = string.Format("Returned {0} results", result.Rows.Count);
                }
                else //No result table
                {
                    if (e.Cancelled)
                        _view.StatusMessage = "Query cancelled";
                    else
                        _view.StatusMessage = string.Format("0 results");
                }
            }
        }

        private FdoFeatureTable _cancelResult;

        void DoWork(object sender, DoWorkEventArgs e)
        {
            IFdoReader reader = null;
            using (FdoFeatureService service = _connection.CreateFeatureService())
            {
                try
                {
                    if (e.Argument is FeatureAggregateOptions) 
                    {
                        reader = service.SelectAggregates((FeatureAggregateOptions)e.Argument);
                    }
                    else if (e.Argument is StandardQuery)
                    {
                        var stdArg = (e.Argument as StandardQuery);
                        reader = service.SelectFeatures(stdArg.query, stdArg.Limit, stdArg.UseExtendedSelect);
                    }
                    else if (e.Argument is string)
                    {
                        reader = service.ExecuteSQLQuery(e.Argument.ToString());
                    }
                    
                    //Init the data grid view
                    FdoFeatureTable table = new FdoFeatureTable();

                    table.RequestSpatialContext += delegate(object o, string name)
                    {
                        SpatialContextInfo c = service.GetSpatialContext(name);
                        if (c != null)
                            table.AddSpatialContext(c);
                    };

                    ClassDefinition clsDef = null;
                    bool hasJoins = false;
                    if (e.Argument is StandardQuery)
                    {
                        var qry = ((StandardQuery)e.Argument).query;
                        clsDef = service.GetClassByName(qry.ClassName); //TODO: Should really qualify this, but our input parameters do not specify a schema
                        hasJoins = qry.JoinCriteria.Count > 0;
                    }
                    table.InitTable(reader, clsDef, hasJoins);

					// need to store object class defn outside loop
					ClassDefinition classDefObject = null;
					ClassDefinition classDefAssoc = null;

                    while (reader.ReadNext() && !_queryWorker.CancellationPending)
                    {
                        //Pass processed feature to data grid view
                        FdoFeature feat = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string name = reader.GetName(i);
                            if (!reader.IsNull(name))
                            {
                                FdoPropertyType pt = reader.GetFdoPropertyType(name);

                                switch (pt)
                                {
                                    case FdoPropertyType.Association:
										// TODO: how to handle for non-StandardQuery
										if (e.Argument is StandardQuery)
										{
											// because the original code used an iterator over the reader.FieldCount
											// it is a bit of a hack to get another definition of the class and check
											// we are at the right property with a name comparison
											// TODO: code review to see if this can be implemented better
											FdoFeatureReader readerFeature = (FdoFeatureReader)reader;
											ClassDefinition classDef = readerFeature.GetClassDefinition();

											foreach (PropertyDefinition propertyDef in classDef.Properties)
											{
												// only looking for the right association
												if ( (PropertyType.PropertyType_AssociationProperty == propertyDef.PropertyType)
													&& (propertyDef.Name.Equals(name)) )
												{
													AssociationPropertyDefinition assocProp = (AssociationPropertyDefinition)propertyDef;
													ClassDefinition classAssoc = assocProp.AssociatedClass;

													// get a reader from the association - nice!
													IFeatureReader readerAssoc = readerFeature.GetFeatureObject(name);
													if ((null != readerAssoc) && (readerAssoc.ReadNext()))
													{
														// TODO: this should be an iterative function

														// the simple reassignment on each instance fails
														//  (classDefObject.Properties is always zero after first record)
														//  so have set classDefObject higher-up and re-use/
														//   - problem will occur if more than one association
														// ClassDefinition classDefObject = readerObject.GetClassDefinition();
														if (null == classDefAssoc)
															classDefAssoc = readerAssoc.GetClassDefinition();

														foreach (PropertyDefinition propertyDefAssoc in classDefAssoc.Properties)
														{
															String nameAssoc = propertyDefAssoc.Name;

															// TODO: only "data" type handled at present
															if (PropertyType.PropertyType_DataProperty == propertyDefAssoc.PropertyType)
															{
																DataPropertyDefinition datapropertyDefAssoc = (DataPropertyDefinition)propertyDefAssoc;

																String szFullName = name + "." + nameAssoc;
																DataType ptAssoc = datapropertyDefAssoc.DataType;
																// TODO: handle all types
																switch (ptAssoc)
																{
																	case DataType.DataType_String:
																		feat[szFullName] = readerAssoc.GetString(nameAssoc);
																	break;

																	case DataType.DataType_Int16:
																		feat[szFullName] = readerAssoc.GetInt16(nameAssoc);
																	break;

																	case DataType.DataType_Int32:
																		feat[szFullName] = readerAssoc.GetInt32(nameAssoc);
																	break;

																	case DataType.DataType_Int64:
																		feat[szFullName] = readerAssoc.GetInt64(nameAssoc);
																	break;
																}
															}
														}
														readerAssoc.Close();
													}
												}
											}
										}
									break;

									case FdoPropertyType.BLOB:
                                        feat[name] = reader.GetLOB(name).Data;
                                        break;
                                    case FdoPropertyType.Boolean:
                                        feat[name] = reader.GetBoolean(name);
                                        break;
                                    case FdoPropertyType.Byte:
                                        feat[name] = reader.GetByte(name);
                                        break;
                                    case FdoPropertyType.CLOB:
                                        feat[name] = reader.GetLOB(name).Data;
                                        break;
                                    case FdoPropertyType.DateTime:
                                        {
                                            try
                                            {
                                                feat[name] = reader.GetDateTime(name);
                                            }
                                            catch //Unrepresentable dates
                                            {
                                                feat[name] = DBNull.Value;
                                            }
                                        }
                                        break;
                                    case FdoPropertyType.Decimal: //We should probably remove this as FDO coerces decimals to doubles (otherwise why is there not a GetDecimal() method in FdoIReader?)
                                        {
                                            double val = reader.GetDouble(name);
                                            if (Double.IsNaN(val))
                                                feat[name] = DBNull.Value;
                                            else
                                                feat[name] = Convert.ToDecimal(val);
                                        }
                                        break;
                                    case FdoPropertyType.Double:
                                        feat[name] = reader.GetDouble(name);
                                        break;
                                    case FdoPropertyType.Geometry:
                                        {
                                            try
                                            {
                                                byte[] fgf = reader.GetGeometry(name);
                                                OSGeo.FDO.Geometry.IGeometry geom = service.GeometryFactory.CreateGeometryFromFgf(fgf);
                                                var fg = new FdoGeometry(geom); 
                                                //OGR geometry trap
                                                using (var env = fg.Envelope) { }
                                                feat[name] = fg;
                                            }
                                            catch
                                            {
                                                feat[name] = DBNull.Value;
                                            }
                                        }
                                        break;
                                    case FdoPropertyType.Int16:
                                        feat[name] = reader.GetInt16(name);
                                        break;
                                    case FdoPropertyType.Int32:
                                        feat[name] = reader.GetInt32(name);
                                        break;
                                    case FdoPropertyType.Int64:
                                        feat[name] = reader.GetInt64(name);
                                    break;

                                    case FdoPropertyType.Object:
										// TODO: how to handle for non-StandardQuery
										if (e.Argument is StandardQuery)
										{
											// because the original code used an iterator over the reader.FieldCount
											// it is a bit of a hack to get another definition of the class and check
											// we are at the right property with a name comparison
											// TODO: code review to see if this can be implemented better
											FdoFeatureReader readerFeature = (FdoFeatureReader)reader;
											ClassDefinition classDef = readerFeature.GetClassDefinition();

											foreach (PropertyDefinition propertyDef in classDef.Properties)
											{
												// only looking for the right object
												if ((PropertyType.PropertyType_ObjectProperty == propertyDef.PropertyType)
													&& (propertyDef.Name.Equals(name)))
												{
													ObjectPropertyDefinition assocProp = (ObjectPropertyDefinition)propertyDef;
													ClassDefinition classAssoc = assocProp.Class;

													// get a reader from the object - nice!
													IFeatureReader readerObject = readerFeature.GetFeatureObject(name);
													if ((null != readerObject) && (readerObject.ReadNext()))
													{
														// TODO: this should be an iterative function

														// the simple reassignment on each instance fails
														//  (classDefObject.Properties is always zero after first record)
														//  so have set classDefObject higher-up and re-use/
														//   - problem will occur if more than one association
														// ClassDefinition classDefObject = readerObject.GetClassDefinition();
														if (null == classDefObject)
															classDefObject = readerObject.GetClassDefinition();

														foreach (PropertyDefinition propertyDefObject in classDefObject.Properties)
														{
															String nameObject = propertyDefObject.Name;

															// TODO: only "data" type handled at present
															if (PropertyType.PropertyType_DataProperty == propertyDefObject.PropertyType)
															{
																DataPropertyDefinition datapropertyDefObject = (DataPropertyDefinition)propertyDefObject;

																String szFullName = name + "." + nameObject;
																DataType ptAssoc = datapropertyDefObject.DataType;
																// TODO: handle all types
																switch (ptAssoc)
																{
																	case DataType.DataType_String:
																		feat[szFullName] = readerObject.GetString(nameObject);
																	break;

																	case DataType.DataType_Int16:
																		feat[szFullName] = readerObject.GetInt16(nameObject);
																	break;

																	case DataType.DataType_Int32:
																		feat[szFullName] = readerObject.GetInt32(nameObject);
																	break;

																	case DataType.DataType_Int64:
																		feat[szFullName] = readerObject.GetInt64(nameObject);
																	break;
																}
															}
														}
														readerObject.Close();
														readerObject = null;
													}
												}
											}
										} 
									break;

                                    //case FdoPropertyType.Raster:
                                    case FdoPropertyType.Single:
                                        feat[name] = reader.GetSingle(name);
                                        break;
                                    case FdoPropertyType.String:
                                        feat[name] = reader.GetString(name);
                                        break;
                                }
                            }
                            else
                            {
                                feat[name] = DBNull.Value;
                            }
                        }
                        table.AddRow(feat);

                        if (table.Rows.Count % 50 == 0)
                        {
                            System.Threading.Thread.Sleep(50);
                        }
                    }

                    if (_queryWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        _cancelResult = table;
                    }
                    else
                    {
                        e.Result = table;
                    }
                }
                finally
                {
                    if(reader != null)
                        reader.Close();
                }
            }
        }

        public void Init(string initSchema, string initClass)
        {
            List<QueryMode> modes = new List<QueryMode>();
            if (!string.IsNullOrEmpty(initSchema) && !string.IsNullOrEmpty(initClass))
            {
                if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select))
                {
                    using (FdoFeatureService service = _connection.CreateFeatureService())
                    {
                        ClassDefinition classDef = service.GetClassByName(initSchema, initClass);
                        if (!ExpressUtility.HasRaster(classDef))
                        {
                            modes.Add(QueryMode.Standard);
                            _queryViews.Add(QueryMode.Standard, new FdoStandardQueryCtl(_connection, initSchema, initClass));
                        }
                    }
                }
                if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
                {
                    modes.Add(QueryMode.Aggregate);
                    _queryViews.Add(QueryMode.Aggregate, new FdoAggregateQueryCtl(_connection, initSchema, initClass));
                }
            }
            else
            {
                if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select))
                {
                    modes.Add(QueryMode.Standard);
                    _queryViews.Add(QueryMode.Standard, new FdoStandardQueryCtl(_connection));
                }
                if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
                {
                    modes.Add(QueryMode.Aggregate);
                    _queryViews.Add(QueryMode.Aggregate, new FdoAggregateQueryCtl(_connection));
                }
            }
            if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand))
            {
                modes.Add(QueryMode.SQL);
                _queryViews.Add(QueryMode.SQL, new FdoSqlQueryCtl());
            }
            foreach (IQuerySubView qv in _queryViews.Values)
            {
                qv.MapPreviewStateChanged += new MapPreviewStateEventHandler(OnMapPreviewStateChanged);
                qv.SetRestrictions(_connection.Capability);
            }
            _view.QueryModes = modes;

            _view.InsertEnabled = (_view.SelectedQueryMode != QueryMode.SQL) && insertSupported;
        }

        void OnMapPreviewStateChanged(object sender, bool enabled)
        {
            _view.MapEnabled = enabled;
        }

        public void QueryModeChanged()
        {
            _view.QueryView = _queryViews[_view.SelectedQueryMode];
            _view.InsertEnabled = insertSupported && (_view.SelectedQueryMode != QueryMode.SQL);
            //_view.MapEnabled = (_view.SelectedQueryMode == QueryMode.Standard);
        }

        class StandardQuery
        {
            public bool UseExtendedSelect;
            public FeatureQueryOptions query;
            public int Limit;
        }

        public void ExecuteQuery()
        {
            object query = null;
            switch (_view.SelectedQueryMode)
            {
                case QueryMode.Aggregate:
                    {
                        query = (_view.QueryView as IFdoAggregateQueryView).QueryObject;
                    }
                    break;
                case QueryMode.SQL:
                    {
                        query = (_view.QueryView as IFdoSqlQueryView).SQLString;
                    }
                    break;
                case QueryMode.Standard:
                    {
                        StandardQuery qry = new StandardQuery();
                        qry.UseExtendedSelect = (_view.QueryView as IFdoStandardQueryView).UseExtendedSelectForOrdering;
                        qry.query = (_view.QueryView as IFdoStandardQueryView).QueryObject;
                        qry.Limit = (_view.QueryView as IFdoStandardQueryView).Limit;
                        query = qry;
                    }
                    break;
            }
            if (query != null)
            {
                long count = 0;
                try
                {
                    count = GetFeatureCount();
                }
                catch (Exception)
                {
                    if (!_view.Confirm(ResourceService.GetString("TITLE_DATA_PREVIEW"), "An error occured attempting to get the total number of features in this feature class. As a result, the size of the result set produced by your query cannot be determined. Proceed anyway?"))
                    {
                        _view.SetBusyCursor(false);
                        return;
                    }
                }

                int limit = Preferences.DataPreviewWarningLimit;

                if (_view.SelectedQueryMode == QueryMode.Standard)
                {
                    if ((query as StandardQuery).Limit <= 0)
                    {
                        if (count > limit && !_view.ConfirmFormatted(ResourceService.GetString("TITLE_DATA_PREVIEW"), ResourceService.GetString("QUESTION_DATA_PREVIEW_LIMIT"), count.ToString()))
                        {
                            _view.SetBusyCursor(false);
                            return;
                        }
                    }
                }
                Clear();
                _view.CancelEnabled = true;
                _view.ClearEnabled = false;
                _view.ExecuteEnabled = false;
                _timer.Start();
                _view.StatusMessage = "Executing Query";
                _view.ElapsedMessage = "00:00:00";
                _queryStart = DateTime.Now;

                _view.SetBusyCursor(true);
                _queryWorker.RunWorkerAsync(query);
            }
        }

        public void CancelCurrentQuery()
        {
            _queryWorker.CancelAsync();
        }

        public long GetFeatureCount()
        {
            IFdoStandardQueryView qv = _view.QueryView as IFdoStandardQueryView;
            if(qv == null)
                return 0;

            ClassDefinition classDef = qv.SelectedClassDefinition;
            var query = qv.QueryObject;

            //Can't count joins (yet)
            if (query.JoinCriteria.Count > 0)
                return 0;

            return _service.GetFeatureCount(classDef, query.Filter, false);
        }

        public void Clear()
        {
            _view.ResultTable = null;
            _view.StatusMessage = string.Empty;
            _view.ElapsedMessage = string.Empty;
        }

        internal bool ConnectionMatch(FdoConnection conn)
        {
            return _connection == conn;
        }

        internal void DoInsert()
        {
            FeatureQueryOptions query = null;
            switch (_view.SelectedQueryMode)
            {
                case QueryMode.Aggregate:
                    {
                        query = (_view.QueryView as IFdoAggregateQueryView).QueryObject;
                    }
                    break;
                case QueryMode.Standard:
                    {
                        query = (_view.QueryView as IFdoStandardQueryView).QueryObject;
                    }
                    break;
            }
            if (query != null)
            {
                Workbench wb = Workbench.Instance;
                FdoInsertScaffold ctl = new FdoInsertScaffold(_connection, query.ClassName);
                wb.ShowContent(ctl, ViewRegion.Dialog);
            }
        }

        internal void DoUpdate(FdoFeature feat)
        {
            FeatureQueryOptions query = null;
            switch (_view.SelectedQueryMode)
            {
                case QueryMode.Aggregate:
                    {
                        query = (_view.QueryView as IFdoAggregateQueryView).QueryObject;
                    }
                    break;
                case QueryMode.Standard:
                    {
                        query = (_view.QueryView as IFdoStandardQueryView).QueryObject;
                    }
                    break;
            }
            if (query != null)
            {
                string filter = GenerateFilter(feat);
                if (string.IsNullOrEmpty(filter))
                {
                    _view.ShowError("Unable to generate an update filter. Possibly this result set has no unique identifiers or this result set was produced from a SQL query");
                    return;
                }
                using (FdoFeatureService service = _connection.CreateFeatureService())
                {
                    //Update is based on a very simple premise, the filter should produce the
                    //same number of affected results when selecting and updating.
                    //
                    //In our case, the filter should affect exactly one result when selecting and updating.
                    long count = service.GetFeatureCount(feat.Table.TableName, filter, true);
                    if (1 == count)
                    {
                        Workbench wb = Workbench.Instance;
                        FdoUpdateScaffold ctl = new FdoUpdateScaffold(feat, _connection, filter);
                        wb.ShowContent(ctl, ViewRegion.Dialog);
                    }
                }
            }
            else
            {
                _view.ShowError("Could not determine the feature class name from the result set");
            }
        }

        private void DeleteFeatures(FdoFeature feat)
        {
            DeleteFeatures(feat.Table, new FdoFeature[] { feat });
        }

        private void DeleteFeatures(FdoFeatureTable table, FdoFeature[] features)
        {
            string filter = GenerateFilter(features);
            if (string.IsNullOrEmpty(filter))
            {
                _view.ShowError("Unable to generate a delete filter. Possibly this result set has no unique identifiers or this result set was produced from a SQL query. If this result set is produced from a standard query, make sure that *ALL* identity properties are selected");
                return;
            }

            int expectedCount = features.Length;
            using (FdoFeatureService service = _connection.CreateFeatureService())
            {
                //Deleting is based on a very simple premise, the filter should produce the
                //same number of affected results when selecting and deleting.
                //
                //In our case, the filter should affect exactly the expected number of results when selecting and deleting.
                long count = service.GetFeatureCount(table.TableName, filter, true);
                if (expectedCount == count)
                {
                    int deleted = service.DeleteFeatures(table.TableName, filter, true);
                    if (expectedCount == deleted)
                    {
                        foreach (FdoFeature feat in features)
                        {
                            table.Rows.Remove(feat);
                        }
                        _view.ShowMessage("Delete Feature", "Feature Deleted");
                    }
                }
                else if (count > expectedCount)
                {
                    _view.ShowError("Delete operation would delete more than the expected number of features (" + expectedCount + ")");
                }
                else if (count == 0)
                {
                    _view.ShowError("Delete operation would not delete any features");
                }
            }
        }

        internal void DoDelete(FdoFeature[] features)
        {
            if (features == null || features.Length == 0)
                return;

            this.DeleteFeatures(features[0].Table, features);
        }

        internal void DoDelete(FdoFeature feat)
        {
            this.DeleteFeatures(feat);
        }

        internal string GenerateFilter(FdoFeature[] features)
        {
            if (features == null || features.Length == 0)
                return null;

            FdoFeatureTable table = features[0].Table;

            //All features in array must originate from the same table
            for (int i = 1; i < features.Length; i++)
            {
                if (features[i].Table != table)
                    return null;
            }

            List<string> filter = new List<string>();

            foreach (FdoFeature feat in features)
            {
                filter.Add(GenerateFilter(feat));
            }

            return string.Join(" OR ", filter.ToArray());
        }

        private string GenerateFilter(FdoFeature feat)
        {
            FdoFeatureTable table = feat.Table;
            if (table.PrimaryKey.Length > 0)
            {
                List<string> filters = new List<string>();
                foreach (DataColumn col in table.PrimaryKey)
                {
                    DataType dt = ExpressUtility.GetFdoDataTypeFromClrType(col.DataType);
                    string f = string.Empty;
                    if (dt == DataType.DataType_DateTime || dt == DataType.DataType_String)
                        f = col.ColumnName + " = '" + feat[col] + "'";
                    else
                        f = col.ColumnName + " = " + feat[col];

                    filters.Add(f);
                }
                return "(" + string.Join(" AND ", filters.ToArray()) + ")";
            }
            return null;
        }
    }
}
