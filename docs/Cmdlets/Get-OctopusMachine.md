### Summary
This cmdlet returns info about Octopus Targets (Tentacles, cloud regions, Offline deployment targets, SHH)
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| MachineName | String[] |  Name of the Machine to filter by     |
| EnvironmentName | String[] |  Name of the Environment to filter by     |
| URL | String[] |  Target URI to filter by     |
| CommunicationStyle | String |  Target communication style to filter by     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusMachine [[-MachineName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]

Get-OctopusMachine [-EnvironmentName <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]

Get-OctopusMachine [-ResourceOnly <SwitchParameter>] [-URL <string[]>] [<CommonParameters>]

Get-OctopusMachine [-CommunicationStyle <string>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
**EXAMPLE 1**

Gets the machine with the name "Database_Prod"

``` powershell 
 Get-OctopusMachine -name "Database_Prod"
``` 

**EXAMPLE 2**

Gets all the machines which name is like "*_Prod"

``` powershell 
 Get-OctopusMachine -name "*_Prod"
``` 

**EXAMPLE 3**

Gets all the machines on the environments "Staging","UAT"

``` powershell 
 Get-OctopusMachine -EnvironmentName "Staging","UAT""
``` 

**EXAMPLE 4**

Gets all the machines with the string "*:10933" at the end of the URL

``` powershell 
 Get-OctopusMachine -URL "*:10933"
``` 

**EXAMPLE 5**

Gets all the machines registered in "Listening" mode. "Polling" is also a valid value

``` powershell 
 PS Get-OctopusMachine -Mode Listening
``` 

