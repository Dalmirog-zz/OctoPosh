
### Summary

Gets information about Octopus Environments
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| EnvironmentName | String[] |  Environment name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the  cmdlet will return a human friendly Octoposh output object     |

### Syntax
``` powershell

Get-OctopusEnvironment [[-EnvironmentName] <string[]>] [-ResourceOnly <SwitchParameter>] 
[<CommonParameters>]




``` 

### Examples 

**EXAMPLE 1**

Gets info about the environment "Production"

``` powershell 
 Get-OctopusEnvironment -name Production
``` 

**EXAMPLE 2**

Gets info about all the environments whose name matches the pattern "FeatureTest*"

``` powershell 
 Get-OctopusEnvironment -name "FeatureTest*"
``` 

