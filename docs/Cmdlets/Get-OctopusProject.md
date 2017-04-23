### Summary
This cmdlet returns info about Octopus Projects
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| ProjectName | String[] |  Project name     |
| ProjectGroupName | String[] |  Gets all projects inside a set of Project Groups     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusProject [[-ProjectName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]

Get-OctopusProject [[-ProjectGroupName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
**EXAMPLE 5**

Gets all the projects of the current Instance

 ``` powershell 
 PS C:\> Get-OctopusProject
 ``` 

**EXAMPLE 5**

Get the project named "MyProject"

 ``` powershell 
 PS C:\> Get-OctopusProject -name MyProject
 ``` 

**EXAMPLE 5**

Get all the projects whose name starts with the string "MyApp"

 ``` powershell 
 PS C:\> Get-OctopusProject -name MyApp*
 ``` 

**EXAMPLE 5**

Gets all the projects inside of the Project Group "MyProduct"

 ``` powershell 
 PS C:\> Get-OctopusProject -ProjectGroupName "MyProduct"
 ``` 

