.. index::
   single: Command Line Utilities

Command Line Utilities
======================

FDO Toolbox ships with a command-line utility ``FdoCmd.exe``

``FdoCmd.exe`` replaces the previously included ``FdoInfo.exe`` and ``FdoUtil.exe`` utilities
 
Basic Usage
-----------

Invocation is generally of the form of:::

 FdoCmd.exe <verb> <arguments>

Where ``<verb>`` is any of the following:::

   apply-schema                     Applies the given schema to the data store

   copy-class                       Copies a feature class (and its data) from a source connection to the target

   copy-to-file                     Bulk copies one or more feature classes from the source connection to a target
                                    SHP/SDF/SQLite file

   create-datastore                 Creates a data store against the given FDO connection

   create-file                      Creates a file-based data store. Only applies to SHP/SDF/SQLite FDO providers

   create-spatial-context           Create a spatial context (or update wherever updates are supported by the underlying
                                    provider)

   destroy-datastore                Destroys a data store against the given FDO connection

   destroy-spatial-context          Destroys a spatial context

   dump-schema                      Dumps the specified schema for the given FDO connection

   execute-sql-query                Executes a SQL query

   execute-sql-command              Executes a non-select SQL command

   get-class-extent                 Gets the extent of the given feature class

   get-feature-count                Gets the total number of features in the given feature class

   list-bcp-tasks                   Lists bulk copy tasks in the givne bulk copy definition file

   list-classes                     Lists classes for the given schema

   list-class-properties            Lists properties for the given class

   list-connection-params           Lists connection parameters for the given FDO provider

   list-create-datastore-params     List all available parameters for data store creation

   list-datastores                  Lists datastores for the given connection

   list-destroy-datastore-params    List all available parameters for data store destruction

   list-providers                   Gets all registered FDO providers

   list-schemas                     Lists schemas for the given connection

   list-spatial-contexts            Gets spatial contexts for the given connection

   query-aggregates                 Queries for aggregations of feature data from the given data store

   query-features                   Queries features from the given data store

   register-provider                Register a FDO provider

   run-task                         Runs a task from a definition file

   unregister-provider              Unregisters a FDO provider

   cs-code-to-wkt                   Converts the given mentor CS code to WKT

   enumerate-cs-categories          Enumerates coordinate system categories

   enumerate-cs                     Enumerates coordinate systems for a given category

   epsg-to-wkt                      Converts the given EPSG code to its coordinate system WKT

   find-cs-by-code                  Looks up the coordinate system for the given CS code and outputs details about the
                                    coordinate system

   find-cs-by-epsg                  Looks up the coordinate system for the given EPSG code and outputs details about the
                                    coordinate system

   is-valid-wkt                     Checks if the given coordinate system WKT is valid

   wkt-to-cs-code                   Converts the given coordinate system WKT to its mentor CS code

   wkt-to-epsg                      Converts the given coordinate system WKT to its EPSG code

   help                             Display more information on a specific command.

   version                          Display version information.

General Notes
-------------

Where commands require connection details, 2 methods of connection are supported:

 1. By specifying a provider and connection parameters
 2. (For file-based providers), by specifying a file path

For example, connecting to a SHP file can be done this way:::

 --provider OSGeo.SHP --connect-params DefaultFileLocation C:\Path\To\MyShapeFile.shp

Or, because SHP is a file-based provider, it can be connected by file path:::

 --from-file C:\Path\To\MyShapeFile.shp

Some commands offer a file-based fallback as a parameter you may have to define may be a bit tricky to define if the value contains quotes or double quotes

 * ``execute-sql-query`` allows you to specify either the SQL inline or a path to a file containing the SQL query for the ``--sql`` parameter
 * ``execute-sql-command`` allows you to specify either the SQL inline or a path to a file containing the SQL query for the ``--sql`` parameter
 * For ``copy-class`` if it is not suitable to pass a coordinate system WKT through ``--override-sc-wkt``, you may instead specify a path to a file containing the coordinate system WKT through ``--override-sc-wkt-from-file``
 * For ``create-spatial-context`` if it is not suitable to pass a coordinate system WKT through ``--cs-wkt``, you may instead specify a path to a file containing the coordinate system WKT through ``--cs-wkt-from-file``

Powershell Scripting Notes
--------------------------

One of the main design goals of ``FdoCmd.exe`` is to better faciliate its use in shell scripting scenarios

 * Output produced by ``FdoCmd.exe`` are easily captured by shell scripts
 * Output produced by ``FdoCmd.exe`` can easily feed into other ``FdoCmd.exe`` commands

On Windows, the predominant shell scripting environment is Powershell and the output that ``FdoCmd.exe`` produces is allows for easy integration with any Powershell scripts you may have.

This section covers some general patterns you can apply to any powershell scripts using ``FdoCmd.exe``

Example: Iterating and acting upon each feature class
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

The `list-classes` command outputs the name of each feature class line-by-line. When captured into a variable in powershell, this is array you can loop over::

  $classes = & .\FdoCmd.exe list-classes --provider OSGeo.SHP --connect-params DefaultFileLocation D:\fdo-trunk\Providers\SHP\TestData\Sheboygan
  foreach ($cls in $classes)
  {
     # Do something with $cls
     Write-Host "Doing something with $cls"
  }

