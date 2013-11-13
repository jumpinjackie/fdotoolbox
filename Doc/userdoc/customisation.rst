.. index::
   single: Customisation

Customisation
=============

FDO Toolbox is designed from the ground up to be as customisable and extensible as possible. The following methods of customisation and extension are described below.

Menu Customisation
------------------

Menus and toolbars in FDO Toolbox are XML-driven. They are defined in .addin files which come with each FDO Toolbox Add-In. The menu and toolbar commands are specified in the following
pre-defined paths:

 * /Workbench/MainMenu
 * /Workspace/Tools
 * /Workbench/Toolbar
 * /ObjectExplorer/Toolbar
 * /AppConsole/Toolbar
 * /ObjectExplorer/ContextMenus/FdoConnections
 * /ObjectExplorer/ContextMenus/SelectedConnection
 * /ObjectExplorer/ContextMenus/SelectedSchema
 * /ObjectExplorer/ContextMenus/SelectedClass
 
When adding new custom commands, it is important to note whether the command is sensitive to FDO provider capabilities. If a menu/toolbar command depends on a FDO command being supported, you 
should wrap your command definition with a Condition element.

Example::

 <Condition action="Disable" name="CommandSupported" command="ApplySchema"> ... </Condition>

Scripting
---------

FDO Toolbox also has a built-in IronPython scripting engine. You can write scripts in Python that can be executed from within FDO Toolbox. A simple application API is exposed for python
scripts to consume that provides basic services such as:

 * Choosing files for open/save
 * Managing FDO Connections
 * Selecting open FDO connections
 * Displaying/logging messages