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
using OSGeo.FDO.Expression;
using System.Collections.Specialized;
using OSGeo.FDO.Commands;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Use this class to set filter criteria for selecting features from a FDO datastore
    /// </summary>
    public class FeatureQueryOptions
    {
        /// <summary>
        /// Gets or sets the name of the feature class to query
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the alias fo the feature class to query
        /// </summary>
        public string ClassAlias
        {
            get;
            set;
        }

        private List<FdoJoinCriteriaInfo> _joinCriteria;

        /// <summary>
        /// Gets the list of join criteria
        /// </summary>
        public ReadOnlyCollection<FdoJoinCriteriaInfo> JoinCriteria => _joinCriteria.AsReadOnly();

        private List<string> _PropertyList;

        /// <summary>
        /// Gets the list of feature class properties to include in the query result
        /// </summary>
        public ReadOnlyCollection<string> PropertyList => _PropertyList.AsReadOnly();

        /// <summary>
        /// Gets a list of computed expressions to include in the query result
        /// </summary>
        public Dictionary<string, Expression> ComputedProperties { get; }

        /// <summary>
        /// Gets or sets the filter to apply to the query
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className"></param>
        public FeatureQueryOptions(string className)
        {
            ClassName = className;
            _PropertyList = new List<string>();
            ComputedProperties = new Dictionary<string, Expression>();
            _OrderBy = new List<string>();
            _joinCriteria = new List<FdoJoinCriteriaInfo>();
        }

        /// <summary>
        /// Returns true if a filter has been defined for this query
        /// </summary>
        public bool IsFilterSet => !string.IsNullOrEmpty(Filter);

        /// <summary>
        /// Adds a computed expression to be part of the query result
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="expression"></param>
        public void AddComputedProperty(string alias, string expression)
        {
            ComputedProperties.Add(alias, Expression.Parse(expression));
        }

        /// <summary>
        /// Adds a series of computed expressions to be part of the query result
        /// </summary>
        /// <param name="computedProperties"></param>
        public void AddComputedProperty(NameValueCollection computedProperties)
        {
            if (computedProperties != null && computedProperties.Count > 0)
            {
                foreach (string alias in computedProperties.Keys)
                {
                    AddComputedProperty(alias, computedProperties[alias]);
                }
            }
        }

        /// <summary>
        /// Adds a join criteria to the query result
        /// </summary>
        /// <param name="criteria"></param>
        public void AddJoinCriteria(FdoJoinCriteriaInfo criteria)
        {
            _joinCriteria.Add(criteria);
        }

        /// <summary>
        /// Removes a join criteria that is to be part of the query result
        /// </summary>
        /// <param name="criteria"></param>
        public void RemoveJoinCriteria(FdoJoinCriteriaInfo criteria)
        {
            _joinCriteria.Remove(criteria);
        }

        /// <summary>
        /// Removes a computed expression that is to be part of the query result
        /// </summary>
        /// <param name="alias"></param>
        public void RemoveComputedProperty(string alias)
        {
            if (ComputedProperties.ContainsKey(alias))
                ComputedProperties.Remove(alias);
        }

        /// <summary>
        /// Removes a property that is to be part of the query result
        /// </summary>
        /// <param name="propertyName"></param>
        public void RemoveFeatureProperty(string propertyName)
        {
            _PropertyList.Remove(propertyName);
        }

        /// <summary>
        /// Adds a property that is to be part of the query result
        /// </summary>
        /// <param name="propertyName"></param>
        public void AddFeatureProperty(string propertyName)
        {
            _PropertyList.Add(propertyName);
        }

        /// <summary>
        /// Adds a series of properties that are to be part of the query result
        /// </summary>
        /// <param name="propertyNames"></param>
        public void AddFeatureProperty(IEnumerable<string> propertyNames)
        {
            _PropertyList.AddRange(propertyNames);
        }

        private List<string> _OrderBy;

        /// <summary>
        /// Gets the properties to order by
        /// </summary>
        public ReadOnlyCollection<string> OrderBy => _OrderBy.AsReadOnly();

        /// <summary>
        /// Gets the ordering option
        /// </summary>
        public OrderingOption OrderOption { get; private set; }

        /// <summary>
        /// Sets the ordering options for this query. Note that most providers do not support ordering.
        /// </summary>
        /// <param name="propertyNames"></param>
        /// <param name="option"></param>
        public void SetOrderingOption(IEnumerable<string> propertyNames, OrderingOption option)
        {
            SetOrderingOption(propertyNames, option, !string.IsNullOrEmpty(this.ClassAlias));
        }

        /// <summary>
        /// Sets the ordering options for this query. Note that most providers do not support ordering.
        /// </summary>
        /// <param name="propertyNames"></param>
        /// <param name="option"></param>
        /// <param name="useAlias"></param>
        public void SetOrderingOption(IEnumerable<string> propertyNames, OrderingOption option, bool useAlias)
        {
            _OrderBy.Clear();
            foreach (var p in propertyNames)
            {
                _OrderBy.Add((useAlias) ? this.ClassAlias + "." + p : p);
            }
            OrderOption = option;
        }
    }

    /// <summary>
    /// Use this class to set the filter criteria used to select groups of features from a FDO datastore or for restricting the values returned to be unique.
    /// </summary>
    public class FeatureAggregateOptions : FeatureQueryOptions
    {
        /// <summary>
        /// Gets or sets whether the query results are to be distinct
        /// </summary>
        public bool Distinct { get; set; }

        /// <summary>
        /// Gets the group filter
        /// </summary>
        public string GroupFilter { get; private set; }

        private List<string> _GroupByProperties;

        /// <summary>
        /// Gets the feature class properties to group by in the query result
        /// </summary>
        public IEnumerable<string> GroupByProperties => _GroupByProperties;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className"></param>
        public FeatureAggregateOptions(string className)
            : base(className)
        {
            _GroupByProperties = new List<string>();
        }

        /// <summary>
        /// Sets the grouping parameters
        /// </summary>
        /// <param name="groupByProperties"></param>
        /// <param name="groupFilter"></param>
        public void SetGroupingFilter(IEnumerable<string> groupByProperties, string groupFilter)
        {
            GroupFilter = groupFilter;
            _GroupByProperties.Clear();
            _GroupByProperties.AddRange(groupByProperties);
        }
    }

    public class FdoJoinCriteriaInfo
    {
        /// <summary>
        /// Gets or sets the join type
        /// </summary>
        public OSGeo.FDO.Expression.JoinType JoinType { get; set; }

        /// <summary>
        /// The prefix to use to disambiguate property names
        /// </summary>
        public string JoinPrefix { get; set; }

        /// <summary>
        /// Gets or sets the schema of the class to join on
        /// </summary>
        public string JoinSchema { get; set; }

        /// <summary>
        /// Gets or sets the class to join on
        /// </summary>
        public string JoinClass { get; set; }

        /// <summary>
        /// Gets or sets the alias for the class to join on
        /// </summary>
        public string JoinClassAlias { get; set; }

        /// <summary>
        /// Gets or sets the join filter
        /// </summary>
        public string JoinFilter { get; set; }

        public OSGeo.FDO.Expression.JoinCriteria AsJoinCriteria()
        {
            var criteria = new OSGeo.FDO.Expression.JoinCriteria
            {
                JoinType = this.JoinType,
                JoinClass = new OSGeo.FDO.Expression.Identifier(this.JoinClass)
            };
            if (!string.IsNullOrEmpty(this.JoinClassAlias))
                criteria.Alias = this.JoinClassAlias;
            criteria.Filter = OSGeo.FDO.Filter.Filter.Parse(this.JoinFilter);
            return criteria;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.JoinType == OSGeo.FDO.Expression.JoinType.JoinType_None)
                return "NO JOIN";
            else
            {
                StringBuilder sb = new StringBuilder();
                switch (this.JoinType)
                {
                    case OSGeo.FDO.Expression.JoinType.JoinType_Cross:
                        sb.Append("CROSS JOIN ");
                        break;
                    case OSGeo.FDO.Expression.JoinType.JoinType_FullOuter:
                        sb.Append("FULL OUTER JOIN ");
                        break;
                    case OSGeo.FDO.Expression.JoinType.JoinType_Inner:
                        sb.Append("INNER JOIN ");
                        break;
                    case OSGeo.FDO.Expression.JoinType.JoinType_LeftOuter:
                        sb.Append("LEFT OUTER JOIN ");
                        break;
                    case OSGeo.FDO.Expression.JoinType.JoinType_RightOuter:
                        sb.Append("RIGHT OUTER JOIN ");
                        break;
                }
                sb.Append(this.JoinClass + " ");
                if (!string.IsNullOrEmpty(this.JoinClassAlias))
                    sb.Append(this.JoinClassAlias + " ON ");
                sb.Append(this.JoinFilter);
                return sb.ToString();
            }
        }
    }
}
