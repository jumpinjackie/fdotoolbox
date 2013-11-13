.. index::
   single: Command Line Utilities

Command Line Utilities
======================

The following command-line utilities are included with FDO Toolbox:

 * FdoUtil.exe
 * FdoInfo.exe
 
General Notes
-------------

Where applicable, the ``-test`` switch performs a FDO capability check to determine if 
execution can go ahead.

Where applicable, the ``-quiet`` switch will suppress all console output. This is not 
entirely the case however for debug builds of the command-line utilities

If a given parameter value contains multiple words, enclose the string with quotes::

 eg. -description:"This is a multi word parameter value"

All command-line utilities will return 0 for successful execution and will return a non-zero value
otherwise. Consult :ref:`cmdline-error-codes` for the list of return codes.

For commands that require a connection string parameter (see below) the connection 
string is of the following format::

 [Name1]=[Value1];[Name2]=[Value2];...

Some examples:

SDF::

 -connection:File=C:\Test\Test.sdf
 
SHP::

 -connection:DefaultFileLocation=C:\Test\Test.shp
 
MySQL::

 -connection:Username=root;Password=1234;Service=localhost:3306;DataStore=mydatastore

.. index::
   single: Command Line Utilities; FdoUtil.exe

FdoUtil.exe
-----------

The general invocation for ``FdoUtil.exe`` is as follows:::

 FdoUtil.exe -cmd:<command name> [-quiet] [-test] <command parameters>

The valid list of commands for ``FdoUtil.exe`` include:
 * ``ApplySchema``
 * ``CreateDataStore``
 * ``Destroy``
 * ``DumpSchema``
 * ``CreateFile``
 * ``RegisterProvider``
 * ``UnregisterProvider``
 * ``BulkCopy``
 * ``RunTask``

Each command for ``FdoUtil.exe`` is described in further detail below:

**ApplySchema**

Description: Applies a schema definition xml file to a FDO data source

Usage::

 FdoUtil.exe -cmd:ApplySchema -file:<schema definition file> -provider:<provider> -connection:<connection string> [-quiet] [-test]

Notes: This will only work on providers that implement the IApplySchema command. 

**Destroy**

Description: Destroys a datastore in an FDO data source

Usage::
 
 FdoUtil.exe -cmd:Destroy -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-quiet]

Notes: 

The ``-properties`` parameter is a the same format as the connection string

eg. For OSGeo.MySQL:: 

 -properties:DataStore=mydatastore

The ``-connection`` parameter is only required for rdbms-based providers. Usually RDBMS-based 
providers require a ``DataStore`` parameter as part of the connection. This is not required
in this case.

**DumpSchema**

Description: Writes a schema in a FDO data store to an XML file

Usage::

 FdoUtil.exe -cmd:DumpSchema -file:<schema file> -provider:<provider> -connection:<connection string> -schema:<selected schema> [-quiet]

Notes: n/a

**CreateFile**

Description: Creates a new flat-file data source with the option of applying a schema to it.

Usage::

 FdoUtil.exe -cmd:CreateFile -path:<path to sdf file> [-schema:<path to schema file>] [-quiet]

Notes: 

If the ``-schema`` parameter points to an non-existent file or is not valid, schema application will not take place.

If the file is a shp file, the ``-schema`` parameter must be defined

**CreateDataStore**

Description: Create a new FDO data store

Usage::

 FdoUtil.exe -cmd:CreateDataStore -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-quiet] [-test]

Notes:

This command is exclusively for creating data stores in RDBMS-based providers. Use the `CreateFile` command if working with flat-file providers

The ``-properties`` parameter is a the same format as the connection string 

eg. For OSGeo.MySQL:: 

 -properties:DataStore=mydatastore

The ``-connection`` parameter is only required for rdbms-based providers. Usually rdbms-based 
providers require a DataStore parameter as part of the connection. This is not required
in this case.

**RegisterProvider**

Description: Registers a new FDO provider

Usage::

 FdoUtil.exe -cmd:RegisterProvider -name:<Provider Name> -displayName:<Display Name> -description:<description> -libraryPath:<Path to provider dll> -version:<version> -fdoVersion:<fdo version> -isManaged:<true|false>

Notes: n/a

**UnregisterProvider**

Description: Unregisters a FDO provider

Usage::

 FdoUtil.exe -cmd:UnregisterProvider -name:<Provider Name>

