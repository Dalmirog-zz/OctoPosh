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
**EXAMPLE 5**

Creates an Environment Resource object

 ``` powershell 
 PS C:\> $EnvironmentObj = Get-OctopusResourceModel -Resource Environment
 ``` 

