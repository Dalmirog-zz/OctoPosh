### Summary
Sets the current Octopus connection info (URL and API Key). Highly recommended to call this function from $profile to avoid having to re-configure this on every session.
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
Set connection info with a specific API Key for an Octopus instance

``` powershell 
 Set-OctopusConnectionInfo -Server "http://MyOctopus.AwesomeCompany.com" -API "API-7CH6XN0HHOU7DDEEUGKUFUR1K"
``` 

