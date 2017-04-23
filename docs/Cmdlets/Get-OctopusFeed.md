### Summary
Gets information about the external feeds registered in Octopus
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| FeedName | String[] |  Feed name     |
| URL | String[] |  Feed URL/Path     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusFeed [[-FeedName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]

Get-OctopusFeed [[-URL] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
Get all the external feeds registered in the current Instance

 ``` powershell 
 PS C:\> Get-OctopusFeed
 ``` 

Get the External Feed named "MyGet"

 ``` powershell 
 PS C:\> Get-OctopusFeed -FeedName "MyGet"
 ``` 

Get a feed with a the string "MyCompany" inside its URL

 ``` powershell 
 PS C:\> Get-OctopusFeed -URL "*Mycompany*"
 ``` 

