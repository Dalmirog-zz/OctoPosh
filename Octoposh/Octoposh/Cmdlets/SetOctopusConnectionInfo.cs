using System;
using System.Management.Automation;
using Octoposh.Model;

namespace Octoposh.Cmdlets
{
    [Cmdlet("Set", "OctopusConnectionInfo")]
    public class SetOctopusConnectionInfo : Cmdlet 
    {
        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true)]
        public string Server { get; set; }

        [Parameter(Position = 2, Mandatory = true, ValueFromPipeline = true)]
        public string ApiKey { get; set; }

        protected override void ProcessRecord()
        {
            Environment.SetEnvironmentVariable("OctopusURL", Server);
            Environment.SetEnvironmentVariable("OctopusAPIKey", ApiKey);

            WriteObject(new OctopusConnectionInfo
            {
                OctopusUrl = Environment.GetEnvironmentVariable("OctopusURL"),
                OctopusApiKey = Environment.GetEnvironmentVariable("OctopusAPIKey")
            });
        }

    }
}
