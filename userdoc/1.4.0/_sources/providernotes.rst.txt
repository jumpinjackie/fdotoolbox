.. _provider-notes:

Provider-specific Notes
=======================

Although FDO aims to be unified spatial data access abstraction layer, some implementation details sadly to leak out to FDO client applications like FDO Toolbox.

This section describes quirks when working with various providers.

.. _shp-constraints:

SHP Provider
------------

 * Geometric Properties can only support one particular Geometric Type. You cannot combine multiple Geometric Types. Eg. You can only have Point, Curve or Surface. You cannot have combinations of the 3.
 * SHP supports a limited set of data types. Take note of this when copying data to SHP or applying a schema.
 * You can only apply a schema in SHP directory connection mode. It does not work when connecting to a single file.

.. _kgora-constraints:

King Oracle Provider
--------------------

 * If applying a new schema, you will have to remove and rebuild the connection to see the changes.
 * Spatial contexts created through FDO only live in-memory within the provider
    * The reason for this is that the provider only cares about the name of the spatial context, which is assumed to be in the format of `OracleSridXXXX` where XXXX is the SRID. From this SRID, all other information (WKT, etc) can be inferred from.
    * In a bulk copy context, you are strongly advised to always override the source name (to use the `OracleSridXXXX` notation) or nominate an existing target spatial context for any feature class to be created
 * Applying schemas will **only create feature classes**. It will not update or delete classes.
    * For this reason, you should prepare all necessary tables up-front through your normal preferred Oracle DB admin tools and set up your bulk copy to copy into these existing tables
    * Also for this reason, using the `copy-class` command to bulk copy data into Oracle is also discouraged, you should just use FdoCmd to executed a bulk copy definition prepared earlier
 * Bulk copying data is slow. Although the provider supports batch inserting (which would theoretically improve throughput), the provider's implementation has bugs that prevent us from reliably using it.
    * As a result, we recommend only using FDO Toolbox for bulk copying into Oracle as a last resort when all other options are not possible.

.. _slt-constraints:

SQLite Provider
---------------

 * The schema name is always locked to **Default**. Take note of this when setting up bulk copies to SQLite.
 * Due to buggy refcounting, or the usage of an API in the provider that has buggy refcounting, this provider exhibits instability when used in FdoCmd. We recommend only interacting with such data stores through the FDO Toolbox windows application.

.. _mysql-constraints:

MySQL Provider
--------------

 * Data stores created through this provider will always have FDO meta-schema tables defined (these are tables starting with `f_`). In other words, data stores created by this provider are fully-managed by FDO and difficult to interop with other tools (that do not know what these FDO meta-schema tables are)
 * MySQL provider will report a reduced set of spatial capabilities when connecting to a server older than 5.6

.. _postgis-constraints:

PostGIS Provider
----------------

 * If applying a new schema, you will have to remove and rebuild the connection to see the changes.

.. _sqlserver-constraints:

SQL Server Spatial (2008) Provider
----------------------------------

 * This provider is extremely sensitive to geometry validity and will reject any input of invalid geometries, FDO itself does not provide any form of geometry validation. This mostly occurs when copying polygon geometries (with invalid ring orientation) to a feature class that uses a spatial context with a geodetic (lat/long) coordinate system. Take note of this when bulk copying to sql server.
 * Bulk copying to this provider is quite slow. Use alternate means of copying data if possible.
 * You cannot create spatial contexts whose WKT does not resolve to a matching entry in the `sys.spatial_reference_systems` system table
    * You must specify either:
    
        1. A CS name that is present in `com/ExtendedCoordSys.txt` or a name such that the following SQL query will produce a result:

        `SELECT * FROM sys.spatial_reference_systems WHERE SUBSTRING( well_known_text, 9, CHARINDEX('"',well_known_text,9) - 9 ) = <name>`

        2. Or, a WKT that is present in `com/ExtendedCoordSys.txt` or a WKT such that the following SQL query will produce a result:

        `SELECT * FROM sys.spatial_reference_systems WHERE well_known_text = <WKT>`

    * In the context of bulk copying, you can use the SC override feature to "fix" such bad source spatial contexts from being copied.