Provider-specific Notes
=======================

.. _shp-constraints:

SHP Provider
------------

 * Geometric Properties can only support one particular Geometric Type. You cannot combine multiple Geometric Types. Eg. You can only have Point, Curve or Surface. You cannot have combinations of the 3.
 * SHP supports a limited set of data types. Take note of this when copying data to SHP.

.. _kgora-constraints:

King Oracle Provider
--------------------

 * If applying a new schema, you will have to remove and rebuild the connection to see the changes.

.. _slt-constraints:

.. _postgis-constraints:

PostGIS Provider
----------------

 * If applying a new schema, you will have to remove and rebuild the connection to see the changes.

.. _sqlserver-constraints:

SQL Server Spatial (2008) Provider
----------------------------------

 * This provider is extremely sensitive to geometry validity and will reject any input of invalid geometries, FDO itself does not provide any form of geometry validation. This mostly occurs when copying polygon geometries (with invalid ring orientation) to a feature class that uses a spatial context with a geodetic (lat/long) coordinate system. Take note of this when bulk copying to sql server.
 * Bulk copying to this provider is quite slow. Use alternate means of copying data if possible.