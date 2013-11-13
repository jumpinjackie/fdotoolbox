FdoToolbox Command-line Utilities readme
----------------------------------------

Contents
--------
1. Introduction [A1]
2. FdoUtil.exe command description [B1]
3. FdoInfo.exe command description [B2]


Introduction [A1]
-----------------

The following command-line utilities are included with FdoToolbox:

- FdoUtil.exe
- FdoInfo.exe

The general invocation is as follows:

FdoUtil.exe -cmd:<command name> [-quiet] [-test] <command parameters>
FdoInfo.exe -cmd:<command name> <command parameters>

The valid list of commands for FdoUtil.exe include:
- ApplySchema
- CreateDataStore
- Destroy
- DumpSchema
- CreateFile
- RegisterProvider
- UnregisterProvider
- BulkCopy
- RunTask
- ExecuteSql

The valid list of command for FdoInfo.exe include:
- GetConnectionParameters
- GetCreateDataStoreParameters
- GetDestroyDataStoreParameters
- ListBcpTasks
- ListClasses
- ListClassProperties
- ListDataStores
- ListProviders
- ListSchemas
- ListSpatialContexts

Where applicable, the -test switch performs a FDO capability check to determine if 
execution can go ahead.

Where applicable, the -quiet switch will suppress all console output. This is not 
entirely the case however for debug builds of the command-line utilities

If a given parameter value contains multiple words, enclose the string with quotes.

eg. -description:"This is a multi word parameter value"

All command-line utilities will return 0 for successful execution and will return a non-zero value
otherwise. Consult CommandStatus.cs for the list of return codes.

For commands that require a connection string parameter (see below) the connection 
string is of the following format:

[Name1]=[Value1];[Name2]=[Value2];...

Some examples:

SDF: -connection:File=C:\Test\Test.sdf
SHP: -connection:DefaultFileLocation=C:\Test\Test.shp
MySQL: -connection:Username=root;Password=1234;Service=localhost:3306;DataStore=mydatastore

======= FdoUtil.exe command description ======= [B1]

Each command for FdoUtil.exe is described in further detail below:

ApplySchema
-----------

Description: Applies a schema definition xml file to a FDO data source

Usage: FdoUtil.exe -cmd:ApplySchema -file:<schema definition file> -provider:<provider> -connection:<connection string> [-quiet] [-test]

Notes: This will only work on providers that implement the IApplySchema command. 

Destroy
-------

Description: Destroys a datastore in an FDO data source

Usage: FdoUtil.exe -cmd:Destroy -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-quiet]

Notes: 

The -properties parameter is a the same format as the connection string
eg. For OSGeo.MySQL: -properties:DataStore=mydatastore

The -connection parameter is only required for rdbms-based providers. Usually rdbms-based 
providers require a DataStore parameter as part of the connection. This is not required
in this case.

DumpSchema
----------

Description: Writes a schema in a FDO data store to an XML file

Usage: FdoUtil.exe -cmd:DumpSchema -file:<schema file> -provider:<provider> -connection:<connection string> -schema:<selected schema> [-quiet]

Notes: n/a

CreateFile
----------

Description: Creates a new flat-file data source with the option of applying a schema to it.

Usage: FdoUtil.exe -cmd:CreateFile -path:<path to sdf file> [-schema:<path to schema file>] [-quiet]

Notes: 

- If the -schema parameter points to an non-existent file or is not valid, schema
application will not take place.

- If the file is a shp file, the -schema parameter must be defined

- Valid file formats include: SDF, SHP and SQLite. SQLite can be any of the following file extensions: .sqlite, .db, .slt

CreateDataStore
---------------

Description: Create a new FDO data store

Usage: FdoUtil.exe -cmd:CreateDataStore -provider:<provider> -properties:<data store properties> -connection:<connection string> [-quiet] [-test]

Notes:

This is for rdbms-based providers only. For flat-file providers, use the CreateFile command

The -properties parameter is a the same format as the connection string
eg. For OSGeo.MySQL: -properties:DataStore=mydatastore

RegisterProvider
----------------

Description: Registers a new FDO provider

Usage: FdoUtil.exe -cmd:RegisterProvider -name:<Provider Name> -displayName:<Display Name> -description:<description> -libraryPath:<Path to provider dll> -version:<version> -fdoVersion:<fdo version> -isManaged:<true|false>

Notes: n/a

UnregisterProvider
------------------

Description: Unregisters a FDO provider

Usage: FdoUtil.exe -cmd:UnregisterProvider -name:<Provider Name>

Notes: The provider name must be fully qualified (inc. version number) otherwise un-registration will fail.

BulkCopy
--------

Description: Copies data from an FDO data source to a flat-file FDO data source

