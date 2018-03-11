# OctoPoshCli: Octoposh wrapped in a cli, no need to install the cmdlet

**OctoposhCli** is a Cli utility which interacts with Octopus Deploy and returns json. All the functionality included relies on Octoposh and the [Octopus .NET client](https://www.nuget.org/packages/Octopus.Client).

## Getting Started

[Usage] 

Use ```<command>``` in place of the Octoposh cmdlet's.

count in the parameter list controls how may objects to return.

groupby in the parameter list controls grouping.

Use ```;``` to seperate multiple values.

```
OctoposhCli <apiKey> <serverUrl> <command> <parameter(s)>
```
Examples:
```
.\OctoposhCli.exe "API-QUCG8W2QIMS56WGQJK6F3AQC" "http://octopus.mysite.com" "Get-OctopusDeployment" "EnvironmentName=dev;prod,ProjectName=someproject,LatestReleases=1,count=1,groupby=EnvironmentName"
```

```
.\OctoposhCli.exe "API-QUCG8W2QIMS56WGQJK6F3AQC" "http://octopus.mysite.com" "Get-OctopusDeployment" "EnvironmentName=dev;stage,ProjectName=someProject;anotherProject,LatestReleases=1"
```



## Documentation

Check the docs site to learn how to use each command and some other advanced usages:

http://octoposh.readthedocs.io

## Questions y feature requests
If you want to request a cmdlet or a feature, or you just wanna ask how to do something with the module, drop by our Gitter channel and ask there:

[![Join the chat at https://gitter.im/Dalmirog/OctoPosh](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Dalmirog/OctoPosh?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Your questions will help shape up the module according to the needs of the community, so don't be a stranger!

## Builds

|               |               Build status               |
| :-----------: | :--------------------------------------: |
| Documentation | ![ReadTheDocs](https://raw.githubusercontent.com/rtfd/readthedocs.org/master/media/images/favicon.png) [![Documentation Status](http://readthedocs.org/projects/octoposh/badge/?version=latest)](http://octoposh.readthedocs.io/en/latest/?badge=latest) |

## Disclaimer
This is an open source project which is NOT supported by the Octopus Deploy team. All questions/bugs about this module should be entered on this github project.

## License

https://github.com/Dalmirog/OctoPosh/blob/master/LICENSE.md
