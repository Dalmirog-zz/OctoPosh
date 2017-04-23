### Summary
This cmdlet sets the path of the "Octopus Tools Folder". This folder is where Install-OctopusTool will download Octo.exe, and its also from where Get-OctopusToolVersion will resolve the path of the downloaded Octo.exe versions.
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Path | String |  Sets the path of the "Octopus Tools folder".     |

### Syntax
``` powershell

Set-OctopusToolsFolder [-Path] <string> [<CommonParameters>]




``` 

### Examples
Sets the "Octopus Tools Folder" to "C:\Tools"

``` powershell 
 Set-OctopusToolsFolder -path C:\tools
``` 

