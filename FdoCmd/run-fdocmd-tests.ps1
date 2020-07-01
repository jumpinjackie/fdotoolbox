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

Function Test-File-Provider {
    param([string]$provider, [string]$extension, [string]$sourceSchema, [string]$targetSchema)

    $sourceClass = "World_Countries"
    $targetClass = "World_Countries"
    $scName = "Default"

    Write-Host ""
    Write-Host "====================================================="
    Write-Host "Testing for provider ($provider)"
    Write-Host "====================================================="
    Write-Host ""

    Write-Host "Testing list-create-datastore-params"
    $res = & $PSScriptRoot\FdoCmd.exe list-create-datastore-params --provider $provider
    Check-Result
    Expect-Result-Contains "File" $res

    Write-Host "Testing create-datastore"
    $res = & $PSScriptRoot\FdoCmd.exe create-datastore --provider $provider --create-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "Created data store using provider: $provider" $res

    Write-Host "Testing apply-schema"
    $res = & $PSScriptRoot\FdoCmd.exe apply-schema --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema-file "TestData/World_Countries.xml" --rename-schemas $sourceSchema $targetSchema
    Check-Result
    Expect-Result-Contains "Applied schema using provider: $provider" $res

    Write-Host "Testing list-schemas"
    $res = & $PSScriptRoot\FdoCmd.exe list-schemas --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result $targetSchema $res

    Write-Host "Testing list-classes"
    $res = & $PSScriptRoot\FdoCmd.exe list-classes --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema $targetSchema
    Check-Result
    Expect-Result $sourceClass $res

    Write-Host "Testing create-spatial-context"
    $res = & $PSScriptRoot\FdoCmd.exe create-spatial-context --from-file "$PSScriptRoot/TestData/Test.$extension" --name $scName --description ""Test SC"" --cs-name ""WGS 84"" --cs-wkt 'GEOGCS[""WGS 84"", DATUM[""World Geodetic System 1984"", ELLIPSOID[""WGS 84"", 6378137, 298.257223563]], PRIMEM[""Greenwich"", 0], UNIT[""Degree"", 0.0174532925199433]]' --xy-tol 0.0001 --z-tol 0.0001 --extent-type SpatialContextExtentType_Dynamic
    Check-Result
    Expect-Result "Created spatial context: $scName" $res

    Write-Host "Testing list-spatial-contexts"
    $res = & $PSScriptRoot\FdoCmd.exe list-spatial-contexts --from-file "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result $scName $res

    $bcpTaskName = "CopyWorld"
    $bcpTask = [xml]@"
<?xml version="1.0" encoding="utf-8"?>
<BulkCopy xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" name="copy">
    <Connections>
    <Connection name="source" provider="OSGeo.SDF">
        <ConnectionString>File=$PSScriptRoot\TestData\World_Countries.sdf</ConnectionString>
    </Connection>
    <Connection name="target" provider="$provider">
        <ConnectionString>File=$PSScriptRoot\TestData\Test.$extension</ConnectionString>
    </Connection>
    </Connections>
    <CopyTasks>
        <CopyTask name="$bcpTaskName" createIfNotExists="true">
            <Source connection="source" schema="$sourceSchema" class="$sourceClass" />
            <Target connection="target" schema="$targetSchema" class="$targetClass" />
            <Options>
                <DeleteTarget>false</DeleteTarget>
                <Filter />
                <FlattenGeometries>false</FlattenGeometries>
                <ForceWKB>false</ForceWKB>
                <UseTargetSpatialContext>$scName</UseTargetSpatialContext>
            </Options>
            <PropertyMappings>
                <PropertyMapping source="MAPKEY" target="MAPKEY" createIfNotExists="true" />
                <PropertyMapping source="NAME" target="NAME" createIfNotExists="true" />
                <PropertyMapping source="KEY" target="KEY" createIfNotExists="true" />
                <PropertyMapping source="SHPGEOM" target="SHPGEOM" createIfNotExists="true" />
            </PropertyMappings>
            <ExpressionMappings />
        </CopyTask>
    </CopyTasks>
</BulkCopy>
"@
    $bcpPath = "$PSScriptRoot\TestData\TestBcp.$extension.BulkCopyDefinition"
    $bcpTask.Save($bcpPath)

    Write-host "Testing list-bcp-tasks"
    $res = & $PSScriptRoot\FdoCmd.exe list-bcp-tasks --file $bcpPath
    Check-Result
    Expect-Result $bcpTaskName $res

    Write-Host "Testing bulk copy"
    $res = & $PSScriptRoot\FdoCmd.exe run-task --file $bcpPath
    Check-Result
    Expect-Result-Contains "[$bcpTaskName]: 419 features in" $res
    Expect-Result-Contains "0 features failed to be processed." $res
    Remove-Item $bcpPath

    Write-Host "Testing list-spatial-contexts on BCP target"
    $res = & $PSScriptRoot\FdoCmd.exe list-spatial-contexts --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "Default" $res

    Write-Host "Testing dump-schema on BCP target"
    $schemaFile = "$PSScriptRoot/TestData/Test.$extension.schema.xml"
    $res = & $PSScriptRoot\FdoCmd.exe dump-schema --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema $targetSchema --schema-path $schemaFile
    Check-Result
    Expect-Result "Schema(s) written to $schemaFile" $res

    if ($provider -eq "OSGeo.SDF")
    {
        Write-Host "Testing list-destroy-datastore-params"
        $res = & $PSScriptRoot\FdoCmd.exe list-destroy-datastore-params --provider $provider
        Check-Result
        Expect-Result "File" $res

        Write-Host "Testing destroy-datastore"
        $res = & $PSScriptRoot\FdoCmd.exe destroy-datastore --provider $provider --destroy-params File "$PSScriptRoot/TestData/Test.$extension"
        Check-Result
        Expect-Result "Destroyed data store using provider: $provider" $res
    }
    else 
    {
        Remove-Item "$PSScriptRoot/TestData/Test.$extension"
    }
}

