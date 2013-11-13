Extract, Transform, Load (ETL) operations
=========================================

There are two main methods of transforming data in FDO Toolbox. They are explained in more detail below

.. index::
   single: Bulk Copy

Bulk Copy
---------

Bulk copy is the first form of data transformation. A bulk copy has the following properties:

 * It involves 1 or more participating connections
 * It contains 1 or more copy tasks.
   * Each copy tasks specifies a series of rules for copying data from [one feature class] in [a participating connection] to [another feature class] in [another participating connection].

To create a new Bulk Copy task, click the :guilabel:`Tasks` node in the **Object Explorer** and choose :guilabel:`New` - :guilabel:`Bulk Copy`, you will be presented with the following interface:

.. image:: content/bulkcopyui.png

Then perform the following tasks:

 1. Add the connections that will be participating in this bulk copy operation
 2. Add any number of tasks. For each task.
 
   2.1. Specify the `source feature class` and `target feature class`
   2.2. Map the source properties to their target properties. Optionally configure the data conversion rules for this mapping
   2.3. Optionally, define computed columns with FDO expressions and map them to any target property. Optionally configure the data conversion rules for this mapping.
   2.4. Optionally, configure the options for this particular copy task.
  
 3. Save the task
 
Once saved, a new node will be visible under :guilabel:`Tasks` in the **Object Explorer**, you can right click that node to:

 * Execute the task
 * Save the task
 * Edit the task
 
Saving the task will create an XML file that describes the bulk copy operation. This saved task can be run from the command-line using `FdoUtil.exe` with the `RunTask` command.

If you load a saved task, it will automatically load any participating connections in the Object Explorer if they don't already exist.

SQL Server Notes
----------------

An important note about bulk copying to SQL Server. If the geometric property refers to a spatial context with a geodetic (lat/long) coordinate system, inserts into SQL Server containing polygon geometries may fail if the ring orientation is not correct. Unfortunately there is no mechanism in FDO or the bulk copy API to fix the ring orientation. As such, avoid using FDO Toolbox for copying such geometries to SQL Server if:

 * Your SQL Server Data Store uses a geodetic coordinate system
 * Your source polygons have invalid ring orientation. SQL Server expects: counter-clockwise exterior rings and clockwise interior rings.
 
Note that this only applies to polygon geometries. Other geometry types are unaffected.

Custom Operations
-----------------

If bulk copy and joins do not satisfy your data transformation needs, you can assemble your own custom operations using the Core API. Custom operations can only be defined in code. There
is no user interface design support.

A data transfomration process can be viewed as a series of connected operations that execute in sequence. In fact, a bulk copy is just a specialisation of this data transformation process. 
To define your own data transformation processes, create a new `EtlProcess` instance and register a bunch of `IFdoOperation` instances in the order you want them to be executed. Although it 
is not enforced by the ETL framework, the first operation should be a `FdoInputOperation` and the last operation should be a `FdoOutputOperation`. The Core API provides the following operations:

 * `FdoBatchedOutputOperation`
 * `FdoBranchingOperation`
 * `FdoCopySpatialContextOperation`
 * `FdoDataValueConversionOperation`
 * `FdoDeleteOperation`
 * `FdoFeatureTableInputOperation`
 * `FdoFilteredOperation`
 * `FdoFlattenGeometryOperation` 
 * `FdoInputOperation`
 * `FdoJoinOperation`
 * `FdoForceWkbOperation`
 * `FdoSingleActionOperation`
 * `FdoCreateDataStoreOperation`
 * `FdoApplySchemaOperation`
 * `FdoOutputOperation`
 
You can define new operations by implementing the `IFdoOperation` interface or alternatively to derive from the `FdoOperationBase` and implement the `Execute` method.