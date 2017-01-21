using System;
using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets information about Octopus Releases</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusRelease -ProjectName "MyProject"</code>
    ///   <para>Get all the realeases of the project "MyProject"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusRelease -ProjectName "MyProject" -version 1.0.1,1.0.2</code>
    ///   <para>Get the release realeases 1.0.1 &amp; 1.0.2 of the project "MyProject"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusRelease -ProjectName "MyProject" -Latest 10</code>
    ///   <para>Get the latest 10 releases of the project "MyProject"</para>    
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

        /// <summary>
        /// <para type="description">Release version number</para>
        /// </summary>
        [Alias("Version","Release")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 2, ParameterSetName = ByVersion)]
        public string[] ReleaseVersion { get; set; }

        /// <summary>
        /// <para type="description">Name of project to filter releases. Only one Project can be passed to this parameter at a time</para>
        /// </summary>
        [Alias("Project")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, Mandatory = true)]
        public string ProjectName { get; set; }

        /// <summary>
        /// <para type="description">Get latest X releases</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [ValidateRange(1,int.MaxValue)]
        [Parameter(ParameterSetName = ByLatest)]
        public int Latest { get; set; }

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
                            try
                            {
                                baseResourceList.Add(_connection.Repository.Projects.GetReleaseByVersion(project, version));
                            }
                            catch (Exception e)
                            {
                                WriteError(new ErrorRecord(e,"ResourceNotFound", ErrorCategory.ObjectNotFound, e.Message));
                            }
                        }
                        break;

                    case ByLatest:

                        var releases = new List<ReleaseResource>();

                        if (Latest > 30)
                        {
                            releases = _connection.Repository.Projects.GetAllReleases(project).ToList();
                        }
                        else
                        {
                            releases = _connection.Repository.Projects.GetReleases(project).Items.ToList();
                        }

                        if(releases.Count > Latest)
                        {
                            baseResourceList.AddRange(releases.GetRange(0, Latest));
                        }
                        else
                        {
                            baseResourceList.AddRange(releases);
                        }

                        break;

                    default:
                        baseResourceList.AddRange(_connection.Repository.Projects.GetAllReleases(project).ToList());
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