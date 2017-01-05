

Task Publish.NuGet -description "Create NuGet packages" {

    # Create a working directory to copy artifacts
    $working_directory = "$env:temp\FWF\publish\nuget"

    # Clear the publish location first
    md "$publish_directory\nuget" -Force | Out-Null
    del "$publish_directory\nuget\*.*" -Recurse -Force | Out-Null

    try {

        # FWF.KeyExchange.Owin
        $module_directory = "$working_directory\FWF.KeyExchange.Owin"

        # Copy all dependencies to a working directory
        robocopy "$src_directory\FWF.KeyExchange.Owin\bin\$buildConfig" "$module_directory\lib\net462" "FWF.*.dll" /mir | Out-Null 

        # Setup the package before calling nuget.exe pack
        Publish-NuGet `
            -nuget_name 'FWF.KeyExchange.Owin' `
            -nuspec_path "$scripts_directory\nuspec\FWF.KeyExchange.Owin.nuspec" `
            -working_dir $module_directory
            
    }
    finally {
        del "$working_directory\*.*" -Recurse | Out-Null
    }

}

Task Push.NuGet.Local -description "Push NuGet packages to local NuGet server" {

    Get-ChildItem "$publish_directory\nuget\*.nupkg" | % {
        exec {
            & "$scripts_directory\tools\nuget.exe" push $_.FullName -Source 'http://localhost/nuget/' 
        }
    }

}


Task Push.NuGet -description "Push NuGet packages to NuGet server" {

    Get-ChildItem "$publish_directory\nuget\*.nupkg" | % {
        exec {
            & "$scripts_directory\tools\nuget.exe" push $_.FullName -Source 'https://www.nuget.org/api/v2/package' 
        }
    }

}


