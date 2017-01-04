Properties {
    [switch]$interactive = $true;
    [string]$buildConfig = "DEBUG";
    [string]$buildRevision = "0";
}

. .\nuget.ps1
. .\publish-nuGet.ps1 

task default 


