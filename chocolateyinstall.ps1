try {

    $modulepath = Join-Path $env:ProgramFiles WindowsPowershell\Modules

    $TargetDirectory = Join-Path $modulepath Octoposh

    If (!(Test-Path $TargetDirectory)){
        Write-Warning "$TargetDirectory doesnt exist, attempting to create"
        New-Item $TargetDirectory -ItemType Directory -Verbose
    }

    If((Get-ChildItem $TargetDirectory).count -gt 0){
        Write-Warning "Deleting content of $TargetDirectory"
        Remove-Item $TargetDirectory\* -Recurse -Exclude "*.dll"
    }

    Write-Host "Copying module files to $TargetDirectory"
    
    Get-ChildItem $modulepath\* -Recurse -Exclude *.nupkg,chocolateyinstall.ps1,*.dll | Copy-Item -Destination $TargetDirectory -Force
    
    Write-Host "Files copied. Close this console and re-open to be able to start using the module"
} 

catch {
    Write-ChocolateyFailure 'Octoposh' $_.Exception.Message
    throw 
}


