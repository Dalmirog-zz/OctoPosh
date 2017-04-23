### Summary
This cmdlet returns Octopus Teams
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| TeamName | String[] |  Team name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusTeam [[-TeamName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
**EXAMPLE 5**

Gets all the teams on the Octopus instance

 ``` powershell 
 PS C:\> Get-OctopusTeam
 ``` 

**EXAMPLE 5**

Gets the team with the name "ProjectA_Managers"

 ``` powershell 
 PS C:\> Get-OctopusTeam -name "ProjectA_Managers"
 ``` 

**EXAMPLE 5**

Gets the teams with the names "ProjectA_Managers" and "ProjectA_Developers"

 ``` powershell 
 PS C:\> Get-OctopusTeam -name "ProjectA_Managers","ProjectA_Developers"
 ``` 

**EXAMPLE 5**

Gets all the teams whose name starts with "ProjectA"

 ``` powershell 
 PS C:\> Get-OctopusTeam -name "ProjectA*"
 ``` 

