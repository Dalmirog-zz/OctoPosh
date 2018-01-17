# Welcome to the OctoPosh wiki!

OctoPosh is a Powershell module that provides cmdlet to do basic tasks on Octopus Deploy. All the cmdlets use a combination of calls to the [Octopus REST API](https://github.com/OctopusDeploy/OctopusDeploy-Api/wiki) and some occasional uses of the [Octopus.Client](http://www.nuget.org/packages/Octopus.Client/)(which in the end, also uses the REST API)

The goal of this module is for it to be the go-to place to programatically get information about your Octopus instance, as well as to automate tasks to help you get your Deployment environment up and running fast and easy. If you go to [Octopus' super amazing support site](http://help.octopusdeploy.com/), you'll notice that there are plenty of questions of the kind `How do I do [this] using the REST API?`. One of the objectives of this module is to create tools to help the user be able to do things in Octopus from Powershell in a rather intuitive way.

## Getting Started

Use the sidebar on the right to learn more about the cmdlets that are inside of the module.

You might wanna start by reading:
- [How to Install the module](http://octoposh.readthedocs.io/en/latest/gettingstarted/installing-the-module/)
- [How to setup your credentials](http://octoposh.readthedocs.io/en/latest/gettingstarted/setting-credentials/)

##Start using the cmdlets

Once you install the module and setup your credentials, you can either use the articles on this wiki to learn the syntax of each cmdlet, or you can run `Get-Help [cmdlet name] -full` from Powershell.

This wiki is programatically populated using the cmdlets built-in help. So all the examples and descriptions will always be identical between the wiki and the cmdlets.
