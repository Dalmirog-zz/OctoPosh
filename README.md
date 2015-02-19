# Octopus Deploy Powershell Module

The goal of this module is to provide a set of useful cmdlets to perform common administration tasks on Octopus Deploy 

##Installing the module

1. Download the module
2. Right click on the downloaded zip -> Properties -> Unblock File
3. Create a folder called *OctopusDeploy* under your [PSModulePath](https://msdn.microsoft.com/en-us/library/dd878326%28v=vs.85%29.aspx). Use *C:\Program Files\WindowsPowerShell\Modules* if its on the list of Module Paths.
4. Extract the contents of the zip on the new *OctopusDeploy* folder
5. Open a powershell console and run ```Get-command -module OctopusDeploy``` to list all the module's cmdlets

##Getting Started

To start using the **OctopusDeploy** module you'll need to set values on the variables ```$env:OctopusURI``` and ```$env:OctopusAPIKey``` to do so you can do

```
Set-OctopusConnectionInfo -URL "Your Octopus Server URI" -APIKey "Your Octopus API Key"
```

or

```
$env:OctopusURI = "Your Octopus server URI"
$env:OctopusAPIKey = "Your API Key"
```

Once the values are set you can check them out using ```Get-OctopusConnectionInfo```

##Cmdlets Overview

| Cmdlet | Description          |
| ------------- | ----------- |
| Get-OctopusConnectionInfo      | Gets the current Octopus Connection info|
| Set-OctopusConnectionInfo     | Sets the Octopus Connection info|
| Get-OctopusDeployments     | Gets information about Octopus Deployments|
| New-OctopusAPIKey     | Creates an API Key for a user|
| Set-OctopusMaintenanceMode     | Turns Octopus Maintenance mode On/Off|
| Set-OctopusUserAccountStatus     | Enables/Disables an Octopus user account|
| Block-OctopusRelease     | Blocks an Octopus release|
| Unblock-OctopusRelease     | Unblocks an Octopus release|
| Get-OctopusOctopusSMTPConfig     | Gets current Octopus SMTP Config|
| Set-OctopusOctopusSMTPConfig     | Sets Octopus SMTP Config|
| Remove-OctopusResource     | Deletes an Octopus Resource|
| Get-OctopusResourceModel     | Returns an empty object from the Octopus Model|


##Cmdlets Syntax

Run ```Get-help [cmdlet]```

##Credits

This module was made using the following awesome tools

| Name | Site|
| ------------- | ----------- |
| Octopus Deploy      | https://octopusdeploy.com/|
| Pester | https://github.com/pester/Pester|
| Fiddler | http://www.telerik.com/fiddler |
| Papercut     | https://papercut.codeplex.com/ |