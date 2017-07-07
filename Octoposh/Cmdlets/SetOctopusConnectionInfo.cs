using System;
using System.Management.Automation;
using Octoposh.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Sets the current Octopus connection info (URL and API Key). Highly recommended to call this function from $profile to avoid having to re-configure this on every session.</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Sets the current Octopus connection info (URL and API Key). Highly recommended to call this function from $profile to avoid having to re-configure this on every session.</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusConnectionInfo -Server "http://MyOctopus.AwesomeCompany.com" -API "API-7CH6XN0HHOU7DDEEUGKUFUR1K"</code>
    ///   <para>Set connection info with a specific API Key for an Octopus instance</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Set", "OctopusConnectionInfo")]
    public class SetOctopusConnectionInfo : Cmdlet 
    {
        /// <summary>
        /// <para type="description">URL of the server you want to connect to</para>
        /// </summary>
        [Alias("URL")]
        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true)]
        public string Server { get; set; }

        /// <summary>
        /// <para type="description">API Key you'll use to authenticate with the Octopus Server</para>
        /// </summary>
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
            },true);
        }

    }
}
