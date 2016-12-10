[CmdletBinding()]
Param(
	#Action to be executed
	[ValidateSet("StopService","StartService","ConfigFile","CreateInstance","RemoveInstance","ImportBackup","ExportBackup")]
	[string]$Action, 

	#To create an instance you need to pass the STRING "True" to this param and the string "CreateInstance" to the [Action] param. Couldn't figure out how to tell cake to avoid a task based on a param, so I'm always running the Action "CreateInstance" but only actually creating it if this is "True".
	[bool]$CreateInstance = $false, 
	
	#Same story as with $CreateInstance
	[bool]$RemoveInstance = $false, 

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

function CreateOctopusInstance {
	& $OctopusServerexe create-instance --instance $Config.OctopusInstance --config "C:\Octopus\$($config.OctopusInstance)\OctopusServer-$($Config.OctopusInstance).config" --console
	& $OctopusServerexe configure --instance $Config.OctopusInstance --home "C:\Octopus\$($Config.OctopusInstance)" --storageConnectionString $config.ConnectionString --upgradeCheck "False" --upgradeCheckWithStatistics "False" --webAuthenticationMode "UsernamePassword" --webForceSSL "False" --webListenPrefixes "http://localhost:$($config.OctopuswebListenPort)/" --commsListenPort $Config.OctopusPollingPort --serverNodeName $env:computername --console
	& $OctopusServerexe database --instance $Config.OctopusInstance --create --grant "NT AUTHORITY\SYSTEM" --console
	& $OctopusServerexe service --instance $Config.OctopusInstance --stop --console
	& $OctopusServerexe admin --instance $Config.OctopusInstance --username $config.OctopusAdmin --password $config.OctopusPassword --console
	& $OctopusServerexe license --instance $Config.OctopusInstance --licenseBase64 $config.OctopusLicenseBase64 --console
	& $OctopusServerexe service --instance $Config.OctopusInstance --install --reconfigure --start --console
}

#$OctopusExportDir = (Resolve-Path $PSScriptRoot\..\DataBackup\OctopusExport).path #Relative to the location of the .cake script, NOT to the location of this ps1 script.
$OctopusExportDir = ".\OctopusExport"
Write-Output "Using config file: $ConfigFile"
$Config = Get-Content $ConfigFile | ConvertFrom-Json

If($Action -eq "CreateInstance"){
	If($CreateInstance){
        Write-Output "Checking if instance exists: $($Config.OctopusInstance)"

		If(ValidateIfInstanceExists){
			Write-Output "Instance [$($Config.OctopusInstance)]already exists, so we won't attempt to create it"
		}
		else{
			Write-Output "Instance not found. Creating: $($Config.OctopusInstance)"	
			CreateOctopusInstance
		}
	}
	Else{
		Write-Output "Not attempting to create Instance: $($Config.OctopusInstance)"
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

			"RemoveInstance" {
				If($RemoveInstance){
					Write-Output "Removing Octopus Instance: $($Config.OctopusInstance)"
					& $OctopusServerexe service --instance $Config.OctopusInstance --stop --uninstall
					& $OctopusServerexe delete-instance --instance $Config.OctopusInstance
				}
				else
				{
					Write-Output "Not attempting to remove Instance: $($Config.OctopusInstance)"
				}				
			}
		}
	}
	Else{
		Throw "Octopus Instance not found one $env:computername : $($Config.OctopusInstance)"
	}
}