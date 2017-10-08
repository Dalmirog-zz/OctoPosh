using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet returns info about Octopus Projects</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet returns info about Octopus Projects</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProject</code>
    ///   <para>Gets all the projects of the current Instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProject -name MyProject</code>
    ///   <para>Get the project named "MyProject"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProject -name MyApp*</code>
    ///   <para>Get all the projects whose name starts with the string "MyApp"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProject -ProjectGroupName "MyProduct"</code>
    ///   <para>Gets all the projects inside of the Project Group "MyProduct"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusProject", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusProject>))]
    [OutputType(typeof(List<ProjectResource>))]
    public class GetOctopusProject : GetOctoposhCmdlet
    {
        private const string ByName = "ByName";
        private const string ByProjectGroup = "ByProjectGroup";
        private const string All = "All";
        
        /// <summary>
        /// <para type="description">Project name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public string[] ProjectName { get; set; }

        /// <summary>
        /// <para type="description">Gets all projects inside a set of Project Groups</para>
        /// </summary>
        [Alias("ProjectGroup")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByProjectGroup)]
        public string[] ProjectGroupName { get; set; }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<ProjectResource>();

            switch (ParameterSetName)
            {
                case All:
                    baseResourceList = Connection.Repository.Projects.FindAll();
                    break;

                case ByName:
                    var projectNameList = ProjectName?.ToList().ConvertAll(s => s.ToLower());

                    baseResourceList = FilterByName(projectNameList, Connection.Repository.Projects,"ProjectName");

                    break;
                case ByProjectGroup:
                    
                    var projectGroupNameList = ProjectGroupName?.ToList().ConvertAll(s => s.ToLower());

                    var projectGroups = FilterByName(projectGroupNameList,Connection.Repository.ProjectGroups,"ProjectGroupName");

                    if (projectGroups.Count != 0)
                    {
                        foreach (var projectGroup in projectGroups)
                        {
                            baseResourceList.AddRange(Connection.Repository.ProjectGroups.GetProjects(projectGroup));
                        }
                    }
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
                var outputList = converter.GetOctopusProject(baseResourceList);

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