Usage: FdoUtil.exe -cmd:BulkCopy -src_provider:<provider name> -src_conn:<connection string> -dest_path:<path to file or directory> -src_schema:<source schema name> [-src_classes:<comma-separated list of class names>] [-copy_srs:<source spatial context name>] [-log:<custom log file name>] [-flatten] [-quiet]

Notes: 

When -dest_path is a directory, it is assumed SHP is the output format, otherwise the output format is determined by file extension given
If -flatten is defined, any geometries copied will have any Z and/or M coordinates stripped away.

If the -log parameter is specified, the error log will be saved using this name. If omitted, a system-generated name will be used.

Valid file extensions include: 
	- sdf (OSGeo.SDF)
	- sqlite (OSGeo.SQLite)
	- db (OSGeo.SQLite)
	
RunTask
-------

Description: Executes a pre-defined task definition

Usage: FdoUtil.exe -cmd:RunTask -task:<path to task definition> [-bcptask:<name of task within bulk copy definition>] [-log:<custom log file name>]

Notes: The task definition must be a valid Bulk Copy or Join definition. Bulk Copy tasks must have a .BulkCopyDefinition extension and Join tasks must have a .JoinDefinition extensions.

RunTask will make no attempts to validate the connections defined within these tasks.

The -bcptask option if specified will only execute the specified task within the bulk copy definition instead of all the tasks (default). If the task is not found, an error is returned.

The -bcptask option does not apply to join tasks. Specifying this option with a join task definition will return an error.

If the -log parameter is specified, the error log will be saved using this name. If omitted, a system-generated name will be used.

ExecuteSql
----------

Description: Executes a non-SELECT SQL statement

Usage: FdoUtil.exe -cmd:ExecuteSql -provider:<fdo provider> -connection:<connection string> -sql:<SQL statement>

Notes: This command only works for FDO providers that support SQL statements. Assuming a stock FDO distribution bundled with FDO Toolbox, then the providers which support SQL statements are:

- OSGeo.MySQL
- OSGeo.ODBC
- OSGeo.PostgreSQL
- OSGeo.SQLServerSpatial
- King.Oracle
- OSGeo.SQLite

FdoUtil.exe does not validate the SQL statement for any syntax errors.

======= FdoInfo.exe command description ======= [B2]

Each command for FdoInfo.exe is described in further detail below:

GetConnectionParameters
-----------------------

Description: Gets and displays the connection parameters for a given provider

Usage: FdoInfo.exe -cmd:GetConnectionParameters -provider:<provider name>

Notes: n/a

GetCreateDataStoreParameters
----------------------------

Description: Gets and displays the parameters required to create a Data Store for a given provider

Usage: FdoInfo.exe -cmd:GetCreateDataStoreParameters -provider:<provider name>

Notes: Only works for providers that support ICreateDataStore

GetDestroyDataStoreParameters
-----------------------------

Description: Gets and displays the parameters required to destroy a Data Store for a given provider

Usage: FdoInfo.exe -cmd:GetDestroyDataStoreParameters -provider:<provider name>

Notes: Only works for providers that support IDestroyDataStore

ListBcpTasks
------------

Description: Lists all available copy tasks for execution in a bulk copy definition file

Usage: FdoInfo.exe -cmd:ListBcpTasks -task:<path to bulk copy definition>

Notes: Use in conjunction with the -bcptask parameter of the FdoUtil.exe RunTask command to selectively execute certain copy tasks from a bulk copy definition

ListClasses
-----------

Description: Displays the feature classes under a given feature schema

Usage: FdoInfo.exe -cmd:ListClasses -provider:<provider name> -connection:<connection string> -schema:<schema name>

Notes: n/a

ListClassProperties
-------------------

Description: Displays the properties under a given feature class

Usage: FdoInfo.exe -cmd:ListClassProperties -provider:<provider name> -connection:<connection string> -schema:<schema name> -class:<class name>

Notes: n/a

ListDataStores
--------------

Description: Displays the data stores of a given connection

Usage: FdoInfo.exe -cmd:ListDataStores -provider:<provider name> -connection:<connection string> [-fdoOnly]

Notes: 
Only works for providers that support IListDataStores
If the -fdoOnly switch is supplied, it will display only fdo-enabled datastores.

ListProviders
-------------

Description: Gets and displays all the registerd FDO providers

Usage: FdoInfo.exe -cmd:ListProviders

Notes: n/a

ListSchemas
-----------

Description: Displays the feature schemas for a given connection

Usage: FdoInfo.exe -cmd:ListSchemas -provider:<provider name> -connection:<connection string>

Notes: n/a

ListSpatialContexts
-------------------

Description: Displays the defined spatial contexts in a given connection

Usage: FdoInfo.exe -cmd:ListSpatialContexts -provider:<provider name> -connection:<connection string>

Notes: n/a
