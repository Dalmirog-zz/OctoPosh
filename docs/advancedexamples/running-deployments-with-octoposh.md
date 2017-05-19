*If you don't know what `Octo.exe` is, stop right here and read the Octopus documentation: https://octopus.com/docs/api-and-integration/octo.exe-command-line*

# Running deployments with Octoposh

While **Octoposh** doesn't have specific cmdlets to create and deploy releases, it does have a set of cmdlets that allows the user to download `Octo.exe` to do all these things.

[Why no dedicated cmdlets? Please read this conversation](https://github.com/Dalmirog/OctoPosh/issues/183)

The cmdlets available for this task are:

- [Get-OctopusToolsFolder](https://github.com/Dalmirog/OctoPosh/wiki/Get-OctopusToolsFolder)
- [Set-OctopusToolsFolder](https://github.com/Dalmirog/OctoPosh/wiki/Set-OctopusToolsFolder)
- [Get-OctopusToolPath](https://github.com/Dalmirog/OctoPosh/wiki/Get-OctopusToolPath)
- [Set-OctopusToolPath](https://github.com/Dalmirog/OctoPosh/wiki/Set-OctopusToolPath)
- [Get-OctopusToolVersion](https://github.com/Dalmirog/OctoPosh/wiki/Get-OctopusToolVersion)
- [Install-OctopusTool](https://github.com/Dalmirog/OctoPosh/wiki/Install-OctopusTool)

Each cmdlet's usage can be found using the links above, or running `Get-Help [cmdlet name]`. This document will mainly focus on the integrated usage of all of them.

## A few key Environment variables

Before we start, there's a few important environment variables that you should get familiar with

| Variable                    | Description                              |
| --------------------------- | ---------------------------------------- |
| **$env:OctopusToolsFolder** | Points to a directory with folders with versions of `Octo.exe` on them |
| **$env:OctoExe**            | Points directly to a version of `Octo.exe` |
| **$env:NugetRepositoryURL** | The full URL/Path of the Nuget repository where from which Octoposh will download `Octo.exe` |
| **$env:OctopusAPIKey**      | Holds the API Key that's being used by all the Octoposh cmdlets |
| **$env:OctopusURL**         | Holds the Octopus Server URL that's being used by all the Octoposh cmdlets |

## Downloading Octo.exe

1) To get started the first thing you need to do is set a value to `$Env:OctopusToolsFolder` so Octoposh knows where to download `Octo.exe`. You can do this using `Set-OctopusToolsFolder`

```powershell
Set-OctopusToolsFolder -path "C:\Tools"
```

2) Then use `Install-OctopusTool` to download `Octo.exe`

```powershell
Install-OctopusTool -version 4.3.12
```

*By default this cmdlet will download `Octo.exe` from Nuget.org. If you want to change this behavior, change the feed URL by setting a new value to `$env:NugetRepositoryURL`*

3) Finally, set the value of `$env:OctoExe` using `Set-OctopusToolPath`

```powershell
Set-OctopusToolPath -version 4.3.12
```

4) Once that is done, you should be able to start using `Octo.exe` like this:

```powershell
& $env:octoexe create-release --server $env:octopusURL --apikey $env:octopusAPIKey --project MyProject --version 1.0.0
```

*notice how it re-utilizes `$env:OctopusURL` and `$env:OctopusAPIKey` to pass the **server** and **apikey** to the call*

## Tips

- Use your Powershell profile to run `Set-OctopusToolsFolder` right when you open your Powershell session. It'll make the overall experience a lot better.

- Typing environment variable with the `$env:` at the start can be annoying. I strongly recommend you to create alias variables in your Powershell profile like `$OctoExe = $env:OctoExe` and so on.

- If you have multiple versions of `Octo.exe` inside of `$env:OctopusToolsFolder`, but you only want to use the latest, simply run `Get-OctopusToolVersion -latest | Set-OctopusToolPath` and the `octo.exe` copy with the highest version will be set to `$env:octoexe`

- Pass `-SetAsDefault` to `Install-OctopusTool` so as soon as it downloads it, it also sets it as the default `Octo.exe`. This way you won't have to run `Set-OctopusToolPath`
