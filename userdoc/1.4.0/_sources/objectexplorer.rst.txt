The Object Explorer
===================

The **Object Explorer** is the heart of the FDO Toolbox application, it is the component that lets you manage (create / delete / rename) any type of object
and to perform certain tasks or operations on them. The Object Explorer handles the following types of objects:

 * FDO Connections
 * ETL Tasks
 
Most contextual commands in the Object Explorer are aware of capabilities of the connection currently in focus in the Object Explorer. Any commands
disabled under the Object Explorer context menu are because such commands are not supported by the current connection. Thus, this enabling/disabling of 
context menu commands based on capabilities ensures that any illegal actions cannot be performed.