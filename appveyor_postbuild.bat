@echo on
if "%APPVEYOR_BUILD_FOLDER%" == "" SET APPVEYOR_BUILD_FOLDER=%CD%
if "%APPVEYOR_BUILD_NUMBER%" == "" SET APPVEYOR_BUILD_NUMBER=0
if "%RELEASE_VERSION%" == "" SET RELEASE_VERSION=master
if "%APPVEYOR_REPO_TAG%" == "true" set ARTIFACT_RELEASE_LABEL=%APPVEYOR_REPO_TAG_NAME%
if "%ARTIFACT_RELEASE_LABEL%" == "" SET ARTIFACT_RELEASE_LABEL=master
if "%CONFIGURATION%" == ""  SET CONFIGURATION=Release
if "%PLATFORM%" == "" SET PLATFORM=x64

SET DOCPATH=%APPVEYOR_BUILD_FOLDER%\Doc
SET FDOTOOLBOX_OUTDIR=%APPVEYOR_BUILD_FOLDER%\out\%PLATFORM%\%CONFIGURATION%

call postbuild_clean.bat

echo Creating installer
%APPVEYOR_BUILD_FOLDER%\Thirdparty\NSIS\makensis.exe /DSLN_CONFIG=%CONFIGURATION% /DCPU=%PLATFORM% /DRELEASE_VERSION=%APPVEYOR_BUILD_VERSION% /DRELEASE_LABEL=%ARTIFACT_RELEASE_LABEL% %APPVEYOR_BUILD_FOLDER%\Install\FdoToolbox.nsi