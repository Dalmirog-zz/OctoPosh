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
**EXAMPLE 1**

Gets all the projects of the current Instance

``` powershell 
 Get-OctopusProject
``` 

**EXAMPLE 2**

Get the project named "MyProject"

``` powershell 
 Get-OctopusProject -name MyProject
``` 

**EXAMPLE 3**

Get all the projects whose name starts with the string "MyApp"

``` powershell 
 Get-OctopusProject -name MyApp*
``` 

**EXAMPLE 4**

Gets all the projects inside of the Project Group "MyProduct"

``` powershell 
 Get-OctopusProject -ProjectGroupName "MyProduct"
``` 

