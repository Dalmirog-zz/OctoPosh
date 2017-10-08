using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Octoposh.Infrastructure;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Updates resources from an Octopus Instance. This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples </para>
    /// </summary>
    /// <summary>
    /// <para type="synopsis">Updates resources from an Octopus Instance. This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples </para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> $pg = Get-OctopusProjectGroup -name SomeProjectName ; $pg.resource.name = "SomeOtherProjectName" ; Update-OctopusResource -resource $pg.resource</code>
    ///   <para> Updates the Name of a ProjectGroup </para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> $machine = Get-OctopusMachine -MachineName "SQL_Production" ; $machine.resource.isdisabled = $true ; Update-OctopusResource -resource $machine.resource </code>
    ///   <para>Updates the [IsDisabled] property of a machine to disable it</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Update", "OctopusResource")]
    [OutputType(typeof(EnvironmentResource))]
    [OutputType(typeof(ProjectResource))]
    [OutputType(typeof(ProjectGroupResource))]
    [OutputType(typeof(NuGetFeedResource))]
    [OutputType(typeof(LibraryVariableSetResource))]
    [OutputType(typeof(MachineResource))]
    [OutputType(typeof(LifecycleResource))]
    [OutputType(typeof(TeamResource))]
    [OutputType(typeof(UserResource))]
    [OutputType(typeof(ChannelResource))]
    [OutputType(typeof(TenantResource))]
    [OutputType(typeof(TagSetResource))]
    public class UpdateOctopusResource : OctoposhConnection
    {
        /// <summary>
        /// <para type="description">Resource Object</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public Resource Resource { get; set; }

        protected override void ProcessRecord()
        {
            Resource outputResource = null;

            switch (Resource.GetType().ToString())
            {
                case "Octopus.Client.Model.EnvironmentResource":
                    var enviroment = (EnvironmentResource)Resource;
                    outputResource = Connection.Repository.Environments.Modify(enviroment);
                    break;

                case "Octopus.Client.Model.ProjectResource":
                    var project = (ProjectResource)Resource;
                    outputResource = Connection.Repository.Projects.Modify(project);
                    break;

                case "Octopus.Client.Model.ProjectGroupResource":
                    var projectGroup = (ProjectGroupResource)Resource;
                    outputResource = Connection.Repository.ProjectGroups.Modify(projectGroup);
                    break;

                case "Octopus.Client.Model.NuGetFeedResource":
                    var nugetFeed = (NuGetFeedResource)Resource;
                    outputResource = Connection.Repository.Feeds.Modify(nugetFeed);
                    break;

                case "Octopus.Client.Model.VariableSetResource":
                    var variableSet = (VariableSetResource)Resource;
                    outputResource = Connection.Repository.VariableSets.Modify(variableSet);
                    break;

                case "Octopus.Client.Model.MachineResource":
                    var machine = (MachineResource)Resource;
                    outputResource = Connection.Repository.Machines.Modify(machine);
                    break;

                case "Octopus.Client.Model.LifecycleResource":
                    var lifecycle = (LifecycleResource)Resource;
                    outputResource = Connection.Repository.Lifecycles.Modify(lifecycle);
                    break;

                case "Octopus.Client.Model.TeamResource":
                    var team = (TeamResource)Resource;
                    outputResource = Connection.Repository.Teams.Modify(team);
                    break;

                case "Octopus.Client.Model.UserResource":
                    var user = (UserResource)Resource;
                    outputResource = Connection.Repository.Users.Modify(user);
                    break;

                case "Octopus.Client.Model.ChannelResource":
                    var channel = (ChannelResource)Resource;
                    outputResource = Connection.Repository.Channels.Modify(channel);
                    break;

                case "Octopus.Client.Model.TenantResource":
                    var tenant = (TenantResource)Resource;
                    outputResource = Connection.Repository.Tenants.Modify(tenant);
                    break;

                case "Octopus.Client.Model.TagSetResource":
                    var tagset = (TagSetResource)Resource;
                    outputResource = Connection.Repository.TagSets.Modify(tagset);
                    break;

                case "Octopus.Client.Model.ReleaseResource":
                    var release = (ReleaseResource)Resource;
                    outputResource = Connection.Repository.Releases.Modify(release);
                    break;

                case "Octopus.Client.Model.UserRoleResource":
                    var userRole = (UserRoleResource)Resource;
                    outputResource = Connection.Repository.UserRoles.Modify(userRole);
                    break;

                default:
                    Console.WriteLine("Update-OctopusResource doesn't support updating objects of type: {0}", Resource.GetType().ToString());
                    break;
            }
            
            WriteObject(outputResource, true);
        }
    }
}