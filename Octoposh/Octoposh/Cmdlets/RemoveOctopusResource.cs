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
    
    [Cmdlet("Remove", "OctopusResource")]
    [OutputType(typeof(bool))]
    public class RemoveOctopusResource : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Name of the Machine to filter by</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public Resource[] Resource { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            foreach (Resource r in Resource)
            {
                try
                {
                    switch (r.GetType().ToString())
                    {
                        case "Octopus.Client.Model.EnvironmentResource":
                            var enviroment = (EnvironmentResource)r;
                            _connection.Repository.Environments.Delete(enviroment);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.ProjectResource":
                            var project = (ProjectResource)r;
                            _connection.Repository.Projects.Delete(project);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.ProjectGroupResource":
                            var projectGroup = (ProjectGroupResource)r;
                            _connection.Repository.ProjectGroups.Delete(projectGroup);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.NuGetFeedResource":
                            var nugetFeed = (NuGetFeedResource)r;
                            _connection.Repository.Feeds.Delete(nugetFeed);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.LibraryVariableSetResource":
                            var libraryVariableSet = (LibraryVariableSetResource)r;
                            _connection.Repository.LibraryVariableSets.Delete(libraryVariableSet);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.MachineResource":
                            var machine = (MachineResource)r;
                            _connection.Repository.Machines.Delete(machine);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.LifecycleResource":
                            var lifecycle = (LifecycleResource)r;
                            _connection.Repository.Lifecycles.Delete(lifecycle);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.TeamResource":
                            var team = (TeamResource)r;
                            _connection.Repository.Teams.Delete(team);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.UserResource":
                            var user = (UserResource)r;
                            _connection.Repository.Users.Delete(user);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.ChannelResource":
                            var channel = (ChannelResource)r;
                            _connection.Repository.Channels.Delete(channel);
                            WriteObject(true);
                            break;

                        case "Octopus.Client.Model.TenantResource":
                            var tenant = (TenantResource)r;
                            _connection.Repository.Tenants.Delete(tenant);
                            WriteObject(true);
                            break;

                        default:
                            Console.WriteLine("Dunno what to delete");
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw;
                }

            }
        }
    }
}