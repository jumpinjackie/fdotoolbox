# Copyright (C) 2020, Jackie Ng
# https://github.com/jumpinjackie/fdotoolbox, jumpinjackie@gmail.com
# 
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
# License as published by the Free Software Foundation; either
# version 2.1 of the License, or (at your option) any later version.
# 
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.
# 
# You should have received a copy of the GNU Lesser General Public
# License along with this library; if not, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
# 
#
# See license.txt for more/additional licensing information

param(
    [parameter(Mandatory = $true)] [string]$provider, # = "OSGeo.SQLServerSpatial"
    [parameter(Mandatory = $true)] [string]$dataStore, # = "FdoBulkCopyTest"
    [parameter(Mandatory = $true)] [string]$service, # = "192.168.0.6"
    [parameter(Mandatory = $true)] [string]$user, # = "sa"
    [parameter(Mandatory = $true)] [string]$pass, # = "Sql2016!"
    [parameter(Mandatory = $true)] [string]$sourceSchema, # = "SHP_Schema"
    [parameter(Mandatory = $true)] [string]$targetSchema # = "dbo"
)

Function Check-Result {
    if ( $LASTEXITCODE -eq 0 ) {
        Write-Host "  > Command returned 0";
        # True, last operation succeeded
    } else {
        Write-Host "  >> Command returned non-zero status: $LASTEXITCODE";
        exit $LASTEXITCODE
    }
}

Function Expect-Result {
    param([string]$expected, [string]$actual)
    if (-Not ($expected -eq $actual)) {
        Write-Host "Failed assertion:";
        Write-Host "Expected:";
        Write-Host "<<<<<<";
        Write-Host $expected;
        Write-Host ">>>>>>";
        Write-Host "Actual:";
        Write-Host "<<<<<<";
        Write-Host $actual;
        Write-Host ">>>>>>";
    } else {
        Write-Host "  > Output matched expected result";
    }
}

Function Expect-Result-Contains {
    param([string]$expected, [string]$actual)
    if (-Not ($actual.Contains($expected))) {
        Write-Host "Failed contains assertion:";
        Write-Host "Expected:";
        Write-Host "<<<<<<";
        Write-Host $expected;
        Write-Host ">>>>>>";
        Write-Host "Actual:";
        Write-Host "<<<<<<";
        Write-Host $actual;
        Write-Host ">>>>>>";
    } else {
        Write-Host "  > Output contains expected result";
    }
}

Function Print-Expr {
    param([string]$invExpr)
    #Write-Host ">>>>>>>>>>>>>"
    #Write-Host "$invExpr"
    #Write-Host "<<<<<<<<<<<<<"
}

$provider_arg_string = "--provider $provider"
$create_params_string = "--create-params DataStore $dataStore"
$pending_connect_params_string = "--connect-params Service $service Username $user Password $pass"
$connect_params_string = "--connect-params Service $service Username $user Password $pass DataStore $dataStore"
$destroy_params_string = "--destroy-params DataStore $dataStore"

$scName = "SCTest"
$scDesc = "Test Spatial Context"
$scCsName = "WGS 84"
$scCsWkt = 'GEOGCS[""WGS 84"", DATUM[""World Geodetic System 1984"", ELLIPSOID[""WGS 84"", 6378137, 298.257223563]], PRIMEM[""Greenwich"", 0], UNIT[""Degree"", 0.0174532925199433]]'
$scXYTol = 0.0001
$scZTol = 0.0001

$sourceDir = "D:\temp\Sheboygan"
$sourceSchema = "Default"

Write-Host ""
Write-Host "====================================================="
Write-Host "Testing for provider ($provider)"
Write-Host "====================================================="
Write-Host ""

Write-Host "Testing create-datastore"
$invExpr = "& .\FdoCmd.exe create-datastore $provider_arg_string $create_params_string $pending_connect_params_string"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result "Created data store using provider: $provider" $res

Write-Host "Getting source SC"
$invExpr = "& .\FdoCmd.exe list-spatial-contexts --provider OSGeo.SHP --connect-params DefaultFileLocation $sourceDir"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
$scCount = 0;
$sourceScName = ""
foreach ($sc in $res) {
    $sourceScName = $sc
    $scCount++
}
Expect-Result 1 $scCount

$invExpr = "& .\FdoCmd.exe list-classes --provider OSGeo.SHP --connect-params DefaultFileLocation $sourceDir"
Print-Expr $invExpr
$sourceClasses = Invoke-Expression "$invExpr"
Check-Result
$bFirst = $true
foreach ($cls in $sourceClasses)
{
    Write-Host "Bulk Copying: $cls"
    if ($bFirst) {
        $invExpr = "& .\FdoCmd.exe copy-class --src-provider OSGeo.SHP --src-connect-params DefaultFileLocation $sourceDir --dst-provider $provider --dst-connect-params Service $service Username $user Password $pass DataStore $dataStore --src-schema $sourceSchema --src-class $cls --dst-schema $targetSchema --dst-class $cls --override-sc-name ""$sourceScName"" --override-sc-cs '$scCsName' --override-sc-target-name '$scName' --override-sc-wkt '$scCsWkt' --save-task-path D:\temp\$cls.BulkCopyDefinition"
    } else {
        $invExpr = "& .\FdoCmd.exe copy-class --src-provider OSGeo.SHP --src-connect-params DefaultFileLocation $sourceDir --dst-provider $provider --dst-connect-params Service $service Username $user Password $pass DataStore $dataStore --src-schema $sourceSchema --src-class $cls --dst-schema $targetSchema --dst-class $cls --use-target-sc '$scName' --save-task-path D:\temp\$cls.BulkCopyDefinition"
    }
    Print-Expr $invExpr
    Invoke-Expression "$invExpr"
    Check-Result
    $bFirst = $false
}

Write-Host "Testing destroy-datastore"
$invExpr = "& .\FdoCmd.exe destroy-datastore $provider_arg_string $destroy_params_string $pending_connect_params_string"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result-Contains "Destroyed data store using provider: $provider" $res

<#
.\FdoCmd.exe create-datastore --provider OSGeo.SQLServerSpatial --create-params DataStore FdoBulkCopyTest IsFdoEnabled false --connect-params Service 192.168.0.6 Username sa Password Sql2016!
.\FdoCmd.exe create-spatial-context --provider OSGeo.SQLServerSpatial --connect-params Service 192.168.0.6 Username sa Password Sql2016! DataStore FdoBulkCopyTest --name SCTest --description ""Test SC"" --cs-name ""WGS 84"" --cs-wkt 'GEOGCS[""WGS 84"", DATUM[""World Geodetic System 1984"", ELLIPSOID[""WGS 84"", 6378137, 298.257223563]], PRIMEM[""Greenwich"", 0], UNIT[""Degree"", 0.0174532925199433]]' --xy-tol 0.0001 --z-tol 0.0001 --extent-type SpatialContextExtentType_Static --extent -180 -90 180 90
.\FdoCmd.exe list-spatial-contexts --provider OSGeo.SQLServerSpatial --connect-params Service 192.168.0.6 Username sa Password Sql2016! DataStore FdoBulkCopyTest
.\FdoCmd.exe destroy-spatial-context --provider OSGeo.SQLServerSpatial --connect-params Service 192.168.0.6 Username sa Password Sql2016! DataStore FdoBulkCopyTest --name SCTest
.\FdoCmd.exe destroy-datastore --provider OSGeo.SQLServerSpatial --destroy-params DataStore FdoBulkCopyTest --connect-params Service 192.168.0.6 Username sa Password Sql2016!
#>