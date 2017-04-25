The module will use the Octopus URL set on the environment variable `$Env:OctopusURL` You can set this variable manually or using [Set-OctopusConnectionInfo](https://github.com/Dalmirog/OctoPosh/wiki/Set-OctopusConnectionInfo)

As for the authentication, you can use the following methods:
### Using an API Key

If you don't have an API Key, you can create one from [the Web UI](http://docs.octopusdeploy.com/display/OD/How+to+create+an+API+key) or using [New-OctopusAPIKey](https://github.com/Dalmirog/OctoPosh/wiki/New-OctopusAPIKey) (super recommended)

When using this method, the module will use the API Key set on the environment variable `$Env:OctopusAPIKey`. You can set this variable manually or using [Set-OctopusConnectionInfo](https://github.com/Dalmirog/OctoPosh/wiki/Set-OctopusConnectionInfo)

If you want to know the API Key and Octopus URL set on your current session, use [Get-OctopusConnectionInfo](https://github.com/Dalmirog/OctoPosh/wiki/Get-OctopusConnectionInfo)

### Setting credentials for future Powershell sessions

If you want to set the credentials for all your Powershell sessions, the best way would be to [include one of the following lines on your Powershell Profile](http://www.howtogeek.com/50236/customizing-your-powershell-profile/) 

```
Set-OctopusConnectionInfo -URL [Your Octopus URL] -APIKey [Your API Key]
```
or
```
$env:OctopusURL = [Your Octopus URL]
$env:OctopusAPIKey = [Your Octopus API Key]
```