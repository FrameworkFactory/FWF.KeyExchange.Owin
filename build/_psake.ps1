Properties {
    [switch]$interactive = $true;
    [string]$buildConfig = "DEBUG";
    [string]$buildRevision = "0";
}

. .\nuget.ps1
. .\publish-nuGet.ps1 

Task Publish.Push.Local {
    Invoke-Task Publish.NuGet
    Invoke-Task Push.NuGet.Local
}

task default 