Write-Host "Testing list-providers"
$res = & $PSScriptRoot\FdoCmd.exe list-providers
Check-Result
Expect-Result-Contains "OSGeo.SDF" $res

Write-Host "Testing create-file with test schema"
$testFile = "$PSScriptRoot\TestData\CreateFileTest.sdf"
$res = & $PSScriptRoot\FdoCmd.exe create-file --file $testFile --schema-path $PSScriptRoot\TestData\World_Countries.xml
Check-Result

Write-Host "Testing list-schemas on created file"
$res = & $PSScriptRoot\FdoCmd.exe list-schemas --provider OSGeo.SDF --connect-params File $testFile
Check-Result
Expect-Result "SHP_Schema" $res

Write-Host "Testing list-schemas by inferred file connection"
$res = & $PSScriptRoot\FdoCmd.exe list-schemas --from-file $testFile
Check-Result
Expect-Result "SHP_Schema" $res

Write-Host "Testing create-spatial-context on created file"
$res = & $PSScriptRoot\FdoCmd.exe create-spatial-context --from-file $testFile --name SCTest --description ""Test SC"" --cs-name ""WGS 84"" --cs-wkt 'GEOGCS[""WGS 84"", DATUM[""World Geodetic System 1984"", ELLIPSOID[""WGS 84"", 6378137, 298.257223563]], PRIMEM[""Greenwich"", 0], UNIT[""Degree"", 0.0174532925199433]]' --xy-tol 0.0001 --z-tol 0.0001 --extent-type SpatialContextExtentType_Dynamic
Check-Result
Expect-Result "Created spatial context: SCTest" $res

Write-Host "Testing list-spatial-contexts on created file"
$res = & $PSScriptRoot\FdoCmd.exe list-spatial-contexts --from-file $testFile
Check-Result
Expect-Result "SCTest" $res

#Write-Host "Testing destroy-spatial-context on created file"
#$res = & $PSScriptRoot\FdoCmd.exe destroy-spatial-context --from-file $testFile --name SCTest
#Check-Result
#Expect-Result "Destroyed spatial context: SCTest" $res

Write-Host "Testing list-classes by inferred file (World_Countries.sdf)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\World_Countries.sdf --qualified
Check-Result
Expect-Result "SHP_Schema:World_Countries" $res

Write-Host "Testing list-classes by inferred file (World_Countries.shp)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\World_Countries.shp --qualified
Check-Result
Expect-Result "Default:World_Countries" $res

Write-Host "Testing list-classes by inferred file (World_Countries.tab)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\World_Countries.tab --qualified
Check-Result
Expect-Result "Default:World_Countries" $res

Write-Host "Testing list-classes by inferred file (assorted.dxf)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\assorted.dxf --qualified
Check-Result
Expect-Result "Default:entities" $res

