### 0.6.2/1.0.0-Beta2

- [249](https://github.com/Dalmirog/OctoPosh/issues/249): `Get-OctopusRelease` now includes package info.
- [247](https://github.com/Dalmirog/OctoPosh/issues/247): `Get-OctopusProject` makes less API Calls and uses more already-in-memory info.

### 0.6.1/1.0.0-Beta1

- [242](https://github.com/Dalmirog/OctoPosh/issues/242): Running `Get-OctopusDashboard` without parameters returns all the values in the dashboard.

- [243](https://github.com/Dalmirog/OctoPosh/issues/243): `Get-OctopusDashboard` parameters are now case **In**sensitive

- [244](https://github.com/Dalmirog/OctoPosh/issues/244): `Get-Octopus*` parameters are now `string[]` instead of `List<string>`

### 0.6.0/1.0.0-Beta0

**[Breaking Change]**
- [234](https://github.com/Dalmirog/OctoPosh/issues/234): Ported module to C#. Please read the entire github issue to know what changed.

### 0.4.6

**Breaking change:** Due to [changes in the Octopus API](http://docs.octopusdeploy.com/display/OD/Sensitive+Properties+API+Changes+in+Release+3.3), the Octopus client that the Octoposh module uses had to be updated to [3.3](https://www.nuget.org/packages/Octopus.Client/). This means that to use Octoposh `0.4.6` you'll need to have your Octopus server in 3.3.x.

- [166](https://github.com/Dalmirog/OctoPosh/issues/166): Updated Octopus.client to 3.3
- [164](https://github.com/Dalmirog/OctoPosh/issues/164): Added cmdlet to get User Roles ([Wiki](https://github.com/Dalmirog/OctoPosh/wiki/Get-OctopusUserRole))
- [163](https://github.com/Dalmirog/OctoPosh/issues/163): Added CRUD support for Octopus Users ([Create](https://github.com/Dalmirog/OctoPosh/wiki/Creating-Resources#users),[Read](https://github.com/Dalmirog/OctoPosh/wiki/Get-OctopusUser),[Update](https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Resources#users),[Delete](https://github.com/Dalmirog/OctoPosh/wiki/Deleting-Resources#users))
- [162](https://github.com/Dalmirog/OctoPosh/issues/162): Added CRUD support for Octopus Teams ([Create](https://github.com/Dalmirog/OctoPosh/wiki/Creating-Resources#teams),[Read](https://github.com/Dalmirog/OctoPosh/wiki/Get-OctopusTeam),[Update](https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Resources#teams),[Delete](https://github.com/Dalmirog/OctoPosh/wiki/Deleting-Resources#teams))

### 0.3.5
- [148](https://github.com/Dalmirog/OctoPosh/issues/148): 3.0 support
- [147](https://github.com/Dalmirog/OctoPosh/issues/147): Add cmdlet to update calamari
- [142](https://github.com/Dalmirog/OctoPosh/issues/142): Get-OctopusEnvironment no longer returns all environments if -Environment -eq ""
- [65](https://github.com/Dalmirog/OctoPosh/issues/65): Add "Release version" filter for `get-octopusdeployment`

### 0.2.72
- [132](https://github.com/Dalmirog/OctoPosh/issues/132): New-OctopusResource - Add support for lifecycles
- [134](https://github.com/Dalmirog/OctoPosh/issues/134): Get-OctopusResourceModel - Add support for lifecycles
- [135](https://github.com/Dalmirog/OctoPosh/issues/135): Update-OctopusResource - Add support for lifecycles 

### 0.2.71
- [133](https://github.com/Dalmirog/OctoPosh/issues/131): Remove-OctopusResource - Add support for lifecycles

### 0.2.69
- [127](https://github.com/Dalmirog/OctoPosh/issues/127): Get-OctopusTask is missing states 'Executing' and 'Canceling'
- [38](https://github.com/Dalmirog/OctoPosh/issues/38): Script to add or modify variable sets

### 0.2.67
- [77](https://github.com/Dalmirog/OctoPosh/issues/77): Added `-ReleaseVersion` parameter to Get-OctopusDeployment

### 0.2.66
- [85](https://github.com/Dalmirog/OctoPosh/issues/85): Added support to update Octopus resources