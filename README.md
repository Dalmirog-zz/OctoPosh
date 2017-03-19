# OctoPosh: The Octopus Deploy Powershell Module

Octoposh is a Powershell module that prives cmdlets to interact with your Octopus Deploy instances. All cmdlets use a combination of the [Octopus .NET client](https://www.nuget.org/packages/Octopus.Client) and some raw REST API calls.

With this module you can do things like:

- Use  all the [Get-Octopus* cmdlets included](https://github.com/Dalmirog/OctoPosh/wiki) to get full-blown .NET objects referencing Octopus resources such as Projects, Evironments, Deployment Targets and much more.

- [Create Octopus Resources](https://github.com/Dalmirog/OctoPosh/wiki/Creating-Resources)

- [Modify Octopus Resources](https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Resources)

- [Delete Octopus Resources](https://github.com/Dalmirog/OctoPosh/wiki/Deleting-Resources)

- Do all sorts of administrative tasks such as
  - Starting a health check on an environment. [Wiki](https://github.com/Dalmirog/OctoPosh/wiki/Start-OctopusHealthCheck)
  - Setting maintenance mode ON/OFF. [wiki](https://github.com/Dalmirog/OctoPosh/wiki/Set-OctopusMaintenanceMode)
  - Start a Server-side retention Policy cleanup. [Wiki](https://github.com/Dalmirog/OctoPosh/wiki/Start-OctopusRetentionPolicy)
  - *More to come*


##Getting Started

- [How to download the module](https://github.com/Dalmirog/OctoPosh/wiki/Installing-the-module)
- [How to setup your credentials](https://github.com/Dalmirog/OctoPosh/wiki/Setting-Credentials)

Check out our [Wiki](https://github.com/Dalmirog/OctoPosh/wiki) for our entire list of cmdlets

##Questions y feature requests
If you want to request a cmdlet or a feature, or you just wanna ask how to do something with the module, drop by our Gitter channel and ask there:

[![Join the chat at https://gitter.im/Dalmirog/OctoPosh](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Dalmirog/OctoPosh?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Your questions will help shape up the module according to the needs of the community, so dont be a stranger!

##Disclaimer
This is an open source project which is NOT supported by the Octopus Deploy team. All questions/bugs about this module should be entered on this github project.

##Credits

This module was made using the following awesome tools

| Name | Site|
| ------------- | ----------- |
| Octopus Deploy      | https://octopusdeploy.com/|
| Pester | https://github.com/pester/Pester|
| Fiddler | http://www.telerik.com/fiddler |
| Papercut     | https://papercut.codeplex.com/ |
| TeamCity    | https://www.jetbrains.com/teamcity/ |
| Powershell Gallery | https://www.powershellgallery.com/|
