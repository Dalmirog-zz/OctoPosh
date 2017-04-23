### Summary
This cmdlet downloads Octo.exe from Nuget to the "Octopus Tools Folder".To learn more about this path run Get-Help Set-OctopusToolsFolder
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Version | String |  Downloads a specific version oc Octo.Exe from nuget     |
| Latest | Switch |  Tells the cmdlet to download the latest version of Octo.exe available in Nuget     |
| SetAsDefault | Switch |  If set to true, the cmdlet will set the just downloaded version of Octo.exe as the default one, making it instantly available using $env:Octoexe     |

### Syntax
``` powershell

Install-OctopusTool -Version <string> [-SetAsDefault <SwitchParameter>] [<CommonParameters>]

Install-OctopusTool [-Latest <SwitchParameter>] [-SetAsDefault <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
Downloads the latest version of Octo.exe to the Octopus Tools Folder.

 ``` powershell 
 PS C:\> Install-OctopusTool -Latest
 ``` 

Downloads version 1.0.0 of Octo.exe to the Octopus Tools Folder.

 ``` powershell 
 PS C:\> Install-OctopusTool -version 1.0.0
 ``` 

Downloads version 1.0.0 of Octo.exe to the Octopus Tools Folder and also sets it as the current defaul version

 ``` powershell 
 PS C:\> Install-OctopusTool -version 1.0.0 -SetAsDefault
 ``` 

