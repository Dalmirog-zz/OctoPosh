### Summary
This cmdlet sets a version of Octo.exe as default. To get the value of the current default Octo.exe version run Get-OctopusToolPath or simply use $env:OctoExe
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Version | String |  Sets the value of the default Octo.exe based on the version of it found by Get-OctopustoolsVersion     |
| Path | String |  Sets the value of the default Octo.exe using a literal path. The cmdlet won't validate if the path leads to an existing octo.exe file     |

### Syntax
``` powershell

Set-OctopusToolPath -Version <string> [<CommonParameters>]

Set-OctopusToolPath [-Path] <string> [<CommonParameters>]




``` 

### Examples
Sets C:\Tools\1.0.0\Octo.exe as the current default Octo.exe version

 ``` powershell 
 PS C:\> Set-OctopusToolPath -path C:\tools\1.0.0\Octo.exe
 ``` 

Uses Get-OctopusToolVersion to look for Octo.exe version 1.0.0 and then sets its path as the current default version

 ``` powershell 
 PS C:\> Set-OctopusToolPath -version 1.0.0
 ``` 

Gets the latest version of Octo.exe installed on the machine using Get-OctopusToolsVersion and sets $env:OctoExe with its path

 ``` powershell 
 PS C:\>Get-OctopusToolsVersion -latest | Set-OctopusToolPath
 ``` 

