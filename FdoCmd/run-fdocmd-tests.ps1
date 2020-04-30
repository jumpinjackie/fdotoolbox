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

Function Test-Provider {
    param([string]$provider, [string]$extension)

    Write-Host "Testing list-create-datastore-params with (provider: $provider)"
    $res = & $PSScriptRoot\FdoCmd.exe list-create-datastore-params --provider $provider
    Check-Result
    Expect-Result "File" $res

    Write-Host "Testing create-datastore with (provider: $provider)"
    $res = & $PSScriptRoot\FdoCmd.exe create-datastore --provider $provider --create-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "Created data store using provider: $provider" $res

    Write-Host "Testing apply-schema with (provider: $provider)"
    $res = & $PSScriptRoot\FdoCmd.exe apply-schema --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema-file "TestData/World_Countries.xml"
    Check-Result
    Expect-Result "Applied schema using provider: $provider" $res

    Write-Host "Testing list-schemas with (provider: $provider)"
    $res = & $PSScriptRoot\FdoCmd.exe list-schemas --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "SHP_Schema" $res

    Write-Host "Testing list-classes with (provider: $provider)"
    $res = & $PSScriptRoot\FdoCmd.exe list-classes --provider $provider --connect-params File "$PSScriptRoot/TestData/Test.$extension" --schema SHP_Schema
    Check-Result
    Expect-Result "World_Countries" $res

    Write-Host "Testing bulk copy with (provider: $provider)"
    $bcpTask = [xml]@"
<?xml version="1.0" encoding="utf-8"?>
<BulkCopy xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" name="copy">
    <Connections>
    <Connection name="source" provider="OSGeo.SDF">
        <ConnectionString>File=$PSScriptRoot\TestData\World_Countries.sdf</ConnectionString>
    </Connection>
    <Connection name="target" provider="OSGeo.SDF">
        <ConnectionString>File=$PSScriptRoot\TestData\Test.$extension</ConnectionString>
    </Connection>
    </Connections>
    <CopyTasks>
        <CopyTask name="CopyTask1" createIfNotExists="true">
            <Source connection="source" schema="SHP_Schema" class="World_Countries" />
            <Target connection="target" schema="SHP_Schema" class="World_Countries" />
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
    $res = & $PSScriptRoot\FdoCmd.exe run-task --file $bcpPath
    Check-Result
    Expect-Result-Contains "[CopyTask1]: 419 features in" $res
    Expect-Result-Contains "0 features failed to be processed." $res
    del $bcpPath

    Write-Host "Testing list-destroy-datastore-params with (provider: $provider)"
    $res = & $PSScriptRoot\FdoCmd.exe list-destroy-datastore-params --provider $provider
    Check-Result
    Expect-Result "File" $res

    Write-Host "Testing destroy-datastore with (provider: $provider)"
    $res = & $PSScriptRoot\FdoCmd.exe destroy-datastore --provider $provider --destroy-params File "$PSScriptRoot/TestData/Test.$extension"
    Check-Result
    Expect-Result "Destroyed data store using provider: $provider" $res
}

Test-Provider "OSGeo.SDF" "sdf"
#Test-Provider "OSGeo.SQLite" "sqlite"