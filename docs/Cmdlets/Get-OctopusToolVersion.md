### Summary
Gets a list of available versions of Octo.exe in the folder set on the "Octopus Tools Folder". If you don't know what this path is run [Get-Help Set-OctopusToolsFolder] to learn more about it
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Latest | Switch |  If set to TRUE the cmdlet will only return the highest version of Octo.exe found on the child folders $env:OctopusToolsFolder. If you don't  know what this path is run [Get-Help Set-OctopusToolsFolder] to learn more about it     |
| Version | String |  Gets a specific version of Octo.exe     |

### Syntax
``` powershell

Get-OctopusToolVersion [-Latest <SwitchParameter>] [<CommonParameters>]

Get-OctopusToolVersion [[-Version] <string>] [<CommonParameters>]




``` 

### Examples
**EXAMPLE 1**

Gets a list of available versions of Octo.exe in the folder set on the "Octopus Tools Folder".

``` powershell 
 Get-OctopusToolVersion
``` 

**EXAMPLE 2**

Gets the latest version of Octo.exe in the folder set on the "Octopus Tools Folder".

``` powershell 
 Get-OctopusToolVersion -latest
``` 

