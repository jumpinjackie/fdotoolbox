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

$provider = "OSGeo.SQLServerSpatial"
$db = "FdoBulkCopyTest"
$service = "192.168.0.6"
$user = "sa"
$pass = "Sql2016!"
$sourceSchema = "SHP_Schema"
$targetSchema = "dbo"

$provider_arg_string = "--provider $provider"
$create_params_string = "--create-params DataStore $db IsFdoEnabled false"
$pending_connect_params_string = "--connect-params Service $service Username $user Password $pass"
$connect_params_string = "--connect-params Service $service Username $user Password $pass DataStore $db"
$destroy_params_string = "--destroy-params DataStore $db"

$scName = "SCTest"
$scDesc = "Test Spatial Context"
$scCsName = "WGS 84"
$scCsWkt = 'GEOGCS[""WGS 84"", DATUM[""World Geodetic System 1984"", ELLIPSOID[""WGS 84"", 6378137, 298.257223563]], PRIMEM[""Greenwich"", 0], UNIT[""Degree"", 0.0174532925199433]]'
$scXYTol = 0.0001
$scZTol = 0.0001

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

Write-Host "Testing create-spatial-context"
$invExpr = "& .\FdoCmd.exe create-spatial-context $provider_arg_string $connect_params_string --name $scName --description $scDesc --cs-name $scCsName --cs-wkt '$scCsWkt' --xy-tol $scXYTol --z-tol $scZTol --extent-type SpatialContextExtentType_Static --extent -180 -90 180 90"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result "Created spatial context: $scName" $res

Write-Host "Testing list-spatial-contexts"
$invExpr = "& .\FdoCmd.exe list-spatial-contexts $provider_arg_string $connect_params_string"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result $scName $res

Write-Host "Testing destroy-spatial-context"
$invExpr = "& .\FdoCmd.exe destroy-spatial-context $provider_arg_string $connect_params_string --name $scName"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result "Destroyed spatial context: $scName" $res

$scName = "Default"
Write-Host "Testing create-spatial-context"
$invExpr = "& .\FdoCmd.exe create-spatial-context $provider_arg_string $connect_params_string --name $scName --description $scDesc --cs-name $scCsName --cs-wkt '$scCsWkt' --xy-tol $scXYTol --z-tol $scZTol --extent-type SpatialContextExtentType_Static --extent -180 -90 180 90"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result "Created spatial context: $scName" $res

Write-Host "Testing apply-schema"
$invExpr = "& .\FdoCmd.exe apply-schema $provider_arg_string $connect_params_string --schema-file TestData/World_Countries.xml --fix-incompatibilities --rename-schemas $sourceSchema $targetSchema"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result-Contains "Renaming schema before apply: $sourceSchema -> $targetSchema" $res
Expect-Result-Contains "Applied schema using provider: $provider" $res

Write-Host "Testing list-schemas"
$invExpr = "& .\FdoCmd.exe list-schemas $provider_arg_string $connect_params_string"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result-Contains $targetSchema $res
Expect-Result-Contains "guest" $res

Write-Host "Testing list-classes"
$invExpr = "& .\FdoCmd.exe list-classes $provider_arg_string $connect_params_string --schema ${targetSchema}"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result-Contains "World_Countries" $res

Write-Host "Testing list-classes (qualified)"
$invExpr = "& .\FdoCmd.exe list-classes $provider_arg_string $connect_params_string --schema ${targetSchema} --qualified"
Print-Expr $invExpr
$res = Invoke-Expression "$invExpr"
Check-Result
Expect-Result-Contains "${targetSchema}:World_Countries" $res

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