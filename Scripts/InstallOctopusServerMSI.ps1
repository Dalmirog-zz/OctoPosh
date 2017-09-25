[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [string]$DownloadUrl = "https://octopus.com/downloads/latest/WindowsX64/OctopusServer"
)

function Install-MSI
{
    param (
        [string]$downloadUrl
    )
    Write-Verbose "Beginning installation"

    mkdir "$($env:SystemDrive)\Octopus" -ErrorAction SilentlyContinue

    $msiPath = "$($env:SystemDrive)\Octopus\Octopus-x64.msi"
    
    if ((Test-Path $msiPath) -eq $true)
    {
        Remove-Item $msiPath -force
    }

    Request-File $downloadUrl $msiPath

    Write-output "Installing MSI..."
    if (-not (Test-Path "$($env:SystemDrive)\Octopus\logs")) { New-Item -type Directory "$($env:SystemDrive)\Octopus\logs" }
    $msiLog = "$($env:SystemDrive)\Octopus\logs\Octopus-x64.msi.log"
    $msiExitCode = (Start-Process -FilePath "msiexec.exe" -ArgumentList "/i $msiPath /quiet /l*v $msiLog" -Wait -Passthru).ExitCode
    Write-output "MSI installer returned exit code $msiExitCode"
    if ($msiExitCode -ne 0)
    {
        throw "Installation of the MSI failed; MSIEXEC exited with code: $msiExitCode. View the log at $msiLog"
    }    
}

function Request-File
{
    param (
        [string]$url,
        [string]$saveAs
    )

    Write-Verbose "Downloading $url to $saveAs"
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12,[System.Net.SecurityProtocolType]::Tls11,[System.Net.SecurityProtocolType]::Tls
    $downloader = new-object System.Net.WebClient
    try {
      $downloader.DownloadFile($url, $saveAs)
    }
    catch
    {
       throw $_.Exception.InnerException
    }
}


Write-output "Checking if Octopus Server is installed"
$installLocation = (Get-ItemProperty -path "HKLM:\Software\Octopus\OctopusServer" -ErrorAction SilentlyContinue).InstallLocation

if($installLocation -ne $null){
    Write-output "Octopus Server is installed on [$env:COMPUTERNAME]"
}

else{
    Write-Host "Octopus Server is not installed on [$env:COMPUTERNAME]"
    Install-MSI -downloadUrl $DownloadUrl
}
