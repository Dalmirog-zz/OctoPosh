### Summary
Gets information about Octopus Releases
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| ReleaseVersion | String[] |  Release version number     |
| ProjectName | String |  Name of project to filter releases. Only one Project can be passed to this parameter at a time     |
| Latest |  |  Get latest X releases     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusRelease [-ProjectName] <string> [[-ReleaseVersion] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]

Get-OctopusRelease [-ProjectName] <string> [-Latest <int>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
Get all the realeases of the project "MyProject"

 ``` powershell 
 PS C:\> Get-OctopusRelease -ProjectName "MyProject"
 ``` 

Get the release realeases 1.0.1 & 1.0.2 of the project "MyProject"

 ``` powershell 
 PS C:\> Get-OctopusRelease -ProjectName "MyProject" -version 1.0.1,1.0.2
 ``` 

Get the latest 10 releases of the project "MyProject"

 ``` powershell 
 PS C:\> Get-OctopusRelease -ProjectName "MyProject" -Latest 10
 ``` 

