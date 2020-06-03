$branch = $env:APPVEYOR_REPO_BRANCH
$tag = $env:APPVEYOR_REPO_TAG
$commit = $env:APPVEYOR_REPO_COMMIT

$revString = $branch + "-" + $commit
if (-Not [string]::IsNullOrEmpty($tag) {
    $revString = $tag + "-" + $commit
}

Write-Host "Stamping git rev string to GlobalAssemblyInfo: $revString"

$asmi = Get-Content .\Properties\GlobalAssemblyInfo.cs
$pasmi = $asmi.Replace("master-abcdefg", $revString);
$pasmi | Set-Content -Path .\Properties\GlobalAssemblyInfo.cs 
