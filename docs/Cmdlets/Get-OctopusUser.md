### Summary
This cmdlet returns info about Octopus Targets (Tentacles, cloud regions, Offline deployment targets, SHH)
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| UserName | String[] |  User Name. Accepts wildcard     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusUser [[-UserName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
**EXAMPLE 5**

Gets all the Users on the Octopus instance

 ``` powershell 
 PS C:\> Get-OctopusUser
 ``` 

**EXAMPLE 5**

Gets the user with the Username "Jotaro Kujo"

 ``` powershell 
 PS C:\> Get-OctopusUser -Username "Jotaro Kujo"
 ``` 

**EXAMPLE 5**

Gets the users with the Usernames "Jotaro Kujo" and "Dio Brando"

 ``` powershell 
 PS C:\> Get-OctopusUser -Username "Jotaro Kujo","Dio Brando"
 ``` 

**EXAMPLE 5**

Gets all the users whose username ends with "Joestar"

 ``` powershell 
 PS C:\> Get-OctopusUser -Username "*Joestar"
 ``` 

