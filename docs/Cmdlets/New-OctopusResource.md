### Summary
Creates a new Octopus Resource. This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Resource |  |  Resource Object     |

### Syntax
``` powershell

New-OctopusResource [[-Resource] <Resource>] [<CommonParameters>]




``` 

### Examples
Creates a new Project Group called "NewProjectGroup" on Octopus

 ``` powershell 
 PS C:\> $pg = Get-OctopusResourceModel -Resource ProjectGroup ; $pg.name = "NewProjectGroup" ; New-OctopusResource -Resource $pg
 ``` 

