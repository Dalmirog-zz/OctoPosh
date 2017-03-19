using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
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
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
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
    public class UpdateOctopusResource : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Resource Object</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public Resource Resource { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            Resource outputResource = null;

            switch (Resource.GetType().ToString())
            {
                case "Octopus.Client.Model.EnvironmentResource":
                    var enviroment = (EnvironmentResource)Resource;
                    outputResource = _connection.Repository.Environments.Modify(enviroment);
                    break;

                case "Octopus.Client.Model.ProjectResource":
                    var project = (ProjectResource)Resource;
                    outputResource = _connection.Repository.Projects.Modify(project);
                    break;

                case "Octopus.Client.Model.ProjectGroupResource":
                    var projectGroup = (ProjectGroupResource)Resource;
                    outputResource = _connection.Repository.ProjectGroups.Modify(projectGroup);
                    break;

                case "Octopus.Client.Model.NuGetFeedResource":
                    var nugetFeed = (NuGetFeedResource)Resource;
                    outputResource = _connection.Repository.Feeds.Modify(nugetFeed);
                    break;

                case "Octopus.Client.Model.LibraryVariableSetResource":
                    var libraryVariableSet = (LibraryVariableSetResource)Resource;
                    outputResource = _connection.Repository.LibraryVariableSets.Modify(libraryVariableSet);
                    break;

                case "Octopus.Client.Model.MachineResource":
                    var machine = (MachineResource)Resource;
                    outputResource = _connection.Repository.Machines.Modify(machine);
                    break;

                case "Octopus.Client.Model.LifecycleResource":
                    var lifecycle = (LifecycleResource)Resource;
                    outputResource = _connection.Repository.Lifecycles.Modify(lifecycle);
                    break;

                case "Octopus.Client.Model.TeamResource":
                    var team = (TeamResource)Resource;
                    outputResource = _connection.Repository.Teams.Modify(team);
                    break;

                case "Octopus.Client.Model.UserResource":
                    var user = (UserResource)Resource;
                    outputResource = _connection.Repository.Users.Modify(user);
                    break;

                case "Octopus.Client.Model.ChannelResource":
                    var channel = (ChannelResource)Resource;
                    outputResource = _connection.Repository.Channels.Modify(channel);
                    break;

                case "Octopus.Client.Model.TenantResource":
                    var tenant = (TenantResource)Resource;
                    outputResource = _connection.Repository.Tenants.Modify(tenant);
                    break;

                case "Octopus.Client.Model.TagSetResource":
                    var tagset = (TagSetResource)Resource;
                    outputResource = _connection.Repository.TagSets.Modify(tagset);
                    break;

                default:
                    Console.WriteLine("Dunno what to modify");
                    break;
            }

            WriteObject(outputResource);
        }
    }
}