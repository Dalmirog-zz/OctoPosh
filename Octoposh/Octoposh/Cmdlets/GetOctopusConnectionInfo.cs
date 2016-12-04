using System;
using System.Management.Automation;
using Octoposh.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="">This is the cmdlet synopsis.</para>
    /// <para type="description">This is part of the longer cmdlet description.</para>
    /// <para type="description">Also part of the longer cmdlet description.</para>
    /// </summary>
    [Cmdlet ("Get","OctopusConnectionInfo")]
    [OutputType(typeof(OctopusConnectionInfo))]
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