using System;
using System.Management.Automation;
using Octoposh.Model;
using Octopus.Client;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates an endpoint to connect to an Octopus Server</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Creates an endpoint to connect to an Octopus Server</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> $c = New-octopusconnection ; $c.repository.environments.findall()</code>
    ///   <para>Gets all the environments on the Octopus instance using the Octopus .NET client repository</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> $c = New-OctopusConnection ; invoke-webrequest -header $c.header -uri http://Octopus.company.com/api/environments/all -method Get </code>
    ///   <para>Uses the [Header] Member of the Object returned by New-OctopusConnection as a header to call the REST API using Invoke-WebRequest and get all the Environments of the instance</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet ("New","OctopusConnection")]
    [OutputType(typeof(OctopusConnection))]
    public class NewOctopusConnection : Cmdlet
    {
        private readonly string _server = Environment.GetEnvironmentVariable("OctopusURL");
        private readonly string _apikey = Environment.GetEnvironmentVariable("OctopusAPIKey");


        protected override void BeginProcessing()
        {
            if (string.IsNullOrEmpty(_server) || string.IsNullOrEmpty(_apikey))
            {
                throw new Exception(
                    "At least one of the following variables does not have a value set: $env:OctopusURL or $env:OctopusAPIKey. Use Set-OctopusConnectionInfo to set these values");
            }
        }

        protected override void ProcessRecord()
        {
            var connection = new OctopusConnection();

            var endpoint = new OctopusServerEndpoint(_server, _apikey);

            connection.Repository = new OctopusRepository(endpoint);
            connection.Header.Add("X-Octopus-APIKey",_apikey);
            WriteObject(connection);
        }
        
    }
}
