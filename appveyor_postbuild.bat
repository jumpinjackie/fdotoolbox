if "%APPVEYOR_BUILD_FOLDER%" == "" SET APPVEYOR_BUILD_FOLDER=%CD%
if "%CONFIGURATION%" == "" SET CONFIGURATION=Release
if "%NETCORE_MONIKER%" == "" SET NETCORE_MONIKER=netcoreapp2.2
if "%APPVEYOR_BUILD_NUMBER%" == "" SET APPVEYOR_BUILD_NUMBER=0
if "%APPVEYOR_REPO_TAG%" == "true" set ARTIFACT_RELEASE_LABEL=%APPVEYOR_REPO_TAG_NAME%
if "%ARTIFACT_RELEASE_LABEL%" == "" SET ARTIFACT_RELEASE_LABEL=master

SET DOCPATH=%APPVEYOR_BUILD_FOLDER%\Doc

echo Building User Documentation
pushd %DOCPATH%\userdoc
call make htmlhelp
popd
pushd %DOCPATH%\userdoc\_build\htmlhelp
hhc FDOToolbox.hhp
copy FDOToolbox.chm %FDOTOOLBOX_OUTDIR%
popd

echo Creating installer
%APPVEYOR_BUILD_FOLDER%\Thirdparty\NSIS\makensis.exe /DSLN_CONFIG=Releas /DCPU=x64 /DRELEASE_VERSION=%APPVEYOR_BUILD_VERSION% /DRELEASE_LABEL=%ARTIFACT_RELEASE_LABEL% FdoToolbox.nsi