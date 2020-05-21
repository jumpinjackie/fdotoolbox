@echo off
pushd %1
if exist FdoToolbox.Base.* del FdoToolbox.Base.*
if exist FdoToolbox.Core.* del FdoToolbox.Core.*
if exist ICSharpCode.* del ICSharpCode.*
if exist log4net.* del log4net.*
if exist SharpMap.* del SharpMap.*
if exist WeifenLuo.* del WeifenLuo.*
if exist FileExtensionMappings.xml del FileExtensionMappings.xml
if exist *.pdb del *.pdb
popd