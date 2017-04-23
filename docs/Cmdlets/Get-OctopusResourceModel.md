### Summary
This cmdlet creates instances of Octopus Resource Objects
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Resource | String |  Resource object model     |

### Syntax
``` powershell

Get-OctopusResourceModel -Resource <string> [<CommonParameters>]




``` 

### Examples
**EXAMPLE 1**

Creates an Environment Resource object

``` powershell 
 $EnvironmentObj = Get-OctopusResourceModel -Resource Environment
``` 

