
### Summary

This cmdlet returns information about Octopus Lifecycles
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| LifecycleName | String[] |  Lifecycle name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the  cmdlet will return a human friendly Octoposh output object     |

### Syntax
``` powershell

Get-OctopusLifecycle [[-LifecycleName] <string[]>] [-ResourceOnly <SwitchParameter>] 
[<CommonParameters>]




``` 

### Examples 

**EXAMPLE 1**

Get all the Lifecycles of the current Instance

``` powershell 
 Get-OctopusLifecycle
``` 

**EXAMPLE 2**

Get the Lifecycle named "MyLifecycle"

``` powershell 
 Get-OctopusLifecycle -name MyLifecycle
``` 

**EXAMPLE 3**

Gets the teams with the names "ProjectA_Lifecycle" and "ProjectB_Lifecycle"

``` powershell 
 Get-OctopusLifecycle -name "ProjectA_Lifecycle","ProjectB_Lifecycle"
``` 

**EXAMPLE 4**

Gets all the lifecycles whose name starts with "ProjectA"

``` powershell 
 Get-OctopusLifecycle -name "ProjectA*"
``` 