Notes: The provider name must be fully qualified (including version number) otherwise un-registration will fail.

**BulkCopy**

Description: Copies data from an FDO data source to a flat-file FDO data source

Usage::

 FdoUtil.exe -cmd:BulkCopy -src_provider:<provider name> -src_conn:<connection string> -dest_path:<path to file or directory> -src_schema:<source schema name> [-src_classes:<comma-separated list of class names>] [-copy_srs:<source spatial context name>] [-quiet]

Notes: When ``-dest_path`` is a directory, it is assumed SHP is the output format, otherwise the output format is determined by file extension given

Valid file extensions include: 
 * sdf (OSGeo.SDF)
 * sqlite (OSGeo.SQLite)
 * db (OSGeo.SQLite)
 
**RunTask**

Description: Executes a pre-defined task definition

Usage:: 

 FdoUtil.exe -cmd:RunTask -task:<path to task definition>

Notes: The task definition must be a valid Bulk Copy or Join definition. Bulk Copy tasks must have a ``.BulkCopyDefinition`` extension and Join tasks must have a ``.JoinDefinition`` extensions.

``RunTask`` will make no attempts to validate the connections defined within these tasks.

.. index::
   single: Command Line Utilities; FdoInfo.exe

FdoInfo.exe
-----------

The general invocation for ``FdoInfo.exe`` is as follows:::

 FdoInfo.exe -cmd:<command name> <command parameters>

The valid list of command for ``FdoInfo.exe`` include:
 * ``GetConnectionParameters``
 * ``GetCreateDataStoreParameters``
 * ``GetDestroyDataStoreParameters``
 * ``ListClasses``
 * ``ListClassProperties``
 * ``ListDataStores``
 * ``ListProviders``
 * ``ListSchemas``
 * ``ListSpatialContexts``

Each command for FdoInfo.exe is described in further detail below:

**GetConnectionParameters**

Description: Gets and displays the connection parameters for a given provider

Usage:: 

 FdoInfo.exe -cmd:GetConnectionParameters -provider:<provider name>

Notes: n/a

**GetCreateDataStoreParameters**

Description: Gets and displays the parameters required to create a Data Store for a given provider

Usage::

 FdoInfo.exe -cmd:GetCreateDataStoreParameters -provider:<provider name>

Notes: Only works for providers that support the ``ICreateDataStore`` FDO command.

**GetDestroyDataStoreParameters**

Description: Gets and displays the parameters required to destroy a Data Store for a given provider

Usage::

 FdoInfo.exe -cmd:GetDestroyDataStoreParameters -provider:<provider name>

Notes: Only works for providers that support the ``IDestroyDataStore`` FDO command.

**ListClasses**

Description: Displays the feature classes under a given feature schema

Usage::

 FdoInfo.exe -cmd:ListClasses -provider:<provider name> -connection:<connection string> -schema:<schema name>

Notes: n/a

**ListClassProperties**

Description: Displays the properties under a given feature class

Usage::

 FdoInfo.exe -cmd:ListClassProperties -provider:<provider name> -connection:<connection string> -schema:<schema name> -class:<class name>

Notes: n/a

**ListDataStores**

Description: Displays the data stores of a given connection

Usage::

 FdoInfo.exe -cmd:ListDataStores -provider:<provider name> -connection:<connection string> [-fdoOnly]

Notes: 

This only works for providers that support the ``IListDataStores`` FDO command.

If the ``-fdoOnly`` switch is supplied, it will display only fdo-enabled datastores.

**ListProviders**

Description: Gets and displays all the registerd FDO providers

Usage::

 FdoInfo.exe -cmd:ListProviders

Notes: n/a

**ListSchemas**

Description: Displays the feature schemas for a given connection

Usage::

 FdoInfo.exe -cmd:ListSchemas -provider:<provider name> -connection:<connection string>

Notes: n/a

**ListSpatialContexts**

Description: Displays the defined spatial contexts in a given connection

Usage::

 FdoInfo.exe -cmd:ListSpatialContexts -provider:<provider name> -connection:<connection string>

Notes: n/a

.. index::
   single: Command Line Utilities; Error Codes

.. _cmdline-error-codes:

Error Codes
-----------

::

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
        /// Unknown failure
        /// </summary>
        E_FAIL_UNKNOWN
    }