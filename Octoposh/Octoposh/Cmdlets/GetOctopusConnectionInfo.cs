using System;
using System.Management.Automation;
using Octoposh.Model;

namespace Octoposh.Cmdlets
{
    [Cmdlet ("Get","OctopusConnectionInfo")]
    public class GetOctopusConnectionInfo : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(new OctopusConnectionInfo
            {
                OctopusUrl = Environment.GetEnvironmentVariable("OctopusURL"),
                OctopusApiKey = Environment.GetEnvironmentVariable("OctopusAPIKey")
            });
        }
    }
}