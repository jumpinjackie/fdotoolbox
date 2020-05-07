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
    param([string]$provider, [string]$extension)

    $sourceSchema = "SHP_Schema"
    $sourceClass = "World_Countries"

    $targetSchema = "SHP_Schema"
    $targetClass = "World_Countries"

    Write-Host ""
    Write-Host "====================================================="
    Write-Host "Testing for provider ($provider)"
    Write-Host "====================================================="
    Write-Host ""

    Write-Host "Testing list-create-datastore-params"
    $res = & $PSScriptRoot\FdoCmd.exe list-create-datastore-params --provider $provider
    Check-Result
    Expect-Result "File" $res

    Write-Host "Testing create-datastore"
    $res = & $PSScriptRoot\FdoCmd.exe create-datastore --provider $provider --create-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "Created data store using provider: $provider" $res

    Write-Host "Testing apply-schema"
    $res = & $PSScriptRoot\FdoCmd.exe apply-schema --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema-file "TestData/World_Countries.xml"
    Check-Result
    Expect-Result "Applied schema using provider: $provider" $res

    Write-Host "Testing list-schemas"
    $res = & $PSScriptRoot\FdoCmd.exe list-schemas --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result $sourceSchema $res

    Write-Host "Testing list-classes"
    $res = & $PSScriptRoot\FdoCmd.exe list-classes --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema SHP_Schema
    Check-Result
    Expect-Result $sourceClass $res

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
    del $bcpPath

    Write-Host "Testing list-spatial-contexts on BCP target"
    $res = & $PSScriptRoot\FdoCmd.exe list-spatial-contexts --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "Default" $res

    Write-Host "Testing dump-schema on BCP target"
    $schemaFile = "$PSScriptRoot/TestData/Test.$extension.schema.xml"
    $res = & $PSScriptRoot\FdoCmd.exe dump-schema --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema $targetSchema --schema-path $schemaFile
    Check-Result
    Expect-Result "Schema(s) written to $schemaFile" $res

    Write-Host "Testing list-destroy-datastore-params"
    $res = & $PSScriptRoot\FdoCmd.exe list-destroy-datastore-params --provider $provider
    Check-Result
    Expect-Result "File" $res

    Write-Host "Testing destroy-datastore"
    $res = & $PSScriptRoot\FdoCmd.exe destroy-datastore --provider $provider --destroy-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "Destroyed data store using provider: $provider" $res
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

del $testFile

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

Test-File-Provider "OSGeo.SDF" "sdf"
#Test-Provider "OSGeo.SQLite" "sqlite"