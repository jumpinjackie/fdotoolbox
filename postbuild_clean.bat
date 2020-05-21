@echo off
if exist out\x64\Release goto clean:
:clean
echo Cleaning DataStoreManager AddIn
call addin_clean.bat out\x64\Release\AddIns\DataStoreManager
echo Cleaning Express AddIn
call addin_clean.bat out\x64\Release\AddIns\Express
echo Cleaning Raster AddIn
call addin_clean.bat out\x64\Release\AddIns\Raster
echo Cleaning Tasks AddIn
call addin_clean.bat out\x64\Release\AddIns\Tasks
goto done
:done