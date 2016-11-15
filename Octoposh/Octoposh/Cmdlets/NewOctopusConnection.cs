using System;
using System.Management.Automation;
using Octoposh.Model;
using Octopus.Client;

namespace Octoposh.Cmdlets
{
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
