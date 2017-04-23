### Summary
Sets the current Octopus connection info (URL and API Key). Highly recommended to call this function from $profile to avoid having to re-configure this on every session.
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Server | String |  URL of the server you want to connect to     |
| ApiKey | String |  API Key you'll use to authenticate with the Octopus Server     |

### Syntax
``` powershell

Set-OctopusConnectionInfo [-Server] <string> [-ApiKey] <string> [<CommonParameters>]




``` 

### Examples
Set connection info with a specific API Key for an Octopus instance

``` powershell 
 Set-OctopusConnectionInfo -Server "http://MyOctopus.AwesomeCompany.com" -API "API-7CH6XN0HHOU7DDEEUGKUFUR1K"
``` 

