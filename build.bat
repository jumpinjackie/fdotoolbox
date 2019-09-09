@echo off

rem Note: 
rem 
rem The following needs to be installed
rem  - Microsoft .net Framework 4.0
rem  - Microsoft HTML Workshop
rem  - Sandcastle May 2008 release (http://sandcastle.codeplex.com)
rem  - Sandcastle Help File Builder (http://shfb.codeplex.com)
rem  - Python 2.x
rem  - Sphinx (http://sphix.pocoo.org)

SET TYPEACTION=build
SET TYPEBUILD=Release
SET PLATFORM=x64

IF "%PROCESSOR_ARCHITECTURE%"=="AMD64" SET HTMLHELP=C:\Program Files (x86)\HTML Help Workshop
IF "%PROCESSOR_ARCHITECTURE%"=="x86" SET HTMLHELP=C:\Program Files\HTML Help Workshop

SET FDOTOOLBOX_OUTDIR=%CD%\out\%PLATFORM%\%TYPEBUILD%
SET DOCPATH=%CD%\Doc
SET THIRDPARTY=%CD%\Thirdparty
SET INSTALL=%CD%\Install
SET FDOINFO=%CD%\FdoInfo
SET FDOUTIL=%CD%\FdoUtil
SET TESTAPP=%CD%\TestApp
SET CORETESTLIBRARY=%CD%\FdoToolbox.Core.Tests
SET BASETESTLIBRARY=%CD%\FdoToolbox.Base.Tests
SET TESTDATA=%CD%\UnitTestData
SET TESTPATH=%CD%\out\%PLATFORM%\Test
SET FDOTOOLBOX=%CD%\FdoToolbox
SET FDOTOOLBOXCORE=%CD%\FdoToolbox.Core
SET FDOTOOLBOXBASE=%CD%\FdoToolbox.Base
SET FDOTOOLBOXADDINMGR=%CD%\FdoToolbox.AddInManager
SET FDOTOOLBOXADODB=%CD%\FdoToolbox.AdoDb
SET FDOTOOLBOXEXPRESS=%CD%\FdoToolbox.Express
SET FDOTOOLBOXRASTER=%CD%\FdoToolbox.Raster
SET TESTMODULE=%CD%\TestModule
SET MGMODULE=%CD%\MGModule
SET RELEASE_VERSION=Trunk

SET PATH=%PATH%;%systemroot%\Microsoft.NET\Framework\v4.0.30319;%THIRDPARTY%\NDoc;%THIRDPARTY%\NSIS;%HTMLHELP%
SET VERBOSITY=/v:q

:study_params
if (%1)==() goto start_build

if "%1"=="-help"    goto help_show
if "%1"=="-h"       goto help_show

if "%1"=="-c"       goto get_conf
if "%1"=="-config"  goto get_conf

if "%1"=="-a"       goto get_action
if "%1"=="-action"  goto get_action

if "%1"=="-p"        goto get_platform
if "%1"=="-platform" goto get_platform

if "%1"=="-version" goto get_version

if "%1"=="-vlabel" goto get_version_label

if "%1"=="-v"       goto get_verbose
if "%1"=="-verbose" goto get_verbose

if "%1"=="-t"       goto test
if "%1"=="-test"    goto test

goto custom_error

:next_param
shift
shift
goto study_params

:get_verbose
SET VERBOSITY=/v:n
goto next_param

:get_version
SET RELEASE_VERSION=%2
goto next_param

:get_version_label
SET RELEASE_LABEL=%2
goto next_param

:get_conf
SET TYPEBUILD=%2
SET FDOTOOLBOX_OUTDIR=%CD%\out\%PLATFORM%\%TYPEBUILD%
if "%2"=="release" goto next_param
if "%2"=="Release" goto next_param
if "%2"=="debug" goto next_param
if "%2"=="Debug" goto next_param
goto custom_error

:get_action
SET TYPEACTION=%2
if "%2"=="build" goto next_param
if "%2"=="clean" goto next_param
goto custom_error

:get_platform
SET PLATFORM=%2
SET TESTPATH=%CD%\out\%PLATFORM%\Test
SET FDOTOOLBOX_OUTDIR=%CD%\out\%PLATFORM%\%TYPEBUILD%
if "%2"=="x86" goto next_param
if "%2"=="x64" goto next_param
goto custom_error

:start_build
if "%TYPEACTION%"=="build" goto build
if "%TYPEACTION%"=="clean" goto clean

:build
echo Configuration is: %TYPEBUILD%
echo Platform is: %PLATFORM%
echo Release Version is: %RELEASE_VERSION%
echo Release Label is: %RELEASE_LABEL%

echo Stamping: %RELEASE_VERSION%
SetAssemblyVersion.exe -set:%RELEASE_VERSION% Properties\GlobalAssemblyInfo.cs

echo Building FdoToolbox
msbuild.exe /p:Configuration=%TYPEBUILD%;Platform=%PLATFORM% %VERBOSITY% FdoToolbox.sln

if exist "%FDOTOOLBOX_OUTDIR%\FDO Toolbox Core API.chm" goto build_user_doc

:build_api_doc
echo Building API Documentation
REM This can take a while so only build if nothing is already there.
if exist "%FDOTOOLBOX_OUTDIR%\FDO Toolbox Core API.chm" goto build_user_doc
pushd %DOCPATH%
msbuild.exe /p:Configuration=%TYPEBUILD%;Platform=%PLATFORM% FdoToolboxCoreApi.%TYPEBUILD%.%PLATFORM%.shfbproj
copy "Help\FDO Toolbox Core API.chm" %FDOTOOLBOX_OUTDIR%
popd

:build_user_doc
echo Building User Documentation
pushd %DOCPATH%\userdoc
call make htmlhelp
popd
pushd %DOCPATH%\userdoc\_build\htmlhelp
hhc FDOToolbox.hhp
copy FDOToolbox.chm %FDOTOOLBOX_OUTDIR%
popd

rem :copy_thirdparty
rem IF NOT EXIST %FDOTOOLBOX_OUTDIR%\FDO xcopy /S /Y /I %THIRDPARTY%\Fdo\*.* %FDOTOOLBOX_OUTDIR%\FDO

:create_installer
echo Creating installer
pushd %INSTALL%
if defined RELEASE_LABEL (
makensis /DSLN_CONFIG=%TYPEBUILD% /DCPU=%PLATFORM% /DRELEASE_VERSION=%RELEASE_VERSION% /DRELEASE_LABEL=%RELEASE_LABEL% FdoToolbox.nsi
) else (
makensis /DSLN_CONFIG=%TYPEBUILD% /DCPU=%PLATFORM% /DRELEASE_VERSION=%RELEASE_VERSION% FdoToolbox.nsi
)
popd
goto quit

:clean
echo Cleaning Temp doc directories
rd /S /Q %DOCPATH%\Help
echo Cleaning Output Directory
rd /S /Q out
echo Cleaning FdoInfo
rd /S /Q %FDOINFO%\bin
rd /S /Q %FDOINFO%\obj
echo Cleaning FdoUtil
rd /S /Q %FDOUTIL%\bin
rd /S /Q %FDOUTIL%\obj
echo Cleaning FdoToolbox.Core.Tests
rd /S /Q %CORETESTLIBRARY%\bin
rd /S /Q %CORETESTLIBRARY%\obj
echo Cleaning FdoToolbox.Base.Tests
rd /S /Q %BASETESTLIBRARY%\bin
rd /S /Q %BASETESTLIBRARY%\obj
echo Cleaning FdoToolbox
rd /S /Q %FDOTOOLBOX%\bin
rd /S /Q %FDOTOOLBOX%\obj
echo Cleaning FdoToolbox.Core
rd /S /Q %FDOTOOLBOXCORE%\bin
rd /S /Q %FDOTOOLBOXCORE%\obj
echo Cleaning FdoToolbox.Base
rd /S /Q %FDOTOOLBOXBASE%\bin
rd /S /Q %FDOTOOLBOXBASE%\obj
echo Cleaning TestModule
rd /S /Q %TESTMODULE%\bin
rd /S /Q %TESTMODULE%\obj
echo Cleaning MGModule
rd /S /Q %MGMODULE%\bin
rd /S /Q %MGMODULE%\obj
echo Cleaning FdoToolbox.AddInManager
rd /S /Q %FDOTOOLBOXADDINMGR%\bin
rd /S /Q %FDOTOOLBOXADDINMGR%\obj
echo Cleaning FdoToolbox.Express
rd /S /Q %FDOTOOLBOXEXPRESS%\bin
rd /S /Q %FDOTOOLBOXEXPRESS%\obj
echo Cleaning FdoToolbox.Raster
rd /S /Q %FDOTOOLBOXRASTER%\bin
rd /S /Q %FDOTOOLBOXRASTER%\obj
echo Cleaning Documentation
pushd %DOCPATH%\userdoc
make clean
popd
goto quit

:test
echo Building unit tests
msbuild.exe /nologo /p:Configuration=Debug;Platform=%PLATFORM% %VERBOSITY% %CORETESTLIBRARY%\FdoToolbox.Core.Tests.csproj
msbuild.exe /nologo /p:Configuration=Debug;Platform=%PLATFORM% %VERBOSITY% %BASETESTLIBRARY%\FdoToolbox.Base.Tests.csproj
xcopy /S /Y /I %TESTDATA%\*.* %TESTPATH%
xcopy /S /Y /I %THIRDPARTY%\Fdo_%PLATFORM%\*.* %TESTPATH%
xcopy /S /Y /I %THIRDPARTY%\NUnit\bin\*.* %TESTPATH%
echo Running unit tests (Core)
%TESTPATH%\nunit-console.exe /nologo /labels %TESTPATH%\FdoToolbox.Core.Tests.dll
echo Running unit tests (Base)
%TESTPATH%\nunit-console.exe /nologo /labels %TESTPATH%\FdoToolbox.Base.Tests.dll
goto quit

:custom_error
echo The command is not recognized.
echo Please use the format:
:help_show
echo ************************************************************************
echo build.bat [-h]
echo           [-t]
echo           [-v]
echo           [-version=x.y.z.r]
echo           [-vlabel=label]
echo           [-p=CPU]
echo           [-c=BuildType]
echo           [-a=Action]
echo Help:                  -h[elp]
echo Test:                  -t[est]
echo Verbose:               -v
echo CPU:                   -p[latform]=x86(default),x64
echo BuildType:             -c[onfig]=Release(default), Debug
echo Action:                -a[ction]=build(default),
echo                                  clean
echo ************************************************************************
:quit
