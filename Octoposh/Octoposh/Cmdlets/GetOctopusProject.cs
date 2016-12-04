﻿using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet returns info about Octopus Projects</para>
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
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusProject", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusProject>))]
    [OutputType(typeof(List<ProjectResource>))]
    public class GetOctopusProject : PSCmdlet
    {
        private const string ByName = "ByName";
        private const string ByProjectGroup = "ByProjectGroup";
        private const string All = "All";
        
        /// <summary>
        /// <para type="description">Name of the Project to filter by</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public List<string> ProjectName { get; set; }

        /// <summary>
        /// <para type="description">Name of the Project Group from which to get all the projects</para>
        /// </summary>
        //todo: make public when team answers/fixes https://octopusdeploy.slack.com/archives/team-fire-and-motion/p1479596369000057
        [Alias("ProjectGroup")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByProjectGroup)]
        private List<string> ProjectGroupName { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<ProjectResource>();

            switch (ParameterSetName)
            {
                case All:
                    baseResourceList = _connection.Repository.Projects.FindAll();
                    break;

                case ByName:
                    ProjectName = ProjectName.ConvertAll(s => s.ToLower());
                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                    if (ProjectName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && ProjectName.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("ProjectName");
                    }
                    //Only 1 wildcarded value (ie -MachineName WebServer*)
                    else if (ProjectName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && ProjectName.Count == 1))
                    {
                        var pattern = new WildcardPattern(ProjectName.First());
                        baseResourceList = _connection.Repository.Projects.FindMany(x => pattern.IsMatch(x.Name.ToLower()));
                    }
                    //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                    else if (!ProjectName.Any(item => WildcardPattern.ContainsWildcardCharacters(item)))
                    {
                        baseResourceList = _connection.Repository.Projects.FindMany(x => ProjectName.Contains(x.Name.ToLower()));
                    }
                    break;
                case ByProjectGroup:
                    ProjectGroupName = ProjectGroupName.ConvertAll(s => s.ToLower());
                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                    if (ProjectGroupName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && ProjectGroupName.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("ProjectGroupName");
                    }
                    //Only 1 wildcarded value (ie -MachineName WebServer*)
                    else if (ProjectGroupName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && ProjectGroupName.Count == 1))
                    {
                        var pattern = new WildcardPattern(ProjectGroupName.First());
                        var projectGroups = _connection.Repository.ProjectGroups.FindMany(x => pattern.IsMatch(x.Name.ToLower()));

                        if (projectGroups.Count != 0)
                        {
                            foreach (var projectGroup in projectGroups)
                            {
                                baseResourceList.AddRange(_connection.Repository.ProjectGroups.GetProjects(projectGroup));
                            }
                        }
                    }
                    //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                    else if (!ProjectGroupName.Any(item => WildcardPattern.ContainsWildcardCharacters(item)))
                    {
                        var projectGroups = _connection.Repository.ProjectGroups.FindMany(x => ProjectGroupName.Contains(x.Name.ToLower()));

                        if (projectGroups.Count != 0)
                        {
                            foreach (var projectGroup in projectGroups)
                            {
                                baseResourceList.AddRange(_connection.Repository.ProjectGroups.GetProjects(projectGroup));
                            }
                        }
                        
                    }
                    break;
            }

            if (ResourceOnly)
            {
                WriteObject(baseResourceList);
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusProject(baseResourceList);

                WriteObject(outputList);
            }
            
        }
    }
}