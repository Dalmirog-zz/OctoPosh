##This DSC config will only take care of DELETING the SQL database of the Octopus instance.
##The new DB for the new instance will be created by Octopus.server.exe

[CmdletBinding()]
param (
    [string]$SQLServerName = $env:COMPUTERNAME,
    [string]$SQLInstanceName,
    [string]$SQLDatabaseName

)

if((Get-DscResource -Module xSqlServer) -eq $null){
    Write-Output "DSC Resource [xSQLServer] not installed. Downloading from PSGallery..."
    Install-Module -Name xSQLServer -Force
}

configuration DSC_SQL {

    Import-DscResource -ModuleName xSqlserver

    node localhost {
        xSQLServerDatabase "OctopusDatabase"{
         Name = $SQLDatabaseName
         SQLServer = $SQLServerName
         SQLInstanceName = $SQLInstanceName
         Ensure = "Absent"
        }
    }
}

DSC_SQL -OutputPath $PSScriptRoot\DSC_SQL
Start-DscConfiguration $PSScriptRoot\DSC_SQL -Force -Wait