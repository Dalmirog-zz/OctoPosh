param(
 [string]$Action,
 [string]$InstanceName,
 [string]$Password,
 [string]$CreateInstance
)

If(Test-Path HKLM:\SOFTWARE\Octopus\OctopusServer\){
    $OctopusInstallPath = (Get-ItemProperty HKLM:\SOFTWARE\Octopus\OctopusServer\).InstallLocation
    $OctopusServerexe = join-path $OctopusInstallPath "Octopus.Server.exe"
    $OctopusMigratorexe = join-path $OctopusInstallPath "Octopus.Migrator.exe"
}
else{
    Throw "VM [$($env:computername)] doesn't have the Octopus Server installed. It needs to be installed to run tests against it"
}

function ValidateIfInstanceExists {
    $Instances = (Get-ChildItem HKLM:\SOFTWARE\Octopus\OctopusServer\).PSChildname

    return $Instances.Contains($InstanceName)
}

$OctopusBackupDir = (Resolve-Path .\DataBackup\OctopusExport).path #Relative to the location of the .cake script, NOT to the location of this ps1 script.

If($Action -eq "CreateInstance"){
	If($CreateInstance -eq "True"){
		If(ValidateIfInstanceExists){
			Write-Output "Instance [$InstanceName]already exists, so we won't attempt to create it"
		}
		else{
			Write-Output "Creating Octopus Instance: $InstanceName"	
		}
	}
	Else{
		Write-Output "Not attempting to create Instance $InstanceName"
	}
}
else{
	If(ValidateIfInstanceExists){
		switch ($Action)
		{
			"StopService" {
				& $OctopusServerexe service --instance $InstanceName --stop
			}
			"StartService" {
				& $OctopusServerexe service --instance $InstanceName --start
					}
			"ImportBackup" {
				& $OctopusMigratorexe import --instance $InstanceName --directory $OctopusBackupDir --password $Password --overwrite --include-tasklogs
				}
			"ExportBackup" {
				& $OctopusMigratorexe export --instance $InstanceName --directory $OctopusBackupDir --password $Password
			}

			"RemoveInstance" {
				Write-Output "Deleting Octopus Instance: $instanceName"
			}
			Default {"yeah do nothing"}
		}
	}
	Else{
		Throw "Instance does not exist on $env:computername : $InstanceName"
	}
}