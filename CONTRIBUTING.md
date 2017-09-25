# Contributing guide to Octoposh

- [Asking Questions](#asking-questions)
- [Reporting Bugs](#reporting-bugs)
- [Sending PRs](#sending-prs)
  - [Submitting a bug fix](#submitting-a-bug-fix)
  - [Submitting a new cmdlet/functionality](#submitting-a-new-cmdletfunctionality)
  - [Submitting changes to the built-in module help](#submitting-changes-to-the-built-in-module-help)
  - [Submitting changes to the wiki](#submitting-changes-to-the-wiki)
- [Setting up the development environment](#setting-up-the-development-environment)
  - [Dev machine requirements](#dev-machine-requirements)
  - [Running the unit tests](#running-the-unit-tests)
  

## Asking Questions

- *How do I do [x] with the module?*
- *Can you add a new cmdlet to do [x] to the module?*
- *Can I get some help setting up the Dev environment to start working on the module code myself?*
- *Which is your favorite JJBA arc*

Please ask all these questions in the project's [Gitter Channel](https://gitter.im/Dalmirog/OctoPosh#initial)

## Reporting Bugs

Go to the project's [Gitter Channel ](https://gitter.im/Dalmirog/OctoPosh#initial) and ask over there is the behavior you are seeing is a bug or not. Try to give as much data as possible to reproduce the issue. One of the project maintainers will reply back and create a Github Issue if the reported behavior actually needs a code change. 

## Sending PR's

We've all been in that situation where we send a PR and the Project owner doesn't merge it because of reasons. So as a general rule: always ask in the project's [Gitter Channel](https://gitter.im/Dalmirog/OctoPosh#initial) before you dedicate your precious time in a PR.

### Submitting a bug fix

Before you start working on a fix, make sure it is an actual bug by asking in the project's [Gitter Channel](https://gitter.im/Dalmirog/OctoPosh#initial). All bug fixes must have their correspondent unit tests.

### Submitting a new cmdlet/functionality

Octoposh was built as an easy way to interact with the Octopus API/Octopus.Client through Powershell. The idea is that It is a **Toolbox** and not a **Spell Book** . 

The cmdlets should do small and very specific things that, put together, should help users achieving big things.

A brief example:  *Lets say you want to Disable all the Tentacles whose name's start with  *"Prod_"* .*

A **Tool-y** approach would be something like:

```powershell
#First a cmdlet that only gets the Tentacles you need
$Tentacles = Get-OctopusTentacle -Name Prod_*

#Then a foreach loop to disable each machine
foreach($tentacle in $tentacles ){
  $tentacle.resource.status = "Disabled"
  
  Update-OctopusResource -resource $tentacle.resource
}
```

A **Spell-y/Magical** approach would be

```powershell
#A one liner that does exactly what you want
Disable-Tentacles -name Prod_*
```

Before you submit any kind of PR for new functionality, please discuss it first with the project owners in the project's [Gitter Channel](https://gitter.im/Dalmirog/OctoPosh#initial) . We don't want you to spend a lot of time on something that might never get merged :pray:.

Of course, every PR must have their respective Unit tests.

### Submitting changes to the built-in module help

When you run `Get-Help -name CmdletName` , the help text that you see comes from the XML Documentation comments at the top of each cmdlet. For example if you wanted to send a PR for the cmdlet `Get-OctopusMachine`, you should be looking at:

- For the cmdles description and examples look at [these lines in the code](https://github.com/Dalmirog/OctoPosh/blob/master/Octoposh/Cmdlets/GetOctopusMachine.cs#L11-L40) 
- For the parameter's help text look at the XML comment at the top of each parameter [like this](https://github.com/Dalmirog/OctoPosh/blob/master/Octoposh/Cmdlets/GetOctopusMachine.cs#L52-L58)

### Submitting changes to the wiki

The source code of the wiki is split between 2 sources:

1) Each cmdlet's help page under the `Cmdlets` section is programmatically generated from the output of `Get-Help -name CmdletName`. So if you want to submit a fix to that, It'll be the same as submitting changes to the built-in module help.

2) The `Getting Started`,`Advanced Usage` and `Release Notes` sections are raw markdown files under `[ProjectRoot]/docs`

## Setting up the development environment

### Dev machine requirements

- SQL Server 2012+. Can be Express
- Octopus Server 3.17+ Installed. No need to have the Tentacle installed.
- .NET CORE 2.0.
- Latest Visual Studio 2017.

### Running the unit tests

All the tests for the Octoposh module are in the `Octoposh.Tests` project. These tests depend on some specific test data which can be programmatically generated using the `Octoposh.TestDataGenerator` project. 

For both projects, make sure to enter the Octopus Username(needs to be admin on the instance), password and Octopus Instance port in their respective `app.config` and `appConfig.json` files.

Once the `Octoposh.TestDataGenerator` console runs successfully, you should be able to run the NUnit3 tests. You should be able to run the `Octoposh.TestDataGenerator` console as many times as you want and it shouldn't have problems with existing data.a
