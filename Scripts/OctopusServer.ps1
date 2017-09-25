[CmdletBinding()]
Param(
	#Action to be executed
	[ValidateSet("StopService","StartService","CreateInstance","RemoveInstance","GenerateTestData")]
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
}

else{
    Throw "VM [$($env:computername)] doesn't have the Octopus Server installed. It needs to be installed to run tests against it"
}

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

		#. $PSscriptRoot\DSC_OctopusServer.ps1 -ConnectionString $ConnectionString  -OctopusInstance $config.OctopusInstance -Ensure Present -Port $config.OctopuswebListenPort -OctopusAdmin $config.OctopusAdmin -OctopusPassword $config.OctopusPassword -serviceState "Started" -Verbose

		#. $PSscriptRoot\DSC_OctopusServerUsernamePasswordAuth.ps1 -OctopusInstance $config.OctopusInstance		
		
		& "C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" create-instance --instance "$($config.OctopusInstance)" --config "C:\Octopus\$($config.OctopusInstance)\OctopusServer-$($config.OctopusInstance).config"
		& "C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" database --instance "$($config.OctopusInstance)" --connectionString $ConnectionString --create
		& "C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" configure --instance "$($config.OctopusInstance)" --upgradeCheck "false" --upgradeCheckWithStatistics "false" --webAuthenticationMode "UsernamePassword" --webForceSSL "False" --webListenPrefixes "http://localhost:$($config.OctopuswebListenPort)/" --commsListenPort "$($config.OctopusPollingPort)" --serverNodeName $env:computername
		& "C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" service --instance "$($config.OctopusInstance)" --stop
		& "C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" admin --instance "$($config.OctopusInstance)" --username "$($config.OctopusAdmin)" --email "user@user.com" --password "$($config.OctopusPassword)"
		& "C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" license --instance "$($config.OctopusInstance)" --licenseBase64 "PExpY2Vuc2UgU2lnbmF0dXJlPSJRQUZWeVN3QU83WDJ3QitLK1Q1cCs4ajNpYmM0WTR4WmY5c3U0c1pRUEdkM2IxUzE3UE5VWnpGQ0lvdXVOTmFITmJ1My9WVzdhbWFETW1JOVpvYUNFdz09Ij4NCiAgPExpY2Vuc2VkVG8+T2N0b3B1czwvTGljZW5zZWRUbz4NCiAgPExpY2Vuc2VLZXk+NTUxNzMtMzM3MjYtMjc0MjQtNjE0MTY8L0xpY2Vuc2VLZXk+DQogIDxWZXJzaW9uPjIuMDwvVmVyc2lvbj4NCiAgPFZhbGlkRnJvbT4yMDE1LTAxLTI4PC9WYWxpZEZyb20+DQogIDxNYWludGVuYW5jZUV4cGlyZXM+MjAxOC0wNi0wMTwvTWFpbnRlbmFuY2VFeHBpcmVzPg0KICA8UHJvamVjdExpbWl0PlVubGltaXRlZDwvUHJvamVjdExpbWl0Pg0KICA8TWFjaGluZUxpbWl0PlVubGltaXRlZDwvTWFjaGluZUxpbWl0Pg0KICA8VXNlckxpbWl0PlVubGltaXRlZDwvVXNlckxpbWl0Pg0KPC9MaWNlbnNlPg0K"
		& "C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" service --instance "$($config.OctopusInstance)" --install --reconfigure --start --username=$($config.OctopusServiceUser) --password=$($config.OctopusServicePassword) #--dependOn 'MSSQL$SQLEXPRESS'
	}

	#. $PSscriptRoot\DSC_OctopusServerUsernamePasswordAuth.ps1 -OctopusInstance $config.OctopusInstance		
}
elseif ($Action -eq "RemoveInstance"){

        If(ValidateIfInstanceExists){
            
            Write-Output "Removing Octopus Instance: $($Config.OctopusInstance)"
            
            $ConnectionString = "Server=$($env:computername)\$($config.SQLInstancename);Database=$($config.SQLDatabaseName);Integrated Security=SSPI"

            #. $PSscriptRoot\DSC_OctopusServer.ps1 -ConnectionString $ConnectionString -OctopusInstance $config.OctopusInstance -Ensure Absent -Port $config.OctopuswebListenPort -OctopusAdmin $config.OctopusAdmin -OctopusPassword $config.OctopusPassword -serviceState "Stopped" -Verbose
			
			&"C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" service --instance "$($config.OctopusInstance)" --stop --uninstall
			&"C:\Program Files\Octopus Deploy\Octopus\Octopus.Server.exe" delete-instance --instance "$($config.OctopusInstance)"

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
			"GenerateTestData" {
								
				$dllpath = get-item ".\Octoposh.TestDataGenerator\Publish\Octoposh.TestDataGenerator.dll"

				dotnet.exe $dllPath.FullName
				
			}			
		}
	}
	Else{
		Throw "Octopus Instance not found one $env:computername : $($Config.OctopusInstance)"
	}
}