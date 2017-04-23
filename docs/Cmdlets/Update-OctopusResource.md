### Summary
Updates resources from an Octopus Instance. This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Resource |  |  Resource Object     |

### Syntax
``` powershell

Update-OctopusResource [[-Resource] <Resource>] [<CommonParameters>]




``` 

### Examples
Updates the Name of a ProjectGroup

``` powershell 
 $pg = Get-OctopusProjectGroup -name SomeProjectName ; $pg.resource.name = "SomeOtherProjectName" ; Update-OctopusResource -resource $pg.resource
``` 

Updates the [IsDisabled] property of a machine to disable it

``` powershell 
 $machine = Get-OctopusMachine -MachineName "SQL_Production" ; $machine.resource.isdisabled = $true ; Update-OctopusResource -resource $machine.resource
``` 

