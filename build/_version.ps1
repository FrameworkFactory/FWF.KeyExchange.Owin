
    # Define build constansts
    $global:platformName = "FWF";
    $global:buildMajor = "1";
    $global:buildMinor = "0";
    $global:buildRelease = "0";
    
    #
    $global:assemblyFileVersion="$buildMajor.$buildMinor.$buildRelease.$buildRevision"
    $global:assemblyVersion="$buildMajor.$buildMinor.0.0"
    $global:assemblyShortVersion="$buildMajor.$buildMinor"

    #
    $global:moduleVersion="$buildMajor.$buildMinor.$buildRelease.$buildRevision"

    # Forward input parameters to global scope
    $global:buildConfig = "$buildConfig";
    $global:buildRevision = "$buildRevision";

