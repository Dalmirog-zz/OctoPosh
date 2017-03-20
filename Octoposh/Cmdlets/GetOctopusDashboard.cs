using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Model;

namespace Octoposh.Cmdlets
{
    //Todo Fix help
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
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusDashboard")]
    [OutputType(typeof(List<OutputOctopusDashboardEntry>))]
    public class GetOctopusDashboard : Cmdlet
    {
        /// <summary>
        /// <para type="description">Name of the Project to filter for.</para>
        /// </summary>
        [Parameter]
        public string[] ProjectName { get; set; }

        /// <summary>
        /// <para type="description">Name of the Project to filter for.</para>
        /// </summary>
        [Parameter]
        public string[] EnvironmentName { get; set; }

        /// <summary>
        /// <para type="description">Target communication style to filter by</para>
        /// </summary>
        [Parameter]
        [ValidateSet("Success", "Failed", "Executing", "Canceled",IgnoreCase = true)]
        public string[] DeploymentStatus { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            var rawDashboard = _connection.Repository.Dashboards.GetDashboard();
            
            var converter = new OutputConverter();
            
            var outputList = converter.GetOctopusDashboard(rawDashboard, ProjectName, EnvironmentName, DeploymentStatus);
            WriteObject(outputList);
        }
    }
}
