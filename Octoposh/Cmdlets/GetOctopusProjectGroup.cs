using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets information about Octopus Project Groups</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Gets information about Octopus Project Groups</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProjectGroup</code>
    ///   <para>Gets all the Project Groups on the Octopus instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProjectGroup -name "MyProjects"</code>
    ///   <para>Gets a Project Group named "MyProjects"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProjectGroup -name "*web*"</code>
    ///   <para>Get all the projects whose name matches the pattern "*web*"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusProjectGroup", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusProjectGroup>))]
    [OutputType(typeof(List<ProjectGroupResource>))]
    public class GetOctopusProjectGroup : GetOctoposhCmdlet
    {
        private const string ByName = "ByName";
        private const string All = "All";

        /// <summary>
        /// <para type="description">Project Group name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public string[] ProjectGroupName { get; set; }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<ProjectGroupResource>();

            switch (ParameterSetName)
            {
                case All:
                    baseResourceList = Connection.Repository.ProjectGroups.FindAll();
                    break;

                case ByName:
                    baseResourceList = FilterByName(ProjectGroupName, Connection.Repository.ProjectGroups, "ProjectGroupName");
                    break;
            }

            if (ResourceOnly)
            {
                if (baseResourceList.Count == 1)
                {
                    WriteObject(baseResourceList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(baseResourceList,true);
                }
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusProjectGroup(baseResourceList);

                if (outputList.Count == 1)
                {
                    WriteObject(outputList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(outputList,true);
                }
            }

        }
    }
}