Write-Host "Testing list-classes by inferred file (geometrycollection.geojson)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\geometrycollection.geojson --qualified
Check-Result
Expect-Result "Default:geometrycollection" $res

<#
Write-Host "Testing list-classes by inferred file (MSTest.mdb)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\MSTest.mdb --qualified
Check-Result
Expect-Result-Contains "Fdo:Cities" $res
Expect-Result-Contains "Fdo:CITIES_PRIMARY" $res
Expect-Result-Contains "Fdo:EMPLOYEES" $res
Expect-Result-Contains "Fdo:hospital" $res
Expect-Result-Contains "Fdo:SNDG" $res
Expect-Result-Contains "Fdo:TABLE1" $res
Expect-Result-Contains "Fdo:TABLE2" $res
Expect-Result-Contains "Fdo:Table4" $res
Expect-Result-Contains "Fdo:Table5" $res
Expect-Result-Contains "Fdo:TABLE6" $res
Expect-Result-Contains "Fdo:TABLE7" $res
Expect-Result-Contains "Fdo:TABLE8" $res
Expect-Result-Contains "Fdo:VIEW1" $res
Expect-Result-Contains "Fdo:VIEW2" $res
#Expect-Result-Contains "Fdo:午前" $res

Write-Host "Testing list-classes by inferred file (MsTest.xls)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\MsTest.xls --qualified
Check-Result
Expect-Result-Contains "Fdo:ALLTYPES" $res
Expect-Result-Contains "Fdo:CITY" $res
Expect-Result-Contains "Fdo:COUNTRY" $res
Expect-Result-Contains "Fdo:EMPLOYEES" $res
Expect-Result-Contains "Fdo:POINTS" $res
Expect-Result-Contains "Fdo:SNDG" $res
Expect-Result-Contains "Fdo:TABLE1" $res
Expect-Result-Contains "Fdo:TABLE2" $res
#>

Write-Host "Testing list-schemas by inferred file (wkt.csv)"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --from-file $PSScriptRoot\TestData\wkt.csv --qualified
Check-Result
Expect-Result "Default:wkt" $res

Write-Host "Testing list-classes on created file"
$res = & $PSScriptRoot\FdoCmd.exe list-classes --provider OSGeo.SDF --connect-params File $testFile --schema SHP_Schema
Check-Result
Expect-Result "World_Countries" $res

Write-Host "Testing list-class-properties on created file"
$res = & $PSScriptRoot\FdoCmd.exe list-class-properties --provider OSGeo.SDF --connect-params File $testFile --schema SHP_Schema --class World_Countries
Check-Result
Expect-Result-Contains "SHPGEOM" $res
Expect-Result-Contains "Autogenerated_ID" $res
Expect-Result-Contains "MAPKEY" $res
Expect-Result-Contains "NAME" $res
Expect-Result-Contains "KEY" $res

Remove-Item $testFile

Write-Host "Testing list-spatial-contexts on test data"
$res = & $PSScriptRoot\FdoCmd.exe list-spatial-contexts --provider OSGeo.SDF --connect-params File "$PSScriptRoot/TestData/World_Countries.sdf"
Check-Result
Expect-Result "Default" $res

Write-Host "Testing query-features on test data as CSV"
$res = & $PSScriptRoot\FdoCmd.exe query-features --provider OSGeo.SDF --connect-params File "$PSScriptRoot/TestData/World_Countries.sdf" --class World_Countries --properties Autogenerated_ID NAME SHPGEOM --filter "NAME = 'Australia'" --format CSV
Check-Result
$arr = $res | ConvertFrom-CSV
Expect-Result 2 $arr.Length
Expect-Result "380" $arr[0].Autogenerated_ID
Expect-Result "381" $arr[1].Autogenerated_ID

Write-Host "Testing Express BCP to SDF target (via copy-to-file)"
$testFile = "$PSScriptRoot\TestData\CreateFileTest.sdf"
$res = & $PSScriptRoot\FdoCmd.exe copy-to-file --src-provider OSGeo.SHP --src-connect-params DefaultFileLocation "$PSScriptRoot/TestData/World_Countries.sdf" --src-schema Default --src-classes World_Countries --dest-path $testFile
Check-Result
Expect-Result-Contains "]: 419 features in" $res
Expect-Result-Contains "0 features failed to be processed." $res

