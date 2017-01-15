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
    /// <para type="synopsis">This cmdlet returns info about Octopus Targets (Tentacles, cloud regions, Offline deployment targets, SHH)</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -name "Database_Prod"</code>
    ///   <para>Gets the machine with the name "Database_Prod"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -name "*_Prod"</code>
    ///   <para>Gets all the machines which name is like "*_Prod"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -EnvironmentName "Staging","UAT""</code>
    ///   <para>Gets all the machines on the environments "Staging","UAT"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -URL "*:10933"</code>
    ///   <para>Gets all the machines with the string "*:10933" at the end of the URL</para>    
    /// </example>
    /// <example>   
    ///   <code>PS Get-OctopusMachine -Mode Listening</code>
    ///   <para>Gets all the machines registered in "Listening" mode. "Polling" is also a valid value</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    
    [Cmdlet("New", "OctopusResource")]
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
    public class NewOctopusResource : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Name of the Machine to filter by</para>
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
                    var enviroment = (EnvironmentResource) Resource;
                    outputResource = _connection.Repository.Environments.Create(enviroment);
                    break;

                case "Octopus.Client.Model.ProjectResource":
                    var project = (ProjectResource)Resource;
                    outputResource = _connection.Repository.Projects.Create(project);
                    break;

                case "Octopus.Client.Model.ProjectGroupResource":
                    var projectGroup = (ProjectGroupResource)Resource;
                    outputResource = _connection.Repository.ProjectGroups.Create(projectGroup);
                    break;

                case "Octopus.Client.Model.NuGetFeedResource":
                    var nugetFeed = (NuGetFeedResource)Resource;
                    outputResource = _connection.Repository.Feeds.Create(nugetFeed);
                    break;

                case "Octopus.Client.Model.LibraryVariableSetResource":
                    var libraryVariableSet = (LibraryVariableSetResource)Resource;
                    outputResource = _connection.Repository.LibraryVariableSets.Create(libraryVariableSet);
                    break;

                case "Octopus.Client.Model.MachineResource":
                    var machine = (MachineResource)Resource;
                    outputResource = _connection.Repository.Machines.Create(machine);
                    break;

                case "Octopus.Client.Model.LifecycleResource":
                    var lifecycle = (LifecycleResource)Resource;
                    outputResource = _connection.Repository.Lifecycles.Create(lifecycle);
                    break;

                case "Octopus.Client.Model.TeamResource":
                    var team = (TeamResource)Resource;
                    outputResource = _connection.Repository.Teams.Create(team);
                    break;

                case "Octopus.Client.Model.UserResource":
                    var user = (UserResource)Resource;
                    outputResource = _connection.Repository.Users.Register(new RegisterCommand()
                    {
                        DisplayName = user.DisplayName,
                        Username = user.Username,
                        Password = user.Password,
                        EmailAddress = user.EmailAddress,
                        IsActive = user.IsActive,
                        IsService = user.IsService,
                        InvitationCode = null
                    });
                    break;

                case "Octopus.Client.Model.ChannelResource":
                    var channel = (ChannelResource)Resource;
                    outputResource = _connection.Repository.Channels.Create(channel);
                    break;

                case "Octopus.Client.Model.TenantResource":
                    var tenant = (TenantResource)Resource;
                    outputResource = _connection.Repository.Tenants.Create(tenant);
                    break;

                default:
                    Console.WriteLine("Dunno what to create");
                    break;
            }

            WriteObject(outputResource);
        }
    }
}