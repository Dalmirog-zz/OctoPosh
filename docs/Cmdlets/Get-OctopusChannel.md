### Summary
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| ChannelName | System.String[] |  Channel name     |
| Name | System.String[] |  Channel name   This is an alias of the ChannelName parameter.     |
| ProjectName | System.String[] |  Project name     |
| Project | System.String[] |  Project name   This is an alias of the ProjectName parameter.     |
| ResourceOnly | System.Management.Automation.SwitchParameter |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set  or set to FALSE, the cmdlet will return a human friendly Octoposh output object     |

### Syntax
``` powershell

Get-OctopusChannel [[-ChannelName] <string[]>] [[-ProjectName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]
``` 

### Examples
**EXAMPLE 1**

Gets all the channels in all the projects of the instance

```Powershell 
PS C:\> Get-OctopusChannel
``` 

**EXAMPLE 2**

Gets all the channels of the project "MyFinantialApp"

 ```Powershell 
PS C:\>PS C:\> Get-OctopusChannel -Project "MyFinantialApp"
 ``` 

**EXAMPLE 3**

Gets the Channel with the name "Hotfix_Website" of the project "MyFinantialApp"

 ```Powershell 
PS C:\>PS C:\> Get-OctopusChannel -name "Hotfix_Website" -Project "MyFinantialApp"
 ``` 

**EXAMPLE 4**

Gets the Channels with the names "Hotfix_Website" and "Hotfix_WebService" of the project "MyFinantialApp"

 ```Powershell 
PS C:\>PS C:\> Get-OctopusChannel -name "Hotfix_Website","Hotfix_WebService" -Project "MyFinantialApp"
 ``` 

**EXAMPLE 5**

Gets all the Channels whose name starts with "Hotfix_" of the project "MyFinantialApp"

 ```Powershell 
PS C:\>PS C:\> Get-OctopusChannel -name "Hotfix_*" -Project "MyFinantialApp"
 ``` 