$res = & $PSScriptRoot\FdoCmd.exe get-feature-count --from-file $testFile --schema Default --class World_Countries
Check-Result
Expect-Result "419" $res

Remove-Item $testFile

Write-Host "Testing Express BCP to SDF target (via copy-class)"
$testFile = "$PSScriptRoot\TestData\CreateFileTest.sdf"
$res = & $PSScriptRoot\FdoCmd.exe create-file --file $testFile
Check-Result
$res = & $PSScriptRoot\FdoCmd.exe copy-class --src-provider OSGeo.SHP --src-connect-params DefaultFileLocation "$PSScriptRoot/TestData/World_Countries.sdf" --src-schema Default --src-class World_Countries --dst-provider OSGeo.SDF --dst-connect-params File $testFile --dst-schema Default --dst-class World_Countries
Check-Result
Expect-Result-Contains "]: 419 features in" $res
Expect-Result-Contains "0 features failed to be processed." $res

$res = & $PSScriptRoot\FdoCmd.exe get-feature-count --from-file $testFile --schema Default --class World_Countries
Check-Result
Expect-Result "419" $res

Remove-Item $testFile

Write-Host "Testing create-spatial-context with SC shortcut"

Write-Host "  >> Making create-datastore"
$testFile = "$PSScriptRoot/TestData/SCByCodeShortcutTest.sdf"
$res = & $PSScriptRoot\FdoCmd.exe create-datastore --provider OSGeo.SDF --create-params File $testFile
Check-Result
Expect-Result "Created data store using provider: OSGeo.SDF" $res

Write-Host "  >> Creating the spatial context with cs code inference"
$res = & $PSScriptRoot\FdoCmd.exe create-spatial-context --from-file $testFile --xy-tol 0.0001 --z-tol 0.0001 --from-code "LL84"
Check-Result
Expect-Result "Created spatial context: LL84" $res

# Before we delete check that the SC matched the cs on the key bits
$cs = & $PSScriptRoot\FdoCmd.exe find-cs-by-code --cs-code "LL84" --csv | ConvertFrom-CSV
Check-Result
$csWkt = & $PSScriptRoot\FdoCmd.exe cs-code-to-wkt --code "LL84"
Check-Result
$res = & $PSScriptRoot\FdoCmd.exe list-spatial-contexts --full-details --from-file $testFile
Check-Result

Expect-Result-Contains "Description: $($cs.Description)" $res
#Expect-Result-Contains "Coordinate System: $($cs.Code)" $res
Expect-Result-Contains "Coordinate System WKT: $csWkt" $res

Remove-Item $testFile

Write-Host "  >> Making create-datastore"
$testFile = "$PSScriptRoot/TestData/SCByEpsgShortcutTest.sdf"
$res = & $PSScriptRoot\FdoCmd.exe create-datastore --provider OSGeo.SDF --create-params File $testFile
Check-Result
Expect-Result "Created data store using provider: OSGeo.SDF" $res

Write-Host "  >> Creating the spatial context with epsg code inference"
$res = & $PSScriptRoot\FdoCmd.exe create-spatial-context --from-file $testFile --xy-tol 0.0001 --z-tol 0.0001 --from-epsg 3857
Check-Result
Expect-Result "Created spatial context: WGS84_PseudoMercator" $res

# Before we delete check that the SC matched the cs on the key bits
$cs = & $PSScriptRoot\FdoCmd.exe find-cs-by-epsg --epsg-code 3857 --csv | ConvertFrom-CSV
Check-Result
$csWkt = & $PSScriptRoot\FdoCmd.exe epsg-to-wkt --epsg 3857
Check-Result
$res = & $PSScriptRoot\FdoCmd.exe list-spatial-contexts --full-details --from-file $testFile
Check-Result
Expect-Result-Contains "Description: $($cs.Description)" $res
#Expect-Result-Contains "Coordinate System: $($cs.Code)" $res
Expect-Result-Contains "Coordinate System WKT: $csWkt" $res

#Remove-Item $testFile

$sourceSchema = "SHP_Schema"
$targetSchema = "SHP_Schema"
Test-File-Provider "OSGeo.SDF" "sdf" $sourceSchema $targetSchema
$sourceSchema = "SHP_Schema"
$targetSchema = "Default"
Test-File-Provider "OSGeo.SQLite" "sqlite" $sourceSchema $targetSchema