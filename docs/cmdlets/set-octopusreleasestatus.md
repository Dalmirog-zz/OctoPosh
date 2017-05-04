
### Summary


### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| ProjectName | String |  Project name     |
| ReleaseVersion |  |  Releases to Block/Unblock     |
| Description | String |  Reason to block the deployment     |
| Status | String |  Status that the release will be put into     |
| Resource |  |  List of [Octopus.Model.ReleaseResource] objects that will get blocked/unblocked. By using this  parameter you do not need to pass values to "ProjectName" or "ReleaseVersion", as that info will  already be available in the Release object     |

### Syntax
``` powershell

Set-OctopusReleaseStatus [-ReleaseVersion] <List`1> [-ProjectName] <string> [-Status] <string> 
[[-Description] <string>] [<CommonParameters>]

Set-OctopusReleaseStatus [-Resource] <List`1> [-Status] <string> [[-Description] <string>] 
[<CommonParameters>]




``` 

### Examples 

**EXAMPLE 1**

Blocks the release [1.0.0] from the project [MyProject] from being deployed with the reason ["Because of reasons"]

``` powershell 
 Set-OctopusReleaseStatus -ProjectName MyProject -ReleaseVersion 1.0.0, 2.0.0 -Description "Because of reasons"
``` 

**EXAMPLE 2**

Blocks the releases [1.0.0],[2.0.0] from the project [MyProject] from being deployed with the reson ["Because of reasons"]

``` powershell 
 Set-OctopusReleaseStatus -ProjectName MyProject -ReleaseVersion 1.0.0, 2.0.0 -state Blocked -Description "Because of reasons"
``` 

**EXAMPLE 3**

Unblocks release [1.0.0] from project [MyProject]

``` powershell 
 Set-OctopusReleaseStatus-ProjectName MyProject -ReleaseVersion 1.0.0 -state Unblocked
``` 

