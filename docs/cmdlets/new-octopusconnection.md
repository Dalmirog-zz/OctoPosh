
### Summary

Creates an endpoint to connect to an Octopus Server
### Syntax
``` powershell

``` 

### Examples 

**EXAMPLE 1**

Gets all the environments on the Octopus instance using the Octopus .NET client repository

``` powershell 
 $c = New-octopusconnection ; $c.repository.environments.findall()
``` 

**EXAMPLE 2**

Uses the [Header] Member of the Object returned by New-OctopusConnection as a header to call the REST API using Invoke-WebRequest and get all the Environments of the instance

``` powershell 
 $c = New-OctopusConnection ; invoke-webrequest -header $c.header -uri http://Octopus.company.com/api/environments/all -method Get
``` 

