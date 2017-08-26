using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet creates instances of Octopus Resource Objects</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> $EnvironmentObj = Get-OctopusResourceModel -Resource Environment</code>
    ///   <para>Creates an Environment Resource object</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusResourceModel")]
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
    public class GetOctopusResourceModel : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Resource object model</para>
        /// </summary>
        [ValidateSet("Environment", "Project", "ProjectGroup", "NugetFeed","ExternalFeed", "LibraryVariableSet", "Machine","Target", "Lifecycle", "Team", "User","Channel","Tenant","TagSet","UserRole")]
        [Parameter(Mandatory = true)]
        public string Resource { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            object baseresource = null;

            switch (Resource)
            {
                case "Environment":
                    baseresource = new EnvironmentResource();
                    break;
                case "Project":
                    baseresource =new ProjectResource();
                    break;
                case "ProjectGroup":
                    baseresource = new ProjectGroupResource();
                    break;
                case "NugetFeed":
                case "ExternalFeed":
                    baseresource = new NuGetFeedResource();
                    break;
                case "LibraryVariableSet":
                    baseresource = new LibraryVariableSetResource();
                    break;
                case "Machine":
                case "Target":
                    baseresource = new MachineResource();
                    break;
                case "Lifecycle":
                    baseresource = new LifecycleResource();
                    break;
                case "Team":
                    baseresource = new TeamResource();
                    break;
                case "User":
                    baseresource = new UserResource();
                    break;
                case "Channel":
                    baseresource = new ChannelResource();
                    break;
                case "Tenant":
                    baseresource = new TenantResource();
                    break;
                case "TagSet":
                    baseresource = new TagSetResource();
                    break;
                case "UserRole":
                    baseresource = new UserRoleResource();
                    break;
            }

            WriteObject(baseresource,true);
        }
    }
}