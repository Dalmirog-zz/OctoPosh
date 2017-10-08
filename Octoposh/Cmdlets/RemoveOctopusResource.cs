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
    /// <para type="synopsis">Deletes resources from an Octopus Instance</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Deletes resources from an Octopus Instance</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> $ProjectResource = Get-OctopusProject -name "MyApp" ; Remove-OctopusResource -resource $ProjectResource</code>
    ///   <para>Deletes the project called "MyApp" from the Octopus Instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusProjectGroup -name "MyProjects" | select -ExpandProperty Projects | Remove-OctopusResource </code>
    ///   <para>Deletes all the projects inside the Project Group "MyProjects"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>

    [Cmdlet("Remove", "OctopusResource")]
    [OutputType(typeof(bool))]
    public class RemoveOctopusResource : OctoposhConnection
    {
        /// <summary>
        /// <para type="description">Resource Object to delete from the Octopus Server</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public Resource[] Resource { get; set; }
        
        //todo Add -Force parameter

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
                            Connection.Repository.Environments.Delete(enviroment);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.ProjectResource":
                            var project = (ProjectResource)r;
                            Connection.Repository.Projects.Delete(project);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.ProjectGroupResource":
                            var projectGroup = (ProjectGroupResource)r;
                            Connection.Repository.ProjectGroups.Delete(projectGroup);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.NuGetFeedResource":
                            var nugetFeed = (NuGetFeedResource)r;
                            Connection.Repository.Feeds.Delete(nugetFeed);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.LibraryVariableSetResource":
                            var libraryVariableSet = (LibraryVariableSetResource)r;
                            Connection.Repository.LibraryVariableSets.Delete(libraryVariableSet);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.MachineResource":
                            var machine = (MachineResource)r;
                            Connection.Repository.Machines.Delete(machine);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.LifecycleResource":
                            var lifecycle = (LifecycleResource)r;
                            Connection.Repository.Lifecycles.Delete(lifecycle);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.TeamResource":
                            var team = (TeamResource)r;
                            Connection.Repository.Teams.Delete(team);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.UserResource":
                            var user = (UserResource)r;
                            Connection.Repository.Users.Delete(user);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.ChannelResource":
                            var channel = (ChannelResource)r;
                            Connection.Repository.Channels.Delete(channel);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.TenantResource":
                            var tenant = (TenantResource)r;
                            Connection.Repository.Tenants.Delete(tenant);
                            WriteObject(true,true);
                            break;
                        case "Octopus.Client.Model.TagSetResource":
                            var tagset = (TagSetResource)r;
                            Connection.Repository.TagSets.Delete(tagset);
                            WriteObject(true,true);
                            break;

                        case "Octopus.Client.Model.UserRoleResource":
                            var userRole = (UserRoleResource)r;
                            Connection.Repository.UserRoles.Delete(userRole);
                            WriteObject(true, true);
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