# Octopus Deploy Powershell Module

Octopus Deploy is a friendly deployment automation system for .NET developers. [Its architecture is built API-First](http://docs.octopusdeploy.com/display/OD/Octopus+REST+API), meaning 99% of what you can do from the Octopus UI can also be performed using the REST API.

This Powershell Module contains a set of CMDLets that use the Octopus REST API to perform common administrative tasks.

DISCLAIMER: This is an open source project which is NOT supported by Octopus Deploy. All questions/bugs about this module should be entered on this github project.

##Installing the module

1. Download the module
2. Right click on the downloaded zip -> **Properties -> **Unblock File**
3. Create a folder called *OctoPosh* under your [PSModulePath](https://msdn.microsoft.com/en-us/library/dd878326%28v=vs.85%29.aspx). Use *C:\Program Files\WindowsPowerShell\Modules* if its on the list of Module Paths.
4. Extract the contents of the zip on the new *OctoPosh* folder
5. Open a powershell console and run ```Get-command -module OctoPosh``` to list all the module's cmdlets

##Getting Started

Check out [Wiki](https://github.com/Dalmirog/OctoPosh/wiki)

##Cmdlets Overview

| Cmdlet | Description          |
| ------------- | ----------- |
| New-OctopusAPIKey     | Creates an API Key for a user|
| New-OctopusConnection     | Creates a connection with an Octopus Server|
| Get-OctopusDeployment     | Gets information about Octopus Deployments|
| Get-OctopusRelease     | Gets information about Octopus Releases|
| New-OctopusResourceModel     | Returns an empty Octopus resource object|
| Get-OctopusMaintenanceMode     | Gets current Octopus Maintenance mode status|
| Get-OctopusEnvironment     | Gets information about Octopus Environments|
| Get-OctopusProject     | Gets information about Octopus Projects|
| Get-OctopusProjectGroup     | Gets information about Octopus Project Groups|
| Get-OctopusProjectVariable     | Gets the variable sets of Octopus Projects|
| Get-OctopusOctopusSMTPConfig     | Gets current Octopus SMTP Config|
| Get-OctopusConnectionInfo      | Gets the current Octopus Connection info|
| Set-OctopusConnectionInfo     | Sets the Octopus Connection info|
| Set-OctopusMaintenanceMode     | Turns Octopus Maintenance mode On/Off|
| Set-OctopusUserAccountStatus     | Enables/Disables an Octopus user account|
| Set-OctopusOctopusSMTPConfig     | Sets Octopus SMTP Config|
| Block-OctopusRelease     | Blocks an Octopus release|
| Unblock-OctopusRelease     | Unblocks an Octopus release|
| Remove-OctopusResource     | Deletes an Octopus Resource|

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
| TeamCity    | https://www.jetbrains.com/teamcity/ |