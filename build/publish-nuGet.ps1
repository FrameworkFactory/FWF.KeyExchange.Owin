   
function Publish-NuGet(
    [string]$nuget_name,
    [string]$nuspec_path,
    [string]$working_directory
    ) {

    $nuget_exe = "$scripts_directory\tools\nuget.exe"

    if ($buildConfig -eq 'RELEASE') {
        $nugetVersion = "$assemblyShortVersion.$buildRevision"
    }
    else  {
        $nugetVersion = "$assemblyShortVersion.$buildRevision-beta"
    }

    # Create a copy of the nuspec file
    $working_nuspec_file = "$working_directory\$nuget_name.nuspec"
    exec { . copy $nuspec_path -destination $working_nuspec_file -force } | Out-Null

    # Update the .nuspec version to the runtime version in the build scripts
    $nuspec_file_xml = [xml](cat "$working_nuspec_file")
    $nuspec_file_xml.package.metadata.version = $nugetVersion
    $nuspec_file_xml.Save("$working_nuspec_file")
    
    # Create the nupkg file
    exec { .$nuget_exe pack $working_nuspec_file -BasePath $working_directory -o "$publish_directory\nuget" -version $nugetVersion }
    
}
