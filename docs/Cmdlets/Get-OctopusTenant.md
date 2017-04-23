### Summary
Gets information about Octopus Tenants
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| TenantName | String[] |  Tenant name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusTenant [[-TenantName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
Gets all the tenants of the instance

``` powershell 
 Get-OctopusTenant
``` 

Gets the tenant with the name "MyAwesometenant"

``` powershell 
 Get-OctopusTenant -name "MyAwesomeTenant"
``` 

Gets the tenants with the names "MyAwesomeTenant" and "MyOtherAwesomeTenant"

``` powershell 
 Get-OctopusTenant -name "MyAwesomeTenant","MyOtherAwesomeTenant"
``` 

Gets all the tenants whose name matches the pattern "*AwesomeTenant"

``` powershell 
 Get-Channel -name "*AwesomeTenant"
``` 