Example: Split each feature out to a separate GeoJSON file
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Some commands in ``FdoCmd.exe`` allow for CSV output, which combined with the ``ConvertFrom-CSV`` powershell cmdlet allows you to capture ``FdoCmd.exe`` output as an array of objects which can be further acted upon, as demonstrated by this example::

  $className = "my_feature_class"
  $srcPath="C:\Path\To\My\Shapefile.shp"
  $featIds = & .\FdoCmd.exe query-features --from-file $srcPath --class $className --properties FeatId --computed-properties FileName "concat('Feature_', FeatId)" --format CSV | ConvertFrom-CSV
  foreach ($item in $featIds) {
     $fid = $item.FeatId
     $fn = $item.FileName
     $outPath = "C:\Path\To\My\Output\$fn.geojson"
     & .\FdoCmd.exe query-features --from-file $srcPath --class $className --filter "FeatId = $fid" --format GeoJSON | Out-File -FilePath $outPath
     Write-Host "Saved: $outPath"
  }

.. index::
   single: Command Line Utilities; Error Codes

.. _cmdline-error-codes:

Error Codes
-----------

By design all ``FdoCmd.exe`` commands exit with code ``0`` to indicate succesfull operation. For any non-zero value, it corresponds to the following values below::

    /// <summary>
    /// Status codes that can be returned by any console application
    /// </summary>
    public enum CommandStatus : int
    {
        /// <summary>
        /// Operation OK, no errors encounters
        /// </summary>
        E_OK = 0,
        /// <summary>
        /// Failed to create SDF
        /// </summary>
        E_FAIL_SDF_CREATE = 1,
        /// <summary>
        /// Failed to apply schema
        /// </summary>
        E_FAIL_APPLY_SCHEMA = 2,
        /// <summary>
        /// Failed to destory datastore
        /// </summary>
        E_FAIL_DESTROY_DATASTORE = 3,
        /// <summary>
        /// Failed to connect
        /// </summary>
        E_FAIL_CONNECT = 4,
        /// <summary>
        /// Failed to serialize a feature schema
        /// </summary>
        E_FAIL_SERIALIZE_SCHEMA_XML = 5,
        /// <summary>
        /// Failed to create datastore
        /// </summary>
        E_FAIL_CREATE_DATASTORE = 6,
        /// <summary>
        /// Failed to bulk copy
        /// </summary>
        E_FAIL_BULK_COPY = 7,
        /// <summary>
        /// Task validation failed
        /// </summary>
        E_FAIL_TASK_VALIDATION = 8,
        /// <summary>
        /// Failed to create connection
        /// </summary>
        E_FAIL_CREATE_CONNECTION = 9,
        /// <summary>
        /// Failed to find intended schema
        /// </summary>
        E_FAIL_SCHEMA_NOT_FOUND = 10,
        /// <summary>
        /// Failed to find intended class
        /// </summary>
        E_FAIL_CLASS_NOT_FOUND = 11,
        /// <summary>
        /// The given capability is not supported
        /// </summary>
        E_FAIL_UNSUPPORTED_CAPABILITY = 12,
        /// <summary>
        /// The query results failed to load
        /// </summary>
        E_FAIL_LOAD_QUERY_RESULTS = 13,
        /// <summary>
        /// The task definition being attempted to load is not a recognised format
        /// </summary>
        E_FAIL_UNRECOGNISED_TASK_FORMAT = 14,
        /// <summary>
        /// Bulk Copy ran with errors logged
        /// </summary>
        E_FAIL_BULK_COPY_WITH_ERRORS = 15,
        /// <summary>
        /// Join operation ran with errors logged
        /// </summary>
        E_FAIL_JOIN_WITH_ERRORS = 16,
        /// <summary>
        /// The specified SQL statement was a SELECT statement
        /// </summary>
        E_FAIL_INVALID_SQL = 17,
        /// <summary>
        /// The specified SQL statement threw an exception during execution
        /// </summary>
        E_FAIL_SQL_EXECUTION_ERROR = 18,
        /// <summary>
        /// The specified provider does not support SQL commands
        /// </summary>
        E_FAIL_SQL_COMMAND_NOT_SUPPORTED = 19,
        /// <summary>
        /// The command was invoked with invalid or incomplete arguments
        /// </summary>
        E_FAIL_INVALID_ARGUMENTS = 20,
        /// <summary>
        /// A RunTask command was executed with a bcptask parameter specifying
        /// a copy task that doesn't exist
        /// </summary>
        E_FAIL_MISSING_BULK_COPY_TASKS = 21,
        /// <summary>
        /// An I/O error occured
        /// </summary>
        E_FAIL_IO_ERROR = 22,
        /// <summary>
        /// Unknown failure
        /// </summary>
        E_FAIL_UNKNOWN = 23,
        /// <summary>
        /// Not supported
        /// </summary>
        E_NOT_SUPPORTED = 24,
        /// <summary>
        /// No data
        /// </summary>
        E_NO_DATA = 25,
        /// <summary>
        /// Error setting up the bulk copy task
        /// </summary>
        E_FAIL_BULK_COPY_SETUP = 26,
        /// <summary>
        /// The specified coordinate system could not be found
        /// </summary>
        E_FAIL_CS_NOT_FOUND = 27,
        /// <summary>
        /// One or more required command line arguments was not specified
        /// </summary>
        E_FAIL_MISSING_CMD_OPTIONS = 28
    }