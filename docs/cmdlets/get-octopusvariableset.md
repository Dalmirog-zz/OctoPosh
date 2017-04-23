
### Summary

Gets Octopus Variable sets. These can belong to a specific Project or to a Library Variable set. "Variable set" is the name of the object that holds the collection of variables for both Projects and Library Sets.
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| LibrarySetName | String[] |  Library Set name     |
| ProjectName | String[] |  Project name     |
| IncludeUsage | Switch |  If set to TRUE the list of Projects on which each Library Variable Set is being used will be displayer     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the  cmdlet will return a human friendly Octoposh output object     |

### Syntax
``` powershell

Get-OctopusVariableSet [[-LibrarySetName] <string[]>] [[-ProjectName] <string[]>] [-IncludeUsage 
<SwitchParameter>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples 

**EXAMPLE 1**

Gets all the Project and Library variable sets of the instance

``` powershell 
 Get-OctopusVariableSet
``` 

**EXAMPLE 2**

Gets the Variable Set of the Library Variable Set with the name "Stands_SC"

``` powershell 
 Get-OctopusVariableSet -LibrarySetName "Stands_SC"
``` 

**EXAMPLE 3**

Gets the Variable Set of the Library Variable Set "Stands_SC" and it also populates the output object property "Usage" with the list of projects that are currently using the set

``` powershell 
 Get-OctopusVariableSet -LibrarySetName "Stands_SC" -IncludeUsage
``` 

**EXAMPLE 4**

Gets the LibraryVariableSets with the names "Stands_SC" and "Stands_DII"

``` powershell 
 Get-OctopusVariableSet -LibrarySetName "Stands_SC","Stands_DII"
``` 

**EXAMPLE 5**

Gets all the LibraryVariableSets whose name matches the pattern "Stands_*"

``` powershell 
 Get-OctopusVariableSet -LibrarySetName "Stands_*"
``` 

**EXAMPLE 6**

Gets all the LibraryVariableSets whose name matches the pattern "Stands_*". Each result will also include a list of Projects on which they are being used

``` powershell 
 Get-OctopusVariableSet -LibrarySetName "Stands_*" -IncludeLibrarySetUsage
``` 

**EXAMPLE 7**

Gets the Variable Sets of the Projects "Website_Stardust" and "Website_Diamond"

``` powershell 
 Get-OctopusVariableSet -ProjectName "Website_Stardust","Website_Diamond"
``` 

**EXAMPLE 8**

Gets the Variable Sets of the Project "Website_Stardust" and the Library variable set "Stands_SC"

``` powershell 
 Get-OctopusVariableSet -ProjectName "Website_Stardust" -LibrarySetName "Stands_SC"
``` 

