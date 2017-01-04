Param(
    [string]$buildConfig = "DEBUG",
    [string]$buildRevision = "1",
    [string]$taskName = "",
    [switch]$packageRestore = $false
)
    Set-StrictMode -Version latest

    # Always run the script with the current script location as the base path
    Set-Location (Split-Path $MyInvocation.MyCommand.Path)

    # Provide defaults and global settings
    . .\_version.ps1

    if ((Get-ExecutionPolicy) -eq "Restricted") {
        Write-Warning @"
Your execution policy is $executionPolicy, this means you will not be able import or use any scripts including modules.
To fix this change your execution policy to something like RemoteSigned.

        PS> Set-ExecutionPolicy RemoteSigned

For more information execute:

        PS> Get-Help about_execution_policies

"@
    }
    
    # Define common paths
    $global:scripts_directory = Resolve-Path .
    $global:root_directory = Resolve-Path ..\
    $global:src_directory = Resolve-Path ..\source

    # It is possible that the packages directory does not exist yet
    $global:packages_directory = "$scripts_directory\packages"
    
    # Reset the publish directory upon every build
    $global:publish_directory = "$root_directory\publish"
    if (!(Test-Path $packages_directory)) {
      md -Force $publish_directory | Out-Null
    }
    
    # Run nuget to install all packages used by the scripts
    $global:nuGetUrl = 'https://api.nuget.org/v3/index.json'
    
    # nuget.exe takes a while - only update the packages if there isn't any packages on the physical drive
    if (!(Test-Path $packages_directory) -Or (gci $packages_directory -Directory -ErrorAction Continue) -eq $null) {
        .$scripts_directory\tools\nuget.exe install packages.config -OutputDirectory packages -Source $nuGetUrl | Out-Null 
    }

    # Need pSake 
    Import-Module .\packages\psake.*\tools\psake.psm1
  
    # If -packageRestore was supplied, then run nuget.exe install
    if ($packageRestore -eq $true) {

        # Find every packages.config file in the source code and restore every package listed in the file to the global packages directory
        gci $src_directory -Recurse "packages.config" |% {
            $projectPackagesConfig = $_.FullName

            Write-Host "Restoring " $projectPackagesConfig
            .$scripts_directory\tools\nuget.exe install $projectPackagesConfig -o $src_directory\packages -Source $nuGetUrl
        }
    }

    [bool]$buildSucceeded = $false

    try {   
        # Remove any errors at this point to record any future errors
        $Error.Clear()

        if ( $taskName -eq "" ) {

            # Run pSake with no task name, this will display the available tasks
            Invoke-Psake .\_psake.ps1 -docs -nologo
  
            if ( $Error.Count -eq 0 ) {

                Write-Host "Please enter a task to run..."
                $taskName = Read-Host "="
            }
        }

        $buildProperties = @{ 
                "interactive" = $true; 
                "buildConfig" = $buildConfig; 
                "buildRevision" = $buildRevision;
            }

        Invoke-Psake .\_psake.ps1 $taskName -nologo -framework "4.0x64" -properties $buildProperties
        
        if($psake) {
            $buildSucceeded = $psake.build_success -eq $true
        }

        Write-Host ""
    }
    catch [system.exception] {
        Write-Host $_
        exit 1
    }

    if ($buildSucceeded) {
        exit 0
    }
    else {
        exit 1
    }


