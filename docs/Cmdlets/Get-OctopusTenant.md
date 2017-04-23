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
**EXAMPLE 5**

Gets all the tenants of the instance

 ``` powershell 
 PS C:\> Get-OctopusTenant
 ``` 

**EXAMPLE 5**

Gets the tenant with the name "MyAwesometenant"

 ``` powershell 
 PS C:\> Get-OctopusTenant -name "MyAwesomeTenant"
 ``` 

**EXAMPLE 5**

Gets the tenants with the names "MyAwesomeTenant" and "MyOtherAwesomeTenant"

 ``` powershell 
 PS C:\> Get-OctopusTenant -name "MyAwesomeTenant","MyOtherAwesomeTenant"
 ``` 

**EXAMPLE 5**

Gets all the tenants whose name matches the pattern "*AwesomeTenant"

 ``` powershell 
 PS C:\> Get-Channel -name "*AwesomeTenant"
 ``` 

