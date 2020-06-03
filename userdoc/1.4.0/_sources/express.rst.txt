The Express Add-In
==================

FDO Toolbox is a front-end for the FDO API. The FDO API allows us to access and manipulate any geospatial data in a generic fashion.

Sometimes however, having to create connections for something as simple as a SDF file is quite slow and cumbersome using 
generic methods provided. Similarly, having to use the generic create data store dialog for creating SDF files is just
as equally slow and cumbersome. 

The Express Add-In has been developed for the purpose of making this process quicker for commonly-used providers, especially file-based ones like SDF, SHP and SQLite.

.. index::
   single: Connecting to Data; Express

.. _connect-express:

Creating connections
--------------------

The Express Add-In offers a faster method of creating connections to spatial data sources is through the Connection Dialogs in the Express Add-In. These dialogs
have been optimised for specific providers. For example, creating a SDF connection is as simple as browsing for the SDF file.

The following providers, have express connection support:

 * SDF
 * SHP
 * SQLite
 * ODBC
  
   * Jet (Microsoft Access)
   * SQL Server
   * Microsoft Excel
   * Text
 
 * OGR
  
   * MapInfo
   * CSV
   * ESRI Personal GeoDB
   
 * PostGIS
 * MySQL
 
Creating Flat Files
-------------------

See :ref:`create-data-store-express`

Express Bulk Copy
-----------------

The Express Add-In also offers a simple bulk copy from one file-based data store to another. This is a very useful tool for doing conversions from one spatial data
format to another. The Express Bulk Copy supports the following providers:

 * SDF
 * SHP
 * SQLite
 
Please take note of the following when using the Express Bulk Copy:

 * When copying from source to target, the target is always overwritten.
 * Copying to SHP may fail if certain conditions are not met. See :ref:`shp-constraints`
 * Object and Assocation properties are omitted from the bulk copy.