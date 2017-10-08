using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;
using Octoposh.Model;

namespace Octoposh.Cmdlets
{
    //Todo Fix help
    /// <summary>
    /// <para type="synopsis">Returns the Octopus Dashboard</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Returns the Octopus Dashboard</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDashboard</code>
    ///   <para>Gets the entire Octopus dashboard</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDashboard -ProjectName MyWebApp</code>
    ///   <para>Gets the dashboard info for the project MyWebApp</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDashboard -EnvironmentName Production</code>
    ///   <para>Gets the dashboard info for all the projects that have a release deployed to the "Production" environment.</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDashboard -DeploymentStatus Success</code>
    ///   <para>Gets all the deployments in "Success" status on the dashboard</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusDashboard")]
    [OutputType(typeof(List<OutputOctopusDashboardEntry>))]
    public class GetOctopusDashboard : OctoposhConnection
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

        protected override void ProcessRecord()
        {
            var rawDashboard = Connection.Repository.Dashboards.GetDashboard();
            
            var converter = new OutputConverter();
            
            var outputList = converter.GetOctopusDashboard(rawDashboard, ProjectName, EnvironmentName, DeploymentStatus);
            WriteObject(outputList,true);
        }
    }
}
