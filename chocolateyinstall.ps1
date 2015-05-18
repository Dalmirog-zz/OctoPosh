try {

    $modulepath = Join-Path $env:ProgramFiles WindowsPowershell\Modules

    $TargetDirectory = Join-Path $modulepath Octoposh

    If (!(Test-Path $TargetDirectory)){
        Write-Warning "$TargetDirectory doesnt exist, attempting to create"
        New-Item $TargetDirectory -ItemType Directory -Verbose
    }

    If((Get-ChildItem $TargetDirectory).count -gt 0){
        Write-Warning "Deleting content of $TargetDirectory"
        Remove-Item $TargetDirectory\* -Recurse -Verbose
    }

    Write-Host "Copying module files to $TargetDirectory"
    Copy-Item $PSScriptRoot\* -Destination $TargetDirectory -Force -Recurse -Exclude *.nupkg,chocolateyinstall.ps1

    Write-ChocolateySuccess 'Octoposh'
} 

catch {
    Write-ChocolateyFailure 'Octoposh' $_.Exception.Message
    throw 
}


