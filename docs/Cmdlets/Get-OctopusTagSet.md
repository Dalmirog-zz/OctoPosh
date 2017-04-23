### Summary
Gets information about Octopus TagSets
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| TagSetName | String[] |  TagSet name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusTagSet [[-TagSetName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
**EXAMPLE 5**

Gets all the TagSets of the instance

 ``` powershell 
 PS C:\> Get-OctopusTagSet
 ``` 

**EXAMPLE 5**

Gets the TagSet with the name "Upgrade Ring"

 ``` powershell 
 PS C:\> Get-OctopusTagSet -name "Upgrade Ring"
 ``` 

**EXAMPLE 5**

Gets the TagSets with the names "Upgrade Ring" and "EAP"

 ``` powershell 
 PS C:\> Get-OctopusTagSet -name "Upgrade Ring","EAP"
 ``` 

**EXAMPLE 5**

Gets all the TagSets whose name matches the pattern "*_Customers"

 ``` powershell 
 PS C:\> Get-OctopustagSet -name "*_Customers"
 ``` 

