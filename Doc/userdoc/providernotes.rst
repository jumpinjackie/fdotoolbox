.. _provider-notes:

Provider-specific Notes
=======================

Although FDO aims to be unified spatial data access abstraction layer, some implementation details sadly to leak out to FDO client applications like FDO Toolbox.

This section describes quirks when working with various providers.

General
-------

When applying a schema (for providers that support the capability), FDO Toolbox will modify the schema in-place to "promote" any unsupported data type to a data type that is supported by the target provider.

The precedence by which unsupported data types are "promoted" is indicated in the table below:

+-----------+----------------------------------------------------------------+
| Data Type | Promoted to (first supported data type in left -> right order) |
+===========+================================================================+
| BLOB      | N/A (Exception is thrown)                                      |
+-----------+----------------------------------------------------------------+
| Boolean   | Byte -> Int16 -> Int32 -> Int64 -> String -> Exception         |
+-----------+----------------------------------------------------------------+
| Byte      | Int16 -> Int32 -> Int64 -> String -> Exception                 |
+-----------+----------------------------------------------------------------+
| BLOB      | N/A (Exception is thrown)                                      |
+-----------+----------------------------------------------------------------+
| DateTime  | String -> Exception                                            |
+-----------+----------------------------------------------------------------+
| Decimal   | Double -> String -> Exception                                  |
+-----------+----------------------------------------------------------------+
| Double    | String -> Exception                                            |
+-----------+----------------------------------------------------------------+
| Int16     | Int32 -> Int64 -> String -> Exception                          |
+-----------+----------------------------------------------------------------+
| Int32     | Int64 -> String -> Exception                                   |
+-----------+----------------------------------------------------------------+
| Int64     | String -> Exception                                            |
+-----------+----------------------------------------------------------------+
| Single    | Double -> String -> Exception                                  |
+-----------+----------------------------------------------------------------+
| String    | N/A (Exception is thrown)                                      |
+-----------+----------------------------------------------------------------+

For auto-generated properties, we do things slightly differently. In this case, we will attempt to promote the data type as usual against the provider's list of supported auto-generated data types. When we cannot promote the data type, we will go on the opposite direction and demote the data type until we find a matching supported auto-generated data type.

The rationale for this is if the source has an int64 auto-generated property, but the target provider only supports int32 auto-generated properties, it should be okay to demote the property down to int32 because unless you need to copy more than 2^32 features, this demotion should not have any effect on the overall bulk copy.

.. _shp-constraints:

SHP Provider
------------

 * Geometric Properties can only support one particular Geometric Type. You cannot combine multiple Geometric Types. Eg. You can only have Point, Curve or Surface. You cannot have combinations of the 3.
   * As a result, this makes the SHP provider unsuitable as a target for some cases of the ``copy-class`` command as it cannot apply a source class if its geometry supports more than one geometry type. A workaround in such cases is to apply a fixed schema (with single-type geometry properties) to the SHP target connection first before doing the ``copy-class``, which will bypass automatic class creation due to them existing as a result of applying the schema beforehand.
 * SHP supports a limited set of data types. Take note of this when copying data to SHP or applying a schema.
 * You can only apply a schema in SHP directory connection mode. It does not work when connecting to a single file.

.. _kgora-constraints:

King Oracle Provider
--------------------

 * If applying a new schema, you will have to remove and rebuild the connection to see the changes.
 * Spatial contexts created through FDO only live in-memory within the provider
    * The reason for this is that the provider only cares about the name of the spatial context, which is assumed to be in the format of ``OracleSridXXXX`` where XXXX is the SRID. From this SRID, all other information (WKT, etc) can be inferred from.
    * In a bulk copy context, you are strongly advised to always override the source name (to use the ``OracleSridXXXX`` notation) or nominate an existing target spatial context for any feature class to be created
 * Applying schemas will **only create feature classes**. It will not update or delete classes.
    * For this reason, you should prepare all necessary tables up-front through your normal preferred Oracle DB admin tools and set up your bulk copy to copy into these existing tables
    * Also for this reason, using the ``copy-class`` command to bulk copy data into Oracle is also discouraged, you should just use FdoCmd to executed a bulk copy definition prepared earlier
 * Bulk copying data is slow. Although the provider supports batch inserting (which would theoretically improve throughput), the provider's implementation has bugs that prevent us from reliably using it.
    * As a result, we recommend only using FDO Toolbox for bulk copying into Oracle as a last resort when all other options are not possible.

.. _slt-constraints:

SQLite Provider
---------------

 * The schema name is always locked to **Default**. Take note of this when setting up bulk copies to SQLite.

.. _mysql-constraints:

MySQL Provider
--------------

 * Data stores created through this provider will always have FDO meta-schema tables defined (these are tables starting with ``f_``). In other words, data stores created by this provider are fully-managed by FDO and difficult to interop with other tools (that do not know what these FDO meta-schema tables are)
 * MySQL provider will report a reduced set of spatial capabilities when connecting to a server older than 5.6

.. _postgis-constraints:

PostGIS Provider
----------------

 * If applying a new schema, you will have to remove and rebuild the connection to see the changes.

.. _sqlserver-constraints:

SQL Server Spatial (2008+) Provider
-----------------------------------

 * This provider is extremely sensitive to geometry validity and will reject any input of invalid geometries. This mostly occurs when copying polygon geometries (with invalid ring orientation) to a feature class that uses a spatial context with a geodetic (lat/long) coordinate system. For Bulk Copy to SQL Server, it will attempt auto-correction of invalid polygon ring orientations.
 * Bulk copying to this provider is quite slow. Use alternate means of copying data if possible.
 * You cannot create spatial contexts whose WKT does not resolve to a matching entry in the ``sys.spatial_reference_systems`` system table
    * You must specify either:
    
        1. A CS name that is present in ``com/ExtendedCoordSys.txt`` or a name such that the following SQL query will produce a result:

        ``SELECT * FROM sys.spatial_reference_systems WHERE SUBSTRING( well_known_text, 9, CHARINDEX('"',well_known_text,9) - 9 ) = <name>``

        2. Or, a WKT that is present in ``com/ExtendedCoordSys.txt`` or a WKT such that the following SQL query will produce a result:

        ``SELECT * FROM sys.spatial_reference_systems WHERE well_known_text = <WKT>``

    * In the context of bulk copying, you can use the SC override feature to "fix" such bad source spatial contexts from being copied.
    * When using the ``copy-class`` command, you are strongly recommended to use the ``--override-sc-from-resolved-wkt`` option which will instruct the command to take the WKT of the source spatial context, resolve it against the coordinate system catalog and apply the override settings from that resolved coordinate system. As the SQL Server provider knows most of the coordinate system catalog (via the bundled ``ExtendedCoordSys.txt`` file), this is a near-bulletproof method of transferring spatial contexts across to SQL Server from any source.