using System;
using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;

using System.Management.Automation;
using Octoposh.Infrastructure;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Blocks or unblocks releases.</para>
    /// </summary>
    /// <summary>
    /// <para type="Description">Blocks or unblocks releases.</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusReleaseStatus -ProjectName MyProject -ReleaseVersion 1.0.0 -state Blocked -Description "Because of reasons"</code>
    ///   <para>Blocks the release [1.0.0] from the project [MyProject] from being deployed with the reason ["Because of reasons"] </para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusReleaseStatus -ProjectName MyProject -ReleaseVersion 1.0.0, 2.0.0 -Description "Because of reasons"</code>
    ///   <para>Blocks the releases [1.0.0],[2.0.0] from the project [MyProject] from being deployed with the reson ["Because of reasons"] </para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusReleaseStatus-ProjectName MyProject -ReleaseVersion 1.0.0 -state Unblocked </code>
    ///   <para>Unblocks release [1.0.0] from project [MyProject]</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Set", "OctopusReleaseStatus")]
    public class SetOctopusReleaseStatus : OctoposhConnection
    {
        private const string ByReleaseResource = "ByReleaseResource";
        private const string ByProjectAndVersion = "ByProjectAndVersion";

        /// <summary>
        /// <para type="description">Project name</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true, ParameterSetName = ByProjectAndVersion)]
        public string ProjectName { get; set; }

        /// <summary>
        /// <para type="description">Releases to Block/Unblock</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ByProjectAndVersion)]
        public List<string> ReleaseVersion { get; set; }

        /// <summary>
        /// <para type="description">Reason to block the deployment</para>
        /// </summary>
        [Alias("Reason")]
        [Parameter(Position = 3)]
        public string Description { get; set; }

        /// <summary>
        /// <para type="description">Status that the release will be put into</para>
        /// </summary>
        [Alias("State")]
        [ValidateSet("Unblocked", "Blocked")]
        [Parameter(Mandatory = true, Position = 2)]
        public string Status { get; set; }

        /// <summary>
        /// <para type="description">List of [Octopus.Model.ReleaseResource] objects that will get blocked/unblocked. By using this parameter you do not need to pass values to "ProjectName" or "ReleaseVersion", as that info will already be available in the Release object</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true, ParameterSetName = ByReleaseResource)]
        public List<ReleaseResource> Resource { get; set; }

        protected override void ProcessRecord()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                Description = $"Blocking release from Octoposh on {DateTime.Now}";
            }

            var releases = new List<ReleaseResource>();

            if (ParameterSetName == ByProjectAndVersion)
            {
                var project = Connection.Repository.Projects.FindByName(ProjectName);

                foreach (var version in ReleaseVersion)
                {
                    releases.Add(Connection.Repository.Projects.GetReleaseByVersion(project, version));
                }
            }
            else
            {
                releases = Resource;
            }

            foreach (var release in releases)
            {
                if (Status == "Unblocked")
                {
                    WriteDebug($"Unblocking release [{release.Version}]");
                    Connection.Repository.Defects.ResolveDefect(release);
                }
                else
                {
                    WriteDebug($"Blocking release [{release.Version}]");
                    Connection.Repository.Defects.RaiseDefect(release, Description);
                }

            }

        }
    }
}