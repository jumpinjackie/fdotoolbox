.. index::
   single: Previewing Data
   single: Querying Data

Previewing and Querying Data
============================

FDO Toolbox has a powerful Data Previewing component which allows you to inspect the data in any FDO data source. The Data Preview component
can support multiple methods of querying (depending on the connection's provider). To preview a data source, right click the connection on
the **Object Explorer** and choose **Data Query**

.. index::
   single: Previewing Data; Standard Query

Standard Query
--------------

The Standard Query mode is the default method of query data. It is supported by all FDO providers, although certain elements of the user interface may not be available because those feature are not supported.

The Standard Query mode user inteface has the following elements:

**Schema/Class**

This specifies the feature schema / feature class that you want to query data from

**Selected Properties**

This specifies the properties (columns) that you want as part of the query result. **NOTE:** In order to edit/delete features, you must have all identity properties selected (these are indicated by a key icon in the **Object Explorer**)

**Computed Properties**

This specifies any FDO expressions that are added as computed properties.

**Filter**

This specifies the FDO expression that filters the query results. Selecting the field will invoke the Expression Editor.

**Ordering**

Allows you to order the query result.

**Joins**

This allows you merge this result set with related results from another Class Definition from the same connection

.. index::
   single: Previewing Data; Aggregate Query

Aggregate Query
---------------

The Aggregate Query mode extends the Standard Query mode user interface with additional elements (again, some may not be supported by the current provider and will be hidden):

**Distinct**

If checked, will return all distinct combinations of the selected properties as part of the query result.

**Computed Properties**

You can specify FDO Aggregate Expressions

**Grouping**

Allows you to group the aggregate query result.

.. index::
   single: Previewing Data; SQL Query

SQL Query
---------

The SQL query mode user interface allows you to enter raw SQL text queries. If the SQL query is not a SELECT query a pre-defined result set is returned indicating the number of records affected. The SQL query mode is usually
only available for RDBMS-based FDO providers (eg. Oracle, MySQL, PostGIS, SQL Server, etc)

.. index::
   single: Previewing Data; Map Preview

Map Preview
-----------

By default, query results are viewed in tabular form. However, sometimes you can visually view the query result by switching to the :guilabel:`Map View` tab and clicking the Zoom Extents button.

In order to be able to visually view query results, you need to have done the following:

 * Specified the designated Geometry property in the list of **Selected Properties**
 * Specified the identity properties in the list of **Selected Properties**
 
Map Preview mode is not available for raw SQL queries

.. index::
   single: Previewing Data; Saving Query Results

Saving Query Results
--------------------

Once a query has been executed, you can choose to save that result in to a new spatial data source. To do this, click the :guilabel:`Save` button and choose the desired output format. As of writing, the current supported file formats are:

 * SDF
 * SQLite
 
When saving a query result, the created spatial data source uses the referenced spatial context of the queried feature class.