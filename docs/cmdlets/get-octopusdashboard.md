
### Summary

Returns the Octopus Dashboard
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| ProjectName | String[] |  Name of the Project to filter for.     |
| EnvironmentName | String[] |  Name of the Project to filter for.     |
| DeploymentStatus | String[] |  Target communication style to filter by     |

### Syntax
``` powershell

Get-OctopusDashboard [-DeploymentStatus <string[]>] [-EnvironmentName <string[]>] [-ProjectName <string[]>] [<CommonParameters>]




``` 

### Examples 

**EXAMPLE 1**

Gets the entire Octopus dashboard

``` powershell 
 Get-OctopusDashboard
``` 

**EXAMPLE 2**

Gets the dashboard info for the project MyWebApp

``` powershell 
 Get-OctopusDashboard -ProjectName MyWebApp
``` 

**EXAMPLE 3**

Gets the dashboard info for all the projects that have a release deployed to the "Production" environment.

``` powershell 
 Get-OctopusDashboard -EnvironmentName Production
``` 

**EXAMPLE 4**

Gets all the deployments in "Success" status on the dashboard

``` powershell 
 Get-OctopusDashboard -DeploymentStatus Success
``` 

