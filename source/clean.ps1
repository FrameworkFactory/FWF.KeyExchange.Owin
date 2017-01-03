
    Set-StrictMode -Version latest
    Set-Location (Split-Path $MyInvocation.MyCommand.Path)

    $dirs = @( 'bin', 'obj', 'packages', 'node_modules', 'bower_components', '.tmp', '.vs' )
    $files = @( '*.dbmdl' )

    # Create a dummy empty directory
    $tmpDir = [System.IO.Path]::Combine( [System.IO.Path]::GetTempPath(), [System.IO.Path]::GetRandomFileName() )
    [System.IO.Directory]::CreateDirectory($tmpDir) | Out-Null
    
    try {
        Get-ChildItem -Path .\ -Directory -Include $dirs -Force -Recurse -ErrorAction Continue |
            % {
                Write-Host -ForegroundColor Red $_.FullName

                # To bypass any path too long exception, use robocopy to mirror an empty directory
                robocopy "$tmpDir" "$($_.FullName)" /mir

                # Then remove the newly emptied directory
                Remove-Item "$($_.FullName)" -Force -ErrorAction Continue
            }

        # Clean out other files
        Get-ChildItem -Path .\ -File -Include $files -Force -Recurse -ErrorAction Continue |
            % {
                Write-Host -ForegroundColor Red $_.FullName
                Remove-Item $_.FullName -Force -ErrorAction Continue
            }

        # Clean out matching
        Get-ChildItem -Path .\ -Directory -Filter 'DTAR_*_DTAR' -Force -Recurse |
            % {
                Write-Host -ForegroundColor Red $_.FullName

                # To bypass any path too long exception, use robocopy to mirror an empty directory
                robocopy "$tmpDir" "$($_.FullName)" /mir

                # Then remove the newly emptied directory
                Remove-Item "$($_.FullName)" -Force -ErrorAction Continue
            }
    }
    finally {
        [System.IO.Directory]::Delete($tmpDir) | Out-Null
    }
    