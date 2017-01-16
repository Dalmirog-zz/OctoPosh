using Octoposh.Model;
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
    [Cmdlet("Get", "OctopusRelease", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusRelease>))]
    [OutputType(typeof(List<ReleaseResource>))]
    public class GetOctopusRelease : PSCmdlet
    {
        private const string ByVersion = "ByVersion";
        private const string ByLatest = "ByLatest";
        private const string All = "All";


        [Alias("Version","Release")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 2, ParameterSetName = ByVersion)]
        public List<string> ReleaseVersion { get; set; }


        [Alias("Project")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, Mandatory = true)]
        public string ProjectName { get; set; }

        [ValidateNotNullOrEmpty()]
        [ValidateRange(1,30)]
        [Parameter(ParameterSetName = ByLatest)]
        public int Latest { get; set; }

        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<ReleaseResource>();

            var project = _connection.Repository.Projects.FindByName(ProjectName);

            if (project == null)
            {
                throw OctoposhExceptions.ResourceNotFound(ProjectName, "Project");
            }
            else
            {
                switch (ParameterSetName)
                {
                    case ByVersion:
                        foreach (var version in ReleaseVersion)
                        {
                            baseResourceList.Add(_connection.Repository.Projects.GetReleaseByVersion(project,version));
                        }
                        break;
                    case ByLatest:
                        
                        var releases = _connection.Repository.Projects.GetReleases(project).Items.ToList();

                        baseResourceList.AddRange(releases.GetRange(0,Latest));

                        break;
                    default:
                        baseResourceList.AddRange(_connection.Repository.Projects.GetReleases(project).Items);
                        break;
                }
            }

            if (ResourceOnly)
            {
                WriteObject(baseResourceList);
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusRelease(baseResourceList);

                WriteObject(outputList);
            }

        }
    }
}