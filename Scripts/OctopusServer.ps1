[CmdletBinding()]
Param(
	#Action to be executed
	[ValidateSet("StopService","StartService","ConfigFile","CreateInstance","RemoveInstance","ImportBackup","ExportBackup")]
	[string]$Action, 

	#Path of the config.json file to be used by this script which should hold connection strings, Octopus user/pass and all that.	
	[string]$ConfigFile = ".\devenvconfig.json"
)

function ValidateIfInstanceExists {
    $Instances = (Get-ChildItem HKLM:\SOFTWARE\Octopus\OctopusServer\).PSChildname
    If($instances.count -ne 0){
        return $Instances.Contains($Config.OctopusInstance)
    }
    else{
        return $false
    }
}

If(Test-Path HKLM:\SOFTWARE\Octopus\OctopusServer\){
    $OctopusInstallPath = (Get-ItemProperty HKLM:\SOFTWARE\Octopus\OctopusServer\).InstallLocation
    $OctopusServerexe = join-path $OctopusInstallPath "Octopus.Server.exe"
    $OctopusMigratorexe = join-path $OctopusInstallPath "Octopus.Migrator.exe"
}

else{
    Throw "VM [$($env:computername)] doesn't have the Octopus Server installed. It needs to be installed to run tests against it"
}

#$OctopusExportDir = (Resolve-Path $PSScriptRoot\..\DataBackup\OctopusExport).path #Relative to the location of the .cake script, NOT to the location of this ps1 script.
$OctopusExportDir = ".\OctopusExport"

Write-Output "Using config file: $ConfigFile"

$Config = Get-Content $ConfigFile | ConvertFrom-Json

If($Action -eq "CreateInstance"){

	Write-Output "Checking if instance exists: $($Config.OctopusInstance)"

	If(ValidateIfInstanceExists){
		Write-Output "Instance [$($Config.OctopusInstance)]already exists, so we won't attempt to create it"
	}
	else{
		Write-Output "Instance not found. Creating: $($Config.OctopusInstance)"	
		
		$ConnectionString = "Server=$($env:computername)\$($config.SQLInstancename);Database=$($config.OctopusInstance);Integrated Security=SSPI"

		. $PSscriptRoot\DSC_OctopusServer.ps1 -ConnectionString $ConnectionString  -OctopusInstance $config.OctopusInstance -Ensure Present -Port $config.OctopuswebListenPort -OctopusAdmin $config.OctopusAdmin -OctopusPassword $config.OctopusPassword -serviceState "Started" -Verbose

		. $PSscriptRoot\DSC_OctopusServerUsernamePasswordAuth.ps1 -OctopusInstance $config.OctopusInstance

	}
}
elseif ($Action -eq "RemoveInstance"){

        If(ValidateIfInstanceExists){
            
            Write-Output "Removing Octopus Instance: $($Config.OctopusInstance)"
            
            $ConnectionString = "Server=$($env:computername)\$($config.SQLInstancename);Database=$($config.SQLDatabaseName);Integrated Security=SSPI"

            . $PSscriptRoot\DSC_OctopusServer.ps1 -ConnectionString $ConnectionString -OctopusInstance $config.OctopusInstance -Ensure Absent -Port $config.OctopuswebListenPort -OctopusAdmin $config.OctopusAdmin -OctopusPassword $config.OctopusPassword -serviceState "Stopped" -Verbose
        
            . $PSScriptRoot\DSC_SQL.ps1 -Verbose -SQLInstanceName $config.SQLInstancename -SQLDatabaseName $config.SQLDatabaseName
        }
        else{
            Write-Output "Not attempting to remove Instance [$($Config.OctopusInstance)] because it does not exist on $($env:computername)"
        } 
}
else{
	If(ValidateIfInstanceExists){
		switch ($Action)
		{
			"StopService" {
				& $OctopusServerexe service --instance $Config.OctopusInstance --stop
			}
			"StartService" {
				& $OctopusServerexe service --instance $Config.OctopusInstance --start
					}
			"ImportBackup" {
				& $OctopusMigratorexe import --instance $Config.OctopusInstance --directory "$OctopusExportDir" --password $config.OctopusPassword --overwrite --include-tasklogs
				}
			"ExportBackup" {				
				& $OctopusMigratorexe export --instance $config.OctopusInstance --directory $OctopusExportDir --password $config.OctopusPassword
			}
		}
	}
	Else{
		Throw "Octopus Instance not found one $env:computername : $($Config.OctopusInstance)"
	}
}