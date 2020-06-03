Other Topics
============

File extension inference
------------------------

Sometimes, you have spatial data files which are viewable/inspectable in FDO Toolbox, but FDO Toolbox cannot directly infer the FDO provider to use based on the extension of the file.

For such file extensions where providers cannot be directly inferred by file extension, a ``FileExtensionMappings.xml`` file allows one to map a file extension to a FDO provider with the appropriate connection string to connect to such a file. For example, this file instructs FDO Toolbox to:

 * Connect using ``OSGeo.OGR`` provider if working with files with a ``.tab`` extension (MapInfo TAB)
 * Connect using ``OSGeo.ODBC`` provider if working with files with a ``.mdb`` extension (MS Access)

Inferrable file extensions have the following benefits in FDO Toolbox:
 
 * You can create connections to such files by just dragging and dropping the files into the **Object Explorer**
 * You can use the ``--from-file`` parameter with such files for ``FdoCmd.exe`` commands that require a connection